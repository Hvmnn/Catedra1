using System.IO.Compression;
using ebooks_dotnet7_api;
using ebooks_dotnet7_api.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var ebooks = app.MapGroup("api/ebook");

// TODO: Add more routes
ebooks.MapPost("/", CreateEBookAsync);
ebooks.MapGet("/", GetEbooksAvailablesAsync);
ebooks.MapPut("/{id}", UpdateEbookAsync);
ebooks.MapPut("/{id}/change-availability", ChangeAvailableEbookAsync);
ebooks.MapPut("/{id}/increment-stock", IncrementStockEbookAsync);
ebooks.MapPost("/purchase", BuyEbookAsync);
ebooks.MapDelete("/{id}", DeleteEbookAsync);

app.Run();

// TODO: Add more methods
async Task<IResult> CreateEBookAsync(DataContext context, CreateEBookDto createEBook)
{
    var titleBook = await context.EBooks.FindAsync(createEBook.titulo);
    var autorBook = await context.EBooks.FindAsync(createEBook.autor);

    if(titleBook != null){
        return Results.BadRequest();
    }
    if(autorBook != null){
        return Results.BadRequest();
    }

    await context.AddAsync(createEBook);
    await context.SaveChangesAsync();

    return Results.Ok();
}

async Task<IResult> GetEbooksAvailablesAsync(DataContext context)
{
    return await context.EBooks.ToListAsync();
}

async Task<IResult> UpdateEbookAsync(DataContext context, UpdateEBookDto updateEBook, int id)
{
    var existUser = await context.EBooks.FindAsync(id);
    if(existUser == null){
        return Results.BadRequest();
    }

    existUser.Title = updateEBook.titulo ?? existUser.Title;
    existUser.Author = updateEBook.autor ?? existUser.Author;
    existUser.Genre = updateEBook.genero ?? existUser.Genre;
    existUser.Format = updateEBook.formato ?? existUser.Format;
    existUser.Price = updateEBook.precio;

    context.Entry(existUser).State = EntityState.Modified;
    await context.SaveChangesAsync();

    return Results.Ok();
}

async Task<IResult> ChangeAvailableEbookAsync(DataContext context, int id)
{
    var existBook = await context.EBooks.FindAsync(id);
    if(existBook == null){
        return Results.BadRequest();
    }

    if(existBook.IsAvailable == true){
        existBook.IsAvailable = false;
        return Results.Ok();    
    }

    existBook.IsAvailable = true;
    return Results.Ok();
}

async Task<IResult> IncrementStockEbookAsync(DataContext context, int id, StockEBookDto stockEBook)
{
    var existBook = await context.EBooks.FindAsync(id);
    if(existBook == null){
        return Results.BadRequest();
    }
    if(stockEBook.stock <= 0){
        return Results.BadRequest();
    } 
    existBook.Stock = stockEBook.stock + existBook.Stock;
    await context.SaveChangesAsync(); 
    return Results.Ok();
}

async Task<IResult> BuyEbookAsync(DataContext context, BuyEBookDto buyEBook)
{
    var existBook = await context.EBooks.FindAsync(buyEBook);
    if(existBook == null){
        return Results.BadRequest();
    }
    int precioTotal = buyEBook.cantidad * existBook.Price;

    return Results.Ok();
}

async Task<IResult> DeleteEbookAsync(DataContext context, int id)
{
    var deleteBook = await context.EBooks.FindAsync(id);
    if(deleteBook == null){
        return Results.BadRequest();
    }

    context.EBooks.Remove(deleteBook);
    
    return Results.Ok();
}
