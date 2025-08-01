# 🛒 ANÁLISE COMPLETA - DOMÍNIO DE VENDAS CSS

## 🎯 ENTIDADES DO DOMÍNIO DE VENDAS
- **Customer** (Clientes)
- **OrderEntry** (Pedidos/Orders) 
- **Contract** (Contratos - apenas backend, sem views)

---

## 📂 VIEWS IDENTIFICADAS

### **👥 Customer/Cliente:**
- ✅ `Customer/Index.cshtml`
- ✅ `Customer/_Create.cshtml`
- ✅ `Customer/_Edit.cshtml` 
- ✅ `Customer/_Details.cshtml`
- ✅ `Customer/_ListaClientes.cshtml`
- ✅ `Cliente/Index.cshtml`
- ✅ `Cliente/Create.cshtml`
- ✅ `Cliente/Edit.cshtml`
- ✅ `Cliente/Details.cshtml`

### **📦 Order/OrderEntry:**
- ✅ `Order/Index.cshtml`
- ✅ `Order/_Create.cshtml`
- ✅ `Order/_Edit.cshtml`
- ✅ `Order/_Details.cshtml`
- ✅ `Order/_Grid.cshtml`
- ✅ `Order/_OrderItems.cshtml`
- ✅ `Order/_ListaOrders.cshtml`

### **📄 Contract:**
- ❌ **Sem views identificadas** (apenas controller/service/repository)

---

## 🔍 CLASSES CSS ESPECÍFICAS DO DOMÍNIO DE VENDAS

### **🎯 CLASSES PARA MIGRAR (site.css → SalesDomain.css)**

#### **1. Customer Table Styles (site.css linhas 242-261)**
```css
/* ===== CUSTOMER/CLIENT AVATARS ===== */
.avatar-sm {
    width: 32px;
    height: 32px;
    font-size: 0.875rem;
    font-weight: 600;
}

/* ===== TABLE ADJUSTMENTS FOR SALES ===== */
.table td {
    vertical-align: middle;
}

.btn-group .btn {
    border-radius: 0.25rem;
    margin-right: 2px;
}

.btn-group .btn:last-child {
    margin-right: 0;
}
```

#### **2. Orders Table DataTables (site.css linhas 316-370)**
```css
/* ===== ORDERS TABLE RESIZING ===== */
#ordersTable {
    table-layout: fixed;
}

#ordersTable th {
    position: relative;
    user-select: none;
}

#ordersTable th:hover {
    border-right: 2px solid #007bff;
}

#ordersTable th.resizing {
    border-right: 2px solid #007bff;
    background-color: rgba(0, 123, 255, 0.1);
}

#ordersTable th .resize-handle {
    position: absolute;
    top: 0;
    right: 0;
    width: 10px;
    height: 100%;
    cursor: col-resize;
    background: transparent;
    z-index: 10;
}

#ordersTable th .resize-handle:hover {
    background: rgba(0, 123, 255, 0.2);
}

#ordersTable td {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

/* ===== COLUMN RESIZE INDICATOR ===== */
.column-resize-line {
    position: absolute;
    top: 0;
    width: 2px;
    background-color: #007bff;
    z-index: 1000;
    pointer-events: none;
    opacity: 0.8;
}

.user-select-none {
    user-select: none !important;
}
```

#### **3. Logo Cliente (site.css linhas 104-108)**
```css
/* ===== CLIENT LOGO ===== */
.logo-cliente {
    margin-top: 50px;
    margin-left: 30px;
    width: 90%;
}
```

### **🔥 CSS INLINE PARA MIGRAR**

#### **Order/_Grid.cshtml (linhas 135-159) - DUPLICADO**
```css
/* CONFLITO: Duplica definições do site.css */
.avatar-sm {
    width: 32px;        /* ✅ Igual ao site.css */
    height: 32px;       /* ✅ Igual ao site.css */
    font-size: 0.75rem; /* ❌ DIFERENTE: site.css usa 0.875rem */
    font-weight: bold;  /* ❌ DIFERENTE: site.css usa 600 */
}

#ordersTable {
    font-size: 0.9rem;  /* ✅ Específico - manter */
}

#ordersTable th {
    border-color: #495057; /* ✅ Específico - manter */
    font-weight: 600;      /* ✅ Específico - manter */
}

#ordersTable td {
    vertical-align: middle; /* ❌ DUPLICADO do site.css */
}

.btn-group .btn {
    margin: 0 1px;  /* ❌ CONFLITO: site.css usa margin-right: 2px */
}
```

---

## 📊 CLASSES BOOTSTRAP PADRÃO (NÃO MIGRAR)

### **🎯 Classes Bootstrap Usadas:**
- `.card`, `.card-header`, `.card-body`, `.card-title`
- `.btn`, `.btn-primary`, `.btn-success`, `.btn-outline-*`
- `.form-control` (usado em todos os formulários)
- `.table`, `.table-striped`, `.table-hover`, `.table-dark`
- `.container-fluid`, `.row`, `.col-*`
- `.bg-*`, `.text-*`, `.rounded-circle`

**❌ Não devem ser migradas** - são classes padrão do Bootstrap

---

## ⚠️ CONFLITOS IDENTIFICADOS

### **🔥 CONFLITO 1: .avatar-sm**
- **site.css**: `font-size: 0.875rem; font-weight: 600;`
- **Order/_Grid.cshtml**: `font-size: 0.75rem; font-weight: bold;`
- **Solução**: Unificar definição no SalesDomain.css

### **🔥 CONFLITO 2: .btn-group .btn**
- **site.css**: `margin-right: 2px;`
- **Order/_Grid.cshtml**: `margin: 0 1px;`
- **Solução**: Definir regra específica para orders

### **🔥 CONFLITO 3: #ordersTable td**
- **site.css**: `vertical-align: middle;` (genérico)
- **Order/_Grid.cshtml**: `vertical-align: middle;` (duplicado)
- **Solução**: Remover duplicação

---

## 🎯 ESTRATÉGIA DE MIGRAÇÃO

### **📋 FASE 1: CONSOLIDAÇÃO**
1. **✅ Migrar classes específicas** do site.css → SalesDomain.css
2. **✅ Resolver conflitos** entre site.css e CSS inline
3. **✅ Remover CSS inline** das views

### **📋 FASE 2: REFERÊNCIAS**
1. **✅ Adicionar SalesDomain.css** nos Index:
   - `Customer/Index.cshtml`
   - `Order/Index.cshtml`
   - `Cliente/Index.cshtml`

### **📋 FASE 3: VALIDAÇÃO**
1. **✅ Testar layouts** de todas as views
2. **✅ Verificar tabelas** (customers e orders)
3. **✅ Confirmar avatars** e botões

---

## 📝 CLASSES A MIGRAR (RESUMO)

### **🔥 DO SITE.CSS:**
- `.logo-cliente` (linhas 104-108)
- `.avatar-sm` (linhas 243-248)
- `.table td` contexto vendas (linhas 250-252)
- `.btn-group .btn` contexto vendas (linhas 254-261)
- `#ordersTable` e relacionadas (linhas 317-354)
- `.column-resize-line` (linhas 357-365)
- `.user-select-none` (linhas 368-370)

### **🔥 CSS INLINE PARA REMOVER:**
- `Order/_Grid.cshtml` (linhas 135-159) - 25 linhas

---

## ⚠️ ATENÇÃO

### **❌ NÃO REMOVER DO SITE.CSS AINDA:**
Classes como `.table td`, `.btn-group .btn` podem ser usadas em outros domínios. **Verificar uso antes de remover!**

### **✅ SEGUROS PARA REMOÇÃO:**
- `.logo-cliente` (específico de cliente)
- `#ordersTable` (específico de orders)
- `.column-resize-line` (específico de DataTables orders)

---

## 🎯 RESULTADO ESPERADO

### **📁 SalesDomain.css Final:**
- **~100 linhas** de CSS específico de vendas
- **0 conflitos** entre definições
- **0 CSS inline** nas views
- **Suporte completo** para Customer/Order funcionalidades

### **🧹 Views Limpas:**
- **1 arquivo** `Order/_Grid.cshtml` sem CSS inline
- **Referências corretas** nos 3 arquivos Index

---

## ✅ STATUS

**📋 ANÁLISE: 100% COMPLETA**
**🎯 PRÓXIMO PASSO: Executar migração das classes identificadas**