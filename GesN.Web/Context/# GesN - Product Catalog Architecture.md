# GesN - Arquitetura do Catálogo de Produtos

Este documento detalha a arquitetura e a lógica por trás do módulo de Catálogo de Produtos do sistema GesN. O objetivo é criar um sistema flexível capaz de modelar desde produtos simples até produtos compostos e kits altamente customizáveis.

## 1. A Entidade Central: `Product`

Tudo no catálogo começa com a entidade `Product`. Ela é a base para qualquer item que possa ser vendido, produzido ou controlado no estoque. Um `Product` sempre terá um dos três tipos a seguir, que definem seu comportamento e como ele interage com outras partes do sistema.

### Atributos Principais de um `Product`:
-   `Id`: Identificador único.
-   `Name`, `Description`, `SKU`: Informações básicas.
-   `Price`, `Cost`: Dados financeiros.
-   `CategoryId`: Para organização e filtros.
-   `ProductType`: O campo mais importante, que define o comportamento do produto.

---

## 2. Os Três Tipos de Produto

### a) `ProductType.Simple` (Produto Simples)

É a forma mais básica de um produto. Representa um item individual que não é composto por outros produtos vendáveis.

-   **Definição:** Um item unitário e indivisível do ponto de vista de venda.
-   **Exemplos:**
    -   "Lata de Coca-Cola 350ml"
    -   "Coxinha comum"
    -   "Pão de queijo"
-   **Relacionamentos:** Pode estar associado a N `Ingredients` para controle de matéria-prima e baixa de estoque (ex: uma "Porção de Batata Frita" consome X gramas de "Batata Crua" e Y ml de "Óleo").

### b) `ProductType.Composite` (Produto Composto / Configurável)

Representa um **único item vendável** cuja composição interna é parametrizável e definida no momento do pedido. É o conceito de "Monte o seu...".

-   **Definição:** Um produto que, embora vendido como uma unidade, ele possui uma parametrização flexível que na visão de Vendas fornece a possibilidade de escolher as opções de composição parametrizadas para o Produto no Catálogo de Produtos. O administrador define as regras de montagem (ex: "escolha 1 massa e 2 recheios") através da associação com a tabela , e o vendedor faz as escolhas.
-   **Exemplos:**
    -   "Bolo de Festa (20 pessoas)"
    -   "Pizza Meio a Meio"
    -   "Sanduíche customizável"
-   **Mecanismo de Funcionamento:** Utiliza a tabela `ProductComponent` para definir sua estrutura. Cada registro em `ProductComponent` representa um "slot" na receita.
    -   **Exemplo de `ProductComponent` para o "Bolo de Festa":**
        -   Registro 1: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Massas de Bolo"), `Quantity`: 1.
        -   Registro 2: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Recheios de Bolo"), `Quantity`: 2.
        -   Registro 3: `CompositeProductId` (ID do Bolo), `ComponentProductId` (ID da **Categoria** "Coberturas"), `Quantity`: 1.
    -   *(Nota: A implementação exata de como `ProductComponent` aponta para uma categoria precisa ser detalhada, mas o conceito é este).*
-   **Tabela de Apoio:** `ProductComponent`
    -   Esta tabela faz a ligação: "Qual produto composto (`CompositeProductId`) é feito de quais componentes (`ComponentProductId`) e em qual quantidade (`Quantity`)?"
    -   **Exemplo de `ProductComponent`:**
        -   `CompositeProductId`: (ID do "X-Salada")
        -   `ComponentProductId`: (ID do "Pão de Hambúrguer")
        -   `Quantity`: 1

### c) `ProductType.Group` (Grupo de Produtos)

Este é o tipo mais flexível e poderoso. Ele representa um "Kit", "Combo" ou "Pacote Promocional" onde o cliente final (ou o atendente) pode fazer escolhas e customizações. O `Product` do tipo `Group` funciona como um container de regras.

-   **Definição:** Um produto vendável que agrupa um conjunto de outros produtos e/ou opções, permitindo flexibilidade na montagem final.
-   **Exemplos:**
    -   "Monte seu Açaí"
    -   "Combo Almoço Executivo (Prato + Bebida + Sobremesa)"
    -   "Kit Festa (Bolo + Salgados + Doces)"
-   **Tabelas de Apoio:** A mágica do `ProductGroup` acontece em suas tabelas relacionadas:

    #### `ProductGroupItem` - O Coração Flexível do Grupo
    Define os "slots" ou itens que compõem o grupo. Sua principal característica é a capacidade de apontar para um produto específico **OU** para uma categoria de produtos.

    -   **Cenário 1: Item Fixo e Obrigatório**
        -   **Como funciona:** O campo `ProductId` é preenchido.
        -   **Exemplo:** No "Combo Almoço Executivo", o item "Arroz e Feijão" é obrigatório.
        -   **Registro no `ProductGroupItem`:**
            -   `ProductGroupId`: (ID do "Combo Almoço")
            -   `ProductId`: (ID do produto "Arroz e Feijão")
            -   `ProductCategoryId`: `NULL`
            -   `IsOptional`: `false`

    -   **Cenário 2: Escolha Dentro de uma Categoria**
        -   **Como funciona:** O campo `ProductCategoryId` é preenchido.
        -   **Exemplo:** No "Combo Almoço Executivo", o cliente pode escolher uma bebida da categoria "Refrigerantes em Lata".
        -   **Registro no `ProductGroupItem`:**
            -   `ProductGroupId`: (ID do "Combo Almoço")
            -   `ProductId`: `NULL`
            -   `ProductCategoryId`: (ID da categoria "Refrigerantes em Lata")
            -   `IsOptional`: `false`
            -   `MinQuantity`: 1, `MaxQuantity`: 1 (O cliente deve escolher exatamente 1 item desta categoria).

    #### `ProductGroupOption` - Customizações Adicionais
    Define opções que não são necessariamente produtos, como "ponto da carne" ou "sem cebola".

    -   **Exemplo:** Para um `ProductGroup` "Monte seu Sanduíche", podemos ter uma opção:
        -   `Name`: "Adicionais"
        -   `OptionType`: `MultipleChoice`
        -   `IsRequired`: `false`
        -   Os valores desta opção ("Bacon Extra", "Queijo Extra") seriam gerenciados em outra tabela (`ProductGroupOptionValue`, por exemplo).

    #### `ProductGroupExchangeRule` - Regras de Troca
    Permite substituir um item padrão do grupo por outro, geralmente com um custo adicional.

    -   **Exemplo:** No "Combo Almoço Executivo", a bebida padrão é um refrigerante. O cliente pode trocá-la por um suco.
        -   `OriginalProductId`: (ID do "Refrigerante Lata")
        -   `ExchangeProductId`: (ID do "Suco Natural de Laranja")
        -   `AdditionalCost`: `R$ 3,00`

---

## 3. Cenário Prático: "Combo Super Burger"

Vamos modelar um combo para ilustrar a arquitetura.

1.  **Criação do Produto Principal:**
    -   Criar um `Product` chamado "Combo Super Burger".
    -   `ProductType` = `Group`.
    -   `BasePrice` = `R$ 35,00`.

2.  **Definição dos Itens do Grupo (`ProductGroupItem`):**

    -   **Item 1 (Hambúrguer - Fixo):**
        -   `ProductId`: Aponta para o `Product` "Super Burger" (que é do tipo `Composite`).
        -   `ProductCategoryId`: `NULL`.
        -   `Quantity`: 1, `IsOptional`: `false`.

    -   **Item 2 (Acompanhamento - Escolha):**
        -   `ProductId`: `NULL`.
        -   `ProductCategoryId`: Aponta para a `ProductCategory` "Acompanhamentos".
        -   `Quantity`: 1, `IsOptional`: `false`.
        -   *O sistema permitirá que o atendente escolha entre "Batata Frita M", "Anéis de Cebola P", etc., que pertencem a essa categoria.*

    -   **Item 3 (Bebida - Escolha):**
        -   `ProductId`: `NULL`.
        -   `ProductCategoryId`: Aponta para a `ProductCategory` "Bebidas Não Alcoólicas".
        -   `Quantity`: 1, `IsOptional`: `false`.

3.  **Definição de uma Regra de Troca (`ProductGroupExchangeRule`):**

    -   **Trocar Batata por Salada:**
        -   `OriginalProductId`: (ID da "Batata Frita M").
        -   `ExchangeProductId`: (ID da "Salada Simples").
        -   `AdditionalCost`: `R$ 0,00`.

4.  **Definição de uma Opção (`ProductGroupOption`):**

    -   **Ponto da Carne:**
        -   `Name`: "Ponto da Carne do Super Burger"
        -   `OptionType`: `SingleChoice` (com valores "Mal Passado", "Ao Ponto", "Bem Passado").
        -   `IsRequired`: `true`.

Com esta estrutura, o GesN pode lidar com uma vasta gama de cenários de venda, desde o mais simples ao mais complexo, de forma coesa e escalável.

