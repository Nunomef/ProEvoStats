using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProEvoStats_EVO7.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Temporadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ano = table.Column<short>(type: "smallint", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temporadas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campeonatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    TemporadaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campeonatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campeonatos_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    JogadorId = table.Column<int>(type: "int", nullable: true),
                    JogoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jogadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipaPrefId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParelhaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogadores_Equipas_EquipaPrefId",
                        column: x => x.EquipaPrefId,
                        principalTable: "Equipas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Jogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResultadoCasa = table.Column<int>(type: "int", nullable: true),
                    ResultadoFora = table.Column<int>(type: "int", nullable: true),
                    ParelhaCasaId = table.Column<int>(type: "int", nullable: true),
                    ParelhaForaId = table.Column<int>(type: "int", nullable: true),
                    EquipaCasaId = table.Column<int>(type: "int", nullable: true),
                    EquipaForaId = table.Column<int>(type: "int", nullable: true),
                    CampeonatoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogos_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jogos_Equipas_EquipaCasaId",
                        column: x => x.EquipaCasaId,
                        principalTable: "Equipas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jogos_Equipas_EquipaForaId",
                        column: x => x.EquipaForaId,
                        principalTable: "Equipas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parelhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jogador1Id = table.Column<int>(type: "int", nullable: false),
                    Jogador2Id = table.Column<int>(type: "int", nullable: false),
                    TemporadasId = table.Column<int>(type: "int", nullable: false),
                    TemporadaId = table.Column<int>(type: "int", nullable: false),
                    JogoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parelhas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parelhas_Jogadores_Jogador1Id",
                        column: x => x.Jogador1Id,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parelhas_Jogadores_Jogador2Id",
                        column: x => x.Jogador2Id,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parelhas_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parelhas_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parelhas_Temporadas_TemporadasId",
                        column: x => x.TemporadasId,
                        principalTable: "Temporadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campeonatos_TemporadaId",
                table: "Campeonatos",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipas_JogadorId",
                table: "Equipas",
                column: "JogadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipas_JogoId",
                table: "Equipas",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_EquipaPrefId",
                table: "Jogadores",
                column: "EquipaPrefId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_ParelhaId",
                table: "Jogadores",
                column: "ParelhaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_Username",
                table: "Jogadores",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_CampeonatoId",
                table: "Jogos",
                column: "CampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EquipaCasaId",
                table: "Jogos",
                column: "EquipaCasaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EquipaForaId",
                table: "Jogos",
                column: "EquipaForaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_ParelhaCasaId",
                table: "Jogos",
                column: "ParelhaCasaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_ParelhaForaId",
                table: "Jogos",
                column: "ParelhaForaId");

            migrationBuilder.CreateIndex(
                name: "IX_Parelhas_Jogador1Id",
                table: "Parelhas",
                column: "Jogador1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Parelhas_Jogador2Id",
                table: "Parelhas",
                column: "Jogador2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Parelhas_JogoId",
                table: "Parelhas",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Parelhas_TemporadaId",
                table: "Parelhas",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Parelhas_TemporadasId",
                table: "Parelhas",
                column: "TemporadasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipas_Jogadores_JogadorId",
                table: "Equipas",
                column: "JogadorId",
                principalTable: "Jogadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipas_Jogos_JogoId",
                table: "Equipas",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogadores_Parelhas_ParelhaId",
                table: "Jogadores",
                column: "ParelhaId",
                principalTable: "Parelhas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaCasaId",
                table: "Jogos",
                column: "ParelhaCasaId",
                principalTable: "Parelhas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaForaId",
                table: "Jogos",
                column: "ParelhaForaId",
                principalTable: "Parelhas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId",
                table: "Campeonatos");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId",
                table: "Parelhas");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadasId",
                table: "Parelhas");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipas_Jogadores_JogadorId",
                table: "Equipas");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Jogadores_Jogador1Id",
                table: "Parelhas");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Jogadores_Jogador2Id",
                table: "Parelhas");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipas_Jogos_JogoId",
                table: "Equipas");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Jogos_JogoId",
                table: "Parelhas");

            migrationBuilder.DropTable(
                name: "Temporadas");

            migrationBuilder.DropTable(
                name: "Jogadores");

            migrationBuilder.DropTable(
                name: "Jogos");

            migrationBuilder.DropTable(
                name: "Campeonatos");

            migrationBuilder.DropTable(
                name: "Equipas");

            migrationBuilder.DropTable(
                name: "Parelhas");
        }
    }
}
