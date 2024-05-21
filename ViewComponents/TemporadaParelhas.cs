using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.ViewModels;

namespace ProEvoStats_EVO7.ViewComponents
{
    public class TemporadaParelhas : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public TemporadaParelhas(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int temporadaId)
        {
            var campeonatos = _context.Campeonatos
                .Where(c => c.TemporadaId == temporadaId)
                .Select(c => c.Id)
                .ToList();

            var resultados = _context.Jogos
                .Where(j => campeonatos.Contains(j.CampeonatoId) && j.ResultadoCasa != null && j.ResultadoFora != null)
                .ToList()
                .SelectMany(j => new[] {
                    new { ParelhaId = j.ParelhaCasaId, GolosMarcados = j.ResultadoCasa, GolosSofridos = j.ResultadoFora, Vitoria = j.ResultadoCasa > j.ResultadoFora ? 1 : 0, Empate = j.ResultadoCasa == j.ResultadoFora ? 1 : 0 },
                    new { ParelhaId = j.ParelhaForaId, GolosMarcados = j.ResultadoFora, GolosSofridos = j.ResultadoCasa, Vitoria = j.ResultadoFora > j.ResultadoCasa ? 1 : 0, Empate = j.ResultadoFora == j.ResultadoCasa ? 1 : 0 },
                })
                .GroupBy(r => r.ParelhaId)
                .Select(g => new
                {
                    ParelhaId = g.Key,
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

            var resultadosViewModel = resultados.Select(r => new TemporadaParelhasViewModel
            {
                Username1 = _context.Parelhas.Find(r.ParelhaId).Jogador1.Username,
                Username2 = _context.Parelhas.Find(r.ParelhaId).Jogador2.Username,
                JogosDisputados = r.JogosDisputados,
                Vitorias = r.Vitorias,
                Empates = r.Empates,
                Derrotas = r.Derrotas,
                GolosMarcados = r.GolosMarcados ?? 0,
                GolosSofridos = r.GolosSofridos ?? 0,
                DiferencaDeGolos = r.DiferencaDeGolos ?? 0,
                Pontos = r.Pontos
            }).ToList();

            ViewData["TemporadaId"] = temporadaId;
            return View(resultadosViewModel);
        }
    }
}
