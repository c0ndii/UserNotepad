using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserInput
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; } = null;
        public SexEnum? Sex { get; set; } = null;
        public IEnumerable<UserAttributeInput> Attributes { get; set; } = new List<UserAttributeInput>();
    }
}
