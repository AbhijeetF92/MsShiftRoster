using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var AllowOrigin = "_allowOrigin";

//Adding Cosmos Db service
builder.Configuration.AddJsonFile("appsettings.json");
var cosmosConnectionString = builder.Configuration.GetConnectionString("CosmosDBConnectionString");
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    return new CosmosClient(cosmosConnectionString);
});



builder.Services.AddCors(options =>
{ 
options.AddPolicy(name: AllowOrigin,

    policy =>
    {
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
}); 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors(AllowOrigin);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
