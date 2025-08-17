# 📋 MAPEAMENTO COMPLETO CSS - SITE.CSS

## 🎯 OBJETIVO
Mapear todas as classes CSS em `site.css` e identificar onde são utilizadas para permitir migração estratégica para arquivos especializados por domínio.

---

## 🔍 CLASSES IDENTIFICADAS E SUAS UTILIZAÇÕES

### **📦 DOMÍNIO PRODUCT (Candidatas para ProductDomain.css)**

#### **1. Floating Labels System**
- **Classes**: `.floating-input-group`, `.floating-input`, `.floating-label`, `.floating-textarea`
- **Localização no site.css**: Linhas 494-509
- **Utilizadas em**:
  - ✅ `Product/_Create.cshtml` (19 ocorrências)
  - ✅ `Product/_EditBasicData.cshtml` (múltiplas)
  - ✅ `ProductComponent/_CreateComponent.cshtml` (9 ocorrências) 
  - ✅ `ProductComponent/_EditComponent.cshtml` (10 ocorrências)
  - ✅ `ProductComponentHierarchy/_CreateHierarchy.cshtml` (4 ocorrências)
  - ✅ `ProductComponentHierarchy/_CreateCompositeProductXHierarchy.cshtml` (7 ocorrências)
  - ✅ `ProductComponentHierarchy/_EditCompositeHierarchyRelation.cshtml` (5 ocorrências)
  - ✅ `ProductGroup/_CreateGroupItem.cshtml` (11 ocorrências)
  - ✅ `ProductGroup/_EditGroupItem.cshtml` (10 ocorrências)
  - ✅ `ProductGroup/_CreateGroupExchangeRule.cshtml` (8 ocorrências)
  - ✅ `ProductGroup/_EditGroupExchangeRule.cshtml` (5 ocorrências)

#### **2. Algolia Autocomplete System**
- **Classes**: `.aa-autocomplete`, `.aa-dropdown-menu`, `.aa-suggestion`, `.aa-input`, `.aa-empty`, `.modal-dropdown`, `.force-visible`
- **Localização no site.css**: Linhas 373-492 + 428-492
- **Utilizadas em**:
  - ✅ `Product/_Create.cshtml` (category-autocomplete aa-input)
  - ✅ `Product/_EditBasicData.cshtml` (category-autocomplete aa-input)
  - ✅ `ProductComponentHierarchy/_CreateCompositeProductXHierarchy.cshtml` (autocomplete-input)
  - ✅ `ProductComponent/_CreateComponent.cshtml` (autocomplete-input)
  - ✅ `ProductComponent/_EditComponent.cshtml` (autocomplete-input)

#### **3. Product Form Cards**
- **Classes**: `.product-form-card`
- **Localização no site.css**: Linhas 521-523
- **Utilizadas em**:
  - ✅ `Product/_Create.cshtml` (2 ocorrências)
  - ✅ `Product/_EditBasicData.cshtml` (4 ocorrências)

#### **4. Product Edit Container**
- **Classes**: `.product-edit-container`
- **Utilizadas em**:
  - ✅ `Product/_Edit.cshtml` (1 ocorrência)

#### **5. Hierarchy Animations & States**
- **Classes**: `.hierarchy-row`, `.table-warning`, `.table-danger`, `.loading`, `.newly-added`, `@keyframes highlight-new`
- **Localização no site.css**: Linhas 536-575
- **Utilizadas em**: JavaScript (CompositeProduct.js, ProductComponentHierarchy.js)

---

### **📊 CLASSES GERAIS (Permanecer em site.css)**

#### **1. Layout Base**
- **Classes**: `html`, `body`, `.home`, `.logo-gesn`, `.btn-lg-home`
- **Utilizadas em**: Views globais, _Layout.cshtml

#### **2. Components Globais**
- **Classes**: `.spinner-border`, `.d-none`, `.toast-top-right`
- **Utilizadas em**: Múltiplas views e JavaScript

#### **3. Table & DataTables**
- **Classes**: `.table td`, `.btn-group .btn`, `#ordersTable`
- **Utilizadas em**: Views de listagem geral

#### **4. UI Autocomplete (Legacy - jQuery UI)**
- **Classes**: `.ui-autocomplete`, `.ui-menu`, `.autocomplete-modal-menu`
- **Localização no site.css**: Linhas 155-240
- **Status**: ⚠️ Legacy - verificar se ainda é usado

---

## 🚨 CLASSES PROBLEMÁTICAS (Duplicações/Conflitos)

### **1. Floating Labels Duplicadas**
- **Problema**: Definições inline em algumas views (ex: `_EditCompositeHierarchyRelation.cshtml` linhas 182-239)
- **Conflito**: Sobrescreve estilos do site.css
- **Solução**: Remover definições inline, centralizar no ProductDomain.css

### **2. Algolia Autocomplete Duplicada**
- **Problema**: Definições inline em `_CreateCompositeProductXHierarchy.cshtml` linhas 227-267
- **Conflito**: CSS duplicado e conflitante
- **Solução**: Consolidar no ProductDomain.css

---

## 📋 ESTRATÉGIA DE MIGRAÇÃO

### **🎯 FASE 1: Preparação (SEM QUEBRAR NADA)**
1. ✅ **Criar ProductDomain.css** (feito)
2. ✅ **Referenciar nas views Product** 
3. ✅ **Copiar classes do site.css para ProductDomain.css** (não mover ainda)
4. ✅ **Testar todas as funcionalidades**

### **🎯 FASE 2: Consolidação (REMOVER DUPLICAÇÕES)**
1. **Remover CSS inline** das views individuais
2. **Substituir por classes no ProductDomain.css**
3. **Teste funcionalidade por funcionalidade**

### **🎯 FASE 3: Migração Final (LIMPEZA)**
1. **Remover classes migradas do site.css**
2. **Validação completa de todas as views**
3. **Documentar mudanças**

---

## 📦 ARQUIVOS DE REFERÊNCIA NECESSÁRIOS

### **Views que precisam referenciar ProductDomain.css:**
```html
<link href="~/css/ProductDomain.css" rel="stylesheet" />
```

**Lista de views:**
- Product/_Create.cshtml ✅ (já tem product-form.css)
- Product/_Edit.cshtml ✅ (já tem product-form.css)
- Product/_EditBasicData.cshtml ❌ (adicionar)
- ProductComponent/_CreateComponent.cshtml ❌ (adicionar)
- ProductComponent/_EditComponent.cshtml ❌ (adicionar)
- ProductComponentHierarchy/_CreateHierarchy.cshtml ❌ (adicionar)  
- ProductComponentHierarchy/_CreateCompositeProductXHierarchy.cshtml ❌ (adicionar)
- ProductComponentHierarchy/_EditCompositeHierarchyRelation.cshtml ❌ (adicionar)
- ProductGroup/_CreateGroupItem.cshtml ❌ (adicionar)
- ProductGroup/_EditGroupItem.cshtml ❌ (adicionar)
- ProductGroup/_CreateGroupExchangeRule.cshtml ❌ (adicionar)
- ProductGroup/_EditGroupExchangeRule.cshtml ❌ (adicionar)

---

## ⚠️ RISCOS IDENTIFICADOS

1. **Alto Impacto**: Floating labels são usadas em 12+ views
2. **Dependências JavaScript**: CompositeProduct.js referencia classes CSS
3. **CSS Inline**: Definições duplicadas em views podem causar conflitos
4. **Teste Necessário**: Todas as combinações modal/tab/form precisam ser testadas

---

## 🎯 RECOMENDAÇÃO

**COMEÇAR IMPLEMENTANDO A FASE 1**:
1. Adicionar referência ao ProductDomain.css nas views que faltam
2. Consolidar todas as classes Product no ProductDomain.css 
3. Testar uma view por vez
4. Só depois remover do site.css

**PRIORIDADE**:
1. 🔥 **Alta**: Autocomplete (problemas ativos)
2. 🔶 **Média**: Floating labels (impacto visual)
3. 🔵 **Baixa**: Animações (funcionalidade secundária)