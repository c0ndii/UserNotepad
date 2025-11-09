using Microsoft.EntityFrameworkCore;
using UserNotepad.Entities;
using UserNotepad.Models;

namespace UserNotepad.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDto>> GetAllUsers();
        public Task<UserDto?> GetUser(Guid id);
        public Task<UserDto> AddUser(UserInput user);
        public Task<UserDto?> UpdateUser(Guid id, UserInput user);
        public Task<Guid?> RemoveUser(Guid id);
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

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            return await _context.Users.Include(x => x.Attributes).Select(x => MapUserFromDb(x)).ToListAsync();
        }

        public async Task<UserDto?> GetUser(Guid id)
        {
            var user = await _context.Users.Include(x => x.Attributes)
                .SingleOrDefaultAsync(x => x.ID == id);

            if (user is null)
                return null;
            return MapUserFromDb(user);
        }

        public async Task<UserDto> AddUser(UserInput user)
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

            await _context.Users.AddAsync(userToDb);
            await _context.SaveChangesAsync();

            return MapUserFromDb(userToDb);
        }

        public async Task<UserDto?> UpdateUser(Guid id, UserInput user)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(x => x.ID == id);
            if (dbUser is null)
                return null;

            dbUser.Name = user.Name;
            dbUser.Surname = user.Surname;
            dbUser.BirthDate = user.BirthDate;
            dbUser.Sex = user.Sex;

            var attributes = await _context.Attributes.Where(x => x.UserID == id).ToListAsync();

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
                    });
                }
            }

            if (toRemove.Count > 0)
            {
                _context.Attributes.RemoveRange(toRemove);
            }

            if (toAdd.Count > 0)
                await _context.Attributes.AddRangeAsync(toAdd);

            await _context.SaveChangesAsync();
            return MapUserFromDb(dbUser);
        }

        public async Task<Guid?> RemoveUser(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.ID == id);

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
