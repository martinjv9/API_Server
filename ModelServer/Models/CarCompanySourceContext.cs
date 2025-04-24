using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ModelServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ModelServer.Models;

public partial class CarCompanySourceContext : IdentityDbContext<CarAppUser>
{
    public CarCompanySourceContext()
    {
    }

    public CarCompanySourceContext(DbContextOptions<CarCompanySourceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarCompany> CarCompanies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2MGQ8LS;Database=CarORM;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>(entity =>
        {
            entity.ToTable("Car");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarCompany).HasColumnName("carCompany");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("model");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.CarCompanyNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarCompany)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Car_CarCompany");
        });

        modelBuilder.Entity<CarCompany>(entity =>
        {
            entity.ToTable("CarCompany");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryOrigin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("countryOrigin");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
