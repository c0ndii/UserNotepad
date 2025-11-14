using UserNotepad.Entities;

namespace UserNotepad.Models
{
    public class UserAttributeInput
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public AttributeTypeEnum? ValueType { get; set; } = null;
    }
}
