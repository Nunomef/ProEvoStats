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
    public class TemporadasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TemporadasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Temporadas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Temporadas.ToListAsync());
        }

        // GET: Temporadas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var temporada = await _context.Temporadas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (temporada == null)
            {
                return NotFound();
            }

            return View(temporada);
        }

        // GET: Temporadas/Create
        public IActionResult Create()
        {
            var temporada = new Temporada
            {
                Ano = (short)DateTime.Now.Year
            };

            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["Jogadores"] = new SelectList(_context.Jogadores, "Id", "Username");

            return View(temporada);
        }

        // POST: Temporadas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ano,Descricao,Status")] Temporada temporada, int Jogador1Id, int Jogador2Id, int Jogador3Id, int Jogador4Id)
        {
            if (ModelState.IsValid)
            {
                int[] jogadoresIds = new int[] { Jogador1Id, Jogador2Id, Jogador3Id, Jogador4Id };

                // Verifica se todos os jogadores são únicos
                if (jogadoresIds.Distinct().Count() != 4)
                {
                    ModelState.AddModelError("", "Todos os jogadores devem ser únicos.");
                    ViewData["Jogadores"] = new SelectList(_context.Jogadores, "Id", "Nome");
                    return View(temporada);
                }

                _context.Add(temporada);
                await _context.SaveChangesAsync();

                // Cria as parelhas
                for (int i = 0; i < jogadoresIds.Length; i++)
                {
                    for (int j = i + 1; j < jogadoresIds.Length; j++)
                    {
                        Parelha parelha = new Parelha
                        {
                            Jogador1Id = jogadoresIds[i],
                            Jogador2Id = jogadoresIds[j],
                            TemporadaId = temporada.Id
                        };
                        _context.Add(parelha);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(temporada);
        }

        // GET: Temporadas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var temporada = await _context.Temporadas.FindAsync(id);
            if (temporada == null)
            {
                return NotFound();
            }
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)), temporada.Status);
            return View(temporada);
        }

        // POST: Temporadas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ano,Descricao,Status")] Temporada temporada)
        {
            if (id != temporada.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(temporada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemporadaExists(temporada.Id))
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
            return View(temporada);
        }

        // GET: Temporadas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var temporada = await _context.Temporadas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (temporada == null)
            {
                return NotFound();
            }

            return View(temporada);
        }

        // POST: Temporadas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var temporada = await _context.Temporadas.FindAsync(id);
            if (temporada != null)
            {
                _context.Temporadas.Remove(temporada);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemporadaExists(int id)
        {
            return _context.Temporadas.Any(e => e.Id == id);
        }
    }
}
