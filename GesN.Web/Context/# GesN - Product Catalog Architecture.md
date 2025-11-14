ï»¿ÃƒÂ¯Ã‚Â»Ã‚Â¿# GesN - Arquitetura do CatÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡logo de Produtos

Este documento detalha a arquitetura e a lÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â³gica por trÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡s do mÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â³dulo de CatÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡logo de Produtos do sistema GesN. O objetivo ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© criar um sistema flexÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­vel capaz de modelar desde produtos simples atÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© produtos compostos e kits altamente customizÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡veis.

## 1. A Entidade Central: `Product`

Tudo no catÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡logo comeÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§a com a entidade `Product`. Ela ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© a base para qualquer item que possa ser vendido, produzido ou controlado no estoque. Um `Product` sempre terÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡ um dos trÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âªs tipos a seguir, que definem seu comportamento e como ele interage com outras partes do sistema.

### Atributos Principais de um `Product`:
-   `Id`: Identificador ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âºnico.
-   `Name`, `Description`, `SKU`: InformaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âµes bÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡sicas.
-   `Price`, `Cost`: Dados financeiros.
-   `CategoryId`: Para organizaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o e filtros.
-   `ProductType`: O campo mais importante, que define o comportamento do produto.

---

## 2. Os TrÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âªs Tipos de Produto

### a) `ProductType.Simple` (Produto Simples)

ÃƒÂƒÃ†Â’ÃƒÂ¢Ã¢Â‚Â¬Ã‚Â° a forma mais bÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡sica de um produto. Representa um item individual que nÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© composto por outros produtos vendÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡veis.

-   **DefiniÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o:** Um item unitÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡rio e indivisÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­vel do ponto de vista de venda.
-   **Exemplos:**
    -   "Lata de Coca-Cola 350ml"
    -   "Coxinha comum"
    -   "PÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o de queijo"
-   **Relacionamentos:** Pode estar associado a N `Ingredients` para controle de matÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â©ria-prima e baixa de estoque (ex: uma "PorÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o de Batata Frita" consome X gramas de "Batata Crua" e Y ml de "ÃƒÂƒÃ†Â’ÃƒÂ¢Ã¢Â‚Â¬Ã…Â“leo").

### b) `ProductType.Composite` (Produto Composto / ConfigurÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡vel)

Representa um **ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âºnico item vendÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡vel** cuja composiÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o interna ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© parametrizÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡vel e definida no momento do pedido. ÃƒÂƒÃ†Â’ÃƒÂ¢Ã¢Â‚Â¬Ã‚Â° o conceito de "Monte o seu...".

-   **DefiniÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o:** Um produto que, embora vendido como uma unidade, ele possui uma parametrizaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o flexÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­vel que na visÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o de Vendas fornece a possibilidade de escolher as opÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âµes de composiÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o parametrizadas para o Produto no CatÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡logo de Produtos. O administrador define as regras de montagem (ex: "escolha 1 massa e 2 recheios") atravÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â©s da associaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o com a tabela , e o vendedor faz as escolhas.
-   **Exemplos:**
    -   "Bolo de Festa (20 pessoas)"
    -   "Pizza Meio a Meio"
    -   "SanduÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­che customizÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡vel"
-   **Mecanismo de Funcionamento:** Utiliza a tabela `ProductComponent` para definir sua estrutura. Cada registro em `ProductComponent` representa um "slot" na receita.
    -   **Exemplo de `ProductComponent` para o "Bolo de Festa":**
        -   Registro 1: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Massas de Bolo"), `Quantity`: 1.
        -   Registro 2: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Recheios de Bolo"), `Quantity`: 2.
        -   Registro 3: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Coberturas"), `Quantity`: 1.
    -   *(Nota: A implementaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o exata de como `ProductComponent` aponta para uma categoria precisa ser detalhada, mas o conceito ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© este).*
-   **Tabela de Apoio:** `ProductComponent`
    -   Esta tabela faz a ligaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o: "Qual produto composto (`CompositeProductId`) ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© feito de quais componentes (`ComponentProductId`) e em qual quantidade (`Quantity`)?"
    -   **Exemplo de `ProductComponent`:**
        -   `CompositeProductId`: (ID do "X-Salada")
        -   `ComponentProductId`: (ID do "PÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o de HambÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âºrguer")
        -   `Quantity`: 1

### c) `ProductType.Group` (Grupo de Produtos)

Este ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© o tipo mais flexÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­vel e poderoso. Ele representa um "Kit", "Combo" ou "Pacote Promocional" onde o cliente final (ou o atendente) pode fazer escolhas e customizaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âµes. O `Product` do tipo `Group` funciona como um container de regras.

-   **DefiniÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o:** Um produto vendÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡vel que agrupa um conjunto de outros produtos e/ou opÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âµes, permitindo flexibilidade na montagem final.
-   **Exemplos:**
    -   "Monte seu AÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§aÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­"
    -   "Combo AlmoÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§o Executivo (Prato + Bebida + Sobremesa)"
    -   "Kit Festa (Bolo + Salgados + Doces)"
-   **Tabelas de Apoio:** A mÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡gica do `ProductGroup` acontece em suas tabelas relacionadas:

    #### `ProductGroupItem` - O CoraÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o FlexÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­vel do Grupo
    Define os "slots" ou itens que compÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âµem o grupo. Sua principal caracterÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­stica ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© a capacidade de apontar para um produto especÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­fico **OU** para uma categoria de produtos.

    -   **CenÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡rio 1: Item Fixo e ObrigatÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â³rio**
        -   **Como funciona:** O campo `ProductId` ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© preenchido.
        -   **Exemplo:** No "Combo AlmoÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§o Executivo", o item "Arroz e FeijÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o" ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© obrigatÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â³rio.
        -   **Registro no `ProductGroupItem`:**
            -   `ProductGroupId`: (ID do "Combo AlmoÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§o")
            -   `ProductId`: (ID do produto "Arroz e FeijÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o")
            -   `ProductCategoryId`: `NULL`
            -   `IsOptional`: `false`

    -   **CenÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡rio 2: Escolha Dentro de uma Categoria**
        -   **Como funciona:** O campo `ProductCategoryId` ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© preenchido.
        -   **Exemplo:** No "Combo AlmoÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§o Executivo", o cliente pode escolher uma bebida da categoria "Refrigerantes em Lata".
        -   **Registro no `ProductGroupItem`:**
            -   `ProductGroupId`: (ID do "Combo AlmoÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§o")
            -   `ProductId`: `NULL`
            -   `ProductCategoryId`: (ID da categoria "Refrigerantes em Lata")
            -   `IsOptional`: `false`
            -   `MinQuantity`: 1, `MaxQuantity`: 1 (O cliente deve escolher exatamente 1 item desta categoria).

    #### `ProductGroupOption` - CustomizaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âµes Adicionais
    Define opÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âµes que nÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o sÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o necessariamente produtos, como "ponto da carne" ou "sem cebola".

    -   **Exemplo:** Para um `ProductGroup` "Monte seu SanduÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â­che", podemos ter uma opÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o:
        -   `Name`: "Adicionais"
        -   `OptionType`: `MultipleChoice`
        -   `IsRequired`: `false`
        -   Os valores desta opÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o ("Bacon Extra", "Queijo Extra") seriam gerenciados em outra tabela (`ProductGroupOptionValue`, por exemplo).

    #### `ProductGroupExchangeRule` - Regras de Troca
    Permite substituir um item padrÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o do grupo por outro, geralmente com um custo adicional.

    -   **Exemplo:** No "Combo AlmoÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§o Executivo", a bebida padrÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© um refrigerante. O cliente pode trocÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡-la por um suco.
        -   `OriginalProductId`: (ID do "Refrigerante Lata")
        -   `ExchangeProductId`: (ID do "Suco Natural de Laranja")
        -   `AdditionalCost`: `R$ 3,00`

---

## 3. CenÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡rio PrÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡tico: "Combo Super Burger"

Vamos modelar um combo para ilustrar a arquitetura.

1.  **CriaÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o do Produto Principal:**
    -   Criar um `Product` chamado "Combo Super Burger".
    -   `ProductType` = `Group`.
    -   `BasePrice` = `R$ 35,00`.

2.  **DefiniÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o dos Itens do Grupo (`ProductGroupItem`):**

    -   **Item 1 (HambÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Âºrguer - Fixo):**
        -   `ProductId`: Aponta para o `Product` "Super Burger" (que ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â© do tipo `Composite`).
        -   `ProductCategoryId`: `NULL`.
        -   `Quantity`: 1, `IsOptional`: `false`.

    -   **Item 2 (Acompanhamento - Escolha):**
        -   `ProductId`: `NULL`.
        -   `ProductCategoryId`: Aponta para a `ProductCategory` "Acompanhamentos".
        -   `Quantity`: 1, `IsOptional`: `false`.
        -   *O sistema permitirÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡ que o atendente escolha entre "Batata Frita M", "AnÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â©is de Cebola P", etc., que pertencem a essa categoria.*

    -   **Item 3 (Bebida - Escolha):**
        -   `ProductId`: `NULL`.
        -   `ProductCategoryId`: Aponta para a `ProductCategory` "Bebidas NÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o AlcoÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â³licas".
        -   `Quantity`: 1, `IsOptional`: `false`.

3.  **DefiniÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o de uma Regra de Troca (`ProductGroupExchangeRule`):**

    -   **Trocar Batata por Salada:**
        -   `OriginalProductId`: (ID da "Batata Frita M").
        -   `ExchangeProductId`: (ID da "Salada Simples").
        -   `AdditionalCost`: `R$ 0,00`.

4.  **DefiniÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o de uma OpÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â§ÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â£o (`ProductGroupOption`):**

    -   **Ponto da Carne:**
        -   `Name`: "Ponto da Carne do Super Burger"
        -   `OptionType`: `SingleChoice` (com valores "Mal Passado", "Ao Ponto", "Bem Passado").
        -   `IsRequired`: `true`.

Com esta estrutura, o GesN pode lidar com uma vasta gama de cenÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡rios de venda, desde o mais simples ao mais complexo, de forma coesa e escalÃƒÂƒÃ†Â’ÃƒÂ‚Ã‚Â¡vel.

