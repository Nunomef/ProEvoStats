using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.Models;
using ProEvoStats_EVO7.ViewModels;
using System.Linq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Authorize]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: User
    public ActionResult UserProfile()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var jogadorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var jogador = _context.Jogadores
         .SingleOrDefault(j => j.Id == jogadorId);

        if (jogador == null)
        {
            return NotFound();
        }

        var equipaPreferida = _context.Equipas
            .SingleOrDefault(e => e.Id == jogador.EquipaPrefId);

        var ultimoCampeonato = _context.Jogos
            .Where(j => j.ParelhaCasa.Jogador1Id == jogadorId || j.ParelhaCasa.Jogador2Id == jogadorId || j.ParelhaFora.Jogador1Id == jogadorId || j.ParelhaFora.Jogador2Id == jogadorId)
            .Include(j => j.Campeonato).ThenInclude(c => c.Temporada)
            .Select(j => j.Campeonato)
            .OrderByDescending(c => c.Data)
            .FirstOrDefault();

        ViewBag.Username = jogador.Username;
        ViewBag.Email = jogador.Email;
        ViewBag.EquipaPreferida = equipaPreferida?.Nome;

        if (ultimoCampeonato != null)
        {            
            ViewBag.CampeonatoId = ultimoCampeonato.Id;
            ViewBag.TemporadaId = ultimoCampeonato.TemporadaId;
            ViewBag.CampeonatoStatus = ultimoCampeonato.Status;
            ViewBag.TemporadaStatus = ultimoCampeonato.Temporada.Status;
            ViewBag.CampeonatoDescricao = ultimoCampeonato.Descricao;
            ViewBag.CampeonatoData = ultimoCampeonato.Data;
            ViewBag.TemporadaDescricao = ultimoCampeonato.Temporada.Descricao;
            ViewBag.TemporadaAno = ultimoCampeonato.Temporada.Ano;
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile(int id)
    {
        var jogador = await _context.Jogadores.FindAsync(id);
        if (jogador == null)
        {
            return NotFound();
        }

        ViewBag.EquipaId = _context.Equipas.Select(e => new SelectListItem
        {
            Value = e.Id.ToString(),
            Text = e.Nome
        }).ToList();

        return View(jogador);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(int id, string Username, string Email, int EquipaPrefId)
    {
        var jogador = await _context.Jogadores.FindAsync(id);

        if (jogador == null)
        {
            return NotFound();
        }

        jogador.Username = Username;
        jogador.Email = Email;
        jogador.EquipaPrefId = EquipaPrefId;

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
            return RedirectToAction(nameof(UserProfile));
        }
        return View(jogador);
    }

    private bool JogadorExists(int id)
    {
        return _context.Jogadores.Any(e => e.Id == id);
    }



    // CAMPEONATOS ======================================================================================================
    public async Task<IActionResult> UserActiveChamp(int id)
    {
        var jogos = await _context.Jogos
            .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador1)
            .Include(j => j.ParelhaCasa).ThenInclude(p => p.Jogador2)
            .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador1)
            .Include(j => j.ParelhaFora).ThenInclude(p => p.Jogador2)
            .Where(j => j.CampeonatoId == id)
            .ToListAsync();

        var equipas = await _context.Equipas.ToListAsync();
        ViewBag.Equipas = new SelectList(equipas, "Id", "Nome");
        ViewBag.CampeonatoId = id;

        return View(jogos);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserActiveChamp(int id, int equipaCasaId, int resultadoCasa, int equipaForaId, int resultadoFora)
    {
        var jogo1 = await _context.Jogos.FindAsync(id);
        if (jogo1 == null)
        {
            return NotFound();
        }

        jogo1.EquipaCasaId = equipaCasaId;
        jogo1.ResultadoCasa = resultadoCasa;
        jogo1.EquipaForaId = equipaForaId;
        jogo1.ResultadoFora = resultadoFora;

        if (ModelState.IsValid)
        {
            _context.Entry(jogo1).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        var jogo2 = await _context.Jogos.FindAsync(jogo1.Id + 3);
        if (jogo2 != null && jogo2.CampeonatoId == jogo1.CampeonatoId)
        {
            jogo2.EquipaCasaId = equipaCasaId;
            jogo2.EquipaForaId = equipaForaId;
            _context.Entry(jogo2).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("UserActiveChamp", new { id = jogo1.CampeonatoId });
    }

    [HttpPost]
    public async Task<IActionResult> EndChampionship(int campeonatoId)
    {
        var jogos = await _context.Jogos
            .Where(j => j.CampeonatoId == campeonatoId)
            .ToListAsync();

        if (jogos.All(j => j.EquipaCasaId != null && j.ResultadoCasa != null && j.EquipaForaId != null && j.ResultadoFora != null))
        {
            var campeonato = await _context.Campeonatos.FindAsync(campeonatoId);
            if (campeonato == null)
            {
                return NotFound();
            }

            campeonato.Status = Status.Inactive;
            _context.Entry(campeonato).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("UserProfile", "User");
        }

        return RedirectToAction("UserActiveChamp", new { id = campeonatoId });
    }

    public IActionResult UserInactiveChamp(int id)
    {
        ViewBag.CampeonatoId = id;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCampeonato(int temporadaId, string descricao, DateOnly data)
    {
        // Criar um novo campeonato
        Campeonato campeonato = new Campeonato
        {
            Data = data,
            Descricao = descricao,
            Status = Status.Active,
            TemporadaId = temporadaId
        };

        _context.Add(campeonato);
        await _context.SaveChangesAsync();

        // Obter as parelhas da temporada
        var parelhas = _context.Parelhas.Where(p => p.TemporadaId == campeonato.TemporadaId).ToList();

        // Verificar se existem exatamente 6 parelhas
        if (parelhas.Count != 6)
        {
            ModelState.AddModelError("", "Deve haver exatamente 6 parelhas para um campeonato.");
            return RedirectToAction("UserActiveTemp", new { id = temporadaId });
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

        // Redirecionar para a view UserActiveChamp com o ID do campeonato recém-criado
        return RedirectToAction("UserActiveChamp", new { id = campeonato.Id });
    }

    // TEMPORADAS ======================================================================================================
    public async Task<IActionResult> UserActiveTemp(int id)
    {
        var temporada = await _context.Temporadas.FindAsync(id);
        if (temporada == null)
        {
            return NotFound();
        }

        var campeonatos = await _context.Campeonatos
            .Where(c => c.TemporadaId == id)
            .OrderByDescending(c => c.Data)
            .Select(c => new CampeonatoViewModel { Id = c.Id, Descricao = c.Descricao, Status = c.Status.ToString() })
            .ToListAsync();

        var ultimoCampeonato = campeonatos.FirstOrDefault();

        ViewBag.TemporadaId = id;
        ViewBag.CampeonatoId = ultimoCampeonato?.Id;
        ViewBag.UltimoCampeonatoStatus = ultimoCampeonato?.Status.ToString();

        return View(campeonatos);
    }

    [HttpPost]
    public async Task<IActionResult> EndSeason(int temporadaId)
    {
        var campeonatos = await _context.Campeonatos
            .Where(c => c.TemporadaId == temporadaId)
            .ToListAsync();

        if (campeonatos.All(c => c.Status == Status.Inactive))
        {
            var temporada = await _context.Temporadas.FindAsync(temporadaId);
            if (temporada == null)
            {
                return NotFound();
            }

            temporada.Status = Status.Inactive;
            _context.Entry(temporada).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("UserInactiveTemp", new { id = temporadaId });
        }

        var activeChamp = campeonatos.FirstOrDefault(c => c.Status == Status.Active);
        return RedirectToAction("UserActiveChamp", new { id = activeChamp?.Id });
    }

    public async Task<IActionResult> UserInactiveTemp(int id)
    {
        var temporada = await _context.Temporadas.FindAsync(id);
        if (temporada == null)
        {
            return NotFound();
        }

        var campeonatos = await _context.Campeonatos
            .Where(c => c.TemporadaId == id)
            .OrderByDescending(c => c.Data)
            .ToListAsync();

        var ultimoCampeonato = campeonatos.FirstOrDefault();

        var jogadores = await _context.Jogadores.ToListAsync();

        ViewBag.Jogadores = new SelectList(jogadores, "Id", "Username");

        ViewBag.TemporadaId = id;
        ViewBag.Campeonatos = new SelectList(campeonatos, "Id", "Descricao");
        ViewBag.CampeonatoId = ultimoCampeonato?.Id;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTemporada(string descricao, int Jogador1Id, int Jogador2Id, int Jogador3Id, int Jogador4Id)
    {
        var temporada = new Temporada
        {
            Descricao = descricao,
            Ano = (short)DateTime.Now.Year,
            Status = Status.Active
        };

        // Add the selected players to the temporada
        // Replace 'GetPlayerById' with the actual method to get a player by ID
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

        return RedirectToAction("UserActiveTemp", new { id = temporada.Id });
    }

    // ESTATISTICAS GLOBAIS ======================================================================================================
    public IActionResult GlobalStats()
    {
        return View();
    }


    // METODOS AUXILIARES ======================================================================================================
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

    public async Task<IActionResult> UpdateCampeonatoClassificacao(int campeonatoId)
    {
        return ViewComponent("CampeonatoClassificacao", new { campeonatoId = campeonatoId });
    }





}
