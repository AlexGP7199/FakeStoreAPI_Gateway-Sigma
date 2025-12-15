using FakeStore.Gateway.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                     ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    // Política permisiva para desarrollo
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    // Política restrictiva para producción (lee desde appsettings.json)
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configurar FakeStore Gateway
builder.Services.AddFakeStoreGateway(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FakeStore Gateway API v1");
        options.DocumentTitle = "FakeStore Gateway API";
    });

    // Redireccionar la raíz a Swagger
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    
    // Usar política permisiva en desarrollo
    app.UseCors("AllowAll");
}
else
{
    // Usar política restrictiva en producción
    app.UseCors("AllowSpecificOrigins");
}

// NO forzar HTTPS redirection en contenedores
// app.UseHttpsRedirection();  // ? Comentado

app.UseAuthorization();

app.MapControllers();

app.Run();
