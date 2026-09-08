using Microsoft.EntityFrameworkCore;
using MiApi.Domain.Entities;

namespace MiApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CompanyId).IsRequired();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PortalId).IsRequired();
                entity.Property(e => e.RoleId).IsRequired();
                entity.Property(e => e.StatusId).IsRequired();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Telephone).HasMaxLength(20);
                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.CreatedOn).IsRequired();
                entity.Property(e => e.UpdatedOn);
                entity.Property(e => e.DeletedOn);

                // Índices para búsquedas comunes
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.CompanyId);

                // 1. Filtro global: Ignora registros eliminados en todas las consultas (GET, SELECT)
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<Employee>();

            foreach (var entry in entries)
            {
                // 2. Interceptación de borrado lógico (DELETE -> UPDATE IsDeleted = true)
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedOn = DateTime.UtcNow;
                    entry.Entity.UpdatedOn = DateTime.UtcNow;
                }
                // 3. Auditoría de actualización
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedOn = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}