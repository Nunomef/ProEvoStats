using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvoStats_EVO7.Models
{
    [Table("Temporadas")]
    public class Temporada
    {
        public Temporada()
        {
            Parelhas = new List<Parelha>();
            Campeonatos = new List<Campeonato>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public short Ano { get; set; }

        [Required]
        [StringLength(45)]
        public string Descricao { get; set; }

        [Required]
        public Status Status { get; set; } = Status.Active;


        public ICollection<Parelha> Parelhas { get; set; }

        public ICollection<Campeonato> Campeonatos { get; set; }
    }
}
