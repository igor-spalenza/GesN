# 🏗️ C4 MODEL - SISTEMA GesN

## 🎯 Visão Geral
O C4 Model (Context, Containers, Components, Code) fornece uma abordagem hierárquica para visualizar a arquitetura do sistema GesN, desde o contexto mais alto até detalhes de implementação.

## 📋 Estrutura dos Níveis C4

### **📊 Níveis de Abstração**

| Nível | Foco | Público Alvo | Arquivo |
|-------|------|--------------|---------|
| **1. System Context** | Sistema como caixa preta + atores externos | Stakeholders não-técnicos | [`level1-system-context.md`](./level1-system-context.md) |
| **2. Container** | Aplicações de alto nível e armazenamento de dados | Arquitetos técnicos e desenvolvedores | [`level2-container-diagram.md`](./level2-container-diagram.md) |
| **3. Component** | Componentes dentro de cada container | Arquitetos de software e developers | [`level3-component-diagrams.md`](./level3-component-diagrams.md) |
| **4. Code** | Classes e interfaces específicas | Desenvolvedores | [`level4-code-diagrams.md`](./level4-code-diagrams.md) |

## 🌐 Contexto do Sistema GesN

### **📦 Core System**
- **GesN**: Sistema Integrado de Gestão (SaaS)
- **Domínios**: Produto, Vendas, Produção, Compras, Financeiro

### **👥 Atores Principais**
- **Usuários Internos**: Gestores, Operadores, Administradores
- **Clientes**: Pessoas físicas e jurídicas
- **Fornecedores**: Empresas parceiras

### **🔌 Sistemas Externos**
- **Google Workspace**: Integração crítica
  - People API (sincronização contatos)
  - Calendar API (agendamento produção)
  - Maps Platform (logística e entregas)

## 🎨 Convenções Visuais C4

### **🎯 Cores por Tipo de Sistema**
- **📦 Core GesN**: `#00a86b` (Verde)
- **🌐 Sistemas Externos**: `#6b7280` (Cinza)
- **👥 Pessoas**: `#3b82f6` (Azul)
- **📱 Aplicações**: `#8b5cf6` (Roxo)

### **🔗 Tipos de Relacionamento**
- **→** Fluxo de dados/comandos
- **↔** Comunicação bidirecional
- **🔄** Sincronização
- **📡** API calls
- **📊** Consulta/leitura

### **📏 Elementos por Nível**

#### **Level 1 - System Context**
```
[Sistema] - Caixa com nome do sistema
(Pessoa) - Boneco/círculo com nome
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
[Service] - Lógica de negócio
[Repository] - Acesso a dados
[Integration] - Comunicação externa
```

#### **Level 4 - Code**
```
Class Product - Classes de domínio
Interface IProductService - Contratos
Enum ProductType - Enumerações
```

## 📊 Métricas e Benefícios

### **🎯 Benefícios por Nível**

| Nível | Benefício Principal |
|-------|-------------------|
| **System Context** | Alinhamento de stakeholders sobre escopo |
| **Container** | Decisões de arquitetura e deployment |
| **Component** | Organização de código e responsabilidades |
| **Code** | Implementação detalhada e padrões |

### **👥 Audiência por Nível**

| Nível | Audiência Principal |
|-------|-------------------|
| **1** | C-Level, Product Owners, Business Analysts |
| **2** | Arquitetos, Tech Leads, DevOps |
| **3** | Desenvolvedores Senior, Arquitetos de Software |
| **4** | Desenvolvedores, Code Reviewers |

---

**Criado em**: 16/06/2025  
**Versão**: 1.0  
**Padrão**: C4 Model by Simon Brown  
**Ferramenta**: Mermaid + Markdown
