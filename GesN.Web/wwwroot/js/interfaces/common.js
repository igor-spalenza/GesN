// ===================================
// INTERFACES COMUNS - GesN
// ===================================
// Erro personalizado para validação
export class ValidationError extends Error {
    constructor(message, errors) {
        super(message);
        this.errors = errors;
        this.name = 'ValidationError';
    }
}
// Erro de negócio
export class BusinessError extends Error {
    constructor(message) {
        super(message);
        this.name = 'BusinessError';
    }
}
//# sourceMappingURL=common.js.map