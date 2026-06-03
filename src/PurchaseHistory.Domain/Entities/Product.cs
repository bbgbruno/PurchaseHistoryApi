namespace PurchaseHistory.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    /*
    ==========================================================
    PRODUTO NORMALIZADO
    ==========================================================
    */

    public string NormalizedName { get; set; }
        = string.Empty;

    /*
    ==========================================================
    INFORMAÇÕES OPCIONAIS
    ==========================================================
    */

    public string? Brand { get; set; }

    public Guid? CategoryId { get; set; }

    /*
    ==========================================================
    CONTROLE
    ==========================================================
    */

    public DateTime CreatedAt { get; set; }
}