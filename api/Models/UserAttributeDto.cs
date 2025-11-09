using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserAttributeDto
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
        public required AttributeTypeEnum ValueType { get; set; }
    }
}
