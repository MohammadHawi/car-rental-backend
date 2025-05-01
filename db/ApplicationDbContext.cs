using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSet for each model
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Nationality> Nationality { get; set; }
    

    // Configure relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Foreign key relationships
        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Car) // Each contract refers to one car
            .WithMany(c => c.Contracts) // A car can have many contracts
            .HasForeignKey(c => c.CarId); // Reference the CarId in Contract (not Plate)

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Customer) // Each contract refers to one customer
            .WithMany(c => c.Contracts) // A customer can have many contracts
            .HasForeignKey(c => c.Cid); // Reference the Cid in Contract

        // Additional constraints or configurations (if needed)
        modelBuilder.Entity<Contract>()
            .Property(c => c.Price)
            .HasDefaultValue(0); // Example of setting a default value for price

        modelBuilder.Entity<Contract>()
            .Property(c => c.Returned)
            .HasDefaultValue(0); // Example of default for returned flag
        
        modelBuilder.Entity<Customer>()
        .HasIndex(c => c.PhoneNumber)
        .IsUnique();

        modelBuilder.Entity<Customer>()
        .HasIndex(c => new { c.FirstName, c.MiddleName, c.LastName })
        .IsUnique();
    }
}
