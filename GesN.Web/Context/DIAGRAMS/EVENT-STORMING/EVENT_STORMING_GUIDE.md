# âš¡ EVENT STORMING - SISTEMA GesN

## ðŸŽ¯ VisÃ£o Geral
Event Storming Ã© uma tÃ©cnica colaborativa para mapear o domÃ­nio de negÃ³cio atravÃ©s de eventos, comandos, agregados e outros elementos do Domain-Driven Design (DDD). Este diretÃ³rio contÃ©m o mapeamento completo dos eventos do sistema GesN.

## ðŸ“‹ Estrutura por DomÃ­nio

### **ðŸ—‚ï¸ OrganizaÃ§Ã£o dos Arquivos**

| DomÃ­nio | Arquivo | Foco Principal |
|---------|---------|----------------|
| **ðŸ“¦ Produto** | [`product-domain-events.md`](./product-domain-events.md) | CriaÃ§Ã£o produtos, configuraÃ§Ãµes, validaÃ§Ãµes |
| **ðŸ’° Vendas** | [`sales-domain-events.md`](./sales-domain-events.md) | Pedidos, confirmaÃ§Ãµes, integraÃ§Ãµes |
| **ðŸ­ ProduÃ§Ã£o** | [`production-domain-events.md`](./production-domain-events.md) | Demandas, execuÃ§Ã£o, finalizaÃ§Ã£o |
| **ðŸ›’ Compras** | [`purchasing-domain-events.md`](./purchasing-domain-events.md) | Ordens compra, recebimentos, IA |
| **ðŸ’³ Financeiro** | [`financial-domain-events.md`](./financial-domain-events.md) | Contas, transaÃ§Ãµes, conciliaÃ§Ã£o |

## ðŸŽ¨ ConvenÃ§Ãµes do Event Storming

### **ðŸŽ­ Elementos e Cores**

| Elemento | Cor | Formato | DescriÃ§Ã£o |
|----------|-----|---------|-----------|
| **ðŸ“‹ Comando** | `#3b82f6` (Azul) | `[CreateOrder]` | AÃ§Ã£o que inicia processo |
| **âš¡ Evento** | `#f59e0b` (Laranja) | `OrderCreated` | Fato que aconteceu |
| **ðŸ‘¤ Ator** | `#10b981` (Verde) | `(Customer)` | Quem executa comando |
| **ðŸ“Š Agregado** | `#8b5cf6` (Roxo) | `{OrderEntry}` | Entidade que processa |
| **ðŸ“‹ Read Model** | `#6b7280` (Cinza) | `[OrderSummary]` | ProjeÃ§Ã£o para leitura |
| **ðŸ”— Sistema Externo** | `#ef4444` (Vermelho) | `<GoogleCalendar>` | DependÃªncia externa |
| **âš ï¸ Hotspot** | `#ec4899` (Rosa) | `(!ComplexRule!)` | Regra complexa/problema |

### **ðŸ”„ Fluxo Temporal**
```
Comando â†’ Evento â†’ ReaÃ§Ã£o â†’ Novo Comando â†’ Novo Evento...
```

### **ðŸ—ï¸ Estrutura de Cada DomÃ­nio**

#### **1. ðŸ“‹ Comandos (Commands)**
- AÃ§Ãµes que usuÃ¡rios/sistemas executam
- Verbos no imperativo
- Podem falhar (validaÃ§Ãµes)

#### **2. âš¡ Eventos (Domain Events)**
- Fatos que aconteceram
- Verbos no passado
- Sempre bem-sucedidos

#### **3. ðŸ“Š Agregados (Aggregates)**
- Entidades que processam comandos
- Garantem consistÃªncia
- Geram eventos

#### **4. ðŸ‘¥ Atores (Actors)**
- UsuÃ¡rios do sistema
- Sistemas externos
- Processos automÃ¡ticos

#### **5. ðŸ”„ PolÃ­ticas (Policies)**
- Regras de negÃ³cio
- "Quando X acontece, entÃ£o Y"
- Conectam eventos a comandos

## âš¡ Eventos de Alto NÃ­vel

### **ðŸŽ¯ Eventos CrÃ­ticos Cross-Domain**

| Evento | DomÃ­nio Origem | DomÃ­nios Impactados | Criticidade |
|--------|----------------|-------------------|------------|
| `OrderConfirmed` | Vendas | ProduÃ§Ã£o + Financeiro | ðŸš¨ CrÃ­tico |
| `DemandCreated` | ProduÃ§Ã£o | Compras (se ingredients low) | âš ï¸ Alto |
| `ProductionCompleted` | ProduÃ§Ã£o | Vendas + Financeiro | ðŸš¨ CrÃ­tico |
| `PurchaseReceived` | Compras | ProduÃ§Ã£o + Financeiro | âš ï¸ Alto |
| `PaymentReceived` | Financeiro | Vendas | ðŸš¨ CrÃ­tico |

### **ðŸ”— Cadeia de Eventos TÃ­pica**
```mermaid
graph LR
    A[OrderConfirmed] --> B[DemandCreated]
    B --> C[ProductionStarted]
    C --> D[IngredientConsumed]
    D --> E[ProductionCompleted]
    E --> F[OrderDelivered]
    F --> G[PaymentReceived]
    G --> H[OrderInvoiced]
```

## ðŸŽ¯ Micro-Eventos

### **ðŸ“Š Granularidade Detalhada**

| NÃ­vel | Exemplo | Quando Usar |
|-------|---------|-------------|
| **Alto NÃ­vel** | `OrderConfirmed` | IntegraÃ§Ãµes entre domÃ­nios |
| **MÃ©dio NÃ­vel** | `OrderItemAdded` | Dentro do domÃ­nio |
| **Micro NÃ­vel** | `ProductComponentCompleted` | Tracking detalhado |

### **âš™ï¸ EstratÃ©gia de ImplementaÃ§Ã£o**
- **Alto NÃ­vel**: Event Bus para integraÃ§Ãµes
- **MÃ©dio NÃ­vel**: Domain Events locais
- **Micro NÃ­vel**: Audit log + mÃ©tricas

## ðŸ“Š PadrÃµes de Eventos

### **ðŸ”„ PadrÃµes Identificados**

#### **1. Saga Pattern**
```
OrderConfirmed â†’ DemandCreated â†’ ProductionScheduled â†’ IngredientReserved
```

#### **2. Event Sourcing Candidates**
- OrderEntry lifecycle
- Demand status changes
- Account payment history

#### **3. CQRS Opportunities**
- Product catalog (read-heavy)
- Financial reports (complex queries)
- Production dashboard (real-time)

## ðŸš¨ Hotspots Identificados

### **âš ï¸ Complexidades do NegÃ³cio**

| Hotspot | DomÃ­nio | DescriÃ§Ã£o | Impacto |
|---------|---------|-----------|---------|
| **Product Configuration** | Produto/Vendas | ValidaÃ§Ã£o de componentes compostos | Alto |
| **Demand Explosion** | Vendas/ProduÃ§Ã£o | 1 OrderItem â†’ N Demands | CrÃ­tico |
| **Stock Reservation** | ProduÃ§Ã£o/Compras | ConcorrÃªncia de ingredientes | Alto |
| **Payment Reconciliation** | Financeiro | Match transaÃ§Ã£o â†” conta | MÃ©dio |

---

**Criado em**: 16/06/2025  
**VersÃ£o**: 1.0  
**TÃ©cnica**: Event Storming by Alberto Brandolini  
**Escopo**: 5 DomÃ­nios + IntegraÃ§Ãµes Google Workspace
