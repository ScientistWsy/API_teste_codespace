using System.ComponentModel.DataAnnotations.Schema;

namespace AgendamentoAPI.Models
{
    [Table("MedicoEspecialidade", Schema = "agendamento")]
    public class MedicoEspecialidade
    {
        [ForeignKey("Medico")]
        public Guid MedicoId { get; set; }

        [ForeignKey("Especialidade")]
        public int EspecialidadeId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegação
        public Medico Medico { get; set; }
        public Especialidade Especialidade { get; set; }
    }
}