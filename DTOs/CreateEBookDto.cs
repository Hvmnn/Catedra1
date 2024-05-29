using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ebooks_dotnet7_api.DTOs;

public class CreateEBookDto{
    public string titulo{get;set;}
    public string autor{get;set;}
    public string genero{get;set;}
    public string formato{get;set;}
    public int precio{get;set;}
    public bool IsAvailable = true;
    public int stock = 0;
}