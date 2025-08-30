# 🚀 Guia de Implementação TypeScript + SCSS - GesN

Este documento detalha como usar TypeScript e SCSS no projeto GesN.

## 📁 Estrutura de Pastas

```
GesN.Web/
├── TypeScript/           ← Source files TypeScript
│   ├── interfaces/       ← Definições de tipos
│   │   ├── common.ts     ← Interfaces comuns
│   │   └── customer.ts   ← Interfaces específicas
│   └── managers/         ← Lógica de negócio
│       └── CustomerManager.ts
├── Styles/               ← Source files SCSS
│   ├── _variables.scss   ← Variáveis globais
│   ├── _mixins.scss      ← Mixins utilitários
│   ├── site.scss         ← Estilo principal
│   └── domains/          ← Estilos por domínio
│       └── ProductDomain.scss
└── wwwroot/
    ├── js/               ← JavaScript compilado (output)
    └── css/              ← CSS compilado (output)
```

## 🛠️ Como Compilar

### **Automático (Recomendado)**
- **TypeScript**: Compila automaticamente ao salvar (se configurado no VS)
- **SCSS**: Compila automaticamente durante o build

### **Manual**
```bash
# Build completo
dotnet build

# Apenas TypeScript (se necessário)
tsc

# Watch mode para desenvolvimento
tsc --watch
```

## 📝 Como Usar

### **1. Criando Nova Interface**

```typescript
// TypeScript/interfaces/product.ts
export interface Product {
    id: number;
    name: string;
    price: number;
}
```

### **2. Criando Novo Manager**

```typescript
// TypeScript/managers/ProductManager.ts
import { Product } from '../interfaces/product';

class ProductManager {
    async getProduct(id: number): Promise<Product> {
        const response = await $.ajax(`/Product/${id}`);
        return response.data;
    }
}
```

### **3. Criando Estilos SCSS**

```scss
// Styles/domains/NewDomain.scss
@import '../variables';
@import '../mixins';

.new-domain {
    color: $primary-color;
    
    &:hover {
        @include hover-lift;
    }
}
```

## 🔄 Processo de Migração

### **Passo 1: Converter JavaScript → TypeScript**
1. Copie arquivo `.js` para TypeScript/managers/
2. Renomeie para `.ts`
3. Adicione tipos nas funções
4. Importe interfaces necessárias

### **Passo 2: Converter CSS → SCSS**
1. Copie arquivo `.css` para Styles/
2. Renomeie para `.scss`
3. Use variáveis do `_variables.scss`
4. Aplique mixins do `_mixins.scss`

### **Passo 3: Atualizar Views**
```html
<!-- Antes -->
<script src="~/js/Customer.js"></script>

<!-- Depois (automaticamente compilado) -->
<script src="~/js/CustomerManager.js"></script>
```

## ⚠️ Troubleshooting

### **Erros de Compilação TypeScript**
```bash
# Verificar erros
dotnet build

# Limpar cache se necessário
dotnet clean
dotnet build
```

### **SCSS Não Compila**
- Verifique se AspNetCore.SassCompiler está instalado
- Confirme que arquivos estão em `Styles/`
- Build o projeto novamente

### **IntelliSense não funciona**
- Reinicie Visual Studio
- Verifique se `tsconfig.json` está correto
- Confirme extensões TypeScript instaladas

## 📊 Benefícios Obtidos

### **TypeScript**
✅ Type safety em todas as funções  
✅ IntelliSense completo  
✅ Detecção de erros em tempo real  
✅ Refatoração segura  

### **SCSS**
✅ Variáveis compartilhadas  
✅ Mixins reutilizáveis  
✅ Código CSS mais organizado  
✅ Funcionalidades avançadas (nesting, functions)  

## 🎯 Próximos Passos

1. **Migrar Customer.js** → `CustomerManager.ts` ✅ (Exemplo pronto)
2. **Migrar Product.js** → `ProductManager.ts`
3. **Migrar Order.js** → `OrderManager.ts`
4. **Converter CSS restantes** para SCSS
5. **Criar interfaces** para todas as entidades

## 📚 Recursos Adicionais

- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Sass Documentation](https://sass-lang.com/documentation)
- [ASP.NET Core com TypeScript](https://docs.microsoft.com/en-us/aspnet/core/client-side/spa-services)

---
**🎉 Implementação concluída! O projeto está pronto para usar TypeScript e SCSS.**


