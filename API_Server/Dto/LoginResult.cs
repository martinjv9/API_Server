namespace API_Server.Dto
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public required string Message {  get; set; }
        public required string Token { get; set; }
        public required string Role { get; set; }
    }
}
