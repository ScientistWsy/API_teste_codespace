using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AgendamentoAPI.Models
{
    [Table("Especialidade", Schema = "agendamento")]
    public class Especialidade
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Nome { get; set; }

        // Navegação
        public ICollection<MedicoEspecialidade> Medicos { get; set; }
        public ICollection<Consulta> Consultas { get; set; }
    }
}