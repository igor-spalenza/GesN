# Arquitetura do Sistema GesN (Gestão do Negócio)

**Versão:** 1.0.0
**Última Atualização:** YYYY-MM-DD

## 1. Visão Geral e Objetivos

### 1.1. Propósito do Sistema
- Descrição do GesN como uma solução SaaS para gerenciamento de jornadas de negócio.
- Domínios de negócio atendidos: Product, Sales, Production, Purchasing, Financial.
- Público-alvo: Pequenas e médias empresas que necessitam de uma gestão integrada.

### 1.2. Objetivos Arquiteturais
- **Multi-Tenancy:** Garantir isolamento de dados e processamento por cliente (tenant).
- **Escalabilidade:** Permitir o crescimento horizontal da aplicação adicionando novos containers.
- **Manutenibilidade:** Código claro, modular e bem documentado para facilitar a evolução.
- **Segurança:** Proteger os dados dos clientes e garantir acesso controlado através de papéis e permissões.
- **Performance:** Respostas rápidas da interface e processamento eficiente no backend.

### 1.3. Restrições
- Stack de tecnologia definida (.NET, SQLite, Docker).
- Orçamento e time de desenvolvimento.
- Hospedagem em VPS com Debian.

## 2. Paisagem da Arquitetura (Visão de Alto Nível)

### 2.1. Diagrama de Contexto
- Um diagrama mostrando as interações do sistema GesN com seus usuários e sistemas externos (se houver).

### 2.2. Diagrama de Containers
- Um diagrama ilustrando a arquitetura de implantação:
  - **Usuário Final** acessando o sistema via navegador.
  - **Traefik (Reverse Proxy)** recebendo as requisições no domínio `gesn.com.br`.
  - **Containers Docker** isolados por tenant.
  - Dentro de cada container:
    - **Aplicação GesN (.NET 8)** rodando com **Kestrel**.
    - **Banco de Dados SQLite** (arquivo `.db` específico do tenant).

## 3. Arquitetura do Backend

### 3.1. Stack de Tecnologias
- **Linguagem:** C# 12
- **Framework:** .NET 8 / ASP.NET Core MVC
- **Acesso a Dados:** Dapper + Microsoft.Data.Sqlite
- **Autenticação e Autorização:** ASP.NET Core Identity
- **Servidor Web:** Kestrel

### 3.2. Estrutura do Projeto (Solution)
- Descrição da organização dos projetos na solução (ex: `GesN.Web`, `GesN.Core`, `GesN.Data`).
- `GesN.Web`: Responsável pela UI, Controllers, ViewModels.
- `GesN.Core`: Contém as entidades de domínio, interfaces de serviços e repositórios.
- `GesN.Data`: Implementação do acesso a dados com Dapper e repositórios.

### 3.3. Padrões e Princípios de Design
- **MVC (Model-View-Controller):** Padrão principal da aplicação web.
- **Injeção de Dependência (DI):** Usada extensivamente para desacoplar componentes (ex: `IProductService` injetado nos controllers).
- **Padrão Repositório-Serviço:**
  - **Camada de Serviço:** Orquestra a lógica de negócio (`ProductComponentHierarchyService`).
  - **Camada de Repositório:** Abstrai o acesso aos dados (Dapper).
- **Programação Assíncrona:** Uso de `async/await` para operações de I/O.

### 3.4. Acesso a Dados
- **ORM:** Dapper (Micro-ORM) para performance e controle sobre as queries SQL.
- **Banco de Dados:** SQLite 3.
- **Estratégia Multi-Tenant:** Banco de dados como arquivo (`tenant_id.db`) dentro do volume de cada container, garantindo isolamento total.
- **Migrations:** Estratégia para atualização de schema do banco de dados (ex: scripts SQL versionados, FluentMigrator, etc.).

### 3.5. Autenticação e Autorização
- **Framework:** ASP.NET Core Identity.
- **Estratégia:** Autenticação baseada em Cookies.
- **Autorização:** Baseada em Papéis (Roles) e Permissões (Claims). O arquivo `Roles.js` e as `[Authorize]` nos controllers confirmam essa abordagem.

### 3.6. API e Endpoints
- A aplicação não expõe uma API REST pública, mas utiliza endpoints internos para comunicação com o frontend.
- **Padrão de Endpoints:**
  - `Controller/Action`: Retorna `PartialView` para carregar seções da página dinamicamente (ex: `_Grid.cshtml`).
  - `Controller/ActionAsJson`: Retorna `JsonResult` para operações como autocomplete, salvamento de formulários e atualizações de status.

## 4. Arquitetura do Frontend

### 4.1. Stack de Tecnologias
- **Core:** HTML5, CSS3, JavaScript (ES6+).
- **Frameworks/Bibliotecas:**
  - **jQuery:** Base para manipulação do DOM e AJAX.
  - **Bootstrap 5:** Estrutura de layout, componentes (Modals, Tabs, etc.) e responsividade.
  - **DataTables.NET:** Para grids de dados interativas com paginação, busca e ordenação.
  - **Select2:** Melhoria de campos `<select>`.
  - **Toastr.js:** Notificações (sucesso, erro, informação).
  - **Autocomplete.js (Algolia):** Para campos de busca com sugestões.
  - **jQuery Validation & Unobtrusive:** Validação de formulários client-side.

### 4.2. Estrutura e Padrões JavaScript
- **Padrão "Manager Object":** A convenção principal é criar um objeto JavaScript por módulo/página (ex: `ordersManager`, `productManager`, `rolesManager`).
  - Cada "manager" encapsula o estado e os comportamentos da sua respectiva funcionalidade.
  - Funções são organizadas dentro do objeto (ex: `carregarLista`, `abrirEdicao`, `salvarNovoModal`).
- **Comunicação com Backend:** Quase exclusivamente via `$.ajax`.
- **Renderização:** A renderização inicial é feita pelo Razor no servidor. Atualizações dinâmicas (novos dados em uma grid, conteúdo de um modal) são feitas injetando o HTML retornado de `PartialViews` via AJAX.

### 4.3. Padrões de UI/UX
- **CRUD em Abas:** Entidades complexas como "Produto" e "Pedido" são editadas em abas dinâmicas para permitir que o usuário trabalhe em múltiplos itens simultaneamente.
- **CRUD em Modal:** Entidades mais simples como "Categoria de Produto" ou a criação inicial de um item são feitas em modais do Bootstrap.
- **Grids Ricas:** O `DataTables.NET` é o padrão para listagem de dados.
- **Feedback ao Usuário:** `Toastr.js` é usado para feedback de operações (ex: "Pedido salvo com sucesso!"). Spinners são usados para indicar carregamento.

## 5. Infraestrutura e Implantação (DevOps)

### 5.1. Ambiente de Hospedagem
- **Provedor:** VPS (DigitalOcean, Vultr, etc.).
- **Sistema Operacional:** Debian.

### 5.2. Containerização
- **Tecnologia:** Docker.
- **Estratégia Multi-Tenant:** Um container Docker por tenant. O provisionamento de um novo tenant implica em criar um novo container com sua própria configuração e volume de dados.
- **Orquestração:** (Atualmente manual/scripted). Mencionar planos futuros para Kubernetes/Swarm se aplicável.

### 5.3. Proxy Reverso e Roteamento
- **Tecnologia:** Traefik.
- **Função:**
  - Ponto de entrada único para todas as requisições HTTP/S.
  - Gerenciamento de certificados SSL/TLS (Let's Encrypt).
  - Roteamento de requisições para o container do tenant correto baseado no subdomínio (ex: `clienteA.gesn.com.br`) ou outro identificador.

### 5.4. Processo de Build e Deploy (CI/CD)
- (Descrever o processo atual ou o processo desejado).
- **Exemplo de fluxo:**
  1. Push para o repositório Git (ex: `main` branch).
  2. Ação do GitHub Actions / Jenkins / Azure DevOps é acionada.
  3. Build do projeto .NET.
  4. Build da imagem Docker da aplicação.
  5. Push da imagem para um Docker Registry (Docker Hub, GitHub Packages).
  6. Conexão via SSH no VPS e execução de um script para `docker-compose pull` e `docker-compose up -d` para atualizar a aplicação.

## 6. Tópicos Transversais (Cross-Cutting Concerns)

### 6.1. Logging
- **Framework:** `ILogger` (implementação via Serilog, NLog, etc.).
- **Estratégia:** Logs são escritos para o console (coletados pelo Docker) e/ou para arquivos de log dentro do volume do container.

### 6.2. Tratamento de Erros
- **Backend:** Middlewares para capturar exceções não tratadas. Blocos `try-catch` nos controllers para erros esperados, retornando `JsonResult` com `{ success: false, message: "..." }`.
- **Frontend:** Uso dos callbacks `.fail()` ou `.error()` do `$.ajax` para tratar respostas de erro do servidor e exibir mensagens via `toastr.error()`.

### 6.3. Segurança
- **Prevenção de CSRF:** Uso de `[ValidateAntiForgeryToken]` em todas as ações `POST` que alteram estado.
- **Prevenção de XSS:** Razor Views encodam saídas por padrão. Cuidado extra na injeção de HTML via JavaScript.
- **Gerenciamento de Segredos:** Como as connection strings e outras chaves são gerenciadas (ex: `appsettings.json`, variáveis de ambiente do Docker).

## 7. Apêndices

### 7.1. Pacotes NuGet Principais
- **Dapper:** Micro-ORM de alta performance para executar queries SQL e mapear resultados para objetos C#.
- **Microsoft.Data.Sqlite:** Provedor de dados ADO.NET para interagir com o banco de dados SQLite.
- **Microsoft.AspNetCore.Identity:** Framework para gerenciamento de usuários, papéis, claims e autenticação. (Nota: A persistência pode ser customizada para Dapper ou usar o `Identity.EntityFrameworkCore` se for uma exceção).

### 7.2. Bibliotecas Client-Side
- **jquery:** Biblioteca fundamental para manipulação de DOM, eventos e AJAX.
- **bootstrap:** Framework CSS para layout, componentes visuais e responsividade.
- **datatables.net:** Plugin jQuery para criação de tabelas de dados avançadas.
- **select2:** Plugin jQuery que substitui e melhora os campos de seleção `<select>`.
- **toastr.js:** Biblioteca para exibir notificações "toast" não-intrusivas.
- **autocomplete.js:** Biblioteca para criar experiências de busca com autocompletar.
- **jquery-validation/jquery-validation-unobtrusive:** Plugins para validação de formulários no lado do cliente, integrados com os Data Annotations do .NET.
- **jquery.mask:** Plugin para aplicar máscaras de entrada em campos de formulário (ex: CPF, telefone, moeda).
- **jquery.qrcode:** Plugin para gerar QR Codes no lado do cliente.

### 7.3. Convenções de Código
- **C#:** Estilo de código (PascalCase para métodos e propriedades, etc.).
- **JavaScript:** Padrão "Manager Object", uso de `const` para o objeto principal, nomes de funções.
- **CSS:** Metodologia (ex: BEM) se aplicável, ou convenções de nomenclatura.
- **Git:** Convenções para nomes de branch e mensagens de commit.

