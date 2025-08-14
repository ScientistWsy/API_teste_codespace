using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoAPI.Models
{
    [Table("Paciente", Schema = "agendamento")]
    public class Paciente
    {
        [Key, ForeignKey("Usuario")]
        public Guid UsuarioId { get; set; }

        [Required, StringLength(11), RegularExpression(@"^[0-9]{11}$")]
        public string CPF { get; set; }

        public DateTime? DataNascimento { get; set; }

        // Navegação
        public Usuario Usuario { get; set; }
        public ICollection<Consulta> Consultas { get; set; }
    }
}