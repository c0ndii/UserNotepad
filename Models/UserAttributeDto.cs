using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserAttributeDto
    {
        required public string Key { get; set; }
        required public string Value { get; set; }
        required public AttributeTypeEnum ValueType { get; set; }
    }
}
