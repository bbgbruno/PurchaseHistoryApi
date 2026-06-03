namespace PurchaseHistory.Domain.Entities;

public class Store
{
    public Guid Id { get; set; }

    /*
    ==========================================================
    IDENTIFICAÇÃO
    ==========================================================
    */

    public string Name { get; set; } = string.Empty;

    public string? TradeName { get; set; }

    public string? DocumentNumber { get; set; }

    public string? StateRegistration { get; set; }

    /*
    ==========================================================
    ENDEREÇO
    ==========================================================
    */

    public string? Address { get; set; }

    public string? AddressNumber { get; set; }

    public string? Neighborhood { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? ZipCode { get; set; }

    /*
    ==========================================================
    CONTROLE
    ==========================================================
    */

    public DateTime CreatedAt { get; set; }
}