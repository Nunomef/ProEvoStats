﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProEvoStats_EVO7.Data;

#nullable disable

namespace ProEvoStats_EVO7.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240407140756_Migration4")]
    partial class Migration4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.2.24128.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Campeonato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Data")
                        .HasColumnType("date");

                    b.Property<string>("Descricao")
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TemporadaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TemporadaId");

                    b.ToTable("Campeonatos");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Equipa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<string>("Pais")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.HasKey("Id");

                    b.ToTable("Equipas");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Jogador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EquipaPrefId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.HasKey("Id");

                    b.HasIndex("EquipaPrefId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Jogadores");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Jogo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CampeonatoId")
                        .HasColumnType("int");

                    b.Property<int?>("EquipaCasaId")
                        .HasColumnType("int");

                    b.Property<int?>("EquipaForaId")
                        .HasColumnType("int");

                    b.Property<int?>("ParelhaCasaId")
                        .HasColumnType("int");

                    b.Property<int?>("ParelhaForaId")
                        .HasColumnType("int");

                    b.Property<int?>("ResultadoCasa")
                        .HasColumnType("int");

                    b.Property<int?>("ResultadoFora")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CampeonatoId");

                    b.HasIndex("EquipaCasaId");

                    b.HasIndex("EquipaForaId");

                    b.HasIndex("ParelhaCasaId");

                    b.HasIndex("ParelhaForaId");

                    b.ToTable("Jogos");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Parelha", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Jogador1Id")
                        .HasColumnType("int");

                    b.Property<int>("Jogador2Id")
                        .HasColumnType("int");

                    b.Property<int>("TemporadaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Jogador1Id");

                    b.HasIndex("Jogador2Id");

                    b.HasIndex("TemporadaId");

                    b.ToTable("Parelhas");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Temporada", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<short>("Ano")
                        .HasColumnType("smallint");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Temporadas");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Campeonato", b =>
                {
                    b.HasOne("ProEvoStats_EVO7.Models.Temporada", "Temporada")
                        .WithMany("Campeonatos")
                        .HasForeignKey("TemporadaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Temporada");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Jogador", b =>
                {
                    b.HasOne("ProEvoStats_EVO7.Models.Equipa", "EquipaPref")
                        .WithMany()
                        .HasForeignKey("EquipaPrefId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("EquipaPref");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Jogo", b =>
                {
                    b.HasOne("ProEvoStats_EVO7.Models.Campeonato", "Campeonato")
                        .WithMany("Jogos")
                        .HasForeignKey("CampeonatoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ProEvoStats_EVO7.Models.Equipa", "EquipaCasa")
                        .WithMany()
                        .HasForeignKey("EquipaCasaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ProEvoStats_EVO7.Models.Equipa", "EquipaFora")
                        .WithMany()
                        .HasForeignKey("EquipaForaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ProEvoStats_EVO7.Models.Parelha", "ParelhaCasa")
                        .WithMany()
                        .HasForeignKey("ParelhaCasaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ProEvoStats_EVO7.Models.Parelha", "ParelhaFora")
                        .WithMany()
                        .HasForeignKey("ParelhaForaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Campeonato");

                    b.Navigation("EquipaCasa");

                    b.Navigation("EquipaFora");

                    b.Navigation("ParelhaCasa");

                    b.Navigation("ParelhaFora");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Parelha", b =>
                {
                    b.HasOne("ProEvoStats_EVO7.Models.Jogador", "Jogador1")
                        .WithMany("ParelhasJogador1")
                        .HasForeignKey("Jogador1Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ProEvoStats_EVO7.Models.Jogador", "Jogador2")
                        .WithMany("ParelhasJogador2")
                        .HasForeignKey("Jogador2Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ProEvoStats_EVO7.Models.Temporada", "Temporada")
                        .WithMany("Parelhas")
                        .HasForeignKey("TemporadaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Jogador1");

                    b.Navigation("Jogador2");

                    b.Navigation("Temporada");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Campeonato", b =>
                {
                    b.Navigation("Jogos");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Jogador", b =>
                {
                    b.Navigation("ParelhasJogador1");

                    b.Navigation("ParelhasJogador2");
                });

            modelBuilder.Entity("ProEvoStats_EVO7.Models.Temporada", b =>
                {
                    b.Navigation("Campeonatos");

                    b.Navigation("Parelhas");
                });
#pragma warning restore 612, 618
        }
    }
}
