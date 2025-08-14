-- Especialidades médicas
INSERT INTO agendamento."Especialidade" (Nome) VALUES
('Cardiologia'),
('Dermatologia'),
('Ginecologia'),
('Ortopedia'),
('Pediatria'),
('Psiquiatria'),
('Clínico Geral');

-- Usuário admin (senha: Admin@123)
INSERT INTO agendamento."Usuario" (Nome, Email, SenhaHash, Perfil) VALUES
('Admin', 'admin@agendamento.com', '$2a$10$X5h./YjQbR3C5R7VXHtwEeI7zZJk8hNn6tTUX6Xo1VlN/sQ8WYQi', 'Medico');