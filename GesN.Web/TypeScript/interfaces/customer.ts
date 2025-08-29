// ===================================
// INTERFACES DE CUSTOMER - GesN
// ===================================

import { ApiResponse, DocumentType, EntityStatus } from './common';

// Dados do formulário de cliente
export interface CustomerFormData {
    name: string;
    email?: string;
    phone?: string;
    documentType: DocumentType;
    documentNumber: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    notes?: string;
    isActive: boolean;
}

// Cliente completo (com ID)
export interface Customer extends CustomerFormData {
    id: number;
    createdAt: string;
    updatedAt?: string;
    status: EntityStatus;
}

// Resposta ao salvar cliente
export interface CustomerSaveResponse {
    id: number;
    name: string;
    documentNumber: string;
}

// Item do autocomplete de cliente
export interface CustomerAutocompleteItem {
    id: number;
    label: string;
    value: string;
    phone?: string;
    email?: string;
    documentType: DocumentType;
    documentNumber: string;
}

// Configuração do cliente manager
export interface CustomerManagerConfig {
    baseUrl: string;
    modalSelector: string;
    gridSelector: string;
    maxNameLength: number;
    documentMasks: Record<DocumentType, string>;
}

// Estado interno do cliente manager
export interface CustomerManagerState {
    currentEditingId: number | null;
    lastLoadedData: Customer | null;
    validationErrors: Record<string, string[]>;
    isFormDirty: boolean;
}

// Filtros específicos do cliente
export interface CustomerFilters {
    search?: string;
    status?: EntityStatus;
    documentType?: DocumentType;
    city?: string;
    state?: string;
}

// Estatísticas do cliente
export interface CustomerStatistics {
    totalCustomers: number;
    activeCustomers: number;
    inactiveCustomers: number;
    cpfCustomers: number;
    cnpjCustomers: number;
    recentCustomers: number;
}

// Opções para exportação
export interface CustomerExportOptions {
    format: 'excel' | 'csv' | 'pdf';
    filters?: CustomerFilters;
    includeInactive?: boolean;
    fields?: (keyof Customer)[];
}

// Resultado da busca de clientes
export interface CustomerSearchResult {
    customers: Customer[];
    totalCount: number;
    currentPage: number;
    totalPages: number;
}
