namespace MyProject.DataAccess;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(item => item.Description)
                .HasMaxLength(2000);
            entity.Property(item => item.Price)
                .HasPrecision(18, 2);
            entity.Property(item => item.ItemState)
                .HasConversion<int>();
        });
    }
}
