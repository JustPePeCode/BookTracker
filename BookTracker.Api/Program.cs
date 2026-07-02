using BookTracker.Api.Application;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker"));
});

builder.Services.AddScoped<IBookRepository, EfBookRepository>();

builder.Services.AddScoped<BookService>();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}
app.MapBookEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program;
