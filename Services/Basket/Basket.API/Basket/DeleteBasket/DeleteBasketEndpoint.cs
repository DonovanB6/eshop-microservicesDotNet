using Carter;
using Mapster;
using MediatR;

namespace Basket.API.Basket.DeleteBasket
{
    //public record DeleteBasketRequest(string userName);

    public record DeleteBasketResponse(bool IsSuccess);
    public class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{username}", async (string UserName, ISender sender) =>
            {
                var result = await sender.Send(new DeleteBasketCommand(UserName));

                var response = result.Adapt<DeleteBasketResponse>();

                return Results.Ok(response);
            })
                .WithName("DeleteBasket")
                .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Delete user's shopping cart")
                .WithDescription("Delete user's shopping cart by username");
        }
    }
}
