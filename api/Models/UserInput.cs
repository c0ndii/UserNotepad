using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserInput
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateTime BirthDate { get; set; }
        public required SexEnum Sex { get; set; }
        public IEnumerable<UserAttributeInput> Attributes { get; set; } = new List<UserAttributeInput>();
    }
}
