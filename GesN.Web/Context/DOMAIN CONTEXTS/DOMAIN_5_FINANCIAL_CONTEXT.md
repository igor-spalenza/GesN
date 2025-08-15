# Descri��o Funcional de Software - Dom�nio Financeiro

## 1. Vis�o Geral

O **Dom�nio Financeiro** � o centro nervoso do sistema GesN, respons�vel por rastrear, gerenciar e relatar todo o fluxo de dinheiro que entra e sai da empresa. Ele consolida as atividades operacionais dos dom�nios de **Vendas** e **Compras**, traduzindo-as em registros financeiros claros e acion�veis, como contas a receber e a pagar.

Este dom�nio � fundamental para a tomada de decis�es estrat�gicas, pois oferece uma vis�o precisa da sa�de financeira do neg�cio, do fluxo de caixa e da lucratividade.

- **Integra��o com Vendas**: Cada `OrderEntry` (Pedido) confirmado no Dom�nio de Vendas gera automaticamente uma ou mais `AccountReceivable` (Contas a Receber), representando a receita a ser coletada dos clientes.
- **Integra��o com Compras**: Cada `PurchaseOrder` (Ordem de Compra) recebida no Dom�nio de Compras gera automaticamente uma `AccountPayable` (Conta a Pagar), representando a obriga��o financeira com os fornecedores.

O objetivo principal � fornecer ferramentas para um controle financeiro rigoroso, automatizando a cria��o de lan�amentos, facilitando a concilia��o de pagamentos e oferecendo relat�rios essenciais como o Fluxo de Caixa.

## 2. Entidades Principais

As seguintes entidades formam a estrutura do Dom�nio Financeiro:

- **`AccountReceivable` (Conta a Receber)**: Representa um valor que a empresa tem o direito de receber de um `Customer` (Cliente). � gerada a partir de um `OrderEntry` e cont�m informa��es como valor, data de vencimento e status do pagamento.
- **`AccountPayable` (Conta a Pagar)**: Representa uma obriga��o financeira que a empresa tem com um `Supplier` (Fornecedor). � gerada a partir de uma `PurchaseOrder` e cont�m informa��es como valor, data de vencimento e status do pagamento.
- **`Transaction` (Transa��o/Lan�amento)**: A entidade mais granular. Representa qualquer movimento de dinheiro, seja uma entrada (cr�dito) ou uma sa�da (d�bito). Cada transa��o est� vinculada a uma conta a receber ou a pagar e registra a data e o valor do pagamento/recebimento efetuado.
- **`CashFlow` (Fluxo de Caixa - Relat�rio/Vis�o)**: N�o � uma entidade de banco de dados, mas uma vis�o consolidada gerada a partir das `Transaction`. Apresenta as entradas, sa�das e o saldo de caixa em um determinado per�odo.

## 3. Jornada do Usu�rio e Fluxos de Trabalho

O Dom�nio Financeiro � organizado em torno de dois fluxos principais: a gest�o de entradas (receb�veis) e a gest�o de sa�das (pag�veis).

### 3.1. Gest�o de Contas a Receber

Este fluxo gerencia o dinheiro que entra na empresa.

1.  **Gera��o Autom�tica**:
    - Quando um `OrderEntry` � **confirmado** no Dom�nio de Vendas, o sistema cria automaticamente um registro de `AccountReceivable`.
    - Se o pedido tiver condi��es de pagamento parceladas, o sistema pode gerar m�ltiplos registros de `AccountReceivable`, cada um com sua respectiva data de vencimento e valor.

2.  **Painel de Contas a Receber**:
    - O usu�rio acessa uma tela que lista todas as contas a receber.
    - A tela oferece filtros por cliente, per�odo de vencimento, status (`Pendente`, `Pago`, `Vencido`).
    - Indicadores visuais destacam as contas que est�o pr�ximas do vencimento ou j� vencidas.

3.  **Registro de Recebimento**:
    - Quando um cliente efetua um pagamento, o usu�rio localiza a `AccountReceivable` correspondente.
    - O usu�rio clica em "Registrar Recebimento" e informa o valor recebido e a data.
    - O sistema cria uma `Transaction` do tipo **Cr�dito**.
    - O status da `AccountReceivable` � atualizado:
        - Se o valor recebido for menor que o total, o status muda para `Parcialmente Pago`.
        - Se o valor recebido quitar o saldo, o status muda para `Pago`.

### 3.2. Gest�o de Contas a Pagar

Este fluxo gerencia o dinheiro que sai da empresa.

1.  **Gera��o Autom�tica**:
    - Quando uma `PurchaseOrder` � marcada como **Recebida Totalmente** no Dom�nio de Compras, o sistema cria automaticamente um registro de `AccountPayable` no valor total da nota, vinculado ao fornecedor.

2.  **Painel de Contas a Pagar**:
    - O usu�rio acessa uma tela que lista todas as contas a pagar.
    - A tela permite filtrar por fornecedor, per�odo de vencimento e status (`Pendente`, `Paga`, `Vencida`).
    - O sistema alerta sobre as contas com vencimento pr�ximo.

3.  **Registro de Pagamento**:
    - Quando a empresa paga um fornecedor, o usu�rio localiza a `AccountPayable` correspondente.
    - O usu�rio clica em "Registrar Pagamento" e informa o valor pago e a data.
    - O sistema cria uma `Transaction` do tipo **D�bito**.
    - O status da `AccountPayable` � atualizado para `Paga`.

### 3.3. An�lise do Fluxo de Caixa (`CashFlow`)

- O usu�rio acessa o relat�rio de Fluxo de Caixa.
- Seleciona um per�odo (ex: M�s Atual, �ltimos 30 dias, Per�odo Personalizado).
- O sistema busca todas as `Transaction` no per�odo selecionado e as agrupa por dia ou semana.
- O relat�rio exibe:
    - **Saldo Inicial**: O caixa no in�cio do per�odo.
    - **Total de Entradas (Receitas)**: Soma de todas as transa��es de cr�dito.
    - **Total de Sa�das (Despesas)**: Soma de todas as transa��es de d�bito.
    - **Saldo Operacional**: Entradas - Sa�das.
    - **Saldo Final**: Saldo Inicial + Saldo Operacional.

## 4. Regras de Neg�cio e Status

- **Status de Contas a Receber**:
    - `Pendente`: Aguardando pagamento.
    - `Parcialmente Pago`: Um ou mais pagamentos foram recebidos, mas n�o quitaram o valor total.
    - `Pago`: O valor total foi recebido.
    - `Vencido`: A data de vencimento passou e a conta n�o est� com status `Pago`.
    - `Cancelado`: A conta foi cancelada (ex: devido ao cancelamento do pedido de venda).

- **Status de Contas a Pagar**:
    - `Pendente`: Aguardando pagamento.
    - `Paga`: O valor total foi pago.
    - `Vencida`: A data de vencimento passou e a conta n�o foi paga.
    - `Cancelada`: A obriga��o de pagamento foi cancelada.

- **Regras de Valida��o**:
    - Uma `Transaction` deve estar obrigatoriamente associada a uma `AccountReceivable` ou `AccountPayable`.
    - A soma dos valores das `Transaction` de uma conta n�o pode exceder o valor total da conta.
    - O sistema deve impedir o registro de novos pagamentos para contas com status `Pago` ou `Cancelado`.
    - O status `Vencido` � um estado de alerta aplicado automaticamente pelo sistema com base na data atual e na data de vencimento.

## 5. Conclus�o

O Dom�nio Financeiro � o destino final de todas as opera��es comerciais dentro do GesN. Ele n�o apenas registra o passado (o que foi pago e recebido), mas tamb�m fornece uma vis�o clara do futuro (o que h� para pagar e receber), capacitando o gestor com a informa��o mais cr�tica para a sobreviv�ncia e o crescimento do neg�cio: o controle sobre o dinheiro. A sua automa��o e integra��o com os outros dom�nios eliminam a necessidade de lan�amentos manuais, reduzindo erros e garantindo que a informa��o financeira seja um reflexo fiel da opera��o.

