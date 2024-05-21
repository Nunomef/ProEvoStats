﻿namespace ProEvoStats_EVO7.ViewModels
{
    public class JogadorEstatisticasViewModel
    {
        public int JogadorId { get; set; }
        public string Username { get; set; }
        public int TemporadasGanhas { get; set; }
        public int CampeonatosGanhos { get; set; }
        public int JogosDisputados { get; set; }
        public int Vitorias { get; set; }
        public int Empates { get; set; }
        public int Derrotas { get; set; }
        public int GolosMarcados { get; set; }
        public int GolosSofridos { get; set; }
        public int DiferencaDeGolos { get; set; }
        public string EquipaMaisUsada { get; set; }
    }
}