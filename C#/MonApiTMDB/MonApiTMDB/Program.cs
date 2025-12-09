// ... début du fichier Program.cs

using MonApiTMDB.Services;

var builder = WebApplication.CreateBuilder(args);

// --- NOUVELLES LIGNES : Ajouter les services pour Swagger/OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// -------------------------------------------------------------------

// 1. Enregistre le TmdbService et configure HttpClient
builder.Services.AddHttpClient<ITmdbService, TmdbService>();

// 2. Ajout des Cors (pour qu'Angular puisse appeler le back-end)
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; 
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // IMPORTANT : Remplacez par l'URL de votre application Angular en production.
            // Pour le développement, on autorise 'localhost:4200' qui est la valeur par défaut d'Angular.
            policy.WithOrigins("http://localhost:4200") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddControllers(); 

// ... (autres services)

var app = builder.Build();

// --- NOUVELLES LIGNES : Utiliser les middlewares Swagger/SwaggerUI ---

// Configuration: Swagger n'est utilisé que dans l'environnement de développement par défaut.
if (app.Environment.IsDevelopment())
{
    // 4. Active le middleware pour servir le document JSON OpenAPI
    app.UseSwagger();
    
    // 5. Active le middleware pour servir l'interface utilisateur de Swagger
    app.UseSwaggerUI(); 
}
// -------------------------------------------------------------------

// ... (autres middlewares)

// 3. Utilise Cors
app.UseCors(MyAllowSpecificOrigins);

// Utilise le routing
app.UseRouting();

// Utilise l'autorisation (si nécessaire)
// app.UseAuthorization();

// Mappe les contrôleurs (essentiel pour que MoviesController fonctionne)
app.MapControllers(); 

app.Run();

// ... (fin du fichier Program.cs)