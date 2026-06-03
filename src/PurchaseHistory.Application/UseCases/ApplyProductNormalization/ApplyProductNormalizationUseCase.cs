using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Domain.Interfaces.Services;

namespace PurchaseHistory.Application.UseCases.ApplyProductNormalization;

public class ApplyProductNormalizationUseCase
{
    private readonly IProductNormalizationMappingRepository _mappingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPurchaseItemRepository _purchaseItemRepository;
    private readonly IProductNormalizationService _normalizationService;

    public ApplyProductNormalizationUseCase(
        IProductNormalizationMappingRepository mappingRepository,
        IProductRepository productRepository,
        IPurchaseItemRepository purchaseItemRepository,
        IProductNormalizationService normalizationService)
    {
        _mappingRepository = mappingRepository;
        _productRepository = productRepository;
        _purchaseItemRepository = purchaseItemRepository;
        _normalizationService = normalizationService;
    }

    public async Task<ApplyProductNormalizationOutput> Handle()
    {
        var totalItemsRedirected = 0;
        var totalProductsRemoved = 0;
        var appliedRules = new List<AppliedRuleResult>();

        /*
        ==========================================================
        SUBSTITUIÇÕES
        ==========================================================
        */

        var allProducts = await _productRepository.GetAllAsync();

        foreach (var product in allProducts)
        {
            var originalName = product.NormalizedName;
            var substituted = await _normalizationService.NormalizeWithSubstitutionsAsync(originalName);

            if (substituted == originalName)
                continue;

            var existing = await _productRepository.FindByNameAsync(substituted);

            if (existing != null && existing.Id != product.Id)
            {
                var items = await _purchaseItemRepository.GetByProductIdAsync(product.Id);
                var itemsList = items.ToList();
                var itemCount = itemsList.Count;

                if (itemCount > 0)
                {
                    await _purchaseItemRepository.RedirectProductIdAsync(product.Id, existing.Id);
                    totalItemsRedirected += itemCount;
                }

                await _productRepository.DeleteAsync(product.Id);
                totalProductsRemoved++;

                appliedRules.Add(new AppliedRuleResult
                {
                    OriginalText = originalName,
                    ReplacementText = substituted,
                    ItemsRedirected = itemCount,
                    ProductRemoved = true
                });
            }
            else
            {
                product.NormalizedName = substituted;
                await _productRepository.UpdateAsync(product);

                appliedRules.Add(new AppliedRuleResult
                {
                    OriginalText = originalName,
                    ReplacementText = substituted,
                    ItemsRedirected = 0,
                    ProductRemoved = false
                });
            }
        }

        /*
        ==========================================================
        MAPPINGS
        ==========================================================
        */

        var mappings = await _mappingRepository.GetAllActiveAsync();

        foreach (var mapping in mappings)
        {
            var sourceProduct = await _productRepository.FindByNameAsync(mapping.OriginalText);
            if (sourceProduct == null)
                continue;

            var targetProduct = await _productRepository.FindByNameAsync(mapping.ReplacementText);
            if (targetProduct == null)
            {
                targetProduct = new Product
                {
                    NormalizedName = mapping.ReplacementText,
                    CreatedAt = DateTime.UtcNow
                };
                var newId = await _productRepository.CreateAsync(targetProduct);
                targetProduct.Id = newId;
            }

            if (sourceProduct.Id == targetProduct.Id)
                continue;

            var items = await _purchaseItemRepository.GetByProductIdAsync(sourceProduct.Id);
            var itemsList = items.ToList();
            var itemCount = itemsList.Count;

            if (itemCount > 0)
            {
                await _purchaseItemRepository.RedirectProductIdAsync(sourceProduct.Id, targetProduct.Id);
                totalItemsRedirected += itemCount;
            }

            await _productRepository.DeleteAsync(sourceProduct.Id);
            totalProductsRemoved++;

            appliedRules.Add(new AppliedRuleResult
            {
                MappingId = mapping.Id,
                OriginalText = mapping.OriginalText,
                ReplacementText = mapping.ReplacementText,
                ItemsRedirected = itemCount,
                ProductRemoved = true
            });
        }

        return new ApplyProductNormalizationOutput
        {
            TotalItemsRedirected = totalItemsRedirected,
            TotalProductsRemoved = totalProductsRemoved,
            AppliedRules = appliedRules
        };
    }
}

public class ApplyProductNormalizationOutput
{
    public int TotalItemsRedirected { get; set; }
    public int TotalProductsRemoved { get; set; }
    public List<AppliedRuleResult> AppliedRules { get; set; } = [];
}

public class AppliedRuleResult
{
    public Guid MappingId { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public string ReplacementText { get; set; } = string.Empty;
    public int ItemsRedirected { get; set; }
    public bool ProductRemoved { get; set; }
}
