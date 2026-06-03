using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using PurchaseHistory.Domain.Interfaces.Repositories;
using PurchaseHistory.Domain.Interfaces.Services;

namespace PurchaseHistory.Infrastructure.Services;

public class ProductNormalizationService
    : IProductNormalizationService
{
    private readonly IProductNormalizationMappingRepository _mappingRepository;
    private readonly IProductKeywordSubstitutionRepository _keywordRepository;

    public ProductNormalizationService(
        IProductNormalizationMappingRepository mappingRepository,
        IProductKeywordSubstitutionRepository keywordRepository)
    {
        _mappingRepository = mappingRepository;
        _keywordRepository = keywordRepository;
    }

    public string Normalize(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            return string.Empty;

        var text =
            RemoveAccents(productName)
            .ToUpperInvariant();

        text = Regex.Replace(
            text,
            @"\b(TRAD|TRADICIONAL|UND|UN)\b",
            string.Empty);

        text = text.Replace("GRAMAS", "G");
        text = text.Replace("GRAMA", "G");
        text = text.Replace("GR", "G");

        text = Regex.Replace(
            text,
            @"\s+",
            " ");

        return text.Trim();
    }

    public async Task<string> NormalizeWithSubstitutionsAsync(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            return string.Empty;

        var text =
            RemoveAccents(productName)
            .ToUpperInvariant();

        var substitutions =
            await _keywordRepository.GetAllActiveAsync();

        foreach (var sub in substitutions)
        {
            var original = Normalize(sub.OriginalTerm);

            if (string.IsNullOrWhiteSpace(original))
                continue;

            var replacement = Normalize(sub.ReplacementTerm);

            if (sub.MatchType == "WholeWord")
            {
                text = Regex.Replace(
                    text,
                    $@"\b{Regex.Escape(original)}\b",
                    replacement);
            }
            else
            {
                text = text.Replace(original, replacement);
            }
        }

        text = Regex.Replace(
            text,
            @"\b(TRAD|TRADICIONAL|UND|UN)\b",
            string.Empty);

        text = text.Replace("GRAMAS", "G");
        text = text.Replace("GRAMA", "G");
        text = text.Replace("GR", "G");

        text = Regex.Replace(
            text,
            @"\s+",
            " ");

        return text.Trim();
    }

    public async Task<string> ApplyMappingsAsync(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;

        var normalized = Normalize(description);
        var mappings = await _mappingRepository.GetAllActiveAsync();

        foreach (var mapping in mappings)
        {
            var mappingNormalized = Normalize(mapping.OriginalText);

            if (mapping.MatchType == "Exact")
            {
                if (normalized.Equals(mappingNormalized, StringComparison.OrdinalIgnoreCase))
                    return Normalize(mapping.ReplacementText);
            }
            else if (mapping.MatchType == "Contains")
            {
                if (normalized.Contains(mappingNormalized, StringComparison.OrdinalIgnoreCase))
                    return Normalize(mapping.ReplacementText);
            }
        }

        return normalized;
    }

    private static string RemoveAccents(
        string text)
    {
        var normalized =
            text.Normalize(
                NormalizationForm.FormD);

        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory =
                CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory !=
                UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString()
            .Normalize(
                NormalizationForm.FormC);
    }
}