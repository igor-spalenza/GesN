namespace GesN.Web.Models.Enumerators
{

    /// <summary>
    /// Status do contrato (Contract)
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum ContractStatus
    {
        Draft,              // Rascunho
        Generated,          // Gerado
        SentForSignature,   // Enviado para Assinatura
        Partiallysigned,    // Parcialmente Assinado
        Signed,             // Assinado
        Active,             // Ativo
        Suspended,          // Suspenso
        Renewed,            // Renovado
        Completed,          // Concluído
        Cancelled,          // Cancelado
        Expired             // Expirado
    }

    /// <summary>
    /// Status de impressão de recibo fiscal
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum ReceiptPrintStatus
    {
        NotPrinted,     // Não Impresso
        Printed,        // Impresso
        Reprinted,      // Reimpresso
        Failed,         // Falha na Impressão
        Cancelled       // Cancelado
    }

    /// <summary>
    /// Status de Order
    /// </summary>
    public enum OrderStatus
    {
        Draft,
        Confirmed,
        InProduction,
        ReadyForDelivery,
        InDelivery,
        Delivered,
        Cancelled,
        Completed
    }

    /// <summary>
    /// Tipos possíveis de um pedido
    /// </summary>
    public enum OrderType
    {
        Order,
        Event
    }

    /// <summary>
    /// Status de impressão de um pedido
    /// </summary>
    public enum PrintStatus
    {
        NotPrinted,
        Printed

    }
} 