using System.ComponentModel.DataAnnotations;
namespace CalidadAireAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UsuarioNombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ContrasenaHash { get; set; } = string.Empty;
    }
}
