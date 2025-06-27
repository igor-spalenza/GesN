# 🔒 Configuração de Segurança - GesN

## ⚡ **SETUP RÁPIDO**

### **Para Desenvolvimento:**
```powershell
# Execute o script de setup
.\setup-development.ps1
```

### **Para Produção:**
Configure as variáveis de ambiente no servidor (veja seção abaixo).

---

## 🛠️ **Configuração Detalhada**

### 1. **Desenvolvimento Local** 

Usamos **User Secrets** (secrets.json) que é **100% seguro**:

```bash
cd GesN.Web

# Inicializar User Secrets (apenas uma vez)
dotnet user-secrets init

# Configurar suas credenciais de desenvolvimento
dotnet user-secrets set "GoogleWorkspace:ClientId" "seu-dev-client-id.apps.googleusercontent.com"
dotnet user-secrets set "GoogleWorkspace:ClientSecret" "GOCSPX-seu-dev-client-secret"
dotnet user-secrets set "EmailSettings:Username" "seu-email-dev"
dotnet user-secrets set "EmailSettings:Password" "sua-senha-dev"

# Listar todos os secrets
dotnet user-secrets list
```

#### 📁 **Onde ficam os secrets?**
- **Windows:** `%APPDATA%\Microsoft\UserSecrets\{UserSecretsId}\secrets.json`
- **macOS/Linux:** `~/.microsoft/usersecrets/{UserSecretsId}/secrets.json`

🔒 **Este arquivo NUNCA vai para o Git!**

### 2. **Produção** 

Configure as **variáveis de ambiente** no servidor:

```bash
# Database
export ConnectionStrings__DefaultConnection="sua-connection-string-producao"

# Email SMTP
export EmailSettings__SmtpServer="smtp.seu-provedor.com"
export EmailSettings__SmtpPort="587"
export EmailSettings__Username="seu-email@dominio.com"
export EmailSettings__Password="sua-senha-smtp"
export EmailSettings__SenderEmail="noreply@seu-dominio.com"

# Google Workspace (OAuth)
export GoogleWorkspace__IsEnabled="true"
export GoogleWorkspace__ClientId="seu-client-id-prod.apps.googleusercontent.com"
export GoogleWorkspace__ClientSecret="GOCSPX-seu-client-secret-prod"
export GoogleWorkspace__Domain="seu-dominio.com"
```

### 3. **Visual Studio Connected Services**

Ao usar Connected Services no Visual Studio:

1. **Clique direito** no projeto → **Add** → **Connected Service**
2. **Escolha** o serviço (Google, Azure, etc.)
3. O Visual Studio **automaticamente** salva no `secrets.json`
4. **Nunca** será commitado!

---

## 🛡️ **Arquivos de Segurança**

### ✅ **Arquivos SEGUROS (podem ser commitados):**
```
- appsettings.json (apenas valores padrão)
- appsettings.Production.json (apenas estrutura/placeholders)
- .gitignore (configurado corretamente)
```

### ❌ **Arquivos PROIBIDOS (NUNCA commitar):**
```
- secrets.json (User Secrets)
- appsettings.Development.json (se tiver credenciais)
- TokenStore/ (tokens OAuth)
- *.token, *.refresh_token
- service-account*.json
- credentials*.json
- .env files
```

---

## 🚀 **Como Funciona**

### **Ordem de Prioridade das Configurações:**
1. **Environment Variables** (produção)
2. **User Secrets** (desenvolvimento)
3. **appsettings.{Environment}.json**
4. **appsettings.json** (valores padrão)

### **Exemplo de uso no código:**
```csharp
// Funciona automaticamente em dev e produção!
var googleClientId = Configuration["GoogleWorkspace:ClientId"];
var connectionString = Configuration.GetConnectionString("DefaultConnection");
```

---

## ⚠️ **Ações de Emergência**

### **Se credenciais foram expostas:**

1. **REVOGUE imediatamente:**
   - Google Console → APIs & Services → Credentials
   - Delete a credencial exposta

2. **Gere novas credenciais**

3. **Atualize:**
   - Dev: `dotnet user-secrets set "chave" "novo-valor"`
   - Prod: Atualize as environment variables

4. **Limpe o histórico Git se necessário:**
```bash
git filter-branch --force --index-filter 'git rm --cached --ignore-unmatch TokenStore/*' --prune-empty --tag-name-filter cat -- --all
```

---

## 🎯 **Checklist de Segurança**

- [ ] ✅ User Secrets configurado para desenvolvimento
- [ ] ✅ Environment Variables configuradas na produção  
- [ ] ✅ .gitignore atualizado
- [ ] ✅ TokenStore/ removido e ignorado
- [ ] ✅ Credenciais antigas revogadas
- [ ] ✅ appsettings.json sem credenciais
- [ ] ✅ Connected Services configurado (se aplicável)
- [ ] ✅ Pipeline de deploy configurado
- [ ] ✅ Documentação atualizada

---

## 🔧 **Comandos Úteis**

```bash
# Ver todos os secrets
dotnet user-secrets list

# Remover um secret específico
dotnet user-secrets remove "chave"

# Limpar todos os secrets
dotnet user-secrets clear

# Ver onde estão os secrets
dotnet user-secrets list --verbose
```

💡 **Dica:** Use o script `setup-development.ps1` para configuração inicial! 