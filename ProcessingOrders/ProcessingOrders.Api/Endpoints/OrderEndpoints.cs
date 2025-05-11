using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessingOrders.Api.Models;
using ProcessingOrders.Application.Features.Orders.Commands.CreateOrder;
using ProcessingOrders.Application.Features.Orders.Commands.UpdateOrderStatus;
using ProcessingOrders.CoreDomain.ValueObjects;

namespace ProcessingOrders.Api.Endpoints;

public static class OrderEndpoints
{
    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        var orders = app.MapGroup("/api/orders")
            .WithTags("Orders")
            .WithOpenApi();
            
        orders.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithDescription("Создает новый заказ с указанными товарами.")
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
            
        orders.MapPut("/{id}/status", UpdateOrderStatus)
            .WithName("UpdateOrderStatus")
            .WithDescription("Обновляет статус заказа. Возможные статусы: Processing, Completed, Cancelled.")
            .Produces<OrderResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
        
    private static async Task<IResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateOrderCommand
        {
            CustomerId = request.CustomerId,
            Items = new List<OrderItemDto>()
        };
            
        foreach (var item in request.Items)
        {
            command.Items.Add(new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price,
                Currency = item.Currency
            });
        }
            
        var order = await mediator.Send(command);
            
        var response = OrderResponse.FromDomain(order);
            
        return Results.Created($"/api/orders/{response.Id}", response);
    }
        
    private static async Task<IResult> UpdateOrderStatus(
        [FromRoute] long id,
        [FromBody] UpdateOrderStatusRequest request,
        [FromServices] IMediator mediator)
    {
        if (!Enum.TryParse<OrderStatus>(request.Status, out var orderStatus))
        {
            return Results.BadRequest($"Недопустимый статус заказа: {request.Status}");
        }
            
        var command = new UpdateOrderStatusCommand
        {
            OrderId = id,
            NewStatus = orderStatus,
            CancellationReason = request.CancellationReason
        };
            
        try
        {
            var order = await mediator.Send(command);
            var response = OrderResponse.FromDomain(order);
            return Results.Ok(response);
        }
        catch (ArgumentException ex)
        {
            if (ex.Message.Contains("не найден"))
                return Results.NotFound($"Заказ с id {id} не найден");
                
            return Results.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
} 