// ===================================
// INTERFACES DE CUSTOMER - GesN
// ===================================

// Import removido - definições globais

// ⚠️ Tipos removidos - já definidos em common.ts

// Dados do formulário de cliente
interface CustomerFormData {
    name: string;
    email?: string;
    phone?: string;
    documentType: 'CPF' | 'CNPJ';
    documentNumber: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    notes?: string;
    isActive: boolean;
}

// Cliente completo (com ID)
interface Customer {
    id: number;
    name: string;
    email?: string;
    phone?: string;
    documentType: 'CPF' | 'CNPJ';
    documentNumber: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    notes?: string;
    isActive: boolean;
    createdAt: string;
    updatedAt?: string;
    status: 'Active' | 'Inactive';
    value?: string; // Para compatibilidade com autocomplete
}

// Resposta ao salvar cliente
interface CustomerSaveResponse {
    id: number;
    name: string;
    documentNumber: string;
}

// Item do autocomplete de cliente
interface CustomerAutocompleteItem {
    id: number;
    label: string;
    value: string;
    phone?: string;
    email?: string;
    documentType?: 'CPF' | 'CNPJ';
    documentNumber?: string;
    data?: Customer; // Dados completos do cliente
}

// Configuração do cliente manager
interface CustomerManagerConfig {
    baseUrl: string;
    modalSelector: string;
    gridSelector: string;
    maxNameLength: number;
    documentMasks: Record<'CPF' | 'CNPJ', string>;
}

// Estado interno do cliente manager
interface CustomerManagerState {
    currentEditingId: number | null;
    lastLoadedData: Customer | null;
    validationErrors: Record<string, string[]>;
    isFormDirty: boolean;
}

// Filtros específicos do cliente
interface CustomerFilters {
    search?: string;
    status?: 'Active' | 'Inactive';
    documentType?: 'CPF' | 'CNPJ';
    city?: string;
    state?: string;
}

// Estatísticas do cliente
interface CustomerStatistics {
    totalCustomers: number;
    activeCustomers: number;
    inactiveCustomers: number;
    cpfCustomers: number;
    cnpjCustomers: number;
    recentCustomers: number;
}

// Opções para exportação
interface CustomerExportOptions {
    format: 'excel' | 'csv' | 'pdf';
    filters?: CustomerFilters;
    includeInactive?: boolean;
    fields?: (keyof Customer)[];
}

// Resultado da busca de clientes
interface CustomerSearchResult {
    customers: Customer[];
    totalCount: number;
    currentPage: number;
    totalPages: number;
}

