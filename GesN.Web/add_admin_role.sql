-- Verificar se a role Admin já existe
BEGIN TRANSACTION;

-- Criar a role Admin se não existir
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (lower(hex(randomblob(16))), 'Admin', 'ADMIN', lower(hex(randomblob(16))));

-- Obter o ID da role Admin
SELECT @roleId := Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN';

-- Obter o ID do usuário
SELECT @userId := Id FROM AspNetUsers WHERE NormalizedEmail = 'IGOR.SPALENZA94@GMAIL.COM';

-- Adicionar o usuário à role Admin se já não estiver
INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
SELECT @userId, @roleId
WHERE NOT EXISTS (
    SELECT 1 FROM AspNetUserRoles 
    WHERE UserId = @userId AND RoleId = @roleId
);

COMMIT; 