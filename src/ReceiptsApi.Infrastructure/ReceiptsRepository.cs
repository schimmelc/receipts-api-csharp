using ReceiptsApi.Core.Interfaces;

namespace ReceiptsApi.Application;
public class ReceiptsRepository : IReceiptsRepository
{
  private readonly ReceiptsApiDbContext _context;

  public ReceiptsRepository(ReceiptsApiDbContext context)
  {
    _context = context;
  }

  public void AddReceipt(ReceiptEntity receipt)
  {
    _context.Add(receipt);
    _context.SaveChanges();
  }

  public ReceiptEntity GetReceiptById(Guid id) => _context.Find<ReceiptEntity>(id);
}
