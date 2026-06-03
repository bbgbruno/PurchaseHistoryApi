using HtmlAgilityPack;
using PurchaseHistory.Domain.Interfaces;
using PurchaseHistory.Domain.Models.Dtos;
using System.Globalization;

namespace PurchaseHistory.Infrastructure.Parsers;

public class NfceHtmlParser : ICouponHtmlParser
{
    public ImportedCouponDto Parse(string html)
    {
        var document = new HtmlDocument();

        document.LoadHtml(html);

        var coupon = new ImportedCouponDto
        {
            StoreName = ExtractStoreName(document),

            DocumentNumber = ExtractDocumentNumber(document),

            PurchaseDate = ExtractPurchaseDate(document),

            AccessKey = ExtractAccessKey(document),

            TotalValue = ExtractTotalValue(document),

            Items = ExtractItems(document)
        };

        return coupon;
    }

    /*
    ==========================================================
    ITEMS
    ==========================================================
    */

    private static List<ImportedItemDto> ExtractItems(
        HtmlDocument document)
    {
        var items = new List<ImportedItemDto>();

        var rows = document.DocumentNode
            .SelectNodes("//table[@id='tbItensList']//tr");

        if (rows == null)
            return items;

        foreach (var row in rows)
        {
            try
            {
                var descriptionNode = row.SelectSingleNode(
                    ".//span[contains(@id,'lblTbItensDescricao')]"
                );

                if (descriptionNode == null)
                    continue;

                var quantityNode = row.SelectSingleNode(
                    ".//span[contains(@id,'lblTbItensQtde')]"
                );

                var unitNode = row.SelectSingleNode(
                    ".//span[contains(@id,'lblTbItensUnidade')]"
                );

                var unitPriceNode = row.SelectSingleNode(
                    ".//span[contains(@id,'lblTbItensValorUnid')]"
                );

                var totalPriceNode = row.SelectSingleNode(
                    ".//span[contains(@id,'lblTbItensValorTotal')]"
                );

                var item = new ImportedItemDto
                {
                    OriginalDescription = Clean(
                        descriptionNode.InnerText
                    ),

                    Quantity = ParseDecimal(
                        quantityNode?.InnerText
                    ),

                    Unit = Clean(
                        unitNode?.InnerText
                    ),

                    UnitPrice = ParseDecimal(
                        unitPriceNode?.InnerText
                    ),

                    TotalPrice = ParseDecimal(
                        totalPriceNode?.InnerText
                    )
                };

                items.Add(item);
            }
            catch
            {
                /*
                ignora linha inválida
                */
            }
        }

        return items;
    }

    /*
    ==========================================================
    STORE NAME
    ==========================================================
    */

    private static string ExtractStoreName(
    HtmlDocument document)
    {
        /*
        ==========================================================
        NOME DA LOJA
        ==========================================================

        Normalmente fica em:

        div.txtTopo > strong
        */

        // Extrair razão social
        var razaoSocialNode = document.DocumentNode.SelectSingleNode("//span[@id='lblRazaoSocialEmitente']");
        string razaoSocial = razaoSocialNode?.InnerText.Replace("RAZÃO SOCIAL:", "").Trim();

        return razaoSocial;
    }

    /*
    ==========================================================
    DOCUMENT NUMBER (CNPJ)
    ==========================================================
    */

    private static string ExtractDocumentNumber(
     HtmlDocument document)
    {
        // Extrair CNPJ
        var cnpjNode = document.DocumentNode.SelectSingleNode("//span[@id='lblCPFCNPJEmitente']");
        string cnpj = cnpjNode?.InnerText.Replace("CNPJ:", "").Trim();

        var text = cnpj;

        /*
        Procura padrão de CNPJ
        */

        var regex = new System.Text.RegularExpressions.Regex(
            @"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}"
        );

        var match = regex.Match(text);

        if (!match.Success)
            return string.Empty;

        return match.Value;
    }

    /*
    ==========================================================
    PURCHASE DATE
    ==========================================================
    */

    private static DateTime? ExtractPurchaseDate(
        HtmlDocument document)
    {
        // Data de emissão
        var dataEmissaoNode = document.DocumentNode.SelectSingleNode("//span[@id='lblDataEmissao']");
        string dataEmissao = dataEmissaoNode?.InnerText;

        var text = dataEmissao;

        var possibleDate = text
            .Split(' ')
            .FirstOrDefault(x => x.Contains("/"));

        if (DateTime.TryParse(
                possibleDate,
                out var date))
        {
            return date;
        }

        return null;
    }

    /*
    ==========================================================
    ACCESS KEY
    ==========================================================
    */

    private static string ExtractAccessKey(
        HtmlDocument document)
    {
        // Chave de acesso
        var chaveNode = document.DocumentNode.SelectSingleNode("//span[@id='lblChave']");
        string chaveAcesso = chaveNode?.InnerText.Trim();

        return chaveAcesso;
    }

    /*
    ==========================================================
    TOTAL VALUE
    ==========================================================
    */

    private static decimal ExtractTotalValue(
        HtmlDocument document)
    {
        // Valor total
        var valorTotalNode = document.DocumentNode.SelectSingleNode("//span[@id='lblValorTotal']");
        

        return ParseDecimal(valorTotalNode?.InnerText.Trim());
    }

    /*
    ==========================================================
    HELPERS
    ==========================================================
    */

    private static string Clean(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return HtmlEntity.DeEntitize(text)
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace("\t", "")
            .Trim();
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        value = value
            .Replace("R$", "")
            .Replace(".", "")
            .Replace(",", ".")
            .Trim();

        decimal.TryParse(
            value,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var result
        );

        return result;
    }

    private static string ExtractBetween(
        string text,
        string start,
        string end)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var startIndex = text.IndexOf(start);

        if (startIndex < 0)
            return string.Empty;

        startIndex += start.Length;

        var endIndex = text.IndexOf(end, startIndex);

        if (endIndex < 0)
            endIndex = text.Length;

        return text[startIndex..endIndex]
            .Replace(":", "")
            .Trim();
    }
}