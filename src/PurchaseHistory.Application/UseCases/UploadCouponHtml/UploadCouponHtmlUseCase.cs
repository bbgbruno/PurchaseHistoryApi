using PurchaseHistory.Domain.Entities;
using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Domain.Interfaces;
using PurchaseHistory.Domain.Interfaces.Services;

namespace PurchaseHistory.Application.UseCases.UploadCouponHtml;

public class UploadCouponHtmlUseCase
{
    private readonly ICouponHtmlParser _parser;

    private readonly IStoreRepository _storeRepository;

    private readonly IPurchaseRepository _purchaseRepository;

    private readonly IPurchaseItemRepository _purchaseItemRepository;

    private readonly IProductRepository _productRepository;

    private readonly IProductNormalizationService _productNormalization;

    public UploadCouponHtmlUseCase(
        ICouponHtmlParser parser,
        IStoreRepository storeRepository,
        IPurchaseRepository purchaseRepository,
        IPurchaseItemRepository purchaseItemRepository,
        IProductRepository productRepository,
        IProductNormalizationService productNormalizationService)
    {
        _parser = parser;
        _storeRepository = storeRepository;
        _purchaseRepository = purchaseRepository;
        _purchaseItemRepository = purchaseItemRepository;
        _productRepository = productRepository;
        _productNormalization = productNormalizationService;
    }

    public async Task<UploadCouponHtmlOutput> Handle(
        UploadCouponHtmlInput input)
    {
        /*
        ==========================================================
        PARSE HTML
        ==========================================================
        */

        var importedCoupon =
            _parser.Parse(input.HtmlContent);

        if (importedCoupon.Items == null ||
            !importedCoupon.Items.Any())
        {
            throw new Exception(
                "Nenhum item encontrado no cupom."
            );
        }

        /*
        ==========================================================
        STORE
        ==========================================================
        */

        var existingStore =
            await _storeRepository.GetByDocumentAsync(
                importedCoupon.DocumentNumber
            );

        Guid storeId;

        if (existingStore == null)
        {
            var store = new Store
            {
                Id = Guid.NewGuid(),

                Name = importedCoupon.StoreName,

                TradeName = importedCoupon.StoreName,

                DocumentNumber =
                    importedCoupon.DocumentNumber,

                CreatedAt = DateTime.UtcNow
            };

            storeId =
                await _storeRepository.CreateAsync(store);
        }
        else
        {
            storeId = existingStore.Id;
        }

        /*
        ==========================================================
        DUPLICIDADE
        ==========================================================
        */

        if (!string.IsNullOrWhiteSpace(importedCoupon.AccessKey))
        {
            var jaImportado =
                await _purchaseRepository.ExistsByAccessKeyAsync(
                    importedCoupon.AccessKey
                );

            if (jaImportado)
            {
                throw new Exception(
                    "Este cupom já foi importado anteriormente."
                );
            }
        }

        /*
        ==========================================================
        PURCHASE
        ==========================================================
        */

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),

            UserId = input.UserId,

            StoreId = storeId,

            AccessKey = importedCoupon.AccessKey,

            PurchaseDate = importedCoupon.PurchaseDate,

            TotalValue = importedCoupon.TotalValue,

            XmlFileName = input.FileName,

            CreatedAt = DateTime.UtcNow
        };

        var purchaseId =
            await _purchaseRepository.CreateAsync(
                purchase
            );

        /*
        ==========================================================
        PURCHASE ITEMS
        ==========================================================
        */

        var purchaseItems =
            new List<PurchaseItem>();

        foreach (var importedItem
                 in importedCoupon.Items)
        {
            /*
            ======================================================
            NORMALIZA NOME DO PRODUTO
            ======================================================
            */

            var normalizedName =
                NormalizeProductName(
                    importedItem.OriginalDescription
                );

            /*
            ======================================================
            APLICA REGRAS DE NORMALIZAÇÃO
            ======================================================
            */

            normalizedName = await _productNormalization.ApplyMappingsAsync(importedItem.OriginalDescription);

            normalizedName = await _productNormalization.NormalizeWithSubstitutionsAsync(normalizedName);

            /*
            ======================================================
            PROCURA PRODUTO
            ======================================================
            */

            var existingProduct =
                await _productRepository.FindByNameAsync(
                    normalizedName,
                    input.UserId
                );

            Guid productId;

            if (existingProduct == null)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),

                    UserId = input.UserId,

                    NormalizedName = normalizedName,

                    CreatedAt = DateTime.UtcNow
                };

                productId =
                    await _productRepository.CreateAsync(
                        product
                    );
            }
            else
            {
                productId = existingProduct.Id;
            }

            /*
            ======================================================
            ITEM
            ======================================================
            */

            purchaseItems.Add(new PurchaseItem
            {
                Id = Guid.NewGuid(),

                PurchaseId = purchaseId,

                ProductId = productId,

                OriginalDescription =
                    importedItem.OriginalDescription,

                Quantity =
                    importedItem.Quantity,

                Unit =
                    importedItem.Unit,

                UnitPrice =
                    importedItem.UnitPrice,

                TotalPrice =
                    importedItem.TotalPrice,

                CreatedAt = DateTime.UtcNow
            });
        }

        /*
        ==========================================================
        SAVE ITEMS
        ==========================================================
        */

        await _purchaseItemRepository.SaveAsync(
            purchaseItems
        );

        /*
        ==========================================================
        OUTPUT
        ==========================================================
        */

        return new UploadCouponHtmlOutput
        {
            PurchaseId = purchaseId,

            StoreName = importedCoupon.StoreName,

            PurchaseDate =
                importedCoupon.PurchaseDate,

            TotalItems = purchaseItems.Count,

            TotalValue = importedCoupon.TotalValue
        };
    }

    /*
    ==============================================================
    NORMALIZAÇÃO SIMPLES
    ==============================================================
    */

    private static string NormalizeProductName(
        string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;

        return description
            .Trim()
            .ToUpperInvariant();
    }
}