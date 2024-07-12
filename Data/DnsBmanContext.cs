using DnsBman.Models;
using DnsBman.Models.IdentityModels;
using DnsBman.Utilities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DnsBman.Data;

public partial class DnsBmanContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public DnsBmanContext()
    {
    }

    public DnsBmanContext(DbContextOptions<DnsBmanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UsersApiKey> UsersApiKeys { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=DnsBman;Integrated Security=True;TrustServerCertificate=True;"); //locale
        //=> optionsBuilder.UseSqlServer(ConfigurationHandler.GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsersApiKey>(entity =>
        {
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IdUser)
                .HasMaxLength(450)
                .HasColumnName("Id_User");
            entity.Property(e => e.Validity).HasDefaultValue(true);

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.ApiKey)
                .HasForeignKey<UsersApiKey>(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ApiKeys_AspNetUsers");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Customer");

            entity.ToTable(tb => tb.HasTrigger("UpdateModificationDate"));

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IdRecordBmanIt).HasColumnName("Id_RecordBmanIt");
            entity.Property(e => e.IdRecordBmanShop).HasColumnName("Id_RecordBmanShop");
            entity.Property(e => e.ModificationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ValueBmanIt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ValueBmanShop)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
