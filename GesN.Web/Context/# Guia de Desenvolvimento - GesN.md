# Guia de Desenvolvimento - GesN

Este documento descreve os padrões de arquitetura e as convenções de código para o projeto GesN. O objetivo é manter a consistência e a qualidade do código em toda a aplicação.

## 🏗️ Arquitetura

### Backend (ASP.NET Core)

-   **Padrão MVC:** A aplicação segue o padrão Model-View-Controller.
-   **Acesso a Dados:** Utilizamos **Dapper** para comunicação com o banco de dados **SQLite**. A lógica de acesso a dados deve ser encapsulada em classes de Repositório ou Serviço.
-   **Inicialização do BD:** A estrutura das tabelas é definida e criada via código na classe `Data/Migrations/DbInit.cs`.
-   **ViewModels:** Cada `View` ou `PartialView` deve ter seu próprio ViewModel (ex: `CreateProductViewModel`, `EditProductViewModel`). Isso evita o uso de `ViewBag`/`ViewData` e o acoplamento das entidades de domínio às views.
-   **Respostas AJAX:** Actions que respondem a chamadas AJAX devem retornar `JsonResult` com um formato padrão: `{ success: boolean, message: string, data: object | null }`.

### Frontend (JavaScript/jQuery)

-   **Padrão "Manager Object":** Para cada módulo/entidade (ex: Pedidos, Produtos, Clientes), deve ser criado um objeto JavaScript que encapsula toda a sua funcionalidade. Isso organiza o código e evita poluir o escopo global.
    -   **Exemplo:** `Order.js` contém `const ordersManager = { ... }`.
    -   **Exemplo:** `Product.js` contém `const productManager = { ... }`.

-   **Padrões de Interface (UI/UX):**
    -   **Listagem Principal:** As telas de índice de cada módulo devem apresentar uma grid de dados carregada via AJAX.
    -   **Criação e Detalhes:** Formulários de criação e telas de detalhes devem ser carregados em **modais do Bootstrap**.
    -   **Edição:** A edição de registros complexos (como Pedidos, Produtos, Categorias) deve ser feita em um sistema de **abas dinâmicas**. A edição de um item abre uma nova aba, permitindo que o usuário trabalhe em múltiplos registros simultaneamente.

-   **Bibliotecas Padrão:**
    -   **Grids e Tabelas:** Utilizar **DataTables.js** para todas as tabelas de dados.
    -   **Notificações:** Utilizar **Toastr.js** para todo feedback ao usuário (sucesso, erro, aviso).
    -   **Seleção com Busca (Dropdowns):** Utilizar **Select2.js** para campos de seleção que necessitam de busca.
    -   **Autocompletar:** Utilizar **Algolia Autocomplete.js** para campos de busca com sugestões dinâmicas (ex: busca de clientes).

## ✍️ Convenções de Código

### C# (Backend)

-   Utilize `async/await` para todas as operações de I/O (acesso ao banco de dados).
-   Siga as convenções de nomenclatura do .NET (PascalCase para métodos e propriedades, camelCase para parâmetros).
-   Use os Data Annotations do `System.ComponentModel.DataAnnotations` para validação nos ViewModels.

### JavaScript (Frontend)

-   Use `const` e `let` em vez de `var`.
-   Nomeie os "manager objects" com camelCase e sufixo `Manager` (ex: `ordersManager`, `productCategoriesManager`).
-   Funções dentro do manager devem ser claras, diretas e em camelCase (ex: `carregarListaOrders`, `salvarNovoModal`).
-   Use `$` como prefixo para variáveis que armazenam objetos jQuery (ex: `const $form = ...`).
-   As chamadas AJAX devem sempre implementar os callbacks `success`, `error`, e `complete` para um feedback adequado ao usuário e controle de estado (ex: desabilitar/reabilitar botões).

## 📋 Instruções para o Gemini Code Assist

Ao solicitar a criação de novas funcionalidades, siga estes padrões:

1.  **Para um novo CRUD completo (ex: Fornecedores):**
    -   Peça a criação do Controller, ViewModels, e o arquivo JavaScript (`Supplier.js` com `supplierManager`).
    -   Especifique que a **edição deve usar o sistema de abas dinâmicas**, similar ao `ordersManager` ou `productCategoriesManager`.
    -   Especifique que a **criação e os detalhes devem usar modais**, similar ao `ordersManager`.

2.  **Para adicionar um campo a um formulário:**
    -   Indique o ViewModel a ser modificado.
    -   Indique a View ou PartialView a ser alterada.
    -   Se for um campo de seleção com busca, especifique o uso de `Select2.js` e o endpoint para buscar os dados.

3.  **Para refatoração:**
    -   Se um arquivo JS não segue o padrão "manager object" (como o `Customer.js` atual), peça para refatorá-lo para se alinhar com `Order.js` ou `Product.js`.

**Exemplo de Prompt:**
> "Crie o CRUD para Fornecedores (`Supplier`). A edição deve abrir em uma nova aba e a criação em um modal, seguindo o padrão do módulo de Produtos. O formulário deve conter os campos Nome, Razão Social e CNPJ."