using Orders;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddObservability("Orders", builder.Configuration);
builder.Services.AddDatabase();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Orders");

if (app.Environment.IsDevelopment())
{
    app.MapGet("/envvars", () =>
    {
        var envVars = Environment.GetEnvironmentVariables();
        var envVarsString = string.Join("\n", envVars.Keys.Cast<string>().Select(key => $"{key}={envVars[key]}"));
        return envVarsString;
    });
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOrdersApi();
app.MapObservability();

app.Run();

[JsonSerializable(typeof(List<Order>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
