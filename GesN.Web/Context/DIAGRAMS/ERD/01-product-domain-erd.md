# 📦 ERD - DOMÍNIO DE PRODUTO

## 🎯 Visão Geral
Diagrama Entity-Relationship completo do Domínio de Produto, mostrando todas as entidades, propriedades, tipos de dados e relacionamentos. Este domínio gerencia o catálogo de produtos/serviços da empresa através de 3 tipos principais: Simple, Composite e Group.

## 🗄️ Diagrama de Entidades e Relacionamentos

```mermaid
erDiagram
    %% === PRODUTO (CLASSE BASE ABSTRATA) ===
    PRODUCT {
        string Id PK "GUID único"
        string ProductType "Simple|Composite|Group"
        string Name "Nome do produto"
        string Description "Descrição detalhada"
        decimal Price "Preço de venda"
        int QuantityPrice "Quantidade para preço"
        decimal UnitPrice "Preço unitário calculado"
        decimal Cost "Custo do produto"
        string CategoryId FK "Categoria (opcional)"
        string SKU "Código único (opcional)"
        string ImageUrl "URL da imagem"
        string Note "Observações"
        int AssemblyTime "Tempo montagem (min)"
        string AssemblyInstructions "Instruções de montagem"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === CATEGORIA DE PRODUTOS ===
    PRODUCT_CATEGORY {
        string Id PK "GUID único"
        string Name "Nome da categoria"
        string Description "Descrição da categoria"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === HIERARQUIA DE COMPONENTES ===
    PRODUCT_COMPONENT_HIERARCHY {
        string Id PK "GUID único"
        string Name "Nome da hierarquia"
        string Description "Descrição da hierarquia"
        string Notes "Observações"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === COMPONENTES ===
    PRODUCT_COMPONENT {
        string Id PK "GUID único"
        string Name "Nome do componente"
        string Description "Descrição do componente"
        string ProductComponentHierarchyId FK "Hierarquia pai"
        decimal AdditionalCost "Custo adicional"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === RELACIONAMENTO PRODUTO COMPOSTO x HIERARQUIA ===
    COMPOSITE_PRODUCT_X_HIERARCHY {
        int Id PK "Auto-incremental"
        string ProductComponentHierarchyId FK "Hierarquia"
        string ProductId FK "Produto composto"
        int MinQuantity "Quantidade mínima"
        int MaxQuantity "Quantidade máxima"
        bool IsOptional "É opcional?"
        int AssemblyOrder "Ordem de montagem"
        string Notes "Observações"
        datetime CreatedDate "Data de criação"
    }

    %% === ITENS DE GRUPO DE PRODUTOS ===
    PRODUCT_GROUP_ITEM {
        string Id PK "GUID único"
        string ProductGroupId FK "Grupo pai"
        string ProductId FK "Produto (opcional)"
        string ProductCategoryId FK "Categoria (opcional)"
        int Quantity "Quantidade padrão"
        int MinQuantity "Quantidade mínima"
        int MaxQuantity "Quantidade máxima"
        int DefaultQuantity "Quantidade padrão"
        bool IsOptional "É opcional?"
        decimal ExtraPrice "Preço extra"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === REGRAS DE TROCA DE GRUPO ===
    PRODUCT_GROUP_EXCHANGE_RULE {
        string Id PK "GUID único"
        string ProductGroupId FK "Grupo pai"
        string SourceGroupItemId FK "Item origem"
        int SourceGroupItemWeight "Peso item origem"
        string TargetGroupItemId FK "Item destino"
        int TargetGroupItemWeight "Peso item destino"
        decimal ExchangeRatio "Proporção de troca"
        bool IsActive "Regra ativa?"
        string StateCode "Active|Inactive"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === INGREDIENTES DE PRODUTO ===
    PRODUCT_INGREDIENT {
        string Id PK "GUID único"
        string ProductId FK "Produto"
        string IngredientId FK "Ingrediente"
        decimal Quantity "Quantidade necessária"
        string UnitOfMeasure "Unidade de medida"
        string Notes "Observações"
        datetime CreatedDate "Data de criação"
        datetime ModifiedDate "Data de modificação"
    }

    %% === INGREDIENTES (REFERÊNCIA DOMÍNIO COMPRAS) ===
    INGREDIENT {
        string Id PK "GUID único"
        string Name "Nome do ingrediente"
        string Description "Descrição"
        string UnitOfMeasure "Unidade padrão"
        decimal MinimumStockLevel "Estoque mínimo"
        string StateCode "Active|Inactive"
    }

    %% ==========================================
    %% RELACIONAMENTOS
    %% ==========================================

    %% Produto -> Categoria (N:1, opcional)
    PRODUCT ||--o{ PRODUCT_CATEGORY : "pertence a"

    %% Hierarquia -> Componentes (1:N)
    PRODUCT_COMPONENT_HIERARCHY ||--o{ PRODUCT_COMPONENT : "contém"

    %% Produto Composto -> Hierarquias (N:N via CPXH)
    PRODUCT ||--o{ COMPOSITE_PRODUCT_X_HIERARCHY : "usa"
    PRODUCT_COMPONENT_HIERARCHY ||--o{ COMPOSITE_PRODUCT_X_HIERARCHY : "aplicada em"

    %% Produto Grupo -> Itens do Grupo (1:N)
    PRODUCT ||--o{ PRODUCT_GROUP_ITEM : "contém itens"
    
    %% Item de Grupo -> Produto/Categoria (opcional, mutuamente exclusivo)
    PRODUCT ||--o{ PRODUCT_GROUP_ITEM : "pode ser item"
    PRODUCT_CATEGORY ||--o{ PRODUCT_GROUP_ITEM : "pode ser categoria"

    %% Produto Grupo -> Regras de Troca (1:N)
    PRODUCT ||--o{ PRODUCT_GROUP_EXCHANGE_RULE : "tem regras"
    PRODUCT_GROUP_ITEM ||--o{ PRODUCT_GROUP_EXCHANGE_RULE : "origem"
    PRODUCT_GROUP_ITEM ||--o{ PRODUCT_GROUP_EXCHANGE_RULE : "destino"

    %% Produto -> Ingredientes (N:N via ProductIngredient)
    PRODUCT ||--o{ PRODUCT_INGREDIENT : "usa"
    INGREDIENT ||--o{ PRODUCT_INGREDIENT : "compõe"

    %% ==========================================
    %% STYLING POR DOMÍNIO
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
    
    %% REFERÊNCIA EXTERNA = Cinza claro
    INGREDIENT {
        background-color "#e0e0e0"
        color "black"
        border-color "#999999"
    }
```

## 📋 Detalhes das Entidades

### **🔷 PRODUCT (Classe Base Abstrata)**
- **Propósito**: Entidade principal com herança TPH (Table Per Hierarchy)
- **Tipos**: Simple, Composite, Group (discriminador ProductType)
- **Características**: Nome, preço, custo, tempo de montagem, instruções

### **📂 PRODUCT_CATEGORY**
- **Propósito**: Agrupamento lógico de produtos
- **Relacionamento**: 1:N com Product (opcional)
- **Exemplos**: "Salgados Tradicionais", "Bolos Especiais"

### **🏗️ PRODUCT_COMPONENT_HIERARCHY**
- **Propósito**: Define "camadas" de personalização para produtos compostos
- **Relacionamento**: N:N com Product via CompositeProductXHierarchy
- **Exemplos**: "Massa", "Recheio", "Cobertura", "Opcionais"

### **🧩 PRODUCT_COMPONENT**
- **Propósito**: Opções específicas dentro de uma hierarquia
- **Relacionamento**: N:1 com ProductComponentHierarchy
- **Exemplos**: "Massa de Chocolate", "Recheio de Brigadeiro"

### **🔗 COMPOSITE_PRODUCT_X_HIERARCHY**
- **Propósito**: Relacionamento N:N com regras de composição
- **Características**: Min/Max quantidade, opcionalidade, ordem de montagem
- **Tipo**: Tabela intermediária com ID auto-incremental

### **📦 PRODUCT_GROUP_ITEM**
- **Propósito**: Itens que compõem um grupo/kit de produtos
- **Relacionamento**: Pode referenciar Product OU ProductCategory (mutuamente exclusivo)
- **Características**: Quantidades (min/max/padrão), opcionalidade, preço extra

### **⚖️ PRODUCT_GROUP_EXCHANGE_RULE**
- **Propósito**: Define regras de troca/proporção entre itens de um grupo
- **Características**: Pesos, ratio de troca, ativação
- **Exemplo**: "2 Salgados Tradicionais ↔ 1 Salgado Especial"

### **🥘 PRODUCT_INGREDIENT**
- **Propósito**: Relacionamento N:N entre Product e Ingredient (receitas)
- **Características**: Quantidade necessária, unidade de medida
- **Integração**: Conecta com Domínio de Compras via Ingredient

## 🔄 Tipos de Produto e Relacionamentos

### **Simple Product**
- Usa apenas: Product + ProductCategory + ProductIngredient
- Estrutura básica sem customização

### **Composite Product**  
- Usa: Product + ProductCategory + CompositeProductXHierarchy + ProductComponentHierarchy + ProductComponent
- Permite customização via hierarquias de componentes

### **Product Group**
- Usa: Product + ProductCategory + ProductGroupItem + ProductGroupExchangeRule
- Kits flexíveis com regras de troca

---

**Arquivo**: `01-product-domain-erd.md`  
**Domínio**: Produto (#00a86b)  
**Tipo**: Entity-Relationship Diagram  
**Nível**: Detalhado (propriedades + tipos + relacionamentos)
