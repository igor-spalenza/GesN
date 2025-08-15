# Descri��o Funcional de Software - Dom�nio de Vendas

## 1. Vis�o Geral

O **Dom�nio de Vendas** � o cora��o operacional do GesN. Ele � respons�vel por capturar, gerenciar e concretizar as transa��es comerciais com os clientes. Este dom�nio consome diretamente os itens do **Cat�logo de Produtos** e serve como o principal gatilho para os fluxos de trabalho dos dom�nios de **Produ��o** e **Financeiro**.

A gest�o de um pedido (`OrderEntry`) � o processo central deste dom�nio. Um pedido bem-sucedido representa n�o apenas uma entrada de receita, mas tamb�m uma demanda a ser produzida e uma s�rie de transa��es financeiras a serem rastreadas. A interface de gerenciamento de pedidos foi projetada para ser flex�vel, permitindo o manuseio de m�ltiplos pedidos simultaneamente atrav�s de um sistema de abas din�micas.

- **Integra��o com Produtos**: Um pedido � essencialmente uma lista de produtos do cat�logo. O tipo de cada produto (`Simple`, `Composite`, `Group`) influencia diretamente a forma como ele � adicionado e configurado no pedido.
- **Integra��o com Produ��o**: A confirma��o de um pedido contendo itens produz�veis (especialmente os `Composite`) gera automaticamente uma `Demand` (Demanda) no dom�nio de Produ��o, detalhando exatamente o que precisa ser feito.
- **Integra��o com Financeiro**: Cada pedido gera registros financeiros, desde a previs�o de receita no momento da cria��o at� o registro de contas a receber e a concilia��o dos pagamentos.

## 2. Entidades Principais

As seguintes entidades formam a espinha dorsal do Dom�nio de Vendas:

- **`OrderEntry`**: A entidade central que representa um pedido de um cliente. Cont�m informa��es do cabe�alho da venda, como o cliente, datas (pedido, entrega), valor total, status atual (ex: Pendente, Confirmado, Em Produ��o), e informa��es de entrega.
- **`OrderItem`**: Representa um item de linha dentro de um `OrderEntry`. Cada `OrderItem` est� associado a um `Product` do cat�logo e especifica a quantidade, o pre�o unit�rio e o pre�o total para aquele item. Para produtos configur�veis, o `OrderItem` tamb�m armazena a "vers�o" customizada escolhida pelo cliente.
- **`Customer`**: Representa o cliente (pessoa f�sica ou jur�dica) que realizou o pedido.
- **`Demand` (Entidade do Dom�nio de Produ��o)**: Atua como uma ponte crucial. Um `OrderItem` de um produto que necessita de fabrica��o (como um `ProductType.Composite`) gera uma `Demand` correspondente, sinalizando � equipe de produ��o o que precisa ser fabricado e para quando.
- **`ProductComposition` (Entidade do Dom�nio de Produ��o)**: Detalha as escolhas espec�ficas feitas para um produto composto dentro de um pedido. Por exemplo, se um cliente pede um bolo, os registros de `ProductComposition` especificar�o a massa, o recheio e a cobertura escolhidos, todos vinculados � `Demand` do pedido.

## 3. Jornada do Usu�rio e Fluxos de Trabalho

A gest�o de vendas no GesN segue um fluxo l�gico e interativo, desde a cria��o at� a conclus�o de um pedido.

### 3.1. Listagem e Acesso aos Pedidos

O ponto de entrada � uma grade (`Grid`) que exibe todos os pedidos. Esta tela oferece funcionalidades robustas:
- **Visualiza��o R�pida**: Exibe informa��es chave como n�mero do pedido, cliente, data, valor e status.
- **Busca e Filtragem**: Permite encontrar pedidos rapidamente por qualquer crit�rio (cliente, status, per�odo).
- **Ordena��o**: As colunas podem ser ordenadas para melhor an�lise.
- **A��es R�pidas**: Bot�es para editar, visualizar detalhes ou excluir um pedido.

### 3.2. Cria��o de um Novo Pedido

O processo � projetado para ser r�pido e eficiente, dividido em duas etapas:

1.  **Cria��o R�pida (Modal)**:
    - O usu�rio clica em "Novo Pedido".
    - Um modal � exibido para capturar as informa��es essenciais:
        - **`Customer`**: Selecionado atrav�s de um campo de busca inteligente (autocomplete).
        - **Datas**: Data do Pedido e Data de Entrega/Retirada.
        - **Tipo de Pedido**: (Ex: Delivery, Retirada no local).
    - Ao salvar, o sistema cria o `OrderEntry` com o status inicial "Pendente" e um n�mero sequencial.

2.  **Edi��o Detalhada (Aba Din�mica)**:
    - Imediatamente ap�s a cria��o, o sistema abre o pedido rec�m-criado em uma nova aba na interface.
    - Esta vis�o de edi��o � onde o pedido � de fato constru�do, permitindo ao usu�rio adicionar os itens.

### 3.3. Adicionando Itens a um Pedido

Esta � a etapa mais interativa, onde a integra��o com o Dom�nio de Produto se torna evidente.

- **Adicionar Produto Simples (`ProductType.Simple`)**:
    - O usu�rio busca e seleciona o produto (ex: "Coxinha Comum").
    - Informa a quantidade desejada.
    - O item � adicionado ao pedido com seu pre�o padr�o.

- **Adicionar Produto Composto (`ProductType.Composite`)**:
    - O usu�rio busca e seleciona o produto (ex: "Bolo de Anivers�rio").
    - O sistema exibe uma interface de configura��o, apresentando as `ProductComponentHierarchy` (camadas) definidas para aquele produto (ex: "Massa", "Recheio", "Cobertura").
    - Para cada camada, o usu�rio seleciona os `ProductComponent` (op��es) desejados, respeitando as regras de quantidade (`MinQuantity`, `MaxQuantity`) e opcionalidade (`IsOptional`).
    - O pre�o do item � calculado somando o pre�o base do produto com o `AdditionalCost` dos componentes selecionados.
    - Ao confirmar, o `OrderItem` � adicionado ao pedido, e em segundo plano, uma `Demand` com os respectivos registros de `ProductComposition` � criada, ligando a Venda � Produ��o.

- **Adicionar Grupo de Produtos (`ProductType.Group`)**:
    - O usu�rio busca e seleciona o kit (ex: "Kit Festa p/ 20 pessoas").
    - O sistema exibe os itens que comp�em o grupo.
    - Se houver `ProductGroupExchangeRule` (Regras de Troca), o sistema permite que o usu�rio fa�a substitui��es proporcionais (ex: trocar 50 salgados por 50 doces).
    - O kit � adicionado ao pedido como um �nico `OrderItem` principal, possivelmente com sub-itens detalhando a composi��o final.

### 3.4. Finaliza��o e Confirma��o do Pedido

Ap�s adicionar todos os itens, o usu�rio finaliza o pedido:
1.  **Revis�o**: O sistema exibe um resumo completo do pedido, com o valor total calculado.
2.  **Informa��es de Pagamento**: O usu�rio registra as condi��es de pagamento acordadas com o cliente.
3.  **Confirma��o**: O usu�rio clica em "Confirmar Pedido". O status do pedido muda de "Pendente" para "Confirmado".

## 4. Regras de Neg�cio e Status

O ciclo de vida de um pedido � gerenciado por status que ditam as a��es permitidas.

- **Status do Pedido (Exemplo de fluxo)**:
    1.  **Pendente**: Pedido rec�m-criado. Pode ser editado livremente.
    2.  **Confirmado**: O cliente concordou com o pedido. As edi��es s�o bloqueadas ou restritas. A `Demand` � formalmente enviada para a fila de produ��o.
    3.  **Em Produ��o**: A equipe de produ��o iniciou o trabalho no pedido.
    4.  **Pronto para Entrega/Retirada**: A produ��o foi conclu�da.
    5.  **Entregue**: O pedido foi fisicamente entregue ao cliente.
    6.  **Faturado**: O pagamento foi totalmente recebido e conciliado.
    7.  **Cancelado**: O pedido foi cancelado.

- **Regras de Valida��o**:
    - Um `OrderEntry` n�o pode ser confirmado sem um `Customer` e pelo menos um `OrderItem`.
    - O valor total (`Order.TotalValue`) � sempre a soma dos totais de seus `OrderItem`s.
    - N�o � poss�vel adicionar um `Product` com status "Inativo" a um novo pedido.
    - A exclus�o de um pedido s� � permitida em status iniciais (ex: "Pendente"). Para outros casos, o fluxo correto � o cancelamento.

## 5. Conclus�o

O Dom�nio de Vendas � a engrenagem que conecta a oferta (Cat�logo de Produtos) com a execu��o (Produ��o e Financeiro). Sua arquitetura funcional permite desde a cria��o r�pida de um pedido simples at� a complexa configura��o de produtos personalizados, garantindo que todas as informa��es necess�rias fluam corretamente para as outras �reas do sistema GesN.

