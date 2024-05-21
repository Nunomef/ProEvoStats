using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvoStats_EVO7.Models
{    
    [Table("Campeonatos")]
    public class Campeonato
    {
        public Campeonato()
        {
            Jogos = new List<Jogo>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public DateOnly Data { get; set; }

        [StringLength(45)]
        public string? Descricao { get; set; }

        [Required]
        public Status Status { get; set; } = Status.Active;

        [Required]
        public int TemporadaId { get; set; }
        [ForeignKey("TemporadaId")]
        public Temporada Temporada { get; set; }

        public ICollection<Jogo> Jogos { get; set; }
    }
}
