using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserInput
    {
        required public string Name { get; set; }
        required public string Surname { get; set; }
        required public DateTime BirthDate { get; set; }
        required public SexEnum Sex { get; set; }
        public IEnumerable<UserAttributeInput>? Attributes { get; set; }
    }
}
