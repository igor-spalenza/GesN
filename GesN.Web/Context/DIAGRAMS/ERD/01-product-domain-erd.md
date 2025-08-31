# ðŸ“¦ ERD - DOMÃNIO DE PRODUTO

## ðŸŽ¯ VisÃ£o Geral
Diagrama Entity-Relationship completo do DomÃ­nio de Produto, mostrando todas as entidades, propriedades, tipos de dados e relacionamentos. Este domÃ­nio gerencia o catÃ¡logo de produtos/serviÃ§os da empresa atravÃ©s de 3 tipos principais: Simple, Composite e Group.

## ðŸ—„ï¸ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === PRODUTO (CLASSE BASE ABSTRATA) ===
    PRODUCT {
        string Id PK "GUID Ãºnico"
        string ProductType "Simple|Composite|Group"
        string Name "Nome do produto"
        string Description "DescriÃ§Ã£o detalhada"
        decimal Price "PreÃ§o de venda"
        int QuantityPrice "Quantidade para preÃ§o"
        decimal UnitPrice "PreÃ§o unitÃ¡rio calculado"
        decimal Cost "Custo do produto"
        string CategoryId FK "Categoria (opcional)"
        string SKU "CÃ³digo Ãºnico (opcional)"
        string ImageUrl "URL da imagem"
        string Note "ObservaÃ§Ãµes"
        int AssemblyTime "Tempo montagem (min)"
        string AssemblyInstructions "InstruÃ§Ãµes de montagem"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === CATEGORIA DE PRODUTOS ===
    PRODUCT_CATEGORY {
        string Id PK "GUID Ãºnico"
        string Name "Nome da categoria"
        string Description "DescriÃ§Ã£o da categoria"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === HIERARQUIA DE COMPONENTES ===
    PRODUCT_COMPONENT_HIERARCHY {
        string Id PK "GUID Ãºnico"
        string Name "Nome da hierarquia"
        string Description "DescriÃ§Ã£o da hierarquia"
        string Notes "ObservaÃ§Ãµes"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === COMPONENTES ===
    PRODUCT_COMPONENT {
        string Id PK "GUID Ãºnico"
        string Name "Nome do componente"
        string Description "DescriÃ§Ã£o do componente"
        string ProductComponentHierarchyId FK "Hierarquia pai"
        decimal AdditionalCost "Custo adicional"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === RELACIONAMENTO PRODUTO COMPOSTO x HIERARQUIA ===
    COMPOSITE_PRODUCT_X_HIERARCHY {
        int Id PK "Auto-incremental"
        string ProductComponentHierarchyId FK "Hierarquia"
        string ProductId FK "Produto composto"
        int MinQuantity "Quantidade mÃ­nima"
        int MaxQuantity "Quantidade mÃ¡xima"
        bool IsOptional "Ã‰ opcional?"
        int AssemblyOrder "Ordem de montagem"
        string Notes "ObservaÃ§Ãµes"
        datetime CreatedDate "Data de criaÃ§Ã£o"
    }

    %% === ITENS DE GRUPO DE PRODUTOS ===
    PRODUCT_GROUP_ITEM {
        string Id PK "GUID Ãºnico"
        string ProductGroupId FK "Grupo pai"
        string ProductId FK "Produto (opcional)"
        string ProductCategoryId FK "Categoria (opcional)"
        int Quantity "Quantidade padrÃ£o"
        int MinQuantity "Quantidade mÃ­nima"
        int MaxQuantity "Quantidade mÃ¡xima"
        int DefaultQuantity "Quantidade padrÃ£o"
        bool IsOptional "Ã‰ opcional?"
        decimal ExtraPrice "PreÃ§o extra"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === REGRAS DE TROCA DE GRUPO ===
    PRODUCT_GROUP_EXCHANGE_RULE {
        string Id PK "GUID Ãºnico"
        string ProductGroupId FK "Grupo pai"
        string SourceGroupItemId FK "Item origem"
        int SourceGroupItemWeight "Peso item origem"
        string TargetGroupItemId FK "Item destino"
        int TargetGroupItemWeight "Peso item destino"
        decimal ExchangeRatio "ProporÃ§Ã£o de troca"
        bool IsActive "Regra ativa?"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === INGREDIENTES DE PRODUTO ===
    PRODUCT_INGREDIENT {
        string Id PK "GUID Ãºnico"
        string ProductId FK "Produto"
        string IngredientId FK "Ingrediente"
        decimal Quantity "Quantidade necessÃ¡ria"
        string UnitOfMeasure "Unidade de medida"
        string Notes "ObservaÃ§Ãµes"
        datetime CreatedDate "Data de criaÃ§Ã£o"
        datetime ModifiedDate "Data de modificaÃ§Ã£o"
    }

    %% === INGREDIENTES (REFERÃŠNCIA DOMÃNIO COMPRAS) ===
    INGREDIENT {
        string Id PK "GUID Ãºnico"
        string Name "Nome do ingrediente"
        string Description "DescriÃ§Ã£o"
        string UnitOfMeasure "Unidade padrÃ£o"
        decimal MinimumStockLevel "Estoque mÃ­nimo"
        string StateCode "Active|Inactive"
    }

    %% ==========================================
    %% RELACIONAMENTOS
    %% ==========================================

    %% Produto -> Categoria (N:1, opcional)
    PRODUCT ||--o{ PRODUCT_CATEGORY : "pertence a"

    %% Hierarquia -> Componentes (1:N)
    PRODUCT_COMPONENT_HIERARCHY ||--o{ PRODUCT_COMPONENT : "contÃ©m"

    %% Produto Composto -> Hierarquias (N:N via CPXH)
    PRODUCT ||--o{ COMPOSITE_PRODUCT_X_HIERARCHY : "usa"
    PRODUCT_COMPONENT_HIERARCHY ||--o{ COMPOSITE_PRODUCT_X_HIERARCHY : "aplicada em"

    %% Produto Grupo -> Itens do Grupo (1:N)
    PRODUCT ||--o{ PRODUCT_GROUP_ITEM : "contÃ©m itens"
    
    %% Item de Grupo -> Produto/Categoria (opcional, mutuamente exclusivo)
    PRODUCT ||--o{ PRODUCT_GROUP_ITEM : "pode ser item"
    PRODUCT_CATEGORY ||--o{ PRODUCT_GROUP_ITEM : "pode ser categoria"

    %% Produto Grupo -> Regras de Troca (1:N)
    PRODUCT ||--o{ PRODUCT_GROUP_EXCHANGE_RULE : "tem regras"
    PRODUCT_GROUP_ITEM ||--o{ PRODUCT_GROUP_EXCHANGE_RULE : "origem"
    PRODUCT_GROUP_ITEM ||--o{ PRODUCT_GROUP_EXCHANGE_RULE : "destino"

    %% Produto -> Ingredientes (N:N via ProductIngredient)
    PRODUCT ||--o{ PRODUCT_INGREDIENT : "usa"
    INGREDIENT ||--o{ PRODUCT_INGREDIENT : "compÃµe"

    %% ==========================================
    %% STYLING POR DOMÃNIO
    %% ==========================================
    
    %% PRODUTO = Verde (#00a86b)
    PRODUCT {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }
    
    PRODUCT_CATEGORY {
        background-color "#00a86b"
        color "white" 
        border-color "#00a86b"
    }
    
    PRODUCT_COMPONENT_HIERARCHY {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b" 
    }
    
    PRODUCT_COMPONENT {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }
    
    COMPOSITE_PRODUCT_X_HIERARCHY {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }
    
    PRODUCT_GROUP_ITEM {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }
    
    PRODUCT_GROUP_EXCHANGE_RULE {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }
    
    PRODUCT_INGREDIENT {
        background-color "#00a86b"
        color "white"
        border-color "#00a86b"
    }
    
    %% REFERÃŠNCIA EXTERNA = Cinza claro
    INGREDIENT {
        background-color "#e0e0e0"
        color "black"
        border-color "#999999"
    }
```

## ðŸ“‹ Detalhes das Entidades

### **ðŸ”· PRODUCT (Classe Base Abstrata)**
- **PropÃ³sito**: Entidade principal com heranÃ§a TPH (Table Per Hierarchy)
- **Tipos**: Simple, Composite, Group (discriminador ProductType)
- **CaracterÃ­sticas**: Nome, preÃ§o, custo, tempo de montagem, instruÃ§Ãµes

### **ðŸ“‚ PRODUCT_CATEGORY**
- **PropÃ³sito**: Agrupamento lÃ³gico de produtos
- **Relacionamento**: 1:N com Product (opcional)
- **Exemplos**: "Salgados Tradicionais", "Bolos Especiais"

### **ðŸ—ï¸ PRODUCT_COMPONENT_HIERARCHY**
- **PropÃ³sito**: Define "camadas" de personalizaÃ§Ã£o para produtos compostos
- **Relacionamento**: N:N com Product via CompositeProductXHierarchy
- **Exemplos**: "Massa", "Recheio", "Cobertura", "Opcionais"

### **ðŸ§© PRODUCT_COMPONENT**
- **PropÃ³sito**: OpÃ§Ãµes especÃ­ficas dentro de uma hierarquia
- **Relacionamento**: N:1 com ProductComponentHierarchy
- **Exemplos**: "Massa de Chocolate", "Recheio de Brigadeiro"

### **ðŸ”— COMPOSITE_PRODUCT_X_HIERARCHY**
- **PropÃ³sito**: Relacionamento N:N com regras de composiÃ§Ã£o
- **CaracterÃ­sticas**: Min/Max quantidade, opcionalidade, ordem de montagem
- **Tipo**: Tabela intermediÃ¡ria com ID auto-incremental

### **ðŸ“¦ PRODUCT_GROUP_ITEM**
- **PropÃ³sito**: Itens que compÃµem um grupo/kit de produtos
- **Relacionamento**: Pode referenciar Product OU ProductCategory (mutuamente exclusivo)
- **CaracterÃ­sticas**: Quantidades (min/max/padrÃ£o), opcionalidade, preÃ§o extra

### **âš–ï¸ PRODUCT_GROUP_EXCHANGE_RULE**
- **PropÃ³sito**: Define regras de troca/proporÃ§Ã£o entre itens de um grupo
- **CaracterÃ­sticas**: Pesos, ratio de troca, ativaÃ§Ã£o
- **Exemplo**: "2 Salgados Tradicionais â†” 1 Salgado Especial"

### **ðŸ¥˜ PRODUCT_INGREDIENT**
- **PropÃ³sito**: Relacionamento N:N entre Product e Ingredient (receitas)
- **CaracterÃ­sticas**: Quantidade necessÃ¡ria, unidade de medida
- **IntegraÃ§Ã£o**: Conecta com DomÃ­nio de Compras via Ingredient

## ðŸ”„ Tipos de Produto e Relacionamentos

### **Simple Product**
- Usa apenas: Product + ProductCategory + ProductIngredient
- Estrutura bÃ¡sica sem customizaÃ§Ã£o

### **Composite Product**  
- Usa: Product + ProductCategory + CompositeProductXHierarchy + ProductComponentHierarchy + ProductComponent
- Permite customizaÃ§Ã£o via hierarquias de componentes

### **Product Group**
- Usa: Product + ProductCategory + ProductGroupItem + ProductGroupExchangeRule
- Kits flexÃ­veis com regras de troca

---

**Arquivo**: `01-product-domain-erd.md`  
**DomÃ­nio**: Produto (#00a86b)  
**Tipo**: Entity-Relationship Diagram  
**NÃ­vel**: Detalhado (propriedades + tipos + relacionamentos)
