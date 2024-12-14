public class ItemEntity
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string ShortDescription { get; set; }
  public string Price { get; set; }

  // Foreign key to ReceiptEntity
  public Guid ReceiptEntityId { get; set; }
  public ReceiptEntity Receipt { get; set; }
}
