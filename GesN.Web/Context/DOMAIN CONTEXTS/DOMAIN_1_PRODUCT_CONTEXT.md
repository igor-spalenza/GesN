# DescriÃ§Ã£o Funcional de Software - DomÃ­nio de Produto

## 1. VisÃ£o Geral

O **DomÃ­nio de Produto** Ã© o pilar central e o ponto de partida para a utilizaÃ§Ã£o do sistema GesN. Ele Ã© responsÃ¡vel por gerenciar o CatÃ¡logo de Produtos e ServiÃ§os que uma empresa oferece. A correta parametrizaÃ§Ã£o deste domÃ­nio Ã© um prÃ©-requisito fundamental, pois todas as outras Ã¡reas do sistema â€” Vendas, ProduÃ§Ã£o, Financeiro e Compras â€” dependem diretamente dos itens cadastrados aqui.

- **Vendas**: NÃ£o Ã© possÃ­vel registrar um pedido sem que os produtos existam no catÃ¡logo.
- **ProduÃ§Ã£o**: O gerenciamento da produÃ§Ã£o (demandas, ordens de produÃ§Ã£o) estÃ¡ intrinsecamente ligado aos produtos, especialmente os compostos.
- **Financeiro**: As receitas sÃ£o geradas a partir das vendas de produtos, e os custos estÃ£o ligados aos ingredientes e componentes.
- **Compras/Estoque**: O controle de insumos (ingredientes) Ã© baseado nas "receitas" dos produtos vendidos e produzidos.

Portanto, a jornada de onboarding de um novo cliente no GesN invariavelmente comeÃ§a pela estruturaÃ§Ã£o do seu CatÃ¡logo de Produtos.

## 2. Entidades Principais

A seguir, uma descriÃ§Ã£o das principais entidades que compÃµem o DomÃ­nio de Produto:

- **`Product`**: A entidade central. Representa um item vendÃ¡vel no catÃ¡logo, que pode ser um produto fÃ­sico, um serviÃ§o ou um agrupamento.
- **`ProductCategory`**: Agrupa produtos por afinidade para facilitar a organizaÃ§Ã£o e a busca (ex: "Salgados Tradicionais", "Bebidas", "Bolos Especiais"). Um produto pertence a uma categoria.
- **`ProductIngredient`**: Representa a relaÃ§Ã£o entre um `Product` e um `Ingredient` (insumo/matÃ©ria-prima), definindo a "receita" de um produto.
- **`ProductComponentHierarchy`**: Define uma "camada" ou um "tipo de escolha" para um produto composto. Ã‰ um conceito abstrato. (ex: "Massa do Bolo", "Recheio", "Cobertura").
- **`ProductComponent`**: Representa uma opÃ§Ã£o concreta dentro de uma `ProductComponentHierarchy`. (ex: "Massa Branca", "Recheio de Brigadeiro").
- **`CompositeProductXHierarchy`**: Tabela de ligaÃ§Ã£o que define as regras de como uma `ProductComponentHierarchy` se aplica a um `Product` do tipo Composto (quantidade, opcionalidade, ordem de montagem).
- **`ProductGroupItem`** (Entidade Conceitual): Representa um item dentro de um `Product` do tipo Grupo. Pode ser um link para outro `Product` ou para uma `ProductCategory`.
- **`ProductGroupExchangeRule`** (Entidade Conceitual): Define as regras de troca e proporÃ§Ã£o entre `ProductGroupItem`s dentro de um mesmo Grupo.

## 3. Tipos de Produto (`ProductType`)

A flexibilidade do catÃ¡logo Ã© garantida pela existÃªncia de trÃªs tipos de produtos distintos.

### 3.1. `ProductType.Simple` (Produto Simples)

Representa a unidade mais bÃ¡sica e atÃ´mica do catÃ¡logo. Ã‰ um item "concreto", nÃ£o configurÃ¡vel no momento da venda.

- **DescriÃ§Ã£o**: Um produto com preÃ§o e receita definidos, sem variaÃ§Ãµes.
- **Exemplos**: "Coxinha Comum", "Kibe com Catupiry", "Refrigerante Lata 350ml".
- **Estrutura e ParametrizaÃ§Ã£o**:
    - Associa-se a uma `ProductCategory`.
    - Pode ter uma lista de `ProductIngredient` para definir sua receita e auxiliar no cÃ¡lculo de custo e controle de estoque de insumos.
    - NÃ£o requer nenhuma outra entidade para sua configuraÃ§Ã£o.

---

### 3.2. `ProductType.Composite` (Produto Composto)

Representa um produto "montÃ¡vel" ou "personalizÃ¡vel", onde o cliente final pode fazer escolhas a partir de opÃ§Ãµes prÃ©-definidas. TambÃ©m Ã© um item "concreto" e vendÃ¡vel.

- **DescriÃ§Ã£o**: Um produto cujo resultado final depende de uma sÃ©rie de escolhas feitas a partir de um conjunto de componentes disponÃ­veis.
- **Exemplos**: "Bolo de AniversÃ¡rio (10 pessoas)", "ServiÃ§o de Jantar para Eventos", "Pacote Comida di Buteco".
- **Estrutura e ParametrizaÃ§Ã£o**: A configuraÃ§Ã£o de um Produto Composto Ã© um processo de mÃºltiplas etapas:

    1.  **DefiniÃ§Ã£o das Hierarquias (`ProductComponentHierarchy`)**: Primeiro, criam-se as "camadas" de personalizaÃ§Ã£o. Cada hierarquia representa uma pergunta que serÃ¡ feita ao cliente.
        - *Exemplo*: Para um bolo, as hierarquias seriam: "Massa", "Recheio", "Cobertura", "Topo de Bolo".

    2.  **DefiniÃ§Ã£o dos Componentes (`ProductComponent`)**: Para cada hierarquia, criam-se as opÃ§Ãµes de escolha. Cada componente Ã© uma resposta possÃ­vel para a "pergunta" da hierarquia.
        - *Exemplo (Hierarquia "Recheio")*: Componentes "Recheio de Brigadeiro", "Recheio de Ninho", "Recheio de Morango".
        - Um `ProductComponent` pode ter um `AdditionalCost` (Custo Adicional), que serÃ¡ somado ao preÃ§o final do produto caso ele seja escolhido.

    3.  **ConfiguraÃ§Ã£o do Produto Composto (`CompositeProductXHierarchy`)**: Na ediÃ§Ã£o do `Product` do tipo `Composite`, o usuÃ¡rio associa as hierarquias (`ProductComponentHierarchy`) que farÃ£o parte da composiÃ§Ã£o daquele produto especÃ­fico. Nesta associaÃ§Ã£o, sÃ£o definidas as regras:
        - **`MinQuantity` / `MaxQuantity`**: Define quantas escolhas o cliente pode/deve fazer para aquela hierarquia. (Ex: Para um bolo, pode-se escolher no mÃ­nimo 1 e no mÃ¡ximo 2 recheios).
        - **`IsOptional`**: Indica se a escolha desta camada Ã© opcional. (Ex: "Topo de Bolo" pode ser opcional).
        - **`AssemblyOrder`**: Define a ordem em que as escolhas devem ser apresentadas ou produzidas.

O resultado Ã© um produto altamente flexÃ­vel que permite ao cliente montar o item final de acordo com suas preferÃªncias, dentro das regras estabelecidas pelo negÃ³cio.

---

### 3.3. `ProductType.Group` (Grupo de Produtos)

Representa um "kit" ou "combo", que Ã© um agrupamento de outros produtos ou categorias de produtos. Ã‰ um objeto "abstrato" no sentido de que seu conteÃºdo Ã© uma coleÃ§Ã£o de outros itens do catÃ¡logo.

- **DescriÃ§Ã£o**: Um pacote que agrupa mÃºltiplos produtos e/ou categorias de produtos, frequentemente com um preÃ§o promocional e regras de troca flexÃ­veis.
- **Exemplos**: "Kit Festa p/ 20 pessoas", "Kit Festa na Caixa", "Combo Casal".
- **Estrutura e ParametrizaÃ§Ã£o**:

    1.  **DefiniÃ§Ã£o dos Itens do Grupo (`ProductGroupItem`)**: Ao criar/editar um `Product` do tipo `Group`, o usuÃ¡rio adiciona os itens que compÃµem o kit. Um item pode ser:
        - **Um link para um `Product` especÃ­fico**: Adiciona uma quantidade especÃ­fica de outro produto (Simples ou Composto).
            - *Exemplo*: "1 unidade de Bolo p/ 20 pessoas".
        - **Um link para uma `ProductCategory`**: Adiciona uma quantidade de itens de uma categoria inteira, permitindo que a escolha final seja feita posteriormente.
            - *Exemplo*: "100 unidades de Salgados Tradicionais".

    2.  **DefiniÃ§Ã£o das Regras de Troca e ProporÃ§Ã£o (`ProductGroupExchangeRule`)**: Este Ã© o grande diferencial do Grupo de Produtos. Permite criar regras de equivalÃªncia entre os itens do grupo.
        - **DescriÃ§Ã£o**: Uma regra define que `X` unidades de um `ProductGroupItem` podem ser trocadas por `Y` unidades de outro `ProductGroupItem`.
        - **Exemplo de Regra**: No "Kit Festa p/ 20 pessoas", pode haver uma regra:
            - `100 unidades de Salgados Tradicionais (1) <--> (1) 100 unidades de Doces Tradicionais`
            - Isso significa que o cliente pode optar por levar 100 salgados, 100 doces, ou uma combinaÃ§Ã£o (ex: 50 de cada), mantendo a proporÃ§Ã£o 1 para 1.

Este tipo de produto oferece a mÃ¡xima flexibilidade para a criaÃ§Ã£o de ofertas e combos complexos.

## 4. Regras de NegÃ³cio e ValidaÃ§Ãµes

A anÃ¡lise do cÃ³digo-fonte (`ProductService.cs`, `ProductComponentHierarchyService.cs`) revela as seguintes regras de negÃ³cio implementadas:

- **Unicidade**:
    - O `SKU` de um produto, se informado, deve ser Ãºnico em todo o catÃ¡logo.
    - O `Name` (Nome) de uma `ProductComponentHierarchy` deve ser Ãºnico.

- **Integridade de Dados**:
    - O `Name` (Nome) de um `Product` Ã© obrigatÃ³rio.
    - `UnitPrice` (PreÃ§o) e `Cost` (Custo) de um produto nÃ£o podem ser negativos.
    - `AssemblyTime` (Tempo de Montagem) de um `Product` Composto nÃ£o pode ser negativo.

- **Regras de ExclusÃ£o (Soft Delete)**:
    - Um `Product` nÃ£o pode ser excluÃ­do (`DeleteAsync`) se estiver associado a algum `OrderItem` (item de pedido) jÃ¡ existente. A recomendaÃ§Ã£o Ã© a inativaÃ§Ã£o (`DeactivateAsync`).
    - Uma `ProductComponentHierarchy` nÃ£o pode ser excluÃ­da se estiver sendo utilizada por algum `Product` Composto.

- **CÃ¡lculos de Custo (`CalculateProductCostAsync`)**:
    - Para produtos **Simples**, o custo Ã© o valor base do produto mais o custo calculado de seus ingredientes.
    - Para produtos **Compostos**, o custo total Ã© a soma do custo base do produto mais o `AdditionalCost` de cada `ProductComponent` selecionado em uma composiÃ§Ã£o.

## 5. Jornada do UsuÃ¡rio (Fluxo de Cadastro)

A seguir, um resumo da jornada do usuÃ¡rio para parametrizar o catÃ¡logo.

### 5.1. Cadastro de um Produto Simples
1. Acessar a Ã¡rea de Categorias e garantir que a categoria desejada exista.
2. Acessar a Ã¡rea de Produtos e clicar em "Novo Produto".
3. Preencher os dados bÃ¡sicos (Nome, PreÃ§o, Custo, SKU).
4. Selecionar o `ProductType` como **Simples**.
5. Associar a `ProductCategory` desejada.
6. (Opcional) Acessar a aba "Receita/Ingredientes" e adicionar os `ProductIngredient`s.
7. Salvar o produto.

### 5.2. Cadastro de um Produto Composto
1. Acessar a Ã¡rea de Hierarquias de Componentes e cadastrar todas as "camadas" necessÃ¡rias (ex: Massa, Recheio).
2. Acessar a Ã¡rea de Componentes e cadastrar todas as "opÃ§Ãµes" para cada hierarquia, associando-as corretamente e definindo custos adicionais se houver.
3. Acessar a Ã¡rea de Produtos e clicar em "Novo Produto".
4. Preencher os dados bÃ¡sicos (Nome, PreÃ§o Base, etc.).
5. Selecionar o `ProductType` como **Composto**.
6. Acessar a aba "ComposiÃ§Ã£o" ou "ConfiguraÃ§Ã£o".
7. Adicionar as `ProductComponentHierarchy`s que farÃ£o parte deste produto.
8. Para cada hierarquia adicionada, configurar as regras (Min/Max Quantidade, Opcional, Ordem).
9. Salvar o produto.

### 5.3. Cadastro de um Grupo de Produtos
1. Garantir que todos os produtos e categorias que farÃ£o parte do kit jÃ¡ existam no sistema.
2. Acessar a Ã¡rea de Produtos e clicar em "Novo Produto".
3. Preencher os dados do kit (Nome, PreÃ§o total do kit).
4. Selecionar o `ProductType` como **Grupo**.
5. Acessar a aba "Itens do Grupo".
6. Adicionar os itens, especificando a quantidade e se o item Ã© um `Product` ou uma `ProductCategory`.
7. Acessar a aba "Regras de Troca".
8. (Opcional) Criar as regras de troca e proporÃ§Ã£o entre os itens adicionados.
9. Salvar o produto.

