using ReceiptsApi.Application;

namespace ReceiptsApi.UnitTests.Core;
public class PointsCalculatorTests
{
  
  [Fact]
  public void CalculatePointsForReceipt_WalmartOneSnickers_Returns6()
  {
    var pointsCalculator = new PointsCalculator();
    // Arrange
    var receipt = new ReceiptEntity
    {
      Retailer = "Walmart",
      Total = "1.99",
      PurchaseTime = new TimeOnly(16, 02),
      PurchaseDate = new DateOnly(2024, 12, 11),
      Items = new[]
      {
        new ItemEntity
        {
          ShortDescription = "Snickers",
          Price = "1.99"
        }
      }
    };

    // Act
    var points = pointsCalculator.CalculatePointsForReceipt(receipt);

    // Assert
    Assert.Equal(6, points);
  }

  [Fact]
  public void CalculatePointsForReceipt_AmountWithRoundDollarNoCents_Returns50OrMore()
  {
    var pointsCalculator = new PointsCalculator();
    // Arrange
    var receipt = new ReceiptEntity
    {
      Retailer = "Walmart",
      Total = "70.00",
      Items = new[]
      {
        new ItemEntity
        {
          ShortDescription = "Snickers",
          Price = "1.99"
        },
        new ItemEntity
        {
          ShortDescription = "Sneakers",
          Price = "68.01"
        }
      }
    };

    // Act
    var points = pointsCalculator.CalculatePointsForReceipt(receipt);

    // Assert that the points are 50 or more
    Assert.True(points >= 50);
  }
}
