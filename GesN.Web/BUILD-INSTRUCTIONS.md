# 🔧 INSTRUÇÕES DE BUILD - TypeScript + SCSS

## 🚀 PRIMEIRO BUILD APÓS MIGRAÇÃO

### **1. Verificar Pacotes Instalados:**
```bash
# Confirmar que estes pacotes estão no .csproj:
# Microsoft.TypeScript.MSBuild
# AspNetCore.SassCompiler
```

### **2. Build Completo:**
```bash
# Limpar tudo primeiro
dotnet clean

# Build completo
dotnet build

# Em caso de erro, tentar:
dotnet build --verbosity detailed
```

### **3. Verificar Outputs Gerados:**
```
wwwroot/js/
├── OrderManager.js      ← Deve existir (compilado do TypeScript)
├── OrderManager.js.map  ← Source map para debugging

wwwroot/css/
├── OrderDomain.css      ← Deve existir (compilado do SCSS)
├── site.css             ← Deve existir (compilado do site.scss)
```

---

## 🔍 TROUBLESHOOTING

### **TypeScript não compila:**
```bash
# Verificar tsconfig.json existe na raiz
# Verificar se arquivos .ts estão na pasta TypeScript/
# Tentar compilar manualmente:
tsc
```

### **SCSS não compila:**
```bash
# Verificar se arquivos .scss estão na pasta Styles/
# Verificar AspNetCore.SassCompiler instalado
# Logs de build podem mostrar erros SCSS
```

### **Erros de Build:**
- Verificar se todos os `import` statements estão corretos
- Checar se interfaces estão sendo importadas corretamente
- Verificar sintaxe SCSS (especialmente imports)

---

## ✅ VALIDAÇÃO PÓS-BUILD

### **1. Arquivos Gerados:**
```bash
# Verificar se existem:
ls wwwroot/js/OrderManager.js
ls wwwroot/css/OrderDomain.css
ls wwwroot/css/site.css
```

### **2. Teste no Navegador:**
1. Abrir aplicação
2. Ir para página de Orders
3. Console do navegador: `typeof ordersManager` deve ser `"object"`
4. Testar função: `ordersManager.carregarListaOrders()`

### **3. Funcionalidade Crítica:**
- [ ] ✅ Grid carrega
- [ ] ✅ Modal "Novo Pedido" abre
- [ ] ✅ Autocomplete funciona
- [ ] ✅ Abas funcionam
- [ ] ✅ Redimensionamento funciona

---

## 📋 CHECKLIST FINAL

- [ ] ✅ Build sem erros
- [ ] ✅ OrderManager.js gerado
- [ ] ✅ CSS compilados gerados
- [ ] ✅ Aplicação roda sem erros
- [ ] ✅ Funcionalidades básicas funcionam
- [ ] ✅ Console sem erros JavaScript

**🎉 Se todos os itens estão ✅, a migração está pronta!**


