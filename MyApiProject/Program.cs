var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // Enable Swagger annotations
});
       builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowVueApp", builder =>
            {
                builder.WithOrigins("http://localhost:8080") // Vue.js frontend URL
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
builder.Services.AddControllers(); // Register controllers
// Enable CORS
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Show Swagger UI in development
}
app.UseCors("AllowVueApp");

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.MapControllers(); // Map controller routes

app.Run();
