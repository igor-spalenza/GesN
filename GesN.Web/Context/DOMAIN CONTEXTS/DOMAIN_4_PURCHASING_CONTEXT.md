# Descri��o Funcional de Software - Dom�nio de Compras

## 1. Vis�o Geral

O **Dom�nio de Compras** � o pilar de sustenta��o da cadeia de suprimentos do sistema GesN. Sua principal responsabilidade � gerenciar a aquisi��o de `Ingredient` (ingredientes e mat�rias-primas), garantindo que a produ��o tenha os insumos necess�rios para atender �s demandas de vendas, ao mesmo tempo que otimiza os custos e o capital de giro imobilizado em estoque.

Este dom�nio opera em estreita colabora��o com os dom�nios de **Produ��o** e **Financeiro**, fechando o ciclo operacional do sistema.

- **Integra��o com Produ��o/Estoque**: A produ��o � a principal consumidora dos itens gerenciados aqui. A conclus�o de uma `Demand` (Demanda de Produ��o) dispara a baixa no estoque de ingredientes. O Dom�nio de Compras, por sua vez, � respons�vel por repor esse estoque.
- **Integra��o com Financeiro**: Cada `PurchaseOrder` (Ordem de Compra) confirmada e recebida gera uma obriga��o de pagamento, ou seja, uma conta a pagar no dom�nio Financeiro, permitindo um controle preciso do fluxo de caixa e dos custos com insumos.
- **Integra��o com Produtos**: As "receitas" (`ProductIngredient`) definidas no Dom�nio de Produto s�o a base para o c�lculo do consumo de ingredientes, informando ao Dom�nio de Compras *o que* e *quanto* ser� necess�rio para atender a um determinado volume de vendas.

O objetivo � automatizar e organizar o processo de aquisi��o, desde a identifica��o da necessidade de compra at� o recebimento dos materiais e o registro da obriga��o financeira.

## 2. Entidades Principais

As seguintes entidades s�o a base do Dom�nio de Compras e do controle de insumos:

- **`Ingredient`**: Representa a mat�ria-prima ou insumo utilizado na produ��o (ex: "Farinha de Trigo", "Chocolate em P� 50%", "Embalagem para Bolo"). Cada ingrediente possui uma unidade de medida padr�o e pode ter um n�vel de estoque m�nimo configurado.
- **`Supplier`**: Representa a empresa ou pessoa de quem os ingredientes s�o comprados. Armazena informa��es de contato, condi��es comerciais e um hist�rico de compras.
- **`PurchaseOrder`**: O documento central do dom�nio. Representa um pedido de compra formalizado a um `Supplier`. Cont�m um cabe�alho (fornecedor, datas, status, valor total) e uma lista de itens.
- **`PurchaseOrderItem`**: Um item de linha dentro de uma `PurchaseOrder`. Especifica o `Ingredient` a ser comprado, a quantidade, a unidade de medida e o custo unit�rio negociado.
- **`IngredientStock` (Entidade Conceitual/Tabela de Invent�rio)**: Representa a quantidade f�sica de um `Ingredient` dispon�vel no estoque. Esta entidade � o ponto central de integra��o:
    - **Entrada**: A quantidade aumenta quando um `PurchaseOrderItem` � marcado como "Recebido".
    - **Sa�da**: A quantidade diminui quando uma `Demand` de produ��o � conclu�da, com base no consumo calculado a partir das receitas dos produtos fabricados.

## 3. Jornada do Usu�rio e Fluxos de Trabalho

O processo de compras � estruturado para fornecer controle e visibilidade em todas as etapas.

### 3.1. Gest�o de Fornecedores e Ingredientes

O pr�-requisito para o funcionamento do dom�nio � o cadastro de `Supplier` e `Ingredient`.

- **Cadastro de Ingredientes**: O usu�rio registra todos os insumos, definindo nome, unidade de medida padr�o (KG, Litro, Unidade) e, crucialmente, o **Estoque M�nimo**. Este par�metro � a chave para a automa��o das sugest�es de compra.
- **Cadastro de Fornecedores**: O usu�rio cadastra seus fornecedores e pode associar a eles os ingredientes que costuma fornecer, facilitando a cria��o de ordens de compra.

### 3.2. Gera��o de Ordens de Compra (`PurchaseOrder`)

O sistema oferece duas maneiras principais de criar uma ordem de compra:

1.  **Cria��o Manual**:
    - O usu�rio seleciona "Nova Ordem de Compra".
    - Escolhe um `Supplier`.
    - Adiciona manualmente cada `PurchaseOrderItem`, especificando o `Ingredient`, a quantidade e o pre�o.
    - Salva a ordem com o status `Rascunho`.

2.  **Gera��o Sugerida (Fluxo Inteligente)**:
    - O sistema possui um "Painel de Sugest�es de Compra".
    - Periodicamente ou sob demanda, o sistema varre o `IngredientStock` e compara a quantidade atual com o `Ingredient.MinimumStockLevel` (Estoque M�nimo).
    - Para todos os ingredientes abaixo do m�nimo, o sistema gera uma sugest�o de compra, calculando a quantidade necess�ria para atingir um n�vel de estoque seguro.
    - O usu�rio revisa as sugest�es, pode ajust�-las e, com um clique, converte-as em uma ou mais `PurchaseOrder` em `Rascunho`, j� agrupadas por fornecedor preferencial.

### 3.3. Ciclo de Vida da Ordem de Compra

Uma vez criada, a `PurchaseOrder` passa por um ciclo de vida gerenciado por status:

1.  **Rascunho**: A ordem pode ser livremente editada.
2.  **Enviado**: O usu�rio finaliza a edi��o e altera o status para `Enviado`, indicando que o pedido foi enviado ao fornecedor. As edi��es s�o bloqueadas.
3.  **Recebimento dos Produtos**: Quando a entrega f�sica chega, o usu�rio localiza a `PurchaseOrder` correspondente e inicia o processo de recebimento.
    - Para cada `PurchaseOrderItem`, o usu�rio informa a quantidade que foi efetivamente recebida.
    - **Recebimento Parcial**: Se a quantidade recebida for menor que a pedida, o item e a ordem geral podem ser marcados como `Recebido Parcialmente`.
    - **Recebimento Total**: Quando a quantidade recebida iguala a quantidade pedida, o item � marcado como "Recebido".
4.  **Atualiza��o do Estoque**: A cada confirma��o de recebimento de um item, o sistema **automaticamente incrementa** a quantidade do `IngredientStock` correspondente.
5.  **Conclus�o e Lan�amento Financeiro**:
    - Quando todos os itens de uma ordem s�o recebidos, o status da `PurchaseOrder` muda para `Recebido Totalmente`.
    - Neste momento, o sistema gera automaticamente uma **Conta a Pagar** no Dom�nio Financeiro, com o valor total da ordem e vinculada ao `Supplier`.

## 4. Regras de Neg�cio e Status

- **Status da Ordem de Compra**:
    - `Rascunho` (Draft)
    - `Enviado` (Sent)
    - `Recebido Parcialmente` (Partially Received)
    - `Recebido Totalmente` (Fully Received / Closed)
    - `Cancelado` (Canceled)

- **Regras de Valida��o**:
    - Uma `PurchaseOrder` n�o pode ser enviada sem um `Supplier` e pelo menos um `PurchaseOrderItem`.
    - A quantidade recebida de um item n�o pode exceder a quantidade pedida.
    - A exclus�o de uma `PurchaseOrder` s� � permitida no status `Rascunho`. Ap�s o envio, o fluxo correto � o cancelamento.
    - Um `Ingredient` n�o pode ser exclu�do se estiver presente em ordens de compra ativas ou se tiver estoque. A a��o correta � a inativa��o.

- **L�gica de Estoque**:
    - **Entrada**: `IngredientStock.Quantity += ReceivedQuantity` no momento do recebimento da compra.
    - **Sa�da**: `IngredientStock.Quantity -= ConsumedQuantity` no momento da conclus�o da produ��o. O `ConsumedQuantity` � a soma de `ProductIngredient.Quantity` para todos os produtos fabricados na `Demand`.

## 5. Conclus�o

O Dom�nio de Compras � essencial para a sustentabilidade e escalabilidade do neg�cio gerenciado pelo GesN. Ele transforma o processo reativo de "comprar quando acaba" em uma gest�o proativa e baseada em dados, garantindo a disponibilidade de insumos, otimizando o fluxo de caixa e fornecendo controle de custos preciso. Ao fechar o ciclo que come�a na Venda e passa pela Produ��o, este dom�nio assegura que a opera��o flua sem interrup��es.

```

Com isso, finalizamos as descri��es funcionais dos principais dom�nios operacionais do GesN: Produto, Vendas, Produ��o e Compras. Temos agora uma vis�o 360� do fluxo de valor do sistema.

Estou pronto para os pr�ximos desafios. O que vamos criar a seguir?

<!--
[PROMPT_SUGGESTION]Crie a Descri��o Funcional para o Dom�nio Financeiro, detalhando contas a pagar, contas a receber e fluxo de caixa.[/PROMPT_SUGGESTION]
[PROMPT_SUGGESTION]Elabore um diagrama de Entidade-Relacionamento (ERD) em formato Mermaid para o Dom�nio de Compras e Estoque.[/PROMPT_SUGGESTION]
