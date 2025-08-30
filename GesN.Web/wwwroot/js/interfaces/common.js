"use strict";
// ===================================
// INTERFACES COMUNS - GesN
// ===================================
// Erro personalizado para validação
class ValidationError extends Error {
    constructor(message, errors) {
        super(message);
        this.errors = errors;
        this.name = 'ValidationError';
    }
}
// Erro de negócio
class BusinessError extends Error {
    constructor(message) {
        super(message);
        this.name = 'BusinessError';
    }
}
//# sourceMappingURL=common.js.map