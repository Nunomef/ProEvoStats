using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.Models;

namespace ProEvoStats_EVO7.ViewComponents
{
    public class JogadorTemporadas : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public JogadorTemporadas(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jogadorId, int temporadaId)
        {
            var campeonatos = _context.Campeonatos
                .Where(c => c.TemporadaId == temporadaId)
                .Select(c => c.Id)
                .ToList();

            var resultados = _context.Jogos
                .Where(j => campeonatos.Contains(j.CampeonatoId) && j.ResultadoCasa != null && j.ResultadoFora != null)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador2)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador2)
                .ToList()
                .SelectMany(j => new[] {
            new { JogadorId = j.ParelhaCasa.Jogador1Id, GolosMarcados = j.ResultadoCasa, GolosSofridos = j.ResultadoFora, Vitoria = j.ResultadoCasa > j.ResultadoFora ? 1 : 0, Empate = j.ResultadoCasa == j.ResultadoFora ? 1 : 0 },
            new { JogadorId = j.ParelhaCasa.Jogador2Id, GolosMarcados = j.ResultadoCasa, GolosSofridos = j.ResultadoFora, Vitoria = j.ResultadoCasa > j.ResultadoFora ? 1 : 0, Empate = j.ResultadoCasa == j.ResultadoFora ? 1 : 0 },
            new { JogadorId = j.ParelhaFora.Jogador1Id, GolosMarcados = j.ResultadoFora, GolosSofridos = j.ResultadoCasa, Vitoria = j.ResultadoFora > j.ResultadoCasa ? 1 : 0, Empate = j.ResultadoFora == j.ResultadoCasa ? 1 : 0 },
            new { JogadorId = j.ParelhaFora.Jogador2Id, GolosMarcados = j.ResultadoFora, GolosSofridos = j.ResultadoCasa, Vitoria = j.ResultadoFora > j.ResultadoCasa ? 1 : 0, Empate = j.ResultadoFora == j.ResultadoCasa ? 1 : 0 },
                })
                .GroupBy(r => r.JogadorId)
                .Select(g => new
                {
                    JogadorId = g.Key,
                    _context.Jogadores.Find(g.Key).Username,
                    JogosDisputados = g.Count(),
                    Vitorias = g.Sum(r => r.Vitoria),
                    Empates = g.Sum(r => r.Empate),
                    Derrotas = g.Count() - g.Sum(r => r.Vitoria) - g.Sum(r => r.Empate),
                    GolosMarcados = g.Sum(r => r.GolosMarcados),
                    GolosSofridos = g.Sum(r => r.GolosSofridos),
                    DiferencaDeGolos = g.Sum(r => r.GolosMarcados) - g.Sum(r => r.GolosSofridos),
                    CampeonatosGanhos = campeonatos.Count(c => GetCampeonatoVencedorId(c) == g.Key)
                })
                .OrderByDescending(r => r.CampeonatosGanhos)
                .ThenByDescending(r => r.DiferencaDeGolos)
                .ThenByDescending(r => r.GolosMarcados)
                .ThenBy(r => r.GolosSofridos)
                .ToList();

            var jogador = resultados.Where(r => r.JogadorId == jogadorId).FirstOrDefault();

            var viewModel = new JogadorTemporadaViewModel
            {
                Username = jogador.Username,
                Posicao = resultados.IndexOf(jogador) + 1,
                CampeonatosGanhos = jogador.CampeonatosGanhos,
                JogosDisputados = jogador.JogosDisputados,
                Vitorias = jogador.Vitorias,
                Empates = jogador.Empates,
                Derrotas = jogador.Derrotas,
                GolosMarcados = jogador.GolosMarcados ?? 0,
                GolosSofridos = jogador.GolosSofridos ?? 0,
                DiferencaDeGolos = jogador.DiferencaDeGolos ?? 0,
                EquipaMaisUsada = GetEquipaMaisUsada(jogadorId, temporadaId),
            };

            return View(viewModel);
        }


        // MÉTODOS AUXILIARES ============================================================================
        // para obter o ID do vencedor de um campeonato
        private int GetCampeonatoVencedorId(int campeonatoId)
        {
            var resultados = _context.Jogos
                .Where(j => j.CampeonatoId == campeonatoId)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador2)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador2)
                .ToList()
                .SelectMany(j => new[] {
                new { JogadorId = j.ParelhaCasa.Jogador1Id, GolosMarcados = j.ResultadoCasa, GolosSofridos = j.ResultadoFora, Vitoria = j.ResultadoCasa > j.ResultadoFora ? 1 : 0, Empate = j.ResultadoCasa == j.ResultadoFora ? 1 : 0 },
                new { JogadorId = j.ParelhaCasa.Jogador2Id, GolosMarcados = j.ResultadoCasa, GolosSofridos = j.ResultadoFora, Vitoria = j.ResultadoCasa > j.ResultadoFora ? 1 : 0, Empate = j.ResultadoCasa == j.ResultadoFora ? 1 : 0 },
                new { JogadorId = j.ParelhaFora.Jogador1Id, GolosMarcados = j.ResultadoFora, GolosSofridos = j.ResultadoCasa, Vitoria = j.ResultadoFora > j.ResultadoCasa ? 1 : 0, Empate = j.ResultadoFora == j.ResultadoCasa ? 1 : 0 },
                new { JogadorId = j.ParelhaFora.Jogador2Id, GolosMarcados = j.ResultadoFora, GolosSofridos = j.ResultadoCasa, Vitoria = j.ResultadoFora > j.ResultadoCasa ? 1 : 0, Empate = j.ResultadoFora == j.ResultadoCasa ? 1 : 0 },
                })
                .GroupBy(r => r.JogadorId)
                .Select(g => new
                {
                    JogadorId = g.Key,
                    JogosDisputados = g.Count(),
                    Vitorias = g.Sum(r => r.Vitoria),
                    Empates = g.Sum(r => r.Empate),
                    Derrotas = g.Count() - g.Sum(r => r.Vitoria) - g.Sum(r => r.Empate),
                    GolosMarcados = g.Sum(r => r.GolosMarcados),
                    GolosSofridos = g.Sum(r => r.GolosSofridos),
                    DiferencaDeGolos = g.Sum(r => r.GolosMarcados) - g.Sum(r => r.GolosSofridos),
                    Pontos = g.Sum(r => r.Vitoria) * 3 + g.Sum(r => r.Empate)
                })
                .OrderByDescending(r => r.Pontos)
                .ThenByDescending(r => r.DiferencaDeGolos)
                .ThenByDescending(r => r.GolosMarcados)
                .ThenBy(r => r.GolosSofridos)
                .ToList();

            var vencedor = resultados
            .OrderByDescending(r => r.Pontos)
            .ThenByDescending(r => r.DiferencaDeGolos)
            .ThenByDescending(r => r.GolosMarcados)
            .ThenBy(r => r.GolosSofridos)
            .FirstOrDefault();

            return vencedor?.JogadorId ?? 0;
        }
        // para obter a equipa mais usada por um jogador
        private string GetEquipaMaisUsada(int jogadorId, int temporadaId)
        {
            var campeonatos = _context.Campeonatos
                .Where(c => c.TemporadaId == temporadaId)
                .Select(c => c.Id)
                .ToList();

            var equipas = _context.Jogos
                .Where(j => campeonatos.Contains(j.CampeonatoId) && (j.ParelhaCasa.Jogador1Id == jogadorId || j.ParelhaCasa.Jogador2Id == jogadorId || j.ParelhaFora.Jogador1Id == jogadorId || j.ParelhaFora.Jogador2Id == jogadorId))
                .Select(j => j.ParelhaCasa.Jogador1Id == jogadorId || j.ParelhaCasa.Jogador2Id == jogadorId ? j.EquipaCasa.Nome : j.EquipaFora.Nome)
                .ToList();

            var equipaMaisUsada = equipas
                .GroupBy(e => e)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return equipaMaisUsada;
        }
    }
}
