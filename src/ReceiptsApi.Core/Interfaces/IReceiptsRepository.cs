namespace ReceiptsApi.Core.Interfaces;
public interface IReceiptsRepository
{
  void AddReceipt(ReceiptEntity receipt);
  ReceiptEntity GetReceiptById(Guid id);
}
