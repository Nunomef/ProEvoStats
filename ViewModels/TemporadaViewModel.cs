using ProEvoStats_EVO7.Models;
using System.Collections.Generic;

namespace ProEvoStats_EVO7.ViewModels
{
    public class TemporadaViewModel
    {
        public Temporada Temporada { get; set; }
        public List<Campeonato> Campeonatos { get; set; }
        public List<Jogador> Jogadores { get; set; }
    }
}
