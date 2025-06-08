namespace GesN.Web.Models.Enumerators
{
    /// <summary>
    /// Tipo de produto (usado na herança de Product)
    /// Valores como TEXT no banco de dados com CHECK constraint
    /// </summary>
    public enum ProductType
    {
        Simple,     // Produto Simples
        Composite,  // Produto Composto
        Group       // Grupo de Produtos
    }

    /// <summary>
    /// Status da ordem de produção
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum ProductionOrderStatus
    {
        Pending,        // Pendente
        Scheduled,      // Agendado
        InProgress,     // Em Andamento
        Paused,         // Pausado
        Completed,      // Concluído
        Cancelled,      // Cancelado
        Failed          // Falhou
    }

    /// <summary>
    /// Prioridade da ordem de produção
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum ProductionOrderPriority
    {
        Low,        // Baixa
        Normal,     // Normal
        High,       // Alta
        Urgent      // Urgente
    }

    /// <summary>
    /// Tipo de opção em grupo de produtos
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum ProductGroupOptionType
    {
        Single,     // Seleção única
        Multiple    // Seleção múltipla
    }

    /// <summary>
    /// Unidades de medida para produtos e ingredientes
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum MeasurementUnit
    {
        // Unidades de massa
        KG,     // Quilograma
        G,      // Grama
        MG,     // Miligrama
        
        // Unidades de volume
        L,      // Litro
        ML,     // Mililitro
        
        // Unidades de contagem
        UN,     // Unidade
        PCT,    // Pacote
        CX,     // Caixa
        DZ,     // Dúzia
        
        // Unidades de comprimento/área
        M,      // Metro
        CM,     // Centímetro
        M2,     // Metro quadrado
        
        // Outras
        TEMP    // Temporária/Outros
    }
} 