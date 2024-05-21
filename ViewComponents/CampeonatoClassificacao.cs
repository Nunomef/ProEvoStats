using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.ViewModels;

namespace ProEvoStats_EVO7.ViewComponents
{
    public class CampeonatoClassificacao : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CampeonatoClassificacao(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int campeonatoId)
        {
            var campeonato = _context.Campeonatos.Find(campeonatoId);
            ViewData["DescricaoCampeonato"] = campeonato?.Descricao ?? "Campeonato desconhecido";

            var resultados = _context.Jogos
                .Where(j => j.CampeonatoId == campeonatoId && j.ResultadoCasa != null && j.ResultadoFora != null && j.ParelhaCasa != null && j.ParelhaFora != null)
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

            var resultadosViewModel = resultados.Select(r => new CampeonatoClassificacaoViewModel
            {
                Username = _context.Jogadores.Find(r.JogadorId).Username,
                JogosDisputados = r.JogosDisputados,
                Vitorias = r.Vitorias,
                Empates = r.Empates,
                Derrotas = r.Derrotas,
                GolosMarcados = r.GolosMarcados ?? 0,
                GolosSofridos = r.GolosSofridos ?? 0,
                DiferencaDeGolos = r.DiferencaDeGolos ?? 0,
                Pontos = r.Pontos
            }).ToList();

            ViewData["CampeonatoId"] = campeonatoId;
            return View(resultadosViewModel);
        }
    }
}
