-- Tabela de Usuários (base para Pacientes e Médicos)
CREATE TABLE agendamento."Usuario" (
    Id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Telefone VARCHAR(20),
    SenhaHash TEXT NOT NULL,
    Perfil VARCHAR(20) CHECK(Perfil IN ('Paciente', 'Medico')) NOT NULL,
    UpdatedAt TIMESTAMP DEFAULT NOW() NOT NULL,
    CreatedAt TIMESTAMP DEFAULT NOW() NOT NULL,
    DeletedAt TIMESTAMP NULL
);

-- Tabela de Pacientes
CREATE TABLE agendamento."Paciente" (
    UsuarioId UUID PRIMARY KEY REFERENCES agendamento."Usuario"(Id),
    CPF CHAR(11) UNIQUE NOT NULL,
    DataNascimento DATE,
    CONSTRAINT formato_cpf CHECK (CPF ~ '^[0-9]{11}$')
);

-- Tabela de Médicos
CREATE TABLE agendamento."Medico" (
    UsuarioId UUID PRIMARY KEY REFERENCES agendamento."Usuario"(Id),
    CRM VARCHAR(10) UNIQUE NOT NULL,
    CONSTRAINT formato_crm CHECK (CRM ~ '^[A-Za-z0-9]{6,10}$')
);

-- Tabela de Especialidades Médicas
CREATE TABLE agendamento."Especialidade" (
    Id SERIAL PRIMARY KEY,
    Nome VARCHAR(50) UNIQUE NOT NULL
);

-- Tabela de Relacionamento Médico-Especialidade
CREATE TABLE agendamento."MedicoEspecialidade" (
    MedicoId UUID REFERENCES agendamento."Medico"(UsuarioId) ON DELETE CASCADE,
    EspecialidadeId INT REFERENCES agendamento."Especialidade"(Id) ON DELETE CASCADE,
    CreatedAt TIMESTAMP DEFAULT NOW() NOT NULL,
    PRIMARY KEY (MedicoId, EspecialidadeId)
);

-- Tabela de Agendamentos
CREATE TABLE agendamento."Consulta" (
    Id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    MedicoId UUID REFERENCES agendamento."Medico"(UsuarioId),
    PacienteId UUID NOT NULL REFERENCES agendamento."Paciente"(UsuarioId) ON DELETE CASCADE,
    EspecialidadeId INT NOT NULL REFERENCES agendamento."Especialidade"(id) ON DELETE CASCADE,
    Sintomas TEXT NOT NULL,
    EspecialidadeRecomendada VARCHAR(50),
    DataConsulta TIMESTAMP NOT NULL,
    Status VARCHAR(20) DEFAULT 'Agendada' CHECK(Status IN ('Agendada', 'Confirmada', 'Cancelada', 'Concluída')),
    CreatedAt TIMESTAMP DEFAULT NOW() NOT NULL,
    UpdatedAt TIMESTAMP DEFAULT NOW() NOT NULL,
    CONSTRAINT consulta_futura CHECK (DataAgendada > NOW())
);

-- Tabela de Refresh Tokens (mantida com nomes originais)
CREATE TABLE agendamento."RefreshToken" (
    Id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    Token TEXT NOT NULL UNIQUE,
    ExpiresAt TIMESTAMP NOT NULL,
    UsuarioId UUID NOT NULL REFERENCES agendamento."Usuario"(Id) ON DELETE CASCADE,
    CreatedAt TIMESTAMP DEFAULT NOW() NOT NULL,
    RevokedAt TIMESTAMP NULL,
    DeviceInfo TEXT NULL
);

-- Índices
CREATE INDEX idx_usuario_email ON agendamento."Usuario"(Email);
CREATE INDEX idx_paciente_cpf ON agendamento."Paciente"(CPF);
CREATE INDEX idx_medico_crm ON agendamento."Medico"(CRM);
CREATE INDEX idx_consulta_medico ON agendamento."Consulta"(MedicoId);
CREATE INDEX idx_consulta_paciente ON agendamento."Consulta"(PacienteId);
CREATE INDEX idx_consulta_data ON agendamento."Consulta"(DataAgendada);
CREATE INDEX idx_refresh_token_usuario ON agendamento."RefreshToken"(UsuarioId);

-- Gatilho para atualizar UpdatedAt (obrigatório)
CREATE OR REPLACE FUNCTION update_consulta_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_usuario_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_consulta_timestamp
BEFORE UPDATE ON agendamento."Consulta"
FOR EACH ROW
EXECUTE FUNCTION update_consulta_timestamp();

CREATE TRIGGER trigger_update_usuario_timestamp
BEFORE UPDATE ON agendamento."Usuario"
FOR EACH ROW

EXECUTE FUNCTION update_usuario_timestamp();
