using Microsoft.EntityFrameworkCore;

public  class ReceiptsApiDbContext : DbContext
{
  public ReceiptsApiDbContext(DbContextOptions<ReceiptsApiDbContext> options)
            : base(options)
  { }

  public DbSet<ReceiptEntity> Receipts { get; set; }
  public DbSet<ItemEntity> Items { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ReceiptEntity>()
        .HasMany(r => r.Items)
        .WithOne(i => i.Receipt)
        .HasForeignKey(i => i.ReceiptEntityId);

    base.OnModelCreating(modelBuilder);
  }
}
