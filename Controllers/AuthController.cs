using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public readonly TodoContext _context;
    private readonly JwtService _jwtService;


    public AuthController(TodoContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        await _jwtService.RevokeToken(request.Token);

        return Ok(new { message = "Successfully logged out" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
    {
        // in prod , verify password hash
        var user = _context.Users.FirstOrDefault(
            u => u.Username == request.Username
        );

        if (user == null)
            return Unauthorized(new
            {
                message = "Invalid username"
            });

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }
        var token = _jwtService.GenerateAccessToken(user);

        var refreshToken = await _jwtService.GenerateRefreshToken(user);

        return new AuthResponse
        {
            AccessToken = token,
            Username = user.Username,
            RefreshToken = refreshToken.Token,
            ExpiryDate = refreshToken.ExpriryDate

        };
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
    {
        try
        {
            var (accessToken, refreshToken) = await _jwtService.RefreshAccessToken(request.Token);
            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiryDate = refreshToken.ExpriryDate
            };
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(
                new
                {
                    message = "Invalid refresh token"
                }
            );
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(AuthRequest request)
    {

        var (isValid, errorMessage) = PasswordValidator.ValidatePassword(request.Password);

        if (!isValid)
        {
            return BadRequest(new { message = errorMessage });
        }

        if (_context.Users.Any(u => u.Username == request.Username))
            return BadRequest(new { message = "Username already exists" });

        // Hash the password with BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Password = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateAccessToken(user);
        var refreshToken = await _jwtService.GenerateRefreshToken(user);
        return new AuthResponse
        {
            AccessToken = token,
            Username = user.Username,
            RefreshToken = refreshToken.Token,
            ExpiryDate = refreshToken.ExpriryDate
        };
    }
}