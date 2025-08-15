# Descrição Funcional de Software - Domínio Financeiro

## 1. Visão Geral

O **Domínio Financeiro** é o centro nervoso do sistema GesN, responsável por rastrear, gerenciar e relatar todo o fluxo de dinheiro que entra e sai da empresa. Ele consolida as atividades operacionais dos domínios de **Vendas** e **Compras**, traduzindo-as em registros financeiros claros e acionáveis, como contas a receber e a pagar.

Este domínio é fundamental para a tomada de decisões estratégicas, pois oferece uma visão precisa da saúde financeira do negócio, do fluxo de caixa e da lucratividade.

- **Integração com Vendas**: Cada `OrderEntry` (Pedido) confirmado no Domínio de Vendas gera automaticamente uma ou mais `AccountReceivable` (Contas a Receber), representando a receita a ser coletada dos clientes.
- **Integração com Compras**: Cada `PurchaseOrder` (Ordem de Compra) recebida no Domínio de Compras gera automaticamente uma `AccountPayable` (Conta a Pagar), representando a obrigação financeira com os fornecedores.

O objetivo principal é fornecer ferramentas para um controle financeiro rigoroso, automatizando a criação de lançamentos, facilitando a conciliação de pagamentos e oferecendo relatórios essenciais como o Fluxo de Caixa.

## 2. Entidades Principais

As seguintes entidades formam a estrutura do Domínio Financeiro:

- **`AccountReceivable` (Conta a Receber)**: Representa um valor que a empresa tem o direito de receber de um `Customer` (Cliente). É gerada a partir de um `OrderEntry` e contém informações como valor, data de vencimento e status do pagamento.
- **`AccountPayable` (Conta a Pagar)**: Representa uma obrigação financeira que a empresa tem com um `Supplier` (Fornecedor). É gerada a partir de uma `PurchaseOrder` e contém informações como valor, data de vencimento e status do pagamento.
- **`Transaction` (Transação/Lançamento)**: A entidade mais granular. Representa qualquer movimento de dinheiro, seja uma entrada (crédito) ou uma saída (débito). Cada transação está vinculada a uma conta a receber ou a pagar e registra a data e o valor do pagamento/recebimento efetuado.
- **`CashFlow` (Fluxo de Caixa - Relatório/Visão)**: Não é uma entidade de banco de dados, mas uma visão consolidada gerada a partir das `Transaction`. Apresenta as entradas, saídas e o saldo de caixa em um determinado período.

## 3. Jornada do Usuário e Fluxos de Trabalho

O Domínio Financeiro é organizado em torno de dois fluxos principais: a gestão de entradas (recebíveis) e a gestão de saídas (pagáveis).

### 3.1. Gestão de Contas a Receber

Este fluxo gerencia o dinheiro que entra na empresa.

1.  **Geração Automática**:
    - Quando um `OrderEntry` é **confirmado** no Domínio de Vendas, o sistema cria automaticamente um registro de `AccountReceivable`.
    - Se o pedido tiver condições de pagamento parceladas, o sistema pode gerar múltiplos registros de `AccountReceivable`, cada um com sua respectiva data de vencimento e valor.

2.  **Painel de Contas a Receber**:
    - O usuário acessa uma tela que lista todas as contas a receber.
    - A tela oferece filtros por cliente, período de vencimento, status (`Pendente`, `Pago`, `Vencido`).
    - Indicadores visuais destacam as contas que estão próximas do vencimento ou já vencidas.

3.  **Registro de Recebimento**:
    - Quando um cliente efetua um pagamento, o usuário localiza a `AccountReceivable` correspondente.
    - O usuário clica em "Registrar Recebimento" e informa o valor recebido e a data.
    - O sistema cria uma `Transaction` do tipo **Crédito**.
    - O status da `AccountReceivable` é atualizado:
        - Se o valor recebido for menor que o total, o status muda para `Parcialmente Pago`.
        - Se o valor recebido quitar o saldo, o status muda para `Pago`.

### 3.2. Gestão de Contas a Pagar

Este fluxo gerencia o dinheiro que sai da empresa.

1.  **Geração Automática**:
    - Quando uma `PurchaseOrder` é marcada como **Recebida Totalmente** no Domínio de Compras, o sistema cria automaticamente um registro de `AccountPayable` no valor total da nota, vinculado ao fornecedor.

2.  **Painel de Contas a Pagar**:
    - O usuário acessa uma tela que lista todas as contas a pagar.
    - A tela permite filtrar por fornecedor, período de vencimento e status (`Pendente`, `Paga`, `Vencida`).
    - O sistema alerta sobre as contas com vencimento próximo.

3.  **Registro de Pagamento**:
    - Quando a empresa paga um fornecedor, o usuário localiza a `AccountPayable` correspondente.
    - O usuário clica em "Registrar Pagamento" e informa o valor pago e a data.
    - O sistema cria uma `Transaction` do tipo **Débito**.
    - O status da `AccountPayable` é atualizado para `Paga`.

### 3.3. Análise do Fluxo de Caixa (`CashFlow`)

- O usuário acessa o relatório de Fluxo de Caixa.
- Seleciona um período (ex: Mês Atual, Últimos 30 dias, Período Personalizado).
- O sistema busca todas as `Transaction` no período selecionado e as agrupa por dia ou semana.
- O relatório exibe:
    - **Saldo Inicial**: O caixa no início do período.
    - **Total de Entradas (Receitas)**: Soma de todas as transações de crédito.
    - **Total de Saídas (Despesas)**: Soma de todas as transações de débito.
    - **Saldo Operacional**: Entradas - Saídas.
    - **Saldo Final**: Saldo Inicial + Saldo Operacional.

## 4. Regras de Negócio e Status

- **Status de Contas a Receber**:
    - `Pendente`: Aguardando pagamento.
    - `Parcialmente Pago`: Um ou mais pagamentos foram recebidos, mas não quitaram o valor total.
    - `Pago`: O valor total foi recebido.
    - `Vencido`: A data de vencimento passou e a conta não está com status `Pago`.
    - `Cancelado`: A conta foi cancelada (ex: devido ao cancelamento do pedido de venda).

- **Status de Contas a Pagar**:
    - `Pendente`: Aguardando pagamento.
    - `Paga`: O valor total foi pago.
    - `Vencida`: A data de vencimento passou e a conta não foi paga.
    - `Cancelada`: A obrigação de pagamento foi cancelada.

- **Regras de Validação**:
    - Uma `Transaction` deve estar obrigatoriamente associada a uma `AccountReceivable` ou `AccountPayable`.
    - A soma dos valores das `Transaction` de uma conta não pode exceder o valor total da conta.
    - O sistema deve impedir o registro de novos pagamentos para contas com status `Pago` ou `Cancelado`.
    - O status `Vencido` é um estado de alerta aplicado automaticamente pelo sistema com base na data atual e na data de vencimento.

## 5. Conclusão

O Domínio Financeiro é o destino final de todas as operações comerciais dentro do GesN. Ele não apenas registra o passado (o que foi pago e recebido), mas também fornece uma visão clara do futuro (o que há para pagar e receber), capacitando o gestor com a informação mais crítica para a sobrevivência e o crescimento do negócio: o controle sobre o dinheiro. A sua automação e integração com os outros domínios eliminam a necessidade de lançamentos manuais, reduzindo erros e garantindo que a informação financeira seja um reflexo fiel da operação.

