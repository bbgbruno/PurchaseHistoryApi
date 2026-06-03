namespace PurchaseHistory.Domain.Interfaces.Services;

public interface IProductNormalizationService
{
    string Normalize(string productName);

    Task<string> NormalizeWithSubstitutionsAsync(string productName);

    Task<string> ApplyMappingsAsync(string description);
}