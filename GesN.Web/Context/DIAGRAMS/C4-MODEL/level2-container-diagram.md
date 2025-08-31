# ðŸ—ï¸ C4 LEVEL 2 - CONTAINER DIAGRAM

## ðŸŽ¯ VisÃ£o Geral
Diagrama de containers do sistema GesN mostrando as aplicaÃ§Ãµes de alto nÃ­vel e tecnologias utilizadas. Este nÃ­vel mostra **"COMO"** o sistema Ã© implementado em termos de aplicaÃ§Ãµes executÃ¡veis e armazenamento de dados.

## ðŸ“Š Diagrama de Containers

```mermaid
C4Container
    title Container Diagram - GesN (Sistema Integrado de GestÃ£o)
    
    Person(users, "UsuÃ¡rios", "Gestores, operadores, clientes e fornecedores")
    System_Ext(google_workspace, "Google Workspace", "APIs: People, Calendar, Maps")
    
    Container_Boundary(gesn_system, "GesN System") {
        Container(reverse_proxy, "Reverse Proxy", "Traefik", "Roteamento de requisiÃ§Ãµes, SSL termination, load balancing para containers tenant")
        
        Container_Boundary(tenant_isolation, "Multi-Tenant Architecture") {
            Container(web_app_t1, "Web App Tenant 1", "ASP.NET Core MVC", "Interface web completa: 5 domÃ­nios + autenticaÃ§Ã£o + autorizaÃ§Ã£o")
            Container(web_app_t2, "Web App Tenant 2", "ASP.NET Core MVC", "Interface web completa: 5 domÃ­nios + autenticaÃ§Ã£o + autorizaÃ§Ã£o")
            Container(web_app_tn, "Web App Tenant N", "ASP.NET Core MVC", "Interface web completa: 5 domÃ­nios + autenticaÃ§Ã£o + autorizaÃ§Ã£o")
        }
        
        Container(background_service, "Background Service", "ASP.NET Core Worker", "Processamento IA, jobs automÃ¡ticos, sincronizaÃ§Ãµes, alertas")
        
        Container(api_gateway, "API Gateway", "ASP.NET Core Web API", "APIs REST para integraÃ§Ãµes externas e mobile apps futuras")
        
        Container_Boundary(data_layer, "Data Layer") {
            ContainerDb(db_tenant1, "Database Tenant 1", "SQLite", "Dados isolados do tenant 1: produtos, vendas, produÃ§Ã£o, compras, financeiro")
            ContainerDb(db_tenant2, "Database Tenant 2", "SQLite", "Dados isolados do tenant 2: produtos, vendas, produÃ§Ã£o, compras, financeiro")
            ContainerDb(db_tenantn, "Database Tenant N", "SQLite", "Dados isolados do tenant N: produtos, vendas, produÃ§Ã£o, compras, financeiro")
            ContainerDb(shared_db, "Shared Database", "SQLite", "Dados compartilhados: configuraÃ§Ãµes sistema, logs, mÃ©tricas")
        }
        
        Container(file_storage, "File Storage", "File System", "Armazenamento de documentos fiscais, imagens, relatÃ³rios gerados")
    }
    
    %% Relacionamentos UsuÃ¡rios
    Rel(users, reverse_proxy, "Acessa aplicaÃ§Ã£o", "HTTPS")
    
    %% Relacionamentos Reverse Proxy
    Rel(reverse_proxy, web_app_t1, "Roteia requisiÃ§Ãµes", "HTTP")
    Rel(reverse_proxy, web_app_t2, "Roteia requisiÃ§Ãµes", "HTTP")
    Rel(reverse_proxy, web_app_tn, "Roteia requisiÃ§Ãµes", "HTTP")
    Rel(reverse_proxy, api_gateway, "Roteia APIs", "HTTP")
    
    %% Relacionamentos Web Apps â†’ Databases
    Rel(web_app_t1, db_tenant1, "LÃª/escreve dados", "ADO.NET/Dapper")
    Rel(web_app_t2, db_tenant2, "LÃª/escreve dados", "ADO.NET/Dapper")
    Rel(web_app_tn, db_tenantn, "LÃª/escreve dados", "ADO.NET/Dapper")
    
    %% Relacionamentos Background Service
    Rel(background_service, db_tenant1, "Processa jobs", "ADO.NET/Dapper")
    Rel(background_service, db_tenant2, "Processa jobs", "ADO.NET/Dapper")
    Rel(background_service, db_tenantn, "Processa jobs", "ADO.NET/Dapper")
    Rel(background_service, shared_db, "Logs e mÃ©tricas", "ADO.NET/Dapper")
    
    %% Relacionamentos File Storage
    Rel(web_app_t1, file_storage, "Upload/download arquivos", "File I/O")
    Rel(web_app_t2, file_storage, "Upload/download arquivos", "File I/O")
    Rel(web_app_tn, file_storage, "Upload/download arquivos", "File I/O")
    Rel(background_service, file_storage, "Processa documentos", "File I/O")
    
    %% Relacionamentos Google Workspace
    Rel(background_service, google_workspace, "Sincroniza dados", "HTTPS/REST APIs")
    Rel(web_app_t1, google_workspace, "Maps integration", "HTTPS/JavaScript APIs")
    Rel(web_app_t2, google_workspace, "Maps integration", "HTTPS/JavaScript APIs")
    Rel(web_app_tn, google_workspace, "Maps integration", "HTTPS/JavaScript APIs")
    
    %% Styling
    UpdateElementStyle(reverse_proxy, $bgColor="#f59e0b", $fontColor="white")
    UpdateElementStyle(web_app_t1, $bgColor="#00a86b", $fontColor="white")
    UpdateElementStyle(web_app_t2, $bgColor="#00a86b", $fontColor="white")
    UpdateElementStyle(web_app_tn, $bgColor="#00a86b", $fontColor="white")
    UpdateElementStyle(background_service, $bgColor="#8b5cf6", $fontColor="white")
    UpdateElementStyle(api_gateway, $bgColor="#3b82f6", $fontColor="white")
    UpdateElementStyle(file_storage, $bgColor="#6b7280", $fontColor="white")
    UpdateElementStyle(google_workspace, $bgColor="#ea4335", $fontColor="white")
```

## ðŸ—ï¸ Detalhamento dos Containers

### **ðŸŒ Reverse Proxy (Traefik)**
```
Responsabilidades:
â”œâ”€â”€ ðŸ”€ Roteamento de requisiÃ§Ãµes por tenant
â”œâ”€â”€ ðŸ”’ SSL/TLS termination
â”œâ”€â”€ âš–ï¸ Load balancing entre instÃ¢ncias
â”œâ”€â”€ ðŸ“Š Monitoring e health checks
â””â”€â”€ ðŸ›¡ï¸ Rate limiting e security headers

ConfiguraÃ§Ã£o:
â”œâ”€â”€ ðŸŽ¯ Routes baseadas em subdomain/header
â”œâ”€â”€ ðŸ“œ SSL certificates automÃ¡ticos (Let's Encrypt)
â”œâ”€â”€ ðŸ“ˆ MÃ©tricas expostas para Prometheus
â””â”€â”€ ðŸ”§ Configuration via Docker labels
```

### **ðŸ’» Web Applications (ASP.NET Core MVC)**
```
Stack TecnolÃ³gico:
â”œâ”€â”€ ðŸŽ¨ Frontend: HTML5, CSS3, JavaScript (ES6+), Bootstrap
â”œâ”€â”€ âš™ï¸ Backend: ASP.NET Core 8.0 MVC
â”œâ”€â”€ ðŸ” Authentication: ASP.NET Core Identity
â”œâ”€â”€ ðŸ—„ï¸ Data Access: Dapper (micro-ORM)
â””â”€â”€ ðŸ“¦ Deployment: Docker containers

DomÃ­nios Implementados:
â”œâ”€â”€ ðŸ“¦ Produto: CatÃ¡logo, tipos, componentes, grupos
â”œâ”€â”€ ðŸ’° Vendas: Clientes, pedidos, itens, status
â”œâ”€â”€ ðŸ­ ProduÃ§Ã£o: Demandas, ordens, composiÃ§Ã£o, execuÃ§Ã£o
â”œâ”€â”€ ðŸ›’ Compras: Fornecedores, ordens, IA processing, estoque
â””â”€â”€ ðŸ’³ Financeiro: Contas, transaÃ§Ãµes, fluxo de caixa

Arquitetura:
â”œâ”€â”€ ðŸŽ® Controllers: Pontos de entrada HTTP
â”œâ”€â”€ âš™ï¸ Services: LÃ³gica de negÃ³cio por domÃ­nio
â”œâ”€â”€ ðŸ—„ï¸ Repositories: Acesso a dados (Repository Pattern)
â””â”€â”€ ðŸ“‹ Models: ViewModels e DTOs
```

### **ðŸ¤– Background Service (ASP.NET Core Worker)**
```
Responsabilidades:
â”œâ”€â”€ ðŸ§  Processamento IA (OCR + ML para notas fiscais)
â”œâ”€â”€ â° Jobs automÃ¡ticos (alertas, sincronizaÃ§Ãµes)
â”œâ”€â”€ ðŸ”„ SincronizaÃ§Ã£o Google Workspace
â”œâ”€â”€ ðŸ“Š GeraÃ§Ã£o de relatÃ³rios automÃ¡ticos
â””â”€â”€ ðŸš¨ Monitoramento e alertas

Jobs Principais:
â”œâ”€â”€ ðŸ•• Daily: VerificaÃ§Ã£o contas vencidas
â”œâ”€â”€ ðŸ•¡ Hourly: SincronizaÃ§Ã£o contatos Google
â”œâ”€â”€ ðŸ• Continuous: Processamento documentos IA
â”œâ”€â”€ ðŸ•• Daily: Backup automÃ¡tico databases
â””â”€â”€ ðŸ• Real-time: Alertas crÃ­ticos de negÃ³cio
```

### **ðŸ”Œ API Gateway (ASP.NET Core Web API)**
```
PropÃ³sito: Futuras integraÃ§Ãµes e apps mobile
APIs Expostas:
â”œâ”€â”€ ðŸ“¦ /api/products: CatÃ¡logo de produtos
â”œâ”€â”€ ðŸ’° /api/orders: Pedidos e status
â”œâ”€â”€ ðŸ­ /api/production: Status de produÃ§Ã£o
â”œâ”€â”€ ðŸ“Š /api/reports: RelatÃ³rios pÃºblicos
â””â”€â”€ ðŸ” /api/auth: AutenticaÃ§Ã£o externa

PadrÃµes:
â”œâ”€â”€ ðŸ“œ OpenAPI/Swagger documentation
â”œâ”€â”€ ðŸ” JWT token authentication
â”œâ”€â”€ âš¡ Rate limiting por cliente
â””â”€â”€ ðŸ“Š Logging e monitoring
```

### **ðŸ—„ï¸ Data Layer (SQLite Databases)**

#### **ðŸ“Š Database per Tenant**
```
Estrutura por Tenant:
â”œâ”€â”€ ðŸ“¦ Product tables: Product, ProductCategory, etc.
â”œâ”€â”€ ðŸ’° Sales tables: Customer, OrderEntry, OrderItem
â”œâ”€â”€ ðŸ­ Production tables: Demand, ProductComposition, ProductionOrder
â”œâ”€â”€ ðŸ›’ Purchasing tables: Supplier, PurchaseOrder, Ingredient
â”œâ”€â”€ ðŸ’³ Financial tables: AccountReceivable, AccountPayable, Transaction
â””â”€â”€ ðŸ” Identity tables: Users, Roles, Claims (tenant-specific)

Isolamento:
â”œâ”€â”€ ðŸ  Cada tenant = 1 database file separado
â”œâ”€â”€ ðŸ”’ Zero cross-tenant data leakage
â”œâ”€â”€ ðŸ“¦ Backup e restore independentes
â””â”€â”€ ðŸš€ Scaling horizontal por tenant
```

#### **ðŸŒ Shared Database**
```
Dados Compartilhados:
â”œâ”€â”€ âš™ï¸ System configurations
â”œâ”€â”€ ðŸ“Š Global metrics e analytics
â”œâ”€â”€ ðŸ“œ Audit logs cross-tenant
â”œâ”€â”€ ðŸ”§ Background job status
â””â”€â”€ ðŸ’¾ Cache compartilhado
```

### **ðŸ“ File Storage (File System)**
```
OrganizaÃ§Ã£o:
â”œâ”€â”€ ðŸ“„ /fiscal-documents/{tenantId}/{year}/{month}/
â”œâ”€â”€ ðŸ–¼ï¸ /product-images/{tenantId}/products/
â”œâ”€â”€ ðŸ“Š /reports/{tenantId}/{reportType}/
â”œâ”€â”€ ðŸ’¾ /backups/{tenantId}/{date}/
â””â”€â”€ ðŸ“œ /logs/{date}/

CaracterÃ­sticas:
â”œâ”€â”€ ðŸ—‚ï¸ Estrutura hierÃ¡rquica por tenant
â”œâ”€â”€ ðŸ”’ Isolamento fÃ­sico de arquivos
â”œâ”€â”€ ðŸ“¦ CompressÃ£o automÃ¡tica para backups
â””â”€â”€ ðŸ—‘ï¸ Limpeza automÃ¡tica de arquivos antigos
```

## ðŸ”„ Fluxos de Dados Principais

### **ðŸ“Š Processamento de Pedido**
```mermaid
sequenceDiagram
    participant User as ðŸ‘¤ UsuÃ¡rio
    participant Proxy as ðŸ”€ Traefik
    participant WebApp as ðŸ’» Web App
    participant DB as ðŸ—„ï¸ Database
    participant BGService as ðŸ¤– Background Service
    
    User->>Proxy: POST /orders (HTTPS)
    Proxy->>WebApp: Roteia para tenant correto
    WebApp->>DB: Salva OrderEntry + OrderItems
    WebApp-->>User: ConfirmaÃ§Ã£o criaÃ§Ã£o
    
    WebApp->>BGService: Trigger processamento (async)
    BGService->>DB: Gera Demands automÃ¡ticas
    BGService->>DB: Atualiza status produÃ§Ã£o
```

### **ðŸ¤– Processamento IA**
```mermaid
sequenceDiagram
    participant User as ðŸ‘¤ UsuÃ¡rio
    participant WebApp as ðŸ’» Web App
    participant FileStorage as ðŸ“ File Storage
    participant BGService as ðŸ¤– Background Service
    participant DB as ðŸ—„ï¸ Database
    
    User->>WebApp: Upload nota fiscal
    WebApp->>FileStorage: Salva documento
    WebApp->>BGService: Queue para processamento IA
    
    BGService->>FileStorage: LÃª documento
    BGService->>BGService: Processa OCR + ML
    BGService->>DB: Salva dados extraÃ­dos
    BGService->>WebApp: Notifica conclusÃ£o
    WebApp-->>User: Mostra formulÃ¡rio prÃ©-preenchido
```

### **ðŸ”„ SincronizaÃ§Ã£o Google**
```mermaid
sequenceDiagram
    participant BGService as ðŸ¤– Background Service
    participant Google as ðŸŒ Google APIs
    participant DB as ðŸ—„ï¸ Database
    participant WebApp as ðŸ’» Web App
    
    Note over BGService: Job executa a cada hora
    
    BGService->>Google: Busca contatos atualizados
    Google-->>BGService: Lista de contatos
    BGService->>DB: Compara e atualiza
    
    BGService->>Google: Envia novos clientes
    Google-->>BGService: ConfirmaÃ§Ã£o
    
    BGService->>WebApp: Invalida cache (SignalR)
```

## ðŸš€ Deployment e Infraestrutura

### **ðŸ³ ContainerizaÃ§Ã£o Docker**
```yaml
# docker-compose.yml structure
services:
  traefik:
    image: traefik:v3.0
    ports: ["80:80", "443:443"]
    
  gesn-tenant1:
    image: gesn-webapp:latest
    environment:
      - TENANT_ID=tenant1
      - DB_PATH=/data/tenant1.db
    
  gesn-tenant2:
    image: gesn-webapp:latest
    environment:
      - TENANT_ID=tenant2
      - DB_PATH=/data/tenant2.db
      
  gesn-background:
    image: gesn-background:latest
    volumes: ["/data:/data", "/files:/files"]
    
  gesn-api:
    image: gesn-api:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

### **ðŸ“Š Monitoring e Observabilidade**
```
Stack de Monitoring:
â”œâ”€â”€ ðŸ“ˆ Metrics: Prometheus + Grafana
â”œâ”€â”€ ðŸ“œ Logs: Serilog â†’ ELK Stack
â”œâ”€â”€ ðŸ” Tracing: OpenTelemetry
â”œâ”€â”€ ðŸš¨ Alerting: AlertManager
â””â”€â”€ ðŸ“Š Health Checks: ASP.NET Core Health Checks

Dashboards:
â”œâ”€â”€ ðŸŒ System: CPU, memory, disk, network
â”œâ”€â”€ ðŸ“¦ Application: Response times, error rates
â”œâ”€â”€ ðŸ—„ï¸ Database: Query performance, connections
â”œâ”€â”€ ðŸ‘¥ Business: Orders/hour, revenue, efficiency
â””â”€â”€ ðŸ”Œ External APIs: Google APIs health
```

## ðŸ”’ SeguranÃ§a e Isolamento

### **ðŸ  Multi-Tenancy Strategy**
```
Isolamento por Container + Database:
â”œâ”€â”€ ðŸ—ï¸ Infrastructure: 1 container per tenant
â”œâ”€â”€ ðŸ—„ï¸ Data: 1 SQLite database per tenant
â”œâ”€â”€ ðŸ“ Files: DiretÃ³rios separados por tenant
â”œâ”€â”€ ðŸŒ Network: Routing via subdomain/headers
â””â”€â”€ ðŸ” Auth: Identity per tenant database
```

### **ðŸ” Security Layers**
```
Camadas de SeguranÃ§a:
â”œâ”€â”€ ðŸŒ Network: Traefik SSL/TLS termination
â”œâ”€â”€ ðŸŽ¯ Application: ASP.NET Core Identity + Authorization
â”œâ”€â”€ ðŸ—„ï¸ Data: Database-level isolation
â”œâ”€â”€ ðŸ“ File: File system permissions
â””â”€â”€ ðŸ”Œ API: JWT tokens + rate limiting
```

## ðŸ“ˆ Escalabilidade

### **ðŸš€ Horizontal Scaling**
```
EstratÃ©gias por Container:
â”œâ”€â”€ ðŸ’» Web Apps: Scale por tenant (demand-based)
â”œâ”€â”€ ðŸ¤– Background Service: Single instance com job distribution
â”œâ”€â”€ ðŸ”Œ API Gateway: Load balancer com mÃºltiplas instÃ¢ncias
â”œâ”€â”€ ðŸ—„ï¸ Databases: SQLite = local, sem bottleneck
â””â”€â”€ ðŸ“ File Storage: Shared volumes ou S3-compatible
```

### **ðŸ“Š Performance Targets**
```
SLA por Container:
â”œâ”€â”€ ðŸ’» Web Apps: < 500ms response time, 99.9% uptime
â”œâ”€â”€ ðŸ¤– Background: < 30s IA processing, 99.5% success
â”œâ”€â”€ ðŸ”Œ APIs: < 200ms response time, 99.9% uptime
â”œâ”€â”€ ðŸ—„ï¸ Database: < 100ms queries, 99.99% availability
â””â”€â”€ ðŸ“ File Storage: < 1s upload/download, 99.9% availability
```

---

**Arquivo**: `level2-container-diagram.md`  
**NÃ­vel C4**: 2 - Container  
**AudiÃªncia**: Arquitetos tÃ©cnicos e desenvolvedores  
**Foco**: Como o sistema Ã© implementado em aplicaÃ§Ãµes executÃ¡veis  
**AtualizaÃ§Ã£o**: 16/06/2025
