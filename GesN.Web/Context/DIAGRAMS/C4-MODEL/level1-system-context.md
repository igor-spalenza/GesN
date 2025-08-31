# ðŸŒ C4 LEVEL 1 - SYSTEM CONTEXT

## ðŸŽ¯ VisÃ£o Geral
Diagrama de contexto do sistema GesN mostrando como o sistema se relaciona com seus usuÃ¡rios e outros sistemas. Este Ã© o nÃ­vel mais alto de abstraÃ§Ã£o, focando no **"O QUE"** o sistema faz, nÃ£o no **"COMO"**.

## ðŸ“Š Diagrama de Contexto do Sistema

```mermaid
C4Context
    title Sistema Context - GesN (Sistema Integrado de GestÃ£o)
    
    Person(managers, "Gestores", "Administradores, supervisores e responsÃ¡veis pela operaÃ§Ã£o")
    Person(operators, "Operadores", "FuncionÃ¡rios que executam produÃ§Ã£o, vendas e operaÃ§Ãµes diÃ¡rias")
    Person(customers, "Clientes", "Pessoas fÃ­sicas e jurÃ­dicas que fazem pedidos")
    Person(suppliers, "Fornecedores", "Empresas que fornecem ingredientes e matÃ©rias-primas")
    
    System(gesn, "GesN", "Sistema SaaS integrado para gestÃ£o de negÃ³cios com 5 domÃ­nios:<br/>ðŸ“¦ Produto â€¢ ðŸ’° Vendas â€¢ ðŸ­ ProduÃ§Ã£o â€¢ ðŸ›’ Compras â€¢ ðŸ’³ Financeiro")
    
    System_Ext(google_workspace, "Google Workspace", "Plataforma Google para produtividade empresarial:<br/>â€¢ People API (contatos)<br/>â€¢ Calendar API (agendamento)<br/>â€¢ Maps Platform (logÃ­stica)")
    
    %% Relacionamentos UsuÃ¡rios â†’ GesN
    Rel(managers, gesn, "Gerenciam negÃ³cio", "Web interface")
    Rel(operators, gesn, "Operam sistema", "Web interface")  
    Rel(customers, gesn, "Fazem pedidos", "Web interface")
    Rel(suppliers, gesn, "Fornecem produtos", "Web interface")
    
    %% Relacionamentos GesN â†” Google Workspace
    Rel(gesn, google_workspace, "Sincroniza contatos", "People API")
    Rel(gesn, google_workspace, "Agenda produÃ§Ã£o", "Calendar API")
    Rel(gesn, google_workspace, "Calcula rotas", "Maps Platform")
    
    UpdateElementStyle(gesn, $bgColor="#00a86b", $fontColor="white", $borderColor="#00a86b")
    UpdateElementStyle(google_workspace, $bgColor="#ea4335", $fontColor="white", $borderColor="#ea4335")
    UpdateElementStyle(managers, $bgColor="#1a73e8", $fontColor="white")
    UpdateElementStyle(operators, $bgColor="#34a853", $fontColor="white")
    UpdateElementStyle(customers, $bgColor="#fbbc04", $fontColor="black")
    UpdateElementStyle(suppliers, $bgColor="#9aa0a6", $fontColor="white")
```

## ðŸ‘¥ Atores do Sistema

### **ðŸŽ­ UsuÃ¡rios Internos**

#### **ðŸ‘‘ Gestores**
```
Perfil: Administradores, supervisores, responsÃ¡veis
Acesso: Todas as funcionalidades + relatÃ³rios
Principais AÃ§Ãµes:
â”œâ”€â”€ ðŸ“Š Visualizar dashboards executivos
â”œâ”€â”€ ðŸŽ¯ Definir metas e polÃ­ticas
â”œâ”€â”€ ðŸ“ˆ Analisar relatÃ³rios financeiros
â”œâ”€â”€ âš™ï¸ Configurar sistema e usuÃ¡rios
â””â”€â”€ ðŸ” Auditar operaÃ§Ãµes
```

#### **âš™ï¸ Operadores**
```
Perfil: FuncionÃ¡rios operacionais
Acesso: Funcionalidades especÃ­ficas por domÃ­nio
Principais AÃ§Ãµes:
â”œâ”€â”€ ðŸ“¦ Gerenciar catÃ¡logo de produtos
â”œâ”€â”€ ðŸ’° Processar pedidos de venda
â”œâ”€â”€ ðŸ­ Executar ordens de produÃ§Ã£o
â”œâ”€â”€ ðŸ›’ Gerenciar compras e estoque
â””â”€â”€ ðŸ’³ Processar transaÃ§Ãµes financeiras
```

### **ðŸŒ UsuÃ¡rios Externos**

#### **ðŸ‘¤ Clientes**
```
Perfil: Pessoas fÃ­sicas e jurÃ­dicas
Acesso: Portal de clientes (limitado)
Principais AÃ§Ãµes:
â”œâ”€â”€ ðŸ›ï¸ Fazer pedidos online
â”œâ”€â”€ ðŸ“‹ Acompanhar status de pedidos
â”œâ”€â”€ ðŸ’³ Realizar pagamentos
â”œâ”€â”€ ðŸ“ž Acessar histÃ³rico de compras
â””â”€â”€ ðŸ“§ Receber notificaÃ§Ãµes
```

#### **ðŸ¢ Fornecedores**
```
Perfil: Empresas parceiras
Acesso: Portal de fornecedores (limitado)
Principais AÃ§Ãµes:
â”œâ”€â”€ ðŸ“„ Receber ordens de compra
â”œâ”€â”€ ðŸ“¦ Confirmar entregas
â”œâ”€â”€ ðŸ’° Acompanhar faturas
â”œâ”€â”€ ðŸ“Š Acessar relatÃ³rios de performance
â””â”€â”€ ðŸ”” Receber notificaÃ§Ãµes
```

## ðŸŒ Sistemas Externos

### **ðŸš€ Google Workspace (DependÃªncia CrÃ­tica)**

#### **ðŸ‘¥ People API**
```
PropÃ³sito: SincronizaÃ§Ã£o de contatos
IntegraÃ§Ã£o: Bidirecional
Fluxo:
â”œâ”€â”€ ðŸ“¥ GesN â†’ Google: Novos clientes/fornecedores
â”œâ”€â”€ ðŸ“¤ Google â†’ GesN: Contatos atualizados
â”œâ”€â”€ ðŸ”„ SincronizaÃ§Ã£o: AutomÃ¡tica (diÃ¡ria)
â””â”€â”€ ðŸŽ¯ BenefÃ­cio: CentralizaÃ§Ã£o de contatos
```

#### **ðŸ“… Calendar API**
```
PropÃ³sito: Agendamento de produÃ§Ã£o
IntegraÃ§Ã£o: Unidirecional (GesN â†’ Google)
Fluxo:
â”œâ”€â”€ âš¡ Trigger: OrderEntry.Status = "SentToProduction"
â”œâ”€â”€ ðŸ­ Evento: ProductionOrder criada
â”œâ”€â”€ ðŸ“… AÃ§Ã£o: Criar evento no Google Calendar
â”œâ”€â”€ â° Dados: Data produÃ§Ã£o, produtos, tempo estimado
â””â”€â”€ ðŸŽ¯ BenefÃ­cio: Visibilidade de agenda
```

#### **ðŸ—ºï¸ Google Maps Platform**
```
PropÃ³sito: LogÃ­stica e cÃ¡lculo de entregas
APIs Utilizadas:
â”œâ”€â”€ ðŸ›£ï¸ Directions API
â”‚   â”œâ”€â”€ Input: EndereÃ§o de entrega
â”‚   â”œâ”€â”€ Output: Rota, distÃ¢ncia, tempo
â”‚   â””â”€â”€ Uso: CÃ¡lculo de frete automÃ¡tico
â”œâ”€â”€ ðŸ—ºï¸ Maps JavaScript API
â”‚   â”œâ”€â”€ Input: Coordenadas da rota
â”‚   â”œâ”€â”€ Output: VisualizaÃ§Ã£o interativa
â”‚   â””â”€â”€ Uso: Interface de acompanhamento
â””â”€â”€ ðŸ’° PrecificaÃ§Ã£o: Valor/hora configurÃ¡vel
```

## ðŸ”„ Fluxos de IntegraÃ§Ã£o CrÃ­ticos

### **ðŸ“Š SincronizaÃ§Ã£o de Contatos (People API)**
```mermaid
sequenceDiagram
    participant GesN as ðŸ“¦ GesN System
    participant Google as ðŸ‘¥ Google People API
    
    Note over GesN, Google: SincronizaÃ§Ã£o AutomÃ¡tica DiÃ¡ria
    
    GesN->>Google: Buscar contatos atualizados
    Google-->>GesN: Lista de contatos + metadados
    
    GesN->>GesN: Comparar com base local
    GesN->>GesN: Identificar novos/alterados
    
    alt Novos contatos no Google
        GesN->>GesN: Criar Customer/Supplier
    else Contatos alterados
        GesN->>GesN: Atualizar dados locais
    end
    
    GesN->>Google: Enviar novos clientes
    Google-->>GesN: ConfirmaÃ§Ã£o de criaÃ§Ã£o
```

### **ðŸ“… Agendamento de ProduÃ§Ã£o (Calendar API)**
```mermaid
sequenceDiagram
    participant Order as ðŸ’° OrderEntry
    participant Prod as ðŸ­ ProductionOrder
    participant GesN as ðŸ“¦ GesN System
    participant Calendar as ðŸ“… Google Calendar
    
    Order->>Order: Status = "SentToProduction"
    Order->>Prod: Criar ProductionOrder
    
    Prod->>GesN: Calcular data/hora produÃ§Ã£o
    GesN->>GesN: Estimar duraÃ§Ã£o total
    
    GesN->>Calendar: Criar evento
    Note over Calendar: TÃ­tulo: "ProduÃ§Ã£o Order #123"<br/>Data: Calculada<br/>DuraÃ§Ã£o: Estimada
    Calendar-->>GesN: Evento criado (ID)
    
    GesN->>Prod: Salvar eventId do Google
```

### **ðŸ—ºï¸ CÃ¡lculo de Rotas (Maps Platform)**
```mermaid
sequenceDiagram
    participant User as ðŸ‘¤ UsuÃ¡rio
    participant GesN as ðŸ“¦ GesN System
    participant Directions as ðŸ›£ï¸ Directions API
    participant Maps as ðŸ—ºï¸ Maps JavaScript API
    
    User->>GesN: Inserir endereÃ§o entrega
    GesN->>Directions: Calcular rota
    Note over Directions: Origem: Empresa<br/>Destino: Cliente
    
    Directions-->>GesN: DistÃ¢ncia + Tempo + Rota
    GesN->>GesN: Calcular frete (valor/hora)
    
    GesN->>Maps: Carregar visualizaÃ§Ã£o
    Maps-->>User: Mapa com rota traÃ§ada
    
    User->>GesN: Confirmar entrega
    GesN->>GesN: Salvar dados de logÃ­stica
```

## ðŸŽ¯ BenefÃ­cios das IntegraÃ§Ãµes

### **ðŸ“Š Impactos de NegÃ³cio**

| IntegraÃ§Ã£o | BenefÃ­cio | MÃ©trica |
|------------|-----------|---------|
| **People API** | ReduÃ§Ã£o duplicaÃ§Ã£o dados | -80% tempo cadastro |
| **Calendar API** | Visibilidade produÃ§Ã£o | +50% aderÃªncia prazos |
| **Maps Platform** | PrecisÃ£o logÃ­stica | -30% custos entrega |

### **âš¡ AutomaÃ§Ãµes Habilitadas**

#### **ðŸ”„ SincronizaÃ§Ã£o AutomÃ¡tica**
- Novos clientes â†’ Google Contacts
- Contatos atualizados â†’ Base GesN
- FrequÃªncia: DiÃ¡ria (configurable)

#### **ðŸ“… Agendamento Inteligente**
- ProductionOrder â†’ Evento Calendar
- Conflitos de agenda â†’ Alertas
- Rescheduling â†’ AtualizaÃ§Ã£o automÃ¡tica

#### **ðŸšš LogÃ­stica Otimizada**
- EndereÃ§o â†’ Rota automÃ¡tica
- DistÃ¢ncia â†’ CÃ¡lculo frete
- VisualizaÃ§Ã£o â†’ Tracking entrega

## ðŸš¨ DependÃªncias CrÃ­ticas

### **âš ï¸ Riscos Identificados**

| Sistema | Risco | Impacto | MitigaÃ§Ã£o |
|---------|-------|---------|-----------|
| **Google APIs** | Indisponibilidade | Alto | Circuit breaker + cache |
| **Internet** | Conectividade | CrÃ­tico | Modo offline limitado |
| **Quotas Google** | Limite atingido | MÃ©dio | Monitoramento + alertas |

### **ðŸ”§ EstratÃ©gias de ContingÃªncia**

#### **ðŸŒ Google APIs IndisponÃ­veis**
```
Fallback Strategy:
â”œâ”€â”€ ðŸ‘¥ People API: Usar cache local (atÃ© 7 dias)
â”œâ”€â”€ ðŸ“… Calendar API: Agendar localmente + sync posterior  
â”œâ”€â”€ ðŸ—ºï¸ Maps API: Usar histÃ³rico de rotas similares
â””â”€â”€ ðŸš¨ Alertas: Notificar administrators
```

#### **ðŸ“Š Monitoramento de SaÃºde**
```
Health Checks (a cada 5 min):
â”œâ”€â”€ ðŸŸ¢ APIs Response Time < 2s
â”œâ”€â”€ ðŸŸ¡ APIs Response Time 2-5s  
â”œâ”€â”€ ðŸ”´ APIs Response Time > 5s ou erro
â””â”€â”€ ðŸ“§ Escalation: ApÃ³s 3 falhas consecutivas
```

## ðŸ“ˆ EvoluÃ§Ã£o Futura

### **ðŸš€ IntegraÃ§Ãµes Planejadas**

| Prioridade | Sistema | PropÃ³sito |
|------------|---------|-----------|
| **Alta** | Google Drive | Armazenamento documentos |
| **MÃ©dia** | Google Sheets | RelatÃ³rios automÃ¡ticos |
| **Baixa** | Google Meet | ReuniÃµes com fornecedores |

### **ðŸŽ¯ Objetivos EstratÃ©gicos**
- **Reduzir**: Entrada manual de dados
- **Aumentar**: AutomaÃ§Ã£o de processos
- **Melhorar**: ExperiÃªncia do usuÃ¡rio
- **Otimizar**: Custos operacionais

---

**Arquivo**: `level1-system-context.md`  
**NÃ­vel C4**: 1 - System Context  
**AudiÃªncia**: Stakeholders nÃ£o-tÃ©cnicos  
**Foco**: O que o sistema faz + dependÃªncias externas  
**AtualizaÃ§Ã£o**: 16/06/2025
