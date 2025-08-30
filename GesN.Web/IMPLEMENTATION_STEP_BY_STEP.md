# 🔧 IMPLEMENTAÇÃO PASSO A PASSO - REFATORAÇÃO ARQUITETURAL

## 🎯 **ORDEM DE IMPLEMENTAÇÃO:**

**Fase 1:** Fundação (Classes Base) ✅  
**Fase 2:** Configuração (DI + Middleware) ✅  
**Fase 3:** Migração (Controllers) ⏳  
**Fase 4:** Frontend (TypeScript) ⏳  

---

## 📦 **FASE 1: CRIAÇÃO DAS CLASSES BASE**

### **1.1 - Models/Common/ApiResponse.cs**
```csharp
using System.Text.Json.Serialization;

namespace GesN.Web.Models.Common
{
    /// <summary>
    /// Resposta padrão para todas as APIs do sistema
    /// Garante consistência entre diferentes endpoints
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem para o usuário (opcional)
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Dados retornados pela API (quando sucesso)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Erros de validação organizados por campo
        /// </summary>
        public Dictionary<string, string[]>? Errors { get; set; }

        /// <summary>
        /// Timestamp da resposta
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Status code HTTP
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ApiResponse() { }

        /// <summary>
        /// Construtor para resposta de sucesso
        /// </summary>
        public ApiResponse(T data, string? message = null, int statusCode = 200)
        {
            Success = true;
            Data = data;
            Message = message;
            StatusCode = statusCode;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor para resposta de erro
        /// </summary>
        public ApiResponse(string message, int statusCode = 400)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor para erros de validação
        /// </summary>
        public ApiResponse(Dictionary<string, string[]> errors, string? message = null)
        {
            Success = false;
            Message = message ?? "Dados inválidos";
            Errors = errors;
            StatusCode = 400;
            Timestamp = DateTime.UtcNow;
        }

        #region Factory Methods

        /// <summary>
        /// Criar resposta de sucesso com dados
        /// </summary>
        public static ApiResponse<T> SuccessResult(T data, string? message = null)
        {
            return new ApiResponse<T>(data, message, 200);
        }

        /// <summary>
        /// Criar resposta de erro
        /// </summary>
        public static ApiResponse<T> ErrorResult(string message, int statusCode = 400)
        {
            return new ApiResponse<T>(message, statusCode);
        }

        /// <summary>
        /// Criar resposta de erro de validação
        /// </summary>
        public static ApiResponse<T> ValidationErrorResult(Dictionary<string, string[]> errors, string? message = null)
        {
            return new ApiResponse<T>(errors, message);
        }

        /// <summary>
        /// Criar resposta de Not Found
        /// </summary>
        public static ApiResponse<T> NotFoundResult(string? message = null)
        {
            return new ApiResponse<T>(message ?? "Recurso não encontrado", 404);
        }

        /// <summary>
        /// Criar resposta de acesso negado
        /// </summary>
        public static ApiResponse<T> UnauthorizedResult(string? message = null)
        {
            return new ApiResponse<T>(message ?? "Acesso negado", 401);
        }

        #endregion
    }

    /// <summary>
    /// Versão não-genérica para operações sem dados de retorno
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() : base() { }
        public ApiResponse(string message, int statusCode = 400) : base(message, statusCode) { }
        public ApiResponse(Dictionary<string, string[]> errors, string? message = null) : base(errors, message) { }

        /// <summary>
        /// Criar resposta de sucesso sem dados
        /// </summary>
        public static ApiResponse SuccessResult(string? message = null)
        {
            return new ApiResponse 
            { 
                Success = true, 
                Message = message, 
                StatusCode = 200, 
                Timestamp = DateTime.UtcNow 
            };
        }
    }
}
```

### **1.2 - Infrastructure/Exceptions/BusinessExceptions.cs**
```csharp
namespace GesN.Web.Infrastructure.Exceptions
{
    /// <summary>
    /// Exceção base para regras de negócio
    /// Será capturada pelo middleware global e transformada em ApiResponse
    /// </summary>
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

        public BusinessException(string message, Exception innerException, int statusCode = 400)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Exceção para recursos não encontrados (404)
    /// </summary>
    public class NotFoundException : BusinessException
    {
        public NotFoundException(string message) : base(message, 404) { }
        public NotFoundException(string resource, object id) 
            : base($"{resource} com ID '{id}' não foi encontrado", 404) { }
    }

    /// <summary>
    /// Exceção para erros de validação (400)
    /// </summary>
    public class ValidationException : BusinessException
    {
        public ValidationException(Dictionary<string, string[]> errors, string? message = null) 
            : base(message ?? "Erro de validação", errors) { }
            
        public ValidationException(string field, string error)
            : base("Erro de validação", new Dictionary<string, string[]> 
            { 
                { field, new[] { error } } 
            }) { }
    }

    /// <summary>
    /// Exceção para acesso negado (401/403)
    /// </summary>
    public class UnauthorizedException : BusinessException
    {
        public UnauthorizedException(string message = "Acesso negado") : base(message, 401) { }
    }

    /// <summary>
    /// Exceção para conflitos (409) - ex: duplicação de dados únicos
    /// </summary>
    public class ConflictException : BusinessException
    {
        public ConflictException(string message) : base(message, 409) { }
        public ConflictException(string resource, string field, object value)
            : base($"{resource} com {field} '{value}' já existe", 409) { }
    }
}
```

### **1.3 - Infrastructure/Middleware/GlobalExceptionMiddleware.cs**
```csharp
using GesN.Web.Infrastructure.Exceptions;
using GesN.Web.Models.Common;
using System.Net;
using System.Text.Json;

namespace GesN.Web.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware para captura global de exceções
    /// Transforma todas as exceções em ApiResponse padronizadas
    /// </summary>
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log da exceção
            _logger.LogError(exception, "Exceção capturada pelo middleware global: {Message}", exception.Message);

            // Configurar response
            context.Response.ContentType = "application/json";

            // Criar resposta baseada no tipo de exceção
            var response = CreateErrorResponse(exception);
            context.Response.StatusCode = response.StatusCode;

            // Serializar e enviar resposta
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private ApiResponse<object> CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                // Exceções de negócio com validação
                BusinessException be when be.ValidationErrors != null =>
                    ApiResponse<object>.ValidationErrorResult(be.ValidationErrors, be.Message),

                // Exceções de negócio simples
                BusinessException be =>
                    ApiResponse<object>.ErrorResult(be.Message, be.StatusCode),

                // Exceção específica de Not Found
                NotFoundException nfe =>
                    ApiResponse<object>.NotFoundResult(nfe.Message),

                // Exceções de validação
                ValidationException ve =>
                    ApiResponse<object>.ValidationErrorResult(ve.ValidationErrors!, ve.Message),

                // Exceções de acesso
                UnauthorizedException ue =>
                    ApiResponse<object>.UnauthorizedResult(ue.Message),

                // Exceções de conflito
                ConflictException ce =>
                    ApiResponse<object>.ErrorResult(ce.Message, 409),

                // Exceções não tratadas (500)
                _ => ApiResponse<object>.ErrorResult("Erro interno do servidor", 500)
            };
        }
    }
}
```

### **1.4 - Controllers/Base/BaseController.cs**
```csharp
using AutoMapper;
using GesN.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace GesN.Web.Controllers.Base
{
    /// <summary>
    /// Controller base com métodos utilitários para respostas padronizadas
    /// Todos os controllers devem herdar desta classe
    /// </summary>
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

        #region User Info

        /// <summary>
        /// Obter ID do usuário logado
        /// </summary>
        protected string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
        }

        /// <summary>
        /// Obter nome do usuário logado
        /// </summary>
        protected string GetUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";
        }

        #endregion

        #region Success Responses

        /// <summary>
        /// Retorna resposta de sucesso com dados
        /// </summary>
        protected IActionResult Success<T>(T data, string? message = null)
        {
            var response = ApiResponse<T>.SuccessResult(data, message);
            return Ok(response);
        }

        /// <summary>
        /// Retorna resposta de sucesso sem dados
        /// </summary>
        protected IActionResult Success(string? message = null)
        {
            var response = ApiResponse.SuccessResult(message);
            return Ok(response);
        }

        /// <summary>
        /// Retorna resposta de sucesso para criação (201)
        /// </summary>
        protected IActionResult Created<T>(T data, string? message = null)
        {
            var response = new ApiResponse<T>(data, message, 201);
            return StatusCode(201, response);
        }

        #endregion

        #region Error Responses

        /// <summary>
        /// Retorna erro genérico
        /// </summary>
        protected IActionResult Error<T>(string message, int statusCode = 400)
        {
            var response = ApiResponse<T>.ErrorResult(message, statusCode);
            return StatusCode(statusCode, response);
        }

        /// <summary>
        /// Retorna erro de validação baseado no ModelState
        /// </summary>
        protected IActionResult ValidationError<T>(ModelStateDictionary modelState)
        {
            var errors = modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
            );

            var response = ApiResponse<T>.ValidationErrorResult(errors);
            return BadRequest(response);
        }

        /// <summary>
        /// Retorna erro de validação com erros customizados
        /// </summary>
        protected IActionResult ValidationError<T>(Dictionary<string, string[]> errors, string? message = null)
        {
            var response = ApiResponse<T>.ValidationErrorResult(errors, message);
            return BadRequest(response);
        }

        /// <summary>
        /// Retorna erro de recurso não encontrado
        /// </summary>
        protected IActionResult NotFound<T>(string? message = null)
        {
            var response = ApiResponse<T>.NotFoundResult(message);
            return NotFound(response);
        }

        /// <summary>
        /// Retorna erro de acesso negado
        /// </summary>
        protected IActionResult Unauthorized<T>(string? message = null)
        {
            var response = ApiResponse<T>.UnauthorizedResult(message);
            return Unauthorized(response);
        }

        #endregion

        #region Validation Helpers

        /// <summary>
        /// Valida se o modelo é válido e retorna erro se não for
        /// </summary>
        protected IActionResult? ValidateModel<T>()
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<T>(ModelState);
            }
            return null;
        }

        /// <summary>
        /// Adiciona erro de validação ao ModelState
        /// </summary>
        protected void AddValidationError(string key, string message)
        {
            ModelState.AddModelError(key, message);
        }

        #endregion

        #region Logging Helpers

        /// <summary>
        /// Log de erro com contexto do usuário
        /// </summary>
        protected void LogError(Exception ex, string message, params object[] args)
        {
            var userId = GetUserId();
            var contextMessage = $"[User: {userId}] {message}";
            _logger.LogError(ex, contextMessage, args);
        }

        /// <summary>
        /// Log de informação com contexto do usuário
        /// </summary>
        protected void LogInfo(string message, params object[] args)
        {
            var userId = GetUserId();
            var contextMessage = $"[User: {userId}] {message}";
            _logger.LogInformation(contextMessage, args);
        }

        #endregion
    }
}
```

---

## ⚙️ **FASE 2: CONFIGURAÇÃO**

### **2.1 - Configuração no Program.cs**
```csharp
// Adicionar após builder.Services.AddControllersWithViews();

// ✅ AUTOMAPPER
builder.Services.AddAutoMapper(typeof(Program));

// ✅ EXCEPTION MIDDLEWARE (será adicionado depois do app = builder.Build())
```

### **2.2 - Adicionar Middleware (após app = builder.Build())**
```csharp
// ✅ EXCEPTION MIDDLEWARE - deve vir antes de outros middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();

// ... outros middlewares existentes
app.UseStaticFiles();
// ...
```

### **2.3 - AutoMapper Profile (criar)**
```csharp
// Infrastructure/Mapping/MappingProfile.cs
using AutoMapper;
using GesN.Web.Models.Entities;
using GesN.Web.Models.ViewModels;
using GesN.Web.Models.DTOs;

namespace GesN.Web.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Customer mappings
            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.StateCode == ObjectState.Active));

            CreateMap<CreateCustomerViewModel, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => ObjectState.Active));

            // Adicionar outros mapeamentos conforme necessário...
        }
    }
}
```

---

## 🎯 **PRÓXIMOS PASSOS:**

### **✅ Para implementar agora:**
1. **Criar as 4 classes** acima na ordem apresentada
2. **Adicionar configurações** no Program.cs  
3. **Testar** com um controller simples

### **⏳ Para próximas iterações:**
1. **Criar DTOs** específicos para cada entidade
2. **Migrar CustomerController** (primeiro teste)
3. **Atualizar TypeScript** correspondente
4. **Migrar demais controllers** progressivamente

---

**🚀 Quer que eu comece criando essas classes base agora?** 
**Recomendo começar pela ApiResponse<T> e depois as exceções!** 🎯
