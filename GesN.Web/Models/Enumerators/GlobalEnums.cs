namespace GesN.Web.Models.Enumerators
{
    /// <summary>
    /// Estado do objeto na aplicação (usado em StateCode)
    /// Valores como INTEGER no banco de dados
    /// </summary>
    public enum ObjectState
    {
        Active = 1,
        Inactive = 2
    }

    /// <summary>
    /// Tipo de documento para identificação
    /// Valores como TEXT no banco de dados
    /// </summary>
    public enum DocumentType
    {
        CPF,
        CNPJ
    }

    /// <summary>
    /// Status legado - mantido para compatibilidade com tabelas antigas
    /// </summary>
    public enum Status
    {
        Ativo = 1,
        Inativo = 2
    }

    /// <summary>
    /// Unidades de medida para produção
    /// Usado em ingredientes, produtos e receitas
    /// </summary>
    public enum ProductionUnit
    {
        // Peso
        Gramas,
        Quilogramas,
        Toneladas,
        
        // Volume
        Mililitros,
        Litros,
        
        // Quantidade
        Unidades,
        Pacotes,
        Caixas,
        Duzias,
        Centenas,
        
        // Comprimento
        Centimetros,
        Metros,
        
        // Área
        MetrosQuadrados,
        
        // Outros
        Porcoes,
        Fatias,
        Colheres,
        Xicaras
    }
}
