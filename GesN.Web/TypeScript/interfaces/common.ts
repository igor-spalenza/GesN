// ===================================
// INTERFACES COMUNS - GesN
// ===================================

// Resposta padrão da API
interface ApiResponse<T = unknown> {
    success: boolean;
    message?: string;
    data?: T;
    errors?: Record<string, string[]>;
}

// Configuração base para managers
interface ManagerConfig {
    baseUrl: string;
    modalSelector?: string;
    gridSelector?: string;
}

// Opções para requisições AJAX
interface AjaxOptions {
    type?: 'GET' | 'POST' | 'PUT' | 'DELETE';
    data?: any;
    dataType?: string;
    processData?: boolean;
    contentType?: string | boolean;
    headers?: Record<string, string>;
}

// Estados de loading
interface LoadingState {
    isLoading: boolean;
    loadingMessage?: string;
}

// Configuração de modal
interface ModalConfig {
    selector: string;
    title: string;
    size?: 'sm' | 'md' | 'lg' | 'xl';
    backdrop?: boolean | 'static';
}

// Configuração de DataTable
interface DataTableConfig {
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
class ValidationError extends Error {
    constructor(
        message: string, 
        public errors?: Record<string, string[]>
    ) {
        super(message);
        this.name = 'ValidationError';
    }
}

// Erro de negócio
class BusinessError extends Error {
    constructor(message: string) {
        super(message);
        this.name = 'BusinessError';
    }
}

// Tipos utilitários
type GesNDocumentType = 'CPF' | 'CNPJ';
type GesNEntityStatus = 'Active' | 'Inactive';
type ProductType = 'Simple' | 'Composite' | 'Group';

// Interface para paginação
interface PaginationInfo {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalItems: number;
}

// Interface para filtros
interface FilterOptions {
    search?: string;
    status?: GesNEntityStatus;
    page?: number;
    pageSize?: number;
}
