using System.Globalization;
using System.Text.RegularExpressions;
using PurchaseHistory.Domain.Interfaces;
using PurchaseHistory.Domain.Models.Dtos;
using UglyToad.PdfPig;

namespace PurchaseHistory.Infrastructure.Parsers;

public class NfcePdfParser : IPdfCouponParser
{
    public ImportedCouponDto Parse(byte[] pdfContent)
    {
        using var pdf = PdfDocument.Open(pdfContent);
        var text = string.Join("\n", pdf.GetPages().Select(p => p.Text));

        return new ImportedCouponDto
        {
            StoreName = ExtractStoreName(text),
            DocumentNumber = ExtractDocumentNumber(text),
            PurchaseDate = ExtractPurchaseDate(text),
            AccessKey = ExtractAccessKey(text),
            TotalValue = ExtractTotalValue(text),
            Items = ExtractItems(text)
        };
    }

    private static string ExtractStoreName(string text)
    {
        var match = Regex.Match(text, @"RAZÃO\s*SOCIAL[:\s]*([^\n]+)", RegexOptions.IgnoreCase);
        if (match.Success)
            return Clean(match.Groups[1].Value);

        match = Regex.Match(text, @"NOME\s*[:\s]*([^\n]+)", RegexOptions.IgnoreCase);
        return match.Success ? Clean(match.Groups[1].Value) : string.Empty;
    }

    private static string ExtractDocumentNumber(string text)
    {
        var match = Regex.Match(text, @"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}");
        return match.Success ? match.Value : string.Empty;
    }

    private static DateTime? ExtractPurchaseDate(string text)
    {
        var match = Regex.Match(text, @"(\d{2}/\d{2}/\d{4})");
        if (!match.Success)
            return null;

        if (DateTime.TryParseExact(match.Groups[1].Value, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        return null;
    }

    private static string ExtractAccessKey(string text)
    {
        var match = Regex.Match(text, @"(\d{44})");
        return match.Success ? match.Value : string.Empty;
    }

    private static decimal ExtractTotalValue(string text)
    {
        var match = Regex.Match(text, @"VALOR\s*TOTAL[:\sR\$]*([\d.,]+)", RegexOptions.IgnoreCase);
        if (!match.Success)
            match = Regex.Match(text, @"TOTAL[:\sR\$]*([\d.,]+)", RegexOptions.IgnoreCase);

        return match.Success ? ParseDecimal(match.Groups[1].Value) : 0;
    }

    private static List<ImportedItemDto> ExtractItems(string text)
    {
        var items = new List<ImportedItemDto>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var inItems = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (line.Contains("PRODUTO", StringComparison.OrdinalIgnoreCase) &&
                (line.Contains("QTD", StringComparison.OrdinalIgnoreCase) ||
                 line.Contains("QUANT", StringComparison.OrdinalIgnoreCase) ||
                 line.Contains("UN", StringComparison.OrdinalIgnoreCase)))
            {
                inItems = true;
                continue;
            }

            if (inItems && string.IsNullOrWhiteSpace(line))
            {
                inItems = false;
                continue;
            }

            if (!inItems) continue;

            if (line.Contains("VALOR TOTAL", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("TOTAL", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("CHAVE", StringComparison.OrdinalIgnoreCase))
            {
                inItems = false;
                continue;
            }

            try
            {
                var item = ParseItemLine(line);
                if (item != null)
                    items.Add(item);
            }
            catch
            {
                // ignora linha inválida
            }
        }

        return items;
    }

    private static ImportedItemDto? ParseItemLine(string line)
    {
        line = line.Trim();
        if (string.IsNullOrWhiteSpace(line))
            return null;

        line = Regex.Replace(line, @"\s+", " ");

        var numbers = Regex.Matches(line, @"[\d.,]+")
            .Select(m => m.Value)
            .ToList();

        if (numbers.Count < 3)
            return null;

        var totalPrice = ParseDecimal(numbers[^1]);
        var unitPrice = ParseDecimal(numbers[^2]);
        var quantity = ParseDecimal(numbers[^3]);

        var descEnd = line.LastIndexOf(numbers[^3], StringComparison.Ordinal);
        if (descEnd < 0) return null;

        var description = line[..descEnd].Trim();
        description = Regex.Replace(description, @"[\d.,]+\s*$", "").Trim();

        if (string.IsNullOrWhiteSpace(description))
            return null;

        string? unit = null;
        var unitMatch = Regex.Match(description, @"\b(UN|KG|G|ML|L|PC|PCT|CX|FD|LT|M|CM|M2|M3)\b$", RegexOptions.IgnoreCase);
        if (unitMatch.Success)
        {
            unit = unitMatch.Value.ToUpperInvariant();
            description = description[..^unitMatch.Length].Trim();
        }

        return new ImportedItemDto
        {
            OriginalDescription = Clean(description),
            Quantity = quantity,
            Unit = unit,
            UnitPrice = unitPrice,
            TotalPrice = totalPrice
        };
    }

    private static string Clean(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        value = value
            .Replace("R$", "")
            .Replace(" ", "")
            .Trim();

        if (value.Contains(',') && value.Contains('.'))
        {
            if (value.LastIndexOf(',') > value.LastIndexOf('.'))
                value = value.Replace(".", "").Replace(",", ".");
            else
                value = value.Replace(",", "");
        }
        else if (value.Contains(','))
        {
            value = value.Replace(",", ".");
        }

        decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
        return result;
    }
}
