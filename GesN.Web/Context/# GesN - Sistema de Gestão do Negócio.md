# GesN - Sistema de GestÃ£o do NegÃ³cio

O GesN Ã© um sistema de gestÃ£o integrada projetado para gerenciar os processos de Vendas, ProduÃ§Ã£o, Financeiro e Compras de uma empresa.

## âœ¨ DomÃ­nios e Funcionalidades Principais

-   **Vendas:**
    -   GestÃ£o de Clientes (`Customer`)
    -   GestÃ£o de Pedidos (`OrderEntry`) com itens e status
    -   GestÃ£o de Contratos (`Contract`)
-   **ProduÃ§Ã£o:**
    -   CatÃ¡logo de Produtos (`Product`) com suporte a produtos Simples, Compostos e Grupos.
    -   GestÃ£o de Categorias de Produtos (`ProductCategory`)
    -   GestÃ£o de Fornecedores (`Supplier`) e Ingredientes (`Ingredient`)
    -   Ordens de ProduÃ§Ã£o (`ProductionOrder`)
-   **Financeiro:**
    -   GestÃ£o de TransaÃ§Ãµes Financeiras (`FinancialTransaction`)
    -   Categorias de TransaÃ§Ã£o e MÃ©todos de Pagamento
-   **AdministraÃ§Ã£o:**
    -   Controle de UsuÃ¡rios, FunÃ§Ãµes (`Roles`) e PermissÃµes (`Claims`)

## ðŸš€ Tecnologias Utilizadas

-   **Backend:** ASP.NET Core MVC, C#
-   **Banco de Dados:** SQLite
-   **Acesso a Dados:** Dapper
-   **Frontend:** HTML5, CSS3, JavaScript (ES6+)
-   **Frameworks e Bibliotecas:**
    -   jQuery 3.x
    -   Bootstrap 5
    -   DataTables.js
    -   Toastr.js
    -   Select2.js
    -   Algolia Autocomplete.js

## ðŸ Como Iniciar

1.  Clone o repositÃ³rio.
2.  Configure a string de conexÃ£o no arquivo `appsettings.json`.
3.  A inicializaÃ§Ã£o do banco de dados Ã© gerenciada pela classe `DbInit.cs` na primeira execuÃ§Ã£o.
4.  Execute o projeto (`dotnet run`).
