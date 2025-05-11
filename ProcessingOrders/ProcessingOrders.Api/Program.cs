using ProcessingOrders.Api.Endpoints;
using ProcessingOrders.Application.Configuration;
using ProcessingOrders.Persistence.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapOrderEndpoints();

var discountEndpoints = app.MapGroup("/api/discounts")
    .WithTags("Discounts")
    .WithOpenApi();
    
discountEndpoints.MapDiscountEndpoints();

app.Run(); 