
using System.Text;
using AniLens.Core.Interfaces;
using AniLens.Core.Services;
using AniLens.Core.Settings;
using AniLens.Server.Settings;
using AniLens.Server.Validators;
using AniLens.Shared.DTO;
using FluentValidation;
using MangaDexSharp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["JwtSettings:Issuer"];
        var audience = builder.Configuration["JwtSettings:Audience"];
        var secretKey = builder.Configuration["JwtSettings:SecretKey"];

        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentException("JWT Issuer isn't set");
        
        if (string.IsNullOrEmpty(audience))
            throw new ArgumentException("JWT audience isn't set");
        
        if (string.IsNullOrEmpty(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 32)
            throw new ArgumentException("JWT secret key must be at least 32 bytes");        
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["jwt"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AuthPolicy", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:*")
            // .AllowCredentials()
            .WithHeaders("Authorization", "Content-Type")
            .WithMethods("POST", "PUT", "UPDATE", "DELETE", "GET");
    });
});



builder.Services.Configure<UserDbSettings>(builder.Configuration.GetSection("MongoDBUser"));
builder.Services.Configure<MangaListDbSettings>(
    builder.Configuration.GetSection("MongoDBMangaList"));
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IHashService, HashService>();
builder.Services.AddMangaDex();
builder.Services.AddScoped<IMangaInfoService, MangaInfoService>();
builder.Services.AddScoped<IMangaListService, MangaListService>();
// builder.Services.AddScoped<IMangaListService, MangaListService>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegistrationValidator>();
builder.Services.AddScoped<IValidator<UpdateUserDto>, UpdateUserValidator>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseHttpsRedirection();
app.UseCors("AuthPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
