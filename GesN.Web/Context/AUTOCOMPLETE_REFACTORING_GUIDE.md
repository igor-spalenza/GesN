# ðŸŽ¯ GUIA DE REFATORAÃ‡ÃƒO - AUTOCOMPLETE PADRÃƒO

## ðŸ“‹ **RESUMO EXECUTIVO**

Este documento define o **PADRÃƒO OURO** para implementaÃ§Ã£o de autocomplete no sistema GesN, baseado na implementaÃ§Ã£o **SUPERIOR** do `ProductComponentHierarchyName` em `_EditComponent.cshtml`.

**ðŸŽŠ OBJETIVO**: Padronizar TODOS os autocomplementes do sistema usando a estrutura mais robusta e eficiente jÃ¡ implementada.

---

## ðŸ—ï¸ **ARQUITETURA PADRÃƒO**

### **1. ðŸ“„ ESTRUTURA HTML**

#### **âœ… TEMPLATE BASE:**
```html
<!-- Input visÃ­vel para o usuÃ¡rio -->
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

<!-- Label semÃ¢ntico -->
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
        Digite para buscar por [entidades] disponÃ­veis
    </small>
</div>
```

#### **ðŸŽ¯ CONVENÃ‡Ã•ES OBRIGATÃ“RIAS:**
- **IDs FIXOS**: `[EntityName]Name` e `[EntityName]Id`
- **Classes CSS**: `autocomplete-input` sempre presente
- **Autocomplete OFF**: `autocomplete="off"` sempre
- **Data attributes**: Para contexto quando necessÃ¡rio
- **Floating labels**: PadrÃ£o do sistema
- **Icons**: FontAwesome relacionado Ã  entidade

---

### **2. ðŸŽ¯ JAVASCRIPT PADRÃƒO**

#### **âœ… ESTRUTURA BASE:**
```javascript
// MÃ©todo de inicializaÃ§Ã£o no manager da entidade
inicializarAutocomplete[EntityName]: function(container) {
    const nameField = container.find('#[EntityName]Name');
    const idField = container.find('#[EntityName]Id');
    
    // âœ… VALIDAÃ‡ÃƒO: Verificar existÃªncia dos campos
    if (nameField.length === 0) {
        return;
    }

    // âœ… CLEANUP: Remove instÃ¢ncia anterior se houver
    if (nameField.data('aaAutocomplete')) {
        nameField.autocomplete.destroy();
    }

    // âœ… ALGOLIA CONFIG: ConfiguraÃ§Ã£o padrÃ£o
    const autocompleteInstance = autocomplete(nameField[0], {
        hint: false,
        debug: false,
        minLength: 2,
        openOnFocus: false,
        autoselect: true,
        appendTo: container[0] // âœ… CRUCIAL: Container correto
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

    // âœ… EVENT HANDLERS: SeleÃ§Ã£o
    autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
        idField.val(suggestion.id);
        nameField.val(suggestion.value);
        
        // âœ… UI UPDATES: Atualizar displays relacionados
        container.find('#display[EntityName]Name').text(suggestion.value);
        
        // âœ… INTEGRATION: Chamar mÃ©todos de atualizaÃ§Ã£o se existirem
        if (typeof [managerName].atualizarDisplay === 'function') {
            [managerName].atualizarDisplay(container);
        }
    });

    // âœ… VALIDATION: Limpar seleÃ§Ã£o se campo ficar vazio
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

#### **ðŸŽ¯ PONTOS CRÃTICOS:**
1. **Container-based**: Sempre usar `container.find()`
2. **Cleanup**: Sempre destruir instÃ¢ncia anterior
3. **Error handling**: Callback vazio em caso de erro
4. **Event integration**: Integrar com outros componentes da UI
5. **Validation**: Limpar hidden field quando input vazio

---

### **3. ðŸŽ¯ BACKEND API PADRÃƒO**

#### **âœ… CONTROLLER ACTION:**
```csharp
/// <summary>
/// Endpoint para autocomplete de [EntityName]
/// </summary>
[HttpGet]
public async Task<IActionResult> Buscar[EntityName]Autocomplete(string termo)
{
    try
    {
        // âœ… VALIDAÃ‡ÃƒO: Minimum length check
        if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
            return Json(new List<object>());

        // âœ… SERVICE LAYER: Usar serviÃ§o especializado
        var entities = await _[entityName]Service.SearchAsync(termo);
        
        var result = entities
            .Where(e => e.StateCode == ObjectState.Active) // âœ… FILTRO: Apenas ativos
            .Take(10) // âœ… PERFORMANCE: Limitar resultados
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
        // âœ… ERROR HANDLING: Log e retorno seguro
        _logger.LogError(ex, "Erro ao buscar [entidades] para autocomplete com termo: {Termo}", termo);
        return Json(new List<object>());
    }
}
```

#### **âœ… VIEWMODEL PADRÃƒO:**
```csharp
/// <summary>
/// ViewModel para autocomplete de [EntityName]
/// </summary>
public class [EntityName]AutocompleteViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // âœ… COMPUTED PROPERTIES: LÃ³gica no backend
    public string Label => !string.IsNullOrWhiteSpace(Description) ? 
        $"{Name} - {Description}" : Name;
    public string Value => Name;
}
```

---

## ðŸ“Š **MAPEAMENTO DE REFATORAÃ‡Ã•ES**

### **ðŸ”´ PRIORITY 1 - CRÃTICO (2-3 dias)**

#### **1. Category Autocomplete (Product.js)**
- **LocalizaÃ§Ã£o**: `GesN.Web/wwwroot/js/Product.js`
- **Views afetadas**: `Product/_Create.cshtml`, `Product/_EditBasicData.cshtml`
- **Problemas atuais**: IDs dinÃ¢micos, posicionamento modal complexo
- **Endpoint**: `/ProductCategory/BuscaProductCategoryAutocomplete` âœ… (jÃ¡ existe)

#### **2. Hierarchy Autocomplete (CompositeProduct.js)**
- **LocalizaÃ§Ã£o**: `GesN.Web/wwwroot/js/CompositeProduct.js`
- **Views afetadas**: `ProductComponentHierarchy/_CreateCompositeProductXHierarchy.cshtml`
- **Problemas atuais**: CÃ³digo duplicado, lÃ³gica complexa de modal
- **Endpoint**: `/ProductComponentHierarchy/BuscarHierarchiaDisponivel` âœ… (jÃ¡ existe)

### **ðŸŸ¡ PRIORITY 2 - IMPORTANTE (1 semana)**

#### **3. Product Autocomplete (ProductGroup.js)**
- **LocalizaÃ§Ã£o**: `GesN.Web/wwwroot/js/ProductGroup.js`
- **Views afetadas**: Modais de ProductGroup
- **Problemas atuais**: ImplementaÃ§Ã£o similar mas nÃ£o padronizada
- **Endpoint**: `/Product/BuscaProductAutocomplete` âœ… (jÃ¡ existe)

#### **4. Supplier Autocomplete**
- **LocalizaÃ§Ã£o**: Views de Ingredient
- **Status**: **NÃƒO IMPLEMENTADO**
- **Endpoint necessÃ¡rio**: `/Supplier/BuscarSupplierAutocomplete` âŒ (criar)

#### **5. Customer Autocomplete**
- **LocalizaÃ§Ã£o**: Views de Order
- **Status**: **NÃƒO IMPLEMENTADO**
- **Endpoint necessÃ¡rio**: `/Customer/BuscarCustomerAutocomplete` âŒ (criar)

### **ðŸŸ¢ PRIORITY 3 - FUTURO (2+ semanas)**

#### **6. User Autocomplete**
- **LocalizaÃ§Ã£o**: Views de Permission/Role
- **Status**: **NÃƒO IMPLEMENTADO**
- **Endpoint necessÃ¡rio**: `/User/BuscarUserAutocomplete` âŒ (criar)

#### **7. Ingredient Autocomplete**
- **LocalizaÃ§Ã£o**: Views de Product Components
- **Status**: **NÃƒO IMPLEMENTADO**
- **Endpoint necessÃ¡rio**: `/Ingredient/BuscarIngredientAutocomplete` âŒ (criar)

---

## ðŸ› ï¸ **PROCESSO DE REFATORAÃ‡ÃƒO**

### **ETAPA 1: ANÃLISE PRÃ‰-REFATORAÃ‡ÃƒO**
1. **Identificar views** que usam o autocomplete atual
2. **Localizar JavaScript** responsÃ¡vel pela funcionalidade
3. **Verificar endpoint** backend (existe ou precisa criar?)
4. **Mapear ViewModels** necessÃ¡rios
5. **Identificar dependÃªncias** e integraÃ§Ãµes

### **ETAPA 2: IMPLEMENTAÃ‡ÃƒO BACKEND**
1. **Criar/ajustar Action** no Controller
2. **Criar ViewModel** especÃ­fico para autocomplete
3. **Implementar Service method** se necessÃ¡rio
4. **Testar endpoint** via Postman/browser

### **ETAPA 3: REFATORAÃ‡ÃƒO FRONTEND**
1. **Ajustar HTML** seguindo template padrÃ£o
2. **Refatorar JavaScript** usando estrutura base
3. **Remover cÃ³digo legacy** (console.log, lÃ³gica complexa)
4. **Atualizar CSS** se necessÃ¡rio
5. **Testar integraÃ§Ã£o** com outras funcionalidades

### **ETAPA 4: VALIDAÃ‡ÃƒO E TESTES**
1. **Testar em diferentes contextos** (modal, aba, pÃ¡gina)
2. **Validar performance** (network, response time)
3. **Verificar error handling** (endpoint down, sem resultados)
4. **Confirmar integraÃ§Ã£o** com validaÃ§Ã£o e UI updates
5. **Testar edge cases** (caracteres especiais, query longa)

### **ETAPA 5: DOCUMENTAÃ‡ÃƒO**
1. **Atualizar este guia** se necessÃ¡rio
2. **Documentar peculiaridades** da implementaÃ§Ã£o
3. **Registrar endpoints** novos criados
4. **Atualizar mapeamento** de prioridades

---

## ðŸ“ **CHECKLIST DE QUALIDADE**

### **âœ… HTML**
- [ ] IDs fixos e consistentes
- [ ] Classes CSS padronizadas  
- [ ] Autocomplete="off" presente
- [ ] Labels semÃ¢nticos corretos
- [ ] Validation spans configurados
- [ ] Help text adequado

### **âœ… JAVASCRIPT**
- [ ] MÃ©todo de inicializaÃ§Ã£o criado
- [ ] Container-based approach
- [ ] Cleanup de instÃ¢ncias anteriores
- [ ] Error handling implementado
- [ ] Event handlers completos
- [ ] IntegraÃ§Ã£o com UI updates
- [ ] Sem console.log de debug

### **âœ… BACKEND**
- [ ] Action no Controller criada
- [ ] ValidaÃ§Ã£o de entrada (min length)
- [ ] Service layer utilizado
- [ ] Filtros adequados (StateCode)
- [ ] LimitaÃ§Ã£o de resultados (Take 10)
- [ ] Error handling e logging
- [ ] ViewModel especÃ­fico

### **âœ… QUALIDADE GERAL**
- [ ] Performance otimizada
- [ ] Funciona em todos os contextos
- [ ] Error handling robusto
- [ ] CÃ³digo limpo e manutenÃ­vel
- [ ] DocumentaÃ§Ã£o atualizada
- [ ] Testes validados

---

## ðŸŽ¯ **EXEMPLOS DE IMPLEMENTAÃ‡ÃƒO**

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
    // ... resto da implementaÃ§Ã£o seguindo padrÃ£o
}
```

```csharp
// Backend
[HttpGet]
public async Task<IActionResult> BuscarCategoryAutocomplete(string termo)
{
    // ... implementaÃ§Ã£o seguindo padrÃ£o
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
    // ... resto da implementaÃ§Ã£o seguindo padrÃ£o
}
```

---

## ðŸš¨ **ANTI-PATTERNS A EVITAR**

### **âŒ NÃƒO FAZER:**
1. **IDs dinÃ¢micos** (`CategoryNameAutocomplete-@Model.Id`)
2. **LÃ³gica complexa** de detecÃ§Ã£o de contexto modal
3. **Console.log** excessivo para debug
4. **CÃ³digo duplicado** entre arquivos
5. **Posicionamento manual** com cÃ¡lculos de offset
6. **appendTo: 'body'** em contextos modais
7. **Endpoints genÃ©ricos** sem filtros adequados
8. **ViewModels reutilizados** para diferentes contextos

### **âœ… SEMPRE FAZER:**
1. **IDs fixos** e previsÃ­veis
2. **Container-based** approach
3. **Cleanup** de instÃ¢ncias anteriores
4. **Error handling** robusto
5. **Service layer** no backend
6. **ViewModels especÃ­ficos** para autocomplete
7. **ValidaÃ§Ã£o** de entrada
8. **Performance** otimizada

---

## ðŸ“ˆ **MÃ‰TRICAS DE SUCESSO**

### **ANTES DA REFATORAÃ‡ÃƒO:**
- âŒ Console poluÃ­do com debug logs
- âŒ Problemas de posicionamento em modais
- âŒ IDs dinÃ¢micos causando conflitos
- âŒ CÃ³digo duplicado entre arquivos
- âŒ Performance sub-Ã³tima

### **DEPOIS DA REFATORAÃ‡ÃƒO:**
- âœ… Console limpo e profissional
- âœ… Funciona perfeitamente em qualquer contexto
- âœ… IDs fixos e previsÃ­veis
- âœ… CÃ³digo reutilizÃ¡vel e padronizado
- âœ… Performance otimizada
- âœ… Manutenibilidade alta
- âœ… Error handling robusto

---

## ðŸŽŠ **CONCLUSÃƒO**

Este guia define o **PADRÃƒO OURO** para autocomplementes no sistema GesN. Todas as futuras implementaÃ§Ãµes e refatoraÃ§Ãµes **DEVEM** seguir esta estrutura para garantir:

- **ðŸš€ Performance** otimizada
- **ðŸ›¡ï¸ Robustez** e confiabilidade  
- **ðŸŽ¨ UX** consistente e superior
- **ðŸ”§ Manutenibilidade** alta
- **ðŸ“± Compatibilidade** total

**ReferÃªncia base**: `ProductComponentHierarchyName` em `_EditComponent.cshtml`

---

*Documento criado em: $(Get-Date)*  
*VersÃ£o: 1.0*  
*Autor: Sistema GesN - RefatoraÃ§Ã£o Autocomplete*
