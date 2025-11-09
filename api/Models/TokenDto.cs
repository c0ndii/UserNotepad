namespace UserNotepad.Models
{
    public class TokenDto
    {
        public required string JwtToken { get; set; }
        public required DateTime Exiration {  get; set; }
    }
}
