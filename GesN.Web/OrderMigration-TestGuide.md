# 🧪 Guia de Teste - Migração Order para TypeScript/SCSS

## 🎯 OBJETIVO
Garantir que a migração do OrderManager mantenha **100% da funcionalidade existente**.

---

## ✅ CHECKLIST DE COMPATIBILIDADE

### **1. COMPILAÇÃO**
```bash
# Verificar se compila sem erros
dotnet build

# Arquivos esperados após build:
# wwwroot/js/OrderManager.js (compilado do TypeScript)
# wwwroot/css/OrderDomain.css (compilado do SCSS)
```

### **2. FUNCIONALIDADES CORE**

#### **📋 Grid de Orders**
- [ ] ✅ `ordersManager.carregarListaOrders()` carrega corretamente
- [ ] ✅ DataTable inicializa com configurações corretas
- [ ] ✅ Ordenação por número decrescente funciona
- [ ] ✅ Paginação (25 itens por página) funciona
- [ ] ✅ Tooltips reativam após redraw

#### **📐 Sistema de Redimensionamento**
- [ ] ✅ Handles aparecem nos headers das colunas
- [ ] ✅ Arrastar handle redimensiona coluna
- [ ] ✅ Duplo clique auto-ajusta largura
- [ ] ✅ Larguras são salvas no localStorage
- [ ] ✅ Larguras são restauradas ao recarregar
- [ ] ✅ Larguras mínimas são respeitadas

#### **🪟 Sistema de Modais**
- [ ] ✅ `ordersManager.novoOrderModal()` abre modal corretamente
- [ ] ✅ Modal título = "Novo Pedido"
- [ ] ✅ Modal size = "lg"
- [ ] ✅ Loading spinner aparece inicialmente
- [ ] ✅ Formulário carrega via `/Order/CreatePartial`

#### **🔍 Autocomplete de Customer**
- [ ] ✅ Campo CustomerName inicializa autocomplete
- [ ] ✅ Busca inicia com 2+ caracteres
- [ ] ✅ Endpoint correto: `/Customer/BuscaCustomerAutocomplete`
- [ ] ✅ Suggestions aparecem formatadas corretamente
- [ ] ✅ Seleção preenche CustomerName e CustomerId
- [ ] ✅ Floating label ativa/desativa corretamente

#### **📑 Sistema de Abas Múltiplas**
- [ ] ✅ `ordersManager.abrirEdicao(id)` cria nova aba
- [ ] ✅ Aba duplicada apenas ativa existente
- [ ] ✅ Contador e qtdAbasAbertas atualizam
- [ ] ✅ Botão X fecha aba corretamente
- [ ] ✅ Fechar última aba volta para aba principal
- [ ] ✅ Conteúdo carrega via `/Order/EditPartial/{id}`

#### **💾 Salvamento**
- [ ] ✅ `ordersManager.salvarNovoModal()` funciona
- [ ] ✅ FormData é enviada corretamente
- [ ] ✅ Botão fica disabled durante salvamento
- [ ] ✅ Sucesso fecha modal e abre aba de edição
- [ ] ✅ Erro exibe mensagem apropriada

#### **🗑️ Exclusão**
- [ ] ✅ `ordersManager.excluirPedido()` solicita confirmação
- [ ] ✅ Requisição DELETE enviada com token CSRF
- [ ] ✅ Sucesso recarrega grid
- [ ] ✅ Erro exibe mensagem apropriada

---

## 🔧 TESTES MANUAIS

### **Teste 1: Funcionalidade Básica**
```javascript
// Console do navegador
console.log(typeof ordersManager); // deve ser "object"
console.log(ordersManager.contador); // deve ser number
ordersManager.carregarListaOrders(); // deve funcionar sem erro
```

### **Teste 2: Sistema de Abas**
1. Abrir uma aba de edição
2. Verificar se `ordersManager.qtdAbasAbertas === 1`
3. Abrir mesma aba novamente → deve apenas ativar
4. Fechar aba → deve voltar para lista principal

### **Teste 3: Redimensionamento**
1. Carregar grid com dados
2. Arrastar handle de uma coluna
3. Recarregar página
4. Verificar se largura foi mantida

### **Teste 4: Modal + Autocomplete**
1. Clicar "Novo Pedido"
2. Modal deve abrir com loading
3. Formulário deve carregar
4. Campo Customer deve ter autocomplete funcionando
5. Digitar 2+ caracteres → deve buscar clientes

---

## 🐛 PROBLEMAS COMUNS E SOLUÇÕES

### **TypeScript não compila**
```bash
# Verificar tsconfig.json
# Verificar se todos os imports estão corretos
# Checar se interfaces estão importadas
```

### **SCSS não compila**
```bash
# Verificar se AspNetCore.SassCompiler está instalado
# Confirmar que arquivo está em Styles/ não wwwroot/
# Build o projeto: dotnet build
```

### **ordersManager não encontrado**
```javascript
// No console do navegador
window.ordersManager // deve existir
```

### **Autocomplete não funciona**
- Verificar se biblioteca autocomplete.js está carregada
- Confirmar endpoint `/Customer/BuscaCustomerAutocomplete` existe
- Checar console para erros JavaScript

### **Abas não funcionam**
- Verificar se Bootstrap 5 está carregado
- Confirmar seletores `#orderTabs` e `#orderTabsContent` existem
- Checar eventos click nos botões de fechar

---

## 📊 MÉTRICAS DE SUCESSO

### **✅ Critérios de Aprovação**
- **Funcionalidade**: 100% das funções existentes funcionam
- **Performance**: Tempo de resposta igual ou melhor
- **UI/UX**: Visual idêntico ou melhorado
- **Compatibilidade**: Zero breaking changes
- **Erro**: Nenhum erro no console

### **📈 Melhorias Esperadas**
- **IntelliSense**: Autocompletar funciona no desenvolvimento
- **Type Safety**: Erros detectados em compile time
- **Maintainability**: Código mais organizado e documentado
- **CSS**: Estilos mais organizados e reutilizáveis

---

## 🚀 DEPLOY CHECKLIST

### **Antes do Deploy:**
- [ ] ✅ Todos os testes manuais passaram
- [ ] ✅ Build em Release mode funciona
- [ ] ✅ Arquivos compilados estão sendo gerados
- [ ] ✅ JavaScript original pode ser removido (backup primeiro)

### **Durante o Deploy:**
1. **Fazer backup** do `Order.js` original
2. **Deploy** com novo TypeScript compilado
3. **Testar** funcionalidades críticas em produção
4. **Rollback** se necessário (restaurar Order.js original)

### **Após o Deploy:**
- [ ] ✅ Monitorar erros JavaScript por 24h
- [ ] ✅ Confirmar que performance não degradou
- [ ] ✅ Coletar feedback dos usuários
- [ ] ✅ Remover arquivos de backup após estabilização

---

## 📝 COMANDOS ÚTEIS

### **Build e Compilação**
```bash
# Build completo
dotnet build

# Apenas TypeScript (se necessário)
tsc

# Watch mode para desenvolvimento
tsc --watch

# Limpar e rebuild
dotnet clean && dotnet build
```

### **Debug**
```javascript
// Console do navegador - verificar estado
console.log('ordersManager:', ordersManager);
console.log('Abas abertas:', ordersManager.qtdAbasAbertas);
console.log('Contador:', ordersManager.contador);

// Testar função específica
ordersManager.carregarListaOrders();
```

---

**🎯 Lembre-se: A migração só está completa quando todos os itens do checklist estão ✅ e nenhuma funcionalidade foi perdida!**
