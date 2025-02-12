using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class JwtService 
{
    private readonly IConfiguration _configuration;
    private readonly TodoContext _context;

    public JwtService(IConfiguration configuration, TodoContext context)
    {
        _configuration = configuration;
        _context = context;

    }


    public string GenerateAccessToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var credentials =  new SigningCredentials(key,SecurityAlgorithms.HmacSha256);


        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: credentials
        );


        return new JwtSecurityTokenHandler().WriteToken(token);

    }



    public async Task<RefreshToken> GenerateRefreshToken(User user){


        var randomNumber  = new byte[32];
        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        // revoke any existing refresh tokens for this user


        var existingTokens = _context.RefreshTokens.Where(
            rt => rt.UserId == user.Id
        );

        foreach ( var token in existingTokens)
        {
            token.isRevoked = true;
        }


        var refreshToken = new RefreshToken {
            Token = Convert.ToBase64String(randomNumber),
            ExpriryDate = DateTime.UtcNow.AddDays(1),
            UserId = user.Id,
            isRevoked= false
        };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync();

        return refreshToken;
    }


    public async Task<(string AccessToken, RefreshToken refreshToken)>  RefreshAccessToken(string refreshToken){
        // find valid tokens, not revoked
        var storedToken = _context.RefreshTokens.Include(
            rt => rt.User
        ).FirstOrDefault(rt => rt.Token == refreshToken && !rt.isRevoked);


        if (storedToken ==null || storedToken.ExpriryDate < DateTime.UtcNow)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }


        // generate new tokens


        var newAccessToken = GenerateAccessToken(storedToken.User);

        var newRefreshToken  = await GenerateRefreshToken(storedToken.User);

        return ( newAccessToken, newRefreshToken);
    }


    public async Task RevokeToken(string refreshToken)
    {
        var storedToken = _context.RefreshTokens.FirstOrDefault(rt => rt.Token ==refreshToken);
        if (storedToken !=null)
        {
            storedToken.isRevoked =true;
            await _context.SaveChangesAsync();
        }
    }


}