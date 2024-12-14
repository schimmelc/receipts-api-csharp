
public class ReceiptEntity
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Retailer { get; set; }
  public DateOnly PurchaseDate { get; set; }
  public TimeOnly PurchaseTime { get; set; }
  public List<ItemEntity> Items { get; set; } = new();
  public string Total { get; set; }
  public int Points { get; set; }
}
