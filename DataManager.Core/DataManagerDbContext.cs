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

    public DbSet<DataModelOne> DataModelOnes { get; set; }
    public DbSet<DataModelTwo> DataModelTwos { get; set; }
    public DbSet<Exit> Exits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}