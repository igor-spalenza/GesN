# Descrição Funcional de Software - Domínio de Compras

## 1. Visão Geral

O **Domínio de Compras** é o pilar de sustentação da cadeia de suprimentos do sistema GesN. Sua principal responsabilidade é gerenciar a aquisição de `Ingredient` (ingredientes e matérias-primas), garantindo que a produção tenha os insumos necessários para atender às demandas de vendas, ao mesmo tempo que otimiza os custos e o capital de giro imobilizado em estoque.

Este domínio opera em estreita colaboração com os domínios de **Produção** e **Financeiro**, fechando o ciclo operacional do sistema.

- **Integração com Produção/Estoque**: A produção é a principal consumidora dos itens gerenciados aqui. A conclusão de uma `Demand` (Demanda de Produção) dispara a baixa no estoque de ingredientes. O Domínio de Compras, por sua vez, é responsável por repor esse estoque.
- **Integração com Financeiro**: Cada `PurchaseOrder` (Ordem de Compra) confirmada e recebida gera uma obrigação de pagamento, ou seja, uma conta a pagar no domínio Financeiro, permitindo um controle preciso do fluxo de caixa e dos custos com insumos.
- **Integração com Produtos**: As "receitas" (`ProductIngredient`) definidas no Domínio de Produto são a base para o cálculo do consumo de ingredientes, informando ao Domínio de Compras *o que* e *quanto* será necessário para atender a um determinado volume de vendas.

O objetivo é automatizar e organizar o processo de aquisição, desde a identificação da necessidade de compra até o recebimento dos materiais e o registro da obrigação financeira.

## 2. Entidades Principais

As seguintes entidades são a base do Domínio de Compras e do controle de insumos:

- **`Ingredient`**: Representa a matéria-prima ou insumo utilizado na produção (ex: "Farinha de Trigo", "Chocolate em Pó 50%", "Embalagem para Bolo"). Cada ingrediente possui uma unidade de medida padrão e pode ter um nível de estoque mínimo configurado.
- **`Supplier`**: Representa a empresa ou pessoa de quem os ingredientes são comprados. Armazena informações de contato, condições comerciais e um histórico de compras.
- **`PurchaseOrder`**: O documento central do domínio. Representa um pedido de compra formalizado a um `Supplier`. Contém um cabeçalho (fornecedor, datas, status, valor total) e uma lista de itens.
- **`PurchaseOrderItem`**: Um item de linha dentro de uma `PurchaseOrder`. Especifica o `Ingredient` a ser comprado, a quantidade, a unidade de medida e o custo unitário negociado.
- **`IngredientStock` (Entidade Conceitual/Tabela de Inventário)**: Representa a quantidade física de um `Ingredient` disponível no estoque. Esta entidade é o ponto central de integração:
    - **Entrada**: A quantidade aumenta quando um `PurchaseOrderItem` é marcado como "Recebido".
    - **Saída**: A quantidade diminui quando uma `Demand` de produção é concluída, com base no consumo calculado a partir das receitas dos produtos fabricados.

## 3. Jornada do Usuário e Fluxos de Trabalho

O processo de compras é estruturado para fornecer controle e visibilidade em todas as etapas.

### 3.1. Gestão de Fornecedores e Ingredientes

O pré-requisito para o funcionamento do domínio é o cadastro de `Supplier` e `Ingredient`.

- **Cadastro de Ingredientes**: O usuário registra todos os insumos, definindo nome, unidade de medida padrão (KG, Litro, Unidade) e, crucialmente, o **Estoque Mínimo**. Este parâmetro é a chave para a automação das sugestões de compra.
- **Cadastro de Fornecedores**: O usuário cadastra seus fornecedores e pode associar a eles os ingredientes que costuma fornecer, facilitando a criação de ordens de compra.

### 3.2. Geração de Ordens de Compra (`PurchaseOrder`)

O sistema oferece duas maneiras principais de criar uma ordem de compra:

1.  **Criação Manual**:
    - O usuário seleciona "Nova Ordem de Compra".
    - Escolhe um `Supplier`.
    - Adiciona manualmente cada `PurchaseOrderItem`, especificando o `Ingredient`, a quantidade e o preço.
    - Salva a ordem com o status `Rascunho`.

2.  **Geração Sugerida (Fluxo Inteligente)**:
    - O sistema possui um "Painel de Sugestões de Compra".
    - Periodicamente ou sob demanda, o sistema varre o `IngredientStock` e compara a quantidade atual com o `Ingredient.MinimumStockLevel` (Estoque Mínimo).
    - Para todos os ingredientes abaixo do mínimo, o sistema gera uma sugestão de compra, calculando a quantidade necessária para atingir um nível de estoque seguro.
    - O usuário revisa as sugestões, pode ajustá-las e, com um clique, converte-as em uma ou mais `PurchaseOrder` em `Rascunho`, já agrupadas por fornecedor preferencial.

### 3.3. Ciclo de Vida da Ordem de Compra

Uma vez criada, a `PurchaseOrder` passa por um ciclo de vida gerenciado por status:

1.  **Rascunho**: A ordem pode ser livremente editada.
2.  **Enviado**: O usuário finaliza a edição e altera o status para `Enviado`, indicando que o pedido foi enviado ao fornecedor. As edições são bloqueadas.
3.  **Recebimento dos Produtos**: Quando a entrega física chega, o usuário localiza a `PurchaseOrder` correspondente e inicia o processo de recebimento.
    - Para cada `PurchaseOrderItem`, o usuário informa a quantidade que foi efetivamente recebida.
    - **Recebimento Parcial**: Se a quantidade recebida for menor que a pedida, o item e a ordem geral podem ser marcados como `Recebido Parcialmente`.
    - **Recebimento Total**: Quando a quantidade recebida iguala a quantidade pedida, o item é marcado como "Recebido".
4.  **Atualização do Estoque**: A cada confirmação de recebimento de um item, o sistema **automaticamente incrementa** a quantidade do `IngredientStock` correspondente.
5.  **Conclusão e Lançamento Financeiro**:
    - Quando todos os itens de uma ordem são recebidos, o status da `PurchaseOrder` muda para `Recebido Totalmente`.
    - Neste momento, o sistema gera automaticamente uma **Conta a Pagar** no Domínio Financeiro, com o valor total da ordem e vinculada ao `Supplier`.

## 4. Regras de Negócio e Status

- **Status da Ordem de Compra**:
    - `Rascunho` (Draft)
    - `Enviado` (Sent)
    - `Recebido Parcialmente` (Partially Received)
    - `Recebido Totalmente` (Fully Received / Closed)
    - `Cancelado` (Canceled)

- **Regras de Validação**:
    - Uma `PurchaseOrder` não pode ser enviada sem um `Supplier` e pelo menos um `PurchaseOrderItem`.
    - A quantidade recebida de um item não pode exceder a quantidade pedida.
    - A exclusão de uma `PurchaseOrder` só é permitida no status `Rascunho`. Após o envio, o fluxo correto é o cancelamento.
    - Um `Ingredient` não pode ser excluído se estiver presente em ordens de compra ativas ou se tiver estoque. A ação correta é a inativação.

- **Lógica de Estoque**:
    - **Entrada**: `IngredientStock.Quantity += ReceivedQuantity` no momento do recebimento da compra.
    - **Saída**: `IngredientStock.Quantity -= ConsumedQuantity` no momento da conclusão da produção. O `ConsumedQuantity` é a soma de `ProductIngredient.Quantity` para todos os produtos fabricados na `Demand`.

## 5. Conclusão

O Domínio de Compras é essencial para a sustentabilidade e escalabilidade do negócio gerenciado pelo GesN. Ele transforma o processo reativo de "comprar quando acaba" em uma gestão proativa e baseada em dados, garantindo a disponibilidade de insumos, otimizando o fluxo de caixa e fornecendo controle de custos preciso. Ao fechar o ciclo que começa na Venda e passa pela Produção, este domínio assegura que a operação flua sem interrupções.

```

Com isso, finalizamos as descrições funcionais dos principais domínios operacionais do GesN: Produto, Vendas, Produção e Compras. Temos agora uma visão 360º do fluxo de valor do sistema.

Estou pronto para os próximos desafios. O que vamos criar a seguir?

<!--
[PROMPT_SUGGESTION]Crie a Descrição Funcional para o Domínio Financeiro, detalhando contas a pagar, contas a receber e fluxo de caixa.[/PROMPT_SUGGESTION]
[PROMPT_SUGGESTION]Elabore um diagrama de Entidade-Relacionamento (ERD) em formato Mermaid para o Domínio de Compras e Estoque.[/PROMPT_SUGGESTION]
