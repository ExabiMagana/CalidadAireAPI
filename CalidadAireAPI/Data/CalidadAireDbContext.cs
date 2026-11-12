using CalidadAireAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalidadAireAPI.Data
{
    public class CalidadAireDbContext : DbContext
    {
        public CalidadAireDbContext(
            DbContextOptions<CalidadAireDbContext> options)
            : base(options)
        {
        }

        public DbSet<SensorCalidadAire> Sensores => Set<SensorCalidadAire>();
        public DbSet<LecturaAire> LecturasAire => Set<LecturaAire>();
        public DbSet<AlertaAire> AlertasAire => Set<AlertaAire>();
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SensorCalidadAire>(entity =>
            {
                entity.ToTable("sensores_calidad_aire");

                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.Ubicacion)
                    .HasColumnName("ubicacion")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.TipoGas)
                    .HasColumnName("tipo_gas")
                    .HasMaxLength(80)
                    .IsRequired();

                entity.Property(x => x.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(20)
                    .HasDefaultValue("Activo")
                    .IsRequired();
            });

            modelBuilder.Entity<LecturaAire>(entity =>
            {
                entity.ToTable("lecturas_aire");

                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.SensorId)
                    .HasColumnName("sensor_id");

                entity.Property(x => x.PM2_5)
                    .HasColumnName("pm2_5")
                    .HasPrecision(10, 2);

                entity.Property(x => x.PM10)
                    .HasColumnName("pm10")
                    .HasPrecision(10, 2);

                entity.Property(x => x.CO2)
                    .HasColumnName("co2")
                    .HasPrecision(10, 2);

                entity.Property(x => x.FechaHora)
                    .HasColumnName("fecha_hora")
                    .HasColumnType("datetime(6)");

                entity.HasOne(x => x.Sensor)
                    .WithMany(x => x.Lecturas)
                    .HasForeignKey(x => x.SensorId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<AlertaAire>(entity =>
            {
                entity.ToTable("alertas_aire");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.SensorId)
                    .HasColumnName("sensor_id");

                entity.Property(x => x.Nivel)
                    .HasColumnName("nivel")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.Mensaje)
                    .HasColumnName("mensaje")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.FechaHora)
                    .HasColumnName("fecha_hora")
                    .HasColumnType("datetime(6)");

                entity.HasOne(x => x.Sensor)
                    .WithMany(x => x.Alertas)
                    .HasForeignKey(x => x.SensorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.UsuarioNombre)
                    .HasColumnName("usuario")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ContrasenaHash)
                    .HasColumnName("contrasena_hash")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.HasIndex(x => x.UsuarioNombre)
                    .IsUnique();
            });
        }
    }
}