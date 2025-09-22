using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CoreWebApi.Data;
using CoreWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(builder.Configuration["Kestrel:Endpoints:Http:Url"] ?? "http://127.0.0.1:5210");

// DbContext (Sqlite by default)
var useMySql = true;
if (useMySql)
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseMySql(builder.Configuration.GetConnectionString("MySql"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
}

builder.Services.AddControllers();

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", p =>
    {
        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? new[] { "http://localhost:5173" };
        p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IM Admin API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Description = "JWT Bearer。示例：Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, new List<string>() } });
});

// services
builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ensure db & seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db);
}

app.Run();