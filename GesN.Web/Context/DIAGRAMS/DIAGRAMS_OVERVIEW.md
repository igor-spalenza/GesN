# 📊 DIAGRAMAS ESPECÍFICOS POR DOMÍNIO - SISTEMA GesN

Este diretório contém diagramas detalhados de cada domínio do sistema GesN, organizados por tipo e complexidade.

## 🗂️ ESTRUTURA DE ARQUIVOS

```
DIAGRAMS/
├── DIAGRAMS_OVERVIEW.md (este arquivo)
├── ERD/
│   ├── 01-product-domain-erd.md
│   ├── 02-sales-domain-erd.md
│   ├── 03-production-domain-erd.md
│   ├── 04-purchasing-domain-erd.md
│   └── 05-financial-domain-erd.md
├── PROCESS-FLOWS/
│   ├── 01-product-creation-flow.md
│   ├── 02-sales-order-flow.md
│   ├── 03-production-demand-flow.md
│   ├── 04-purchasing-flow.md
│   └── 05-financial-flow.md
├── STATE-DIAGRAMS/
│   ├── order-lifecycle.md
│   ├── demand-lifecycle.md
│   ├── purchase-order-lifecycle.md
│   └── account-lifecycle.md
├── CLASS-DIAGRAMS/
│   └── product-tph-inheritance.md
└── LEGENDS/
    └── colors-and-conventions.md
```

## 🎨 CONVENÇÕES VISUAIS

### **Cores por Domínio:**
- **Produto**: #00a86b (verde)
- **Vendas**: #f36b21 (laranja)
- **Produção**: #fba81d (dourado)
- **Compras**: #0562aa (azul)
- **Financeiro**: #083e61 (azul escuro)

### **Tipos de Relacionamentos:**
- **1:1**: Linha simples com extremidades |—|
- **1:N**: Linha simples com extremidades |—<
- **N:N**: Linha simples com extremidades >—<
- **Opcional**: Linha tracejada
- **Herança**: Linha com triângulo ▲

### **Níveis de Detalhamento:**
- **ERD**: Entidades completas com propriedades e tipos
- **Fluxos**: Processos step-by-step com decisões
- **Estados**: Transições de status com condições
- **Classes**: Estrutura de herança e interfaces

---

**Criado em**: 16/06/2025  
**Versão**: 1.0  
**Responsável**: Igor Spalenza Chaves
