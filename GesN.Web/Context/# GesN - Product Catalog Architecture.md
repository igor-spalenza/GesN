# GesN - Arquitetura do CatÃ¡logo de Produtos

Este documento detalha a arquitetura e a lÃ³gica por trÃ¡s do mÃ³dulo de CatÃ¡logo de Produtos do sistema GesN. O objetivo Ã© criar um sistema flexÃ­vel capaz de modelar desde produtos simples atÃ© produtos compostos e kits altamente customizÃ¡veis.

## 1. A Entidade Central: `Product`

Tudo no catÃ¡logo comeÃ§a com a entidade `Product`. Ela Ã© a base para qualquer item que possa ser vendido, produzido ou controlado no estoque. Um `Product` sempre terÃ¡ um dos trÃªs tipos a seguir, que definem seu comportamento e como ele interage com outras partes do sistema.

### Atributos Principais de um `Product`:
-   `Id`: Identificador Ãºnico.
-   `Name`, `Description`, `SKU`: InformaÃ§Ãµes bÃ¡sicas.
-   `Price`, `Cost`: Dados financeiros.
-   `CategoryId`: Para organizaÃ§Ã£o e filtros.
-   `ProductType`: O campo mais importante, que define o comportamento do produto.

---

## 2. Os TrÃªs Tipos de Produto

### a) `ProductType.Simple` (Produto Simples)

Ã‰ a forma mais bÃ¡sica de um produto. Representa um item individual que nÃ£o Ã© composto por outros produtos vendÃ¡veis.

-   **DefiniÃ§Ã£o:** Um item unitÃ¡rio e indivisÃ­vel do ponto de vista de venda.
-   **Exemplos:**
    -   "Lata de Coca-Cola 350ml"
    -   "Coxinha comum"
    -   "PÃ£o de queijo"
-   **Relacionamentos:** Pode estar associado a N `Ingredients` para controle de matÃ©ria-prima e baixa de estoque (ex: uma "PorÃ§Ã£o de Batata Frita" consome X gramas de "Batata Crua" e Y ml de "Ã“leo").

### b) `ProductType.Composite` (Produto Composto / ConfigurÃ¡vel)

Representa um **Ãºnico item vendÃ¡vel** cuja composiÃ§Ã£o interna Ã© parametrizÃ¡vel e definida no momento do pedido. Ã‰ o conceito de "Monte o seu...".

-   **DefiniÃ§Ã£o:** Um produto que, embora vendido como uma unidade, ele possui uma parametrizaÃ§Ã£o flexÃ­vel que na visÃ£o de Vendas fornece a possibilidade de escolher as opÃ§Ãµes de composiÃ§Ã£o parametrizadas para o Produto no CatÃ¡logo de Produtos. O administrador define as regras de montagem (ex: "escolha 1 massa e 2 recheios") atravÃ©s da associaÃ§Ã£o com a tabela , e o vendedor faz as escolhas.
-   **Exemplos:**
    -   "Bolo de Festa (20 pessoas)"
    -   "Pizza Meio a Meio"
    -   "SanduÃ­che customizÃ¡vel"
-   **Mecanismo de Funcionamento:** Utiliza a tabela `ProductComponent` para definir sua estrutura. Cada registro em `ProductComponent` representa um "slot" na receita.
    -   **Exemplo de `ProductComponent` para o "Bolo de Festa":**
        -   Registro 1: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Massas de Bolo"), `Quantity`: 1.
        -   Registro 2: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Recheios de Bolo"), `Quantity`: 2.
        -   Registro 3: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Coberturas"), `Quantity`: 1.
    -   *(Nota: A implementaÃ§Ã£o exata de como `ProductComponent` aponta para uma categoria precisa ser detalhada, mas o conceito Ã© este).*
-   **Tabela de Apoio:** `ProductComponent`
    -   Esta tabela faz a ligaÃ§Ã£o: "Qual produto composto (`CompositeProductId`) Ã© feito de quais componentes (`ComponentProductId`) e em qual quantidade (`Quantity`)?"
    -   **Exemplo de `ProductComponent`:**
        -   `CompositeProductId`: (ID do "X-Salada")
        -   `ComponentProductId`: (ID do "PÃ£o de HambÃºrguer")
        -   `Quantity`: 1

### c) `ProductType.Group` (Grupo de Produtos)

Este Ã© o tipo mais flexÃ­vel e poderoso. Ele representa um "Kit", "Combo" ou "Pacote Promocional" onde o cliente final (ou o atendente) pode fazer escolhas e customizaÃ§Ãµes. O `Product` do tipo `Group` funciona como um container de regras.

-   **DefiniÃ§Ã£o:** Um produto vendÃ¡vel que agrupa um conjunto de outros produtos e/ou opÃ§Ãµes, permitindo flexibilidade na montagem final.
-   **Exemplos:**
    -   "Monte seu AÃ§aÃ­"
    -   "Combo AlmoÃ§o Executivo (Prato + Bebida + Sobremesa)"
    -   "Kit Festa (Bolo + Salgados + Doces)"
-   **Tabelas de Apoio:** A mÃ¡gica do `ProductGroup` acontece em suas tabelas relacionadas:

    #### `ProductGroupItem` - O CoraÃ§Ã£o FlexÃ­vel do Grupo
    Define os "slots" ou itens que compÃµem o grupo. Sua principal caracterÃ­stica Ã© a capacidade de apontar para um produto especÃ­fico **OU** para uma categoria de produtos.

    -   **CenÃ¡rio 1: Item Fixo e ObrigatÃ³rio**
        -   **Como funciona:** O campo `ProductId` Ã© preenchido.
        -   **Exemplo:** No "Combo AlmoÃ§o Executivo", o item "Arroz e FeijÃ£o" Ã© obrigatÃ³rio.
        -   **Registro no `ProductGroupItem`:**
            -   `ProductGroupId`: (ID do "Combo AlmoÃ§o")
            -   `ProductId`: (ID do produto "Arroz e FeijÃ£o")
            -   `ProductCategoryId`: `NULL`
            -   `IsOptional`: `false`

    -   **CenÃ¡rio 2: Escolha Dentro de uma Categoria**
        -   **Como funciona:** O campo `ProductCategoryId` Ã© preenchido.
        -   **Exemplo:** No "Combo AlmoÃ§o Executivo", o cliente pode escolher uma bebida da categoria "Refrigerantes em Lata".
        -   **Registro no `ProductGroupItem`:**
            -   `ProductGroupId`: (ID do "Combo AlmoÃ§o")
            -   `ProductId`: `NULL`
            -   `ProductCategoryId`: (ID da categoria "Refrigerantes em Lata")
            -   `IsOptional`: `false`
            -   `MinQuantity`: 1, `MaxQuantity`: 1 (O cliente deve escolher exatamente 1 item desta categoria).

    #### `ProductGroupOption` - CustomizaÃ§Ãµes Adicionais
    Define opÃ§Ãµes que nÃ£o sÃ£o necessariamente produtos, como "ponto da carne" ou "sem cebola".

    -   **Exemplo:** Para um `ProductGroup` "Monte seu SanduÃ­che", podemos ter uma opÃ§Ã£o:
        -   `Name`: "Adicionais"
        -   `OptionType`: `MultipleChoice`
        -   `IsRequired`: `false`
        -   Os valores desta opÃ§Ã£o ("Bacon Extra", "Queijo Extra") seriam gerenciados em outra tabela (`ProductGroupOptionValue`, por exemplo).

    #### `ProductGroupExchangeRule` - Regras de Troca
    Permite substituir um item padrÃ£o do grupo por outro, geralmente com um custo adicional.

    -   **Exemplo:** No "Combo AlmoÃ§o Executivo", a bebida padrÃ£o Ã© um refrigerante. O cliente pode trocÃ¡-la por um suco.
        -   `OriginalProductId`: (ID do "Refrigerante Lata")
        -   `ExchangeProductId`: (ID do "Suco Natural de Laranja")
        -   `AdditionalCost`: `R$ 3,00`

---

## 3. CenÃ¡rio PrÃ¡tico: "Combo Super Burger"

Vamos modelar um combo para ilustrar a arquitetura.

1.  **CriaÃ§Ã£o do Produto Principal:**
    -   Criar um `Product` chamado "Combo Super Burger".
    -   `ProductType` = `Group`.
    -   `BasePrice` = `R$ 35,00`.

2.  **DefiniÃ§Ã£o dos Itens do Grupo (`ProductGroupItem`):**

    -   **Item 1 (HambÃºrguer - Fixo):**
        -   `ProductId`: Aponta para o `Product` "Super Burger" (que Ã© do tipo `Composite`).
        -   `ProductCategoryId`: `NULL`.
        -   `Quantity`: 1, `IsOptional`: `false`.

    -   **Item 2 (Acompanhamento - Escolha):**
        -   `ProductId`: `NULL`.
        -   `ProductCategoryId`: Aponta para a `ProductCategory` "Acompanhamentos".
        -   `Quantity`: 1, `IsOptional`: `false`.
        -   *O sistema permitirÃ¡ que o atendente escolha entre "Batata Frita M", "AnÃ©is de Cebola P", etc., que pertencem a essa categoria.*

    -   **Item 3 (Bebida - Escolha):**
        -   `ProductId`: `NULL`.
        -   `ProductCategoryId`: Aponta para a `ProductCategory` "Bebidas NÃ£o AlcoÃ³licas".
        -   `Quantity`: 1, `IsOptional`: `false`.

3.  **DefiniÃ§Ã£o de uma Regra de Troca (`ProductGroupExchangeRule`):**

    -   **Trocar Batata por Salada:**
        -   `OriginalProductId`: (ID da "Batata Frita M").
        -   `ExchangeProductId`: (ID da "Salada Simples").
        -   `AdditionalCost`: `R$ 0,00`.

4.  **DefiniÃ§Ã£o de uma OpÃ§Ã£o (`ProductGroupOption`):**

    -   **Ponto da Carne:**
        -   `Name`: "Ponto da Carne do Super Burger"
        -   `OptionType`: `SingleChoice` (com valores "Mal Passado", "Ao Ponto", "Bem Passado").
        -   `IsRequired`: `true`.

Com esta estrutura, o GesN pode lidar com uma vasta gama de cenÃ¡rios de venda, desde o mais simples ao mais complexo, de forma coesa e escalÃ¡vel.

