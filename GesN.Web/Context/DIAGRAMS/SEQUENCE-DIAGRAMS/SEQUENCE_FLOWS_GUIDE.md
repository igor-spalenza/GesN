# ðŸ”„ SEQUENCE DIAGRAMS - SISTEMA GesN

## ðŸŽ¯ VisÃ£o Geral
Diagramas de sequÃªncia detalhados mostrando as interaÃ§Ãµes entre objetos ao longo do tempo para os fluxos mais crÃ­ticos do sistema GesN. Cada diagrama inclui validaÃ§Ãµes internas e integraÃ§Ãµes entre domÃ­nios.

## ðŸ“‹ Fluxos CrÃ­ticos Mapeados

### **ðŸ”„ Fluxos AutomÃ¡ticos**

| Fluxo | Arquivo | Trigger | Complexidade |
|-------|---------|---------|--------------|
| **OrderItem â†’ Demand** | [`01-orderitem-to-demand-flow.md`](./01-orderitem-to-demand-flow.md) | OrderItem criado/editado | ðŸš¨ Alta |
| **PurchaseOrder â†’ AccountPayable** | [`02-purchase-to-payable-flow.md`](./02-purchase-to-payable-flow.md) | PurchaseOrder recebida | âš ï¸ MÃ©dia |
| **OrderEntry â†’ AccountReceivable** | [`03-order-to-receivable-flow.md`](./03-order-to-receivable-flow.md) | OrderEntry confirmada | âš ï¸ MÃ©dia |

### **ðŸ¤– Fluxos com IA/Complexidade**

| Fluxo | Arquivo | Trigger | Complexidade |
|-------|---------|---------|--------------|
| **IA Processing** | [`04-ai-processing-flow.md`](./04-ai-processing-flow.md) | Upload nota fiscal | ðŸš¨ Alta |
| **Product Configuration** | [`05-product-configuration-flow.md`](./05-product-configuration-flow.md) | SeleÃ§Ã£o produto composto | ðŸš¨ Alta |

## ðŸŽ¨ ConvenÃ§Ãµes dos Sequence Diagrams

### **ðŸŽ¯ Elementos Visuais**

| Elemento | RepresentaÃ§Ã£o | Cor | DescriÃ§Ã£o |
|----------|---------------|-----|-----------|
| **Actor** | `ðŸ‘¤ User` | `#3b82f6` | UsuÃ¡rio humano |
| **Controller** | `ðŸŽ® Controller` | `#8b5cf6` | Controlador web |
| **Service** | `âš™ï¸ Service` | Cor do domÃ­nio | LÃ³gica de negÃ³cio |
| **Repository** | `ðŸ—„ï¸ Repository` | `#6b7280` | Acesso a dados |
| **External API** | `ðŸŒ GoogleAPI` | `#ef4444` | Sistema externo |
| **Database** | `ðŸ’¾ Database` | `#374151` | PersistÃªncia |

### **ðŸ”— Tipos de InteraÃ§Ã£o**

| SÃ­mbolo | Tipo | DescriÃ§Ã£o |
|---------|------|-----------|
| `->` | **Chamada SÃ­ncrona** | Aguarda resposta |
| `->>` | **Chamada AssÃ­ncrona** | NÃ£o aguarda resposta |
| `-->>` | **Resposta** | Retorno de dados |
| `-x` | **Chamada que Falha** | Erro ou exceÃ§Ã£o |
| `Note over` | **Nota/ObservaÃ§Ã£o** | InformaÃ§Ã£o adicional |

### **âš¡ Tipos de AtivaÃ§Ã£o**

| PadrÃ£o | Significado |
|--------|-------------|
| `activate/deactivate` | PerÃ­odo de processamento |
| `par/and` | Processamento paralelo |
| `alt/else` | CondiÃ§Ãµes alternativas |
| `opt` | Processamento opcional |
| `loop` | IteraÃ§Ã£o |

## ðŸ“Š MÃ©tricas de Complexidade

### **ðŸŽ¯ CritÃ©rios de AvaliaÃ§Ã£o**

| NÃ­vel | Participantes | InteraÃ§Ãµes | Cross-Domain | ValidaÃ§Ãµes |
|-------|---------------|------------|--------------|------------|
| **ðŸŸ¢ Baixa** | 2-3 | < 10 | 0-1 | BÃ¡sicas |
| **âš ï¸ MÃ©dia** | 4-6 | 10-20 | 1-2 | MÃºltiplas |
| **ðŸš¨ Alta** | 7+ | 20+ | 2+ | Complexas |

### **ðŸ“‹ Fluxos por Complexidade**

#### **ðŸš¨ Alta Complexidade**
1. **OrderItem â†’ Demand**: Multiple product types, complex rules
2. **IA Processing**: OCR, mapping, validation, user review
3. **Product Configuration**: Hierarchies, components, pricing

#### **âš ï¸ MÃ©dia Complexidade**
1. **PurchaseOrder â†’ AccountPayable**: Status validation, financial creation
2. **OrderEntry â†’ AccountReceivable**: Payment terms, installments

## ðŸ”„ PadrÃµes de IntegraÃ§Ã£o

### **ðŸŒ Cross-Domain Patterns**

#### **1. ðŸ“‹ Command-Event Pattern**
```
Domain A â†’ Command â†’ Domain B â†’ Event â†’ Domain C
```

#### **2. ðŸ”„ Synchronous Integration**
```
Service A â†’ API Call â†’ Service B â†’ Response â†’ Service A
```

#### **3. âš¡ Asynchronous Integration**
```
Service A â†’ Event Bus â†’ Service B (eventual consistency)
```

#### **4. ðŸ¤– External API Pattern**
```
System â†’ External API â†’ Response â†’ Process â†’ Store
```

## ðŸš¨ Hotspots e ValidaÃ§Ãµes

### **âš ï¸ Pontos CrÃ­ticos Identificados**

| Hotspot | Fluxo | DescriÃ§Ã£o | MitigaÃ§Ã£o |
|---------|-------|-----------|-----------|
| **Product Type Detection** | OrderItemâ†’Demand | Different logic per type | Strategy pattern |
| **IA Accuracy** | IA Processing | OCR errors, mapping fails | Human validation |
| **Component Validation** | Product Config | Complex business rules | Rule engine |
| **Concurrency** | Multiple flows | Race conditions | Pessimistic locking |
| **External Dependencies** | All flows | Google APIs down | Circuit breaker |

### **âœ… ValidaÃ§Ãµes Implementadas**

#### **1. ðŸ“Š Business Validations**
- Product active status
- Stock availability
- Business rules compliance
- Data consistency

#### **2. ðŸ”’ Technical Validations**
- Input sanitization
- Data format validation
- Authorization checks
- Rate limiting

#### **3. ðŸŒ Integration Validations**
- External API availability
- Response format validation
- Timeout handling
- Retry mechanisms

## ðŸ“ˆ Performance Considerations

### **âš¡ OtimizaÃ§Ãµes Identificadas**

| Ãrea | Problema | SoluÃ§Ã£o |
|------|----------|---------|
| **Database Queries** | N+1 problems | Eager loading |
| **External APIs** | Latency | Caching + async |
| **Complex Rules** | Performance | Rule caching |
| **File Processing** | Large files | Streaming |

### **ðŸ“Š SLA Targets**

| Fluxo | Target Response Time | Availability |
|-------|---------------------|--------------|
| **OrderItemâ†’Demand** | < 2s | 99.9% |
| **Product Config** | < 1s | 99.9% |
| **IA Processing** | < 30s | 99.5% |
| **Financial Creation** | < 5s | 99.9% |

---

**Criado em**: 16/06/2025  
**VersÃ£o**: 1.0  
**PadrÃ£o**: UML Sequence Diagrams  
**Ferramenta**: Mermaid + Markdown  
**Escopo**: 5 fluxos crÃ­ticos + validaÃ§Ãµes completas
