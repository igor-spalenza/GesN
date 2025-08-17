# Descri��o Funcional de Software - Dom�nio de Produ��o

## 1. Vis�o Geral

O **Dom�nio de Produ��o** � o centro de execu��o do GesN, respons�vel por traduzir os pedidos de venda confirmados em tarefas de produ��o tang�veis e rastre�veis. Ele funciona como a "esteira de produ��o" do neg�cio, garantindo que os produtos, especialmente os personalizados (`Composite`), sejam montados corretamente e dentro do prazo estipulado.

Este dom�nio � ativado principalmente pelo **Dom�nio de Vendas**. Quando um `OrderEntry` (Pedido) contendo itens que exigem fabrica��o � confirmado, uma ou mais `Demand` (Demandas de Produ��o) s�o geradas automaticamente, iniciando o fluxo de trabalho da produ��o.

- **Integra��o com Vendas**: A confirma��o de um `OrderItem` de um produto composto ou de fabrica��o interna dispara a cria��o de uma `Demand`. A data de entrega do pedido (`Order.DeliveryDate`) define o prazo para a produ��o.
- **Integra��o com Produtos**: A produ��o consome as "receitas" e "regras de montagem" definidas no Dom�nio de Produto. As `ProductComponentHierarchy` e os `ProductComponent` de um produto composto se transformam na lista de tarefas (`ProductComposition`) de uma demanda.
- **Integra��o com Estoque/Compras**: A conclus�o das demandas informa o consumo de `Ingredient` (ingredientes/insumos), permitindo que o sistema atualize os n�veis de estoque e auxilie na gera��o de novas ordens de compra.

O objetivo principal � fornecer � equipe de produ��o uma vis�o clara e organizada do que precisa ser feito, em que ordem e para quando, ao mesmo tempo que oferece aos gestores visibilidade sobre o andamento e a capacidade da produ��o.

## 2. Entidades Principais

As seguintes entidades s�o fundamentais para o funcionamento do Dom�nio de Produ��o:

- **`Demand`**: A entidade central, representando uma ordem de produ��o. Geralmente, uma `Demand` est� ligada a um `OrderItem` espec�fico de um pedido de venda. Ela agrega todas as informa��es necess�rias para a produ��o: o produto final a ser montado, a quantidade, o prazo de entrega e o seu status geral no fluxo de produ��o.
- **`ProductComposition`**: Representa uma tarefa ou um componente espec�fico dentro de uma `Demand`. � a unidade de trabalho da produ��o. Se uma demanda � para "1 Bolo de Anivers�rio", haver� registros de `ProductComposition` para "Massa Branca", "Recheio de Ninho", "Cobertura de Chocolate", etc., cada um com sua pr�pria quantidade e status de processamento.
- **`Product` (Entidade do Dom�nio de Produto)**: A especifica��o do item a ser produzido. A produ��o consulta as propriedades do produto, como `AssemblyTime` e `AssemblyInstructions`.
- **`ProductComponent` (Entidade do Dom�nio de Produto)**: A especifica��o do componente a ser usado em uma tarefa `ProductComposition`.

## 3. Jornada do Usu�rio e Fluxos de Trabalho

O fluxo de produ��o � projetado para ser um processo gerenciado por status, desde o recebimento da demanda at� a sua conclus�o.

### 3.1. Painel de Demandas (Production Dashboard)

O ponto de entrada para a equipe de produ��o � o painel de demandas (`Demand/Index.cshtml`). Esta tela centraliza o gerenciamento e oferece:

- **Vis�o Geral com Status**: Cards de resumo exibem a quantidade de demandas em cada est�gio do processo: `Pendente`, `Confirmado`, `Em Produ��o`, `Finalizando`, `Entregue` e `Atrasado`.
- **Grade de Demandas**: Uma lista detalhada de todas as demandas, mostrando informa��es cruciais como o produto, quantidade, cliente (via pedido), data de entrega e status atual.
- **Filtros Avan�ados**: Ferramentas poderosas para que o gerente de produ��o possa focar no que � mais importante, filtrando por:
    - Status da demanda.
    - Produto espec�fico.
    - Per�odo de entrega (data inicial e final).
    - Apenas demandas atrasadas.

### 3.2. Gera��o da Demanda

- **Autom�tica (Fluxo Padr�o)**: Ao confirmar um `OrderEntry` no Dom�nio de Vendas, o sistema analisa cada `OrderItem`. Para itens que s�o `ProductType.Composite` ou que necessitam de produ��o, uma `Demand` � criada automaticamente. As escolhas feitas pelo cliente (ex: recheios do bolo) s�o transformadas em registros de `ProductComposition` vinculados a essa nova `Demand`.
- **Manual**: A interface permite a cria��o de uma "Nova Demanda" manual. Isso � �til para ordens de produ��o internas, produ��o para estoque (sem um pedido de cliente atrelado) ou para corrigir falhas no processo autom�tico.

### 3.3. Gerenciamento e Execu��o de uma Demanda

1.  **An�lise e Confirma��o**: Uma nova demanda entra no painel com status `Pendente`. O gerente de produ��o a revisa e a move para `Confirmado`, sinalizando que ela est� pronta para ser iniciada.

2.  **In�cio da Produ��o**: A equipe seleciona uma demanda `Confirmada` e altera seu status para `Em Produ��o`.

3.  **Execu��o das Tarefas (`ProductComposition`)**: O detalhe da demanda exibe a lista de todos os seus itens de `ProductComposition`. A equipe de produ��o executa cada tarefa:
    - O sistema permite marcar o in�cio e o fim do processamento de cada componente (`StartProcessing()`, `CompleteProcessing()`).
    - Isso oferece um rastreamento granular do progresso. � poss�vel saber, por exemplo, que a "massa" e o "recheio" de um bolo j� est�o prontos, mas a "cobertura" ainda est� pendente.

4.  **Finaliza��o**: Uma vez que todos os itens de `ProductComposition` de uma demanda s�o marcados como `Completed`, a demanda principal pode ter seu status alterado para `Finalizando` (para etapas de embalagem, por exemplo) e, posteriormente, para `Pronto para Entrega`.

5.  **Entrega**: Quando o produto � efetivamente entregue ou retirado (informa��o que pode vir do Dom�nio de Vendas), o status da demanda � atualizado para `Entregue`, concluindo seu ciclo de vida na produ��o.

## 4. Regras de Neg�cio e Status

O ciclo de vida de uma `Demand` � governado por uma m�quina de estados que reflete o processo f�sico na f�brica.

- **Status da Demanda**:
    - **Pendente**: Rec�m-criada, aguardando revis�o da produ��o.
    - **Confirmado**: Revisada e apta para iniciar a produ��o.
    - **Em Produ��o**: O trabalho na demanda foi iniciado.
    - **Finalizando**: Todos os componentes foram produzidos; em fase de montagem final/embalagem.
    - **Pronto para Entrega**: Produ��o conclu�da e aguardando a log�stica.
    - **Entregue**: Ciclo de produ��o finalizado.
    - **Cancelado**: A demanda foi cancelada (geralmente devido ao cancelamento do pedido de venda).
    - **Atrasado**: Um estado de alerta, n�o um status de fluxo. Uma demanda � considerada atrasada se a data atual ultrapassar a data de entrega e ela ainda n�o estiver `Pronta para Entrega` ou `Entregue`.

- **Regras de Valida��o**:
    - Uma `Demand` n�o pode ser movida para `Em Produ��o` sem antes ser `Confirmada`.
    - O status de uma `Demand` s� pode ser alterado para `Pronto para Entrega` se todos os seus `ProductComposition`s estiverem conclu�dos.
    - A cria��o de uma `ProductComposition` requer um `DemandId`, um `ProductComponentId` e o nome da hierarquia (`HierarchyName`) para manter o contexto, mesmo que a hierarquia original seja alterada no futuro.

## 5. Conclus�o

O Dom�nio de Produ��o � o elo vital que transforma a promessa de uma venda em um produto real. Ele organiza o caos da "cozinha" ou "f�brica", fornecendo um fluxo de trabalho estruturado, rastreabilidade de ponta a ponta e dados valiosos para a gest�o de capacidade e efici�ncia operacional. A sua integra��o direta com Vendas e Produtos garante que a produ��o esteja sempre alinhada com a demanda do cliente e com as especifica��es do cat�logo.

