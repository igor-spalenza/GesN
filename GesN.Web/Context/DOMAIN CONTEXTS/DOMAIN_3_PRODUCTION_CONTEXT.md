# Descrição Funcional de Software - Domínio de Produção

## 1. Visão Geral

O **Domínio de Produção** é o centro de execução do GesN, responsável por traduzir os pedidos de venda confirmados em tarefas de produção tangíveis e rastreáveis. Ele funciona como a "esteira de produção" do negócio, garantindo que os produtos, especialmente os personalizados (`Composite`), sejam montados corretamente e dentro do prazo estipulado.

Este domínio é ativado principalmente pelo **Domínio de Vendas**. Quando um `OrderEntry` (Pedido) contendo itens que exigem fabricação é confirmado, uma ou mais `Demand` (Demandas de Produção) são geradas automaticamente, iniciando o fluxo de trabalho da produção.

- **Integração com Vendas**: A confirmação de um `OrderItem` de um produto composto ou de fabricação interna dispara a criação de uma `Demand`. A data de entrega do pedido (`Order.DeliveryDate`) define o prazo para a produção.
- **Integração com Produtos**: A produção consome as "receitas" e "regras de montagem" definidas no Domínio de Produto. As `ProductComponentHierarchy` e os `ProductComponent` de um produto composto se transformam na lista de tarefas (`ProductComposition`) de uma demanda.
- **Integração com Estoque/Compras**: A conclusão das demandas informa o consumo de `Ingredient` (ingredientes/insumos), permitindo que o sistema atualize os níveis de estoque e auxilie na geração de novas ordens de compra.

O objetivo principal é fornecer à equipe de produção uma visão clara e organizada do que precisa ser feito, em que ordem e para quando, ao mesmo tempo que oferece aos gestores visibilidade sobre o andamento e a capacidade da produção.

## 2. Entidades Principais

As seguintes entidades são fundamentais para o funcionamento do Domínio de Produção:

- **`Demand`**: A entidade central, representando uma ordem de produção. Geralmente, uma `Demand` está ligada a um `OrderItem` específico de um pedido de venda. Ela agrega todas as informações necessárias para a produção: o produto final a ser montado, a quantidade, o prazo de entrega e o seu status geral no fluxo de produção.
- **`ProductComposition`**: Representa uma tarefa ou um componente específico dentro de uma `Demand`. É a unidade de trabalho da produção. Se uma demanda é para "1 Bolo de Aniversário", haverá registros de `ProductComposition` para "Massa Branca", "Recheio de Ninho", "Cobertura de Chocolate", etc., cada um com sua própria quantidade e status de processamento.
- **`Product` (Entidade do Domínio de Produto)**: A especificação do item a ser produzido. A produção consulta as propriedades do produto, como `AssemblyTime` e `AssemblyInstructions`.
- **`ProductComponent` (Entidade do Domínio de Produto)**: A especificação do componente a ser usado em uma tarefa `ProductComposition`.

## 3. Jornada do Usuário e Fluxos de Trabalho

O fluxo de produção é projetado para ser um processo gerenciado por status, desde o recebimento da demanda até a sua conclusão.

### 3.1. Painel de Demandas (Production Dashboard)

O ponto de entrada para a equipe de produção é o painel de demandas (`Demand/Index.cshtml`). Esta tela centraliza o gerenciamento e oferece:

- **Visão Geral com Status**: Cards de resumo exibem a quantidade de demandas em cada estágio do processo: `Pendente`, `Confirmado`, `Em Produção`, `Finalizando`, `Entregue` e `Atrasado`.
- **Grade de Demandas**: Uma lista detalhada de todas as demandas, mostrando informações cruciais como o produto, quantidade, cliente (via pedido), data de entrega e status atual.
- **Filtros Avançados**: Ferramentas poderosas para que o gerente de produção possa focar no que é mais importante, filtrando por:
    - Status da demanda.
    - Produto específico.
    - Período de entrega (data inicial e final).
    - Apenas demandas atrasadas.

### 3.2. Geração da Demanda

- **Automática (Fluxo Padrão)**: Ao confirmar um `OrderEntry` no Domínio de Vendas, o sistema analisa cada `OrderItem`. Para itens que são `ProductType.Composite` ou que necessitam de produção, uma `Demand` é criada automaticamente. As escolhas feitas pelo cliente (ex: recheios do bolo) são transformadas em registros de `ProductComposition` vinculados a essa nova `Demand`.
- **Manual**: A interface permite a criação de uma "Nova Demanda" manual. Isso é útil para ordens de produção internas, produção para estoque (sem um pedido de cliente atrelado) ou para corrigir falhas no processo automático.

### 3.3. Gerenciamento e Execução de uma Demanda

1.  **Análise e Confirmação**: Uma nova demanda entra no painel com status `Pendente`. O gerente de produção a revisa e a move para `Confirmado`, sinalizando que ela está pronta para ser iniciada.

2.  **Início da Produção**: A equipe seleciona uma demanda `Confirmada` e altera seu status para `Em Produção`.

3.  **Execução das Tarefas (`ProductComposition`)**: O detalhe da demanda exibe a lista de todos os seus itens de `ProductComposition`. A equipe de produção executa cada tarefa:
    - O sistema permite marcar o início e o fim do processamento de cada componente (`StartProcessing()`, `CompleteProcessing()`).
    - Isso oferece um rastreamento granular do progresso. É possível saber, por exemplo, que a "massa" e o "recheio" de um bolo já estão prontos, mas a "cobertura" ainda está pendente.

4.  **Finalização**: Uma vez que todos os itens de `ProductComposition` de uma demanda são marcados como `Completed`, a demanda principal pode ter seu status alterado para `Finalizando` (para etapas de embalagem, por exemplo) e, posteriormente, para `Pronto para Entrega`.

5.  **Entrega**: Quando o produto é efetivamente entregue ou retirado (informação que pode vir do Domínio de Vendas), o status da demanda é atualizado para `Entregue`, concluindo seu ciclo de vida na produção.

## 4. Regras de Negócio e Status

O ciclo de vida de uma `Demand` é governado por uma máquina de estados que reflete o processo físico na fábrica.

- **Status da Demanda**:
    - **Pendente**: Recém-criada, aguardando revisão da produção.
    - **Confirmado**: Revisada e apta para iniciar a produção.
    - **Em Produção**: O trabalho na demanda foi iniciado.
    - **Finalizando**: Todos os componentes foram produzidos; em fase de montagem final/embalagem.
    - **Pronto para Entrega**: Produção concluída e aguardando a logística.
    - **Entregue**: Ciclo de produção finalizado.
    - **Cancelado**: A demanda foi cancelada (geralmente devido ao cancelamento do pedido de venda).
    - **Atrasado**: Um estado de alerta, não um status de fluxo. Uma demanda é considerada atrasada se a data atual ultrapassar a data de entrega e ela ainda não estiver `Pronta para Entrega` ou `Entregue`.

- **Regras de Validação**:
    - Uma `Demand` não pode ser movida para `Em Produção` sem antes ser `Confirmada`.
    - O status de uma `Demand` só pode ser alterado para `Pronto para Entrega` se todos os seus `ProductComposition`s estiverem concluídos.
    - A criação de uma `ProductComposition` requer um `DemandId`, um `ProductComponentId` e o nome da hierarquia (`HierarchyName`) para manter o contexto, mesmo que a hierarquia original seja alterada no futuro.

## 5. Conclusão

O Domínio de Produção é o elo vital que transforma a promessa de uma venda em um produto real. Ele organiza o caos da "cozinha" ou "fábrica", fornecendo um fluxo de trabalho estruturado, rastreabilidade de ponta a ponta e dados valiosos para a gestão de capacidade e eficiência operacional. A sua integração direta com Vendas e Produtos garante que a produção esteja sempre alinhada com a demanda do cliente e com as especificações do catálogo.

