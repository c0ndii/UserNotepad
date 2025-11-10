namespace UserNotepad.Entities
{
    public class Operator
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Nickname { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }
}
