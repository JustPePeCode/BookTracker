using BookTracker.Api.Application;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

builder.Services.AddScoped<BookService>();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapGet("/books", async (BookService service) => Results.Ok(await service.GetAllBooks()));

app.MapPost(
    "/books",
    async (CreateBookRequest request, BookService service) =>
    {
        var response = await service.CreateBook(request);
        return Results.Created($"/book/{response.Id}", response);
    }
);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program;
