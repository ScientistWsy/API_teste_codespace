using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoAPI.Models
{
    [Table("Medico", Schema = "agendamento")]
    public class Medico
    {
        [Key, ForeignKey("Usuario")]
        public Guid UsuarioId { get; set; }

        [Required, StringLength(10), RegularExpression(@"^[A-Za-z0-9]{6,10}$")]
        public string CRM { get; set; }

        // Navegação
        public Usuario Usuario { get; set; }
        public ICollection<MedicoEspecialidade> Especialidades { get; set; }
        public ICollection<Consulta> Consultas { get; set; }
    }
}