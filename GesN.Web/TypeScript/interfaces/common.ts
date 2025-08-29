// ===================================
// INTERFACES COMUNS - GesN
// ===================================

// Resposta padrão da API
export interface ApiResponse<T = unknown> {
    success: boolean;
    message?: string;
    data?: T;
    errors?: Record<string, string[]>;
}

// Configuração base para managers
export interface ManagerConfig {
    baseUrl: string;
    modalSelector?: string;
    gridSelector?: string;
}

// Opções para requisições AJAX
export interface AjaxOptions {
    type?: 'GET' | 'POST' | 'PUT' | 'DELETE';
    data?: any;
    dataType?: string;
    processData?: boolean;
    contentType?: string | boolean;
    headers?: Record<string, string>;
}

// Estados de loading
export interface LoadingState {
    isLoading: boolean;
    loadingMessage?: string;
}

// Configuração de modal
export interface ModalConfig {
    selector: string;
    title: string;
    size?: 'sm' | 'md' | 'lg' | 'xl';
    backdrop?: boolean | 'static';
}

// Configuração de DataTable
export interface DataTableConfig {
    pageLength?: number;
    lengthMenu?: (number | string)[][];
    order?: [number, string][];
    columnDefs?: any[];
    responsive?: boolean;
    language?: {
        url?: string;
    };
}

// Erro personalizado para validação
export class ValidationError extends Error {
    constructor(
        message: string, 
        public errors?: Record<string, string[]>
    ) {
        super(message);
        this.name = 'ValidationError';
    }
}

// Erro de negócio
export class BusinessError extends Error {
    constructor(message: string) {
        super(message);
        this.name = 'BusinessError';
    }
}

// Tipos utilitários
export type DocumentType = 'CPF' | 'CNPJ';
export type EntityStatus = 'Active' | 'Inactive';
export type ProductType = 'Simple' | 'Composite' | 'Group';

// Interface para paginação
export interface PaginationInfo {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalItems: number;
}

// Interface para filtros
export interface FilterOptions {
    search?: string;
    status?: EntityStatus;
    page?: number;
    pageSize?: number;
}
