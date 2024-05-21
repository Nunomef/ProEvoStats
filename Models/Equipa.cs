using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProEvoStats_EVO7.Models
{
    [Table("Equipas")]
    public class Equipa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(45)]
        public string Nome { get; set; }

        [Required]
        [StringLength(45)]
        public string Pais { get; set; }
    }
}
