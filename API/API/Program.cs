using Microsoft.EntityFrameworkCore;
using API.Data;  // You can comment this out if the correct DbContext is in the Models folder
using API.Models;  // This is the correct DbContext

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Commented out to avoid using the wrong DbContext
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Correct DbContext from Models
builder.Services.AddDbContext<StudentManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS configuration here
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // Allow requests from any origin
              .AllowAnyMethod()      // Allow any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();     // Allow any headers
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS in the request pipeline
app.UseCors("AllowAll");  // This enables the CORS policy we defined

// Comment this in if routing is required for controllers to map properly
app.UseRouting();  // Ensure routing middleware is enabled

// Ensure endpoints are mapped correctly for controllers
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  // Ensures the API controllers are used
});

app.Run();
