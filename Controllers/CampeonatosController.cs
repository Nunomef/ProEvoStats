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
    public class CampeonatosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CampeonatosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Campeonatos
        public async Task<IActionResult> Index()
        {
            var campeonatos = _context.Campeonatos
                .Include(c => c.Temporada)
                .AsQueryable();

            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");
            ViewData["CampeonatoData"] = new SelectList(_context.Campeonatos, "Data", "Data");
            return View(await campeonatos.ToListAsync());
        }

        // POST: Campeonatos/Index
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int? temporadaId, DateOnly? campeonatoData)
        {
            var campeonatos = _context.Campeonatos
                .Include(c => c.Temporada)
                .AsQueryable();

            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");

            if (temporadaId.HasValue)
            {
                campeonatos = campeonatos.Where(c => c.TemporadaId == temporadaId.Value);
            }
            if (campeonatoData.HasValue)
            {
                campeonatos = campeonatos.Where(c => c.Data == campeonatoData.Value);
            }

            return View(await campeonatos.ToListAsync());
        }

        // GET: Campeonatos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campeonato = await _context.Campeonatos
                .Include(c => c.Temporada)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campeonato == null)
            {
                return NotFound();
            }

            return View(campeonato);
        }

        // GET: Campeonatos/Create
        public IActionResult Create()
        {
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");
            return View();
        }

        // POST: Campeonatos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateOnly Data, string Descricao, string Status, int TemporadaId)
        {
            Campeonato campeonato = new Campeonato
            {
                Data = Data,
                Descricao = Descricao,
                Status = (Status)Enum.Parse(typeof(Status), Status),
                TemporadaId = TemporadaId
            };

            if (ModelState.IsValid)
            {
                _context.Add(campeonato);
                await _context.SaveChangesAsync();

                // Obter as parelhas da temporada
                var parelhas = _context.Parelhas.Where(p => p.TemporadaId == campeonato.TemporadaId).ToList();

                // Verificar se existem exatamente 6 parelhas
                if (parelhas.Count != 6)
                {
                    ModelState.AddModelError("", "Deve haver exatamente 6 parelhas para um campeonato.");
                    ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
                    ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao", campeonato.TemporadaId);
                    return View(campeonato);
                }

                // Randomizar parelhas até que as parelhas que se defrontam não tenham o mesmo jogador
                var random = new Random();
                do
                {
                    parelhas = parelhas.OrderBy(x => random.Next()).ToList();
                } while (!ParelhasValidas(parelhas));

                // Criar os jogos
                for (int i = 0; i < 3; i++)
                {
                    var jogo = new Jogo
                    {
                        CampeonatoId = campeonato.Id,
                        ParelhaCasaId = parelhas[i].Id,
                        ParelhaForaId = parelhas[i + 3].Id,
                    };
                    _context.Add(jogo);
                }

                // Criar os jogos com as parelhas trocadas
                for (int i = 0; i < 3; i++)
                {
                    var jogo = new Jogo
                    {
                        CampeonatoId = campeonato.Id,
                        ParelhaCasaId = parelhas[i + 3].Id,
                        ParelhaForaId = parelhas[i].Id,
                    };
                    _context.Add(jogo);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao", campeonato.TemporadaId);
            return View(campeonato);
        }

        // GET: Campeonatos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campeonato = await _context.Campeonatos.FindAsync(id);
            if (campeonato == null)
            {
                return NotFound();
            }

            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao", campeonato.TemporadaId);
            return View(campeonato);
        }

        // POST: Campeonatos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int Id, DateOnly Data,string Descricao,string Status,int TemporadaId)
        {
            Campeonato campeonato = new Campeonato
            {
                Id = Id,
                Data = Data,
                Descricao = Descricao,
                Status = (Status)Enum.Parse(typeof(Status), Status),
                TemporadaId = TemporadaId
            };

            if (id != campeonato.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(campeonato);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampeonatoExists(campeonato.Id))
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

            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao", campeonato.TemporadaId);
            return View(campeonato);
        }

        // GET: Campeonatos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campeonato = await _context.Campeonatos
                .Include(c => c.Temporada)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campeonato == null)
            {
                return NotFound();
            }

            return View(campeonato);
        }

        // POST: Campeonatos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var campeonato = await _context.Campeonatos.FindAsync(id);
            if (campeonato != null)
            {
                _context.Campeonatos.Remove(campeonato);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CampeonatoExists(int id)
        {
            return _context.Campeonatos.Any(e => e.Id == id);
        }

        // Verificarna Task Create() se as parelhas são válidas
        private bool ParelhasValidas(List<Parelha> parelhas)
        {
            for (int i = 0; i < 3; i++)
            {
                var parelhaCasa = parelhas[i];
                var parelhaFora = parelhas[i + 3];
                if (parelhaCasa.Jogador1Id == parelhaFora.Jogador1Id || parelhaCasa.Jogador1Id == parelhaFora.Jogador2Id ||
                    parelhaCasa.Jogador2Id == parelhaFora.Jogador1Id || parelhaCasa.Jogador2Id == parelhaFora.Jogador2Id)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
