using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProEvoStats_EVO7.Models
{
    public enum Role
    {
        User,
        Admin
    }

    public enum Status
    {
        Active,
        Inactive
    }

    [Table("Jogadores")]
    public class Jogador
    {
        public Jogador()
        {
            ParelhasJogador1 = new List<Parelha>();
            ParelhasJogador2 = new List<Parelha>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(45)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z\.]).{7,15}$", ErrorMessage = "A password deve ter entre 7 e 15 caracteres e conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere99999999 especial.")]
        public string Password { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public Role Role { get; set; } = Role.User;

        public int? EquipaPrefId { get; set; }
        [ForeignKey("EquipaPrefId")]
        public Equipa? EquipaPref { get; set; }

        [Required]
        public Status Status { get; set; } = Status.Active;



        public ICollection<Parelha> ParelhasJogador1 { get; set; }
        public ICollection<Parelha> ParelhasJogador2 { get; set; }
    }
}
