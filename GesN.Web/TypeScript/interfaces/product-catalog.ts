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
    onProductLoaded?: (product: ProductCatalogItem) => void;
    onProductsLoaded?: (data: ProductCatalogData) => void;
    onCategoryChanged?: (category: string) => void;
    onSearchPerformed?: (searchTerm: string) => void;
    onPageChanged?: (page: number) => void;
    onAddToCart?: (productId: string, quantity: number) => void;
    onError?: (error: ProductCatalogError) => void;
    
    // NOVOS EVENTOS PARA COMUNICAÇÃO COM ORDERITEMMANAGER
    onSimpleProductSelected?: (productId: string, quantity: number, notes?: string) => void;
    onCompositeProductConfigured?: (productId: string, config: CompositeItemConfiguration[]) => void;
    onGroupProductConfigured?: (productId: string, config: GroupItemConfiguration[]) => void;
}

interface ProductCatalogError {
    action: string;
    code?: string;
    message: string;
    details?: any;
    timestamp: number;
}

// ===================================
// COMPONENTES UI
// ===================================

interface ProductCatalogUI {
    showProduct: (product: ProductCatalogItem) => void;
    showProductList: (products: ProductCatalogItem[]) => void;
    showPagination: (pagination: ProductCatalogPagination) => void;
    showLoading: (show: boolean) => void;
    showError: (error: ProductCatalogError) => void;
    showEmpty: (message: string) => void;
    clearContent: () => void;
}

interface ProductCatalogPagination {
    currentPage: number;
    totalPages: number;
    totalProducts: number;
    pageSize: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
    startItem?: number;
    endItem?: number;
}

// ===================================
// UTILITÁRIOS E HELPERS
// ===================================

interface ProductCatalogSearch {
    term: string;
    timestamp: number;
    resultsCount: number;
    filters: ProductCatalogFilters;
}

interface ProductCatalogCache {
    key: string;
    data: ProductCatalogData;
    timestamp: number;
    expiry: number;
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
interface SavedCatalogState {
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
interface CatalogStateManager {
    savedStates: Map<string, SavedCatalogState>;
    currentOrderId: string | null;
    persistToStorage: boolean;
    maxStates: number;
}

/**
 * Configuração do sistema de contextos
 */
interface ContextConfig {
    enablePersistence: boolean;
    maxStatesInMemory: number;
    storageKey: string;
    autoSaveInterval: number;
    debugMode: boolean;
}

/**
 * Evento de mudança de contexto
 */
interface ContextChangeEvent {
    previousOrderId: string | null;
    newOrderId: string | null;
    stateRestored: boolean;
    timestamp: number;
}

/**
 * Controle de visibilidade do catálogo slide
 */
interface CatalogSlideControl {
    isVisible: boolean;
    isAnimating: boolean;
    currentOrderId: string | null;
    slideDirection: 'in' | 'out';
}

// ===================================
// ADD TO CART - NOVA ARQUITETURA OrderItemController
// ===================================

/**
 * Request para adicionar produto simples ao carrinho
 */
interface AddSimpleItemRequest {
    orderId: string;
    productId: string;
    quantity: number;
    discountAmount?: number;
    taxAmount?: number;
    notes?: string;
}

/**
 * Configuração de componente para produto composto
 */
interface CompositeItemConfiguration {
    componentId: string;
    hierarchyId: string;
    quantity: number;
    componentName?: string;
    hierarchyName?: string;
    additionalCost?: number;
}

/**
 * Request para adicionar produto composto ao carrinho (FASE 2)
 */
interface AddCompositeItemRequest {
    orderId: string;
    productId: string;
    quantity: number;
    componentConfigurations: CompositeItemConfiguration[];
    discountAmount?: number;
    taxAmount?: number;
    notes?: string;
}

/**
 * Configuração de item do grupo
 */
interface GroupItemConfiguration {
    groupItemId: string;
    selectedProductId: string;
    quantity: number;
    groupItemName?: string;
    productName?: string;
    extraPrice?: number;
    itemType?: string; // "Produto" ou "Categoria"
}

/**
 * Request para adicionar grupo de produtos ao carrinho (FASE 3)
 */
interface AddGroupItemRequest {
    orderId: string;
    productId: string;
    quantity: number;
    groupConfigurations: GroupItemConfiguration[];
    discountAmount?: number;
    taxAmount?: number;
    notes?: string;
}

/**
 * Response dos endpoints de adicionar ao carrinho
 */
interface AddToCartItemResponse {
    success: boolean;
    message: string;
    item?: {
        id: string;
        productId: string;
        productName: string;
        quantity: number;
        unitPrice: number;
        discountAmount: number;
        taxAmount: number;
        subtotal: number;
        total: number;
    };
}

/**
 * Request para atualizar item do pedido
 */
interface UpdateOrderItemRequest {
    itemId: string;
    quantity: number;
    unitPrice: number;
    discountAmount?: number;
    taxAmount?: number;
    notes?: string;
}

/**
 * Request para remover item do pedido
 */
interface RemoveOrderItemRequest {
    itemId: string;
}

// ===================================
// PRODUCT VALIDATION - ProductController
// ===================================

/**
 * Request para validar produto simples
 */
interface ValidateSimpleProductRequest {
    productId: string;
}

/**
 * Request para validar produto composto (FASE 2)
 */
interface ValidateCompositeProductRequest {
    productId: string;
}

/**
 * Request para validar grupo de produtos (FASE 3)
 */
interface ValidateGroupProductRequest {
    productId: string;
}

/**
 * Response da validação de produto
 */
interface ValidateProductResponse {
    success: boolean;
    message: string;
    product?: {
        id: string;
        name: string;
        description?: string;
        sku?: string;
        price: number;
        unitPrice: number;
        cost: number;
        categoryId?: string;
        categoryName?: string;
        productType: string;
        assemblyTime: number;
        assemblyInstructions?: string;
        imageUrl?: string;
    };
}

/**
 * Componente de uma hierarquia
 */
interface ProductComponent {
    id: string;
    name: string;
    description?: string;
    additionalCost: number;
    hierarchyId: string;
}

/**
 * Hierarquia de componentes de um produto composto
 */
interface ProductHierarchy {
    id: string;
    name: string;
    description?: string;
    minQuantity: number;
    maxQuantity: number;
    isOptional: boolean;
    assemblyOrder: number;
    components: ProductComponent[];
}

/**
 * Response da validação de produto composto
 */
interface ValidateCompositeProductResponse {
    success: boolean;
    message: string;
    product?: {
        id: string;
        name: string;
        description?: string;
        sku?: string;
        price: number;
        unitPrice: number;
        cost: number;
        categoryId?: string;
        categoryName?: string;
        productType: string;
        assemblyTime: number;
        assemblyInstructions?: string;
        imageUrl?: string;
        hierarchies: ProductHierarchy[];
    };
}

/**
 * Seleção de componente para cálculo de preço
 */
interface ComponentSelection {
    componentId: string;
    quantity: number;
    hierarchyId?: string;
}

/**
 * Request para calcular preço de produto composto
 */
interface CalculateCompositePriceRequest {
    productId: string;
    productQuantity: number;
    componentSelections: ComponentSelection[];
}

/**
 * Response do cálculo de preço
 */
interface CalculateCompositePriceResponse {
    success: boolean;
    message: string;
    pricing?: {
        basePrice: number;
        additionalCost: number;
        unitPrice: number;
        quantity: number;
        totalPrice: number;
    };
}

// ===================================
// GROUP PRODUCTS - FASE 3
// ===================================

/**
 * Opção de produto para item de grupo
 */
interface ProductOption {
    id: string;
    name: string;
    description?: string;
    sku?: string;
    price: number;
    effectivePrice: number;
    isAvailable: boolean;
    productType: string;
    categoryId?: string;
    categoryName?: string;
}

/**
 * Item de grupo de produtos
 */
interface GroupItem {
    id: string;
    productGroupId: string;
    productId?: string;
    productCategoryId?: string;
    quantity: number;
    minQuantity: number;
    maxQuantity?: number;
    defaultQuantity: number;
    isOptional: boolean;
    extraPrice: number;
    itemType: string; // "Produto" ou "Categoria"
    displayName: string;
    priceInfo: string;
    availabilityStatus: string;
    productOptions: ProductOption[];
}

/**
 * Regra de troca entre itens de grupo
 */
interface ExchangeRule {
    id: string;
    sourceGroupItemId: string;
    targetGroupItemId: string;
    sourceWeight: number;
    targetWeight: number;
    exchangeRatio: number;
    description: string;
    ratioDescription: string;
}

/**
 * Response da validação de grupo de produtos
 */
interface ValidateGroupProductResponse {
    success: boolean;
    message: string;
    product?: {
        id: string;
        name: string;
        description?: string;
        sku?: string;
        price: number;
        unitPrice: number;
        cost: number;
        categoryId?: string;
        categoryName?: string;
        productType: string;
        assemblyTime: number;
        assemblyInstructions?: string;
        imageUrl?: string;
        groupItems: GroupItem[];
        exchangeRules: ExchangeRule[];
        totalItems: number;
        requiredItems: number;
        optionalItems: number;
    };
}

/**
 * Seleção de item do grupo para cálculo de preço
 */
interface GroupItemSelection {
    groupItemId: string;
    selectedProductId: string;
    quantity: number;
}

/**
 * Request para calcular preço de grupo de produtos
 */
interface CalculateGroupPriceRequest {
    productId: string;
    groupQuantity: number;
    groupSelections: GroupItemSelection[];
}

/**
 * Response do cálculo de preço de grupo
 */
interface CalculateGroupPriceResponse {
    success: boolean;
    message: string;
    pricing?: {
        basePrice: number;
        itemsTotal: number;
        groupQuantity: number;
        finalPrice: number;
        selections: Array<{
            groupItemId: string;
            selectedProductId: string;
            productName: string;
            quantity: number;
            unitPrice: number;
            extraPrice: number;
            effectivePrice: number;
            totalPrice: number;
        }>;
    };
}

// ===================================
// CONFIGURAÇÃO PARA DATATABLES VERSION
// ===================================

interface ProductCatalogDataTablesConfig {
    baseUrl: string;
    catalogContainerSelector: string;
    tableSelector: string;
    categoryFiltersSelector: string;
    summarySelector: string;
}
