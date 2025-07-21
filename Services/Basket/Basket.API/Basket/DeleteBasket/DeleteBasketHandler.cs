using BuildingBlocks.CQRS;
using FluentValidation;

namespace Basket.API.Basket.DeleteBasket
{

    public record DeleteBasketCommand(string userName) : ICommand<DeleteBasketResult>;

    public record DeleteBasketResult(bool isSuccess);

    public class DeleteBasketCommandValidator: AbstractValidator<DeleteBasketCommand>
    {
        public DeleteBasketCommandValidator()
        {
            RuleFor(x => x.userName).NotEmpty().WithMessage("UserName cannot be empty");
        }
    }

    public class DeleteBasketHandler : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
    {
        public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
        {
            //Todo: delete basket from database and Cache
            // session.Delete;

            return new DeleteBasketResult(true);
        }
    }
}
