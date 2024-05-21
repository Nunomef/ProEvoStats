using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.ViewModels;

namespace ProEvoStats_EVO7.ViewComponents
{
    public class EquipaEstatisticas : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public EquipaEstatisticas(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var jogos = await _context.Jogos
                .Where(j => j.EquipaCasa != null && j.EquipaFora != null)
                .Select(j => new { EquipaCasaNome = j.EquipaCasa.Nome, EquipaForaNome = j.EquipaFora.Nome, j.ResultadoCasa, j.ResultadoFora })
                .ToListAsync();

            var equipas = jogos
            .SelectMany(j => new[] {
                new { Equipa = j.EquipaCasaNome, Vitoria = (j.ResultadoCasa ?? 0) > (j.ResultadoFora ?? 0) ? 1 : 0, Empate = (j.ResultadoCasa ?? 0) == (j.ResultadoFora ?? 0) ? 1 : 0, Derrota = (j.ResultadoCasa ?? 0) < (j.ResultadoFora ?? 0) ? 1 : 0, GolosMarcados = j.ResultadoCasa ?? 0, GolosSofridos = j.ResultadoFora ?? 0 },
                new { Equipa = j.EquipaForaNome, Vitoria = (j.ResultadoFora ?? 0) > (j.ResultadoCasa ?? 0) ? 1 : 0, Empate = (j.ResultadoFora ?? 0) == (j.ResultadoCasa ?? 0) ? 1 : 0, Derrota = (j.ResultadoFora ?? 0) < (j.ResultadoCasa ?? 0) ? 1 : 0, GolosMarcados = j.ResultadoFora ?? 0, GolosSofridos = j.ResultadoCasa ?? 0 }
            })
            .GroupBy(e => e.Equipa)
            .Select(g => new EquipaEstatisticasViewModel
            {
                Equipa = g.Key,
                Contagem = g.Count(),
                Vitorias = g.Sum(e => e.Vitoria),
                Empates = g.Sum(e => e.Empate),
                Derrotas = g.Sum(e => e.Derrota),
                GolosMarcados = g.Sum(e => e.GolosMarcados),
                GolosSofridos = g.Sum(e => e.GolosSofridos),
                DiferencaDeGolos = g.Sum(e => e.GolosMarcados) - g.Sum(e => e.GolosSofridos),
                Pontos = g.Sum(e => e.Vitoria * 3 + e.Empate)
            })
            .OrderByDescending(g => g.Pontos)
            .ThenByDescending(g => g.Vitorias)
            .ToList();

            return View(equipas);
        }

    }
}
