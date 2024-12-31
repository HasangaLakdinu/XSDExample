using SharedLibrary;
using SharedLibrary.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapPost("/start-process", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();

    var producerResponse = await client.GetAsync("https://localhost:7049/generate-data");

    if (!producerResponse.IsSuccessStatusCode)
    {
        {
            var errorContent = await producerResponse.Content.ReadAsStringAsync();
            try
            {
                var errorResponse = System.Text.Json.JsonSerializer.Deserialize<object>(errorContent, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Results.BadRequest(new
                {
                    Message = "Failed to retrieve valid XML data from producer.",
                    ProducerError = errorResponse
                });
            }
            catch
            {
                // Fallback for non-JSON error content
                return Results.BadRequest(new
                {
                    Message = "Failed to retrieve valid XML data from producer.",
                    RawError = errorContent
                });
            }
        }
    }


    var xmlData = await producerResponse.Content.ReadAsStringAsync();
    var validationErrors = new List<string>();
    var isValid = XmlHelper.ValidateXml(xmlData, validationErrors);

    if (!isValid)
    {
        return Results.BadRequest(new
        {
            Message = "Invalid XML received from producer.",
            Errors = validationErrors
        });
    }

    var person = XmlHelper.ParseXml<Person>(xmlData);
    Console.WriteLine($"Saving Person to DB: {person.Name}, {person.Age}");
    return Results.Ok($"Process completed successfully... Saving {person.Name} to DB");
});

app.Run();

