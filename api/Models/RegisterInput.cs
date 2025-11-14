namespace UserNotepad.Models
{
    public class RegisterInput
    {
        public string Nickname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RepeatPassword {  get; set; } = string.Empty;
    }
}
