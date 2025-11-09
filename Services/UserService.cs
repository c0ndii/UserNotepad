using Microsoft.EntityFrameworkCore;
using UserNotepad.Entities;
using UserNotepad.Models;

namespace UserNotepad.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDto>> GetAllUsers();
        public Task<UserDto?> GetUser(Guid id);
        public Task AddUser(UserInput user);
        public Task<bool> UpdateUser(Guid id, UserInput user);
        public Task<bool> RemoveUser(Guid id);
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
            return await _context.Users.Include(x => x.Attributes).Select(x => new UserDto
            {
                ID = x.ID,
                Name = x.Name,
                Surname = x.Surname,
                BirthDate = x.BirthDate,
                Sex = x.Sex,
                Attributes = x.Attributes.Select(y => new UserAttributeDto
                {
                    Key = y.Key,
                    Value = y.Value,
                    ValueType = y.ValueType
                }),
            }).ToListAsync();
        }

        public async Task<UserDto?> GetUser(Guid id)
        {
            return await _context.Users.Include(x => x.Attributes)
                .Where(x => x.ID == id)
                .Select(x => new UserDto
                {
                    ID = x.ID,
                    Name = x.Name,
                    Surname = x.Surname,
                    BirthDate = x.BirthDate,
                    Sex = x.Sex,
                    Attributes = x.Attributes.Select(y => new UserAttributeDto
                    {
                        Key = y.Key,
                        Value = y.Value,
                        ValueType = y.ValueType
                    }),
                }).SingleOrDefaultAsync();
        }

        public async Task AddUser(UserInput user)
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
        }

        public async Task<bool> UpdateUser(Guid id, UserInput user)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(x => x.ID == id);
            if (dbUser is null)
                return false;

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
            return true;
        }

        public async Task<bool> RemoveUser(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.ID == id);

            if (user is null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private ICollection<UserAttribute> MapAttributesFromInput(IEnumerable<UserAttributeInput> attributes)
        {
            return attributes
                .Where(x => x is not null)
                .Select(x => new UserAttribute
                {
                    Key = x.Key,
                    Value = x.Value,
                    ValueType = x.ValueType,
                }).ToList();
        }
    }
}
