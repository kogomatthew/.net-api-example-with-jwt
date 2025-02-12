public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Username { get; set;}

    public DateTime ExpiryDate { get; set;}

    
}