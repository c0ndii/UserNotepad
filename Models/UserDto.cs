using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserDto
    {
        required public Guid ID { get; set; } = Guid.NewGuid();
        required public string Name { get; set; }
        required public string Surname { get; set; }
        required public DateTime BirthDate { get; set; }
        required public SexEnum Sex { get; set; }
        public IEnumerable<UserAttribute>? Attributes { get; set; }
    }
}
