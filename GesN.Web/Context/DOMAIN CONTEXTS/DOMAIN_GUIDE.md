# ðŸ“– DocumentaÃ§Ã£o de DomÃ­nios â€“ GesN SaaS

## ðŸ“‹ Ãndice

1. [IntroduÃ§Ã£o](#1-introduÃ§Ã£o)
   - 1.1 [Objetivo do Documento](#11-objetivo-do-documento)
   - 1.2 [PÃºblico-Alvo](#12-pÃºblico-alvo)
   - 1.3 [Escopo e VisÃ£o Geral](#13-escopo-e-visÃ£o-geral)

2. [Mapeamento de DomÃ­nios (DDD)](#2-mapeamento-de-domÃ­nios-ddd)
   - 2.1 [Context Map (VisÃ£o Geral)](#21-context-map-visÃ£o-geral)
   - 2.2 [Bounded Contexts (Por DomÃ­nio)](#22-bounded-contexts-por-domÃ­nio)
     - 2.2.1 [DOMÃNIO DE PRODUTO](#221-domÃ­nio-de-produto)
       - 2.2.1.1 [Responsabilidade Central](#2211-responsabilidade-central)
       - 2.2.1.2 [Principais Entidades](#2212-principais-entidades)
       - 2.2.1.3 [Tipos de Produtos e Casos de Uso](#2213-tipos-de-produtos-e-casos-de-uso)
         - 2.2.1.3.1 [Produto Simples](#22131-produto-simples-producttypesimple)
         - 2.2.1.3.2 [Produto Composto](#22132-produto-composto-producttypecomposite)
         - 2.2.1.3.3 [Grupo de Produtos](#22133-grupo-de-produtos-producttypegroup)
       - 2.2.1.4 [Regras de NegÃ³cio do DomÃ­nio](#2214-regras-de-negÃ³cio-do-domÃ­nio)
       - 2.2.1.5 [IntegraÃ§Ãµes com Outros DomÃ­nios](#2215-integraÃ§Ãµes-com-outros-domÃ­nios)
     - 2.2.2 [DOMÃNIO DE VENDAS](#222-domÃ­nio-de-vendas)
       - 2.2.2.1 [Responsabilidade Central](#2221-responsabilidade-central)
       - 2.2.2.2 [Principais Entidades](#2222-principais-entidades)
       - 2.2.2.3 [Fluxos de Trabalho e Jornada do UsuÃ¡rio](#2223-fluxos-de-trabalho-e-jornada-do-usuÃ¡rio)
         - 2.2.2.3.1 [Listagem e Acesso aos Pedidos](#22231-listagem-e-acesso-aos-pedidos)
         - 2.2.2.3.2 [CriaÃ§Ã£o de Novo Pedido](#22232-criaÃ§Ã£o-de-novo-pedido)
         - 2.2.2.3.3 [AdiÃ§Ã£o de Itens por Tipo de Produto](#22233-adiÃ§Ã£o-de-itens-por-tipo-de-produto)
         - 2.2.2.3.4 [FinalizaÃ§Ã£o e ConfirmaÃ§Ã£o](#22234-finalizaÃ§Ã£o-e-confirmaÃ§Ã£o)
       - 2.2.2.4 [Regras de NegÃ³cio do DomÃ­nio](#2224-regras-de-negÃ³cio-do-domÃ­nio)
       - 2.2.2.5 [IntegraÃ§Ãµes com Outros DomÃ­nios](#2225-integraÃ§Ãµes-com-outros-domÃ­nios)
     - 2.2.3 [DOMÃNIO DE PRODUÃ‡ÃƒO](#223-domÃ­nio-de-produÃ§Ã£o)
       - 2.2.3.1 [Responsabilidade Central](#2231-responsabilidade-central)
       - 2.2.3.2 [Principais Entidades](#2232-principais-entidades)
       - 2.2.3.3 [Fluxos de Trabalho e Jornada do UsuÃ¡rio](#2233-fluxos-de-trabalho-e-jornada-do-usuÃ¡rio)
         - 2.2.3.3.1 [Painel de Demandas (Production Dashboard)](#22331-painel-de-demandas-production-dashboard)
         - 2.2.3.3.2 [GeraÃ§Ã£o de Demandas](#22332-geraÃ§Ã£o-de-demandas)
         - 2.2.3.3.3 [Gerenciamento e ExecuÃ§Ã£o de Demandas](#22333-gerenciamento-e-execuÃ§Ã£o-de-demandas)
       - 2.2.3.4 [Regras de NegÃ³cio do DomÃ­nio](#2234-regras-de-negÃ³cio-do-domÃ­nio)
       - 2.2.3.5 [IntegraÃ§Ãµes com Outros DomÃ­nios](#2235-integraÃ§Ãµes-com-outros-domÃ­nios)
     - 2.2.4 [DOMÃNIO DE COMPRAS](#224-domÃ­nio-de-compras)
       - 2.2.4.1 [Responsabilidade Central](#2241-responsabilidade-central)
       - 2.2.4.2 [Principais Entidades](#2242-principais-entidades)
       - 2.2.4.3 [Fluxos de Trabalho e Jornada do UsuÃ¡rio](#2243-fluxos-de-trabalho-e-jornada-do-usuÃ¡rio)
         - 2.2.4.3.1 [GestÃ£o de Fornecedores e Ingredientes](#22431-gestÃ£o-de-fornecedores-e-ingredientes)
         - 2.2.4.3.2 [GeraÃ§Ã£o de Ordens de Compra](#22432-geraÃ§Ã£o-de-ordens-de-compra)
         - 2.2.4.3.3 [Ciclo de Vida da Ordem de Compra](#22433-ciclo-de-vida-da-ordem-de-compra)
       - 2.2.4.4 [Regras de NegÃ³cio do DomÃ­nio](#2244-regras-de-negÃ³cio-do-domÃ­nio)
       - 2.2.4.5 [IntegraÃ§Ãµes com Outros DomÃ­nios](#2245-integraÃ§Ãµes-com-outros-domÃ­nios)
     - 2.2.5 [DOMÃNIO FINANCEIRO](#225-domÃ­nio-financeiro)
       - 2.2.5.1 [Responsabilidade Central](#2251-responsabilidade-central)
       - 2.2.5.2 [Principais Entidades](#2252-principais-entidades)
       - 2.2.5.3 [Fluxos de Trabalho e Jornada do UsuÃ¡rio](#2253-fluxos-de-trabalho-e-jornada-do-usuÃ¡rio)
         - 2.2.5.3.1 [GestÃ£o de Contas a Receber](#22531-gestÃ£o-de-contas-a-receber)
         - 2.2.5.3.2 [GestÃ£o de Contas a Pagar](#22532-gestÃ£o-de-contas-a-pagar)
         - 2.2.5.3.3 [AnÃ¡lise do Fluxo de Caixa](#22533-anÃ¡lise-do-fluxo-de-caixa)
       - 2.2.5.4 [Regras de NegÃ³cio do DomÃ­nio](#2254-regras-de-negÃ³cio-do-domÃ­nio)
       - 2.2.5.5 [IntegraÃ§Ãµes com Outros DomÃ­nios](#2255-integraÃ§Ãµes-com-outros-domÃ­nios)
   - 2.3 [Ubiquitous Language (GlossÃ¡rio de Termos)](#23-ubiquitous-language-glossÃ¡rio-de-termos)

3. [Diagramas](#3-diagramas)
   - 3.1 [Context Map Geral](#31-context-map-geral)
   - 3.2 [Bounded Context - DomÃ­nio de Produto (Detalhado)](#32-bounded-context---domÃ­nio-de-produto-detalhado)
   - 3.3 [Fluxo de Dados - Produto para Vendas](#33-fluxo-de-dados---produto-para-vendas)

4. [Regras e Diretrizes](#4-regras-e-diretrizes)
   - 4.1 [ManutenÃ§Ã£o do Documento](#41-manutenÃ§Ã£o-do-documento)
   - 4.2 [PadrÃ£o de Nomenclatura](#42-padrÃ£o-de-nomenclatura)
   - 4.3 [CritÃ©rios para EvoluÃ§Ã£o dos DomÃ­nios](#43-critÃ©rios-para-evoluÃ§Ã£o-dos-domÃ­nios)

5. [PrÃ³ximos Passos](#5-prÃ³ximos-passos)
   - 5.1 [DocumentaÃ§Ã£o Pendente](#51-documentaÃ§Ã£o-pendente)
   - 5.2 [Diagramas Adicionais](#52-diagramas-adicionais)
   - 5.3 [ImplementaÃ§Ã£o](#53-implementaÃ§Ã£o)

---

## 1. IntroduÃ§Ã£o

### 1.1 Objetivo do Documento

Este documento apresenta o **mapeamento completo dos domÃ­nios** do sistema **GesN (GestÃ£o de NegÃ³cios)**, uma soluÃ§Ã£o SaaS desenvolvida para gerenciar integralmente os processos de pequenas e mÃ©dias empresas. A documentaÃ§Ã£o utiliza conceitos de **Domain-Driven Design (DDD)** para definir bounded contexts, estabelecer a linguagem ubÃ­qua e mapear as relaÃ§Ãµes entre os domÃ­nios.

### 1.2 PÃºblico-Alvo

- **Desenvolvedores**: Para compreender a arquitetura e implementar funcionalidades alinhadas aos domÃ­nios
- **Arquitetos de Software**: Para manter a integridade dos bounded contexts e evoluir a arquitetura
- **Analistas de NegÃ³cio**: Para entender o fluxo de valor e mapear requisitos aos domÃ­nios corretos
- **Stakeholders e Product Owners**: Para visualizar como os processos de negÃ³cio se traduzem em software

### 1.3 Escopo e VisÃ£o Geral

O **GesN** Ã© uma plataforma integrada que digitaliza a jornada completa de negÃ³cios, desde o cadastro de produtos atÃ© o controle financeiro. O sistema Ã© estruturado em **5 domÃ­nios principais** que se integram de forma sequencial e cÃ­clica:

```
PRODUTO â†’ VENDAS â†’ PRODUÃ‡ÃƒO â†’ COMPRAS â†’ FINANCEIRO
   â†‘                                        â†“
   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€ RETROALIMENTAÃ‡ÃƒO â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

**CaracterÃ­sticas Principais:**
- **Arquitetura Multi-Tenant**: Isolamento completo por cliente via containers Docker
- **Tecnologia**: ASP.NET Core MVC, Dapper, SQLite, JavaScript (ES6+)
- **PadrÃµes**: DDD, Repository Pattern, Service Layer, MVC
- **UI/UX**: Sistema de abas dinÃ¢micas, autocomplete inteligente, grids interativas

---

## 2. Mapeamento de DomÃ­nios (DDD)

### 2.1 Context Map (VisÃ£o Geral)

O **Context Map** do GesN ilustra como os 5 domÃ­nios principais interagem atravÃ©s de integraÃ§Ãµes bem definidas:

```mermaid
graph LR
    A[ðŸ“¦ PRODUTO<br/>CatÃ¡logo Central] --> B[ðŸ’° VENDAS<br/>GestÃ£o de Pedidos]
    B --> C[ðŸ­ PRODUÃ‡ÃƒO<br/>Ordens de ProduÃ§Ã£o]
    C --> D[ðŸ›’ COMPRAS<br/>AquisiÃ§Ã£o de Insumos]
    B --> E[ðŸ’³ FINANCEIRO<br/>Contas e Fluxo de Caixa]
    D --> E
    E -.-> A
    C -.-> A
    
    style A fill:#e1f5fe
    style B fill:#f3e5f5
    style C fill:#e8f5e8
    style D fill:#fff3e0
    style E fill:#fce4ec
```

**Relacionamentos Chave:**
- **Produto â†’ Vendas**: CatÃ¡logo alimenta criaÃ§Ã£o de pedidos
- **Vendas â†’ ProduÃ§Ã£o**: Pedidos confirmados geram demandas de produÃ§Ã£o
- **Vendas â†’ Financeiro**: Pedidos geram contas a receber
- **ProduÃ§Ã£o â†’ Compras**: Demandas consomem ingredientes, disparando necessidades de compra
- **Compras â†’ Financeiro**: Ordens de compra geram contas a pagar
- **RetroalimentaÃ§Ã£o**: Dados financeiros e de produÃ§Ã£o influenciam decisÃµes de produto


```mermaid
graph TB
    subgraph "ðŸ—ï¸ SISTEMA GesN - DOMÃNIOS INTEGRADOS"
        direction TB
        
        subgraph "ðŸ“¦ DOMÃNIO DE PRODUTO"
            P[Product<br/>Base Abstract]
            SP[SimpleProduct<br/>Produtos BÃ¡sicos]
            CP[CompositeProduct<br/>PersonalizÃ¡veis]
            PG[ProductGroup<br/>Kits e Combos]
            PC[ProductCategory<br/>Categorias]
            PCH[ProductComponentHierarchy<br/>Camadas]
            PCO[ProductComponent<br/>Componentes]
        end
        
        subgraph "ðŸ’° DOMÃNIO DE VENDAS"
            OE[OrderEntry<br/>Pedidos]
            OI[OrderItem<br/>Itens do Pedido]
            CU[Customer<br/>Clientes]
        end
        
        subgraph "ðŸ­ DOMÃNIO DE PRODUÃ‡ÃƒO"
            D[Demand<br/>Demandas]
            PRO[ProductComposition<br/>Tarefas]
        end
        
        subgraph "ðŸ›’ DOMÃNIO DE COMPRAS"
            PO[PurchaseOrder<br/>Ordens de Compra]
            S[Supplier<br/>Fornecedores]
            I[Ingredient<br/>Ingredientes]
            IS[IngredientStock<br/>Estoque]
        end
        
        subgraph "ðŸ’³ DOMÃNIO FINANCEIRO"
            AR[AccountReceivable<br/>Contas a Receber]
            AP[AccountPayable<br/>Contas a Pagar]
            T[Transaction<br/>TransaÃ§Ãµes]
        end
    end
    
    P --> SP
    P --> CP
    P --> PG
    P --> PC
    CP --> PCH
    PCH --> PCO
    
    P -.->|"fornece catÃ¡logo"| OE
    OE --> OI
    OE --> CU
    
    OI -.->|"gera demandas"| D
    D --> PRO
    
    PRO -.->|"consome ingredientes"| IS
    IS --> I
    I --> PO
    PO --> S
    
    OE -.->|"gera contas a receber"| AR
    PO -.->|"gera contas a pagar"| AP
    AR --> T
    AP --> T
    
    style P fill:#1976d2,color:#fff
    style SP fill:#4fc3f7
    style CP fill:#4fc3f7
    style PG fill:#4fc3f7
    style OE fill:#9c27b0,color:#fff
    style D fill:#388e3c,color:#fff
    style PO fill:#f57c00,color:#fff
    style AR fill:#e91e63,color:#fff
```


### 2.2 Bounded Contexts (Por DomÃ­nio)

---

#### 2.2.1 **DOMÃNIO DE PRODUTO**

##### 2.2.1.1 **Responsabilidade Central**
Gerenciamento completo do **CatÃ¡logo de Produtos e ServiÃ§os** da empresa. Ã‰ o **domÃ­nio fundamental** e ponto de partida obrigatÃ³rio para todo o sistema, pois todos os outros domÃ­nios dependem dos itens cadastrados aqui.

##### 2.2.1.2 **Principais Entidades**

###### **Entidades Centrais:**
- **`Product`** *(Classe Abstrata Base)*: Entidade principal que representa qualquer item vendÃ¡vel
- **`ProductCategory`**: Agrupamento lÃ³gico de produtos (ex: "Salgados Tradicionais", "Bolos Especiais")
- **`ProductIngredient`**: Relacionamento que define a "receita" de um produto (qual ingrediente e em que quantidade)

###### **Entidades de EspecializaÃ§Ã£o (HeranÃ§a TPH):**
- **`SimpleProduct`**: Produtos bÃ¡sicos sem configuraÃ§Ã£o (ex: "Coxinha Comum")
- **`CompositeProduct`**: Produtos personalizÃ¡veis com hierarquias de componentes (ex: "Bolo Personalizado")
- **`ProductGroup`**: Kits/combos flexÃ­veis com regras de troca (ex: "Kit Festa p/ 20 pessoas")

###### **Entidades de ComposiÃ§Ã£o:**
- **`ProductComponentHierarchy`**: Define "camadas" de personalizaÃ§Ã£o (ex: "Massa", "Recheio", "Cobertura")
- **`ProductComponent`**: OpÃ§Ãµes especÃ­ficas dentro de uma hierarquia (ex: "Massa de Chocolate", "Recheio de Brigadeiro")
- **`CompositeProductXHierarchy`**: Relacionamento M:N que define regras de composiÃ§Ã£o (quantidade mÃ­n/mÃ¡x, opcionalidade, ordem)

###### **Entidades de Agrupamento:**
- **`ProductGroupItem`**: Item individual dentro de um grupo (pode referenciar Product OU ProductCategory)
- **`ProductGroupExchangeRule`**: Define regras de proporÃ§Ã£o e troca entre itens do grupo

##### 2.2.1.3 **Tipos de Produtos e Casos de Uso**

###### 2.2.1.3.1 **ðŸ”· Produto Simples (`ProductType.Simple`)**
**DefiniÃ§Ã£o**: Unidade mais bÃ¡sica do catÃ¡logo, item concreto sem variaÃ§Ãµes.

**Estrutura de Dados:**
```csharp
SimpleProduct {
    Name: "Coxinha Comum"
    Price: 3.50
    Cost: 1.80
    CategoryId: "salgados-tradicionais"
    SKU: "COX001"
    AssemblyTime: 5 // minutos
}
```

**Fluxo de CriaÃ§Ã£o:**
1. Definir dados bÃ¡sicos (nome, preÃ§o, categoria)
2. Opcional: configurar receita com ingredientes
3. Definir tempo de montagem
4. Ativar produto no catÃ¡logo

**Exemplo Real:**
```
Produto: "Coxinha Comum"
â”œâ”€â”€ Categoria: "Salgados Tradicionais"  
â”œâ”€â”€ PreÃ§o: R$ 3,50
â”œâ”€â”€ Receita:
â”‚   â”œâ”€â”€ Massa de Coxinha: 50g
â”‚   â”œâ”€â”€ Frango Desfiado: 30g
â”‚   â””â”€â”€ Temperos Diversos: 5g
â””â”€â”€ Tempo de Montagem: 5 minutos
```

###### 2.2.1.3.2 **ðŸ”¶ Produto Composto (`ProductType.Composite`)**
**DefiniÃ§Ã£o**: Produto configurÃ¡vel onde o cliente escolhe componentes a partir de hierarquias prÃ©-definidas.

**Estrutura de Dados:**
```csharp
CompositeProduct {
    Name: "Bolo Personalizado p/ 20 pessoas"
    BasePrice: 45.00
    Hierarchies: [
        {
            HierarchyId: "massa-bolo",
            MinQuantity: 1,
            MaxQuantity: 1,
            IsOptional: false,
            AssemblyOrder: 1
        },
        {
            HierarchyId: "recheio-bolo", 
            MinQuantity: 1,
            MaxQuantity: 2,
            IsOptional: false,
            AssemblyOrder: 2
        }
    ]
}
```

**Processo de ConfiguraÃ§Ã£o:**
1. **Criar Hierarquias**: Definir camadas (Massa, Recheio, Cobertura, Opcionais)
2. **Criar Componentes**: OpÃ§Ãµes dentro de cada camada
3. **Configurar Produto**: Associar hierarquias com regras (min/max, ordem, opcionalidade)
4. **Definir PreÃ§os**: PreÃ§o base + custos adicionais por componente

**Exemplo Real Completo:**
```
Produto: "Bolo Personalizado p/ 20 pessoas"
â”œâ”€â”€ PreÃ§o Base: R$ 45,00
â”œâ”€â”€ Hierarquias:
â”‚   â”œâ”€â”€ 1. MASSA (obrigatÃ³ria, min=1, max=1)
â”‚   â”‚   â”œâ”€â”€ Massa Branca (sem custo adicional)
â”‚   â”‚   â”œâ”€â”€ Massa de Chocolate (sem custo adicional)
â”‚   â”‚   â””â”€â”€ Massa Red Velvet (+R$ 8,00)
â”‚   â”œâ”€â”€ 2. RECHEIO (obrigatÃ³ria, min=1, max=2)  
â”‚   â”‚   â”œâ”€â”€ Brigadeiro (sem custo adicional)
â”‚   â”‚   â”œâ”€â”€ Beijinho (sem custo adicional)
â”‚   â”‚   â”œâ”€â”€ Morango (+R$ 5,00)
â”‚   â”‚   â””â”€â”€ Nutella (+R$ 12,00)
â”‚   â”œâ”€â”€ 3. COBERTURA (obrigatÃ³ria, min=1, max=1)
â”‚   â”‚   â”œâ”€â”€ Chantilly (sem custo adicional)
â”‚   â”‚   â”œâ”€â”€ Ganache (+R$ 4,00)
â”‚   â”‚   â””â”€â”€ Fondant (+R$ 18,00)
â”‚   â””â”€â”€ 4. OPCIONAIS (opcional, min=0, max=3)
â”‚       â”œâ”€â”€ Frutas Vermelhas (+R$ 6,00)
â”‚       â”œâ”€â”€ Granulado Colorido (+R$ 2,00)
â”‚       â””â”€â”€ Vela Personalizada (+R$ 15,00)
```

**Fluxo de Venda:**
1. Cliente seleciona "Bolo Personalizado"
2. Sistema apresenta hierarquias em sequÃªncia
3. Cliente faz escolhas respeitando regras de quantidade
4. Sistema calcula preÃ§o final: Base + Î£(custos adicionais)
5. ConfiguraÃ§Ã£o Ã© salva no OrderItem + gera Demand para produÃ§Ã£o

###### 2.2.1.3.3 **ðŸ”¸ Grupo de Produtos (`ProductType.Group`)**
**DefiniÃ§Ã£o**: Pacote abstrato que agrupa mÃºltiplos produtos/categorias com regras de troca flexÃ­veis.

**Estrutura de Dados:**
```csharp
ProductGroup {
    Name: "Kit Festa p/ 50 pessoas"
    BasePrice: 280.00
    GroupItems: [
        {
            ProductId: "bolo-25-pessoas",
            Quantity: 2,
            DefaultQuantity: 2,
            IsOptional: false
        },
        {
            ProductCategoryId: "salgados-tradicionais",
            Quantity: 200,
            MinQuantity: 150,
            MaxQuantity: 250,
            DefaultQuantity: 200,
            IsOptional: false
        }
    ],
    ExchangeRules: [
        {
            SourceItem: "salgados-tradicionais",
            TargetItem: "salgados-especiais", 
            ExchangeRatio: 2.0 // 2 tradicionais = 1 especial
        }
    ]
}
```

**Exemplo Real:**
```
Kit: "Kit Festa p/ 50 pessoas" - R$ 280,00
â”œâ”€â”€ Itens Base:
â”‚   â”œâ”€â”€ 2x Bolo p/ 25 pessoas (fixo)
â”‚   â”œâ”€â”€ 200x Salgados Tradicionais (150-250, configurÃ¡vel)
â”‚   â”œâ”€â”€ 100x Doces Tradicionais (50-150, configurÃ¡vel)
â”‚   â””â”€â”€ 3x Refrigerante 2L (2-5, configurÃ¡vel)
â”œâ”€â”€ Regras de Troca:
â”‚   â”œâ”€â”€ Salgados Tradicionais (2) â†” Salgados Especiais (1)
â”‚   â”œâ”€â”€ Doces Tradicionais (3) â†” Torta Individual (1)
â”‚   â””â”€â”€ Refrigerante 2L (1) â†” Suco Natural 1L (1)
â””â”€â”€ Flexibilidade: Cliente pode trocar itens mantendo proporÃ§Ãµes
```

**Fluxo de ConfiguraÃ§Ã£o pelo Cliente:**
1. Cliente seleciona o kit base
2. Sistema apresenta itens configurÃ¡veis
3. Cliente ajusta quantidades dentro dos limites
4. Cliente aplica trocas baseadas nas regras
5. Sistema recalcula preÃ§o: Base + ajustes de quantidade + diferenÃ§as de troca
6. Kit configurado Ã© adicionado ao pedido

##### 2.2.1.4 **Regras de NegÃ³cio do DomÃ­nio**

###### **Regras Gerais:**
- **SKU Ãšnico**: Quando informado, deve ser Ãºnico em todo o sistema
- **Categoria Opcional**: Produtos podem existir sem categoria, mas Ã© recomendado
- **HeranÃ§a TPH**: Todos os tipos usam a tabela `Product` com discriminador `ProductType`
- **Estado Ativo**: Apenas produtos ativos aparecem no catÃ¡logo de vendas
- **Integridade Referencial**: Produto nÃ£o pode ser excluÃ­do se tiver OrderItems associados

###### **Regras por Tipo:**

**Produto Simples:**
- Nome e PreÃ§o sÃ£o obrigatÃ³rios
- Receita de ingredientes Ã© opcional mas recomendada para controle de custo
- Tempo de montagem padrÃ£o: 0 minutos

**Produto Composto:**
- Deve ter pelo menos 1 hierarquia associada
- Cada hierarquia deve ter pelo menos 1 componente ativo
- MinQuantity â‰¥ 1 para hierarquias obrigatÃ³rias
- AssemblyOrder deve ser sequencial (1, 2, 3...)
- ValidaÃ§Ã£o em tempo real: escolhas do cliente devem respeitar limites

**Grupo de Produtos:**
- Deve ter pelo menos 1 item no grupo
- Item pode ser Product OU ProductCategory (mutuamente exclusivo)
- MinQuantity â‰¤ DefaultQuantity â‰¤ MaxQuantity
- Regras de troca: Source â‰  Target, ExchangeRatio > 0
- CÃ¡lculo dinÃ¢mico de preÃ§o baseado em configuraÃ§Ã£o final

##### 2.2.1.5 **IntegraÃ§Ãµes com Outros DomÃ­nios**

###### **â†’ Vendas (Customer-Supplier)**
**IntegraÃ§Ã£o**: O domÃ­nio de Produto **fornece** o catÃ¡logo para Vendas
- **Dados Fornecidos**: Lista de produtos ativos, preÃ§os, configuraÃ§Ãµes disponÃ­veis
- **OperaÃ§Ãµes**: Busca de produtos, validaÃ§Ã£o de configuraÃ§Ãµes, cÃ¡lculo de preÃ§os
- **Protocolo**: Vendas consome via ProductService mÃ©todos como `GetActiveProducts()`, `ValidateProductConfiguration()`, `CalculatePrice()`

###### **â†’ ProduÃ§Ã£o (Customer-Supplier)**  
**IntegraÃ§Ã£o**: Produto **fornece** especificaÃ§Ãµes para produÃ§Ã£o
- **Dados Fornecidos**: Receitas (ProductIngredient), instruÃ§Ãµes de montagem, tempo de produÃ§Ã£o
- **OperaÃ§Ãµes**: Consulta de componentes para produtos compostos, tempo de assembly
- **Protocolo**: ProduÃ§Ã£o consulta via `GetProductComposition()`, `GetAssemblyInstructions()`

###### **â†’ Financeiro (Conformist)**
**IntegraÃ§Ã£o**: Produto **informa** custos para cÃ¡lculos financeiros
- **Dados Fornecidos**: Custo base dos produtos, custos adicionais de componentes
- **OperaÃ§Ãµes**: CÃ¡lculo de margem, anÃ¡lise de lucratividade por produto
- **Protocolo**: Financeiro consome via `CalculateProductCost()`, `GetCostBreakdown()`

> ðŸ“– **Para documentaÃ§Ã£o tÃ©cnica completa** do DomÃ­nio de Produto, incluindo arquitetura detalhada, cÃ³digo C#, fluxos Mermaid, exemplos prÃ¡ticos e consideraÃ§Ãµes de performance, consulte: **[DOMAIN_1_PRODUCT_CONTEXT.md](./DOMAIN_1_PRODUCT_CONTEXT.md)**

---

#### 2.2.2 **DOMÃNIO DE VENDAS**

##### 2.2.2.1 **Responsabilidade Central**
O **DomÃ­nio de Vendas** Ã© o coraÃ§Ã£o operacional do GesN, responsÃ¡vel por capturar, gerenciar e concretizar as transaÃ§Ãµes comerciais com os clientes. Este domÃ­nio consome diretamente os itens do **CatÃ¡logo de Produtos** e serve como o principal gatilho para os fluxos de trabalho dos domÃ­nios de **ProduÃ§Ã£o** e **Financeiro**.

A gestÃ£o de um pedido (`OrderEntry`) Ã© o processo central deste domÃ­nio. Um pedido bem-sucedido representa nÃ£o apenas uma entrada de receita, mas tambÃ©m uma demanda a ser produzida e uma sÃ©rie de transaÃ§Ãµes financeiras a serem rastreadas.

##### 2.2.2.2 **Principais Entidades**

###### **Entidades Centrais:**
- **`OrderEntry`**: Entidade central que representa um pedido de um cliente. ContÃ©m informaÃ§Ãµes do cabeÃ§alho da venda (cliente, datas, valor total, status atual)
- **`OrderItem`**: Representa um item de linha dentro de um `OrderEntry`. Cada item estÃ¡ associado a um `Product` e especifica quantidade, preÃ§o unitÃ¡rio e configuraÃ§Ãµes
- **`Customer`**: Representa o cliente (pessoa fÃ­sica ou jurÃ­dica) que realizou o pedido

###### **Entidades de IntegraÃ§Ã£o:**
- **`Demand`** *(DomÃ­nio de ProduÃ§Ã£o)*: Gerada automaticamente para produtos que necessitam fabricaÃ§Ã£o
- **`ProductComposition`** *(DomÃ­nio de ProduÃ§Ã£o)*: Detalha as escolhas especÃ­ficas feitas para produtos compostos
- **`AccountReceivable`** *(DomÃ­nio Financeiro)*: Conta a receber gerada automaticamente do pedido

##### 2.2.2.3 **Fluxos de Trabalho e Jornada do UsuÃ¡rio**

###### **2.2.2.3.1 Listagem e Acesso aos Pedidos**
- **Grid Principal**: VisualizaÃ§Ã£o de todos os pedidos com funcionalidades robustas
- **Busca e Filtragem**: Por cliente, status, perÃ­odo, valor
- **AÃ§Ãµes RÃ¡pidas**: Editar, visualizar detalhes, excluir, duplicar pedidos

###### **2.2.2.3.2 CriaÃ§Ã£o de Novo Pedido**
**Processo em Duas Etapas:**

**1. CriaÃ§Ã£o RÃ¡pida (Modal):**
```
- SeleÃ§Ã£o de Customer (autocomplete inteligente)
- DefiniÃ§Ã£o de datas (pedido e entrega)
- Tipo de pedido (delivery, retirada)
- GeraÃ§Ã£o automÃ¡tica de nÃºmero sequencial
```

**2. EdiÃ§Ã£o Detalhada (Aba DinÃ¢mica):**
```
- Abertura automÃ¡tica em nova aba
- Interface para adiÃ§Ã£o de itens
- ConfiguraÃ§Ã£o de produtos compostos
- AplicaÃ§Ã£o de regras de grupos
- CÃ¡lculo automÃ¡tico de totais
```

###### **2.2.2.3.3 AdiÃ§Ã£o de Itens por Tipo de Produto**

**Produto Simples:**
```
1. Busca e seleÃ§Ã£o do produto
2. DefiniÃ§Ã£o da quantidade
3. AplicaÃ§Ã£o do preÃ§o padrÃ£o
4. AdiÃ§Ã£o imediata ao pedido
```

**Produto Composto:**
```
1. SeleÃ§Ã£o do produto base
2. ApresentaÃ§Ã£o de hierarquias de componentes
3. ConfiguraÃ§Ã£o por camadas (massa, recheio, cobertura)
4. ValidaÃ§Ã£o de regras (min/max, opcionalidade)
5. CÃ¡lculo dinÃ¢mico: preÃ§o base + custos adicionais
6. GeraÃ§Ã£o automÃ¡tica de Demand para produÃ§Ã£o
```

**Grupo de Produtos:**
```
1. SeleÃ§Ã£o do kit base
2. VisualizaÃ§Ã£o de itens componentes
3. AplicaÃ§Ã£o de regras de troca (se disponÃ­veis)
4. ConfiguraÃ§Ã£o de quantidades dentro dos limites
5. CÃ¡lculo proporcional de preÃ§os
```

###### **2.2.2.3.4 FinalizaÃ§Ã£o e ConfirmaÃ§Ã£o**
```
1. RevisÃ£o completa do pedido
2. ValidaÃ§Ã£o de dados obrigatÃ³rios
3. Registro de condiÃ§Ãµes de pagamento
4. ConfirmaÃ§Ã£o final â†’ Status: "Pendente" â†’ "Confirmado"
5. Disparo automÃ¡tico de integraÃ§Ãµes (ProduÃ§Ã£o + Financeiro)
```

##### 2.2.2.4 **Regras de NegÃ³cio do DomÃ­nio**

###### **Ciclo de Vida do Pedido (Status):**
1. **Pendente**: Pedido recÃ©m-criado, pode ser editado livremente
2. **Confirmado**: Cliente concordou, ediÃ§Ãµes restritas, Demand enviada para produÃ§Ã£o
3. **Em ProduÃ§Ã£o**: Equipe de produÃ§Ã£o iniciou o trabalho
4. **Pronto para Entrega**: ProduÃ§Ã£o concluÃ­da, aguardando logÃ­stica
5. **Entregue**: Produto entregue fisicamente ao cliente
6. **Faturado**: Pagamento totalmente recebido e conciliado
7. **Cancelado**: Pedido cancelado por qualquer motivo

###### **Regras de ValidaÃ§Ã£o:**
- `OrderEntry` nÃ£o pode ser confirmado sem `Customer` e pelo menos um `OrderItem`
- Valor total Ã© sempre a soma dos totais de seus `OrderItem`s
- Produtos inativos nÃ£o podem ser adicionados a novos pedidos
- ExclusÃ£o sÃ³ permitida em status iniciais (Pendente)
- Para outros status: fluxo correto Ã© cancelamento

###### **Regras de IntegraÃ§Ã£o:**
- **Produto Composto â†’ ProduÃ§Ã£o**: Gera `Demand` + `ProductComposition` automaticamente
- **ConfirmaÃ§Ã£o â†’ Financeiro**: Gera `AccountReceivable` com condiÃ§Ãµes de pagamento
- **Cancelamento**: Cancela `Demand` e `AccountReceivable` relacionadas

##### 2.2.2.5 **IntegraÃ§Ãµes com Outros DomÃ­nios**

###### **â† Produto (Customer-Supplier)**
**IntegraÃ§Ã£o**: Vendas **consome** catÃ¡logo de Produto
- **Dados Consumidos**: Lista de produtos ativos, preÃ§os, configuraÃ§Ãµes, regras de composiÃ§Ã£o
- **OperaÃ§Ãµes**: Busca de produtos, validaÃ§Ã£o de configuraÃ§Ãµes, cÃ¡lculo de preÃ§os finais
- **Protocolo**: Vendas consome via `IProductService` mÃ©todos como `GetActiveProducts()`, `ValidateConfiguration()`, `CalculateCompositePrice()`

###### **â†’ ProduÃ§Ã£o (Customer-Supplier)**
**IntegraÃ§Ã£o**: Vendas **gera** demandas para ProduÃ§Ã£o
- **Dados Fornecidos**: EspecificaÃ§Ãµes de produtos a fabricar, quantidades, datas limite, configuraÃ§Ãµes especÃ­ficas
- **OperaÃ§Ãµes**: CriaÃ§Ã£o de demandas, cancelamento de demandas, consulta de status de produÃ§Ã£o
- **Protocolo**: `IDemandService.CreateFromOrderItem()`, `CancelDemand()`, `GetProductionStatus()`

###### **â†’ Financeiro (Customer-Supplier)**
**IntegraÃ§Ã£o**: Vendas **gera** contas a receber para Financeiro
- **Dados Fornecidos**: Valor a receber, cliente, condiÃ§Ãµes de pagamento, datas de vencimento
- **OperaÃ§Ãµes**: CriaÃ§Ã£o de contas a receber, cancelamento por cancelamento de pedido
- **Protocolo**: `IAccountReceivableService.CreateFromOrder()`, `CancelAccountReceivable()`

###### **Eventos de DomÃ­nio:**
- `OrderConfirmed`: Dispara criaÃ§Ã£o de `Demand` e `AccountReceivable`
- `OrderCancelled`: Dispara cancelamento em domÃ­nios dependentes
- `OrderItemAdded`: Recalcula totais e valida disponibilidade
- `OrderDelivered`: Atualiza status e notifica produÃ§Ã£o e financeiro

---

#### 2.2.3 **DOMÃNIO DE PRODUÃ‡ÃƒO**

##### 2.2.3.1 **Responsabilidade Central**
O **DomÃ­nio de ProduÃ§Ã£o** Ã© o centro de execuÃ§Ã£o do GesN, responsÃ¡vel por traduzir os pedidos de venda confirmados em tarefas de produÃ§Ã£o tangÃ­veis e rastreÃ¡veis. Ele funciona como a "esteira de produÃ§Ã£o" do negÃ³cio, garantindo que os produtos, especialmente os personalizados (`Composite`), sejam montados corretamente e dentro do prazo estipulado.

Este domÃ­nio Ã© ativado principalmente pelo **DomÃ­nio de Vendas**. Quando um `OrderEntry` contendo itens que exigem fabricaÃ§Ã£o Ã© confirmado, uma ou mais `Demand` sÃ£o geradas automaticamente, iniciando o fluxo de trabalho da produÃ§Ã£o.

##### 2.2.3.2 **Principais Entidades**

###### **Entidades Centrais:**
- **`Demand`**: Entidade central representando uma ordem de produÃ§Ã£o. Ligada a um `OrderItem` especÃ­fico, agrega todas as informaÃ§Ãµes necessÃ¡rias para produÃ§Ã£o
- **`ProductComposition`**: Representa uma tarefa ou componente especÃ­fico dentro de uma `Demand`. Ã‰ a unidade de trabalho da produÃ§Ã£o
- **`ProductionOrder`**: Agrupamento de mÃºltiplas demandas para otimizaÃ§Ã£o de produÃ§Ã£o e recursos

###### **Entidades de ReferÃªncia:**
- **`Product`** *(DomÃ­nio de Produto)*: EspecificaÃ§Ã£o do item a ser produzido (AssemblyTime, AssemblyInstructions)
- **`ProductComponent`** *(DomÃ­nio de Produto)*: EspecificaÃ§Ã£o dos componentes a usar em uma tarefa
- **`Ingredient`** *(DomÃ­nio de Compras)*: MatÃ©rias-primas consumidas na produÃ§Ã£o

##### 2.2.3.3 **Fluxos de Trabalho e Jornada do UsuÃ¡rio**

###### **2.2.3.3.1 Painel de Demandas (Production Dashboard)**
**Dashboard Centralizado:**
```
- Cards de Resumo por Status (Pendente, Confirmado, Em ProduÃ§Ã£o, Finalizando, Entregue, Atrasado)
- Grade detalhada de todas as demandas
- InformaÃ§Ãµes crÃ­ticas: produto, quantidade, cliente, data de entrega, status atual
```

**Filtros AvanÃ§ados:**
```
- Por status da demanda
- Por produto especÃ­fico  
- Por perÃ­odo de entrega (data inicial e final)
- Apenas demandas atrasadas
- Por cliente/pedido de origem
```

###### **2.2.3.3.2 GeraÃ§Ã£o de Demandas**

**AutomÃ¡tica (Fluxo PadrÃ£o):**
```
1. OrderEntry confirmado no DomÃ­nio de Vendas
2. Sistema analisa cada OrderItem
3. Para ProductType.Composite ou itens que necessitam produÃ§Ã£o:
   â†’ Cria Demand automaticamente
   â†’ Transforma escolhas do cliente em ProductComposition
   â†’ Define data limite baseada em OrderEntry.DeliveryDate
```

**Manual (Casos Especiais):**
```
- ProduÃ§Ã£o para estoque (sem pedido de cliente)
- Ordens de produÃ§Ã£o internas
- CorreÃ§Ã£o de falhas no processo automÃ¡tico
- ProduÃ§Ã£o de amostras/protÃ³tipos
```

###### **2.2.3.3.3 Gerenciamento e ExecuÃ§Ã£o de Demandas**

**1. AnÃ¡lise e ConfirmaÃ§Ã£o:**
```
- Nova demanda: Status "Pendente"
- Gerente revisa especificaÃ§Ãµes e recursos necessÃ¡rios
- ValidaÃ§Ã£o de disponibilidade de ingredientes
- MovimentaÃ§Ã£o para "Confirmado" (pronta para iniciar)
```

**2. InÃ­cio da ProduÃ§Ã£o:**
```
- SeleÃ§Ã£o de demanda "Confirmada"
- VerificaÃ§Ã£o final de recursos e ingredientes
- AlteraÃ§Ã£o de status para "Em ProduÃ§Ã£o"
- InÃ­cio da execuÃ§Ã£o das tarefas ProductComposition
```

**3. ExecuÃ§Ã£o Granular das Tarefas:**
```
- Lista detalhada de ProductComposition por demanda
- Cada tarefa pode ser marcada individualmente:
  â†’ StartProcessing() - inÃ­cio da tarefa
  â†’ CompleteProcessing() - conclusÃ£o da tarefa
- Rastreamento granular do progresso
- Exemplo: "massa pronta", "recheio pronto", "cobertura pendente"
```

**4. FinalizaÃ§Ã£o e Entrega:**
```
- Todos ProductComposition "Completed" â†’ Demand "Finalizando"
- Etapas finais: embalagem, decoraÃ§Ã£o, acabamento
- Status "Pronto para Entrega" â†’ aguarda logÃ­stica
- Status "Entregue" â†’ ciclo de produÃ§Ã£o finalizado
```

##### 2.2.3.4 **Regras de NegÃ³cio do DomÃ­nio**

###### **MÃ¡quina de Estados da Demanda:**
1. **Pendente**: RecÃ©m-criada, aguardando revisÃ£o da produÃ§Ã£o
2. **Confirmado**: Revisada e apta para iniciar produÃ§Ã£o
3. **Em ProduÃ§Ã£o**: Trabalho na demanda foi iniciado
4. **Finalizando**: Todos componentes produzidos, em fase de montagem final/embalagem
5. **Pronto para Entrega**: ProduÃ§Ã£o concluÃ­da, aguardando logÃ­stica
6. **Entregue**: Ciclo de produÃ§Ã£o finalizado
7. **Cancelado**: Demanda cancelada (por cancelamento do pedido)
8. **Atrasado**: Estado de alerta quando data atual > data entrega e ainda nÃ£o estÃ¡ pronto

###### **Regras de TransiÃ§Ã£o de Status:**
- Demanda sÃ³ pode ir para "Em ProduÃ§Ã£o" se estiver "Confirmada"
- Status "Pronto para Entrega" sÃ³ se todos `ProductComposition` estiverem "Completed"
- `ProductComposition` requer `DemandId`, `ProductComponentId` e `HierarchyName`
- Data limite sempre baseada em `OrderEntry.DeliveryDate` menos tempo de montagem

###### **Regras de Capacidade e Recursos:**
- ValidaÃ§Ã£o de disponibilidade de ingredientes antes de confirmar demanda
- Controle de capacidade por tempo de montagem (`AssemblyTime`)
- PriorizaÃ§Ã£o automÃ¡tica por data de entrega (FIFO modificado)
- Alertas automÃ¡ticos para demandas em risco de atraso

##### 2.2.3.5 **IntegraÃ§Ãµes com Outros DomÃ­nios**

###### **â† Vendas (Customer-Supplier)**
**IntegraÃ§Ã£o**: ProduÃ§Ã£o **recebe** demandas de Vendas
- **Dados Recebidos**: EspecificaÃ§Ãµes de produtos, quantidades, configuraÃ§Ãµes escolhidas pelo cliente, data limite
- **OperaÃ§Ãµes**: CriaÃ§Ã£o automÃ¡tica de demandas, sincronizaÃ§Ã£o de status, cancelamentos
- **Protocolo**: Vendas chama `IDemandService.CreateFromOrderItem()`, `UpdateDemandStatus()`, `CancelDemand()`

###### **â† Produto (Customer-Supplier)**
**IntegraÃ§Ã£o**: ProduÃ§Ã£o **consulta** especificaÃ§Ãµes de Produto
- **Dados Consumidos**: Receitas (`ProductIngredient`), instruÃ§Ãµes de montagem, tempo de produÃ§Ã£o, componentes
- **OperaÃ§Ãµes**: Consulta de composiÃ§Ã£o para produtos compostos, validaÃ§Ã£o de componentes ativos
- **Protocolo**: `IProductService.GetProductComposition()`, `GetAssemblyInstructions()`, `GetProductIngredients()`

###### **â†’ Compras (Customer-Supplier)**
**IntegraÃ§Ã£o**: ProduÃ§Ã£o **informa** consumo para Compras
- **Dados Fornecidos**: Ingredientes consumidos, quantidades utilizadas, datas de consumo
- **OperaÃ§Ãµes**: Baixa automÃ¡tica de estoque, disparo de alertas de estoque mÃ­nimo
- **Protocolo**: `IIngredientStockService.ConsumeIngredients()`, `CheckMinimumLevels()`

###### **â†’ Vendas (Shared Kernel)**
**IntegraÃ§Ã£o**: ProduÃ§Ã£o **atualiza** status para Vendas
- **Dados Compartilhados**: Status de produÃ§Ã£o, previsÃ£o de conclusÃ£o, alertas de atraso
- **OperaÃ§Ãµes**: SincronizaÃ§Ã£o de status de pedidos, notificaÃ§Ãµes de conclusÃ£o
- **Protocolo**: Eventos de domÃ­nio `DemandStatusChanged`, `ProductionCompleted`, `ProductionDelayed`

###### **Eventos de DomÃ­nio:**
- `DemandCreated`: Nova demanda gerada a partir de pedido
- `DemandStarted`: ProduÃ§Ã£o iniciada, atualiza status do pedido
- `DemandCompleted`: ProduÃ§Ã£o finalizada, produto pronto para entrega
- `DemandDelayed`: Atraso detectado, alerta para vendas e cliente
- `IngredientConsumed`: Consumo de ingrediente, atualiza estoque

---

#### 2.2.4 **DOMÃNIO DE COMPRAS**

##### 2.2.4.1 **Responsabilidade Central**
O **DomÃ­nio de Compras** Ã© o pilar de sustentaÃ§Ã£o da cadeia de suprimentos do sistema GesN. Sua principal responsabilidade Ã© gerenciar a aquisiÃ§Ã£o de `Ingredient` (ingredientes e matÃ©rias-primas), garantindo que a produÃ§Ã£o tenha os insumos necessÃ¡rios para atender Ã s demandas de vendas, ao mesmo tempo que otimiza os custos e o capital de giro imobilizado em estoque.

Este domÃ­nio opera em estreita colaboraÃ§Ã£o com os domÃ­nios de **ProduÃ§Ã£o** e **Financeiro**, fechando o ciclo operacional do sistema e automatizando o processo de aquisiÃ§Ã£o desde a identificaÃ§Ã£o da necessidade atÃ© o recebimento e pagamento.

##### 2.2.4.2 **Principais Entidades**

###### **Entidades Centrais:**
- **`Ingredient`**: Representa matÃ©ria-prima ou insumo utilizado na produÃ§Ã£o. Possui unidade de medida padrÃ£o e nÃ­vel de estoque mÃ­nimo configurado
- **`Supplier`**: Representa empresa ou pessoa fornecedora dos ingredientes. Armazena informaÃ§Ãµes de contato, condiÃ§Ãµes comerciais e histÃ³rico
- **`PurchaseOrder`**: Documento central do domÃ­nio. Pedido de compra formalizado a um fornecedor com cabe Ã§alho e lista de itens
- **`PurchaseOrderItem`**: Item de linha dentro de uma ordem de compra. Especifica ingrediente, quantidade, unidade e custo unitÃ¡rio

###### **Entidades de Controle:**
- **`IngredientStock`**: Quantidade fÃ­sica de um ingrediente disponÃ­vel em estoque. Ponto central de integraÃ§Ã£o entre domÃ­nios
- **`SupplierIngredient`**: Relacionamento entre fornecedor e ingredientes que ele pode fornecer, incluindo preÃ§os preferenciais

##### 2.2.4.3 **Fluxos de Trabalho e Jornada do UsuÃ¡rio**

###### **2.2.4.3.1 GestÃ£o de Fornecedores e Ingredientes**

**Cadastro de Ingredientes:**
```
1. Registro de insumos com nome e unidade de medida padrÃ£o (KG, Litro, Unidade)
2. DefiniÃ§Ã£o do Estoque MÃ­nimo (chave para automaÃ§Ã£o)
3. ConfiguraÃ§Ã£o de cÃ³digos internos e descriÃ§Ãµes
4. Estabelecimento de fornecedores preferenciais
```

**Cadastro de Fornecedores:**
```
1. Dados bÃ¡sicos: razÃ£o social, CNPJ, contatos
2. CondiÃ§Ãµes comerciais: prazo de entrega, condiÃ§Ãµes de pagamento
3. AssociaÃ§Ã£o com ingredientes que costuma fornecer
4. HistÃ³rico de desempenho e avaliaÃ§Ãµes
```

###### **2.2.4.3.2 GeraÃ§Ã£o de Ordens de Compra**

**CriaÃ§Ã£o Manual:**
```
1. SeleÃ§Ã£o de fornecedor
2. AdiÃ§Ã£o manual de PurchaseOrderItem:
   â†’ Escolha de ingrediente
   â†’ DefiniÃ§Ã£o de quantidade
   â†’ NegociaÃ§Ã£o de preÃ§o
3. Salvamento com status "Rascunho"
```

**GeraÃ§Ã£o Sugerida (Fluxo Inteligente):**
```
1. Sistema varre IngredientStock periodicamente
2. Compara quantidade atual com Ingredient.MinimumStockLevel
3. Para ingredientes abaixo do mÃ­nimo:
   â†’ Calcula quantidade necessÃ¡ria para nÃ­vel seguro
   â†’ Identifica fornecedor preferencial
   â†’ Gera sugestÃ£o de compra
4. UsuÃ¡rio revisa, ajusta e converte em PurchaseOrder
5. Agrupamento automÃ¡tico por fornecedor
```

###### **2.2.4.3.3 Ciclo de Vida da Ordem de Compra**

**1. Rascunho (Draft):**
```
- Ordem pode ser livremente editada
- AdiÃ§Ã£o/remoÃ§Ã£o de itens permitida
- AlteraÃ§Ã£o de quantidades e preÃ§os
- Cancelamento sem impactos
```

**2. Enviado (Sent):**
```
- Ordem finalizada e enviada ao fornecedor
- EdiÃ§Ãµes bloqueadas para manter integridade
- Aguardando confirmaÃ§Ã£o e entrega
- Rastreamento de prazos iniciado
```

**3. Recebimento dos Produtos:**
```
- Chegada da entrega fÃ­sica
- Processo de conferÃªncia item a item:
  â†’ VerificaÃ§Ã£o de quantidade recebida vs pedida
  â†’ Controle de qualidade dos ingredientes
  â†’ Registro de quantidades efetivamente recebidas
```

**4. Recebimento Parcial vs Total:**
```
- Parcial: quantidade recebida < quantidade pedida
  â†’ Item marcado como "Recebido Parcialmente"
  â†’ Ordem continua aguardando saldo
- Total: quantidade recebida = quantidade pedida
  â†’ Item marcado como "Recebido"
  â†’ AtualizaÃ§Ã£o automÃ¡tica do IngredientStock
```

**5. ConclusÃ£o e LanÃ§amento Financeiro:**
```
- Todos itens recebidos â†’ Status "Recebido Totalmente"
- GeraÃ§Ã£o automÃ¡tica de AccountPayable no Financeiro
- VinculaÃ§Ã£o com fornecedor para controle de pagamento
```

##### 2.2.4.4 **Regras de NegÃ³cio do DomÃ­nio**

###### **Estados da Ordem de Compra:**
1. **Rascunho (Draft)**: Pode ser editada livremente
2. **Enviado (Sent)**: Enviada ao fornecedor, ediÃ§Ãµes bloqueadas
3. **Recebido Parcialmente**: Alguns itens recebidos, aguardando saldo
4. **Recebido Totalmente (Closed)**: Todos itens recebidos, ordem finalizada
5. **Cancelado**: Ordem cancelada por qualquer motivo

###### **Regras de ValidaÃ§Ã£o:**
- `PurchaseOrder` nÃ£o pode ser enviada sem `Supplier` e pelo menos um `PurchaseOrderItem`
- Quantidade recebida nÃ£o pode exceder quantidade pedida
- ExclusÃ£o sÃ³ permitida no status "Rascunho"
- ApÃ³s envio: fluxo correto Ã© cancelamento, nÃ£o exclusÃ£o
- `Ingredient` nÃ£o pode ser excluÃ­do se tiver estoque ou ordens ativas

###### **LÃ³gica de Estoque:**
**Entrada de Estoque:**
```
IngredientStock.Quantity += ReceivedQuantity (no recebimento da compra)
```

**SaÃ­da de Estoque:**
```
IngredientStock.Quantity -= ConsumedQuantity (na conclusÃ£o da produÃ§Ã£o)
ConsumedQuantity = Î£(ProductIngredient.Quantity) dos produtos fabricados
```

**Alertas AutomÃ¡ticos:**
```
- Estoque abaixo do mÃ­nimo â†’ SugestÃ£o de compra
- Ingredientes prÃ³ximos ao vencimento â†’ Alerta de uso prioritÃ¡rio
- Fornecedores com atraso recorrente â†’ AvaliaÃ§Ã£o de desempenho
```

##### 2.2.4.5 **IntegraÃ§Ãµes com Outros DomÃ­nios**

###### **â† ProduÃ§Ã£o (Customer-Supplier)**
**IntegraÃ§Ã£o**: Compras **recebe** demandas de consumo de ProduÃ§Ã£o
- **Dados Recebidos**: Ingredientes consumidos, quantidades utilizadas, datas de consumo
- **OperaÃ§Ãµes**: Baixa automÃ¡tica de estoque, cÃ¡lculo de necessidades futuras, alertas de estoque mÃ­nimo
- **Protocolo**: ProduÃ§Ã£o chama `IIngredientStockService.ConsumeIngredients()`, `GetStockLevels()`, `CheckAvailability()`

###### **â† Produto (Customer-Supplier)**
**IntegraÃ§Ã£o**: Compras **consulta** receitas de Produto
- **Dados Consumidos**: Receitas (`ProductIngredient`), projeÃ§Ãµes de demanda baseadas em vendas
- **OperaÃ§Ãµes**: CÃ¡lculo de necessidades futuras, planejamento de compras por sazonalidade
- **Protocolo**: `IProductService.GetProductIngredients()`, `CalculateIngredientDemand()`

###### **â†’ Financeiro (Customer-Supplier)**
**IntegraÃ§Ã£o**: Compras **gera** contas a pagar para Financeiro
- **Dados Fornecidos**: Valor a pagar, fornecedor, condiÃ§Ãµes de pagamento, datas de vencimento
- **OperaÃ§Ãµes**: CriaÃ§Ã£o automÃ¡tica de `AccountPayable`, cancelamento por cancelamento de ordem
- **Protocolo**: `IAccountPayableService.CreateFromPurchaseOrder()`, `CancelAccountPayable()`

###### **â†’ ProduÃ§Ã£o (Shared Kernel)**
**IntegraÃ§Ã£o**: Compras **informa** disponibilidade para ProduÃ§Ã£o
- **Dados Compartilhados**: NÃ­veis de estoque atualizados, previsÃ£o de recebimentos, alertas de indisponibilidade
- **OperaÃ§Ãµes**: ValidaÃ§Ã£o de viabilidade de demandas, bloqueio de produÃ§Ã£o por falta de insumos
- **Protocolo**: Eventos de domÃ­nio `StockUpdated`, `LowStockAlert`, `IngredientReceived`

###### **Eventos de DomÃ­nio:**
- `PurchaseOrderSent`: Ordem enviada ao fornecedor
- `IngredientReceived`: Ingrediente recebido, estoque atualizado
- `StockLevelLow`: Estoque abaixo do mÃ­nimo, necessÃ¡rio reposiÃ§Ã£o
- `SupplierDelayed`: Fornecedor com atraso, alerta para produÃ§Ã£o
- `PurchaseOrderCompleted`: Ordem totalmente recebida, gera conta a pagar

---

#### 2.2.5 **DOMÃNIO FINANCEIRO**

##### 2.2.5.1 **Responsabilidade Central**
O **DomÃ­nio Financeiro** Ã© o centro nervoso do sistema GesN, responsÃ¡vel por rastrear, gerenciar e relatar todo o fluxo de dinheiro que entra e sai da empresa. Ele consolida as atividades operacionais dos domÃ­nios de **Vendas** e **Compras**, traduzindo-as em registros financeiros claros e acionÃ¡veis, como contas a receber e a pagar.

Este domÃ­nio Ã© fundamental para a tomada de decisÃµes estratÃ©gicas, pois oferece uma visÃ£o precisa da saÃºde financeira do negÃ³cio, do fluxo de caixa e da lucratividade, automatizando a criaÃ§Ã£o de lanÃ§amentos e facilitando a conciliaÃ§Ã£o de pagamentos.

##### 2.2.5.2 **Principais Entidades**

###### **Entidades Centrais:**
- **`AccountReceivable`**: Representa valor que a empresa tem direito de receber de um `Customer`. Gerada a partir de `OrderEntry` com informaÃ§Ãµes de valor, vencimento e status
- **`AccountPayable`**: Representa obrigaÃ§Ã£o financeira que a empresa tem com um `Supplier`. Gerada a partir de `PurchaseOrder` com valor, vencimento e status
- **`Transaction`**: Entidade mais granular. Representa qualquer movimento de dinheiro (entrada/crÃ©dito ou saÃ­da/dÃ©bito)
- **`CashFlow`**: VisÃ£o consolidada gerada a partir das `Transaction`. Apresenta entradas, saÃ­das e saldo em determinado perÃ­odo

###### **Entidades de Controle:**
- **`PaymentMethod`**: Formas de pagamento aceitas (dinheiro, cartÃ£o, PIX, boleto)
- **`FinancialCategory`**: CategorizaÃ§Ã£o de receitas e despesas para relatÃ³rios gerenciais
- **`BankAccount`**: Contas bancÃ¡rias da empresa para controle de saldos

##### 2.2.5.3 **Fluxos de Trabalho e Jornada do UsuÃ¡rio**

###### **2.2.5.3.1 GestÃ£o de Contas a Receber**
**Fluxo de Entrada de Dinheiro:**

**1. GeraÃ§Ã£o AutomÃ¡tica:**
```
- OrderEntry confirmado no Vendas â†’ Cria AccountReceivable automaticamente
- CondiÃ§Ãµes de pagamento parceladas â†’ MÃºltiplos AccountReceivable com vencimentos diferentes
- Cada parcela com valor e data de vencimento especÃ­ficos
```

**2. Painel de Contas a Receber:**
```
- Lista de todas as contas a receber
- Filtros: cliente, perÃ­odo de vencimento, status (Pendente, Pago, Vencido)
- Indicadores visuais: prÃ³ximas ao vencimento, jÃ¡ vencidas
- Totalizadores: a receber hoje, esta semana, este mÃªs
```

**3. Registro de Recebimento:**
```
1. Cliente efetua pagamento
2. UsuÃ¡rio localiza AccountReceivable correspondente
3. Clica "Registrar Recebimento":
   â†’ Informa valor recebido e data
   â†’ Seleciona mÃ©todo de pagamento
   â†’ Sistema cria Transaction tipo "CrÃ©dito"
4. Status atualizado:
   â†’ Valor < Total: "Parcialmente Pago"
   â†’ Valor = Total: "Pago"
```

###### **2.2.5.3.2 GestÃ£o de Contas a Pagar**
**Fluxo de SaÃ­da de Dinheiro:**

**1. GeraÃ§Ã£o AutomÃ¡tica:**
```
- PurchaseOrder marcada como "Recebida Totalmente" â†’ Cria AccountPayable automaticamente
- Valor total da nota vinculado ao fornecedor
- Data de vencimento baseada em condiÃ§Ãµes comerciais
```

**2. Painel de Contas a Pagar:**
```
- Lista de todas as contas a pagar
- Filtros: fornecedor, perÃ­odo de vencimento, status (Pendente, Paga, Vencida)
- Alertas para contas com vencimento prÃ³ximo
- Planejamento de pagamentos por disponibilidade de caixa
```

**3. Registro de Pagamento:**
```
1. Empresa decide pagar fornecedor
2. UsuÃ¡rio localiza AccountPayable correspondente
3. Clica "Registrar Pagamento":
   â†’ Informa valor pago e data
   â†’ Seleciona conta bancÃ¡ria/mÃ©todo
   â†’ Sistema cria Transaction tipo "DÃ©bito"
4. Status atualizado para "Paga"
```

###### **2.2.5.3.3 AnÃ¡lise do Fluxo de Caixa**

**RelatÃ³rio DinÃ¢mico:**
```
1. UsuÃ¡rio seleciona perÃ­odo (mÃªs atual, Ãºltimos 30 dias, personalizado)
2. Sistema busca todas Transaction no perÃ­odo
3. Agrupa por dia/semana/mÃªs
4. Apresenta:
   â†’ Saldo Inicial do perÃ­odo
   â†’ Total de Entradas (Î£ transaÃ§Ãµes crÃ©dito)
   â†’ Total de SaÃ­das (Î£ transaÃ§Ãµes dÃ©bito)
   â†’ Saldo Operacional (Entradas - SaÃ­das)
   â†’ Saldo Final (Inicial + Operacional)
```

**ProjeÃ§Ãµes e AnÃ¡lises:**
```
- Contas a receber futuras (previsÃ£o de entradas)
- Contas a pagar futuras (previsÃ£o de saÃ­das)
- Saldo projetado por perÃ­odo
- IdentificaÃ§Ã£o de perÃ­odos crÃ­ticos de caixa
```

##### 2.2.5.4 **Regras de NegÃ³cio do DomÃ­nio**

###### **Estados das Contas a Receber:**
1. **Pendente**: Aguardando pagamento do cliente
2. **Parcialmente Pago**: Pagamentos parciais recebidos
3. **Pago**: Valor total recebido e quitado
4. **Vencido**: Data de vencimento passou sem pagamento
5. **Cancelado**: Conta cancelada (por cancelamento do pedido)

###### **Estados das Contas a Pagar:**
1. **Pendente**: Aguardando pagamento ao fornecedor
2. **Paga**: Valor total pago ao fornecedor
3. **Vencida**: Data de vencimento passou sem pagamento
4. **Cancelada**: ObrigaÃ§Ã£o cancelada

###### **Regras de ValidaÃ§Ã£o:**
- `Transaction` deve estar obrigatoriamente associada a `AccountReceivable` OU `AccountPayable`
- Soma dos valores das `Transaction` nÃ£o pode exceder valor total da conta
- NÃ£o permitir novos pagamentos para contas com status "Pago" ou "Cancelado"
- Status "Vencido" aplicado automaticamente pelo sistema baseado na data atual

###### **Regras de CÃ¡lculo:**
- **Receita Bruta**: Soma de todos os `AccountReceivable` do perÃ­odo
- **Custo Direto**: Soma de todos os `AccountPayable` relacionados a ingredientes/produÃ§Ã£o
- **Margem Bruta**: Receita Bruta - Custo Direto
- **Fluxo de Caixa LÃ­quido**: Î£(TransaÃ§Ãµes CrÃ©dito) - Î£(TransaÃ§Ãµes DÃ©bito)

##### 2.2.5.5 **IntegraÃ§Ãµes com Outros DomÃ­nios**

###### **â† Vendas (Customer-Supplier)**
**IntegraÃ§Ã£o**: Financeiro **recebe** contas a receber de Vendas
- **Dados Recebidos**: Valor a receber, cliente, condiÃ§Ãµes de pagamento, datas de vencimento
- **OperaÃ§Ãµes**: CriaÃ§Ã£o automÃ¡tica de `AccountReceivable`, parcelamento, cancelamento
- **Protocolo**: Vendas chama `IAccountReceivableService.CreateFromOrder()`, `CreateInstallments()`, `CancelAccountReceivable()`

###### **â† Compras (Customer-Supplier)**  
**IntegraÃ§Ã£o**: Financeiro **recebe** contas a pagar de Compras
- **Dados Recebidos**: Valor a pagar, fornecedor, condiÃ§Ãµes comerciais, datas de vencimento
- **OperaÃ§Ãµes**: CriaÃ§Ã£o automÃ¡tica de `AccountPayable`, agendamento de pagamentos
- **Protocolo**: Compras chama `IAccountPayableService.CreateFromPurchaseOrder()`, `SchedulePayment()`, `CancelAccountPayable()`

###### **â†’ Produto (Conformist)**
**IntegraÃ§Ã£o**: Financeiro **fornece** dados de custo para Produto
- **Dados Fornecidos**: Custos reais de ingredientes, margens de lucratividade por produto, anÃ¡lise de rentabilidade
- **OperaÃ§Ãµes**: AnÃ¡lise de lucratividade, sugestÃµes de ajuste de preÃ§os, relatÃ³rios de performance
- **Protocolo**: `IFinancialAnalysisService.GetProductProfitability()`, `CalculateRealCosts()`, `GetMarginAnalysis()`

###### **â†’ Todos os DomÃ­nios (Shared Kernel)**
**IntegraÃ§Ã£o**: Financeiro **consolida** dados de todos os domÃ­nios
- **Dados Compartilhados**: Indicadores financeiros, alertas de fluxo de caixa, relatÃ³rios consolidados
- **OperaÃ§Ãµes**: Dashboard executivo, relatÃ³rios gerenciais, alertas de performance
- **Protocolo**: Eventos de domÃ­nio `CashFlowAlert`, `ProfitabilityChanged`, `PaymentOverdue`

###### **Eventos de DomÃ­nio:**
- `AccountReceivableCreated`: Nova conta a receber gerada
- `PaymentReceived`: Pagamento de cliente recebido  
- `PaymentMade`: Pagamento a fornecedor efetuado
- `AccountOverdue`: Conta vencida, necessÃ¡ria cobranÃ§a
- `CashFlowAlert`: Alerta de fluxo de caixa baixo
- `ProfitabilityCalculated`: Lucratividade recalculada por produto

---

### 2.3 Ubiquitous Language (GlossÃ¡rio de Termos)

| Termo | DefiniÃ§Ã£o | DomÃ­nio Principal |
|-------|-----------|-------------------|
| **Produto** | Item comercializÃ¡vel ou fabricÃ¡vel no catÃ¡logo da empresa | Produto |
| **Produto Simples** | Item bÃ¡sico sem configuraÃ§Ãµes ou variaÃ§Ãµes | Produto |
| **Produto Composto** | Item personalizÃ¡vel com hierarquias de componentes | Produto |
| **Grupo de Produtos** | Kit/combo de mÃºltiplos itens com regras de troca | Produto |
| **Hierarquia de Componentes** | Camada de personalizaÃ§Ã£o (ex: "Massa", "Recheio") | Produto |
| **Componente** | OpÃ§Ã£o especÃ­fica dentro de uma hierarquia | Produto |
| **Pedido de Venda** | SolicitaÃ§Ã£o comercial feita por um cliente | Vendas |
| **Item do Pedido** | Linha individual dentro de um pedido | Vendas |
| **Demanda de ProduÃ§Ã£o** | Ordem interna para fabricar produtos | ProduÃ§Ã£o |
| **ComposiÃ§Ã£o do Produto** | Tarefa especÃ­fica de montagem de um componente | ProduÃ§Ã£o |
| **Ordem de Compra** | RequisiÃ§Ã£o de aquisiÃ§Ã£o de insumos/matÃ©ria-prima | Compras |
| **Fornecedor** | Empresa/pessoa que vende insumos | Compras |
| **Ingrediente** | MatÃ©ria-prima ou insumo usado na produÃ§Ã£o | Compras |
| **Conta a Receber** | Valor que a empresa tem direito de receber | Financeiro |
| **Conta a Pagar** | ObrigaÃ§Ã£o financeira com fornecedores | Financeiro |
| **TransaÃ§Ã£o** | Movimento de entrada ou saÃ­da de dinheiro | Financeiro |

---

## 3. Diagramas

> **ðŸ“ LocalizaÃ§Ã£o**: Todos os diagramas detalhados estÃ£o disponÃ­veis no diretÃ³rio [`GesN.Web/Context/DIAGRAMS/`](./DIAGRAMS/) organizados por tipo e domÃ­nio.

### 3.1 Context Map Geral

```mermaid
graph TB
    subgraph "GesN - Sistema Integrado de GestÃ£o"
        A[ðŸ“¦ PRODUTO<br/>Bounded Context]
        B[ðŸ’° VENDAS<br/>Bounded Context]  
        C[ðŸ­ PRODUÃ‡ÃƒO<br/>Bounded Context]
        D[ðŸ›’ COMPRAS<br/>Bounded Context]
        E[ðŸ’³ FINANCEIRO<br/>Bounded Context]
    end
    
    A -->|Customer-Supplier<br/>CatÃ¡logo de Produtos| B
    A -->|Customer-Supplier<br/>Receitas e EspecificaÃ§Ãµes| C
    A -->|Conformist<br/>Custos de Produtos| E
    
    B -->|Customer-Supplier<br/>Demandas de ProduÃ§Ã£o| C
    B -->|Customer-Supplier<br/>Contas a Receber| E
    
    C -->|Customer-Supplier<br/>Consumo de Ingredientes| D
    
    D -->|Customer-Supplier<br/>Contas a Pagar| E
    
    E -.->|Shared Kernel<br/>Dados Financeiros| A
    C -.->|Shared Kernel<br/>Dados de ProduÃ§Ã£o| A
    
    style A fill:#00a86b
    style B fill:#f36b21  
    style C fill:#fba81d
    style D fill:#0562aa
    style E fill:#083e61
```

### 3.2 Entity-Relationship Diagrams (ERDs)

#### **ðŸ“Š Diagramas ERD Detalhados por DomÃ­nio**

| DomÃ­nio | Arquivo | DescriÃ§Ã£o |
|---------|---------|-----------|
| ðŸ“¦ **Produto** | [`01-product-domain-erd.md`](./DIAGRAMS/ERD/01-product-domain-erd.md) | ERD completo com heranÃ§a TPH, componentes e grupos |
| ðŸ’° **Vendas** | [`02-sales-domain-erd.md`](./DIAGRAMS/ERD/02-sales-domain-erd.md) | Customer â†’ OrderEntry â†’ OrderItem + integraÃ§Ãµes |
| ðŸ­ **ProduÃ§Ã£o** | [`03-production-domain-erd.md`](./DIAGRAMS/ERD/03-production-domain-erd.md) | Demand, ProductComposition e ProductionOrder |
| ðŸ›’ **Compras** | [`04-purchasing-domain-erd.md`](./DIAGRAMS/ERD/04-purchasing-domain-erd.md) | PurchaseOrder, Supplier, Ingredient com IA |
| ðŸ’³ **Financeiro** | [`05-financial-domain-erd.md`](./DIAGRAMS/ERD/05-financial-domain-erd.md) | AccountReceivable/Payable + Transactions |

### 3.3 Diagramas de Classes

#### **ðŸ—ï¸ PadrÃ£o Table Per Hierarchy (TPH)**

| Tipo | Arquivo | DescriÃ§Ã£o |
|------|---------|-----------|
| ðŸ§¬ **Product TPH** | [`product-tph-inheritance.md`](./DIAGRAMS/CLASS-DIAGRAMS/product-tph-inheritance.md) | HeranÃ§a Product â†’ Simple/Composite/Group + Interfaces |

### 3.4 Fluxogramas de Processo

#### **ðŸ”„ Processos CrÃ­ticos por DomÃ­nio**

| DomÃ­nio | Arquivo | Processo Principal |
|---------|---------|-------------------|
| ðŸ’° **Vendas** | [`02-sales-order-flow.md`](./DIAGRAMS/PROCESS-FLOWS/02-sales-order-flow.md) | **CriaÃ§Ã£o de Pedidos**: Simple vs Composite vs Group + Ciclo de vida |
| ðŸ­ **ProduÃ§Ã£o** | [`03-production-demand-flow.md`](./DIAGRAMS/PROCESS-FLOWS/03-production-demand-flow.md) | **GeraÃ§Ã£o AutomÃ¡tica de Demands**: 1 OrderItem â†’ 1:N Demands |
| ðŸ›’ **Compras** | [`04-purchasing-flow.md`](./DIAGRAMS/PROCESS-FLOWS/04-purchasing-flow.md) | **CriaÃ§Ã£o Manual com IA**: Upload nota fiscal â†’ Processamento â†’ ValidaÃ§Ã£o |
| ðŸ’³ **Financeiro** | [`05-financial-flow.md`](./DIAGRAMS/PROCESS-FLOWS/05-financial-flow.md) | **Contas a Receber vs Pagar**: GeraÃ§Ã£o automÃ¡tica + AnÃ¡lise lucratividade |

### 3.5 Diagramas de Estado (Ciclos de Vida)

#### **ðŸ“ˆ Estados e TransiÃ§Ãµes de Entidades CrÃ­ticas**

| Entidade | Arquivo | Estados Principais |
|----------|---------|-------------------|
| ðŸ“‹ **OrderEntry** | [`order-lifecycle.md`](./DIAGRAMS/STATE-DIAGRAMS/order-lifecycle.md) | Pending â†’ Confirmed â†’ InProduction â†’ Delivered â†’ Invoiced |
| ðŸ­ **Demand** | [`demand-lifecycle.md`](./DIAGRAMS/STATE-DIAGRAMS/demand-lifecycle.md) | Pending â†’ Confirmed â†’ InProduction â†’ Ready â†’ Delivered |
| ðŸ›’ **PurchaseOrder** | [`purchase-order-lifecycle.md`](./DIAGRAMS/STATE-DIAGRAMS/purchase-order-lifecycle.md) | Draft â†’ Sent â†’ PartiallyReceived â†’ FullyReceived |
| ðŸ’³ **Accounts** | [`account-lifecycle.md`](./DIAGRAMS/STATE-DIAGRAMS/account-lifecycle.md) | Pending â†’ PartiallyPaid â†’ Paid (+ Overdue) |

### 3.6 ConvenÃ§Ãµes Visuais

#### **ðŸŽ¨ Cores por DomÃ­nio**
- **ðŸ“¦ Produto**: `#00a86b` (Verde)
- **ðŸ’° Vendas**: `#f36b21` (Laranja)  
- **ðŸ­ ProduÃ§Ã£o**: `#fba81d` (Dourado)
- **ðŸ›’ Compras**: `#0562aa` (Azul)
- **ðŸ’³ Financeiro**: `#083e61` (Azul Escuro)

#### **ðŸ“Š Tipos de Relacionamentos**
- **1:1**: Linha simples `|â€”|`
- **1:N**: Linha simples `|â€”<`  
- **N:N**: Linha simples `>â€”<`
- **Opcional**: Linha tracejada `|-..-|`
- **HeranÃ§a**: Linha com triÃ¢ngulo `|â€”â–²`

#### **âš¡ TransiÃ§Ãµes de Estado**
- **ðŸ¤– AutomÃ¡tica**: Trigger do sistema
- **ðŸ‘¤ Manual**: AÃ§Ã£o do usuÃ¡rio
- **âš ï¸ Condicional**: Baseada em regras
- **ðŸš¨ ExceÃ§Ã£o**: Cancelamento ou erro

### 3.7 IntegraÃ§Ã£o entre DomÃ­nios

#### **ðŸ”— Fluxos AutomÃ¡ticos CrÃ­ticos**

##### **ðŸ’° Vendas â†’ ðŸ­ ProduÃ§Ã£o**
```mermaid
sequenceDiagram
    participant OI as OrderItem
    participant D as Demand
    participant PC as ProductComposition
    
    OI->>D: CriaÃ§Ã£o automÃ¡tica (1:N)
    Note over D: Status: Pending
    
    OI->>PC: Se ProductType.Composite
    Note over PC: Status: Pending
    
    D->>D: OrderEntry.Confirmed
    Note over D: Status: Confirmed
```

##### **ðŸ­ ProduÃ§Ã£o â†’ ðŸ›’ Compras**
```mermaid
sequenceDiagram
    participant PC as ProductComposition
    participant IS as IngredientStock
    participant PO as PurchaseOrder
    
    PC->>IS: Consumir ingredientes
    IS->>IS: Verificar estoque mÃ­nimo
    IS->>PO: Sugerir compra (se necessÃ¡rio)
    Note over PO: CriaÃ§Ã£o automÃ¡tica ou manual
```

##### **ðŸ›’ Compras â†’ ðŸ’³ Financeiro**
```mermaid
sequenceDiagram
    participant PO as PurchaseOrder
    participant AP as AccountPayable
    participant T as Transaction
    
    PO->>AP: FullyReceived â†’ Criar conta
    Note over AP: Status: Pending
    
    AP->>T: Pagamento registrado
    Note over AP: Status: Paid
```

##### **ðŸ’° Vendas â†’ ðŸ’³ Financeiro**
```mermaid
sequenceDiagram
    participant OE as OrderEntry
    participant AR as AccountReceivable
    participant T as Transaction
    
    OE->>AR: Confirmed â†’ Criar conta
    Note over AR: Status: Pending
    
    AR->>T: Recebimento registrado
    Note over AR: Status: Paid
```

### 3.8 MÃ©tricas e Monitoramento

#### **ðŸ“Š KPIs por DomÃ­nio**

| DomÃ­nio | MÃ©tricas Principais | Alertas CrÃ­ticos |
|---------|-------------------|------------------|
| **ðŸ“¦ Produto** | Produtos ativos, ConfiguraÃ§Ãµes vÃ¡lidas | Produto inativo em pedido |
| **ðŸ’° Vendas** | Volume vendas, Ticket mÃ©dio, ConversÃ£o | Pedido sem produÃ§Ã£o > 2h |
| **ðŸ­ ProduÃ§Ã£o** | Tempo ciclo, EficiÃªncia, Qualidade | Demand atrasada, Estoque crÃ­tico |
| **ðŸ›’ Compras** | Lead time, Qualidade fornecedor, IA accuracy | Fornecedor atrasado, IA erro |
| **ðŸ’³ Financeiro** | DSO, DPO, Fluxo caixa, InadimplÃªncia | Conta vencida, Fluxo negativo |

---

## 4. Regras e Diretrizes

### 4.1 ManutenÃ§Ã£o do Documento

#### **Responsabilidades por Papel:**
- **Arquiteto de Software**: Atualizar Context Map e relacionamentos entre domÃ­nios
- **Tech Lead de cada DomÃ­nio**: Manter atualizada a seÃ§Ã£o especÃ­fica do seu domÃ­nio
- **Product Owner**: Validar e aprovar mudanÃ§as na linguagem ubÃ­qua
- **Analista de NegÃ³cio**: Garantir alinhamento entre regras de negÃ³cio e documentaÃ§Ã£o

#### **FrequÃªncia de RevisÃ£o:**
- **Mensal**: RevisÃ£o geral da estrutura e relacionamentos
- **Por Sprint**: AtualizaÃ§Ã£o de entidades e regras modificadas
- **Por Release**: ValidaÃ§Ã£o completa com stakeholders

### 4.2 PadrÃ£o de Nomenclatura

#### **Entidades:**
- **PascalCase** para classes (ex: `ProductComponent`)
- **camelCase** para propriedades (ex: `additionalCost`)
- **Prefixos por DomÃ­nio**: Evitar quando possÃ­vel, usar namespaces

#### **Bounded Contexts:**
- **Nome do DomÃ­nio** em portuguÃªs para documentaÃ§Ã£o (ex: "DomÃ­nio de Produto")
- **Namespace** em inglÃªs no cÃ³digo (ex: `GesN.Product`)

#### **Relacionamentos:**
- **Customer-Supplier**: Quando um domÃ­nio consome serviÃ§os de outro
- **Conformist**: Quando um domÃ­nio se adapta ao modelo de outro
- **Shared Kernel**: Para dados compartilhados entre domÃ­nios

### 4.3 CritÃ©rios para EvoluÃ§Ã£o dos DomÃ­nios

#### **CriaÃ§Ã£o de Novo DomÃ­nio:**
- **Complexidade**: Mais de 10 entidades inter-relacionadas
- **Equipe Dedicada**: Justifica equipe de desenvolvimento especÃ­fica  
- **Ciclo de Vida Independente**: Pode evoluir sem impactar outros domÃ­nios
- **Linguagem EspecÃ­fica**: Possui vocabulÃ¡rio prÃ³prio significativo

#### **ModificaÃ§Ã£o de DomÃ­nio Existente:**
- **AnÃ¡lise de Impacto**: Avaliar efeitos nos domÃ­nios dependentes
- **Versionamento**: Considerar compatibilidade com integraÃ§Ãµes existentes
- **MigraÃ§Ã£o**: Planejar perÃ­odo de convivÃªncia entre versÃµes

#### **IntegraÃ§Ã£o entre DomÃ­nios:**
- **Evitar Acoplamento Forte**: Preferir eventos a chamadas diretas
- **Definir Contratos Claros**: APIs bem documentadas entre domÃ­nios
- **Monitorar Performance**: Medir latÃªncia de integraÃ§Ãµes crÃ­ticas

---

## 5. PrÃ³ximos Passos

### 5.1 DocumentaÃ§Ã£o Pendente
- [x] **Detalhamento do DomÃ­nio de Vendas** (processo de pedidos, integraÃ§Ãµes) âœ… **CONCLUÃDO**
- [x] **Detalhamento do DomÃ­nio de ProduÃ§Ã£o** (gestÃ£o de demandas, status) âœ… **CONCLUÃDO**
- [x] **Detalhamento do DomÃ­nio de Compras** (fornecedores, estoque) âœ… **CONCLUÃDO**
- [x] **Detalhamento do DomÃ­nio Financeiro** (contas, fluxo de caixa) âœ… **CONCLUÃDO**

### 5.2 Diagramas Adicionais
- [x] **ERDs Detalhados** para todos os 5 domÃ­nios âœ… **CONCLUÃDO**
- [x] **Diagramas de Classes** com heranÃ§a TPH âœ… **CONCLUÃDO**
- [x] **Fluxogramas de Processo** para fluxos crÃ­ticos âœ… **CONCLUÃDO**
- [x] **Diagramas de Estado** para ciclos de vida âœ… **CONCLUÃDO**
- [ ] **C4 Model** para cada domÃ­nio (containers e componentes)
- [ ] **Event Storming** para capturar eventos de domÃ­nio

### 5.3 ImplementaÃ§Ã£o
- [ ] **ValidaÃ§Ã£o com Stakeholders** do mapeamento atual
- [ ] **Refactoring de CÃ³digo** para alinhar com bounded contexts definidos
- [ ] **MÃ©tricas de DomÃ­nio** para monitorar integridade dos contextos

---

**Documento criado em**: 16/06/2025  
**VersÃ£o**: 3.0 (DocumentaÃ§Ã£o Completa + Diagramas Detalhados)  
**PrÃ³xima revisÃ£o**: Julho 2025  
**ResponsÃ¡vel**: Igor Spalenza Chaves  
**Status**: âœ… **DOCUMENTAÃ‡ÃƒO COMPLETA** - Todos os 5 domÃ­nios + Diagramas especializados  

### ðŸ“Š **Diagramas IncluÃ­dos nesta VersÃ£o:**
- **5 ERDs Detalhados** (todas propriedades + tipos + relacionamentos)
- **1 Diagrama de Classes TPH** (heranÃ§a Product)  
- **4 Fluxogramas de Processo** (fluxos crÃ­ticos por domÃ­nio)
- **4 Diagramas de Estado** (ciclos de vida principais)
- **Total**: **14 diagramas especializados** organizados em [`/DIAGRAMS/`](./DIAGRAMS/)
