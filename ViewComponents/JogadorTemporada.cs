using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.ViewModels;

namespace ProEvoStats_EVO7.ViewComponents
{
    public class JogadorTemporada : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public JogadorTemporada(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jogadorId, int temporadaId)
        {
            var jogador = _context.Jogadores.Find(jogadorId);
            var campeonatos = _context.Campeonatos
                .Where(c => c.TemporadaId == temporadaId)
                .Select(c => c.Id)
                .ToList();

            var resultados = _context.Jogos
                 .Where(j => campeonatos.Contains(j.CampeonatoId) && j.ResultadoCasa.HasValue && j.ResultadoFora.HasValue).Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
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
                 .Where(r => r.JogadorId == jogadorId)
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
                     CampeonatosGanhos = campeonatos.Count(c => GetCampeonatoVencedorId(c) == g.Key)
                 })
                 .FirstOrDefault();

            var viewModel = new JogadorTemporadaViewModel
            {
                Username = jogador.Username,
                //Posicao = resultados != null ? GetPosicaoNaTemporada(resultados.CampeonatosGanhos, temporadaId) : 0,
                CampeonatosGanhos = resultados?.CampeonatosGanhos ?? 0,
                JogosDisputados = resultados?.JogosDisputados ?? 0,
                Vitorias = resultados?.Vitorias ?? 0,
                Empates = resultados?.Empates ?? 0,
                Derrotas = resultados?.Derrotas ?? 0,
                GolosMarcados = resultados?.GolosMarcados ?? 0,
                GolosSofridos = resultados?.GolosSofridos ?? 0,
                DiferencaDeGolos = resultados?.DiferencaDeGolos ?? 0,
                EquipaMaisUsada = GetEquipaMaisUsada(jogadorId, temporadaId)
            };

            return View(viewModel);
        }

        // MÉTODOS AUXILIARES  ==========================================================================================
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

        //private int GetPosicaoNaTemporada(int campeonatosGanhos, int temporadaId)
        //{
        //    var campeonatos = _context.Campeonatos
        //        .Where(c => c.TemporadaId == temporadaId)
        //        .Select(c => c.Id)
        //        .ToList();

        //    var classificacao = _context.Jogadores
        //        .ToList()
        //        .Select(j => new
        //        {
        //            JogadorId = j.Id,
        //            CampeonatosGanhos = campeonatos.Count(c => GetCampeonatoVencedorId(c) == j.Id)
        //        })
        //        .OrderByDescending(r => r.CampeonatosGanhos)
        //        .ToList();

        //    var posicao = classificacao.FindIndex(r => r.CampeonatosGanhos == campeonatosGanhos) + 1;

        //    return posicao;
        //}


        private string GetEquipaMaisUsada(int jogadorId, int temporadaId)
        {
            var campeonatos = _context.Campeonatos
                .Where(c => c.TemporadaId == temporadaId)
                .Select(c => c.Id)
                .ToList();

            var jogos = _context.Jogos
                .Where(j => campeonatos.Contains(j.CampeonatoId) && (j.ParelhaCasa.Jogador1Id == jogadorId || j.ParelhaCasa.Jogador2Id == jogadorId || j.ParelhaFora.Jogador1Id == jogadorId || j.ParelhaFora.Jogador2Id == jogadorId))
                .ToList();

            var equipasUsadas = jogos
                .SelectMany(j => new[] { j.EquipaCasa?.Nome, j.EquipaFora?.Nome })
                .Where(nome => nome != null)
                .GroupBy(e => e)
                .Select(g => new { Equipa = g.Key, Count = g.Count() })
                .OrderByDescending(e => e.Count)
                .FirstOrDefault();

            return equipasUsadas?.Equipa ?? string.Empty;
        }

    }
}
