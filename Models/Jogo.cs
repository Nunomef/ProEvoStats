using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvoStats_EVO7.Models
{
    [Table("Jogos")]
    public class Jogo
    {
        [Key]
        public int Id { get; set; }

        public int? ResultadoCasa { get; set; }
        public int? ResultadoFora { get; set; }
        public int? ParelhaCasaId { get; set; }
        [ForeignKey("ParelhaCasaId")]
        public Parelha ParelhaCasa { get; set; }
        public int? ParelhaForaId { get; set; }
        [ForeignKey("ParelhaForaId")]
        public Parelha ParelhaFora { get; set; }
        public int? EquipaCasaId { get; set; }
        [ForeignKey("EquipaCasaId")]
        public Equipa EquipaCasa { get; set; }
        public int? EquipaForaId { get; set; }
        [ForeignKey("EquipaForaId")]
        public Equipa EquipaFora { get; set; }

        [Required]
        public int CampeonatoId { get; set; }
        [ForeignKey("CampeonatoId")]
        public Campeonato Campeonato { get; set; }
    }
}
