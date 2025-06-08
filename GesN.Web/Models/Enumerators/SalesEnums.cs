namespace GesN.Web.Models.Enumerators
{
    /// <summary>
    /// Status do pedido (Order)
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum OrderStatus
    {
        Draft,          // Rascunho
        Confirmed,      // Confirmado
        InProduction,   // Em Produção
        ReadyForDelivery, // Pronto para Entrega
        Delivered,      // Entregue
        Cancelled,      // Cancelado
        Completed       // Finalizado
    }

    /// <summary>
    /// Tipo do pedido (Order)
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum OrderType
    {
        Order,          // Encomenda
        Event,          // Evento
        Catering,       // Buffet
        Delivery,       // Entrega
        PickUp          // Retirada
    }

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
} 