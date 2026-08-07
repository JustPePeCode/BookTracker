using BookTracker.Api.Wiring;



var builder = WebApplication.CreateBuilder(args);
var frontendOrigin = builder.Configuration["FrontendOrigin"]
    ?? "http://localhost:5173";
    
builder.AddBookTracker();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
         policy
            .WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseBookTracker();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program;
