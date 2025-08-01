# Análise CSS - Production e Purchasing Domains

## Production Domain

### Entities: Demand, ProductionOrder
### Views analisadas: Index, _Grid de cada entidade

### Classes específicas identificadas:

#### Demand (Views/Demand/*)
- `.border-left-warning`, `.border-left-info`, `.border-left-primary`, `.border-left-secondary`, `.border-left-success`, `.border-left-danger` - **NÃO ENCONTRADAS** nos CSS customizados (presumivelmente Bootstrap utilities)
- Demais classes são Bootstrap padrão: `table-responsive`, `table-hover`, `shadow`, etc.

#### ProductionOrder (Views/ProductionOrder/*)
- `.small-box`, `.small-box-footer` - **NÃO ENCONTRADAS** nos CSS customizados (presumivelmente AdminLTE)
- `.bg-info`, `.bg-success`, `.bg-warning`, `.bg-secondary` - Bootstrap utilities
- Badges: `badge-*` - Bootstrap padrão

### **CONCLUSÃO PRODUCTION DOMAIN:**
- **NÃO HÁ CLASSES CUSTOMIZADAS PARA MIGRAR**
- Todas as classes são Bootstrap padrão ou AdminLTE (externa)
- ProductionDomain.css seria praticamente vazio

---

## Purchasing Domain

### Entities: Ingredient, Supplier
### Views analisadas: Index, _Grid de cada entidade

### Classes específicas identificadas:

#### Layout classes (site.css)
```css
.venda-main {
    /* definição no site.css */
}

.venda-lateral-fixa {
    /* definição no site.css */
}

.venda-conteudo-dinamico {
    /* definição no site.css */
}

.btn-sales {
    /* definição no site.css */
}
```

### **CONCLUSÃO PURCHASING DOMAIN:**
- **4 CLASSES ESPECÍFICAS PARA MIGRAR** do site.css
- Demais classes são Bootstrap padrão

---

## Resultado Final

1. **Production Domain**: Não necessita arquivo específico CSS (só Bootstrap/AdminLTE)
2. **Purchasing Domain**: PurchasingDomain.css precisa ser criado com as 4 classes do layout venda-*