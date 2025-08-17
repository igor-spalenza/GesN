# Arquitetura do Sistema GesN (Gest�o do Neg�cio)

**Vers�o:** 1.0.0
**�ltima Atualiza��o:** YYYY-MM-DD

## 1. Vis�o Geral e Objetivos

### 1.1. Prop�sito do Sistema
- Descri��o do GesN como uma solu��o SaaS para gerenciamento de jornadas de neg�cio.
- Dom�nios de neg�cio atendidos: Product, Sales, Production, Purchasing, Financial.
- P�blico-alvo: Pequenas e m�dias empresas que necessitam de uma gest�o integrada.

### 1.2. Objetivos Arquiteturais
- **Multi-Tenancy:** Garantir isolamento de dados e processamento por cliente (tenant).
- **Escalabilidade:** Permitir o crescimento horizontal da aplica��o adicionando novos containers.
- **Manutenibilidade:** C�digo claro, modular e bem documentado para facilitar a evolu��o.
- **Seguran�a:** Proteger os dados dos clientes e garantir acesso controlado atrav�s de pap�is e permiss�es.
- **Performance:** Respostas r�pidas da interface e processamento eficiente no backend.

### 1.3. Restri��es
- Stack de tecnologia definida (.NET, SQLite, Docker).
- Or�amento e time de desenvolvimento.
- Hospedagem em VPS com Debian.

## 2. Paisagem da Arquitetura (Vis�o de Alto N�vel)

### 2.1. Diagrama de Contexto
- Um diagrama mostrando as intera��es do sistema GesN com seus usu�rios e sistemas externos (se houver).

### 2.2. Diagrama de Containers
- Um diagrama ilustrando a arquitetura de implanta��o:
  - **Usu�rio Final** acessando o sistema via navegador.
  - **Traefik (Reverse Proxy)** recebendo as requisi��es no dom�nio `gesn.com.br`.
  - **Containers Docker** isolados por tenant.
  - Dentro de cada container:
    - **Aplica��o GesN (.NET 8)** rodando com **Kestrel**.
    - **Banco de Dados SQLite** (arquivo `.db` espec�fico do tenant).

## 3. Arquitetura do Backend

### 3.1. Stack de Tecnologias
- **Linguagem:** C# 12
- **Framework:** .NET 8 / ASP.NET Core MVC
- **Acesso a Dados:** Dapper + Microsoft.Data.Sqlite
- **Autentica��o e Autoriza��o:** ASP.NET Core Identity
- **Servidor Web:** Kestrel

### 3.2. Estrutura do Projeto (Solution)
- Descri��o da organiza��o dos projetos na solu��o (ex: `GesN.Web`, `GesN.Core`, `GesN.Data`).
- `GesN.Web`: Respons�vel pela UI, Controllers, ViewModels.
- `GesN.Core`: Cont�m as entidades de dom�nio, interfaces de servi�os e reposit�rios.
- `GesN.Data`: Implementa��o do acesso a dados com Dapper e reposit�rios.

### 3.3. Padr�es e Princ�pios de Design
- **MVC (Model-View-Controller):** Padr�o principal da aplica��o web.
- **Inje��o de Depend�ncia (DI):** Usada extensivamente para desacoplar componentes (ex: `IProductService` injetado nos controllers).
- **Padr�o Reposit�rio-Servi�o:**
  - **Camada de Servi�o:** Orquestra a l�gica de neg�cio (`ProductComponentHierarchyService`).
  - **Camada de Reposit�rio:** Abstrai o acesso aos dados (Dapper).
- **Programa��o Ass�ncrona:** Uso de `async/await` para opera��es de I/O.

### 3.4. Acesso a Dados
- **ORM:** Dapper (Micro-ORM) para performance e controle sobre as queries SQL.
- **Banco de Dados:** SQLite 3.
- **Estrat�gia Multi-Tenant:** Banco de dados como arquivo (`tenant_id.db`) dentro do volume de cada container, garantindo isolamento total.
- **Migrations:** Estrat�gia para atualiza��o de schema do banco de dados (ex: scripts SQL versionados, FluentMigrator, etc.).

### 3.5. Autentica��o e Autoriza��o
- **Framework:** ASP.NET Core Identity.
- **Estrat�gia:** Autentica��o baseada em Cookies.
- **Autoriza��o:** Baseada em Pap�is (Roles) e Permiss�es (Claims). O arquivo `Roles.js` e as `[Authorize]` nos controllers confirmam essa abordagem.

### 3.6. API e Endpoints
- A aplica��o n�o exp�e uma API REST p�blica, mas utiliza endpoints internos para comunica��o com o frontend.
- **Padr�o de Endpoints:**
  - `Controller/Action`: Retorna `PartialView` para carregar se��es da p�gina dinamicamente (ex: `_Grid.cshtml`).
  - `Controller/ActionAsJson`: Retorna `JsonResult` para opera��es como autocomplete, salvamento de formul�rios e atualiza��es de status.

## 4. Arquitetura do Frontend

### 4.1. Stack de Tecnologias
- **Core:** HTML5, CSS3, JavaScript (ES6+).
- **Frameworks/Bibliotecas:**
  - **jQuery:** Base para manipula��o do DOM e AJAX.
  - **Bootstrap 5:** Estrutura de layout, componentes (Modals, Tabs, etc.) e responsividade.
  - **DataTables.NET:** Para grids de dados interativas com pagina��o, busca e ordena��o.
  - **Select2:** Melhoria de campos `<select>`.
  - **Toastr.js:** Notifica��es (sucesso, erro, informa��o).
  - **Autocomplete.js (Algolia):** Para campos de busca com sugest�es.
  - **jQuery Validation & Unobtrusive:** Valida��o de formul�rios client-side.

### 4.2. Estrutura e Padr�es JavaScript
- **Padr�o "Manager Object":** A conven��o principal � criar um objeto JavaScript por m�dulo/p�gina (ex: `ordersManager`, `productManager`, `rolesManager`).
  - Cada "manager" encapsula o estado e os comportamentos da sua respectiva funcionalidade.
  - Fun��es s�o organizadas dentro do objeto (ex: `carregarLista`, `abrirEdicao`, `salvarNovoModal`).
- **Comunica��o com Backend:** Quase exclusivamente via `$.ajax`.
- **Renderiza��o:** A renderiza��o inicial � feita pelo Razor no servidor. Atualiza��es din�micas (novos dados em uma grid, conte�do de um modal) s�o feitas injetando o HTML retornado de `PartialViews` via AJAX.

### 4.3. Padr�es de UI/UX
- **CRUD em Abas:** Entidades complexas como "Produto" e "Pedido" s�o editadas em abas din�micas para permitir que o usu�rio trabalhe em m�ltiplos itens simultaneamente.
- **CRUD em Modal:** Entidades mais simples como "Categoria de Produto" ou a cria��o inicial de um item s�o feitas em modais do Bootstrap.
- **Grids Ricas:** O `DataTables.NET` � o padr�o para listagem de dados.
- **Feedback ao Usu�rio:** `Toastr.js` � usado para feedback de opera��es (ex: "Pedido salvo com sucesso!"). Spinners s�o usados para indicar carregamento.

## 5. Infraestrutura e Implanta��o (DevOps)

### 5.1. Ambiente de Hospedagem
- **Provedor:** VPS (DigitalOcean, Vultr, etc.).
- **Sistema Operacional:** Debian.

### 5.2. Containeriza��o
- **Tecnologia:** Docker.
- **Estrat�gia Multi-Tenant:** Um container Docker por tenant. O provisionamento de um novo tenant implica em criar um novo container com sua pr�pria configura��o e volume de dados.
- **Orquestra��o:** (Atualmente manual/scripted). Mencionar planos futuros para Kubernetes/Swarm se aplic�vel.

### 5.3. Proxy Reverso e Roteamento
- **Tecnologia:** Traefik.
- **Fun��o:**
  - Ponto de entrada �nico para todas as requisi��es HTTP/S.
  - Gerenciamento de certificados SSL/TLS (Let's Encrypt).
  - Roteamento de requisi��es para o container do tenant correto baseado no subdom�nio (ex: `clienteA.gesn.com.br`) ou outro identificador.

### 5.4. Processo de Build e Deploy (CI/CD)
- (Descrever o processo atual ou o processo desejado).
- **Exemplo de fluxo:**
  1. Push para o reposit�rio Git (ex: `main` branch).
  2. A��o do GitHub Actions / Jenkins / Azure DevOps � acionada.
  3. Build do projeto .NET.
  4. Build da imagem Docker da aplica��o.
  5. Push da imagem para um Docker Registry (Docker Hub, GitHub Packages).
  6. Conex�o via SSH no VPS e execu��o de um script para `docker-compose pull` e `docker-compose up -d` para atualizar a aplica��o.

## 6. T�picos Transversais (Cross-Cutting Concerns)

### 6.1. Logging
- **Framework:** `ILogger` (implementa��o via Serilog, NLog, etc.).
- **Estrat�gia:** Logs s�o escritos para o console (coletados pelo Docker) e/ou para arquivos de log dentro do volume do container.

### 6.2. Tratamento de Erros
- **Backend:** Middlewares para capturar exce��es n�o tratadas. Blocos `try-catch` nos controllers para erros esperados, retornando `JsonResult` com `{ success: false, message: "..." }`.
- **Frontend:** Uso dos callbacks `.fail()` ou `.error()` do `$.ajax` para tratar respostas de erro do servidor e exibir mensagens via `toastr.error()`.

### 6.3. Seguran�a
- **Preven��o de CSRF:** Uso de `[ValidateAntiForgeryToken]` em todas as a��es `POST` que alteram estado.
- **Preven��o de XSS:** Razor Views encodam sa�das por padr�o. Cuidado extra na inje��o de HTML via JavaScript.
- **Gerenciamento de Segredos:** Como as connection strings e outras chaves s�o gerenciadas (ex: `appsettings.json`, vari�veis de ambiente do Docker).

## 7. Ap�ndices

### 7.1. Pacotes NuGet Principais
- **Dapper:** Micro-ORM de alta performance para executar queries SQL e mapear resultados para objetos C#.
- **Microsoft.Data.Sqlite:** Provedor de dados ADO.NET para interagir com o banco de dados SQLite.
- **Microsoft.AspNetCore.Identity:** Framework para gerenciamento de usu�rios, pap�is, claims e autentica��o. (Nota: A persist�ncia pode ser customizada para Dapper ou usar o `Identity.EntityFrameworkCore` se for uma exce��o).

### 7.2. Bibliotecas Client-Side
- **jquery:** Biblioteca fundamental para manipula��o de DOM, eventos e AJAX.
- **bootstrap:** Framework CSS para layout, componentes visuais e responsividade.
- **datatables.net:** Plugin jQuery para cria��o de tabelas de dados avan�adas.
- **select2:** Plugin jQuery que substitui e melhora os campos de sele��o `<select>`.
- **toastr.js:** Biblioteca para exibir notifica��es "toast" n�o-intrusivas.
- **autocomplete.js:** Biblioteca para criar experi�ncias de busca com autocompletar.
- **jquery-validation/jquery-validation-unobtrusive:** Plugins para valida��o de formul�rios no lado do cliente, integrados com os Data Annotations do .NET.
- **jquery.mask:** Plugin para aplicar m�scaras de entrada em campos de formul�rio (ex: CPF, telefone, moeda).
- **jquery.qrcode:** Plugin para gerar QR Codes no lado do cliente.

### 7.3. Conven��es de C�digo
- **C#:** Estilo de c�digo (PascalCase para m�todos e propriedades, etc.).
- **JavaScript:** Padr�o "Manager Object", uso de `const` para o objeto principal, nomes de fun��es.
- **CSS:** Metodologia (ex: BEM) se aplic�vel, ou conven��es de nomenclatura.
- **Git:** Conven��es para nomes de branch e mensagens de commit.

