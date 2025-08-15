# GesN - Sistema de Gestão do Negócio

O GesN é um sistema de gestão integrada projetado para gerenciar os processos de Vendas, Produção, Financeiro e Compras de uma empresa.

## ✨ Domínios e Funcionalidades Principais

-   **Vendas:**
    -   Gestão de Clientes (`Customer`)
    -   Gestão de Pedidos (`OrderEntry`) com itens e status
    -   Gestão de Contratos (`Contract`)
-   **Produção:**
    -   Catálogo de Produtos (`Product`) com suporte a produtos Simples, Compostos e Grupos.
    -   Gestão de Categorias de Produtos (`ProductCategory`)
    -   Gestão de Fornecedores (`Supplier`) e Ingredientes (`Ingredient`)
    -   Ordens de Produção (`ProductionOrder`)
-   **Financeiro:**
    -   Gestão de Transações Financeiras (`FinancialTransaction`)
    -   Categorias de Transação e Métodos de Pagamento
-   **Administração:**
    -   Controle de Usuários, Funções (`Roles`) e Permissões (`Claims`)

## 🚀 Tecnologias Utilizadas

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

## 🏁 Como Iniciar

1.  Clone o repositório.
2.  Configure a string de conexão no arquivo `appsettings.json`.
3.  A inicialização do banco de dados é gerenciada pela classe `DbInit.cs` na primeira execução.
4.  Execute o projeto (`dotnet run`).