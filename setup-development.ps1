# 🔧 Setup de Desenvolvimento - GesN
# Execute este script para configurar os secrets localmente

Write-Host "🚀 Configurando ambiente de desenvolvimento..." -ForegroundColor Green

# Navegar para o projeto
Set-Location "GesN.Web"

# Verificar se User Secrets está inicializado
Write-Host "📋 Verificando User Secrets..." -ForegroundColor Yellow
$userSecretsId = dotnet user-secrets list 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "🔧 Inicializando User Secrets..." -ForegroundColor Yellow
    dotnet user-secrets init
}

Write-Host "🔑 Configurando secrets de desenvolvimento..." -ForegroundColor Yellow

# Database Connection
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=./Data/Database/gesn.db;Cache=Shared;Pooling=false;"

# Email Settings (Development)
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.mailtrap.io"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:Username" "seu-username-mailtrap"
dotnet user-secrets set "EmailSettings:Password" "sua-senha-mailtrap"
dotnet user-secrets set "EmailSettings:SenderEmail" "dev@gesn.com"

# Google Workspace (Development) - SUBSTITUA COM SUAS CREDENCIAIS DE DEV
dotnet user-secrets set "GoogleWorkspace:IsEnabled" "false"
dotnet user-secrets set "GoogleWorkspace:ClientId" "SEU-DEV-CLIENT-ID.apps.googleusercontent.com"
dotnet user-secrets set "GoogleWorkspace:ClientSecret" "GOCSPX-SEU-DEV-CLIENT-SECRET"
dotnet user-secrets set "GoogleWorkspace:Domain" "seu-dominio-dev.com"

Write-Host ""
Write-Host "✅ Configuração concluída!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Para editar os secrets:" -ForegroundColor Cyan
Write-Host "   dotnet user-secrets set 'chave' 'valor'"
Write-Host ""
Write-Host "📋 Para listar os secrets:" -ForegroundColor Cyan
Write-Host "   dotnet user-secrets list"
Write-Host ""
Write-Host "🗑️ Para limpar os secrets:" -ForegroundColor Cyan
Write-Host "   dotnet user-secrets clear"
Write-Host ""
Write-Host "⚠️  LEMBRE-SE:" -ForegroundColor Red
Write-Host "   1. Substitua as credenciais Google pelos valores reais de desenvolvimento"
Write-Host "   2. Configure Mailtrap ou outro SMTP para testes de email"
Write-Host "   3. Nunca commite credentials no código!"
Write-Host ""

# Listar secrets configurados
Write-Host "🔍 Secrets atualmente configurados:" -ForegroundColor Blue
dotnet user-secrets list 