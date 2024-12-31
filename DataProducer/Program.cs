using SharedLibrary;
using SharedLibrary.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/generate-data", () =>
{
    var person = new Person { ID = 1, Name = "John Doe", Age = 30, Email = "hasanga@gmail.com" };

    var (xmlData, validationErrors) = XmlHelper.GenerateAndValidateXml(person);
    if (xmlData == null)
    {
        return Results.BadRequest(new
        {
            Message = "Invalid data schema.",
            Errors = validationErrors
        });
    }

    return Results.Text(xmlData, "application/xml");
});

app.Run();
