using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProEvoStats_EVO7.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId",
                table: "Campeonatos");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipas_Jogadores_JogadorId",
                table: "Equipas");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipas_Jogos_JogoId",
                table: "Equipas");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogadores_Equipas_EquipaPrefId",
                table: "Jogadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogadores_Parelhas_ParelhaId",
                table: "Jogadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaCasaId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaForaId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Jogos_JogoId",
                table: "Parelhas");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId",
                table: "Parelhas");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadasId",
                table: "Parelhas");

            migrationBuilder.DropIndex(
                name: "IX_Parelhas_JogoId",
                table: "Parelhas");

            migrationBuilder.DropIndex(
                name: "IX_Jogadores_ParelhaId",
                table: "Jogadores");

            migrationBuilder.DropIndex(
                name: "IX_Equipas_JogadorId",
                table: "Equipas");

            migrationBuilder.DropIndex(
                name: "IX_Equipas_JogoId",
                table: "Equipas");

            migrationBuilder.DropColumn(
                name: "JogoId",
                table: "Parelhas");

            migrationBuilder.DropColumn(
                name: "ParelhaId",
                table: "Jogadores");

            migrationBuilder.DropColumn(
                name: "JogadorId",
                table: "Equipas");

            migrationBuilder.DropColumn(
                name: "JogoId",
                table: "Equipas");

            migrationBuilder.RenameColumn(
                name: "TemporadasId",
                table: "Parelhas",
                newName: "TemporadaId1");

            migrationBuilder.RenameIndex(
                name: "IX_Parelhas_TemporadasId",
                table: "Parelhas",
                newName: "IX_Parelhas_TemporadaId1");

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
                name: "IX_Jogos_CampeonatoId1",
                table: "Jogos",
                column: "CampeonatoId1");

            migrationBuilder.CreateIndex(
                name: "IX_Campeonatos_TemporadaId1",
                table: "Campeonatos",
                column: "TemporadaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId",
                table: "Campeonatos",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId1",
                table: "Campeonatos",
                column: "TemporadaId1",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jogadores_Equipas_EquipaPrefId",
                table: "Jogadores",
                column: "EquipaPrefId",
                principalTable: "Equipas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId",
                table: "Jogos",
                column: "CampeonatoId",
                principalTable: "Campeonatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId1",
                table: "Jogos",
                column: "CampeonatoId1",
                principalTable: "Campeonatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaCasaId",
                table: "Jogos",
                column: "ParelhaCasaId",
                principalTable: "Parelhas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaForaId",
                table: "Jogos",
                column: "ParelhaForaId",
                principalTable: "Parelhas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId",
                table: "Parelhas",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId1",
                table: "Parelhas",
                column: "TemporadaId1",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId",
                table: "Campeonatos");

            migrationBuilder.DropForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId1",
                table: "Campeonatos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogadores_Equipas_EquipaPrefId",
                table: "Jogadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId1",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaCasaId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Parelhas_ParelhaForaId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId",
                table: "Parelhas");

            migrationBuilder.DropForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId1",
                table: "Parelhas");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_CampeonatoId1",
                table: "Jogos");

            migrationBuilder.DropIndex(
                name: "IX_Campeonatos_TemporadaId1",
                table: "Campeonatos");

            migrationBuilder.DropColumn(
                name: "CampeonatoId1",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "TemporadaId1",
                table: "Campeonatos");

            migrationBuilder.RenameColumn(
                name: "TemporadaId1",
                table: "Parelhas",
                newName: "TemporadasId");

            migrationBuilder.RenameIndex(
                name: "IX_Parelhas_TemporadaId1",
                table: "Parelhas",
                newName: "IX_Parelhas_TemporadasId");

            migrationBuilder.AddColumn<int>(
                name: "JogoId",
                table: "Parelhas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParelhaId",
                table: "Jogadores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JogadorId",
                table: "Equipas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JogoId",
                table: "Equipas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parelhas_JogoId",
                table: "Parelhas",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_ParelhaId",
                table: "Jogadores",
                column: "ParelhaId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipas_JogadorId",
                table: "Equipas",
                column: "JogadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipas_JogoId",
                table: "Equipas",
                column: "JogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campeonatos_Temporadas_TemporadaId",
                table: "Campeonatos",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Jogadores_Equipas_EquipaPrefId",
                table: "Jogadores",
                column: "EquipaPrefId",
                principalTable: "Equipas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogadores_Parelhas_ParelhaId",
                table: "Jogadores",
                column: "ParelhaId",
                principalTable: "Parelhas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Campeonatos_CampeonatoId",
                table: "Jogos",
                column: "CampeonatoId",
                principalTable: "Campeonatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Parelhas_Jogos_JogoId",
                table: "Parelhas",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadaId",
                table: "Parelhas",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Parelhas_Temporadas_TemporadasId",
                table: "Parelhas",
                column: "TemporadasId",
                principalTable: "Temporadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
