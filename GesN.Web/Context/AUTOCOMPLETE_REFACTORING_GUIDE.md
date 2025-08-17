# 🎯 GUIA DE REFATORAÇÃO - AUTOCOMPLETE PADRÃO

## 📋 **RESUMO EXECUTIVO**

Este documento define o **PADRÃO OURO** para implementação de autocomplete no sistema GesN, baseado na implementação **SUPERIOR** do `ProductComponentHierarchyName` em `_EditComponent.cshtml`.

**🎊 OBJETIVO**: Padronizar TODOS os autocomplementes do sistema usando a estrutura mais robusta e eficiente já implementada.

---

## 🏗️ **ARQUITETURA PADRÃO**

### **1. 📄 ESTRUTURA HTML**

#### **✅ TEMPLATE BASE:**
```html
<!-- Input visível para o usuário -->
<input type="text" 
       id="[EntityName]Name" 
       name="[EntityName]Name"
       value="@Model.[EntityName]Name"
       class="form-control floating-input autocomplete-input" 
       placeholder=" " 
       autocomplete="off"
       data-[context]-name="@Model.Name" />

<!-- Input hidden para armazenar o ID -->
<input type="hidden" 
       asp-for="[EntityName]Id" 
       id="[EntityName]Id" />

<!-- Label semântico -->
<label for="[EntityName]Name" class="floating-label">
    <i class="fas fa-[icon] text-primary"></i>
    [Display Name]
</label>

<!-- Validation span -->
<span asp-validation-for="[EntityName]Id" class="field-validation-valid text-danger"></span>

<!-- Help text (opcional) -->
<div class="form-text">
    <small class="text-muted">
        <i class="fas fa-info-circle"></i>
        Digite para buscar por [entidades] disponíveis
    </small>
</div>
```

#### **🎯 CONVENÇÕES OBRIGATÓRIAS:**
- **IDs FIXOS**: `[EntityName]Name` e `[EntityName]Id`
- **Classes CSS**: `autocomplete-input` sempre presente
- **Autocomplete OFF**: `autocomplete="off"` sempre
- **Data attributes**: Para contexto quando necessário
- **Floating labels**: Padrão do sistema
- **Icons**: FontAwesome relacionado à entidade

---

### **2. 🎯 JAVASCRIPT PADRÃO**

#### **✅ ESTRUTURA BASE:**
```javascript
// Método de inicialização no manager da entidade
inicializarAutocomplete[EntityName]: function(container) {
    const nameField = container.find('#[EntityName]Name');
    const idField = container.find('#[EntityName]Id');
    
    // ✅ VALIDAÇÃO: Verificar existência dos campos
    if (nameField.length === 0) {
        return;
    }

    // ✅ CLEANUP: Remove instância anterior se houver
    if (nameField.data('aaAutocomplete')) {
        nameField.autocomplete.destroy();
    }

    // ✅ ALGOLIA CONFIG: Configuração padrão
    const autocompleteInstance = autocomplete(nameField[0], {
        hint: false,
        debug: false,
        minLength: 2,
        openOnFocus: false,
        autoselect: true,
        appendTo: container[0] // ✅ CRUCIAL: Container correto
    }, [{
        source: function(query, callback) {
            $.ajax({
                url: '/[Controller]/Buscar[EntityName]Autocomplete',
                type: 'GET',
                dataType: 'json',
                data: { termo: query },
                success: function(data) {
                    const suggestions = $.map(data, function(item) {
                        return {
                            label: item.label,
                            value: item.value,
                            id: item.id,
                            description: item.description,
                            data: item
                        };
                    });
                    callback(suggestions);
                },
                error: function() {
                    callback([]);
                }
            });
        },
        displayKey: 'label',
        templates: {
            suggestion: function(suggestion) {
                return '<div class="autocomplete-suggestion">' +
                       '<div class="suggestion-title">' + (suggestion.data.name || suggestion.label) + '</div>' +
                       (suggestion.data.description ? '<div class="suggestion-subtitle">' + suggestion.data.description + '</div>' : '') +
                       '</div>';
            }
        }
    }]);

    // ✅ EVENT HANDLERS: Seleção
    autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
        idField.val(suggestion.id);
        nameField.val(suggestion.value);
        
        // ✅ UI UPDATES: Atualizar displays relacionados
        container.find('#display[EntityName]Name').text(suggestion.value);
        
        // ✅ INTEGRATION: Chamar métodos de atualização se existirem
        if (typeof [managerName].atualizarDisplay === 'function') {
            [managerName].atualizarDisplay(container);
        }
    });

    // ✅ VALIDATION: Limpar seleção se campo ficar vazio
    nameField.on('blur', function() {
        if ($(this).val() === '') {
            idField.val('');
            container.find('#display[EntityName]Name').text('-');
            
            if (typeof [managerName].atualizarDisplay === 'function') {
                [managerName].atualizarDisplay(container);
            }
        }
    });
},
```

#### **🎯 PONTOS CRÍTICOS:**
1. **Container-based**: Sempre usar `container.find()`
2. **Cleanup**: Sempre destruir instância anterior
3. **Error handling**: Callback vazio em caso de erro
4. **Event integration**: Integrar com outros componentes da UI
5. **Validation**: Limpar hidden field quando input vazio

---

### **3. 🎯 BACKEND API PADRÃO**

#### **✅ CONTROLLER ACTION:**
```csharp
/// <summary>
/// Endpoint para autocomplete de [EntityName]
/// </summary>
[HttpGet]
public async Task<IActionResult> Buscar[EntityName]Autocomplete(string termo)
{
    try
    {
        // ✅ VALIDAÇÃO: Minimum length check
        if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
            return Json(new List<object>());

        // ✅ SERVICE LAYER: Usar serviço especializado
        var entities = await _[entityName]Service.SearchAsync(termo);
        
        var result = entities
            .Where(e => e.StateCode == ObjectState.Active) // ✅ FILTRO: Apenas ativos
            .Take(10) // ✅ PERFORMANCE: Limitar resultados
            .Select(e => new [EntityName]AutocompleteViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description
            })
            .ToList();

        return Json(result);
    }
    catch (Exception ex)
    {
        // ✅ ERROR HANDLING: Log e retorno seguro
        _logger.LogError(ex, "Erro ao buscar [entidades] para autocomplete com termo: {Termo}", termo);
        return Json(new List<object>());
    }
}
```

#### **✅ VIEWMODEL PADRÃO:**
```csharp
/// <summary>
/// ViewModel para autocomplete de [EntityName]
/// </summary>
public class [EntityName]AutocompleteViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // ✅ COMPUTED PROPERTIES: Lógica no backend
    public string Label => !string.IsNullOrWhiteSpace(Description) ? 
        $"{Name} - {Description}" : Name;
    public string Value => Name;
}
```

---

## 📊 **MAPEAMENTO DE REFATORAÇÕES**

### **🔴 PRIORITY 1 - CRÍTICO (2-3 dias)**

#### **1. Category Autocomplete (Product.js)**
- **Localização**: `GesN.Web/wwwroot/js/Product.js`
- **Views afetadas**: `Product/_Create.cshtml`, `Product/_EditBasicData.cshtml`
- **Problemas atuais**: IDs dinâmicos, posicionamento modal complexo
- **Endpoint**: `/ProductCategory/BuscaProductCategoryAutocomplete` ✅ (já existe)

#### **2. Hierarchy Autocomplete (CompositeProduct.js)**
- **Localização**: `GesN.Web/wwwroot/js/CompositeProduct.js`
- **Views afetadas**: `ProductComponentHierarchy/_CreateCompositeProductXHierarchy.cshtml`
- **Problemas atuais**: Código duplicado, lógica complexa de modal
- **Endpoint**: `/ProductComponentHierarchy/BuscarHierarchiaDisponivel` ✅ (já existe)

### **🟡 PRIORITY 2 - IMPORTANTE (1 semana)**

#### **3. Product Autocomplete (ProductGroup.js)**
- **Localização**: `GesN.Web/wwwroot/js/ProductGroup.js`
- **Views afetadas**: Modais de ProductGroup
- **Problemas atuais**: Implementação similar mas não padronizada
- **Endpoint**: `/Product/BuscaProductAutocomplete` ✅ (já existe)

#### **4. Supplier Autocomplete**
- **Localização**: Views de Ingredient
- **Status**: **NÃO IMPLEMENTADO**
- **Endpoint necessário**: `/Supplier/BuscarSupplierAutocomplete` ❌ (criar)

#### **5. Customer Autocomplete**
- **Localização**: Views de Order
- **Status**: **NÃO IMPLEMENTADO**
- **Endpoint necessário**: `/Customer/BuscarCustomerAutocomplete` ❌ (criar)

### **🟢 PRIORITY 3 - FUTURO (2+ semanas)**

#### **6. User Autocomplete**
- **Localização**: Views de Permission/Role
- **Status**: **NÃO IMPLEMENTADO**
- **Endpoint necessário**: `/User/BuscarUserAutocomplete` ❌ (criar)

#### **7. Ingredient Autocomplete**
- **Localização**: Views de Product Components
- **Status**: **NÃO IMPLEMENTADO**
- **Endpoint necessário**: `/Ingredient/BuscarIngredientAutocomplete` ❌ (criar)

---

## 🛠️ **PROCESSO DE REFATORAÇÃO**

### **ETAPA 1: ANÁLISE PRÉ-REFATORAÇÃO**
1. **Identificar views** que usam o autocomplete atual
2. **Localizar JavaScript** responsável pela funcionalidade
3. **Verificar endpoint** backend (existe ou precisa criar?)
4. **Mapear ViewModels** necessários
5. **Identificar dependências** e integrações

### **ETAPA 2: IMPLEMENTAÇÃO BACKEND**
1. **Criar/ajustar Action** no Controller
2. **Criar ViewModel** específico para autocomplete
3. **Implementar Service method** se necessário
4. **Testar endpoint** via Postman/browser

### **ETAPA 3: REFATORAÇÃO FRONTEND**
1. **Ajustar HTML** seguindo template padrão
2. **Refatorar JavaScript** usando estrutura base
3. **Remover código legacy** (console.log, lógica complexa)
4. **Atualizar CSS** se necessário
5. **Testar integração** com outras funcionalidades

### **ETAPA 4: VALIDAÇÃO E TESTES**
1. **Testar em diferentes contextos** (modal, aba, página)
2. **Validar performance** (network, response time)
3. **Verificar error handling** (endpoint down, sem resultados)
4. **Confirmar integração** com validação e UI updates
5. **Testar edge cases** (caracteres especiais, query longa)

### **ETAPA 5: DOCUMENTAÇÃO**
1. **Atualizar este guia** se necessário
2. **Documentar peculiaridades** da implementação
3. **Registrar endpoints** novos criados
4. **Atualizar mapeamento** de prioridades

---

## 📝 **CHECKLIST DE QUALIDADE**

### **✅ HTML**
- [ ] IDs fixos e consistentes
- [ ] Classes CSS padronizadas  
- [ ] Autocomplete="off" presente
- [ ] Labels semânticos corretos
- [ ] Validation spans configurados
- [ ] Help text adequado

### **✅ JAVASCRIPT**
- [ ] Método de inicialização criado
- [ ] Container-based approach
- [ ] Cleanup de instâncias anteriores
- [ ] Error handling implementado
- [ ] Event handlers completos
- [ ] Integração com UI updates
- [ ] Sem console.log de debug

### **✅ BACKEND**
- [ ] Action no Controller criada
- [ ] Validação de entrada (min length)
- [ ] Service layer utilizado
- [ ] Filtros adequados (StateCode)
- [ ] Limitação de resultados (Take 10)
- [ ] Error handling e logging
- [ ] ViewModel específico

### **✅ QUALIDADE GERAL**
- [ ] Performance otimizada
- [ ] Funciona em todos os contextos
- [ ] Error handling robusto
- [ ] Código limpo e manutenível
- [ ] Documentação atualizada
- [ ] Testes validados

---

## 🎯 **EXEMPLOS DE IMPLEMENTAÇÃO**

### **EXEMPLO 1: Category Autocomplete**
```html
<!-- HTML -->
<input type="text" id="CategoryName" name="CategoryName" 
       class="form-control floating-input autocomplete-input" />
<input type="hidden" asp-for="CategoryId" id="CategoryId" />
```

```javascript
// JavaScript
inicializarAutocompleteCategory: function(container) {
    const nameField = container.find('#CategoryName');
    const idField = container.find('#CategoryId');
    // ... resto da implementação seguindo padrão
}
```

```csharp
// Backend
[HttpGet]
public async Task<IActionResult> BuscarCategoryAutocomplete(string termo)
{
    // ... implementação seguindo padrão
}
```

### **EXEMPLO 2: Supplier Autocomplete**
```html
<!-- HTML -->
<input type="text" id="SupplierName" name="SupplierName" 
       class="form-control floating-input autocomplete-input" />
<input type="hidden" asp-for="SupplierId" id="SupplierId" />
```

```javascript
// JavaScript  
inicializarAutocompleteSupplier: function(container) {
    const nameField = container.find('#SupplierName');
    const idField = container.find('#SupplierId');
    // ... resto da implementação seguindo padrão
}
```

---

## 🚨 **ANTI-PATTERNS A EVITAR**

### **❌ NÃO FAZER:**
1. **IDs dinâmicos** (`CategoryNameAutocomplete-@Model.Id`)
2. **Lógica complexa** de detecção de contexto modal
3. **Console.log** excessivo para debug
4. **Código duplicado** entre arquivos
5. **Posicionamento manual** com cálculos de offset
6. **appendTo: 'body'** em contextos modais
7. **Endpoints genéricos** sem filtros adequados
8. **ViewModels reutilizados** para diferentes contextos

### **✅ SEMPRE FAZER:**
1. **IDs fixos** e previsíveis
2. **Container-based** approach
3. **Cleanup** de instâncias anteriores
4. **Error handling** robusto
5. **Service layer** no backend
6. **ViewModels específicos** para autocomplete
7. **Validação** de entrada
8. **Performance** otimizada

---

## 📈 **MÉTRICAS DE SUCESSO**

### **ANTES DA REFATORAÇÃO:**
- ❌ Console poluído com debug logs
- ❌ Problemas de posicionamento em modais
- ❌ IDs dinâmicos causando conflitos
- ❌ Código duplicado entre arquivos
- ❌ Performance sub-ótima

### **DEPOIS DA REFATORAÇÃO:**
- ✅ Console limpo e profissional
- ✅ Funciona perfeitamente em qualquer contexto
- ✅ IDs fixos e previsíveis
- ✅ Código reutilizável e padronizado
- ✅ Performance otimizada
- ✅ Manutenibilidade alta
- ✅ Error handling robusto

---

## 🎊 **CONCLUSÃO**

Este guia define o **PADRÃO OURO** para autocomplementes no sistema GesN. Todas as futuras implementações e refatorações **DEVEM** seguir esta estrutura para garantir:

- **🚀 Performance** otimizada
- **🛡️ Robustez** e confiabilidade  
- **🎨 UX** consistente e superior
- **🔧 Manutenibilidade** alta
- **📱 Compatibilidade** total

**Referência base**: `ProductComponentHierarchyName` em `_EditComponent.cshtml`

---

*Documento criado em: $(Get-Date)*  
*Versão: 1.0*  
*Autor: Sistema GesN - Refatoração Autocomplete*