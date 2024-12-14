namespace ReceiptsApi.Core.Interfaces;
public interface IPointsCalculator
{
  public int CalculatePointsForReceipt(ReceiptEntity receipt);
}
