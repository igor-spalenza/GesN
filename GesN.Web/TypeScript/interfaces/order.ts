// ===================================
// INTERFACES DE ORDER - GesN
// ===================================

// Import removido - definições globais

// ===================================
// ENTIDADES CORE
// ===================================

interface Order {
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

interface OrderItem {
    id: number;
    orderId: number;
    productId: number;
    productName: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    notes?: string;
}

// ⚠️ Interface Customer removida - já definida em customer.ts

// ===================================
// ENUMS E TIPOS
// ===================================

type OrderType = 'Sale' | 'Purchase' | 'Production' | 'Transfer';
type OrderStatus = 'Draft' | 'Confirmed' | 'InProgress' | 'Completed' | 'Cancelled';

// ===================================
// FORMULÁRIOS
// ===================================

interface OrderFormData {
    customerId: number;
    customerName: string;
    orderDate: string;
    deliveryDate?: string;
    orderType: OrderType;
    notes?: string;
}

interface OrderSaveResponse {
    id: string; // ✅ Corrigido: ID é string UUID, não number
    numberSequence: string;
    message?: string;
    success: boolean; // ✅ Adicionado: Resposta inclui success diretamente
}

// ===================================
// AUTOCOMPLETE
// ===================================

// ⚠️ Interface CustomerAutocompleteItem removida - já definida em customer.ts

// ===================================
// SISTEMA DE ABAS
// ===================================

interface OrderTab {
    id: string;
    orderId: number;
    numberSequence: string;
    isActive: boolean;
    isDirty: boolean;
    lastModified?: Date;
}

interface OrderTabsState {
    openTabs: Map<number, OrderTab>;
    activeTabId: string | null;
    tabCounter: number;
    maxTabs: number;
}

// ===================================
// DATATABLES
// ===================================

interface OrderDataTableConfig {
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

interface DataTableColumnDef {
    targets: number[];
    orderable?: boolean;
    searchable?: boolean;
}

// ===================================
// SISTEMA DE REDIMENSIONAMENTO
// ===================================

interface ResizeConfig {
    minWidths: Record<number, number>;
    storageKey: string;
    isDragging: boolean;
    currentColumn: number | null;
    startX: number;
    startWidth: number;
}

interface ColumnWidths {
    [columnIndex: number]: string;
}

// ===================================
// ESTADO DO MANAGER
// ===================================

interface OrderManagerState {
    contador: number;
    qtdAbasAbertas: number;
    resizeConfig: ResizeConfig;
    tabsState: OrderTabsState;
}

interface OrderManagerConfig {
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

interface OrderEventHandlers {
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

interface OrderFilters {
    search?: string;
    status?: OrderStatus;
    orderType?: OrderType;
    customerId?: number;
    dateFrom?: string;
    dateTo?: string;
}

interface OrderListResponse {
    orders: Order[];
    totalCount: number;
    filteredCount: number;
}

// ===================================
// FLOATING LABELS
// ===================================

interface FloatingLabelConfig {
    containerClass: string;
    inputClass: string;
    labelClass: string;
    activeClass: string;
    errorClass: string;
}

// ===================================
// MODAL CONFIG
// ===================================

interface OrderModalConfig {
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

interface AutocompleteConfig {
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

interface AjaxErrorResponse {
    responseJSON?: {
        message?: string;
        errors?: Record<string, string[]>;
    };
    status: number;
    statusText: string;
}

interface OrderLoadingState {
    isLoading: boolean;
    message?: string;
    target?: string;
}

// ===================================
// VALIDAÇÃO
// ===================================

interface OrderValidationErrors {
    customerId?: string[];
    customerName?: string[];
    orderDate?: string[];
    orderType?: string[];
    general?: string[];
}

interface ValidationResult {
    isValid: boolean;
    errors: OrderValidationErrors;
}
