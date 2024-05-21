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
using ProEvoStats_EVO7.Services;

namespace ProEvoStats_EVO7.Controllers
{
    [Authorize(Roles = "Admin")]
    public class JogadoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JogadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jogadores
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Jogadores.Include(j => j.EquipaPref);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Jogadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogador = await _context.Jogadores
                .Include(j => j.EquipaPref)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jogador == null)
            {
                return NotFound();
            }

            return View(jogador);
        }

        // GET: Jogadores/Create
        public IActionResult Create()
        {
            ViewData["Role"] = new SelectList(Enum.GetValues(typeof(Role)));
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["EquipaPrefId"] = new SelectList(_context.Equipas, "Id", "Nome");
            return View();
        }

        // POST: Jogadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Role,EquipaPrefId,Status")] Jogador jogador)
        {
            if (ModelState.IsValid)
            {
                jogador.Password = HashUtils.ComputeSha256Hash(jogador.Password); 
                _context.Add(jogador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Role"] = new SelectList(Enum.GetValues(typeof(Role)));
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["EquipaPrefId"] = new SelectList(_context.Equipas, "Id", "Nome", jogador.EquipaPrefId);
            return View(jogador);
        }

        // GET: Jogadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogador = await _context.Jogadores.FindAsync(id);
            if (jogador == null)
            {
                return NotFound();
            }
            ViewData["Role"] = new SelectList(Enum.GetValues(typeof(Role)));
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)));
            ViewData["EquipaPrefId"] = new SelectList(_context.Equipas, "Id", "Nome", jogador.EquipaPrefId);
            return View(jogador);
        }

        // POST: Jogadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int Id,string Username,string Password, string? Email,string Role,int EquipaPrefId,string Status)
        {
            Jogador jogador = new Jogador
            {
                Id = Id,
                Username = Username,
                Password = Password,
                Email = Email,
                Role = (Role)Enum.Parse(typeof(Role), Role),
                EquipaPrefId = EquipaPrefId,
                Status = (Status)Enum.Parse(typeof(Status), Status)
            };

            if (id != jogador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jogador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogadorExists(jogador.Id))
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
            ViewData["EquipaPrefId"] = new SelectList(_context.Equipas, "Id", "Nome", jogador.EquipaPrefId);
            return View(jogador);
        }

        // GET: Jogadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogador = await _context.Jogadores
                .Include(j => j.EquipaPref)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jogador == null)
            {
                return NotFound();
            }

            return View(jogador);
        }

        // POST: Jogadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogador = await _context.Jogadores.FindAsync(id);
            if (jogador != null)
            {
                _context.Jogadores.Remove(jogador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JogadorExists(int id)
        {
            return _context.Jogadores.Any(e => e.Id == id);
        }
    }
}
