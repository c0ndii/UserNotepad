using Microsoft.EntityFrameworkCore;
using UserNotepad.Entities;
using UserNotepad.Models;

namespace UserNotepad.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDto>> GetAllUsers();
        public Task<UserDto> GetUser(Guid id);
        public Task AddUser(UserInput user);
        public Task UpdateUser(Guid id, UserInput user);
        public Task RemoveUser(Guid id);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        private IEnumerable<UserAttribute>? MapAttributesFromInput(IEnumerable<UserAttributeInput>? attributes)
        {
            if (attributes is not null)
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

            return null;
        }

        public UserService(AppDbContext context)
        {
            this._context = context;
        }

        public async Task AddUser(UserInput user)
        {
            var userToDb = new User
            {
                Name = user.Name,
                Surname = user.Surname,
                BirthDate = user.BirthDate,
                Sex = user.Sex,
                Attributes = MapAttributesFromInput(user.Attributes)
            };

            await _context.Users.AddAsync(userToDb);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<UserDto>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUser(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.ID == id);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public Task UpdateUser(Guid id, UserInput user)
        {
            throw new NotImplementedException();
        }
    }
}
