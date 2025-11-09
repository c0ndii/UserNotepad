namespace UserNotepad.Models
{
    public class RegisterInput
    {
        public required string Nickname { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string RepeatPassword {  get; set; }
    }
}
