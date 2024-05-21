using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.Models;
using ProEvoStats_EVO7.ViewModels;

namespace ProEvoStats_EVO7.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var temporadas = _context.Temporadas
                .Include(t => t.Parelhas).ThenInclude(p => p.Jogador1)
                .Include(t => t.Parelhas).ThenInclude(p => p.Jogador2)
                .Include(t => t.Campeonatos).ThenInclude(c => c.Jogos)
                .ToList();

            var temporadaViewModels = new List<TemporadaViewModel>();

            foreach (var temporada in temporadas)
            {
                var jogadores = new List<Jogador>();

                foreach (var parelha in temporada.Parelhas)
                {
                    if (!jogadores.Contains(parelha.Jogador1))
                    {
                        jogadores.Add(parelha.Jogador1);
                    }
                    if (!jogadores.Contains(parelha.Jogador2))
                    {
                        jogadores.Add(parelha.Jogador2);
                    }
                }

                temporadaViewModels.Add(new TemporadaViewModel
                {
                    Temporada = temporada,
                    Jogadores = jogadores,
                    Campeonatos = temporada.Campeonatos.ToList()
                });
            }

            return View(temporadaViewModels);
        }

        public IActionResult Campeonato(int campeonatoId)
        {
            ViewData["CampeonatoId"] = campeonatoId;
            return View();
        }

        public IActionResult Temporada(int temporadaId)
        {
            ViewData["TemporadaId"] = temporadaId;
            return View();
        }

        public IActionResult Jogador(int jogadorId, int temporadaId)
        {
            ViewData["Jogador"] = _context.Jogadores.Find(jogadorId);
            ViewData["TemporadaId"] = temporadaId;
            return View();
        }
    }
}
