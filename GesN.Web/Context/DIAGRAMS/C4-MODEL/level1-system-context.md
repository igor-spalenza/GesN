# 🌐 C4 LEVEL 1 - SYSTEM CONTEXT

## 🎯 Visão Geral
Diagrama de contexto do sistema GesN mostrando como o sistema se relaciona com seus usuários e outros sistemas. Este é o nível mais alto de abstração, focando no **"O QUE"** o sistema faz, não no **"COMO"**.

## 📊 Diagrama de Contexto do Sistema

```mermaid
C4Context
    title Sistema Context - GesN (Sistema Integrado de Gestão)
    
    Person(managers, "Gestores", "Administradores, supervisores e responsáveis pela operação")
    Person(operators, "Operadores", "Funcionários que executam produção, vendas e operações diárias")
    Person(customers, "Clientes", "Pessoas físicas e jurídicas que fazem pedidos")
    Person(suppliers, "Fornecedores", "Empresas que fornecem ingredientes e matérias-primas")
    
    System(gesn, "GesN", "Sistema SaaS integrado para gestão de negócios com 5 domínios:<br/>📦 Produto • 💰 Vendas • 🏭 Produção • 🛒 Compras • 💳 Financeiro")
    
    System_Ext(google_workspace, "Google Workspace", "Plataforma Google para produtividade empresarial:<br/>• People API (contatos)<br/>• Calendar API (agendamento)<br/>• Maps Platform (logística)")
    
    %% Relacionamentos Usuários → GesN
    Rel(managers, gesn, "Gerenciam negócio", "Web interface")
    Rel(operators, gesn, "Operam sistema", "Web interface")  
    Rel(customers, gesn, "Fazem pedidos", "Web interface")
    Rel(suppliers, gesn, "Fornecem produtos", "Web interface")
    
    %% Relacionamentos GesN ↔ Google Workspace
    Rel(gesn, google_workspace, "Sincroniza contatos", "People API")
    Rel(gesn, google_workspace, "Agenda produção", "Calendar API")
    Rel(gesn, google_workspace, "Calcula rotas", "Maps Platform")
    
    UpdateElementStyle(gesn, $bgColor="#00a86b", $fontColor="white", $borderColor="#00a86b")
    UpdateElementStyle(google_workspace, $bgColor="#ea4335", $fontColor="white", $borderColor="#ea4335")
    UpdateElementStyle(managers, $bgColor="#1a73e8", $fontColor="white")
    UpdateElementStyle(operators, $bgColor="#34a853", $fontColor="white")
    UpdateElementStyle(customers, $bgColor="#fbbc04", $fontColor="black")
    UpdateElementStyle(suppliers, $bgColor="#9aa0a6", $fontColor="white")
```

## 👥 Atores do Sistema

### **🎭 Usuários Internos**

#### **👑 Gestores**
```
Perfil: Administradores, supervisores, responsáveis
Acesso: Todas as funcionalidades + relatórios
Principais Ações:
├── 📊 Visualizar dashboards executivos
├── 🎯 Definir metas e políticas
├── 📈 Analisar relatórios financeiros
├── ⚙️ Configurar sistema e usuários
└── 🔍 Auditar operações
```

#### **⚙️ Operadores**
```
Perfil: Funcionários operacionais
Acesso: Funcionalidades específicas por domínio
Principais Ações:
├── 📦 Gerenciar catálogo de produtos
├── 💰 Processar pedidos de venda
├── 🏭 Executar ordens de produção
├── 🛒 Gerenciar compras e estoque
└── 💳 Processar transações financeiras
```

### **🌐 Usuários Externos**

#### **👤 Clientes**
```
Perfil: Pessoas físicas e jurídicas
Acesso: Portal de clientes (limitado)
Principais Ações:
├── 🛍️ Fazer pedidos online
├── 📋 Acompanhar status de pedidos
├── 💳 Realizar pagamentos
├── 📞 Acessar histórico de compras
└── 📧 Receber notificações
```

#### **🏢 Fornecedores**
```
Perfil: Empresas parceiras
Acesso: Portal de fornecedores (limitado)
Principais Ações:
├── 📄 Receber ordens de compra
├── 📦 Confirmar entregas
├── 💰 Acompanhar faturas
├── 📊 Acessar relatórios de performance
└── 🔔 Receber notificações
```

## 🌐 Sistemas Externos

### **🚀 Google Workspace (Dependência Crítica)**

#### **👥 People API**
```
Propósito: Sincronização de contatos
Integração: Bidirecional
Fluxo:
├── 📥 GesN → Google: Novos clientes/fornecedores
├── 📤 Google → GesN: Contatos atualizados
├── 🔄 Sincronização: Automática (diária)
└── 🎯 Benefício: Centralização de contatos
```

#### **📅 Calendar API**
```
Propósito: Agendamento de produção
Integração: Unidirecional (GesN → Google)
Fluxo:
├── ⚡ Trigger: OrderEntry.Status = "SentToProduction"
├── 🏭 Evento: ProductionOrder criada
├── 📅 Ação: Criar evento no Google Calendar
├── ⏰ Dados: Data produção, produtos, tempo estimado
└── 🎯 Benefício: Visibilidade de agenda
```

#### **🗺️ Google Maps Platform**
```
Propósito: Logística e cálculo de entregas
APIs Utilizadas:
├── 🛣️ Directions API
│   ├── Input: Endereço de entrega
│   ├── Output: Rota, distância, tempo
│   └── Uso: Cálculo de frete automático
├── 🗺️ Maps JavaScript API
│   ├── Input: Coordenadas da rota
│   ├── Output: Visualização interativa
│   └── Uso: Interface de acompanhamento
└── 💰 Precificação: Valor/hora configurável
```

## 🔄 Fluxos de Integração Críticos

### **📊 Sincronização de Contatos (People API)**
```mermaid
sequenceDiagram
    participant GesN as 📦 GesN System
    participant Google as 👥 Google People API
    
    Note over GesN, Google: Sincronização Automática Diária
    
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
    Google-->>GesN: Confirmação de criação
```

### **📅 Agendamento de Produção (Calendar API)**
```mermaid
sequenceDiagram
    participant Order as 💰 OrderEntry
    participant Prod as 🏭 ProductionOrder
    participant GesN as 📦 GesN System
    participant Calendar as 📅 Google Calendar
    
    Order->>Order: Status = "SentToProduction"
    Order->>Prod: Criar ProductionOrder
    
    Prod->>GesN: Calcular data/hora produção
    GesN->>GesN: Estimar duração total
    
    GesN->>Calendar: Criar evento
    Note over Calendar: Título: "Produção Order #123"<br/>Data: Calculada<br/>Duração: Estimada
    Calendar-->>GesN: Evento criado (ID)
    
    GesN->>Prod: Salvar eventId do Google
```

### **🗺️ Cálculo de Rotas (Maps Platform)**
```mermaid
sequenceDiagram
    participant User as 👤 Usuário
    participant GesN as 📦 GesN System
    participant Directions as 🛣️ Directions API
    participant Maps as 🗺️ Maps JavaScript API
    
    User->>GesN: Inserir endereço entrega
    GesN->>Directions: Calcular rota
    Note over Directions: Origem: Empresa<br/>Destino: Cliente
    
    Directions-->>GesN: Distância + Tempo + Rota
    GesN->>GesN: Calcular frete (valor/hora)
    
    GesN->>Maps: Carregar visualização
    Maps-->>User: Mapa com rota traçada
    
    User->>GesN: Confirmar entrega
    GesN->>GesN: Salvar dados de logística
```

## 🎯 Benefícios das Integrações

### **📊 Impactos de Negócio**

| Integração | Benefício | Métrica |
|------------|-----------|---------|
| **People API** | Redução duplicação dados | -80% tempo cadastro |
| **Calendar API** | Visibilidade produção | +50% aderência prazos |
| **Maps Platform** | Precisão logística | -30% custos entrega |

### **⚡ Automações Habilitadas**

#### **🔄 Sincronização Automática**
- Novos clientes → Google Contacts
- Contatos atualizados → Base GesN
- Frequência: Diária (configurable)

#### **📅 Agendamento Inteligente**
- ProductionOrder → Evento Calendar
- Conflitos de agenda → Alertas
- Rescheduling → Atualização automática

#### **🚚 Logística Otimizada**
- Endereço → Rota automática
- Distância → Cálculo frete
- Visualização → Tracking entrega

## 🚨 Dependências Críticas

### **⚠️ Riscos Identificados**

| Sistema | Risco | Impacto | Mitigação |
|---------|-------|---------|-----------|
| **Google APIs** | Indisponibilidade | Alto | Circuit breaker + cache |
| **Internet** | Conectividade | Crítico | Modo offline limitado |
| **Quotas Google** | Limite atingido | Médio | Monitoramento + alertas |

### **🔧 Estratégias de Contingência**

#### **🌐 Google APIs Indisponíveis**
```
Fallback Strategy:
├── 👥 People API: Usar cache local (até 7 dias)
├── 📅 Calendar API: Agendar localmente + sync posterior  
├── 🗺️ Maps API: Usar histórico de rotas similares
└── 🚨 Alertas: Notificar administrators
```

#### **📊 Monitoramento de Saúde**
```
Health Checks (a cada 5 min):
├── 🟢 APIs Response Time < 2s
├── 🟡 APIs Response Time 2-5s  
├── 🔴 APIs Response Time > 5s ou erro
└── 📧 Escalation: Após 3 falhas consecutivas
```

## 📈 Evolução Futura

### **🚀 Integrações Planejadas**

| Prioridade | Sistema | Propósito |
|------------|---------|-----------|
| **Alta** | Google Drive | Armazenamento documentos |
| **Média** | Google Sheets | Relatórios automáticos |
| **Baixa** | Google Meet | Reuniões com fornecedores |

### **🎯 Objetivos Estratégicos**
- **Reduzir**: Entrada manual de dados
- **Aumentar**: Automação de processos
- **Melhorar**: Experiência do usuário
- **Otimizar**: Custos operacionais

---

**Arquivo**: `level1-system-context.md`  
**Nível C4**: 1 - System Context  
**Audiência**: Stakeholders não-técnicos  
**Foco**: O que o sistema faz + dependências externas  
**Atualização**: 16/06/2025
