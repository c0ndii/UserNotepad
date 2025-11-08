namespace UserNotepad.Entities
{
    public class UserAttribute
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        required public string Key { get; set; }
        required public string Value { get; set; }
        required public AttributeTypeEnum ValueType { get; set; }
        public Guid UserID { get; set; }
        public User User { get; set; }
    }
}
