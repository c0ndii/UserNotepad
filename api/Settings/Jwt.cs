namespace UserNotepad.Settings
{
    public class Jwt
    {
        public required string Key { get; set; }
        public required int ExpireMinutes { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
    }
}
