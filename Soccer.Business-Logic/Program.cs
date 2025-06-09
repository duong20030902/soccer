/*using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Soccer.Business_Logic.Services;
using Soccer.Data_Access.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddDbContext<SoccerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ServerConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host) => true));
});
builder.Services.AddScoped<EmailService>();
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", policy =>
    {
        policy
            .WithOrigins("https://soccer.soccertips.org", "http://localhost:7170")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CORSPolicy");
app.MapControllers();

app.Run();
*/

using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Soccer.Business_Logic.Services;
using Soccer.Data_Access.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
// Thêm dịch vụ vào container.
builder.Services.AddDbContext<SoccerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ServerConnection")));
builder.Services.AddControllers();
// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Soccer API", Version = "v1" });
    // Thêm cấu hình bảo mật cho JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field (e.g., Bearer <token>)",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", policy =>
    {
        policy
            .WithOrigins("https://www.soccer.soccertips.org", "https://localhost:7170")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<EmailService>();
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24); // Thời gian sống của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // Điều chỉnh SameSite và SecurePolicy dựa trên việc có HTTPS hay không
    var isHttps = builder.Environment.IsProduction() || builder.Configuration["Kestrel:Endpoints:Https"] != null;
    if (isHttps)
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Yêu cầu HTTPS
    }
    else
    {
        options.Cookie.SameSite = SameSiteMode.Lax; // Mặc định Lax nếu không có HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Cho phép HTTP nếu cần
    }
});

// Thêm chính sách ủy quyền tùy chỉnh
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowCookieAuth", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var httpContext = context.Resource as HttpContext;
            return httpContext != null && (
                httpContext.Request.Cookies.ContainsKey("UserId") ||
                httpContext.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null
            );
        });
    });
});

var app = builder.Build();

// Cấu hình pipeline HTTP
app.UseHsts(); // Bật HTTP Strict Transport Security
app.UseHttpsRedirection(); // Chuyển hướng tất cả yêu cầu sang HTTPS

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.UseCors("CORSPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

