// ===================================
// ORDER ITEM INTERFACES - GesN (TypeScript)
// Interfaces específicas para gerenciamento de itens do pedido
// ===================================

// ===================================
// CONFIGURAÇÃO DO ORDER ITEM MANAGER
// ===================================

interface OrderItemManagerConfig {
    containerSelector: string;
    endpoints: {
        reloadItems: string;
        addSimple: string;
        addComposite: string;
        addGroup: string;
        update: string;
        remove: string;
    };
}

// ===================================
// EVENTOS DO ORDER ITEM MANAGER
// ===================================

interface OrderItemEvents {
    onItemAdded?: (item: OrderItemData) => void;
    onItemUpdated?: (item: OrderItemData) => void;
    onItemRemoved?: (itemId: string) => void;
    onItemsReloaded?: () => void;
    onOrderTotalsChanged?: (totals: OrderTotals) => void;
    onError?: (error: OrderItemError) => void;
}

// ===================================
// DADOS DE ORDER ITEM
// ===================================

interface OrderItemData {
    id: string;
    orderId: string;
    productId: string;
    productName: string;
    productType: ProductType;
    quantity: number;
    unitPrice: number;
    discountAmount: number;
    taxAmount: number;
    subtotal: number;
    total: number;
    notes?: string;
    
    // Dados específicos por tipo de produto
    compositeConfiguration?: CompositeItemConfiguration[];
    groupConfiguration?: GroupItemConfiguration[];
}

// ===================================
// TOTAIS DO PEDIDO
// ===================================

interface OrderTotals {
    subtotal: number;
    discountTotal: number;
    taxTotal: number;
    total: number;
    itemCount: number;
}

// ===================================
// ERROS ESPECÍFICOS DO ORDER ITEM
// ===================================

interface OrderItemError {
    action: string;
    message: string;
    details?: any;
    timestamp: number;
}

// ===================================
// REQUESTS PARA UPDATE E REMOVE
// ===================================

interface UpdateItemRequest {
    itemId: string;
    quantity?: number;
    discountAmount?: number;
    taxAmount?: number;
    notes?: string;
}

interface UpdateItemResponse {
    success: boolean;
    message: string;
    item?: OrderItemData;
    totals?: OrderTotals;
}

interface RemoveItemRequest {
    itemId: string;
    orderId: string;
}

interface RemoveItemResponse {
    success: boolean;
    message: string;
    removedItemId?: string;
    totals?: OrderTotals;
}

// ===================================
// COMUNICAÇÃO ENTRE MANAGERS
// ===================================

interface CatalogToOrderItemEvents {
    onSimpleProductSelected?: (productId: string, quantity: number, notes?: string) => void;
    onCompositeProductConfigured?: (productId: string, config: CompositeItemConfiguration[]) => void;
    onGroupProductConfigured?: (productId: string, config: GroupItemConfiguration[]) => void;
}

interface OrderItemToCatalogEvents {
    onItemAdded?: (item: OrderItemData) => void;
    onItemsReloaded?: () => void;
    onError?: (error: OrderItemError) => void;
}

// ===================================
// OPERAÇÕES CRUD ESPECÍFICAS
// ===================================

interface OrderItemCRUDOperations {
    // Create
    addSimpleItem(productId: string, quantity: number, options?: OrderItemOptions): Promise<AddToCartItemResponse>;
    addCompositeItem(productId: string, quantity: number, config: CompositeItemConfiguration[], options?: OrderItemOptions): Promise<AddToCartItemResponse>;
    addGroupItem(productId: string, quantity: number, config: GroupItemConfiguration[], options?: OrderItemOptions): Promise<AddToCartItemResponse>;
    
    // Read
    reloadItems(): Promise<void>;
    getItems(): OrderItemData[];
    
    // Update
    updateItem(itemId: string, data: UpdateItemRequest): Promise<UpdateItemResponse>;
    
    // Delete
    removeItem(itemId: string): Promise<RemoveItemResponse>;
}

// ===================================
// OPÇÕES PARA ITENS DO PEDIDO
// ===================================

interface OrderItemOptions {
    discountAmount?: number;
    taxAmount?: number;
    notes?: string;
}

// ===================================
// ESTADO INTERNO DO MANAGER
// ===================================

interface OrderItemManagerState {
    orderId: string;
    items: OrderItemData[];
    isLoading: boolean;
    lastReloadTimestamp: number;
}

// ===================================
// CONFIGURAÇÃO DE VALIDAÇÃO
// ===================================

interface OrderItemValidationConfig {
    minQuantity: number;
    maxQuantity: number;
    allowNegativeDiscount: boolean;
    allowNegativeTax: boolean;
    maxNotesLength: number;
}

// ===================================
// MÉTRICAS E ANALYTICS
// ===================================

interface OrderItemMetrics {
    totalItems: number;
    totalValue: number;
    averageItemValue: number;
    mostExpensiveItem?: OrderItemData;
    categoryBreakdown: Record<string, number>;
}


