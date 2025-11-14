# ðŸ›’ DIAGRAMA DE ESTADOS - CICLO DE VIDA DA ORDEM DE COMPRA

## ðŸŽ¯ VisÃ£o Geral
Diagrama de estados completo mostrando o ciclo de vida de uma PurchaseOrder (Ordem de Compra), desde sua criaÃ§Ã£o manual (com ou sem IA) atÃ© o recebimento total e geraÃ§Ã£o automÃ¡tica de contas a pagar no domÃ­nio financeiro.

## ðŸ”„ Diagrama Principal de Estados

```mermaid
stateDiagram-v2
    [*] --> Draft : ðŸ†• CriaÃ§Ã£o inicial<br/>Manual ou IA processing
    
    %% === ESTADOS PRINCIPAIS ===
    Draft --> Sent : ðŸ“¤ Enviada ao fornecedor<br/>Manual: UsuÃ¡rio confirma envio
    Sent --> PartiallyReceived : ðŸ“¦ Recebimento parcial<br/>Manual: ConferÃªncia de itens
    PartiallyReceived --> FullyReceived : ðŸ“¦ Recebimento total<br/>Auto: Todos itens recebidos
    Sent --> FullyReceived : ðŸ“¦ Recebimento total direto<br/>Manual: Todos itens de uma vez
    FullyReceived --> [*] : ðŸŽ‰ Processo completamente finalizado
    
    %% === CANCELAMENTOS ===
    Draft --> Cancelled : âŒ Cancelar antes envio<br/>Manual: UsuÃ¡rio cancela
    Sent --> Cancelled : âŒ Cancelar apÃ³s envio<br/>Manual: Acordo com fornecedor
    PartiallyReceived --> Cancelled : âŒ Cancelar restante<br/>Manual: Problemas qualidade
    
    %% === ESTADO FINAL DE CANCELAMENTO ===
    Cancelled --> [*] : ðŸš« Processo cancelado
    
    %% === STYLING POR FASE ===
    
    %% CRIAÃ‡ÃƒO
    classDef draft fill:#fef3c7,stroke:#f59e0b,stroke-width:3px,color:black
    class Draft draft
    
    %% ENVIADA
    classDef sent fill:#d1fae5,stroke:#10b981,stroke-width:3px,color:black
    class Sent sent
    
    %% RECEBIMENTO
    classDef receiving fill:#dbeafe,stroke:#3b82f6,stroke-width:3px,color:black
    class PartiallyReceived,FullyReceived receiving
    
    %% CANCELAMENTO
    classDef cancelled fill:#fecaca,stroke:#ef4444,stroke-width:3px,color:black
    class Cancelled cancelled
```

## ðŸ“‹ Detalhamento dos Estados

### **ðŸŸ¡ DRAFT (Rascunho)**
```
ðŸ“Œ Estado Inicial
â”œâ”€â”€ Origem: CriaÃ§Ã£o manual ou processamento IA
â”œâ”€â”€ DescriÃ§Ã£o: Ordem criada mas nÃ£o enviada ao fornecedor
â”œâ”€â”€ Permitido: EdiÃ§Ã£o livre de dados e itens
â”œâ”€â”€ Bloqueado: NÃ£o gera compromissos financeiros
â””â”€â”€ PrÃ³ximo Estado: Sent ou Cancelled

MÃ©todos de CriaÃ§Ã£o:
â”œâ”€â”€ ðŸ“ Manual tradicional: FormulÃ¡rio em branco
â”œâ”€â”€ ðŸ¤– Com IA: Upload nota fiscal + processamento
â”œâ”€â”€ ðŸ’¡ SugestÃ£o automÃ¡tica: Estoque mÃ­nimo
â””â”€â”€ ðŸ“‹ Recorrente: Baseada em histÃ³rico
```

**CriaÃ§Ã£o com IA (Fluxo Detalhado):**
```mermaid
flowchart TD
    A[ðŸ“„ Upload nota fiscal] --> B[ðŸ’¾ Salvar FISCAL_DOCUMENT<br/>Status: Processing]
    B --> C[ðŸ¤– IA processa documento]
    C --> D{âœ… Processamento<br/>bem-sucedido?}
    
    D -->|Erro| E[âŒ Status: Error<br/>UsuÃ¡rio vÃª erro]
    D -->|Sucesso| F[âœ… Status: Processed<br/>AIExtractedData preenchida]
    
    E --> G[ðŸ”„ Tentar novamente<br/>ou modo manual]
    F --> H[ðŸ” Identificar fornecedor<br/>por CNPJ]
    H --> I[ðŸ¥˜ Mapear ingredientes<br/>por nome/cÃ³digo]
    I --> J[ðŸ“‹ Gerar formulÃ¡rio<br/>prÃ©-preenchido]
    J --> K[ðŸ‘¤ UsuÃ¡rio confere<br/>e ajusta dados]
    K --> L[ðŸ’¾ Criar PurchaseOrder<br/>Status: Draft]
    
    G --> L
    
    classDef aiStyle fill:#8b5cf6,stroke:#7c3aed,stroke-width:2px,color:white
    class A,B,C,D,F,H,I,J aiStyle
    
    classDef errorStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class E,G errorStyle
    
    classDef userStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class K,L userStyle
```

**ValidaÃ§Ãµes no Estado Draft:**
- âœ… Supplier deve estar selecionado e ativo
- âœ… Pelo menos 1 PurchaseOrderItem ativo
- âœ… Todos ingredientes devem estar mapeados e ativos
- âœ… Quantidades > 0 para todos itens
- âœ… PreÃ§os unitÃ¡rios > 0
- âœ… Total calculado corretamente

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Adicionar/remover PurchaseOrderItem
- âœ… Editar quantidades e preÃ§os
- âœ… Alterar Supplier
- âœ… Modificar datas e condiÃ§Ãµes
- âœ… Anexar/processar documentos fiscais
- âœ… Cancelar ordem
- âœ… Enviar ao fornecedor

### **ðŸŸ¢ SENT (Enviada)**
```
ðŸ“Œ Estado de Ordem Ativa
â”œâ”€â”€ Trigger: UsuÃ¡rio confirma envio ao fornecedor
â”œâ”€â”€ DescriÃ§Ã£o: Ordem enviada, aguardando entrega
â”œâ”€â”€ Permitido: Receber itens parcial ou totalmente
â”œâ”€â”€ Bloqueado: Editar itens e dados crÃ­ticos
â””â”€â”€ PrÃ³ximo Estado: PartiallyReceived, FullyReceived ou Cancelled

AÃ§Ãµes no Envio:
â”œâ”€â”€ ðŸ“§ Notificar fornecedor (email/sistema)
â”œâ”€â”€ ðŸ“… Registrar data de envio
â”œâ”€â”€ ðŸ”’ Bloquear ediÃ§Ãµes crÃ­ticas
â””â”€â”€ â° Iniciar tracking de prazo de entrega
```

**Processo de Envio:**
```mermaid
flowchart TD
    A[ðŸ‘¤ UsuÃ¡rio clica "Enviar"] --> B[âœ… Validar dados obrigatÃ³rios]
    B --> C{ðŸ” ValidaÃ§Ã£o OK?}
    
    C -->|NÃ£o| D[âš ï¸ Exibir erros<br/>Corrigir antes envio]
    C -->|Sim| E[ðŸ“§ Enviar para fornecedor]
    
    E --> F[ðŸ“… OrderDate = hoje]
    F --> G[ðŸ“ˆ Status: Draft â†’ Sent]
    G --> H[â° Calcular ExpectedDeliveryDate<br/>baseado em LeadTime]
    H --> I[ðŸ”” Configurar alertas<br/>de acompanhamento]
    
    D --> A
    
    classDef validationStyle fill:#fef3c7,stroke:#f59e0b,stroke-width:2px,color:black
    class A,B,C,D validationStyle
    
    classDef sendingStyle fill:#d1fae5,stroke:#10b981,stroke-width:2px,color:black
    class E,F,G,H,I sendingStyle
```

**Tracking de Prazo:**
```
ExpectedDeliveryDate = OrderDate + Supplier.DeliveryDays

Alertas automÃ¡ticos:
â”œâ”€â”€ ðŸ“§ Lembrete 2 dias antes do prazo
â”œâ”€â”€ âš ï¸ Alerta no dia do vencimento
â”œâ”€â”€ ðŸš¨ Alerta crÃ­tico se atrasar
â””â”€â”€ ðŸ“Š Atualizar rating do fornecedor
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Receber itens (parcial ou total)
- âœ… Cancelar ordem (com acordo fornecedor)
- âœ… Acompanhar status de entrega
- âœ… Comunicar com fornecedor
- â›” Editar itens ou quantidades
- â›” Alterar fornecedor

### **ðŸ”µ PARTIALLY_RECEIVED (Parcialmente Recebida)**
```
ðŸ“Œ Estado de Recebimento Parcial
â”œâ”€â”€ Trigger: Recebimento de alguns itens
â”œâ”€â”€ DescriÃ§Ã£o: Parte da ordem foi entregue
â”œâ”€â”€ Permitido: Continuar recebendo itens restantes
â”œâ”€â”€ Bloqueado: Alterar itens jÃ¡ recebidos
â””â”€â”€ PrÃ³ximo Estado: FullyReceived ou Cancelled

Controle por Item:
â”œâ”€â”€ ðŸ” PurchaseOrderItem.ItemStatus individual
â”œâ”€â”€ ðŸ“Š QuantityReceived vs QuantityOrdered
â”œâ”€â”€ ðŸ“… ActualDeliveryDate por item
â””â”€â”€ ðŸ§ª QualityNotes por item
```

**Processo de Recebimento Parcial:**
```mermaid
flowchart TD
    A[ðŸ“¦ Mercadoria chega] --> B[ðŸ” Localizar PurchaseOrder<br/>pelo nÃºmero]
    B --> C[ðŸ“‹ Listar itens<br/>da ordem]
    C --> D[ðŸ“¦ Para cada item recebido]
    
    D --> E[âš–ï¸ Conferir quantidade<br/>fÃ­sica vs pedida]
    E --> F[ðŸ§ª Verificar qualidade]
    F --> G[ðŸ“… Verificar validade]
    G --> H{âœ… Item estÃ¡<br/>conforme?}
    
    H -->|NÃ£o| I[âŒ Rejeitar item<br/>QualityNotes + motivo]
    H -->|Sim| J[âœ… Aceitar item<br/>QuantityReceived += Qty]
    
    I --> K[ðŸ“ˆ ItemStatus: Rejected]
    J --> L[ðŸ“ˆ ItemStatus: PartiallyReceived<br/>ou FullyReceived]
    
    K --> M{ðŸ”„ Mais itens<br/>para conferir?}
    L --> M
    
    M -->|Sim| D
    M -->|NÃ£o| N[ðŸ“Š Atualizar status geral<br/>da PurchaseOrder]
    
    N --> O{ðŸ“‹ Algum item<br/>ainda pendente?}
    O -->|Sim| P[ðŸ“ˆ Status: PartiallyReceived]
    O -->|NÃ£o| Q[ðŸ“ˆ Status: FullyReceived]
    
    classDef receivingStyle fill:#dbeafe,stroke:#3b82f6,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,J,L,M,N,O,P,Q receivingStyle
    
    classDef qualityStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class H,I,K qualityStyle
```

**Estados dos PurchaseOrderItem:**
```
ItemStatus possÃ­veis:
â”œâ”€â”€ ðŸ“‹ Pending: Aguardando entrega
â”œâ”€â”€ ðŸ“¦ PartiallyReceived: Parte recebida
â”œâ”€â”€ âœ… FullyReceived: Totalmente recebido
â”œâ”€â”€ âŒ Rejected: Rejeitado por qualidade
â””â”€â”€ â° Overdue: Atrasado (automÃ¡tico)
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Continuar recebendo itens pendentes
- âœ… Rejeitar itens por qualidade
- âœ… Cancelar itens restantes
- âœ… Comunicar problemas ao fornecedor
- â›” Alterar itens jÃ¡ recebidos totalmente

### **ðŸŸ¦ FULLY_RECEIVED (Totalmente Recebida)**
```
ðŸ“Œ Estado Final de Sucesso
â”œâ”€â”€ Trigger: Todos itens recebidos e aceitos
â”œâ”€â”€ DescriÃ§Ã£o: Ordem completamente entregue
â”œâ”€â”€ Permitido: AnÃ¡lise e arquivo
â”œâ”€â”€ Bloqueado: Qualquer alteraÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

IntegraÃ§Ãµes AutomÃ¡ticas:
â”œâ”€â”€ ðŸ­ Atualizar INGREDIENT_STOCK automaticamente
â”œâ”€â”€ ðŸ’° Gerar ACCOUNT_PAYABLE no Financeiro
â”œâ”€â”€ ðŸ“Š Avaliar performance do fornecedor
â””â”€â”€ ðŸ“ˆ Atualizar mÃ©tricas de compras
```

**AtualizaÃ§Ãµes AutomÃ¡ticas no Recebimento Total:**

#### **AtualizaÃ§Ã£o de Estoque:**
```sql
-- Para cada PurchaseOrderItem totalmente recebido:
UPDATE INGREDIENT_STOCK 
SET CurrentQuantity = CurrentQuantity + @QuantityReceived,
    AvailableQuantity = AvailableQuantity + @QuantityReceived,
    AverageCost = (AverageCost * CurrentQuantity + @UnitCost * @QuantityReceived) 
                  / (CurrentQuantity + @QuantityReceived),
    LastPurchaseDate = @ActualDeliveryDate,
    LastPurchaseCost = @UnitCost,
    LastUpdated = GETDATE()
WHERE IngredientId = @IngredientId
```

#### **GeraÃ§Ã£o de Conta a Pagar:**
```mermaid
flowchart TD
    A[PurchaseOrder: FullyReceived] --> B[ðŸ’° Criar AccountPayable]
    B --> C[ðŸ“Š Calcular DueDate<br/>baseado em PaymentTerms]
    C --> D[ðŸ’µ TotalAmount = PO.TotalValue]
    D --> E[ðŸ“ˆ AccountStatus: Pending]
    E --> F[ðŸ”” Notificar financeiro<br/>sobre nova obrigaÃ§Ã£o]
    
    classDef financialStyle fill:#083e61,stroke:#083e61,stroke-width:2px,color:white
    class A,B,C,D,E,F financialStyle
```

**CÃ¡lculo de Performance do Fornecedor:**
```
MÃ©tricas atualizadas:
â”œâ”€â”€ â° Pontualidade: ActualDeliveryDate vs ExpectedDeliveryDate
â”œâ”€â”€ ðŸ§ª Qualidade: % itens aceitos vs rejeitados
â”œâ”€â”€ ðŸ’° PrecisÃ£o preÃ§o: VariaÃ§Ã£o vs cotaÃ§Ã£o inicial
â”œâ”€â”€ ðŸ“Š Rating geral: MÃ©dia ponderada das mÃ©tricas
â””â”€â”€ ðŸ“ˆ HistÃ³rico de entregas
```

**AÃ§Ãµes DisponÃ­veis:**
- âœ… Consultar dados histÃ³ricos
- âœ… Analisar performance fornecedor
- âœ… Gerar relatÃ³rios de compra
- âœ… Avaliar qualidade recebida
- â›” Qualquer alteraÃ§Ã£o

### **âŒ CANCELLED (Cancelada)**
```
ðŸ“Œ Estado Final de Cancelamento
â”œâ”€â”€ Trigger: Cancelamento manual em qualquer fase
â”œâ”€â”€ DescriÃ§Ã£o: Ordem cancelada por acordo ou problema
â”œâ”€â”€ Permitido: Consulta e auditoria
â”œâ”€â”€ Bloqueado: ReativaÃ§Ã£o
â””â”€â”€ PrÃ³ximo Estado: [Finalizado]

Motivos de Cancelamento:
â”œâ”€â”€ ðŸš« UsuÃ¡rio cancela antes do envio (Draft)
â”œâ”€â”€ ðŸ¤ Acordo com fornecedor (Sent)
â”œâ”€â”€ ðŸ§ª Problemas de qualidade (PartiallyReceived)
â””â”€â”€ â° Atraso excessivo do fornecedor
```

**Processo de Cancelamento:**
```mermaid
flowchart TD
    A[ðŸ‘¤ UsuÃ¡rio solicita<br/>cancelamento] --> B{ðŸ“‹ Estado atual<br/>da ordem?}
    
    B -->|Draft| C[âœ… Cancelamento simples<br/>Sem impactos]
    
    B -->|Sent| D[ðŸ“§ Negociar com fornecedor<br/>PossÃ­vel multa]
    
    B -->|PartiallyReceived| E[ðŸ“¦ Cancelar apenas<br/>itens nÃ£o recebidos]
    
    C --> F[ðŸ“ˆ Status: Cancelled]
    D --> G{ðŸ¤ Fornecedor<br/>aceita cancelamento?}
    E --> H[ðŸ“Š Manter itens recebidos<br/>Cancelar restante]
    
    G -->|Sim| F
    G -->|NÃ£o| I[âš ï¸ Negociar multa<br/>ou manter ordem]
    
    H --> F
    I --> J[ðŸ’° Registrar multa<br/>se aplicÃ¡vel]
    J --> F
    
    F --> K[ðŸ“ Registrar motivo<br/>do cancelamento]
    K --> L[ðŸš« Ordem cancelada]
    
    classDef cancelStyle fill:#fecaca,stroke:#ef4444,stroke-width:2px,color:black
    class A,B,C,D,E,F,G,H,I,J,K,L cancelStyle
```

**Impactos do Cancelamento:**
```
Por Estado de Origem:

Draft â†’ Cancelled:
â”œâ”€â”€ âš¡ AÃ§Ã£o: ExclusÃ£o simples
â”œâ”€â”€ ðŸ”„ ReversÃ£o: Nenhuma necessÃ¡ria
â””â”€â”€ ðŸ“Š Impacto: Apenas PurchaseOrder

Sent â†’ Cancelled:
â”œâ”€â”€ âš¡ AÃ§Ã£o: Cancelamento negociado
â”œâ”€â”€ ðŸ”„ ReversÃ£o: PossÃ­vel multa contratual
â””â”€â”€ ðŸ“Š Impacto: Compras + relacionamento fornecedor

PartiallyReceived â†’ Cancelled:
â”œâ”€â”€ âš¡ AÃ§Ã£o: Cancelamento parcial
â”œâ”€â”€ ðŸ”„ ReversÃ£o: Manter itens recebidos
â””â”€â”€ ðŸ“Š Impacto: Compras + Estoque + Financeiro
```

## âš¡ TransiÃ§Ãµes AutomÃ¡ticas vs Manuais

### **ðŸ¤– TransiÃ§Ãµes AutomÃ¡ticas:**
```
PartiallyReceived â†’ FullyReceived:
â”œâ”€â”€ Trigger: Ãšltimo item marcado como FullyReceived
â”œâ”€â”€ CondiÃ§Ã£o: Todos PurchaseOrderItem recebidos
â””â”€â”€ Tempo: Imediato apÃ³s Ãºltima conferÃªncia

FullyReceived â†’ [Finalizado]:
â”œâ”€â”€ Trigger: AccountPayable gerada com sucesso
â”œâ”€â”€ CondiÃ§Ã£o: IntegraÃ§Ãµes completadas
â””â”€â”€ Tempo: Processamento em background

Sent â†’ Overdue (alerta):
â”œâ”€â”€ Trigger: ExpectedDeliveryDate vencida
â”œâ”€â”€ CondiÃ§Ã£o: Ainda nÃ£o recebido
â””â”€â”€ Tempo: Job automÃ¡tico diÃ¡rio
```

### **ðŸ‘¤ TransiÃ§Ãµes Manuais:**
```
Draft â†’ Sent:
â”œâ”€â”€ Trigger: UsuÃ¡rio clica "Enviar ao Fornecedor"
â”œâ”€â”€ Interface: BotÃ£o de envio
â””â”€â”€ ValidaÃ§Ã£o: Dados obrigatÃ³rios preenchidos

Sent â†’ PartiallyReceived:
â”œâ”€â”€ Trigger: UsuÃ¡rio registra recebimento
â”œâ”€â”€ Interface: Tela de conferÃªncia
â””â”€â”€ ValidaÃ§Ã£o: Pelo menos 1 item recebido

PartiallyReceived â†’ Cancelled:
â”œâ”€â”€ Trigger: UsuÃ¡rio cancela itens restantes
â”œâ”€â”€ Interface: AÃ§Ã£o de cancelamento
â””â”€â”€ ValidaÃ§Ã£o: Justificativa obrigatÃ³ria
```

## ðŸš¨ ValidaÃ§Ãµes e Alertas

### **ValidaÃ§Ãµes por Estado:**
```
Estado              â”‚ Editar Item â”‚ Alterar Fornec â”‚ Cancelar â”‚ Receber
â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
Draft               â”‚     âœ…      â”‚       âœ…       â”‚    âœ…    â”‚   â›”
Sent                â”‚     â›”      â”‚       â›”       â”‚    âœ…    â”‚   âœ…
PartiallyReceived   â”‚     âš ï¸      â”‚       â›”       â”‚    âš ï¸    â”‚   âœ…
FullyReceived       â”‚     â›”      â”‚       â›”       â”‚    â›”    â”‚   â›”

Legenda: âœ… Permitido | â›” Bloqueado | âš ï¸ Apenas itens nÃ£o recebidos
```

### **Alertas AutomÃ¡ticos:**
```
ðŸš¨ Alertas CrÃ­ticos:
â”œâ”€â”€ Ordem atrasada > 5 dias
â”œâ”€â”€ Fornecedor com > 3 atrasos consecutivos
â”œâ”€â”€ Item rejeitado por qualidade
â”œâ”€â”€ PreÃ§o recebido > 20% da cotaÃ§Ã£o

âš ï¸ Alertas de AtenÃ§Ã£o:
â”œâ”€â”€ Prazo de entrega em 2 dias
â”œâ”€â”€ Primeiro pedido com fornecedor novo
â”œâ”€â”€ Quantidade recebida â‰  quantidade pedida
â”œâ”€â”€ Ingrediente prÃ³ximo do vencimento
```

### **ValidaÃ§Ãµes de NegÃ³cio:**
```
PurchaseOrderItem:
â”œâ”€â”€ âœ… QuantityReceived â‰¤ QuantityOrdered
â”œâ”€â”€ âœ… ActualDeliveryDate â‰¥ OrderDate
â”œâ”€â”€ âœ… UnitCost > 0
â”œâ”€â”€ âœ… ExpirationDate > hoje (se aplicÃ¡vel)

IngredientStock:
â”œâ”€â”€ âœ… Capacidade de armazenagem suficiente
â”œâ”€â”€ âœ… Ingredient ativo no sistema
â”œâ”€â”€ âœ… UnitOfMeasure consistente
â”œâ”€â”€ âœ… CurrentQuantity apÃ³s recebimento > 0
```

## ðŸŽ¯ Eventos de DomÃ­nio por TransiÃ§Ã£o

```
PurchaseOrderStatusChanged:
â”œâ”€â”€ PurchaseOrderId: ID da ordem
â”œâ”€â”€ From: Estado anterior
â”œâ”€â”€ To: Novo estado
â”œâ”€â”€ Timestamp: Data/hora da mudanÃ§a
â”œâ”€â”€ UserId: UsuÃ¡rio responsÃ¡vel
â”œâ”€â”€ Reason: Motivo da mudanÃ§a (se cancelamento)
â””â”€â”€ AdditionalData: Dados especÃ­ficos

Eventos EspecÃ­ficos:
â”œâ”€â”€ PurchaseOrderCreated: Nova ordem criada
â”œâ”€â”€ PurchaseOrderSent: Ordem enviada ao fornecedor
â”œâ”€â”€ ItemReceived: Item especÃ­fico recebido
â”œâ”€â”€ PurchaseOrderCompleted: Ordem totalmente recebida
â”œâ”€â”€ StockUpdated: Estoque atualizado automaticamente
â”œâ”€â”€ AccountPayableCreated: Conta a pagar gerada
â”œâ”€â”€ SupplierEvaluated: Performance do fornecedor avaliada
â””â”€â”€ PurchaseOrderCancelled: Ordem cancelada
```

## ðŸ“Š MÃ©tricas e KPIs por Estado

### **Indicadores de Performance:**
```
Por PurchaseOrder:
â”œâ”€â”€ â° Lead Time: OrderDate â†’ ActualDeliveryDate
â”œâ”€â”€ ðŸ’° Cost Variance: Custo real vs estimado
â”œâ”€â”€ ðŸ§ª Quality Rate: % itens aceitos vs total
â”œâ”€â”€ ðŸ“Š Fulfillment Rate: % quantidade recebida vs pedida

Por Supplier:
â”œâ”€â”€ ðŸ“ˆ On-Time Delivery: % entregas no prazo
â”œâ”€â”€ ðŸ† Quality Score: MÃ©dia de qualidade
â”œâ”€â”€ ðŸ’µ Price Stability: VariaÃ§Ã£o de preÃ§os
â”œâ”€â”€ ðŸ¤ Reliability Index: Ãndice geral de confiabilidade
```

---

**Arquivo**: `purchase-order-lifecycle.md`  
**DomÃ­nio**: Compras (#0562aa)  
**Tipo**: State Diagram  
**Foco**: Ciclo Completo PurchaseOrder + IA Integration + Stock Management
