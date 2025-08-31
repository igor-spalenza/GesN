# ðŸ—ï¸ C4 MODEL - SISTEMA GesN

## ðŸŽ¯ VisÃ£o Geral
O C4 Model (Context, Containers, Components, Code) fornece uma abordagem hierÃ¡rquica para visualizar a arquitetura do sistema GesN, desde o contexto mais alto atÃ© detalhes de implementaÃ§Ã£o.

## ðŸ“‹ Estrutura dos NÃ­veis C4

### **ðŸ“Š NÃ­veis de AbstraÃ§Ã£o**

| NÃ­vel | Foco | PÃºblico Alvo | Arquivo |
|-------|------|--------------|---------|
| **1. System Context** | Sistema como caixa preta + atores externos | Stakeholders nÃ£o-tÃ©cnicos | [`level1-system-context.md`](./level1-system-context.md) |
| **2. Container** | AplicaÃ§Ãµes de alto nÃ­vel e armazenamento de dados | Arquitetos tÃ©cnicos e desenvolvedores | [`level2-container-diagram.md`](./level2-container-diagram.md) |
| **3. Component** | Componentes dentro de cada container | Arquitetos de software e developers | [`level3-component-diagrams.md`](./level3-component-diagrams.md) |
| **4. Code** | Classes e interfaces especÃ­ficas | Desenvolvedores | [`level4-code-diagrams.md`](./level4-code-diagrams.md) |

## ðŸŒ Contexto do Sistema GesN

### **ðŸ“¦ Core System**
- **GesN**: Sistema Integrado de GestÃ£o (SaaS)
- **DomÃ­nios**: Produto, Vendas, ProduÃ§Ã£o, Compras, Financeiro

### **ðŸ‘¥ Atores Principais**
- **UsuÃ¡rios Internos**: Gestores, Operadores, Administradores
- **Clientes**: Pessoas fÃ­sicas e jurÃ­dicas
- **Fornecedores**: Empresas parceiras

### **ðŸ”Œ Sistemas Externos**
- **Google Workspace**: IntegraÃ§Ã£o crÃ­tica
  - People API (sincronizaÃ§Ã£o contatos)
  - Calendar API (agendamento produÃ§Ã£o)
  - Maps Platform (logÃ­stica e entregas)

## ðŸŽ¨ ConvenÃ§Ãµes Visuais C4

### **ðŸŽ¯ Cores por Tipo de Sistema**
- **ðŸ“¦ Core GesN**: `#00a86b` (Verde)
- **ðŸŒ Sistemas Externos**: `#6b7280` (Cinza)
- **ðŸ‘¥ Pessoas**: `#3b82f6` (Azul)
- **ðŸ“± AplicaÃ§Ãµes**: `#8b5cf6` (Roxo)

### **ðŸ”— Tipos de Relacionamento**
- **â†’** Fluxo de dados/comandos
- **â†”** ComunicaÃ§Ã£o bidirecional
- **ðŸ”„** SincronizaÃ§Ã£o
- **ðŸ“¡** API calls
- **ðŸ“Š** Consulta/leitura

### **ðŸ“ Elementos por NÃ­vel**

#### **Level 1 - System Context**
```
[Sistema] - Caixa com nome do sistema
(Pessoa) - Boneco/cÃ­rculo com nome
{Sistema Externo} - Caixa cinza com nome
```

#### **Level 2 - Container**
```
[Web Application] - Container principal
[Database] - Armazenamento
[API Gateway] - Ponto de entrada
[Background Service] - Processamento
```

#### **Level 3 - Component**
```
[Controller] - Pontos de entrada
[Service] - LÃ³gica de negÃ³cio
[Repository] - Acesso a dados
[Integration] - ComunicaÃ§Ã£o externa
```

#### **Level 4 - Code**
```
Class Product - Classes de domÃ­nio
Interface IProductService - Contratos
Enum ProductType - EnumeraÃ§Ãµes
```

## ðŸ“Š MÃ©tricas e BenefÃ­cios

### **ðŸŽ¯ BenefÃ­cios por NÃ­vel**

| NÃ­vel | BenefÃ­cio Principal |
|-------|-------------------|
| **System Context** | Alinhamento de stakeholders sobre escopo |
| **Container** | DecisÃµes de arquitetura e deployment |
| **Component** | OrganizaÃ§Ã£o de cÃ³digo e responsabilidades |
| **Code** | ImplementaÃ§Ã£o detalhada e padrÃµes |

### **ðŸ‘¥ AudiÃªncia por NÃ­vel**

| NÃ­vel | AudiÃªncia Principal |
|-------|-------------------|
| **1** | C-Level, Product Owners, Business Analysts |
| **2** | Arquitetos, Tech Leads, DevOps |
| **3** | Desenvolvedores Senior, Arquitetos de Software |
| **4** | Desenvolvedores, Code Reviewers |

---

**Criado em**: 16/06/2025  
**VersÃ£o**: 1.0  
**PadrÃ£o**: C4 Model by Simon Brown  
**Ferramenta**: Mermaid + Markdown
