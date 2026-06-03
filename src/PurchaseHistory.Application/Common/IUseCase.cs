namespace PurchaseHistory.Application.Common;

public interface IUseCase<TInput, TOutput>
{
    Task<TOutput> Handle(TInput input);
}