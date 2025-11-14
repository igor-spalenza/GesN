# Guia de Desenvolvimento - GesN

Este documento descreve os padrÃµes de arquitetura e as convenÃ§Ãµes de cÃ³digo para o projeto GesN. O objetivo Ã© manter a consistÃªncia e a qualidade do cÃ³digo em toda a aplicaÃ§Ã£o.

## ðŸ—ï¸ Arquitetura

### Backend (ASP.NET Core)

-   **PadrÃ£o MVC:** A aplicaÃ§Ã£o segue o padrÃ£o Model-View-Controller.
-   **Acesso a Dados:** Utilizamos **Dapper** para comunicaÃ§Ã£o com o banco de dados **SQLite**. A lÃ³gica de acesso a dados deve ser encapsulada em classes de RepositÃ³rio ou ServiÃ§o.
-   **InicializaÃ§Ã£o do BD:** A estrutura das tabelas Ã© definida e criada via cÃ³digo na classe `Data/Migrations/DbInit.cs`.
-   **ViewModels:** Cada `View` ou `PartialView` deve ter seu prÃ³prio ViewModel (ex: `CreateProductViewModel`, `EditProductViewModel`). Isso evita o uso de `ViewBag`/`ViewData` e o acoplamento das entidades de domÃ­nio Ã s views.
-   **Respostas AJAX:** Actions que respondem a chamadas AJAX devem retornar `JsonResult` com um formato padrÃ£o: `{ success: boolean, message: string, data: object | null }`.

### Frontend (JavaScript/jQuery)

-   **PadrÃ£o "Manager Object":** Para cada mÃ³dulo/entidade (ex: Pedidos, Produtos, Clientes), deve ser criado um objeto JavaScript que encapsula toda a sua funcionalidade. Isso organiza o cÃ³digo e evita poluir o escopo global.
    -   **Exemplo:** `Order.js` contÃ©m `const ordersManager = { ... }`.
    -   **Exemplo:** `Product.js` contÃ©m `const productManager = { ... }`.

-   **PadrÃµes de Interface (UI/UX):**
    -   **Listagem Principal:** As telas de Ã­ndice de cada mÃ³dulo devem apresentar uma grid de dados carregada via AJAX.
    -   **CriaÃ§Ã£o e Detalhes:** FormulÃ¡rios de criaÃ§Ã£o e telas de detalhes devem ser carregados em **modais do Bootstrap**.
    -   **EdiÃ§Ã£o:** A ediÃ§Ã£o de registros complexos (como Pedidos, Produtos, Categorias) deve ser feita em um sistema de **abas dinÃ¢micas**. A ediÃ§Ã£o de um item abre uma nova aba, permitindo que o usuÃ¡rio trabalhe em mÃºltiplos registros simultaneamente.

-   **Bibliotecas PadrÃ£o:**
    -   **Grids e Tabelas:** Utilizar **DataTables.js** para todas as tabelas de dados.
    -   **NotificaÃ§Ãµes:** Utilizar **Toastr.js** para todo feedback ao usuÃ¡rio (sucesso, erro, aviso).
    -   **SeleÃ§Ã£o com Busca (Dropdowns):** Utilizar **Select2.js** para campos de seleÃ§Ã£o que necessitam de busca.
    -   **Autocompletar:** Utilizar **Algolia Autocomplete.js** para campos de busca com sugestÃµes dinÃ¢micas (ex: busca de clientes).

## âœï¸ ConvenÃ§Ãµes de CÃ³digo

### C# (Backend)

-   Utilize `async/await` para todas as operaÃ§Ãµes de I/O (acesso ao banco de dados).
-   Siga as convenÃ§Ãµes de nomenclatura do .NET (PascalCase para mÃ©todos e propriedades, camelCase para parÃ¢metros).
-   Use os Data Annotations do `System.ComponentModel.DataAnnotations` para validaÃ§Ã£o nos ViewModels.

### JavaScript (Frontend)

-   Use `const` e `let` em vez de `var`.
-   Nomeie os "manager objects" com camelCase e sufixo `Manager` (ex: `ordersManager`, `productCategoriesManager`).
-   FunÃ§Ãµes dentro do manager devem ser claras, diretas e em camelCase (ex: `carregarListaOrders`, `salvarNovoModal`).
-   Use `$` como prefixo para variÃ¡veis que armazenam objetos jQuery (ex: `const $form = ...`).
-   As chamadas AJAX devem sempre implementar os callbacks `success`, `error`, e `complete` para um feedback adequado ao usuÃ¡rio e controle de estado (ex: desabilitar/reabilitar botÃµes).

## ðŸ“‹ InstruÃ§Ãµes para o Gemini Code Assist

Ao solicitar a criaÃ§Ã£o de novas funcionalidades, siga estes padrÃµes:

1.  **Para um novo CRUD completo (ex: Fornecedores):**
    -   PeÃ§a a criaÃ§Ã£o do Controller, ViewModels, e o arquivo JavaScript (`Supplier.js` com `supplierManager`).
    -   Especifique que a **ediÃ§Ã£o deve usar o sistema de abas dinÃ¢micas**, similar ao `ordersManager` ou `productCategoriesManager`.
    -   Especifique que a **criaÃ§Ã£o e os detalhes devem usar modais**, similar ao `ordersManager`.

2.  **Para adicionar um campo a um formulÃ¡rio:**
    -   Indique o ViewModel a ser modificado.
    -   Indique a View ou PartialView a ser alterada.
    -   Se for um campo de seleÃ§Ã£o com busca, especifique o uso de `Select2.js` e o endpoint para buscar os dados.

3.  **Para refatoraÃ§Ã£o:**
    -   Se um arquivo JS nÃ£o segue o padrÃ£o "manager object" (como o `Customer.js` atual), peÃ§a para refatorÃ¡-lo para se alinhar com `Order.js` ou `Product.js`.

**Exemplo de Prompt:**
> "Crie o CRUD para Fornecedores (`Supplier`). A ediÃ§Ã£o deve abrir em uma nova aba e a criaÃ§Ã£o em um modal, seguindo o padrÃ£o do mÃ³dulo de Produtos. O formulÃ¡rio deve conter os campos Nome, RazÃ£o Social e CNPJ."
