namespace GesN.Web.Models.Enumerators
{
    /// <summary>
    /// Tipo de transação financeira
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum TransactionType
    {
        Income,     // Receita
        Expense     // Despesa
    }

    /// <summary>
    /// Categoria de transação financeira
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum TransactionCategory
    {
        // Receitas
        Sales,              // Vendas
        ServiceRevenue,     // Receita de Serviços
        FinancialIncome,    // Receita Financeira
        OtherIncome,        // Outras Receitas
        
        // Despesas Operacionais
        RawMaterials,       // Matéria Prima
        Labor,              // Mão de Obra
        Equipment,          // Equipamentos
        Utilities,          // Utilidades (água, luz, gás)
        Rent,               // Aluguel
        Insurance,          // Seguros
        Marketing,          // Marketing
        Maintenance,        // Manutenção
        
        // Despesas Administrativas
        OfficeSupplies,     // Material de Escritório
        Professional,       // Serviços Profissionais
        Taxes,              // Impostos
        BankFees,           // Taxas Bancárias
        
        // Outras
        Transportation,     // Transporte
        Travel,             // Viagem
        Training,           // Treinamento
        Software,           // Software
        OtherExpenses       // Outras Despesas
    }

    /// <summary>
    /// Método de pagamento
    /// Valores como TEXT no banco de dados (usado em PaymentMethod.Name como sugestão)
    /// </summary>
    public enum PaymentMethodType
    {
        Cash,           // Dinheiro
        CreditCard,     // Cartão de Crédito
        DebitCard,      // Cartão de Débito
        BankTransfer,   // Transferência Bancária
        PIX,            // PIX
        Check,          // Cheque
        Voucher,        // Vale
        Other           // Outros
    }

    /// <summary>
    /// Status de pagamento
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum PaymentStatus
    {
        Pending,        // Pendente
        Processing,     // Processando
        Completed,      // Concluído
        Failed,         // Falhou
        Cancelled,      // Cancelado
        Refunded        // Reembolsado
    }
} 