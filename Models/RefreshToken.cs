public class RefreshToken
{
    public int Id { get; set;}
    public string Token { get; set;}
    

    public DateTime ExpriryDate { get;set;}


    public bool isRevoked { get; set;}

    public int UserId { get; set;}

    public User User { get; set;}

    
}