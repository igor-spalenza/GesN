# Plano de Limpeza do site.css

## Classes Migradas e Status

### ✅ MIGRADAS PARA ProductDomain.css:
- Todas as classes de Product Domain (Product, ProductCategory, ProductComponent, ProductGroup, ProductComponentHierarchy, CompositeProductXHierarchy)
- **Total migrado**: ~845 linhas de CSS

### ✅ MIGRADAS PARA SalesDomain.css:
- `.avatar-sm` (Customer/Order específico)
- `.btn-group .btn` (Sales específico)
- `#ordersTable` (Orders DataTables específico)
- **Total migrado**: ~76 linhas de CSS

### ✅ MIGRADAS PARA PurchasingDomain.css:
- `.btn-sales`
- `.btn-sales:hover` 
- `.venda-main`
- `.venda-lateral-fixa`
- `.venda-conteudo-dinamico`
- **Total migrado**: ~20 linhas de CSS

## Classes que PODEM SER REMOVIDAS do site.css:

### 🔴 PURCHASING DOMAIN (linhas 111-134):
```css
.btn-sales {
    color: #fff;
    background-color: #007a86;
    border-color: #198754;
}

.btn-sales:hover {
    color: #fff;
    background-color: #005159;
    border-color: #146c43;
}

.venda-main {
    display: flex !important;
    width: 100%;
}

.venda-lateral-fixa {
    width: 20%;
}

.venda-conteudo-dinamico {
    width: 80%;
}
```
**AÇÃO**: Remover essas classes do site.css (já estão em PurchasingDomain.css)

### 🔴 SALES DOMAIN - Classes específicas já migradas:
**CONFIRMADO**: Essas classes EXISTEM no site.css e já foram migradas para SalesDomain.css:
- `.avatar-sm` (linha 237)
- `#ordersTable` e related (linhas 311-348)

**AÇÃO**: REMOVER essas classes do site.css

### 🔴 PRODUCT DOMAIN - Classes já migradas:
**CONFIRMADO**: Essas classes EXISTEM no site.css e já foram migradas para ProductDomain.css:
- Classes Algolia Autocomplete (linhas 271, 306, 370, 392, 415, 423, 443, 451, 459-465, 469-490, 587, 591)
- **IMPORTANTE**: Essas classes podem estar sendo usadas por Category autocomplete em Product forms

**AÇÃO**: CUIDADO - Verificar se Category autocomplete funciona sem essas classes no site.css

## Classes que DEVEM PERMANECER no site.css:

### ✅ GLOBAL/COMPARTILHADAS:
- Classes gerais de layout (.container-fluid personalizações)
- Classes de home (.home-*, .logo-*, etc.)
- Classes de navegação global
- Classes de componentes compartilhados
- Classes Bootstrap customizadas globais
- Classes de utilidades globais

## Próximos Passos:

1. **Identificar duplicatas**: Verificar se as classes migradas ainda existem no site.css
2. **Verificar dependências**: Confirmar que as classes são usadas APENAS pelos domínios migrados
3. **Executar remoção**: Remover classes seguras do site.css
4. **Testar**: Verificar que não houve quebras visuais

## IMPORTANTE:
- NÃO remover classes que possam ser usadas por outros domínios (Cliente, Pedido, Financial, Admin)
- NÃO remover classes globais/compartilhadas
- NÃO remover classes Home específicas
- SEMPRE verificar dependências antes de remover