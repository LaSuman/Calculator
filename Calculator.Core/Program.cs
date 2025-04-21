using System.Text.Json;
using System.Xml.Serialization;
using Calculator.Core.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/calculate", async (HttpRequest request) =>
    {
        string contentType = request.ContentType?.ToLowerInvariant() ?? "";
        string body;

        using var reader = new StreamReader(request.Body);
        body = await reader.ReadToEndAsync();

        Maths? maths = null;

        if (contentType.Contains("application/json"))
        {

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var json = JsonDocument.Parse(body);
            var root = json.RootElement;

            JsonElement actualRoot;

            if (root.TryGetProperty("Maths", out var m))
                actualRoot = m;
            else if (root.TryGetProperty("MyMaths", out var mm))
                actualRoot = mm;
            else
                return Results.BadRequest("Invalid input");

            // Normalize Operation/MyOperation inside
            string normalized = actualRoot.GetRawText()
                .Replace("MyOperation", "Operation")
                .Replace("@ID", "ID");

            maths = JsonSerializer.Deserialize<Maths>(normalized, options);
        }
        else if (contentType.Contains("application/xml"))
        {
            // Normalize tags to "Maths" and "Operation" if needed
            body = body.Replace("<MyMaths>", "<Maths>").Replace("</MyMaths>", "</Maths>");
            body = body.Replace("<MyOperation", "<Operation").Replace("</MyOperation>", "</Operation>");

            var serializer = new XmlSerializer(typeof(Maths));
            using var stringReader = new StringReader(body);
            maths = (Maths?)serializer.Deserialize(stringReader);
        }
        else
        {
            return Results.BadRequest("Unsupported content type");
        }

        if (maths?.Operation == null)
            return Results.BadRequest("Invalid input");

        double result = maths.Operation.Calculate();
        return Results.Ok(new { Result = result });
    })
    .Accepts<Maths>("application/json", "application/xml")
    .Produces(200)
    .WithName("Calculate");

app.Run();
