using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonApiTMDB.Data;
using MonApiTMDB.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION BASE DE DONNÉES (MySQL) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Chaîne de connexion introuvable.");

// On utilise UseMySql avec l'auto-détection de version
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- 2. SERVICES ---
builder.Services.AddHttpClient<ITmdbService, TmdbService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 3. AUTHENTIFICATION JWT ---
var jwtSection = builder.Configuration.GetSection("JwtSettings");
// Clé de secours si non trouvée dans appsettings (pour le dev)
var secretKey = jwtSection["SecretKey"] ?? "MA_CLE_SECRETE_TRES_LONGUE_123456789"; 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false, 
        ValidateAudience = false,
        ValidateLifetime = true
    };
    
    // Lire le token depuis le Cookie (Optionnel, pratique pour le Web)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                context.Token = context.Request.Cookies["AuthToken"];
            }
            return Task.CompletedTask;
        }
    };
});

// --- 4. CORS (Pour Angular/React) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// --- PIPELINE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();