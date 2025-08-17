# Descrição Funcional de Software - Domínio de Produto

## 1. Visão Geral

O **Domínio de Produto** é o pilar central e o ponto de partida para a utilização do sistema GesN. Ele é responsável por gerenciar o Catálogo de Produtos e Serviços que uma empresa oferece. A correta parametrização deste domínio é um pré-requisito fundamental, pois todas as outras áreas do sistema — Vendas, Produção, Financeiro e Compras — dependem diretamente dos itens cadastrados aqui.

- **Vendas**: Não é possível registrar um pedido sem que os produtos existam no catálogo.
- **Produção**: O gerenciamento da produção (demandas, ordens de produção) está intrinsecamente ligado aos produtos, especialmente os compostos.
- **Financeiro**: As receitas são geradas a partir das vendas de produtos, e os custos estão ligados aos ingredientes e componentes.
- **Compras/Estoque**: O controle de insumos (ingredientes) é baseado nas "receitas" dos produtos vendidos e produzidos.

Portanto, a jornada de onboarding de um novo cliente no GesN invariavelmente começa pela estruturação do seu Catálogo de Produtos.

## 2. Entidades Principais

A seguir, uma descrição das principais entidades que compõem o Domínio de Produto:

- **`Product`**: A entidade central. Representa um item vendável no catálogo, que pode ser um produto físico, um serviço ou um agrupamento.
- **`ProductCategory`**: Agrupa produtos por afinidade para facilitar a organização e a busca (ex: "Salgados Tradicionais", "Bebidas", "Bolos Especiais"). Um produto pertence a uma categoria.
- **`ProductIngredient`**: Representa a relação entre um `Product` e um `Ingredient` (insumo/matéria-prima), definindo a "receita" de um produto.
- **`ProductComponentHierarchy`**: Define uma "camada" ou um "tipo de escolha" para um produto composto. É um conceito abstrato. (ex: "Massa do Bolo", "Recheio", "Cobertura").
- **`ProductComponent`**: Representa uma opção concreta dentro de uma `ProductComponentHierarchy`. (ex: "Massa Branca", "Recheio de Brigadeiro").
- **`CompositeProductXHierarchy`**: Tabela de ligação que define as regras de como uma `ProductComponentHierarchy` se aplica a um `Product` do tipo Composto (quantidade, opcionalidade, ordem de montagem).
- **`ProductGroupItem`** (Entidade Conceitual): Representa um item dentro de um `Product` do tipo Grupo. Pode ser um link para outro `Product` ou para uma `ProductCategory`.
- **`ProductGroupExchangeRule`** (Entidade Conceitual): Define as regras de troca e proporção entre `ProductGroupItem`s dentro de um mesmo Grupo.

## 3. Tipos de Produto (`ProductType`)

A flexibilidade do catálogo é garantida pela existência de três tipos de produtos distintos.

### 3.1. `ProductType.Simple` (Produto Simples)

Representa a unidade mais básica e atômica do catálogo. É um item "concreto", não configurável no momento da venda.

- **Descrição**: Um produto com preço e receita definidos, sem variações.
- **Exemplos**: "Coxinha Comum", "Kibe com Catupiry", "Refrigerante Lata 350ml".
- **Estrutura e Parametrização**:
    - Associa-se a uma `ProductCategory`.
    - Pode ter uma lista de `ProductIngredient` para definir sua receita e auxiliar no cálculo de custo e controle de estoque de insumos.
    - Não requer nenhuma outra entidade para sua configuração.

---

### 3.2. `ProductType.Composite` (Produto Composto)

Representa um produto "montável" ou "personalizável", onde o cliente final pode fazer escolhas a partir de opções pré-definidas. Também é um item "concreto" e vendável.

- **Descrição**: Um produto cujo resultado final depende de uma série de escolhas feitas a partir de um conjunto de componentes disponíveis.
- **Exemplos**: "Bolo de Aniversário (10 pessoas)", "Serviço de Jantar para Eventos", "Pacote Comida di Buteco".
- **Estrutura e Parametrização**: A configuração de um Produto Composto é um processo de múltiplas etapas:

    1.  **Definição das Hierarquias (`ProductComponentHierarchy`)**: Primeiro, criam-se as "camadas" de personalização. Cada hierarquia representa uma pergunta que será feita ao cliente.
        - *Exemplo*: Para um bolo, as hierarquias seriam: "Massa", "Recheio", "Cobertura", "Topo de Bolo".

    2.  **Definição dos Componentes (`ProductComponent`)**: Para cada hierarquia, criam-se as opções de escolha. Cada componente é uma resposta possível para a "pergunta" da hierarquia.
        - *Exemplo (Hierarquia "Recheio")*: Componentes "Recheio de Brigadeiro", "Recheio de Ninho", "Recheio de Morango".
        - Um `ProductComponent` pode ter um `AdditionalCost` (Custo Adicional), que será somado ao preço final do produto caso ele seja escolhido.

    3.  **Configuração do Produto Composto (`CompositeProductXHierarchy`)**: Na edição do `Product` do tipo `Composite`, o usuário associa as hierarquias (`ProductComponentHierarchy`) que farão parte da composição daquele produto específico. Nesta associação, são definidas as regras:
        - **`MinQuantity` / `MaxQuantity`**: Define quantas escolhas o cliente pode/deve fazer para aquela hierarquia. (Ex: Para um bolo, pode-se escolher no mínimo 1 e no máximo 2 recheios).
        - **`IsOptional`**: Indica se a escolha desta camada é opcional. (Ex: "Topo de Bolo" pode ser opcional).
        - **`AssemblyOrder`**: Define a ordem em que as escolhas devem ser apresentadas ou produzidas.

O resultado é um produto altamente flexível que permite ao cliente montar o item final de acordo com suas preferências, dentro das regras estabelecidas pelo negócio.

---

### 3.3. `ProductType.Group` (Grupo de Produtos)

Representa um "kit" ou "combo", que é um agrupamento de outros produtos ou categorias de produtos. É um objeto "abstrato" no sentido de que seu conteúdo é uma coleção de outros itens do catálogo.

- **Descrição**: Um pacote que agrupa múltiplos produtos e/ou categorias de produtos, frequentemente com um preço promocional e regras de troca flexíveis.
- **Exemplos**: "Kit Festa p/ 20 pessoas", "Kit Festa na Caixa", "Combo Casal".
- **Estrutura e Parametrização**:

    1.  **Definição dos Itens do Grupo (`ProductGroupItem`)**: Ao criar/editar um `Product` do tipo `Group`, o usuário adiciona os itens que compõem o kit. Um item pode ser:
        - **Um link para um `Product` específico**: Adiciona uma quantidade específica de outro produto (Simples ou Composto).
            - *Exemplo*: "1 unidade de Bolo p/ 20 pessoas".
        - **Um link para uma `ProductCategory`**: Adiciona uma quantidade de itens de uma categoria inteira, permitindo que a escolha final seja feita posteriormente.
            - *Exemplo*: "100 unidades de Salgados Tradicionais".

    2.  **Definição das Regras de Troca e Proporção (`ProductGroupExchangeRule`)**: Este é o grande diferencial do Grupo de Produtos. Permite criar regras de equivalência entre os itens do grupo.
        - **Descrição**: Uma regra define que `X` unidades de um `ProductGroupItem` podem ser trocadas por `Y` unidades de outro `ProductGroupItem`.
        - **Exemplo de Regra**: No "Kit Festa p/ 20 pessoas", pode haver uma regra:
            - `100 unidades de Salgados Tradicionais (1) <--> (1) 100 unidades de Doces Tradicionais`
            - Isso significa que o cliente pode optar por levar 100 salgados, 100 doces, ou uma combinação (ex: 50 de cada), mantendo a proporção 1 para 1.

Este tipo de produto oferece a máxima flexibilidade para a criação de ofertas e combos complexos.

## 4. Regras de Negócio e Validações

A análise do código-fonte (`ProductService.cs`, `ProductComponentHierarchyService.cs`) revela as seguintes regras de negócio implementadas:

- **Unicidade**:
    - O `SKU` de um produto, se informado, deve ser único em todo o catálogo.
    - O `Name` (Nome) de uma `ProductComponentHierarchy` deve ser único.

- **Integridade de Dados**:
    - O `Name` (Nome) de um `Product` é obrigatório.
    - `UnitPrice` (Preço) e `Cost` (Custo) de um produto não podem ser negativos.
    - `AssemblyTime` (Tempo de Montagem) de um `Product` Composto não pode ser negativo.

- **Regras de Exclusão (Soft Delete)**:
    - Um `Product` não pode ser excluído (`DeleteAsync`) se estiver associado a algum `OrderItem` (item de pedido) já existente. A recomendação é a inativação (`DeactivateAsync`).
    - Uma `ProductComponentHierarchy` não pode ser excluída se estiver sendo utilizada por algum `Product` Composto.

- **Cálculos de Custo (`CalculateProductCostAsync`)**:
    - Para produtos **Simples**, o custo é o valor base do produto mais o custo calculado de seus ingredientes.
    - Para produtos **Compostos**, o custo total é a soma do custo base do produto mais o `AdditionalCost` de cada `ProductComponent` selecionado em uma composição.

## 5. Jornada do Usuário (Fluxo de Cadastro)

A seguir, um resumo da jornada do usuário para parametrizar o catálogo.

### 5.1. Cadastro de um Produto Simples
1. Acessar a área de Categorias e garantir que a categoria desejada exista.
2. Acessar a área de Produtos e clicar em "Novo Produto".
3. Preencher os dados básicos (Nome, Preço, Custo, SKU).
4. Selecionar o `ProductType` como **Simples**.
5. Associar a `ProductCategory` desejada.
6. (Opcional) Acessar a aba "Receita/Ingredientes" e adicionar os `ProductIngredient`s.
7. Salvar o produto.

### 5.2. Cadastro de um Produto Composto
1. Acessar a área de Hierarquias de Componentes e cadastrar todas as "camadas" necessárias (ex: Massa, Recheio).
2. Acessar a área de Componentes e cadastrar todas as "opções" para cada hierarquia, associando-as corretamente e definindo custos adicionais se houver.
3. Acessar a área de Produtos e clicar em "Novo Produto".
4. Preencher os dados básicos (Nome, Preço Base, etc.).
5. Selecionar o `ProductType` como **Composto**.
6. Acessar a aba "Composição" ou "Configuração".
7. Adicionar as `ProductComponentHierarchy`s que farão parte deste produto.
8. Para cada hierarquia adicionada, configurar as regras (Min/Max Quantidade, Opcional, Ordem).
9. Salvar o produto.

### 5.3. Cadastro de um Grupo de Produtos
1. Garantir que todos os produtos e categorias que farão parte do kit já existam no sistema.
2. Acessar a área de Produtos e clicar em "Novo Produto".
3. Preencher os dados do kit (Nome, Preço total do kit).
4. Selecionar o `ProductType` como **Grupo**.
5. Acessar a aba "Itens do Grupo".
6. Adicionar os itens, especificando a quantidade e se o item é um `Product` ou uma `ProductCategory`.
7. Acessar a aba "Regras de Troca".
8. (Opcional) Criar as regras de troca e proporção entre os itens adicionados.
9. Salvar o produto.

