namespace UserNotepad.Models
{
    public class LoginDto
    {
        public required string UserNickname { get; set; }
        public required string JwtToken { get; set; }
        public required DateTime JwtExpiration {  get; set; }
    }
}
