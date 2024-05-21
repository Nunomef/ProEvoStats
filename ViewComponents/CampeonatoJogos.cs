using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;

public class CampeonatoJogos : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CampeonatoJogos(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int campeonatoId)
    {
        var jogos = await _context.Jogos
            .Include(j => j.EquipaCasa)
            .Include(j => j.EquipaFora)
            .Where(j => j.CampeonatoId == campeonatoId)
            .ToListAsync();

        return View(jogos);
    }
}
