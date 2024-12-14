using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ReceiptsApi.Web.CustomAttributes;

namespace ReceiptsApi.Web.Receipts;

public class Receipt
{
  [Description("The name of the retailer or store the receipt is from.")]
  [JsonPropertyName("retailer")]
  [Required]
  [RegularExpression("^[\\w\\s\\-&]+$")]
  [OpenApiExampleAttribute("Costco")]
  public string Retailer { get; set; }

  [Description("The date of the purchase printed on the receipt.")]
  [JsonPropertyName("purchaseDate")]
  [Required]
  [OpenApiExampleAttribute("2024-12-11")]
  public DateOnly PurchaseDate { get; set; }

  [Description("The time of the purchase printed on the receipt. 24-hour time expected.")]
  [JsonPropertyName("purchaseTime")]
  [Required]
  [RegularExpression("^\\d{2}:\\d{2}$")]
  [OpenApiExampleAttribute("16:02")]
  public TimeOnly PurchaseTime { get; set; }

  [JsonPropertyName("items")]
  [Required]
  [MinLength(1)]
  public Item[] Items { get; set; }

  [Description("The total amount paid on the receipt.")]
  [JsonPropertyName("total")]
  [Required]
  [RegularExpression("^\\d+\\.\\d{2}$")]
  [OpenApiExampleAttribute("143.90")]
  public string Total { get; set; }
}

public class Item
{
  [Description("The Short Product Description for the item.")]
  [JsonPropertyName("shortDescription")]
  [Required]
  [RegularExpression("^[\\w\\s\\-]+$")]
  [OpenApiExampleAttribute("Entenmann's Little Bites Chocolate Chip Muffins 20-Count")]
  public string ShortDescription { get; set; }

  [Description("The total price payed for this item.")]
  [JsonPropertyName("price")]
  [Required]
  [RegularExpression("^\\d+\\.\\d{2}$")]
  [OpenApiExampleAttribute("13.99")]
  public string Price { get; set; }
}
