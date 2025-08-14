using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AgendamentoAPI.Models;

namespace AgendamentoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets para todas as entidades
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Especialidade> Especialidades { get; set; }
        public DbSet<MedicoEspecialidade> MedicoEspecialidades { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as configurações que implementam IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configuração da herança Usuario -> Paciente/Medico
            modelBuilder.Entity<Usuario>()
                .HasDiscriminator<string>("Perfil")
                .HasValue<Paciente>("Paciente")
                .HasValue<Medico>("Medico");

            // Configuração da tabela Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("NOW()");
            });

            // Configuração da tabela Paciente
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasIndex(p => p.CPF).IsUnique();
                entity.Property(p => p.CPF).HasMaxLength(11);
            });

            // Configuração da tabela Medico
            modelBuilder.Entity<Medico>(entity =>
            {
                entity.HasIndex(m => m.CRM).IsUnique();
                entity.Property(m => m.CRM).HasMaxLength(10);
            });

            // Configuração da tabela Especialidade
            modelBuilder.Entity<Especialidade>(entity =>
            {
                entity.HasIndex(e => e.Nome).IsUnique();
                entity.Property(e => e.Nome).HasMaxLength(50);
            });

            // Configuração da tabela MedicoEspecialidade
            modelBuilder.Entity<MedicoEspecialidade>(entity =>
            {
                entity.HasKey(me => new { me.MedicoId, me.EspecialidadeId });

                entity.HasOne(me => me.Medico)
                    .WithMany(m => m.Especialidades)
                    .HasForeignKey(me => me.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(me => me.Especialidade)
                    .WithMany(e => e.Medicos)
                    .HasForeignKey(me => me.EspecialidadeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da tabela Consulta
            modelBuilder.Entity<Consulta>(entity =>
            {
                entity.Property(c => c.Status).HasDefaultValue("Agendada");
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("NOW()");
                entity.HasCheckConstraint("consulta_futura", "\"DataConsulta\" > NOW()");

                entity.HasOne(c => c.Medico)
                    .WithMany(m => m.Consultas)
                    .HasForeignKey(c => c.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Paciente)
                    .WithMany(p => p.Consultas)
                    .HasForeignKey(c => c.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Especialidade)
                    .WithMany()
                    .HasForeignKey(c => c.EspecialidadeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da tabela RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(rt => rt.Token).IsUnique();

                entity.HasOne(rt => rt.Usuario)
                    .WithMany()
                    .HasForeignKey(rt => rt.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}