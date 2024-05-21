using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.Models;

namespace ProEvoStats_EVO7.Controllers
{
    [Authorize(Roles = "Admin")]
    public class JogosController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public JogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jogos
        public async Task<IActionResult> Index()
        {
            var jogos = _context.Jogos
                .Include(j => j.Campeonato).ThenInclude(c => c.Temporada)
                .Include(j => j.EquipaCasa)
                .Include(j => j.EquipaFora)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador2)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador2)
                .AsQueryable();

            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");
            ViewData["CampeonatoId"] = new SelectList(_context.Campeonatos, "Id", "Descricao");

            return View(await jogos.ToListAsync());
        }

        // POST: Jogos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int? temporadaId, int? campeonatoId, DateOnly? campeonatoData)
        {
            var jogos = _context.Jogos
                .Include(j => j.Campeonato).ThenInclude(c => c.Temporada)
                .Include(j => j.EquipaCasa)
                .Include(j => j.EquipaFora)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador2)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador1)
                .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador2)
                .AsQueryable();

            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");
            ViewData["CampeonatoId"] = new SelectList(_context.Campeonatos, "Id", "Descricao");

            if (temporadaId.HasValue)
            {
                jogos = jogos.Where(j => j.Campeonato.TemporadaId == temporadaId.Value);
            }

            if (campeonatoId.HasValue)
            {
                jogos = jogos.Where(j => j.CampeonatoId == campeonatoId.Value);
            }

            if (campeonatoData.HasValue)
            {
                jogos = jogos.Where(j => j.Campeonato.Data == campeonatoData.Value);
            }

            return View(await jogos.ToListAsync());
        }

        // GET: Jogos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _context.Jogos
               .Include(j => j.Campeonato)
               .Include(j => j.EquipaCasa)
               .Include(j => j.EquipaFora)
               .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
               .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador2)
               .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador1)
               .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador2)
               .FirstOrDefaultAsync(m => m.Id == id);
            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }

        // GET: Jogos/Create
        public IActionResult Create()
        {
            FillViewData();
            return View();
        }

        // POST: Jogos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ResultadoCasa,int ResultadoFora,int ParelhaCasaId, int ParelhaForaId, int EquipaCasaId,int EquipaForaId, int CampeonatoId)
        {
            Jogo jogo = new Jogo
            {
                ResultadoCasa = ResultadoCasa,
                ResultadoFora = ResultadoFora,
                ParelhaCasaId = ParelhaCasaId,
                ParelhaForaId = ParelhaForaId,
                EquipaCasaId = EquipaCasaId,
                EquipaForaId = EquipaForaId,
                CampeonatoId = CampeonatoId
            };

            if (ModelState.IsValid)
            {
                _context.Add(jogo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            FillViewData();
            return View(jogo);
        }

        // GET: Jogos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
            {
                return NotFound();
            }

            int temporadaId = _context.Jogos.Where(j => j.Id == id)
                                            .Select(j => j.Campeonato.TemporadaId)
                                            .FirstOrDefault();

            FillViewData(temporadaId);
            return View(jogo);
        }

        // POST: Jogos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int Id, int? ResultadoCasa, int? ResultadoFora, int ParelhaCasaId, int ParelhaForaId, int EquipaCasaId, int EquipaForaId, int CampeonatoId)
        {
            Jogo jogo = new Jogo
            {
                Id = Id,
                ResultadoCasa = ResultadoCasa,
                ResultadoFora = ResultadoFora,
                ParelhaCasaId = ParelhaCasaId,
                ParelhaForaId = ParelhaForaId,
                EquipaCasaId = EquipaCasaId,
                EquipaForaId = EquipaForaId,
                CampeonatoId = CampeonatoId
            };

            if (id != jogo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jogo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogoExists(jogo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            int temporadaId = _context.Campeonatos.Where(c => c.Id == CampeonatoId)
                                                  .Select(c => c.TemporadaId)
                                                  .FirstOrDefault();

            FillViewData(temporadaId);
            return View(jogo);
        }

        // GET: Jogos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _context.Jogos
                .Include(j => j.Campeonato)
                .Include(j => j.EquipaCasa)
                .Include(j => j.EquipaFora)
                .Include(j => j.ParelhaCasa)
                .Include(j => j.ParelhaFora)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }

        // POST: Jogos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo != null)
            {
                _context.Jogos.Remove(jogo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JogoExists(int id)
        {
            return _context.Jogos.Any(e => e.Id == id);
        }



        // MÉTODOS AUXILIARES ===================================================================================================
        private IEnumerable<dynamic>? parelhasCasa;
        private IEnumerable<dynamic>? parelhasFora;

        // Inicializa as parelhas com concatenação de Usernames para serem usadas em FillViewData()
        private void InitializeParelhas(int? TemporadaId = null)
        {
            var parelhas = _context.Parelhas.AsQueryable();

            if (TemporadaId.HasValue)
            {
                parelhas = parelhas.Where(p => p.TemporadaId == TemporadaId.Value);
            }

            var parelhasList = parelhas
                .Include(p => p.Jogador1)
                .Include(p => p.Jogador2)
                .ToList();

            parelhasCasa = parelhasList.Select(p => new
            {
                Id = p.Id,
                Usernames = $"{p.Jogador1.Username} & {p.Jogador2.Username}"
            });

            parelhasFora = parelhasList.Select(p => new
            {
                Id = p.Id,
                Usernames = $"{p.Jogador1.Username} & {p.Jogador2.Username}"
            });
        }

        // Preenche a ViewData com os dados necessários
        private void FillViewData(int? temporadaId = null)
        {
            InitializeParelhas(temporadaId);

            ViewData["CampeonatoId"] = new SelectList(_context.Campeonatos, "Id", "Descricao");
            ViewData["EquipaCasaId"] = new SelectList(_context.Equipas, "Id", "Nome");
            ViewData["EquipaForaId"] = new SelectList(_context.Equipas, "Id", "Nome");
            ViewData["ParelhaCasaId"] = new SelectList(parelhasCasa, "Id", "Usernames");
            ViewData["ParelhaForaId"] = new SelectList(parelhasFora, "Id", "Usernames");
        }


    }
}
