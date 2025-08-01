# 📊 MAPEAMENTO COMPLETO CSS - DOMÍNIO DE VENDAS

## 🎯 DOMÍNIO DE VENDAS DEFINIDO
- **Customer** (views: Index, _Create, _Edit, _Details, _ListaClientes)
- **Order/OrderEntry** (views: Index, _Create, _Edit, _Details, _Grid, _OrderItems, _ListaOrders)
- **Contract** (❌ sem views)

❌ **Cliente NÃO é domínio Sales** (excluído da análise)

---

## 📋 CLASSES MAPEADAS DO SITE.CSS (637 linhas)

### **🏠 CLASSES HOME (❌ NÃO MIGRAR)**
- `.home` (linha 24)
- `.home-logo` (linha 31)
- `#logo` (linha 40)
- `.logo-gesn` (linha 45)
- `.logo-cliente` (linha 99)

### **🎯 CLASSES GERAIS DO SISTEMA**
- `html` (linhas 1, 15)
- `body` (linha 20)
- `.btn:focus, .btn:active:focus, .btn-link.nav-link:focus, .form-control:focus, .form-check-input:focus` (linha 11)
- `.btn-lg-home` (linha 105)
- `.btn-sales` (linha 111)
- `.btn-sales:hover` (linha 117)
- `.venda-main` (linha 123)
- `.venda-lateral-fixa` (linha 128)
- `.venda-conteudo-dinamico` (linha 132)
- `.vendas-principal` (linha 136)
- `.spinner-border` (linha 142)
- `.d-none` (linha 146)
- `.toast-top-right` (linha 158)
- `#barra-btns` (linha 162)

### **🔍 CLASSES AUTOCOMPLETE (jQuery UI)**
- `.ui-autocomplete` (linhas 151, 167)
- `.ui-menu` (linha 177)
- `.ui-menu-item` (linha 184)
- `.ui-menu-item div` (linha 190)
- `.ui-state-active, .ui-widget-content .ui-state-active` (linha 195)
- `.autocomplete-modal-menu` (linha 203)
- `.autocomplete-modal-menu .ui-menu-item` (linha 216)
- `.autocomplete-modal-menu .ui-menu-item-wrapper` (linha 222)
- `.autocomplete-modal-menu .ui-menu-item-wrapper:hover, .autocomplete-modal-menu .ui-state-active` (linha 228)

### **👥 CLASSES CUSTOMER TABLE (POSSÍVEL MIGRAÇÃO)**
- `.avatar-sm` (linha 238) 🔥
- `.table td` (linha 245) 🔥
- `.btn-group .btn` (linha 249) 🔥
- `.btn-group .btn:last-child` (linha 254) 🔥

### **🔍 CLASSES ALGOLIA AUTOCOMPLETE (POSSÍVEL MIGRAÇÃO)**
- `.algolia-autocomplete` (linhas 259, 371)
- `.algolia-autocomplete .aa-input, .algolia-autocomplete .aa-hint` (linha 263)
- `.algolia-autocomplete .aa-hint` (linha 268)
- `.algolia-autocomplete .aa-dropdown-menu` (linha 272)
- `.algolia-autocomplete .aa-suggestion` (linha 284)
- `.algolia-autocomplete .aa-suggestion:hover, .algolia-autocomplete .aa-suggestion.aa-cursor` (linha 291)
- `.algolia-autocomplete .aa-suggestion:last-child` (linha 297)
- `.algolia-autocomplete .aa-suggestion em` (linha 301)
- `.modal .algolia-autocomplete .aa-dropdown-menu` (linha 307)

### **📦 CLASSES ORDERS TABLE (POSSÍVEL MIGRAÇÃO)**
- `#ordersTable` (linha 312) 🔥
- `#ordersTable th` (linha 316) 🔥
- `#ordersTable th:hover` (linha 321) 🔥
- `#ordersTable th.resizing` (linha 325) 🔥
- `#ordersTable th .resize-handle` (linha 330) 🔥
- `#ordersTable th .resize-handle:hover` (linha 341) 🔥
- `#ordersTable td` (linha 345) 🔥
- `.column-resize-line` (linha 352) 🔥
- `.user-select-none` (linha 363) 🔥

### **🔍 CLASSES ALGOLIA AUTOCOMPLETE AVANÇADO**
- `.aa-autocomplete` (linha 371)
- `.category-autocomplete.aa-input` (linha 378)
- `.category-autocomplete.aa-input:focus` (linha 386)
- `.aa-dropdown-menu` (linha 393)
- `.modal .aa-dropdown-menu` (linha 416)
- `.aa-dropdown-menu.modal-dropdown` (linha 424)
- `.aa-dropdown-menu.force-visible` (linha 444)
- [... muitas outras classes aa-* ...]

### **🏗️ CLASSES HIERARQUIA/ANIMAÇÕES**
- `.hierarchy-row` (linha 600)
- `.hierarchy-row.table-warning` (linha 605)
- `.hierarchy-row.table-danger` (linha 611)
- `.hierarchy-row.loading` (linha 616)
- `@keyframes highlight-new` (linha 622)
- `.hierarchy-row.newly-added` (linha 633)

---

## 📋 CLASSES MAPEADAS DO PRODUCT-FORM.CSS (972 linhas)

### **📊 PRODUCT TABLE BADGES**
- `#productsTable .badge` (linha 4)
- `#productsTable .badge.bg-primary` (linha 17)
- `#productsTable .badge.bg-info` (linha 22)
- `#productsTable .badge.bg-success` (linha 27)
- `#productsTable .badge.bg-secondary` (linha 32)
- `#productsTable .badge.bg-light` (linha 37)
- `#productsTable .badge.bg-danger` (linha 43)

### **🏷️ FLOATING LABELS SYSTEM**
- `.floating-input-group` (linha 48)
- `.floating-input-group .form-text` (linha 53)
- `.floating-input` (linha 58)
- `.floating-input:focus` (linha 69)
- `.floating-input.has-value, .floating-input:focus` (linha 74)
- `.floating-label` (linha 80)
- `.floating-input:focus + .floating-label, .floating-input.has-value + .floating-label` (linha 91)
- `.floating-input.is-invalid` (linha 99)
- `.floating-input.is-invalid:focus` (linha 103)
- `.floating-input.is-invalid + .floating-label` (linha 107)
- `.floating-select` (linha 112)
- `.floating-textarea` (linha 146)
- [... muitas outras variações ...]

### **🔗 INPUT GROUPS & ADDONS**
- `.floating-input-group-addon` (linha 180)
- `.floating-input-group-addon .input-group-text` (linha 186)
- [... múltiplas variações ...]

### **🎨 SELECT2 INTEGRATION**
- `.floating-input-group .select2-container` (linha 300)
- `.floating-input-group .select2-selection` (linha 304)
- [... muitas classes select2 ...]

### **🔍 ALGOLIA AUTOCOMPLETE**
- `.algolia-autocomplete, .floating-input-group .autocomplete-container` (linha 429)
- [... muitas classes aa-* duplicadas do site.css ...]

### **📦 PRODUCT CARDS & STYLING**
- `.product-form-card` (linha 520)
- `.product-form-card .card-header` (linha 527)
- `.product-form-card .card-body` (linha 540)

### **🎛️ MODERN COMPONENTS**
- `.modern-switch` (linha 565)
- `.modern-switch:hover` (linha 576)
- `.modern-switch .form-check-input` (linha 580)
- `.modern-switch .form-check-label` (linha 587)

### **📱 MODAL & RESPONSIVE**
- `.modal-content` (linha 615)
- `.modal-header` (linha 621)
- `.modal-footer` (linha 627)
- `@keyframes floatUp` (linha 634)
- `@keyframes floatDown` (linha 645)

### **🏢 PRODUCT GROUP SECTIONS**
- `.product-group-section` (linha 657)
- [... muitas classes relacionadas ...]

### **⚙️ COMPONENT MANAGEMENT**
- `.stats-card` (linha 803)
- `.component-table .table-responsive` (linha 813)
- `.component-management .btn-group .btn` (linha 818)

### **🔄 EXCHANGE RULES**
- `.exchange-rules-list` (linha 873)
- `.exchange-rule-card` (linha 878)
- `#itemExchangeRulesModal .modal-dialog` (linha 891)
- `#groupItemsContainer .table th:nth-child(6)` (linha 906)

---

## ⏳ AGUARDANDO VERIFICAÇÃO DE USO

**🔍 PRÓXIMO PASSO:** Verificar sistematicamente quais dessas classes são REALMENTE usadas nas views:

### **👥 Customer Views:**
- `Customer/Index.cshtml`
- `Customer/_Create.cshtml`
- `Customer/_Edit.cshtml`
- `Customer/_Details.cshtml`
- `Customer/_ListaClientes.cshtml`

### **📦 Order Views:**
- `Order/Index.cshtml`
- `Order/_Create.cshtml`
- `Order/_Edit.cshtml`
- `Order/_Details.cshtml`
- `Order/_Grid.cshtml`
- `Order/_OrderItems.cshtml`
- `Order/_ListaOrders.cshtml`

### **🔥 CSS INLINE IDENTIFICADO:**
- `Order/_Grid.cshtml` (linhas 135-159) - **DUPLICA .avatar-sm e #ordersTable**

---

## 🎯 ESTRATÉGIA DE VERIFICAÇÃO

1. **🔍 Buscar cada classe nas views** Customer e Order
2. **📊 Catalogar uso real** vs definições
3. **🏗️ Identificar dependências** entre classes
4. **📋 Criar plano de migração** preciso
5. **⚠️ Mapear conflitos** entre arquivos

**STATUS:** Mapeamento completo de definições ✅ | Verificação de uso ⏳**