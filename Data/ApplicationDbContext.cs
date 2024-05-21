using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Models;

namespace ProEvoStats_EVO7.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Campeonato> Campeonatos { get; set; }
        public DbSet<Equipa> Equipas { get; set; }
        public DbSet<Jogador> Jogadores { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Parelha> Parelhas { get; set; }
        public DbSet<Temporada> Temporadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração tabela Jogador=======================================
            modelBuilder.Entity<Jogador>()
                .HasIndex(j => j.Username)
                .IsUnique();

            modelBuilder.Entity<Jogador>()
                .Property(j => j.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Jogador>()
                .Property(j => j.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Jogador>()
                .HasOne<Equipa>("EquipaPref")
                .WithMany()
                .HasForeignKey("EquipaPrefId")
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração tabela Jogo=======================================
            modelBuilder.Entity<Jogo>()
                .HasOne<Equipa>("EquipaCasa")
                .WithMany()
                .HasForeignKey("EquipaCasaId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Jogo>()
                .HasOne<Equipa>("EquipaFora")
                .WithMany()
                .HasForeignKey("EquipaForaId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Jogo>()
                .HasOne<Parelha>("ParelhaCasa")
                .WithMany()
                .HasForeignKey("ParelhaCasaId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Jogo>()
                .HasOne<Parelha>("ParelhaFora")
                .WithMany()
                .HasForeignKey("ParelhaForaId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Jogo>()
                .HasOne<Campeonato>("Campeonato")
                .WithMany("Jogos")
                .HasForeignKey(j => j.CampeonatoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração tabela Parelha=======================================
            modelBuilder.Entity<Parelha>()
                .HasOne<Jogador>("Jogador1")
                .WithMany("ParelhasJogador1")
                .HasForeignKey("Jogador1Id")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parelha>()
                .HasOne<Jogador>("Jogador2")
                .WithMany("ParelhasJogador2")
                .HasForeignKey("Jogador2Id")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parelha>()
                .HasOne<Temporada>("Temporada")
                .WithMany("Parelhas")
                .HasForeignKey(p => p.TemporadaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração tabela Campeonato=======================================
            modelBuilder.Entity<Campeonato>()
                .HasOne<Temporada>("Temporada")
                .WithMany("Campeonatos")
                .HasForeignKey(c => c.TemporadaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Campeonato>()
                .Property(c => c.Status)
                .HasConversion<string>();

            // Configuração tabela Temporada=======================================
            modelBuilder.Entity<Temporada>()
                .Property(t => t.Status)
                .HasConversion<string>();
        }
    }
}
