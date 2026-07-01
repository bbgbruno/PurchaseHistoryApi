using System.Globalization;
using System.Text.RegularExpressions;
using PurchaseHistory.Domain.Interfaces;
using PurchaseHistory.Domain.Models.Dtos;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PurchaseHistory.Infrastructure.Parsers;

public class NfcePdfParser : IPdfCouponParser
{
    public ImportedCouponDto Parse(byte[] pdfContent)
    {
        using var pdf = PdfDocument.Open(pdfContent);
        var text = ExtractTextWithLines(pdf);

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
        var match = Regex.Match(text, @"RAZÃO\s*SOCIAL[:\s]*([^\n]*?)(?:CNPJ|IE|ENDEREÇO)", RegexOptions.IgnoreCase);
        if (match.Success)
            return Clean(match.Groups[1].Value);

        match = Regex.Match(text, @"NOME\s*[:\s]*([^\n]*?)(?:CPF|CNPJ)", RegexOptions.IgnoreCase);
        return match.Success ? Clean(match.Groups[1].Value) : string.Empty;
    }

    private static string ExtractDocumentNumber(string text)
    {
        var match = Regex.Match(text, @"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}");
        return match.Success ? match.Value : string.Empty;
    }

    private static DateTime? ExtractPurchaseDate(string text)
    {
        var match = Regex.Match(text, @"Data de Emissão[:\s]*(\d{2}/\d{2}/\d{4})", RegexOptions.IgnoreCase);
        if (!match.Success)
            match = Regex.Match(text, @"(\d{2}/\d{2}/\d{4})");

        if (match.Success && DateTime.TryParseExact(match.Groups[1].Value, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        return null;
    }

    private static string ExtractAccessKey(string text)
    {
        var match = Regex.Match(text, @"CHAVE\s*DE\s*ACESSO\s*NFCe\s*(\d{44})", RegexOptions.IgnoreCase);
        if (match.Success)
            return match.Groups[1].Value;

        match = Regex.Match(text, @"CHAVE\s*DE\s*ACESSO[^\d]*(\d{44})", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static decimal ExtractTotalValue(string text)
    {
        var match = Regex.Match(text, @"Valor\s*Total\s*da\s*Nota.*?R\$\s*([\d.,]+)", RegexOptions.IgnoreCase);
        if (match.Success)
            return ParseDecimal(match.Groups[1].Value);

        match = Regex.Match(text, @"TOTAL\s*[:\sR\$]*([\d.,]+)", RegexOptions.IgnoreCase);
        return match.Success ? ParseDecimal(match.Groups[1].Value) : 0;
    }

    private static string ExtractTextWithLines(PdfDocument pdf)
    {
        var lines = new List<string>();

        foreach (var page in pdf.GetPages())
        {
            var letters = page.Letters.ToList();
            var grouped = letters
                .GroupBy(l => Math.Round(l.GlyphRectangle.Bottom, 1))
                .OrderByDescending(g => g.Key);

            foreach (var group in grouped)
            {
                var line = string.Join("",
                    group.OrderBy(l => l.GlyphRectangle.Left)
                         .Select(l => l.Value));
                lines.Add(line.TrimEnd());
            }
        }

        return string.Join("\n", lines);
    }

    private static List<ImportedItemDto> ExtractItems(string text)
    {
        var items = new List<ImportedItemDto>();
        var allLines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var headerIdx = -1;
        for (var i = 0; i < allLines.Length; i++)
        {
            var line = allLines[i].Trim();
            if (line.Length >= 10 && line[..10].Equals("ItemCódigo"))
            {
                headerIdx = i;
                break;
            }
        }

        if (headerIdx < 0) return items;

        for (var i = headerIdx + 1; i < allLines.Length; i++)
        {
            var line = allLines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("Valor Total", StringComparison.OrdinalIgnoreCase)) break;

            try
            {
                var item = ParseItemLine(line);
                if (item != null)
                    items.Add(item);
            }
            catch { }
        }

        return items;
    }

    private static ImportedItemDto? ParseItemLine(string line)
    {
        var asterisk = line.IndexOf('*');
        var rest = asterisk >= 0
            ? line[(asterisk + 1)..]
            : Regex.Replace(line, @"^\d+", "");

        var unitMatch = Regex.Match(rest, @"(UN|KG|G|ML|L|PC|PCT|CX|FD|LT|M|M2|M3)(?=\d+,\d{2})", RegexOptions.IgnoreCase);
        if (!unitMatch.Success) return null;

        var unit = unitMatch.Groups[1].Value.ToUpperInvariant();
        var beforeUnit = rest[..unitMatch.Index];
        var numbersPart = rest[(unitMatch.Index + unitMatch.Length)..];

        var numbers = Regex.Matches(numbersPart, @"\d+,\d{2}")
            .Select(m => m.Value)
            .ToList();

        if (numbers.Count < 2) return null;

        var unitPrice = ParseDecimal(numbers[0]);
        var totalPrice = ParseDecimal(numbers[^1]);
        var quantity = unitPrice > 0
            ? Math.Round(totalPrice / unitPrice, 3)
            : 1;

        var description = Clean(beforeUnit);
        description = Regex.Replace(description, @"\d{4}\d{8,9}[\d/]+\s*$", "").Trim();

        return new ImportedItemDto
        {
            OriginalDescription = description,
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
        return Regex.Replace(text.Trim(), @"\s+", " ").Trim();
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;

        value = value.Replace("R$", "").Replace(" ", "").Trim();

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
