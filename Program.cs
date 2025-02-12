using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder  = WebApplication.CreateBuilder(args); 

// add services to the container

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(

    c => {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });


        c.AddSecurityRequirement (new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
);

// JWT CONFIG

// Add JWT configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")))
        };
    });

// Register JwtService
builder.Services.AddScoped<JwtService>();

// add mysql 

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapControllers();

app.Run();