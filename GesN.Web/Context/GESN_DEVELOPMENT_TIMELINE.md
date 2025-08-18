# 📋 PLANEJAMENTO - IMPLEMENTAÇÃO DE DOMÍNIOS

## 🎯 SPRINT 1: Domínio Compartilhado + Fundações

### 1.1 Enumerações e Value Objects

**Enumerações:**
- OrderStatus (Pending, Confirmed, InProduction, Ready, Delivered, Canceled)
- OrderType (Takeaway, Delivery, Event)
- DocumentType (CPF, CNPJ, RG, Passport)
- TransactionType (Income, Expense)
- ProductionStatus (Pending, InProgress, Completed, Canceled)
- ObjectState (Active, Inactive)

**Value Objects:**
- Address (Street, Number, Complement, Neighborhood, City, State, ZipCode)
- FiscalData (DocumentNumber, DocumentType, StateRegistration)

### 1.2 Entidade Base
- Entity (Id, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy, IsActive)

### 1.3 Domínio de Vendas - Básico
- Customer (entidade, repository, service, controller, views, js)
- Contract (entidade, repository, service, controller, views, js)

## 🎯 SPRINT 2: Domínio de Vendas - Completo

### 2.1 Entidades Principais
- Order (entidade, repository, service, controller, views, js)
- OrderItem (entidade, repository, service, controller, views, js)

### 2.2 Relacionamentos
- Order → Customer (FK)
- Order → Contract (FK opcional)
- OrderItem → Order (FK)
- OrderItem → Product (FK)

## 🎯 SPRINT 3: Domínio de Produção - Básico

### 3.1 Entidades Fundamentais
- ProductCategory (entidade, repository, service, controller, views, js)
- Supplier (entidade, repository, service, controller, views, js)
- Ingredient (entidade, repository, service, controller, views, js)

### 3.2 Produto Base
- Product (classe abstrata + SimpleProduct)
- ProductIngredient (entidade de relacionamento many-to-many)

### 3.3 Relacionamentos
- Product → ProductCategory (FK)
- Ingredient → Supplier (FK opcional)
- ProductIngredient → Product (FK)
- ProductIngredient → Ingredient (FK)

## 🎯 SPRINT 4: Domínio de Produção - Avançado

### 4.1 Produtos Compostos
- CompositeProduct (herda de Product)
- ProductComponent (entidade de relacionamento)

### 4.2 Grupos de Produtos
- ProductGroup (herda de Product)
- ProductGroupItem (entidade de relacionamento)
- ProductGroupOption (configurações de opções)
- GroupExchangeRule (regras de troca)

### 4.3 Relacionamentos Complexos
- CompositeProduct → ProductComponent (1:N)
- ProductComponent → Product (FK para produto filho)
- ProductComponent → ProductIngredient (1:N)
- ProductGroup → ProductGroupItem (1:N)
- ProductGroupItem → Product (FK)
- ProductGroup → ProductGroupOption (1:N)
- ProductGroup → GroupExchangeRule (1:N)

### 4.4 Produção
- ProductionOrder (entidade, repository, service, controller, views, js)