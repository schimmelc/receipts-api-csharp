using ReceiptsApi.Core;
using ReceiptsApi.Core.Interfaces;

namespace ReceiptsApi.Application;

public class PointsCalculator : IPointsCalculator
{
  public List<PointsRule> pointsRules = new List<PointsRule>();

  public PointsCalculator()
  {
    PopulatePointsRulesList();
  }
  public int CalculatePointsForReceipt(ReceiptEntity receipt)
  {
    var points = 0;

    //apply each rule to the receipt
    foreach (var rule in pointsRules)
    {
      points += rule.Rule(receipt);
    }

    return points;
  }

  private void PopulatePointsRulesList()
  {
    pointsRules =
    [
      new PointsRule
      {
        RuleName = "Retailer Name",
        RuleDescription = "One point for every alphanumeric character in the retailer name.",
        Rule = (receipt) => receipt.Retailer.Count(char.IsLetterOrDigit)
      },
      new PointsRule
      {
        RuleName = "Total Round Dollar Amount",
        RuleDescription = "50 points if the total is a round dollar amount with no cents.",
        Rule = (receipt) => receipt.Total.Contains(".00") ? 50 : 0
      },
      new PointsRule
      {
        RuleName = "Total Multiple of 0.25",
        RuleDescription = "25 points if the total is a multiple of 0.25.",
        Rule = (receipt) => decimal.Parse(receipt.Total) % 0.25m == 0 ? 25 : 0
      },
      new PointsRule
      {
        RuleName = "Two Items",
        RuleDescription = "5 points for every two items on the receipt.",
        Rule = (receipt) => receipt.Items.Count() / 2 * 5
      },
      new PointsRule
      {
        RuleName = "Item Description Length",
        RuleDescription = "If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.",
        Rule = (receipt) => receipt.Items.Sum(item => item.ShortDescription.Trim().Length % 3 == 0 ? (int)Math.Ceiling(decimal.Parse(item.Price) * 0.2m) : 0)
      },
      new PointsRule
      {
        RuleName = "Odd Day",
        RuleDescription = "6 points if the day in the purchase date is odd.",
        Rule = (receipt) => receipt.PurchaseDate.Day % 2 == 1 ? 6 : 0
      },
      new PointsRule
      {
        RuleName = "Afternoon Purchase",
        RuleDescription = "10 points if the time of purchase is after 2:00pm and before 4:00pm.",
        Rule = (receipt) => receipt.PurchaseTime.Hour >= 14 && receipt.PurchaseTime.Hour < 16 ? 10 : 0
      },
    ];
  }
}
