# Descrição Funcional de Software - Domínio de Vendas

## 1. Visão Geral

O **Domínio de Vendas** é o coração operacional do GesN. Ele é responsável por capturar, gerenciar e concretizar as transações comerciais com os clientes. Este domínio consome diretamente os itens do **Catálogo de Produtos** e serve como o principal gatilho para os fluxos de trabalho dos domínios de **Produção** e **Financeiro**.

A gestão de um pedido (`OrderEntry`) é o processo central deste domínio. Um pedido bem-sucedido representa não apenas uma entrada de receita, mas também uma demanda a ser produzida e uma série de transações financeiras a serem rastreadas. A interface de gerenciamento de pedidos foi projetada para ser flexível, permitindo o manuseio de múltiplos pedidos simultaneamente através de um sistema de abas dinâmicas.

- **Integração com Produtos**: Um pedido é essencialmente uma lista de produtos do catálogo. O tipo de cada produto (`Simple`, `Composite`, `Group`) influencia diretamente a forma como ele é adicionado e configurado no pedido.
- **Integração com Produção**: A confirmação de um pedido contendo itens produzíveis (especialmente os `Composite`) gera automaticamente uma `Demand` (Demanda) no domínio de Produção, detalhando exatamente o que precisa ser feito.
- **Integração com Financeiro**: Cada pedido gera registros financeiros, desde a previsão de receita no momento da criação até o registro de contas a receber e a conciliação dos pagamentos.

## 2. Entidades Principais

As seguintes entidades formam a espinha dorsal do Domínio de Vendas:

- **`OrderEntry`**: A entidade central que representa um pedido de um cliente. Contém informações do cabeçalho da venda, como o cliente, datas (pedido, entrega), valor total, status atual (ex: Pendente, Confirmado, Em Produção), e informações de entrega.
- **`OrderItem`**: Representa um item de linha dentro de um `OrderEntry`. Cada `OrderItem` está associado a um `Product` do catálogo e especifica a quantidade, o preço unitário e o preço total para aquele item. Para produtos configuráveis, o `OrderItem` também armazena a "versão" customizada escolhida pelo cliente.
- **`Customer`**: Representa o cliente (pessoa física ou jurídica) que realizou o pedido.
- **`Demand` (Entidade do Domínio de Produção)**: Atua como uma ponte crucial. Um `OrderItem` de um produto que necessita de fabricação (como um `ProductType.Composite`) gera uma `Demand` correspondente, sinalizando à equipe de produção o que precisa ser fabricado e para quando.
- **`ProductComposition` (Entidade do Domínio de Produção)**: Detalha as escolhas específicas feitas para um produto composto dentro de um pedido. Por exemplo, se um cliente pede um bolo, os registros de `ProductComposition` especificarão a massa, o recheio e a cobertura escolhidos, todos vinculados à `Demand` do pedido.

## 3. Jornada do Usuário e Fluxos de Trabalho

A gestão de vendas no GesN segue um fluxo lógico e interativo, desde a criação até a conclusão de um pedido.

### 3.1. Listagem e Acesso aos Pedidos

O ponto de entrada é uma grade (`Grid`) que exibe todos os pedidos. Esta tela oferece funcionalidades robustas:
- **Visualização Rápida**: Exibe informações chave como número do pedido, cliente, data, valor e status.
- **Busca e Filtragem**: Permite encontrar pedidos rapidamente por qualquer critério (cliente, status, período).
- **Ordenação**: As colunas podem ser ordenadas para melhor análise.
- **Ações Rápidas**: Botões para editar, visualizar detalhes ou excluir um pedido.

### 3.2. Criação de um Novo Pedido

O processo é projetado para ser rápido e eficiente, dividido em duas etapas:

1.  **Criação Rápida (Modal)**:
    - O usuário clica em "Novo Pedido".
    - Um modal é exibido para capturar as informações essenciais:
        - **`Customer`**: Selecionado através de um campo de busca inteligente (autocomplete).
        - **Datas**: Data do Pedido e Data de Entrega/Retirada.
        - **Tipo de Pedido**: (Ex: Delivery, Retirada no local).
    - Ao salvar, o sistema cria o `OrderEntry` com o status inicial "Pendente" e um número sequencial.

2.  **Edição Detalhada (Aba Dinâmica)**:
    - Imediatamente após a criação, o sistema abre o pedido recém-criado em uma nova aba na interface.
    - Esta visão de edição é onde o pedido é de fato construído, permitindo ao usuário adicionar os itens.

### 3.3. Adicionando Itens a um Pedido

Esta é a etapa mais interativa, onde a integração com o Domínio de Produto se torna evidente.

- **Adicionar Produto Simples (`ProductType.Simple`)**:
    - O usuário busca e seleciona o produto (ex: "Coxinha Comum").
    - Informa a quantidade desejada.
    - O item é adicionado ao pedido com seu preço padrão.

- **Adicionar Produto Composto (`ProductType.Composite`)**:
    - O usuário busca e seleciona o produto (ex: "Bolo de Aniversário").
    - O sistema exibe uma interface de configuração, apresentando as `ProductComponentHierarchy` (camadas) definidas para aquele produto (ex: "Massa", "Recheio", "Cobertura").
    - Para cada camada, o usuário seleciona os `ProductComponent` (opções) desejados, respeitando as regras de quantidade (`MinQuantity`, `MaxQuantity`) e opcionalidade (`IsOptional`).
    - O preço do item é calculado somando o preço base do produto com o `AdditionalCost` dos componentes selecionados.
    - Ao confirmar, o `OrderItem` é adicionado ao pedido, e em segundo plano, uma `Demand` com os respectivos registros de `ProductComposition` é criada, ligando a Venda à Produção.

- **Adicionar Grupo de Produtos (`ProductType.Group`)**:
    - O usuário busca e seleciona o kit (ex: "Kit Festa p/ 20 pessoas").
    - O sistema exibe os itens que compõem o grupo.
    - Se houver `ProductGroupExchangeRule` (Regras de Troca), o sistema permite que o usuário faça substituições proporcionais (ex: trocar 50 salgados por 50 doces).
    - O kit é adicionado ao pedido como um único `OrderItem` principal, possivelmente com sub-itens detalhando a composição final.

### 3.4. Finalização e Confirmação do Pedido

Após adicionar todos os itens, o usuário finaliza o pedido:
1.  **Revisão**: O sistema exibe um resumo completo do pedido, com o valor total calculado.
2.  **Informações de Pagamento**: O usuário registra as condições de pagamento acordadas com o cliente.
3.  **Confirmação**: O usuário clica em "Confirmar Pedido". O status do pedido muda de "Pendente" para "Confirmado".

## 4. Regras de Negócio e Status

O ciclo de vida de um pedido é gerenciado por status que ditam as ações permitidas.

- **Status do Pedido (Exemplo de fluxo)**:
    1.  **Pendente**: Pedido recém-criado. Pode ser editado livremente.
    2.  **Confirmado**: O cliente concordou com o pedido. As edições são bloqueadas ou restritas. A `Demand` é formalmente enviada para a fila de produção.
    3.  **Em Produção**: A equipe de produção iniciou o trabalho no pedido.
    4.  **Pronto para Entrega/Retirada**: A produção foi concluída.
    5.  **Entregue**: O pedido foi fisicamente entregue ao cliente.
    6.  **Faturado**: O pagamento foi totalmente recebido e conciliado.
    7.  **Cancelado**: O pedido foi cancelado.

- **Regras de Validação**:
    - Um `OrderEntry` não pode ser confirmado sem um `Customer` e pelo menos um `OrderItem`.
    - O valor total (`Order.TotalValue`) é sempre a soma dos totais de seus `OrderItem`s.
    - Não é possível adicionar um `Product` com status "Inativo" a um novo pedido.
    - A exclusão de um pedido só é permitida em status iniciais (ex: "Pendente"). Para outros casos, o fluxo correto é o cancelamento.

## 5. Conclusão

O Domínio de Vendas é a engrenagem que conecta a oferta (Catálogo de Produtos) com a execução (Produção e Financeiro). Sua arquitetura funcional permite desde a criação rápida de um pedido simples até a complexa configuração de produtos personalizados, garantindo que todas as informações necessárias fluam corretamente para as outras áreas do sistema GesN.

