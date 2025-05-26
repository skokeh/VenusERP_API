namespace VenusERP_API.Models
{
    public class UserResponse
    {
        public string Token { get; set; }
        public string? Access_token { get; set; }
        public string? Token_type { get; set; }
        public string? Expiry { get; set; }
        public string UserName { get; set; }
    }
}
