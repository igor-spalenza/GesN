-- Criar a role Admin se não existir
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES ('1', 'Admin', 'ADMIN', '1');

-- Buscar o ID do usuário
-- Substitua este SELECT por uma consulta direta se você souber o ID do usuário
-- SELECT Id FROM AspNetUsers WHERE NormalizedEmail = 'IGOR.SPALENZA94@GMAIL.COM';

-- Adicionar o usuário à role Admin (substitua 'USER_ID_HERE' pelo ID do usuário)
-- INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId) VALUES ('USER_ID_HERE', '1'); 