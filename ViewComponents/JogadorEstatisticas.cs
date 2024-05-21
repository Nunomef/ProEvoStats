using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.Models;
using ProEvoStats_EVO7.ViewModels;

namespace ProEvoStats_EVO7.ViewComponents
{
    public class JogadorEstatisticas : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public JogadorEstatisticas(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jogadorId)
        {
            var temporadas = _context.Temporadas
                .Select(t => t.Id)
                .ToList();

            var resultados = new List<dynamic>();

            foreach (var temporadaId in temporadas)
            {
                var campeonatos = _context.Campeonatos
                    .Where(c => c.TemporadaId == temporadaId)
                    .Select(c => c.Id)
                    .ToList();

                var resultadosTemporada = _context.Jogos
                    .Where(j => campeonatos.Contains(j.CampeonatoId) && j.ResultadoCasa != null && j.ResultadoFora != null)
                    .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
                    .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador2)
                    .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador1)
                    .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador2)
                    .ToList()
                    .Where(j => j.ParelhaCasa != null && j.ParelhaFora != null)
                    .SelectMany(j => new[] {
                new { JogadorId = j.ParelhaCasa.Jogador1Id, GolosMarcados = j.ResultadoCasa, GolosSofridos = j.ResultadoFora, Vitoria = j.ResultadoCasa > j.ResultadoFora ? 1 : 0, Empate = j.ResultadoCasa == j.ResultadoFora ? 1 : 0 },
                new { JogadorId = j.ParelhaCasa.Jogador2Id, GolosMarcados = j.ResultadoCasa, GolosSofridos = j.ResultadoFora, Vitoria = j.ResultadoCasa > j.ResultadoFora ? 1 : 0, Empate = j.ResultadoCasa == j.ResultadoFora ? 1 : 0 },
                new { JogadorId = j.ParelhaFora.Jogador1Id, GolosMarcados = j.ResultadoFora, GolosSofridos = j.ResultadoCasa, Vitoria = j.ResultadoFora > j.ResultadoCasa ? 1 : 0, Empate = j.ResultadoFora == j.ResultadoCasa ? 1 : 0 },
                new { JogadorId = j.ParelhaFora.Jogador2Id, GolosMarcados = j.ResultadoFora, GolosSofridos = j.ResultadoCasa, Vitoria = j.ResultadoFora > j.ResultadoCasa ? 1 : 0, Empate = j.ResultadoFora == j.ResultadoCasa ? 1 : 0 },
                    })
                    .ToList();

                resultados.AddRange(resultadosTemporada);
            }

            var campeonatosTotal = _context.Campeonatos.ToList();
            var estatisticasJogadores = resultados
                .GroupBy(r => r.JogadorId)
                .Select(g => new JogadorEstatisticasViewModel
                {
                    JogadorId = g.Key,
                    Username = _context.Jogadores.Find(g.Key).Username,
                    TemporadasGanhas = temporadas.Count(t => GetTemporadaVencedorId(t) == g.Key),
                    CampeonatosGanhos = campeonatosTotal.Count(c => GetCampeonatoVencedorId(c.Id) == g.Key),
                    JogosDisputados = g.Count(),
                    Vitorias = g.Sum(r => r.Vitoria),
                    Empates = g.Sum(r => r.Empate),
                    Derrotas = g.Count() - g.Sum(r => r.Vitoria) - g.Sum(r => r.Empate),
                    GolosMarcados = g.Sum(r => r.GolosMarcados),
                    GolosSofridos = g.Sum(r => r.GolosSofridos),
                    DiferencaDeGolos = g.Sum(r => r.GolosMarcados) - g.Sum(r => r.GolosSofridos),
                    EquipaMaisUsada = GetEquipaMaisUsada(g.Key),
                })
                .OrderByDescending(r => r.TemporadasGanhas)
                .ThenByDescending(r => r.CampeonatosGanhos)
                .ThenByDescending(r => r.GolosMarcados)
                .ThenBy(r => r.GolosSofridos)
                .ToList();

            ViewData["Posicao"] = estatisticasJogadores.FindIndex(j => j.JogadorId == jogadorId) + 1;

            var jogadorEstatisticas = estatisticasJogadores.FirstOrDefault(j => j.JogadorId == jogadorId);

            return View(jogadorEstatisticas);
        }

        // MÈTODOS AUXILIARES ============================================================================================================
        private int GetTemporadaVencedorId(int temporadaId)
        {
            var campeonatos = _context.Campeonatos
                .Where(c => c.TemporadaId == temporadaId && c.Temporada.Status == Status.Inactive)
                .Select(c => c.Id)
                .ToList();

            var resultados = new List<dynamic>();

            foreach (var campeonatoId in campeonatos)
            {
                var resultadosCampeonato = _context.Jogos
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
                    .ToList();

                resultados.AddRange(resultadosCampeonato);
            }

            var vencedor = resultados
                .GroupBy(r => r.JogadorId)
                .Select(g => new
                {
                    JogadorId = g.Key,
                    Pontos = g.Sum(r => r.Vitoria) * 3 + g.Sum(r => r.Empate)
                })
                .OrderByDescending(r => r.Pontos)
                .FirstOrDefault();

            return vencedor?.JogadorId ?? 0;
        }

        private int GetCampeonatoVencedorId(int campeonatoId)
        {
            var resultadosCampeonato = _context.Jogos
                .Where(j => j.CampeonatoId == campeonatoId && j.Campeonato.Status == Status.Inactive)
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
                .ToList();

            var vencedor = resultadosCampeonato
                .GroupBy(r => r.JogadorId)
                .Select(g => new
                {
                    JogadorId = g.Key,
                    Pontos = g.Sum(r => r.Vitoria) * 3 + g.Sum(r => r.Empate)
                })
                .OrderByDescending(r => r.Pontos)
                .FirstOrDefault();

            return vencedor?.JogadorId ?? 0;
        }


        //private int GetTotalCampeonatosVencidos(int jogadorId)
        //{
        //    var campeonatos = _context.Campeonatos
        //        .Where(c => c.Status == Status.Inactive)
        //        .ToList();

        //    var campeonatosVencidos = campeonatos
        //        .Count(c => GetCampeonatoVencedorId(c.Id) == jogadorId);

        //    return campeonatosVencidos;
        //}


        private string GetEquipaMaisUsada(int jogadorId)
        {
            var equipas = _context.Jogos
                .Where(j => (j.ParelhaCasa.Jogador1Id == jogadorId || j.ParelhaCasa.Jogador2Id == jogadorId || j.ParelhaFora.Jogador1Id == jogadorId || j.ParelhaFora.Jogador2Id == jogadorId) && j.EquipaCasa != null && j.EquipaFora != null)
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