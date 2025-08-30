// ===================================
// INTERFACES PRODUCT CATALOG - GesN
// ===================================

// ===================================
// CONFIGURAÇÕES E ESTADO
// ===================================

interface ProductCatalogConfig {
    baseUrl: string;
    catalogContainerSelector: string;
    productListSelector: string;
    orderItemsContainerSelector: string;
    pageSize: number;
    maxSearchLength: number;
    debounceMs: number;
}

interface ProductCatalogState {
    orderId: string | null;
    currentPage: number;
    currentCategory: string;
    currentSearchTerm: string;
    totalProducts: number;
    isLoading: boolean;
    lastLoadedData: ProductCatalogData | null;
}

interface ProductCatalogFilters {
    category?: string;
    search?: string;
    page: number;
    pageSize: number;
}

// ===================================
// DADOS E MODELOS
// ===================================

interface ProductCatalogData {
    products: ProductCatalogItem[];
    currentPage: number;
    totalProducts: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
    filters: ProductCatalogFilters;
}

interface ProductCatalogItem {
    id: string;
    name: string;
    description?: string;
    sku?: string;
    price: number;
    unitPrice: number;
    cost?: number;
    categoryId?: string;
    categoryName?: string;
    productType: ProductType;
    imageUrl?: string;
    assemblyTime?: number;
    isActive: boolean;
    createdAt: string;
}

interface ProductCategory {
    id: string;
    name: string;
    description?: string;
    isActive: boolean;
    productCount?: number;
}

// ===================================
// REQUISIÇÕES E RESPOSTAS
// ===================================

interface AddToCartRequest {
    orderId: string;
    productId: string;
    quantity: number;
    notes?: string;
}

interface AddToCartResponse {
    success: boolean;
    message?: string;
    totalItems: number;
    orderItem?: OrderItemData;
}

interface OrderItemData {
    id: string;
    productId: string;
    productName: string;
    quantity: number;
    unitPrice: number;
    discountAmount: number;
    taxAmount: number;
    totalPrice: number;
}

interface CatalogLoadRequest {
    category?: string;
    search?: string;
    page: number;
    pageSize: number;
}

interface CatalogLoadResponse {
    success: boolean;
    message?: string;
    data: ProductCatalogData;
}

// ===================================
// EVENTOS E CALLBACKS
// ===================================

interface ProductCatalogEvents {
    onProductLoaded?: (products: ProductCatalogItem[]) => void;
    onProductAdded?: (product: ProductCatalogItem) => void;
    onCategoryChanged?: (category: string) => void;
    onSearchPerformed?: (searchTerm: string) => void;
    onPageChanged?: (page: number) => void;
    onLoadingStateChanged?: (isLoading: boolean) => void;
    onError?: (error: ProductCatalogError) => void;
}

interface ProductCatalogError {
    code: string;
    message: string;
    details?: any;
    timestamp: Date;
}

// ===================================
// COMPONENTES UI
// ===================================

interface ProductCatalogUI {
    showProduct(product: ProductCatalogItem): string;
    showProductList(products: ProductCatalogItem[]): string;
    showPagination(data: ProductCatalogData): string;
    showLoading(): string;
    showEmpty(): string;
    showError(error: ProductCatalogError): string;
}

interface ProductCatalogPagination {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalItems: number;
    hasNext: boolean;
    hasPrevious: boolean;
    startItem: number;
    endItem: number;
    paginationInfo: string;
}

// ===================================
// UTILITÁRIOS E HELPERS
// ===================================

interface ProductCatalogSearch {
    term: string;
    timestamp: Date;
    resultsCount: number;
    filters: ProductCatalogFilters;
}

interface ProductCatalogCache {
    key: string;
    data: ProductCatalogData;
    timestamp: Date;
    expiration: Date;
}

// ===================================
// TIPOS UNION E ENUMS
// ===================================

type ProductCatalogView = 'grid' | 'list';
type ProductSortBy = 'name' | 'price' | 'category' | 'created' | 'popularity';
type ProductSortOrder = 'asc' | 'desc';

type CatalogLoadingState = 'idle' | 'loading' | 'success' | 'error';
type CatalogMode = 'browse' | 'search' | 'category' | 'filtered';

// ===================================
// EXTENSÕES DE INTERFACES EXISTENTES  
// ===================================

// ProductType já definido em common.ts como type union

// ===================================
// GERENCIAMENTO DE ESTADO POR ABA
// ===================================

/**
 * Estado do catálogo salvo para uma aba específica
 */
export interface SavedCatalogState {
    orderId: string;
    category: string;
    searchTerm: string;
    currentPage: number;
    pageSize: number;
    timestamp: number;
    productCache?: ProductCatalogItem[];
    totalProducts?: number;
    totalPages?: number;
    scrollPosition?: number;
}

/**
 * Gerenciador de estados múltiplos do catálogo
 */
export interface CatalogStateManager {
    savedStates: Map<string, SavedCatalogState>;
    currentOrderId: string | null;
    persistToStorage: boolean;
    maxStates: number;
}

/**
 * Configuração do sistema de contextos
 */
export interface ContextConfig {
    enablePersistence: boolean;
    maxStatesInMemory: number;
    storageKey: string;
    autoSaveInterval: number;
    debugMode: boolean;
}

/**
 * Evento de mudança de contexto
 */
export interface ContextChangeEvent {
    previousOrderId: string | null;
    newOrderId: string | null;
    stateRestored: boolean;
    timestamp: number;
}

/**
 * Controle de visibilidade do catálogo slide
 */
export interface CatalogSlideControl {
    isVisible: boolean;
    isAnimating: boolean;
    currentOrderId: string | null;
    slideDirection: 'in' | 'out';
}
