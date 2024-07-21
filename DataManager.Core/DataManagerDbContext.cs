using DataManager.Core.DBModels;
using Microsoft.EntityFrameworkCore;

namespace DataManager.Core;

public class DataManagerDbContext : DbContext
{
    public DataManagerDbContext() : base()
    {

    }

    public DataManagerDbContext(DbContextOptions options) : base(options)
    {

    }

    public virtual DbSet<DataModelOne> DataModelOnes { get; set; }
    public virtual DbSet<DataModelTwo> DataModelTwos { get; set; }
    public virtual DbSet<Exit> Exits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Host=localhost;Port=5432;Database=DataManagerDB;Username=postgres;Password=admin;";

        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<DataModelOne>(entity =>
        {
            entity.HasOne(a => a.Exit).WithMany(e => e.DataModelOnes).HasForeignKey(a => a.ExitId).IsRequired();
            entity.Property(e => e.Port).IsRequired();
            entity.Property(e => e.UserGroup).IsRequired();
            entity.Property(e => e.Country).IsRequired();
            entity.Property(e => e.MemberId).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.GainAmountOne).IsRequired();
            entity.Property(e => e.GainAmountTwo).IsRequired();
            entity.Property(e => e.Loss).IsRequired();
            entity.Property(e => e.Total).IsRequired();

        });

        modelBuilder.Entity<DataModelTwo>(entity =>
        {
            entity.HasOne(f => f.Exit).WithMany(e => e.DataModelTwos).HasForeignKey(f => f.ExitId).IsRequired();
            entity.Property(e => e.PeriodStartDate).IsRequired();
            entity.Property(e => e.PeriodEndDate).IsRequired();
            entity.Property(e => e.GainAmountThree).IsRequired();
        });

        modelBuilder.Entity<Exit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
    }
}