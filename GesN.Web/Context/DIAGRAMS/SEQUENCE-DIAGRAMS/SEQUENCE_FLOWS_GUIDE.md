# 🔄 SEQUENCE DIAGRAMS - SISTEMA GesN

## 🎯 Visão Geral
Diagramas de sequência detalhados mostrando as interações entre objetos ao longo do tempo para os fluxos mais críticos do sistema GesN. Cada diagrama inclui validações internas e integrações entre domínios.

## 📋 Fluxos Críticos Mapeados

### **🔄 Fluxos Automáticos**

| Fluxo | Arquivo | Trigger | Complexidade |
|-------|---------|---------|--------------|
| **OrderItem → Demand** | [`01-orderitem-to-demand-flow.md`](./01-orderitem-to-demand-flow.md) | OrderItem criado/editado | 🚨 Alta |
| **PurchaseOrder → AccountPayable** | [`02-purchase-to-payable-flow.md`](./02-purchase-to-payable-flow.md) | PurchaseOrder recebida | ⚠️ Média |
| **OrderEntry → AccountReceivable** | [`03-order-to-receivable-flow.md`](./03-order-to-receivable-flow.md) | OrderEntry confirmada | ⚠️ Média |

### **🤖 Fluxos com IA/Complexidade**

| Fluxo | Arquivo | Trigger | Complexidade |
|-------|---------|---------|--------------|
| **IA Processing** | [`04-ai-processing-flow.md`](./04-ai-processing-flow.md) | Upload nota fiscal | 🚨 Alta |
| **Product Configuration** | [`05-product-configuration-flow.md`](./05-product-configuration-flow.md) | Seleção produto composto | 🚨 Alta |

## 🎨 Convenções dos Sequence Diagrams

### **🎯 Elementos Visuais**

| Elemento | Representação | Cor | Descrição |
|----------|---------------|-----|-----------|
| **Actor** | `👤 User` | `#3b82f6` | Usuário humano |
| **Controller** | `🎮 Controller` | `#8b5cf6` | Controlador web |
| **Service** | `⚙️ Service` | Cor do domínio | Lógica de negócio |
| **Repository** | `🗄️ Repository` | `#6b7280` | Acesso a dados |
| **External API** | `🌐 GoogleAPI` | `#ef4444` | Sistema externo |
| **Database** | `💾 Database` | `#374151` | Persistência |

### **🔗 Tipos de Interação**

| Símbolo | Tipo | Descrição |
|---------|------|-----------|
| `->` | **Chamada Síncrona** | Aguarda resposta |
| `->>` | **Chamada Assíncrona** | Não aguarda resposta |
| `-->>` | **Resposta** | Retorno de dados |
| `-x` | **Chamada que Falha** | Erro ou exceção |
| `Note over` | **Nota/Observação** | Informação adicional |

### **⚡ Tipos de Ativação**

| Padrão | Significado |
|--------|-------------|
| `activate/deactivate` | Período de processamento |
| `par/and` | Processamento paralelo |
| `alt/else` | Condições alternativas |
| `opt` | Processamento opcional |
| `loop` | Iteração |

## 📊 Métricas de Complexidade

### **🎯 Critérios de Avaliação**

| Nível | Participantes | Interações | Cross-Domain | Validações |
|-------|---------------|------------|--------------|------------|
| **🟢 Baixa** | 2-3 | < 10 | 0-1 | Básicas |
| **⚠️ Média** | 4-6 | 10-20 | 1-2 | Múltiplas |
| **🚨 Alta** | 7+ | 20+ | 2+ | Complexas |

### **📋 Fluxos por Complexidade**

#### **🚨 Alta Complexidade**
1. **OrderItem → Demand**: Multiple product types, complex rules
2. **IA Processing**: OCR, mapping, validation, user review
3. **Product Configuration**: Hierarchies, components, pricing

#### **⚠️ Média Complexidade**
1. **PurchaseOrder → AccountPayable**: Status validation, financial creation
2. **OrderEntry → AccountReceivable**: Payment terms, installments

## 🔄 Padrões de Integração

### **🌐 Cross-Domain Patterns**

#### **1. 📋 Command-Event Pattern**
```
Domain A → Command → Domain B → Event → Domain C
```

#### **2. 🔄 Synchronous Integration**
```
Service A → API Call → Service B → Response → Service A
```

#### **3. ⚡ Asynchronous Integration**
```
Service A → Event Bus → Service B (eventual consistency)
```

#### **4. 🤖 External API Pattern**
```
System → External API → Response → Process → Store
```

## 🚨 Hotspots e Validações

### **⚠️ Pontos Críticos Identificados**

| Hotspot | Fluxo | Descrição | Mitigação |
|---------|-------|-----------|-----------|
| **Product Type Detection** | OrderItem→Demand | Different logic per type | Strategy pattern |
| **IA Accuracy** | IA Processing | OCR errors, mapping fails | Human validation |
| **Component Validation** | Product Config | Complex business rules | Rule engine |
| **Concurrency** | Multiple flows | Race conditions | Pessimistic locking |
| **External Dependencies** | All flows | Google APIs down | Circuit breaker |

### **✅ Validações Implementadas**

#### **1. 📊 Business Validations**
- Product active status
- Stock availability
- Business rules compliance
- Data consistency

#### **2. 🔒 Technical Validations**
- Input sanitization
- Data format validation
- Authorization checks
- Rate limiting

#### **3. 🌐 Integration Validations**
- External API availability
- Response format validation
- Timeout handling
- Retry mechanisms

## 📈 Performance Considerations

### **⚡ Otimizações Identificadas**

| Área | Problema | Solução |
|------|----------|---------|
| **Database Queries** | N+1 problems | Eager loading |
| **External APIs** | Latency | Caching + async |
| **Complex Rules** | Performance | Rule caching |
| **File Processing** | Large files | Streaming |

### **📊 SLA Targets**

| Fluxo | Target Response Time | Availability |
|-------|---------------------|--------------|
| **OrderItem→Demand** | < 2s | 99.9% |
| **Product Config** | < 1s | 99.9% |
| **IA Processing** | < 30s | 99.5% |
| **Financial Creation** | < 5s | 99.9% |

---

**Criado em**: 16/06/2025  
**Versão**: 1.0  
**Padrão**: UML Sequence Diagrams  
**Ferramenta**: Mermaid + Markdown  
**Escopo**: 5 fluxos críticos + validações completas
