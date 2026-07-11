using Microsoft.EntityFrameworkCore;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Associado> Associados => Set<Associado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Associado>(entity =>
        {
            entity.ToTable("associados");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Nome)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(a => a.Cpf)
                .HasConversion(cpf => cpf.Value, value => new Cpf(value))
                .HasMaxLength(11)
                .IsRequired();

            entity.HasIndex(a => a.Cpf).IsUnique();

            entity.Property(a => a.Placa)
                .HasConversion(placa => placa.Value, value => new Placa(value))
                .HasMaxLength(7)
                .IsRequired();

            entity.Property(a => a.Telefone)
                .HasMaxLength(20)
                .IsRequired();

            entity.OwnsOne(a => a.Endereco, endereco =>
            {
                endereco.Property(e => e.Logradouro).HasMaxLength(200).IsRequired();
                endereco.Property(e => e.Numero).HasMaxLength(20).IsRequired();
                endereco.Property(e => e.Complemento).HasMaxLength(100);
                endereco.Property(e => e.Bairro).HasMaxLength(100).IsRequired();
                endereco.Property(e => e.Cidade).HasMaxLength(100).IsRequired();
                endereco.Property(e => e.Uf).HasMaxLength(2).IsFixedLength().IsRequired();
                endereco.Property(e => e.Cep).HasMaxLength(8).IsRequired();
            });

            entity.Navigation(a => a.Endereco).IsRequired();
        });
    }
}
