using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.Models;
using ProEvoStats_EVO7.ViewModels;

namespace ProEvoStats_EVO7.ViewComponents
{
    public class TemporadaClassificacao: ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public TemporadaClassificacao(ApplicationDbContext context)
        {
            _context = context;
        }
       
        public async Task<IViewComponentResult> InvokeAsync(int temporadaId)
        {
            var temporada = _context.Temporadas.Find(temporadaId);
            ViewData["DescricaoTemporada"] = temporada?.Descricao ?? "Temporada desconhecida";

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
                    JogosDisputados = g.Count(),
                    Vitorias = g.Sum(r => r.Vitoria),
                    Empates = g.Sum(r => r.Empate),
                    Derrotas = g.Count() - g.Sum(r => r.Vitoria) - g.Sum(r => r.Empate),
                    GolosMarcados = g.Sum(r => r.GolosMarcados),
                    GolosSofridos = g.Sum(r => r.GolosSofridos),
                    DiferencaDeGolos = g.Sum(r => r.GolosMarcados) - g.Sum(r => r.GolosSofridos),
                    CampeonatosGanhos = GetTotalCampeonatosVencidos(g.Key, temporadaId)
                })
                .OrderByDescending(r => r.CampeonatosGanhos)
                .ThenByDescending(r => r.DiferencaDeGolos)
                .ThenByDescending(r => r.GolosMarcados)
                .ThenBy(r => r.GolosSofridos)
                .ToList();

            var resultadosViewModel = resultados.Select(r => new TemporadaClassificacaoViewModel
            {
                Username = _context.Jogadores.Find(r.JogadorId).Username,
                JogosDisputados = r.JogosDisputados,
                Vitorias = r.Vitorias,
                Empates = r.Empates,
                Derrotas = r.Derrotas,
                GolosMarcados = r.GolosMarcados ?? 0,
                GolosSofridos = r.GolosSofridos ?? 0,
                DiferencaDeGolos = r.DiferencaDeGolos ?? 0,
                CampeonatosGanhos = r.CampeonatosGanhos,
                EquipaMaisUsada = GetEquipaMaisUsada(r.JogadorId, temporadaId)
            }).ToList();

            ViewData["TemporadaId"] = temporadaId;
            return View(resultadosViewModel);
        }






        // MÉTODO AUXILIAR =============================================================================
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

        private int GetTotalCampeonatosVencidos(int jogadorId, int temporadaId)
        {
            var campeonatos = _context.Campeonatos
                .Where(c => c.Status == Status.Inactive && c.TemporadaId == temporadaId)
                .ToList(); // traz os dados para a memória

            var campeonatosVencidos = campeonatos
                .Where(c => GetCampeonatoVencedorId(c.Id) == jogadorId)
                .Count();

            return campeonatosVencidos;
        }

        // para obter a equipa mais usada por um jogador
        private string GetEquipaMaisUsada(int jogadorId, int temporadaId)
        {
            var campeonatos = _context.Campeonatos
                .Where(c => c.TemporadaId == temporadaId)
                .Select(c => c.Id)
                .ToList();

            var equipas = _context.Jogos
                .Where(j => campeonatos.Contains(j.CampeonatoId) && (j.ParelhaCasa.Jogador1Id == jogadorId || j.ParelhaCasa.Jogador2Id == jogadorId || j.ParelhaFora.Jogador1Id == jogadorId || j.ParelhaFora.Jogador2Id == jogadorId) && j.EquipaCasa != null && j.EquipaFora != null)
                .Select(j => j.ParelhaCasa.Jogador1Id == jogadorId || j.ParelhaCasa.Jogador2Id == jogadorId ? j.EquipaCasa.Nome : j.EquipaFora.Nome)
                .ToList();

             var equipaMaisUsada = equipas
                .GroupBy(e => e)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Equipa = g.Key, Contagem = g.Count() })
                .FirstOrDefault();

            return equipaMaisUsada == null ? null : $"{equipaMaisUsada.Equipa} ({equipaMaisUsada.Contagem})";

        }
    }
}
