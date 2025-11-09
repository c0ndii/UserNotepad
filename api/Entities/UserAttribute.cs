namespace UserNotepad.Entities
{
    public class UserAttribute
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public required string Key { get; set; }
        public required string Value { get; set; }
        public required AttributeTypeEnum ValueType { get; set; }
        public Guid UserID { get; set; }
        public User User { get; set; }
    }
}
