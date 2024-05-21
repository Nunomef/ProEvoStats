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
    public class ParelhasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParelhasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parelhas
        public async Task<IActionResult> Index()
        {
            var parelhas = _context.Parelhas
                .Include(p => p.Jogador1)
                .Include(p => p.Jogador2)
                .Include(p => p.Temporada)
                .AsQueryable();

            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");
            ViewData["JogadorId"] = new SelectList(_context.Jogadores, "Id", "Username");
            return View(await parelhas.ToListAsync());
        }

        // POST: Parelhas/Index
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int? temporadaId, int? jogadorId)
        {
            var parelhas = _context.Parelhas
                .Include(p => p.Jogador1)
                .Include(p => p.Jogador2)
                .Include(p => p.Temporada)
                .AsQueryable();

            if (temporadaId.HasValue)
            {
                parelhas = parelhas.Where(p => p.TemporadaId == temporadaId);
            }

            if (jogadorId.HasValue)
            {
                parelhas = parelhas.Where(p => p.Jogador1Id == jogadorId || p.Jogador2Id == jogadorId);
            }

            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");
            ViewData["JogadorId"] = new SelectList(_context.Jogadores, "Id", "Username");
            return View(await parelhas.ToListAsync());
        }

        // GET: Parelhas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parelha = await _context.Parelhas
                .Include(p => p.Jogador1)
                .Include(p => p.Jogador2)
                .Include(p => p.Temporada)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parelha == null)
            {
                return NotFound();
            }

            return View(parelha);
        }

        // GET: Parelhas/Create
        public IActionResult Create()
        {
            ViewData["Jogador1Id"] = new SelectList(_context.Jogadores, "Id", "Username");
            ViewData["Jogador2Id"] = new SelectList(_context.Jogadores, "Id", "Username");
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao");
            return View();
        }

        // POST: Parelhas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Jogador1Id, int Jogador2Id, int TemporadaId )
        {
            Parelha parelha = new Parelha();
            parelha.Jogador1Id = Jogador1Id;
            parelha.Jogador2Id = Jogador2Id;
            parelha.TemporadaId = TemporadaId;

            if (ModelState.IsValid)
            {
                _context.Add(parelha);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Jogador1Id"] = new SelectList(_context.Jogadores, "Id", "Username", parelha.Jogador1Id);
            ViewData["Jogador2Id"] = new SelectList(_context.Jogadores, "Id", "Username", parelha.Jogador2Id);
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao", parelha.TemporadaId);
            return View(parelha);
        }

        // GET: Parelhas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parelha = await _context.Parelhas.FindAsync(id);
            if (parelha == null)
            {
                return NotFound();
            }
            ViewData["Jogador1Id"] = new SelectList(_context.Jogadores, "Id", "Username", parelha.Jogador1Id);
            ViewData["Jogador2Id"] = new SelectList(_context.Jogadores, "Id", "Username", parelha.Jogador2Id);
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao", parelha.TemporadaId);
            return View(parelha);
        }

        // POST: Parelhas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int Id,int Jogador1Id,int Jogador2Id,int TemporadaId)
        {
            Parelha parelha = new Parelha
            {
                Id = Id,
                Jogador1Id = Jogador1Id,
                Jogador2Id = Jogador2Id,
                TemporadaId = TemporadaId
            };

            if (id != parelha.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parelha);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParelhaExists(parelha.Id))
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
            ViewData["Jogador1Id"] = new SelectList(_context.Jogadores, "Id", "Username", parelha.Jogador1Id);
            ViewData["Jogador2Id"] = new SelectList(_context.Jogadores, "Id", "Username", parelha.Jogador2Id);
            ViewData["TemporadaId"] = new SelectList(_context.Temporadas, "Id", "Descricao", parelha.TemporadaId);
            return View(parelha);
        }

        // GET: Parelhas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parelha = await _context.Parelhas
                .Include(p => p.Jogador1)
                .Include(p => p.Jogador2)
                .Include(p => p.Temporada)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parelha == null)
            {
                return NotFound();
            }

            return View(parelha);
        }

        // POST: Parelhas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parelha = await _context.Parelhas.FindAsync(id);
            if (parelha != null)
            {
                _context.Parelhas.Remove(parelha);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParelhaExists(int id)
        {
            return _context.Parelhas.Any(e => e.Id == id);
        }
    }
}
