# PLANO FINAL DE LIMPEZA SITE.CSS

## Status dos Domínios:

### ✅ Product Domain → ProductDomain.css 
**MIGRADO COMPLETO** (845 linhas + hierarchy classes já estão lá)

### ✅ Sales Domain → SalesDomain.css
**MIGRADO COMPLETO** (76 linhas específicas)

### ✅ Purchasing Domain → PurchasingDomain.css  
**MIGRADO COMPLETO** (20 linhas específicas)

### ✅ Production Domain → ProductionDomain.css
**USA APENAS BOOTSTRAP PADRÃO** (border-left-*, text-xs, etc. são utilities Bootstrap)

---

## CLASSES PARA REMOVER DO SITE.CSS:

### 🔴 PURCHASING DOMAIN (linhas 111-134) - 24 linhas
```css
.btn-sales { /* já em PurchasingDomain.css */ }
.btn-sales:hover { /* já em PurchasingDomain.css */ }
.venda-main { /* já em PurchasingDomain.css */ }
.venda-lateral-fixa { /* já em PurchasingDomain.css */ }
.venda-conteudo-dinamico { /* já em PurchasingDomain.css */ }
```

### 🔴 SALES DOMAIN (linhas 237-366) - 130 linhas
```css
.avatar-sm { /* já em SalesDomain.css */ }
.table td { /* já em SalesDomain.css */ }
.btn-group .btn { /* já em SalesDomain.css */ }
#ordersTable { /* já em SalesDomain.css */ }
.column-resize-line { /* já em SalesDomain.css */ }
.user-select-none { /* já em SalesDomain.css */ }
```

### 🔴 PRODUCT DOMAIN (linhas 258-595) - 338 linhas  
```css
/* Algolia Autocomplete - já em ProductDomain.css */
.aa-* { /* já em ProductDomain.css */ }
.algolia-autocomplete { /* já em ProductDomain.css */ }
.modal-dropdown { /* já em ProductDomain.css */ }
```

### 🔴 PRODUCT DOMAIN Hierarchies (linhas 597-637) - 41 linhas
```css
.hierarchy-row { /* já em ProductDomain.css */ }
@keyframes highlight-new { /* já em ProductDomain.css */ }
.newly-added { /* já em ProductDomain.css */ }
```

---

## TOTAL A REMOVER: 533 linhas

## RESULTADO FINAL SITE.CSS: ~104 linhas (só global/home/utilities)

### ✅ MANTER NO SITE.CSS:
- **Estrutura Global** (linhas 1-23): html, body base
- **Home Específico** (linhas 24-109): .home, .logo-gesn, etc.  
- **Utilitários Globais** (linhas 136-256): .spinner-border, .d-none, jQuery UI