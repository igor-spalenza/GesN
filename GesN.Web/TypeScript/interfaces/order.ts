// ===================================
// INTERFACES DE ORDER - GesN
// ===================================

import { ApiResponse, EntityStatus } from './common';

// ===================================
// ENTIDADES CORE
// ===================================

export interface Order {
    id: number;
    numberSequence: string;
    customerId: number;
    customerName: string;
    orderDate: string;
    deliveryDate?: string;
    orderType: OrderType;
    status: OrderStatus;
    totalValue: number;
    notes?: string;
    items: OrderItem[];
    createdAt: string;
    updatedAt?: string;
}

export interface OrderItem {
    id: number;
    orderId: number;
    productId: number;
    productName: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    notes?: string;
}

export interface Customer {
    id: number;
    name: string;
    value?: string;
    phone?: string;
    email?: string;
    documentNumber?: string;
}

// ===================================
// ENUMS E TIPOS
// ===================================

export type OrderType = 'Sale' | 'Purchase' | 'Production' | 'Transfer';
export type OrderStatus = 'Draft' | 'Confirmed' | 'InProgress' | 'Completed' | 'Cancelled';

// ===================================
// FORMULÁRIOS
// ===================================

export interface OrderFormData {
    customerId: number;
    customerName: string;
    orderDate: string;
    deliveryDate?: string;
    orderType: OrderType;
    notes?: string;
}

export interface OrderSaveResponse {
    id: number;
    numberSequence: string;
    message?: string;
}

// ===================================
// AUTOCOMPLETE
// ===================================

export interface CustomerAutocompleteItem {
    id: number;
    label: string;
    value: string;
    phone?: string;
    email?: string;
    data: Customer;
}

// ===================================
// SISTEMA DE ABAS
// ===================================

export interface OrderTab {
    id: string;
    orderId: number;
    numberSequence: string;
    isActive: boolean;
    isDirty: boolean;
    lastModified?: Date;
}

export interface OrderTabsState {
    openTabs: Map<number, OrderTab>;
    activeTabId: string | null;
    tabCounter: number;
    maxTabs: number;
}

// ===================================
// DATATABLES
// ===================================

export interface OrderDataTableConfig {
    language: {
        url: string;
    };
    responsive: boolean;
    pageLength: number;
    lengthMenu: (number | string)[][];
    order: [number, string][];
    columnDefs: DataTableColumnDef[];
    dom: string;
    drawCallback: () => void;
}

export interface DataTableColumnDef {
    targets: number[];
    orderable?: boolean;
    searchable?: boolean;
}

// ===================================
// SISTEMA DE REDIMENSIONAMENTO
// ===================================

export interface ResizeConfig {
    minWidths: Record<number, number>;
    storageKey: string;
    isDragging: boolean;
    currentColumn: number | null;
    startX: number;
    startWidth: number;
}

export interface ColumnWidths {
    [columnIndex: number]: string;
}

// ===================================
// ESTADO DO MANAGER
// ===================================

export interface OrderManagerState {
    contador: number;
    qtdAbasAbertas: number;
    resizeConfig: ResizeConfig;
    tabsState: OrderTabsState;
}

export interface OrderManagerConfig {
    baseUrl: string;
    modalSelector: string;
    gridSelector: string;
    containerSelector: string;
    tabsSelector: string;
    tabsContentSelector: string;
}

// ===================================
// EVENTOS E CALLBACKS
// ===================================

export interface OrderEventHandlers {
    onOrderCreated?: (order: OrderSaveResponse) => void;
    onOrderUpdated?: (orderId: number) => void;
    onOrderDeleted?: (orderId: number) => void;
    onTabOpened?: (tab: OrderTab) => void;
    onTabClosed?: (tabId: string) => void;
    onDataTableInitialized?: () => void;
}

// ===================================
// FILTROS E BUSCA
// ===================================

export interface OrderFilters {
    search?: string;
    status?: OrderStatus;
    orderType?: OrderType;
    customerId?: number;
    dateFrom?: string;
    dateTo?: string;
}

export interface OrderListResponse {
    orders: Order[];
    totalCount: number;
    filteredCount: number;
}

// ===================================
// FLOATING LABELS
// ===================================

export interface FloatingLabelConfig {
    containerClass: string;
    inputClass: string;
    labelClass: string;
    activeClass: string;
    errorClass: string;
}

// ===================================
// MODAL CONFIG
// ===================================

export interface OrderModalConfig {
    selector: string;
    sizes: {
        create: 'lg' | 'xl';
        edit: 'lg' | 'xl';
        details: 'xl';
    };
}

// ===================================
// AUTOCOMPLETE CONFIG
// ===================================

export interface AutocompleteConfig {
    minLength: number;
    endpoint: string;
    hint: boolean;
    debug: boolean;
    openOnFocus: boolean;
    autoselect: boolean;
}

// ===================================
// UTILITÁRIOS
// ===================================

export interface AjaxErrorResponse {
    responseJSON?: {
        message?: string;
        errors?: Record<string, string[]>;
    };
    status: number;
    statusText: string;
}

export interface LoadingState {
    isLoading: boolean;
    message?: string;
    target?: string;
}

// ===================================
// VALIDAÇÃO
// ===================================

export interface OrderValidationErrors {
    customerId?: string[];
    customerName?: string[];
    orderDate?: string[];
    orderType?: string[];
    general?: string[];
}

export interface ValidationResult {
    isValid: boolean;
    errors: OrderValidationErrors;
}
