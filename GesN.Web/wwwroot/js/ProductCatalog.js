/**
 * ProductCatalog.js
 * Gerencia o catálogo de produtos e integração com carrinho de pedidos
 * Responsável por: carregamento de produtos, filtros, busca, paginação e adição ao carrinho
 */

const productCatalog = {
    orderId: null,
    currentPage: 1,
    pageSize: 5,
    currentCategory: '',
    currentSearchTerm: '',
    
    /**
     * Inicializa o catálogo de produtos
     * @param {string} orderId - ID do pedido em edição
     */
    init: function(orderId) {
        this.orderId = orderId;
        this.bindEvents();
        this.loadCatalog();
    },
    
    /**
     * Associa eventos aos elementos da interface
     */
    bindEvents: function() {
        // Click nas tabs de categoria
        $(document).off('click', '#categoryTabs button').on('click', '#categoryTabs button', (e) => {
            e.preventDefault();
            const category = $(e.target).data('category') || '';
            this.filterByCategory(category);
            
            // Atualizar UI das tabs
            $('#categoryTabs button').removeClass('active');
            $(e.target).addClass('active');
        });
        
        // Botão de busca
        $(document).off('click', '#search-btn').on('click', '#search-btn', () => {
            this.performSearch();
        });
        
        // Enter no campo de busca
        $(document).off('keypress', '#search-input').on('keypress', '#search-input', (e) => {
            if (e.which === 13) { // Enter key
                this.performSearch();
            }
        });
        
        // Botão adicionar produto ao carrinho
        $(document).off('click', '.add-to-cart').on('click', '.add-to-cart', (e) => {
            e.preventDefault();
            const productId = $(e.currentTarget).data('product-id');
            this.addProductToCart(productId);
        });
        
        // Paginação
        $(document).off('click', '.pagination-btn').on('click', '.pagination-btn', (e) => {
            e.preventDefault();
            const page = parseInt($(e.target).data('page'));
            if (page && page !== this.currentPage) {
                this.loadPage(page);
            }
        });
        
        // Seletor de itens por página
        $(document).off('change', '#page-size-selector').on('change', '#page-size-selector', () => {
            this.pageSize = parseInt($('#page-size-selector').val()) || 5;
            this.currentPage = 1; // Reset para primeira página
            this.loadCatalog();
        });
    },
    
    /**
     * Carrega o catálogo completo
     */
    loadCatalog: function() {
        this.makeRequest('');
    },
    
    /**
     * Filtra produtos por categoria
     * @param {string} category - Categoria selecionada
     */
    filterByCategory: function(category) {
        this.currentCategory = category;
        this.currentPage = 1; // Reset para primeira página
        this.loadCatalog();
    },
    
    /**
     * Executa busca de produtos
     */
    performSearch: function() {
        this.currentSearchTerm = $('#search-input').val() || '';
        this.currentPage = 1; // Reset para primeira página
        this.loadCatalog();
    },
    
    /**
     * Carrega página específica
     * @param {number} page - Número da página
     */
    loadPage: function(page) {
        this.currentPage = page;
        this.loadCatalog();
    },
    
    /**
     * Faz requisição AJAX para carregar produtos
     * @param {string} category - Categoria (opcional)
     */
    makeRequest: function(category = null) {
        const params = {
            category: category || this.currentCategory,
            search: this.currentSearchTerm,
            page: this.currentPage,
            pageSize: this.pageSize
        };
        
        // Mostrar loading
        $('#productListContainer').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Carregando produtos...</span>
                </div>
            </div>
        `);
        
        $.get('/Product/CatalogProducts', params)
            .done((data) => {
                $('#productCatalogContainer').html(data);
                this.bindEvents(); // Re-bind eventos após reload
            })
            .fail((xhr) => {
                console.error('Erro ao carregar catálogo:', xhr);
                toastr.error('Erro ao carregar catálogo de produtos');
                
                $('#productListContainer').html(`
                    <div class="text-center py-4">
                        <i class="fas fa-exclamation-triangle text-warning"></i>
                        <p class="mt-2 mb-1">Erro ao carregar produtos</p>
                        <button class="btn btn-sm btn-primary" onclick="productCatalog.loadCatalog()">
                            <i class="fas fa-redo"></i> Tentar novamente
                        </button>
                    </div>
                `);
            });
    },
    
    /**
     * Adiciona produto ao carrinho (OrderItems)
     * @param {string} productId - ID do produto
     */
    addProductToCart: function(productId) {
        if (!this.orderId) {
            toastr.error('ID do pedido não encontrado');
            return;
        }
        
        // Mostrar loading no botão
        const button = $(`.add-to-cart[data-product-id="${productId}"]`);
        const originalHtml = button.html();
        button.html('<i class="fas fa-spinner fa-spin"></i>').prop('disabled', true);
        
        $.post('/Order/AddOrderItem', {
            orderEntryId: this.orderId,
            productId: productId,
            quantity: 1 // Quantidade padrão
        })
        .done((response) => {
            if (response.success) {
                toastr.success('Produto adicionado ao carrinho');
                
                // Recarregar lista de OrderItems (Coluna 2)
                this.reloadOrderItems();
            } else {
                toastr.error(response.message || 'Erro ao adicionar produto');
            }
        })
        .fail((xhr) => {
            console.error('Erro ao adicionar produto:', xhr);
            toastr.error('Erro ao adicionar produto ao carrinho');
        })
        .always(() => {
            // Restaurar botão
            button.html(originalHtml).prop('disabled', false);
        });
    },
    
    /**
     * Recarrega a lista de OrderItems (Coluna 2)
     */
    reloadOrderItems: function() {
        if (!this.orderId) return;
        
        $.get('/Order/GetOrderItems', { orderEntryId: this.orderId })
            .done((data) => {
                $('#order-items-container').html(data);
            })
            .fail((xhr) => {
                console.error('Erro ao recarregar itens:', xhr);
            });
    }
};

/**
 * Auto-inicialização quando o DOM estiver pronto
 * Procura pelo container de edição de pedido e extrai o ID necessário
 */
$(document).ready(function() {
    const editContainer = $('#editOrderContainer');
    if (editContainer.length > 0) {
        // Procurar por input hidden com o ID do pedido
        const orderIdInput = $('input[name="Id"]');
        if (orderIdInput.length > 0) {
            const orderId = orderIdInput.val();
            if (orderId) {
                productCatalog.init(orderId);
            }
        }
    }
});

/**
 * Função global para inicializar manualmente (se necessário)
 * @param {string} orderId - ID do pedido
 */
function initProductCatalog(orderId) {
    productCatalog.init(orderId);
}
