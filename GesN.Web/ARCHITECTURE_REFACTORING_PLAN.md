# 🏗️ PLANO DE REFATORAÇÃO ARQUITETURAL - GesN

## 🎯 **OBJETIVO:**
Implementar padronização de respostas JSON, tratamento global de erros e AutoMapper para elevar a qualidade arquitetural do projeto.

---

## 📊 **ANÁLISE ATUAL vs PROPOSTA:**

### **❌ PROBLEMAS ATUAIS:**
```csharp
// Controllers inconsistentes:
return Json(new { success = true, data = customer });           // CustomerController
return Ok(new { message = "Sucesso", order = orderData });      // OrderController  
return BadRequest("Erro genérico");                            // Vários controllers

// Tratamento de erro manual:
try { /* ... */ } catch { return BadRequest("Erro"); }         // Repetido em todo lugar

// Mapeamento manual:
var viewModel = new CustomerViewModel {                         // Código repetitivo
    Id = entity.Id,
    Name = entity.Name,
    // ... 20+ propriedades
};
```

### **✅ ARQUITETURA PROPOSTA:**
```csharp
// Respostas padronizadas:
return Success(customer);                                       // ApiResponse<Customer>
return Error("Mensagem de erro", errors);                      // ApiResponse com erros
return NotFound<Customer>();                                    // ApiResponse<Customer> 404

// Tratamento global automático:
throw new BusinessException("Regra violada");                  // → Automaticamente vira ApiResponse

// Mapeamento automático:
var viewModel = _mapper.Map<CustomerViewModel>(entity);         // Uma linha
```

---

## 🏛️ **ESTRUTURA ARQUITETURAL:**

### **1. 📦 Models/Common/ApiResponse.cs**
```csharp
namespace GesN.Web.Models.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int StatusCode { get; set; }

        // Métodos estáticos para criação
        public static ApiResponse<T> SuccessResult(T data, string? message = null)
        public static ApiResponse<T> ErrorResult(string message, int statusCode = 400)
        public static ApiResponse<T> ValidationErrorResult(Dictionary<string, string[]> errors)
        public static ApiResponse<T> NotFoundResult(string? message = null)
    }

    // Versão não-genérica para operações sem dados
    public class ApiResponse : ApiResponse<object> { }
}
```

### **2. 🎯 Controllers/Base/BaseController.cs**
```csharp
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly IMapper _mapper;
    protected readonly ILogger<BaseController> _logger;

    protected BaseController(IMapper mapper, ILogger<BaseController> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    // Métodos utilitários para respostas padronizadas
    protected IActionResult Success<T>(T data, string? message = null)
    {
        var response = ApiResponse<T>.SuccessResult(data, message);
        return Ok(response);
    }

    protected IActionResult Error<T>(string message, int statusCode = 400)
    {
        var response = ApiResponse<T>.ErrorResult(message, statusCode);
        return StatusCode(statusCode, response);
    }

    protected IActionResult ValidationError<T>(ModelStateDictionary modelState)
    {
        var errors = modelState.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
        );
        var response = ApiResponse<T>.ValidationErrorResult(errors);
        return BadRequest(response);
    }

    protected IActionResult NotFound<T>(string? message = null)
    {
        var response = ApiResponse<T>.NotFoundResult(message);
        return NotFound(response);
    }
}
```

### **3. ⚠️ Infrastructure/Exceptions/BusinessExceptions.cs**
```csharp
namespace GesN.Web.Infrastructure.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; }
        public Dictionary<string, string[]>? ValidationErrors { get; }

        public BusinessException(string message, int statusCode = 400) 
            : base(message)
        {
            StatusCode = statusCode;
        }

        public BusinessException(string message, Dictionary<string, string[]> validationErrors) 
            : base(message)
        {
            StatusCode = 400;
            ValidationErrors = validationErrors;
        }
    }

    public class NotFoundException : BusinessException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }

    public class ValidationException : BusinessException
    {
        public ValidationException(Dictionary<string, string[]> errors) 
            : base("Validation failed", errors) { }
    }
}
```

### **4. 🛡️ Infrastructure/Middleware/GlobalExceptionMiddleware.cs**
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            BusinessException be when be.ValidationErrors != null => 
                ApiResponse<object>.ValidationErrorResult(be.ValidationErrors),
            BusinessException be => 
                ApiResponse<object>.ErrorResult(be.Message, be.StatusCode),
            NotFoundException => 
                ApiResponse<object>.NotFoundResult(exception.Message),
            _ => ApiResponse<object>.ErrorResult("An internal server error occurred", 500)
        };

        context.Response.StatusCode = response.StatusCode;
        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}
```

### **5. 🗺️ Infrastructure/Mapping/MappingProfile.cs**
```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Customer mappings
        CreateMap<Customer, CustomerViewModel>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == EntityStatus.Active));

        CreateMap<CustomerCreateDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Order mappings
        CreateMap<Order, OrderViewModel>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.OrderItems.Count));

        CreateMap<OrderCreateDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Adicionar outros mapeamentos conforme necessário
    }
}
```

---

## 🔧 **IMPLEMENTAÇÃO PASSO A PASSO:**

### **FASE 1: Fundação (Semana 1)**
1. ✅ Criar `ApiResponse<T>` e classes de exceção
2. ✅ Implementar `GlobalExceptionMiddleware`
3. ✅ Configurar AutoMapper no `Program.cs`
4. ✅ Criar `BaseController`

### **FASE 2: Migração Controllers (Semana 2)**
1. ✅ Migrar `CustomerController` → usar `BaseController`
2. ✅ Migrar `OrderController` → usar `BaseController`
3. ✅ Migrar controllers restantes
4. ✅ Remover tratamento de erro manual

### **FASE 3: Frontend (Semana 3)**
1. ✅ Atualizar TypeScript interfaces para `ApiResponse<T>`
2. ✅ Refatorar managers para nova estrutura
3. ✅ Melhorar tratamento de erros no frontend

### **FASE 4: Otimização (Semana 4)**
1. ✅ Adicionar validação automática com FluentValidation
2. ✅ Implementar logging estruturado
3. ✅ Testes unitários para nova arquitetura

---

## 🎯 **BENEFÍCIOS DA REFATORAÇÃO:**

### **📈 Qualidade de Código:**
- ✅ **Consistência:** Todas as APIs retornam o mesmo formato
- ✅ **Manutenibilidade:** Tratamento centralizado de erros
- ✅ **DRY:** Elimina código repetitivo de mapeamento

### **🚀 Produtividade:**
- ✅ **Desenvolvimento:** Novos controllers seguem padrão automático
- ✅ **Debug:** Erros padronizados facilitam debugging
- ✅ **Frontend:** Estrutura previsível de dados

### **🛡️ Robustez:**
- ✅ **Error Handling:** Nenhuma exceção passa despercebida
- ✅ **Logging:** Todos os erros são logados automaticamente
- ✅ **Validation:** Validação centralizada e consistente

---

## 📊 **EXEMPLO DE USO FINAL:**

### **Controller Antes:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateDto dto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(/* ... código repetitivo ... */);
            return BadRequest(new { success = false, errors = errors });
        }

        var entity = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            // ... mapeamento manual de 10+ propriedades
        };

        var created = await _service.CreateAsync(entity);
        
        var viewModel = new CustomerViewModel
        {
            Id = created.Id,
            Name = created.Name,
            // ... mapeamento manual de volta
        };

        return Json(new { success = true, data = viewModel, message = "Cliente criado com sucesso" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao criar cliente");
        return BadRequest(new { success = false, message = "Erro interno" });
    }
}
```

### **Controller Depois:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateDto dto)
{
    if (!ModelState.IsValid)
        return ValidationError<CustomerViewModel>(ModelState);

    var entity = _mapper.Map<Customer>(dto);
    var created = await _service.CreateAsync(entity);
    var viewModel = _mapper.Map<CustomerViewModel>(created);

    return Success(viewModel, "Cliente criado com sucesso");
}
// Tratamento de erros automático via middleware
```

### **Frontend TypeScript:**
```typescript
// Antes - inconsistente:
if (response.success) { /* ... */ }
if (response.ok) { /* ... */ }
if (response.status === 'success') { /* ... */ }

// Depois - consistente:
const response: ApiResponse<Customer> = await this.apiCall();
if (response.success) {
    this.handleSuccess(response.data, response.message);
} else {
    this.handleErrors(response.errors);
}
```

---

## 🚀 **PRÓXIMOS PASSOS:**

1. **Aprovação do plano** → Confirmar arquitetura proposta
2. **Setup inicial** → Criar classes base e configurações
3. **Migração incremental** → Um controller por vez
4. **Testes** → Validar cada etapa
5. **Deploy** → Liberação gradual

**💡 Esta refatoração deixará o código mais profissional, maintível e escalável!**

**Quer começar pela implementação das classes base?** 🎯

