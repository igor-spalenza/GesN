# 🔄 Estratégia de Refatoração Frontend - GesN

## 🎯 Workflow v0 → Razor Pages Adaptation

### **FASE 1: Geração com v0**
1. **Identificar componente** para refatoração
2. **Descrever no v0** o que você quer
3. **Gerar componente React** no v0
4. **Analisar estrutura** e CSS gerado

### **FASE 2: Adaptação para Razor**
1. **Extrair HTML structure** do JSX
2. **Converter CSS** para SCSS
3. **Adaptar JavaScript** para TypeScript
4. **Implementar em Razor Pages**

---

## 📋 EXEMPLO PRÁTICO

### **1. Prompt no v0:**
```
"Create a modern product card component with:
- Product image placeholder
- Product name and description  
- Price display with currency
- Add to cart button
- Responsive design
- Modern shadows and hover effects"
```

### **2. Código React Gerado (v0):**
```jsx
// Exemplo do que v0 geraria
import React from 'react';

export default function ProductCard({ product }) {
  return (
    <div className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300 overflow-hidden">
      <div className="aspect-square w-full bg-gray-200 relative">
        <img 
          src={product.image || "/placeholder.jpg"} 
          alt={product.name}
          className="w-full h-full object-cover"
        />
      </div>
      <div className="p-4">
        <h3 className="font-semibold text-lg text-gray-900 mb-2">
          {product.name}
        </h3>
        <p className="text-gray-600 text-sm mb-3 line-clamp-2">
          {product.description}
        </p>
        <div className="flex items-center justify-between">
          <span className="text-2xl font-bold text-green-600">
            ${product.price}
          </span>
          <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg transition-colors duration-200">
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  );
}
```

### **3. Adaptação para Razor (.cshtml):**
```html
<!-- Views/Shared/_ProductCard.cshtml -->
@model Product

<div class="product-card">
    <div class="product-image">
        <img src="@(Model.ImageUrl ?? "/img/placeholder.jpg")" 
             alt="@Model.Name" />
    </div>
    <div class="product-content">
        <h3 class="product-name">@Model.Name</h3>
        <p class="product-description">@Model.Description</p>
        <div class="product-footer">
            <span class="product-price">@Model.Price.ToString("C")</span>
            <button type="button" 
                    class="btn-add-cart" 
                    onclick="addToCart(@Model.Id)">
                Adicionar
            </button>
        </div>
    </div>
</div>
```

### **4. CSS Convertido (SCSS):**
```scss
// Styles/components/_product-card.scss
@import '../variables';
@import '../mixins';

.product-card {
    background: $bg-white;
    border-radius: $border-radius-lg;
    box-shadow: $box-shadow;
    overflow: hidden;
    transition: $transition-base;
    
    @include hover-lift;

    .product-image {
        aspect-ratio: 1;
        width: 100%;
        background: lighten($border-color, 10%);
        position: relative;

        img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }
    }

    .product-content {
        padding: 1rem;

        .product-name {
            font-weight: 600;
            font-size: 1.125rem;
            color: $text-dark;
            margin-bottom: 0.5rem;
        }

        .product-description {
            color: $text-muted;
            font-size: $font-size-sm;
            margin-bottom: 0.75rem;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .product-footer {
            display: flex;
            align-items: center;
            justify-content: space-between;

            .product-price {
                font-size: 1.5rem;
                font-weight: 700;
                color: $success-color;
            }

            .btn-add-cart {
                @include button-variant($primary-color);
                padding: 0.5rem 1rem;
                border-radius: $border-radius;
                font-size: $font-size-sm;
                transition: $transition-base;
            }
        }
    }
}
```

### **5. TypeScript para Interações:**
```typescript
// TypeScript/components/ProductCard.ts
import { Product } from '../interfaces/product';
import { CartManager } from '../managers/CartManager';

export class ProductCardComponent {
    private cartManager: CartManager;

    constructor() {
        this.cartManager = new CartManager();
        this.initializeEvents();
    }

    private initializeEvents(): void {
        $(document).on('click', '.btn-add-cart', (event) => {
            const productId = parseInt($(event.currentTarget).data('product-id'));
            this.addToCart(productId);
        });
    }

    private async addToCart(productId: number): Promise<void> {
        try {
            await this.cartManager.addItem(productId, 1);
            this.showSuccessFeedback();
        } catch (error) {
            this.showErrorFeedback(error.message);
        }
    }

    private showSuccessFeedback(): void {
        // Animação de sucesso, toastr, etc.
        toastr.success('Produto adicionado ao carrinho!');
    }

    private showErrorFeedback(message: string): void {
        toastr.error(`Erro: ${message}`);
    }
}
```

---

## 🛠️ FERRAMENTAS COMPLEMENTARES

### **Cursor Extensions Úteis:**
- **Tailwind to CSS**: Converte classes Tailwind
- **React to HTML**: Converte JSX para HTML
- **CSS to SCSS**: Converte CSS vanilla para SCSS

### **Prompts Cursor Customizados:**
```
"Convert this React JSX component to Razor Pages partial view, maintaining the same structure but using ASP.NET Core syntax"

"Adapt this Tailwind CSS to SCSS using our existing variable system in _variables.scss"

"Convert this React event handler to TypeScript for use with jQuery in our ASP.NET Core project"
```

---

## 📊 ÁREAS PRIORITÁRIAS PARA REFATORAÇÃO

### **1. Sistema de Cards (Maior Impacto)**
- Product cards
- Customer cards  
- Order cards
- Statistics cards

### **2. Formulários Complexos**
- Product creation/edit
- Customer management
- Order processing

### **3. Navegação e Layout**
- Tab systems
- Modal layouts
- Responsive navigation

### **4. Data Visualization**
- Statistics dashboards
- Charts and graphs
- Data tables

---

## 🎯 PROCESSO RECOMENDADO

### **Week 1-2: Setup & First Components**
1. Configure Cursor com extensions
2. Refactor 2-3 card components usando v0
3. Create SCSS component library
4. Test integration workflow

### **Week 3-4: Complex Forms** 
1. Use v0 para form designs modernos
2. Adapt para floating labels existentes
3. Integrate com TypeScript validation
4. Test com sistema atual

### **Week 5-6: Layout Systems**
1. Modernize tab systems
2. Improve modal layouts  
3. Enhance responsive design
4. Performance optimization

---

## 📝 CHECKLIST DE ADAPTAÇÃO

### **Para cada componente v0:**
- [ ] ✅ HTML structure extraída
- [ ] ✅ CSS convertido para SCSS
- [ ] ✅ JavaScript adaptado para TypeScript
- [ ] ✅ Integrado com sistema existente
- [ ] ✅ Testado em diferentes viewports
- [ ] ✅ Validado com dados reais

### **Qualidade assurance:**
- [ ] ✅ Funciona com dados do backend
- [ ] ✅ Mantém funcionalidades existentes
- [ ] ✅ Performance não degradada
- [ ] ✅ Acessibilidade mantida
- [ ] ✅ Cross-browser compatibility

---

**🚀 Este workflow permite aproveitar o poder do v0 mesmo em projetos ASP.NET Core!**
