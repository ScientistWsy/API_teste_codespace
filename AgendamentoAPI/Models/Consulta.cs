using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AgendamentoAPI.Enums;

namespace AgendamentoAPI.Models
{
    public class Consulta
    {
        public Guid Id { get; set; }
        public Guid MedicoId { get; set; }
        public Guid PacienteId { get; set; }
        public int EspecialidadeId { get; set; }
        public string Sintomas { get; set; }
        public string EspecialidadeRecomendada { get; set; }
        public DateTime DataConsulta { get; set; }
        public StatusConsulta Status { get; set; } = StatusConsulta.Agendada;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Medico Medico { get; set; }
        public Paciente Paciente { get; set; }
        public Especialidade Especialidade { get; set; }
    }
}