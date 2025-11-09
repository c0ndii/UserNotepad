using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserDto
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateTime BirthDate { get; set; }
        public required SexEnum Sex { get; set; }
        public IEnumerable<UserAttributeDto>? Attributes { get; set; }
    }
}
