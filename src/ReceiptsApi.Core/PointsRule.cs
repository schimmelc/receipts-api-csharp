namespace ReceiptsApi.Core;
public class PointsRule
{
  public string RuleName { get; set; }
  public string RuleDescription { get; set; }
  public Func<ReceiptEntity, int> Rule { get; set; }
}
