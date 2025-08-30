"use strict";
// ===================================
// PRODUCT CATALOG MANAGER - GesN (TypeScript)
// Gerenciamento completo do catálogo de produtos na edição de pedidos
// ===================================
class ProductCatalogManager {
    constructor() {
        this.cache = new Map();
        this.debounceTimer = null;
        // Configuração seguindo padrão dos outros managers
        this.config = {
            baseUrl: '/Product',
            catalogContainerSelector: '#productCatalogContainer',
            productListSelector: '#products-list',
            orderItemsContainerSelector: '#orderItemsContainer',
            pageSize: 8,
            maxSearchLength: 100,
            debounceMs: 300
        };
        // Estado inicial
        this.state = {
            orderId: null,
            currentPage: 1,
            currentCategory: '',
            currentSearchTerm: '',
            totalProducts: 0,
            isLoading: false,
            lastLoadedData: null
        };
        // Eventos (opcionais)
        this.events = {};
        // Configuração do sistema de contextos
        this.contextConfig = {
            enablePersistence: true,
            maxStatesInMemory: 10,
            storageKey: 'gesn_catalog_states',
            autoSaveInterval: 2000,
            debugMode: false
        };
        // Gerenciador de estados por aba
        this.stateManager = {
            savedStates: new Map(),
            currentOrderId: null,
            persistToStorage: this.contextConfig.enablePersistence,
            maxStates: this.contextConfig.maxStatesInMemory
        };
        // Controle de slide
        this.slideControl = {
            isVisible: false,
            isAnimating: false,
            currentOrderId: null,
            slideDirection: 'out'
        };
        // Carregar estados persistidos
        this.loadPersistedStates();
        // Interface de UI
        this.ui = {
            showProduct: this.renderProduct.bind(this),
            showProductList: this.renderProductList.bind(this),
            showPagination: this.renderPagination.bind(this),
            showLoading: this.renderLoading.bind(this),
            showEmpty: this.renderEmpty.bind(this),
            showError: this.renderError.bind(this)
        };
    }
    // ===================================
    // INICIALIZAÇÃO PRINCIPAL
    // ===================================
    /**
     * Inicializa o catálogo de produtos
     * @param orderId ID do pedido em edição
     */
    init(orderId) {
        try {
            this.state.orderId = orderId;
            this.bindEvents();
            this.loadInitialData();
            console.log(`ProductCatalogManager iniciado para pedido: ${orderId}`);
        }
        catch (error) {
            console.error('Erro ao inicializar ProductCatalogManager:', error);
            this.handleError({
                code: 'INIT_ERROR',
                message: 'Erro ao inicializar catálogo de produtos',
                details: error,
                timestamp: new Date()
            });
        }
    }
    /**
     * Associa eventos aos elementos da interface
     */
    bindEvents() {
        // Unbind eventos anteriores para evitar duplicação
        this.unbindEvents();
        // Click nas tabs de categoria (novos seletores)
        $(document).on('click.productCatalog', '.category-chip[data-category]', (e) => {
            e.preventDefault();
            const category = $(e.currentTarget).data('category') || '';
            this.filterByCategory(category);
        });
        // Busca de produtos
        $(document).on('click.productCatalog', '#search-btn', () => {
            this.performSearch();
        });
        // Enter no campo de busca
        $(document).on('keypress.productCatalog', '#product-search', (e) => {
            if (e.which === 13) { // Enter key
                this.performSearch();
            }
        });
        // Input de busca com debounce
        $(document).on('input.productCatalog', '#product-search', () => {
            this.debouncedSearch();
        });
        // Botões de adicionar ao carrinho
        $(document).on('click.productCatalog', '.btn-add-to-cart', (e) => {
            e.preventDefault();
            const productId = $(e.currentTarget).data('product-id');
            const quantity = parseInt($(e.currentTarget).data('quantity')) || 1;
            if (productId) {
                this.addToCart(productId, quantity);
            }
        });
        // Paginação
        $(document).on('click.productCatalog', '.pagination-btn[data-page]', (e) => {
            e.preventDefault();
            const page = parseInt($(e.currentTarget).data('page'));
            if (!isNaN(page)) {
                this.goToPage(page);
            }
        });
        // Seletor de itens por página
        $(document).on('change.productCatalog', '#page-size', (e) => {
            const newPageSize = parseInt($(e.currentTarget).val());
            if (!isNaN(newPageSize)) {
                this.changePageSize(newPageSize);
            }
        });
        console.log('ProductCatalogManager: Eventos associados');
    }
    /**
     * Remove associações de eventos
     */
    unbindEvents() {
        $(document).off('.productCatalog');
    }
    // ===================================
    // CARREGAMENTO DE DADOS
    // ===================================
    /**
     * Carrega dados iniciais do catálogo
     */
    async loadInitialData() {
        try {
            await this.loadProducts({
                page: 1,
                pageSize: this.config.pageSize
            });
        }
        catch (error) {
            console.error('Erro ao carregar dados iniciais:', error);
            this.handleError({
                code: 'INITIAL_LOAD_ERROR',
                message: 'Erro ao carregar catálogo inicial',
                details: error,
                timestamp: new Date()
            });
        }
    }
    /**
     * Carrega produtos com filtros especificados
     */
    async loadProducts(filters) {
        try {
            // Verificar se já está carregando
            if (this.state.isLoading) {
                console.log('ProductCatalogManager: Carregamento já em andamento');
                return;
            }
            // Atualizar estado
            this.setLoadingState(true);
            // Verificar cache
            const cacheKey = this.generateCacheKey(filters);
            const cached = this.getCachedData(cacheKey);
            if (cached) {
                console.log('ProductCatalogManager: Usando dados do cache');
                this.handleLoadSuccess(cached.data);
                return;
            }
            // Mostrar loading UI
            this.showLoadingUI();
            // Fazer requisição
            const response = await this.makeProductRequest(filters);
            if (response.success && response.data) {
                // Cachear resultado
                this.setCachedData(cacheKey, response.data);
                // Processar dados
                this.handleLoadSuccess(response.data);
                // Notificar eventos
                this.events.onProductLoaded?.(response.data.products);
            }
            else {
                throw new Error(response.message || 'Erro desconhecido ao carregar produtos');
            }
        }
        catch (error) {
            console.error('Erro ao carregar produtos:', error);
            this.handleLoadError(error);
        }
        finally {
            this.setLoadingState(false);
        }
    }
    /**
     * Faz requisição AJAX para carregar produtos
     */
    async makeProductRequest(filters) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `${this.config.baseUrl}/CatalogProducts`,
                type: 'GET',
                data: {
                    category: filters.category || '',
                    search: filters.search || '',
                    page: filters.page || 1,
                    pageSize: filters.pageSize || this.config.pageSize,
                    returnJson: true // Solicitar resposta JSON
                },
                dataType: 'json',
                timeout: 30000,
                success: (response) => {
                    try {
                        // Converter resposta do backend para formato esperado
                        const catalogResponse = {
                            success: response.success || false,
                            message: response.message || 'Produtos carregados',
                            data: {
                                products: (response.data?.products || []).map((p) => ({
                                    id: p.id,
                                    name: p.name,
                                    description: p.description,
                                    sku: p.sku,
                                    price: p.price,
                                    unitPrice: p.unitPrice,
                                    cost: p.cost,
                                    categoryId: p.categoryId,
                                    categoryName: p.categoryName,
                                    productType: this.parseProductType(p.productType),
                                    imageUrl: p.imageUrl,
                                    assemblyTime: p.assemblyTime,
                                    isActive: p.isActive,
                                    createdAt: p.createdAt
                                })),
                                currentPage: response.data?.currentPage || 1,
                                totalProducts: response.data?.totalProducts || 0,
                                totalPages: response.data?.totalPages || 0,
                                hasNextPage: response.data?.hasNextPage || false,
                                hasPreviousPage: response.data?.hasPreviousPage || false,
                                filters: response.data?.filters || filters
                            }
                        };
                        resolve(catalogResponse);
                    }
                    catch (parseError) {
                        console.error('Erro ao processar resposta:', parseError);
                        reject(parseError);
                    }
                },
                error: (xhr, status, error) => {
                    console.error('Erro na requisição:', { status, error, response: xhr });
                    const errorResponse = {
                        success: false,
                        message: xhr.responseJSON?.message || `Erro ao carregar produtos: ${error}`,
                        data: {
                            products: [],
                            currentPage: 1,
                            totalProducts: 0,
                            totalPages: 0,
                            hasNextPage: false,
                            hasPreviousPage: false,
                            filters: filters
                        }
                    };
                    resolve(errorResponse);
                }
            });
        });
    }
    // ===================================
    // FUNCIONALIDADES PRINCIPAIS
    // ===================================
    /**
     * Filtra produtos por categoria
     */
    async filterByCategory(category) {
        try {
            this.state.currentCategory = category;
            this.state.currentPage = 1; // Reset página
            // Atualizar UI das tabs
            this.updateCategoryTabs(category);
            // Carregar produtos filtrados
            await this.loadProducts({
                category: category,
                search: this.state.currentSearchTerm,
                page: 1,
                pageSize: this.config.pageSize
            });
            // Notificar evento
            this.events.onCategoryChanged?.(category);
            // Auto-save estado após filtrar
            if (this.stateManager.currentOrderId) {
                this.saveStateForOrder(this.stateManager.currentOrderId);
            }
        }
        catch (error) {
            console.error('Erro ao filtrar por categoria:', error);
            this.handleError({
                code: 'CATEGORY_FILTER_ERROR',
                message: 'Erro ao filtrar produtos por categoria',
                details: error,
                timestamp: new Date()
            });
        }
    }
    /**
     * Realiza busca de produtos
     */
    async performSearch() {
        try {
            const searchTerm = ($('#product-search').val() || '').trim();
            // Validar busca
            if (searchTerm.length > this.config.maxSearchLength) {
                this.showToast('warning', `Busca limitada a ${this.config.maxSearchLength} caracteres`);
                return;
            }
            this.state.currentSearchTerm = searchTerm;
            this.state.currentPage = 1; // Reset página
            // Carregar produtos com busca
            await this.loadProducts({
                category: this.state.currentCategory,
                search: searchTerm,
                page: 1,
                pageSize: this.config.pageSize
            });
            // Notificar evento
            this.events.onSearchPerformed?.(searchTerm);
            // Auto-save estado após buscar
            if (this.stateManager.currentOrderId) {
                this.saveStateForOrder(this.stateManager.currentOrderId);
            }
        }
        catch (error) {
            console.error('Erro ao realizar busca:', error);
            this.handleError({
                code: 'SEARCH_ERROR',
                message: 'Erro ao buscar produtos',
                details: error,
                timestamp: new Date()
            });
        }
    }
    /**
     * Busca com debounce
     */
    debouncedSearch() {
        if (this.debounceTimer !== null) {
            clearTimeout(this.debounceTimer);
        }
        this.debounceTimer = window.setTimeout(() => {
            this.performSearch();
        }, this.config.debounceMs);
    }
    /**
     * Adiciona produto ao carrinho
     */
    async addToCart(productId, quantity = 1) {
        try {
            if (!this.state.orderId) {
                this.showToast('error', 'ID do pedido não encontrado');
                return;
            }
            if (!productId) {
                this.showToast('error', 'ID do produto não encontrado');
                return;
            }
            // Preparar dados da requisição
            const request = {
                orderId: this.state.orderId,
                productId: productId,
                quantity: Math.max(1, quantity)
            };
            // Mostrar loading no botão
            const button = $(`.btn-add-to-cart[data-product-id="${productId}"]`);
            const originalHtml = button.html();
            button.html('<i class="fas fa-spinner fa-spin"></i> Adicionando...').prop('disabled', true);
            // Fazer requisição
            const response = await this.makeAddToCartRequest(request);
            if (response.success) {
                this.showToast('success', response.message || 'Produto adicionado ao carrinho!');
                // Recarregar lista de OrderItems
                await this.reloadOrderItems();
                // Notificar evento
                const product = this.findProductById(productId);
                if (product) {
                    this.events.onProductAdded?.(product);
                }
            }
            else {
                this.showToast('error', response.message || 'Erro ao adicionar produto');
            }
        }
        catch (error) {
            console.error('Erro ao adicionar ao carrinho:', error);
            this.showToast('error', 'Erro interno ao adicionar produto');
            this.handleError({
                code: 'ADD_TO_CART_ERROR',
                message: 'Erro ao adicionar produto ao carrinho',
                details: error,
                timestamp: new Date()
            });
        }
        finally {
            // Restaurar botão
            const button = $(`.btn-add-to-cart[data-product-id="${productId}"]`);
            const originalHtml = 'Adicionar';
            button.html(originalHtml).prop('disabled', false);
        }
    }
    /**
     * Faz requisição para adicionar ao carrinho
     */
    async makeAddToCartRequest(request) {
        return new Promise((resolve) => {
            $.ajax({
                url: '/Order/AdicionarProdutoAoCarrinho',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: (response) => {
                    resolve(response);
                },
                error: (xhr) => {
                    const errorResponse = {
                        success: false,
                        message: xhr.responseJSON?.message || 'Erro ao adicionar produto ao carrinho',
                        totalItems: 0
                    };
                    resolve(errorResponse);
                }
            });
        });
    }
    /**
     * Recarrega lista de OrderItems
     */
    async reloadOrderItems() {
        return new Promise((resolve, reject) => {
            try {
                if (!this.state.orderId) {
                    resolve();
                    return;
                }
                $.ajax({
                    url: '/Order/RecarregarItens',
                    type: 'GET',
                    data: { orderId: this.state.orderId },
                    dataType: 'html'
                })
                    .done((response) => {
                    $(this.config.orderItemsContainerSelector).html(response);
                    resolve();
                })
                    .fail((xhr) => {
                    console.error('Erro ao recarregar OrderItems:', xhr);
                    this.showToast('error', 'Erro ao atualizar carrinho');
                    reject(new Error('Erro ao recarregar OrderItems'));
                });
            }
            catch (error) {
                console.error('Erro ao recarregar OrderItems:', error);
                this.showToast('error', 'Erro ao atualizar carrinho');
                reject(error);
            }
        });
    }
    // ===================================
    // NAVEGAÇÃO E PAGINAÇÃO
    // ===================================
    /**
     * Vai para página específica
     */
    async goToPage(page) {
        try {
            if (page < 1 || page === this.state.currentPage) {
                return;
            }
            this.state.currentPage = page;
            await this.loadProducts({
                category: this.state.currentCategory,
                search: this.state.currentSearchTerm,
                page: page,
                pageSize: this.config.pageSize
            });
            this.events.onPageChanged?.(page);
            // Auto-save estado após navegar
            if (this.stateManager.currentOrderId) {
                this.saveStateForOrder(this.stateManager.currentOrderId);
            }
        }
        catch (error) {
            console.error('Erro ao navegar para página:', error);
            this.handleError({
                code: 'PAGINATION_ERROR',
                message: 'Erro ao navegar entre páginas',
                details: error,
                timestamp: new Date()
            });
        }
    }
    /**
     * Muda quantidade de itens por página
     */
    async changePageSize(newPageSize) {
        try {
            this.config.pageSize = Math.max(1, Math.min(50, newPageSize));
            this.state.currentPage = 1; // Reset para primeira página
            await this.loadProducts({
                category: this.state.currentCategory,
                search: this.state.currentSearchTerm,
                page: 1,
                pageSize: this.config.pageSize
            });
        }
        catch (error) {
            console.error('Erro ao mudar tamanho da página:', error);
        }
    }
    // ===================================
    // RENDERIZAÇÃO E UI
    // ===================================
    renderProduct(product) {
        const typeDisplay = this.getProductTypeDisplay(product.productType);
        const priceDisplay = this.formatPrice(product.unitPrice);
        const imageUrl = product.imageUrl || '/images/product-placeholder.png';
        return `
            <div class="product-item" data-product-id="${product.id}" data-product-type="${product.productType}">
                <div class="product-card">
                    <div class="product-image">
                        <img src="${imageUrl}" alt="${product.name}" />
                        ${product.productType !== 'Simple' ? `<span class="product-type-badge">${typeDisplay}</span>` : ''}
                    </div>
                    <div class="product-content">
                        <div class="product-header">
                            <h6 class="product-name">${product.name}</h6>
                            ${product.sku ? `<small class="product-sku">SKU: ${product.sku}</small>` : ''}
                        </div>
                        ${product.description ? `<p class="product-description">${product.description}</p>` : ''}
                        <div class="product-footer">
                            <div class="product-price">
                                <span class="price-value">${priceDisplay}</span>
                                ${product.categoryName ? `<small class="product-category">${product.categoryName}</small>` : ''}
                            </div>
                            <button type="button" class="btn btn-primary btn-sm btn-add-to-cart" 
                                    data-product-id="${product.id}" data-quantity="1">
                                <i class="fas fa-plus"></i> Adicionar
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }
    renderProductList(products) {
        if (products.length === 0) {
            return this.renderEmpty();
        }
        return `
            <div class="products-grid">
                ${products.map(product => this.renderProduct(product)).join('')}
            </div>
        `;
    }
    renderPagination(data) {
        if (data.totalPages <= 1) {
            return '';
        }
        const pagination = this.calculatePagination(data);
        return `
            <div class="catalog-pagination">
                <div class="pagination-info">
                    <small>${pagination.paginationInfo}</small>
                </div>
                <div class="pagination-controls">
                    <button class="btn btn-sm btn-outline-primary pagination-btn" 
                            data-page="${pagination.currentPage - 1}" 
                            ${!pagination.hasPrevious ? 'disabled' : ''}>
                        <i class="fas fa-chevron-left"></i>
                    </button>
                    <span class="pagination-current">
                        ${pagination.currentPage} de ${pagination.totalPages}
                    </span>
                    <button class="btn btn-sm btn-outline-primary pagination-btn" 
                            data-page="${pagination.currentPage + 1}" 
                            ${!pagination.hasNext ? 'disabled' : ''}>
                        <i class="fas fa-chevron-right"></i>
                    </button>
                </div>
                <div class="pagination-size">
                    <select id="page-size" class="form-select form-select-sm">
                        <option value="5" ${this.config.pageSize === 5 ? 'selected' : ''}>5</option>
                        <option value="8" ${this.config.pageSize === 8 ? 'selected' : ''}>8</option>
                        <option value="12" ${this.config.pageSize === 12 ? 'selected' : ''}>12</option>
                        <option value="16" ${this.config.pageSize === 16 ? 'selected' : ''}>16</option>
                    </select>
                </div>
            </div>
        `;
    }
    renderLoading() {
        return `
            <div class="catalog-loading">
                <div class="loading-spinner">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Carregando produtos...</span>
                    </div>
                </div>
                <p class="loading-text">Carregando catálogo de produtos...</p>
            </div>
        `;
    }
    renderEmpty() {
        const searchTerm = this.state.currentSearchTerm;
        const category = this.state.currentCategory;
        let message = 'Nenhum produto encontrado';
        if (searchTerm) {
            message = `Nenhum produto encontrado para "${searchTerm}"`;
        }
        else if (category) {
            message = `Nenhum produto encontrado na categoria "${category}"`;
        }
        return `
            <div class="catalog-empty">
                <div class="empty-icon">
                    <i class="fas fa-search"></i>
                </div>
                <h5 class="empty-title">${message}</h5>
                <p class="empty-description">Tente ajustar os filtros ou buscar por outros termos.</p>
                ${searchTerm || category ? `
                    <button type="button" class="btn btn-outline-primary btn-clear-filters" onclick="productCatalogManager.clearFilters()">
                        <i class="fas fa-times"></i> Limpar Filtros
                    </button>
                ` : ''}
            </div>
        `;
    }
    renderError(error) {
        return `
            <div class="catalog-error">
                <div class="error-icon">
                    <i class="fas fa-exclamation-triangle text-warning"></i>
                </div>
                <h5 class="error-title">Erro ao carregar produtos</h5>
                <p class="error-message">${error.message}</p>
                <button type="button" class="btn btn-primary btn-retry" onclick="productCatalogManager.retry()">
                    <i class="fas fa-redo"></i> Tentar Novamente
                </button>
            </div>
        `;
    }
    // ===================================
    // UTILITÁRIOS E HELPERS
    // ===================================
    /**
     * Limpa todos os filtros
     */
    async clearFilters() {
        this.state.currentCategory = '';
        this.state.currentSearchTerm = '';
        this.state.currentPage = 1;
        $('#product-search').val('');
        this.updateCategoryTabs('');
        await this.loadProducts({
            page: 1,
            pageSize: this.config.pageSize
        });
        this.showToast('success', 'Filtros limpos');
    }
    /**
     * Retry em caso de erro
     */
    async retry() {
        await this.loadProducts({
            category: this.state.currentCategory,
            search: this.state.currentSearchTerm,
            page: this.state.currentPage,
            pageSize: this.config.pageSize
        });
    }
    updateCategoryTabs(activeCategory) {
        $('.category-chip').removeClass('active');
        if (activeCategory) {
            $(`.category-chip[data-category="${activeCategory}"]`).addClass('active');
        }
        else {
            $('.category-chip[data-category=""]').addClass('active');
        }
    }
    showLoadingUI() {
        $(this.config.productListSelector).html(this.renderLoading());
    }
    setLoadingState(isLoading) {
        this.state.isLoading = isLoading;
        this.events.onLoadingStateChanged?.(isLoading);
    }
    handleLoadSuccess(data) {
        this.state.lastLoadedData = data;
        this.state.currentPage = data.currentPage;
        this.state.totalProducts = data.totalProducts;
        // Renderizar produtos
        const productsHtml = this.renderProductList(data.products);
        const paginationHtml = this.renderPagination(data);
        $(this.config.productListSelector).html(`
            ${productsHtml}
            ${paginationHtml}
        `);
    }
    handleLoadError(error) {
        const catalogError = {
            code: 'LOAD_ERROR',
            message: typeof error === 'string' ? error : 'Erro ao carregar produtos',
            details: error,
            timestamp: new Date()
        };
        $(this.config.productListSelector).html(this.renderError(catalogError));
        this.handleError(catalogError);
    }
    handleError(error) {
        console.error('ProductCatalogManager Error:', error);
        this.events.onError?.(error);
    }
    // ===================================
    // CACHE E PERFORMANCE
    // ===================================
    generateCacheKey(filters) {
        return `catalog_${filters.category || 'all'}_${filters.search || 'none'}_${filters.page}_${filters.pageSize}`;
    }
    getCachedData(key) {
        const cached = this.cache.get(key);
        if (cached && cached.expiration > new Date()) {
            return cached;
        }
        return null;
    }
    setCachedData(key, data) {
        const expiration = new Date();
        expiration.setMinutes(expiration.getMinutes() + 5); // Cache por 5 minutos
        this.cache.set(key, {
            key,
            data,
            timestamp: new Date(),
            expiration
        });
    }
    // ===================================
    // FORMATADORES E UTILITÁRIOS
    // ===================================
    getProductTypeDisplay(type) {
        switch (type) {
            case 'Simple':
                return 'Simples';
            case 'Composite':
                return 'Composto';
            case 'Group':
                return 'Grupo';
            default:
                return 'Produto';
        }
    }
    formatPrice(price) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(price);
    }
    calculatePagination(data) {
        const startItem = ((data.currentPage - 1) * this.config.pageSize) + 1;
        const endItem = Math.min(data.currentPage * this.config.pageSize, data.totalProducts);
        return {
            currentPage: data.currentPage,
            totalPages: data.totalPages,
            pageSize: this.config.pageSize,
            totalItems: data.totalProducts,
            hasNext: data.hasNextPage,
            hasPrevious: data.hasPreviousPage,
            startItem,
            endItem,
            paginationInfo: data.totalProducts > 0
                ? `${startItem}-${endItem} de ${data.totalProducts}`
                : '0-0 de 0'
        };
    }
    findProductById(productId) {
        return this.state.lastLoadedData?.products.find(p => p.id === productId);
    }
    /**
     * Converte string do tipo de produto para enum
     */
    parseProductType(productTypeString) {
        switch (productTypeString?.toLowerCase()) {
            case 'simple':
                return 'Simple';
            case 'composite':
                return 'Composite';
            case 'group':
                return 'Group';
            default:
                return 'Simple';
        }
    }
    showToast(type, message) {
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        }
        else {
            console.log(`[${type.toUpperCase()}] ${message}`);
        }
    }
    // ===================================
    // CLEANUP E DESTRUIÇÃO
    // ===================================
    /**
     * Limpa recursos e remove event listeners
     */
    destroy() {
        this.unbindEvents();
        this.cache.clear();
        if (this.debounceTimer !== null) {
            clearTimeout(this.debounceTimer);
            this.debounceTimer = null;
        }
        console.log('ProductCatalogManager: Recursos limpos');
    }
    // ===================================
    // GERENCIAMENTO DE ESTADO POR ABA
    // ===================================
    /**
     * Salva o estado atual para uma aba específica
     */
    saveStateForOrder(orderId) {
        if (!orderId)
            return;
        const currentState = {
            orderId: orderId,
            category: this.state.currentCategory,
            searchTerm: this.state.currentSearchTerm,
            currentPage: this.state.currentPage,
            pageSize: this.config.pageSize,
            timestamp: Date.now(),
            totalProducts: this.state.totalProducts,
            totalPages: this.state.totalPages || 1,
            scrollPosition: $(this.config.productListSelector).scrollTop() || 0
        };
        // Salvar no Map
        this.stateManager.savedStates.set(orderId, currentState);
        // Limitar número de estados em memória
        if (this.stateManager.savedStates.size > this.stateManager.maxStates) {
            const oldestKey = Array.from(this.stateManager.savedStates.keys())[0];
            this.stateManager.savedStates.delete(oldestKey);
        }
        // Persistir se habilitado
        if (this.stateManager.persistToStorage) {
            this.persistStatesToStorage();
        }
        if (this.contextConfig.debugMode) {
            console.log(`💾 Estado salvo para pedido ${orderId}:`, currentState);
        }
    }
    /**
     * Carrega estado salvo para uma aba específica
     */
    async loadStateForOrder(orderId) {
        if (!orderId)
            return false;
        const savedState = this.stateManager.savedStates.get(orderId);
        if (!savedState) {
            if (this.contextConfig.debugMode) {
                console.log(`📭 Nenhum estado salvo encontrado para pedido ${orderId}`);
            }
            return false;
        }
        // Restaurar estado
        this.state.currentCategory = savedState.category;
        this.state.currentSearchTerm = savedState.searchTerm;
        this.state.currentPage = savedState.currentPage;
        this.config.pageSize = savedState.pageSize;
        this.state.totalProducts = savedState.totalProducts || 0;
        this.state.totalPages = savedState.totalPages || 1;
        // Atualizar UI
        $('#product-search').val(savedState.searchTerm);
        this.updateCategoryTabs(savedState.category);
        // Recarregar produtos com filtros restaurados
        await this.loadProducts({
            category: savedState.category,
            search: savedState.searchTerm,
            page: savedState.currentPage,
            pageSize: savedState.pageSize
        });
        // Restaurar posição de scroll
        setTimeout(() => {
            if (savedState.scrollPosition) {
                $(this.config.productListSelector).scrollTop(savedState.scrollPosition);
            }
        }, 100);
        if (this.contextConfig.debugMode) {
            console.log(`📂 Estado restaurado para pedido ${orderId}:`, savedState);
        }
        return true;
    }
    /**
     * Troca contexto entre abas (salva atual + carrega novo)
     */
    async switchContext(newOrderId) {
        const previousOrderId = this.stateManager.currentOrderId;
        // Salvar estado atual se existir
        if (previousOrderId) {
            this.saveStateForOrder(previousOrderId);
        }
        // Atualizar OrderId atual
        this.stateManager.currentOrderId = newOrderId;
        this.state.orderId = newOrderId;
        // Tentar carregar estado salvo
        const stateRestored = await this.loadStateForOrder(newOrderId);
        // Se não há estado salvo, resetar para padrões
        if (!stateRestored) {
            await this.resetToDefaults();
        }
        const contextEvent = {
            previousOrderId,
            newOrderId,
            stateRestored,
            timestamp: Date.now()
        };
        if (this.contextConfig.debugMode) {
            console.log(`🔄 Contexto alterado:`, contextEvent);
        }
        return contextEvent;
    }
    /**
     * Reseta catálogo para valores padrão
     */
    async resetToDefaults() {
        this.state.currentCategory = '';
        this.state.currentSearchTerm = '';
        this.state.currentPage = 1;
        this.config.pageSize = 8;
        $('#product-search').val('');
        this.updateCategoryTabs('');
        await this.loadProducts({
            page: 1,
            pageSize: this.config.pageSize
        });
    }
    /**
     * Limpa estado salvo de uma aba específica
     */
    clearStateForOrder(orderId) {
        if (!orderId)
            return;
        this.stateManager.savedStates.delete(orderId);
        if (this.stateManager.persistToStorage) {
            this.persistStatesToStorage();
        }
        if (this.contextConfig.debugMode) {
            console.log(`🗑️ Estado removido para pedido ${orderId}`);
        }
    }
    /**
     * Persiste estados no localStorage
     */
    persistStatesToStorage() {
        if (!this.contextConfig.enablePersistence)
            return;
        try {
            const statesArray = Array.from(this.stateManager.savedStates.entries());
            const serialized = JSON.stringify(statesArray);
            localStorage.setItem(this.contextConfig.storageKey, serialized);
            if (this.contextConfig.debugMode) {
                console.log(`💾 ${statesArray.length} estados persistidos no localStorage`);
            }
        }
        catch (error) {
            console.error('Erro ao persistir estados:', error);
        }
    }
    /**
     * Carrega estados persistidos do localStorage
     */
    loadPersistedStates() {
        if (!this.contextConfig.enablePersistence)
            return;
        try {
            const serialized = localStorage.getItem(this.contextConfig.storageKey);
            if (serialized) {
                const statesArray = JSON.parse(serialized);
                this.stateManager.savedStates = new Map(statesArray);
                if (this.contextConfig.debugMode) {
                    console.log(`📂 ${statesArray.length} estados carregados do localStorage`);
                }
            }
        }
        catch (error) {
            console.error('Erro ao carregar estados persistidos:', error);
            this.stateManager.savedStates.clear();
        }
    }
    /**
     * Mostra catálogo lateral (slide in)
     */
    showCatalog(orderId) {
        if (this.slideControl.isAnimating)
            return;
        this.slideControl.isAnimating = true;
        this.slideControl.currentOrderId = orderId;
        this.slideControl.slideDirection = 'in';
        // Trocar contexto
        this.switchContext(orderId);
        // Animar slide
        $('#orderWorkspace').addClass('catalog-active');
        setTimeout(() => {
            this.slideControl.isVisible = true;
            this.slideControl.isAnimating = false;
        }, 400);
        if (this.contextConfig.debugMode) {
            console.log(`👁️ Catálogo exibido para pedido ${orderId}`);
        }
    }
    /**
     * Esconde catálogo lateral (slide out)
     */
    hideCatalog() {
        if (this.slideControl.isAnimating)
            return;
        this.slideControl.isAnimating = true;
        this.slideControl.slideDirection = 'out';
        // Salvar estado atual antes de esconder
        if (this.stateManager.currentOrderId) {
            this.saveStateForOrder(this.stateManager.currentOrderId);
        }
        // Animar slide
        $('#orderWorkspace').removeClass('catalog-active');
        setTimeout(() => {
            this.slideControl.isVisible = false;
            this.slideControl.isAnimating = false;
            this.slideControl.currentOrderId = null;
            this.stateManager.currentOrderId = null;
        }, 400);
        if (this.contextConfig.debugMode) {
            console.log(`🙈 Catálogo escondido`);
        }
    }
    /**
     * Alterna visibilidade do catálogo
     */
    toggleCatalog(orderId) {
        if (this.slideControl.isVisible) {
            this.hideCatalog();
        }
        else {
            // Usar orderId fornecido ou orderId atual
            const targetOrderId = orderId || this.stateManager.currentOrderId;
            if (targetOrderId) {
                this.showCatalog(targetOrderId);
            }
            else {
                console.warn('⚠️ Nenhum pedido ativo para mostrar catálogo');
                this.showToast('warning', 'Abra um pedido para visualizar o catálogo');
            }
        }
    }
    /**
     * Obtém estados salvos (para debug)
     */
    getSavedStates() {
        return this.stateManager.savedStates;
    }
    /**
     * Limpa todos os estados salvos
     */
    clearAllStates() {
        this.stateManager.savedStates.clear();
        if (this.contextConfig.enablePersistence) {
            localStorage.removeItem(this.contextConfig.storageKey);
        }
        if (this.contextConfig.debugMode) {
            console.log(`🧹 Todos os estados foram limpos`);
        }
    }
}
// ===================================
// INSTÂNCIA GLOBAL E INICIALIZAÇÃO
// ===================================
// Instância global mantendo compatibilidade
const productCatalogManager = new ProductCatalogManager();
// Disponibilizar globalmente
window.productCatalogManager = productCatalogManager;
// Auto-inicialização removida - será feita pelo OrderManager
// quando a view de edição for carregada via AJAX
//# sourceMappingURL=ProductCatalogManager.js.map