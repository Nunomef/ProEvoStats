using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProEvoStats_EVO7.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId1",
                table: "Campeonatos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId1",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId1",
                table: "Parelhas");

            migrationBuilder.DropIndex(
                name: "IX_Parelhas_TemporadaId1",
                table: "Parelhas");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_CampeonatoId1",
                table: "Jogos");

            migrationBuilder.DropIndex(
                name: "IX_Campeonatos_TemporadaId1",
                table: "Campeonatos");

            migrationBuilder.DropColumn(
                name: "TemporadaId1",
                table: "Parelhas");

            migrationBuilder.DropColumn(
                name: "CampeonatoId1",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "TemporadaId1",
                table: "Campeonatos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemporadaId1",
                table: "Parelhas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CampeonatoId1",
                table: "Jogos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TemporadaId1",
                table: "Campeonatos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Parelhas_TemporadaId1",
                table: "Parelhas",
                column: "TemporadaId1");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_CampeonatoId1",
                table: "Jogos",
                column: "CampeonatoId1");

            migrationBuilder.CreateIndex(
                name: "IX_Campeonatos_TemporadaId1",
                table: "Campeonatos",
                column: "TemporadaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId1",
                table: "Campeonatos",
                column: "TemporadaId1",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId1",
                table: "Jogos",
                column: "CampeonatoId1",
                principalTable: "Campeonatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId1",
                table: "Parelhas",
                column: "TemporadaId1",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
