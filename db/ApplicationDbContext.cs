// using CarRental.API.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ToDoTask> Tasks { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Nationality> Nationality { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CarStatus> CarStatuses { get; set; }

    // Configure relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>()
            .HasOne(c => c.CarStatus)
            .WithOne(cs => cs.Car)
            .HasForeignKey<CarStatus>(cs => cs.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure unique constraint on CarId in CarStatus table
        modelBuilder.Entity<CarStatus>()
            .HasIndex(cs => cs.CarId)
            .IsUnique();

    }
}
