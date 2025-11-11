using Microsoft.EntityFrameworkCore;
using UserNotepad.Entities;
using UserNotepad.Models;

namespace UserNotepad.Services
{
    public interface IUserService
    {
        public Task<PageDto<UserDto>> GetAllUsers(PageInput pageRequest, CancellationToken cancellationToken);
        public Task<UserDto?> GetUser(Guid id, CancellationToken cancellationToken);
        public Task<UserDto> AddUser(UserInput user, CancellationToken cancellationToken);
        public Task<UserDto?> UpdateUser(Guid id, UserInput user, CancellationToken cancellationToken);
        public Task<Guid?> RemoveUser(Guid id, CancellationToken cancellationToken);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IUserService> _logger;

        public UserService(AppDbContext context, ILogger<IUserService> logger)
        {
            this._context = context;
            this._logger = logger;

        }

        public async Task<PageDto<UserDto>> GetAllUsers(PageInput pageRequest, CancellationToken cancellationToken)
        {
            var query = _context.Users.Include(x => x.Attributes).AsQueryable();

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(x => x.CreatedAt)
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(x => MapUserFromDb(x))
                .ToListAsync(cancellationToken);

            return new PageDto<UserDto>
            {
                Items = users,
                TotalCount = totalCount,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize
            };
        }

        public async Task<UserDto?> GetUser(Guid id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(x => x.Attributes)
                .SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (user is null)
                return null;
            return MapUserFromDb(user);
        }

        public async Task<UserDto> AddUser(UserInput user, CancellationToken cancellationToken)
        {
            var userToDb = new User
            {
                Name = user.Name,
                Surname = user.Surname,
                BirthDate = user.BirthDate,
                Sex = user.Sex,
            };

            if (user.Attributes is not null)
                userToDb.Attributes = MapAttributesFromInput(user.Attributes);

            await _context.Users.AddAsync(userToDb, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return MapUserFromDb(userToDb);
        }

        public async Task<UserDto?> UpdateUser(Guid id, UserInput user, CancellationToken cancellationToken)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);
            if (dbUser is null)
                return null;

            dbUser.Name = user.Name;
            dbUser.Surname = user.Surname;
            dbUser.BirthDate = user.BirthDate;
            dbUser.Sex = user.Sex;

            var attributes = await _context.Attributes.Where(x => x.UserID == id).ToListAsync(cancellationToken);

            var toRemove = attributes.Where(x => !user.Attributes.Any(y => y.Key == x.Key)).ToList();
            var toAdd = new List<UserAttribute>();

            foreach (var attribute in user.Attributes)
            {
                var existingAttribute = attributes.SingleOrDefault(x => x.Key == attribute.Key);
                if (existingAttribute is not null)
                {
                    existingAttribute.Value = attribute.Value;
                    existingAttribute.ValueType = attribute.ValueType;
                }
                else
                {
                    toAdd.Add(new UserAttribute
                    {
                        Key = attribute.Key,
                        Value = attribute.Value,
                        ValueType = attribute.ValueType,
                        UserID = dbUser.ID,
                        User = dbUser
                    });
                }
            }

            if (toAdd.Count > 0)
                await _context.Attributes.AddRangeAsync(toAdd, cancellationToken);

            if (toRemove.Count > 0)
            {
                _context.Attributes.RemoveRange(toRemove);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return MapUserFromDb(dbUser);
        }

        public async Task<Guid?> RemoveUser(Guid id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (user is null) return null;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user.ID;
        }

        private IEnumerable<UserAttribute> MapAttributesFromInput(IEnumerable<UserAttributeInput> attributes)
        {
            return attributes
                .Select(x => new UserAttribute
                {
                    Key = x.Key,
                    Value = x.Value,
                    ValueType = x.ValueType,
                }).ToList();
        }

        private static UserDto MapUserFromDb(User user)
        {
            return new UserDto
            {
                ID = user.ID,
                Name = user.Name,
                Surname = user.Surname,
                BirthDate = user.BirthDate,
                Sex = user.Sex,
                Attributes = user.Attributes.Select(x => new UserAttributeDto
                {
                    Key = x.Key,
                    Value = x.Value,
                    ValueType = x.ValueType
                }).ToList()
            };
        }
    }
}
