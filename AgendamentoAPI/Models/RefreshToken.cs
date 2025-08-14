using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoAPI.Models
{
    [Table("RefreshToken", Schema = "agendamento")]
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required, ForeignKey("Usuario")]
        public Guid UsuarioId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public string? DeviceInfo { get; set; }
        public Usuario Usuario { get; set; }
    }
}