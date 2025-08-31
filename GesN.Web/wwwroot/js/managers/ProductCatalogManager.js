"use strict";
// ===================================
// PRODUCT CATALOG MANAGER - VERSÃO SIMPLIFICADA (Padrão _Grid.cshtml)
// Seguindo exatamente o padrão usado em _Grid.cshtml do projeto
// ===================================
/// <reference path="../interfaces/common.ts" />
/// <reference path="../interfaces/product-catalog.ts" />
/// <reference path="../types/globals.d.ts" />
class ProductCatalogManager {
    constructor() {
        this.dataTable = null;
        this.debounceTimer = null;
        this.isInitializing = false;
        // Configuração para DataTables
        this.config = {
            baseUrl: '/Product',
            catalogContainerSelector: '#productCatalogContainer',
            tableSelector: '#productCatalogTable',
            categoryFiltersSelector: '.category-filter',
            summarySelector: '#catalog-summary'
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
        // Eventos customizados (compatível com interfaces existentes)
        this.events = {
            onSimpleProductSelected: (productId, quantity) => {
                const event = new CustomEvent('simple-product-added', {
                    detail: { productId, quantity }
                });
                document.dispatchEvent(event);
            },
            onCompositeProductConfigured: (productId, config) => {
                const event = new CustomEvent('composite-product-configured', {
                    detail: { productId, config }
                });
                document.dispatchEvent(event);
            },
            onGroupProductConfigured: (productId, config) => {
                const event = new CustomEvent('group-product-configured', {
                    detail: { productId, config }
                });
                document.dispatchEvent(event);
            },
            onError: (error) => {
                const event = new CustomEvent('catalog-error', {
                    detail: error
                });
                document.dispatchEvent(event);
            }
        };
        // State manager
        this.stateManager = new CatalogStateManager();
        console.log('🏗️ ProductCatalogManager inicializado (padrão _Grid.cshtml)');
    }
    // ===================================
    // INICIALIZAÇÃO (Seguindo padrão _Grid.cshtml)
    // ===================================
    async init(orderId) {
        // Prevenir múltiplas inicializações simultâneas
        if (this.isInitializing) {
            console.log(`⏳ Inicialização já em andamento para pedido: ${orderId}`);
            return;
        }
        // Se já está inicializado para este pedido, não precisa reinicializar
        if (this.dataTable && this.state.orderId === orderId) {
            console.log(`✅ Catálogo já inicializado para pedido: ${orderId}`);
            return;
        }
        try {
            this.isInitializing = true;
            console.log(`🏗️ Inicializando catálogo para pedido: ${orderId} (padrão _Grid.cshtml)`);
            this.state.orderId = orderId;
            this.stateManager.currentOrderId = orderId;
            // Carregar dados e renderizar HTML (como _Grid.cshtml)
            await this.loadCatalogData(orderId);
            this.loadStateForOrder(orderId);
            console.log('✅ Catálogo inicializado seguindo padrão do projeto');
        }
        catch (error) {
            console.error('❌ Erro ao inicializar catálogo:', error);
            this.showToast('error', 'Erro ao inicializar catálogo de produtos');
        }
        finally {
            this.isInitializing = false;
        }
    }
    async loadCatalogData(orderId) {
        try {
            console.log(`🔄 Carregando catálogo para pedido: ${orderId} (padrão _Grid.cshtml)`);
            // Buscar HTML do catálogo via AJAX (como _Grid.cshtml)
            const response = await $.get(`/Order/ProductCatalog?category=${encodeURIComponent(this.state.currentCategory)}&search=${encodeURIComponent(this.state.currentSearchTerm)}`);
            // Injetar HTML no container
            $(this.config.catalogContainerSelector).html(response);
            console.log(`✅ HTML do catálogo carregado`);
            // Aguardar DOM ser atualizado e inicializar DataTables
            setTimeout(() => {
                this.initializeStaticDataTable();
            }, 100);
        }
        catch (error) {
            console.error('❌ Erro ao carregar catálogo:', error);
            this.showToast('error', 'Erro ao carregar catálogo de produtos');
            throw error;
        }
    }
    initializeStaticDataTable() {
        try {
            console.log('🔄 Inicializando DataTable estático (padrão _Grid.cshtml)');
            const tableElement = $(this.config.tableSelector);
            if (!tableElement.length) {
                console.warn('⚠️ Tabela do catálogo não encontrada:', this.config.tableSelector);
                return;
            }
            // Verificar se há dados na tabela
            const rowCount = tableElement.find('tbody tr').length;
            console.log(`📊 Linhas encontradas no HTML: ${rowCount}`);
            // Destruir instância existente se houver
            if ($.fn.DataTable.isDataTable(this.config.tableSelector)) {
                $(this.config.tableSelector).DataTable().destroy();
                console.log('🗑️ DataTable anterior destruído');
            }
            // Configuração simplificada seguindo padrão do projeto (como _Grid.cshtml)
            const dataTableConfig = {
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                },
                responsive: true,
                pageLength: 10,
                lengthMenu: [[5, 10, 25, 50], [5, 10, 25, 50]],
                order: [[0, 'asc']], // Ordenar por nome do produto (primeira coluna agora)
                columnDefs: [
                    {
                        targets: [3], // Coluna de ações (ajustado para 4 colunas)
                        orderable: false,
                        searchable: false,
                        className: 'text-center'
                    },
                    {
                        targets: [2], // Coluna de preço (ajustado para 4 colunas)
                        orderable: true,
                        searchable: false,
                        className: 'text-end'
                    }
                ],
                dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rtip',
                drawCallback: () => {
                    // Reaplica eventos após redraw da tabela (padrão do projeto)
                    this.bindAddToCartEvents();
                }
            };
            // Inicializar DataTable (como ordersTable em _Grid.cshtml)
            this.dataTable = tableElement.DataTable(dataTableConfig);
            console.log('✅ DataTable estático inicializado seguindo padrão _Grid.cshtml');
            console.log('🔍 Produtos na tabela:', this.dataTable.rows().count());
            // Configurar eventos
            this.setupEventListeners();
            this.bindAddToCartEvents();
            this.updateCatalogSummary();
        }
        catch (error) {
            console.error('❌ Erro ao inicializar DataTable estático:', error);
            this.showToast('error', 'Erro ao configurar tabela do catálogo');
            throw error;
        }
    }
    // ===================================
    // FILTROS E BUSCA (Server-side como _Grid.cshtml)
    // ===================================
    filterByCategory(category) {
        console.log(`🏷️ Filtro aplicado: categoria '${category}' (server-side)`);
        this.state.currentCategory = category;
        // Atualizar visual dos botões de filtro
        $(this.config.categoryFiltersSelector).removeClass('active');
        $(`[data-category="${category}"]`).addClass('active');
        // Recarregar catálogo com filtro server-side (padrão do projeto)
        if (this.state.orderId) {
            this.loadCatalogData(this.state.orderId);
        }
    }
    searchProducts(searchTerm) {
        // Limpar timer anterior
        if (this.debounceTimer) {
            clearTimeout(this.debounceTimer);
        }
        // Aplicar debounce de 500ms
        this.debounceTimer = window.setTimeout(() => {
            console.log(`🔍 Busca aplicada: '${searchTerm}' (server-side)`);
            this.state.currentSearchTerm = searchTerm;
            // Recarregar catálogo com busca server-side (padrão do projeto)
            if (this.state.orderId) {
                this.loadCatalogData(this.state.orderId);
            }
        }, 500);
    }
    // ===================================
    // EVENT HANDLERS E BINDINGS
    // ===================================
    setupEventListeners() {
        // Remover listeners existentes para evitar duplicação
        $(document).off('click.productCatalog');
        // Filtros de categoria (com namespace para evitar conflitos)
        $(document).on('click.productCatalog', this.config.categoryFiltersSelector, (e) => {
            e.preventDefault();
            const category = $(e.currentTarget).data('category') || '';
            this.filterByCategory(category);
        });
    }
    bindAddToCartEvents() {
        // Remover eventos anteriores para evitar duplicação
        $('.btn-add-simple, .btn-configure-composite, .btn-configure-group').off('click.catalog');
        // Produtos simples
        $('.btn-add-simple').on('click.catalog', async (e) => {
            e.preventDefault();
            const $btn = $(e.currentTarget);
            const productId = $btn.data('product-id');
            await this.addSimpleToCart(productId, 1);
        });
        // Produtos compostos
        $('.btn-configure-composite').on('click.catalog', async (e) => {
            e.preventDefault();
            const $btn = $(e.currentTarget);
            const productId = $btn.data('product-id');
            await this.showCompositeProductModal(productId);
        });
        // Grupos de produtos
        $('.btn-configure-group').on('click.catalog', async (e) => {
            e.preventDefault();
            const $btn = $(e.currentTarget);
            const productId = $btn.data('product-id');
            await this.showGroupProductModal(productId);
        });
    }
    // ===================================
    // ADICIONAR PRODUTOS AO CARRINHO
    // ===================================
    async addSimpleToCart(productId, quantity = 1) {
        try {
            console.log(`➕ Adicionando produto simples ao carrinho: ${productId}`);
            this.events.onSimpleProductSelected?.(productId, quantity);
        }
        catch (error) {
            console.error('❌ Erro ao adicionar produto simples:', error);
            this.events.onError?.({
                action: 'add-simple-product',
                message: error instanceof Error ? error.message : String(error),
                details: { productId, quantity },
                timestamp: Date.now()
            });
        }
    }
    // ===================================
    // MODAIS DE CONFIGURAÇÃO
    // ===================================
    async showCompositeProductModal(productId, initialQuantity = 1) {
        try {
            console.log(`🔧 Abrindo modal de produto composto: ${productId}`);
            const response = await $.get(`/Order/CompositeProductModal/${productId}?initialQuantity=${initialQuantity}`);
            const $modal = $('#compositeProductModal');
            $modal.find('.modal-body').html(response);
            $modal.modal('show');
        }
        catch (error) {
            console.error('❌ Erro ao abrir modal de produto composto:', error);
            this.showToast('error', 'Erro ao carregar configuração do produto');
        }
    }
    async showGroupProductModal(productId, initialQuantity = 1) {
        try {
            console.log(`👥 Abrindo modal de grupo de produtos: ${productId}`);
            const response = await $.get(`/Order/GroupProductModal/${productId}?initialQuantity=${initialQuantity}`);
            const $modal = $('#groupProductModal');
            $modal.find('.modal-body').html(response);
            $modal.modal('show');
        }
        catch (error) {
            console.error('❌ Erro ao abrir modal de grupo de produtos:', error);
            this.showToast('error', 'Erro ao carregar configuração do grupo');
        }
    }
    confirmCompositeAddToCart(productId, config) {
        console.log(`✅ Confirmando adição de produto composto:`, { productId, config });
        this.events.onCompositeProductConfigured?.(productId, config);
    }
    confirmGroupAddToCart(productId, config) {
        console.log(`✅ Confirmando adição de grupo de produtos:`, { productId, config });
        this.events.onGroupProductConfigured?.(productId, config);
    }
    // ===================================
    // UTILITÁRIOS E HELPERS
    // ===================================
    updateCatalogSummary() {
        if (this.dataTable) {
            const info = this.dataTable.page.info();
            const summaryText = `${info.recordsDisplay} produtos encontrados`;
            $(this.config.summarySelector).text(summaryText);
        }
    }
    showToast(type, message) {
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        }
        else {
            console.log(`Toast ${type}: ${message}`);
        }
    }
    // ===================================
    // GERENCIAMENTO DE ESTADO
    // ===================================
    saveStateForOrder(orderId) {
        this.stateManager.saveState(orderId, {
            orderId: orderId,
            category: this.state.currentCategory,
            searchTerm: this.state.currentSearchTerm,
            currentPage: this.state.currentPage,
            pageSize: 10,
            timestamp: Date.now()
        });
    }
    loadStateForOrder(orderId) {
        const savedState = this.stateManager.loadState(orderId);
        if (savedState) {
            this.state.currentCategory = savedState.category || '';
            this.state.currentSearchTerm = savedState.searchTerm || '';
            // Atualizar UI com estado salvo
            $(this.config.categoryFiltersSelector).removeClass('active');
            $(`[data-category="${this.state.currentCategory}"]`).addClass('active');
        }
    }
    // ===================================
    // INTERFACE PÚBLICA (Compatibilidade OrderManager)
    // ===================================
    showCatalog(orderId) {
        try {
            // Inicializar se necessário
            if (!this.dataTable || this.state.orderId !== orderId) {
                console.log(`👁️ Inicializando catálogo para pedido: ${orderId}`);
                this.init(orderId);
            }
            // Mostrar sidebar
            this.showCatalogSidebar();
        }
        catch (error) {
            console.error('❌ Erro em showCatalog:', error);
            this.showToast('error', 'Erro ao mostrar catálogo de produtos');
        }
    }
    hideCatalog() {
        $('#orderWorkspace').removeClass('catalog-active');
    }
    toggleCatalog(orderId) {
        const workspace = $('#orderWorkspace');
        if (workspace.hasClass('catalog-active')) {
            this.hideCatalog();
        }
        else if (orderId || this.state.orderId) {
            this.showCatalog(orderId || this.state.orderId);
        }
    }
    showCatalogSidebar() {
        $('#orderWorkspace').addClass('catalog-active');
    }
    async switchContext(orderId) {
        // Se já estamos no contexto correto, não fazer nada
        if (this.state.orderId === orderId && this.dataTable) {
            console.log(`✅ Já no contexto correto para pedido: ${orderId}`);
            this.showCatalogSidebar();
            return;
        }
        try {
            // Salvar estado atual se existe um orderId ativo e diferente
            if (this.state.orderId && this.state.orderId !== orderId) {
                this.saveStateForOrder(this.state.orderId);
            }
            // Se DataTable não está inicializada, inicializar primeiro
            if (!this.dataTable) {
                console.log(`🏗️ DataTable não inicializada, chamando init() para pedido: ${orderId}`);
                await this.init(orderId);
                return; // init() já configura tudo necessário
            }
            // Mudar para o novo contexto
            this.state.orderId = orderId;
            // Carregar estado salvo para este pedido (se existir)
            this.loadStateForOrder(orderId);
            // Recarregar catálogo para o novo contexto (server-side)
            await this.loadCatalogData(orderId);
            // Mostrar a sidebar do catálogo
            this.showCatalogSidebar();
        }
        catch (error) {
            console.error('❌ Erro em switchContext:', error);
            this.showToast('error', 'Erro ao trocar contexto do catálogo');
        }
    }
    clearStateForOrder(orderId) {
        console.log(`🧹 Limpando estado para pedido ${orderId}`);
        this.stateManager.clearState(orderId);
        if (this.state.orderId === orderId) {
            this.state.currentCategory = '';
            this.state.currentSearchTerm = '';
        }
    }
}
// ===================================
// STATE MANAGER (Reutilizado)
// ===================================
class CatalogStateManager {
    constructor() {
        this.currentOrderId = null;
        this.STORAGE_KEY = 'product_catalog_state';
    }
    saveState(orderId, state) {
        try {
            const allStates = this.getAllStates();
            allStates[orderId] = state;
            localStorage.setItem(this.STORAGE_KEY, JSON.stringify(allStates));
        }
        catch (error) {
            console.warn('⚠️ Erro ao salvar estado do catálogo:', error);
        }
    }
    loadState(orderId) {
        try {
            const allStates = this.getAllStates();
            return allStates[orderId] || null;
        }
        catch (error) {
            console.warn('⚠️ Erro ao carregar estado do catálogo:', error);
            return null;
        }
    }
    clearState(orderId) {
        try {
            const allStates = this.getAllStates();
            delete allStates[orderId];
            localStorage.setItem(this.STORAGE_KEY, JSON.stringify(allStates));
        }
        catch (error) {
            console.warn('⚠️ Erro ao limpar estado do catálogo:', error);
        }
    }
    getAllStates() {
        try {
            const stored = localStorage.getItem(this.STORAGE_KEY);
            return stored ? JSON.parse(stored) : {};
        }
        catch (error) {
            console.warn('⚠️ Erro ao ler estados do localStorage:', error);
            return {};
        }
    }
}
// ===================================
// EXPOSIÇÃO GLOBAL
// ===================================
if (typeof window !== 'undefined') {
    window.productCatalogManager = new ProductCatalogManager();
    console.log('🏗️ ProductCatalogManager inicializado (padrão _Grid.cshtml)');
}
//# sourceMappingURL=ProductCatalogManager.js.map