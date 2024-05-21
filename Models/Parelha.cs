using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvoStats_EVO7.Models
{
    [Table("Parelhas")]
    public class Parelha
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Jogador1Id { get; set; }
        [ForeignKey("Jogador1Id")]
        public Jogador Jogador1 { get; set; }

        [Required]
        public int Jogador2Id { get; set; }
        [ForeignKey("Jogador2Id")]
        public Jogador Jogador2 { get; set; }

        [Required]
        public int TemporadaId { get; set; }
        [ForeignKey("TemporadaId")]
        public Temporada Temporada { get; set; }
    }
}
