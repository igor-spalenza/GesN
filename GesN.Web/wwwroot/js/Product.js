// Product Management JavaScript
const productManager = {
    // Configurações globais
    config: {
        baseUrl: '/Product',
        modalId: '#productModal',
        gridId: '#productsTable',
        currentProductId: null,
        currentEditMode: null
    },

    // Inicialização
    init: function() {
        this.bindEvents();
        this.loadGrid();
        this.loadStatistics();
    },

    // Eventos principais
    bindEvents: function() {
        // Botão novo produto
        $('#btnNovoProduct').on('click', () => this.novoProduct());
        
        // Filtros
        $('#filterProductType').on('change', () => this.aplicarFiltros());
        $('#filterCategory').on('change', () => this.aplicarFiltros());
        $('#filterStatus').on('change', () => this.aplicarFiltros());
        $('#searchProduct').on('input', debounce(() => this.aplicarFiltros(), 300));
        
        // Botão limpar filtros
        $('#btnLimparFiltros').on('click', () => this.limparFiltros());
    },

    // Carregar grid principal
    loadGrid: function() {
        $.ajax({
            url: `${this.config.baseUrl}/_Grid`,
            type: 'GET',
            data: this.getFilterData(),
            success: (data) => {
                $('#gridContainer').html(data);
                this.initializeDataTable();
            },
            error: (xhr) => {
                console.error('Erro ao carregar grid:', xhr);
                toastr.error('Erro ao carregar lista de produtos');
            }
        });
    },

    // Inicializar DataTable
    initializeDataTable: function() {
        if ($.fn.DataTable.isDataTable(this.config.gridId)) {
            $(this.config.gridId).DataTable().destroy();
        }

        $(this.config.gridId).DataTable({
            language: {
                url: '/lib/datatables.net/pt-BR.json'
            },
            responsive: true,
            pageLength: 25,
            order: [[1, 'asc']], // Ordenar por nome
            columnDefs: [
                { targets: [0, -1], orderable: false }, // Primeira e última coluna não ordenáveis
                { targets: [4, 5], className: 'text-end' }, // Preço e estoque alinhados à direita
            ]
        });
    },

    // Carregar estatísticas
    loadStatistics: function() {
        $.ajax({
            url: `${this.config.baseUrl}/Statistics`,
            type: 'GET',
            success: (data) => {
                $('#totalProducts').text(data.totalProducts);
                $('#activeProducts').text(data.activeProducts);
                $('#lowStockProducts').text(data.lowStockProducts);
                $('#totalValue').text(data.totalValue);
                
                // Atualizar badges de alerta
                if (data.lowStockProducts > 0) {
                    $('#lowStockBadge').removeClass('d-none').text(data.lowStockProducts);
                } else {
                    $('#lowStockBadge').addClass('d-none');
                }
            },
            error: (xhr) => {
                console.error('Erro ao carregar estatísticas:', xhr);
            }
        });
    },

    // Obter dados dos filtros
    getFilterData: function() {
        return {
            productType: $('#filterProductType').val(),
            categoryId: $('#filterCategory').val(),
            status: $('#filterStatus').val(),
            search: $('#searchProduct').val()
        };
    },

    // Aplicar filtros
    aplicarFiltros: function() {
        this.loadGrid();
        this.loadStatistics();
    },

    // Limpar filtros
    limparFiltros: function() {
        $('#filterProductType').val('');
        $('#filterCategory').val('');
        $('#filterStatus').val('');
        $('#searchProduct').val('');
        this.aplicarFiltros();
    },

    // Novo produto
    novoProduct: function() {
        this.config.currentEditMode = 'create';
        
        $.ajax({
            url: `${this.config.baseUrl}/FormularioCriacao`,
            type: 'GET',
            success: (data) => {
                $(this.config.modalId + ' .modal-title').html('<i class="fas fa-plus"></i> Novo Produto');
                $(this.config.modalId + ' .modal-body').html(data);
                $(this.config.modalId + ' .modal-dialog').removeClass('modal-xl').addClass('modal-lg');
                $(this.config.modalId).modal('show');
            },
            error: (xhr) => {
                console.error('Erro ao abrir modal de criação:', xhr);
                toastr.error('Erro ao abrir formulário de criação');
            }
        });
    },

    // Função de compatibilidade para resolver problemas de cache
    novoProductModal: function() {
        console.warn('Função novoProductModal é deprecated, use novoProduct()');
        this.novoProduct();
    },

    // Salvar novo produto
    salvarNovoProduct: function() {
        const form = $('#formNovoProduct');
        const formData = form.serialize();

        if (!this.validateForm(form)) {
            return;
        }

        $.ajax({
            url: `${this.config.baseUrl}/SalvarNovo`,
            type: 'POST',
            data: formData,
            success: (response) => {
                if (response.success) {
                    toastr.success('Produto criado com sucesso!');
                    $(this.config.modalId).modal('hide');
                    this.loadGrid();
                    this.loadStatistics();
                    
                    // Se for produto composto ou grupo, abrir para edição
                    if (response.productType !== 0) { // 0 = Simple
                        setTimeout(() => {
                            this.editarProduct(response.productId);
                        }, 500);
                    }
                } else {
                    toastr.error(response.message || 'Erro ao criar produto');
                    this.showValidationErrors(response.errors);
                }
            },
            error: (xhr) => {
                console.error('Erro ao salvar produto:', xhr);
                toastr.error('Erro ao salvar produto');
            }
        });
    },

    // Ver detalhes do produto
    verDetalhes: function(productId) {
        $.ajax({
            url: `${this.config.baseUrl}/DetalhesProduct/${productId}`,
            type: 'GET',
            success: (data) => {
                $(this.config.modalId + ' .modal-title').html('<i class="fas fa-eye"></i> Detalhes do Produto');
                $(this.config.modalId + ' .modal-body').html(data);
                $(this.config.modalId + ' .modal-dialog').removeClass('modal-lg').addClass('modal-xl');
                $(this.config.modalId).modal('show');
            },
            error: (xhr) => {
                console.error('Erro ao carregar detalhes:', xhr);
                toastr.error('Erro ao carregar detalhes do produto');
            }
        });
    },

    // Editar produto
    editarProduct: function(productId) {
        this.config.currentProductId = productId;
        this.config.currentEditMode = 'edit';
        
        $.ajax({
            url: `${this.config.baseUrl}/FormularioEdicao/${productId}`,
            type: 'GET',
            success: (data) => {
                $(this.config.modalId + ' .modal-title').html('<i class="fas fa-edit"></i> Editar Produto');
                $(this.config.modalId + ' .modal-body').html(data);
                $(this.config.modalId + ' .modal-dialog').removeClass('modal-lg').addClass('modal-xl');
                $(this.config.modalId).modal('show');
            },
            error: (xhr) => {
                console.error('Erro ao abrir edição:', xhr);
                toastr.error('Erro ao abrir formulário de edição');
            }
        });
    },

    // Salvar edição
    salvarEdicao: function() {
        const form = $('#formEditProduct');
        const formData = form.serialize();

        if (!this.validateForm(form)) {
            return;
        }

        $.ajax({
            url: `${this.config.baseUrl}/SalvarEdicaoProduct/${this.config.currentProductId}`,
            type: 'POST',
            data: formData,
            success: (response) => {
                if (response.success) {
                    toastr.success('Produto atualizado com sucesso!');
                    this.loadGrid();
                    this.loadStatistics();
                } else {
                    toastr.error(response.message || 'Erro ao atualizar produto');
                    this.showValidationErrors(response.errors);
                }
            },
            error: (xhr) => {
                console.error('Erro ao salvar edição:', xhr);
                toastr.error('Erro ao salvar alterações');
            }
        });
    },

    // Cancelar edição
    cancelarEdicao: function() {
        $(this.config.modalId).modal('hide');
    },

    // Excluir produto
    excluirProduct: function(productId, productName) {
        if (confirm(`Tem certeza que deseja excluir o produto "${productName}"?\n\nEsta ação não pode ser desfeita.`)) {
            $.ajax({
                url: `${this.config.baseUrl}/ExcluirProduct/${productId}`,
                type: 'POST',
                data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
                success: (response) => {
                    if (response.success) {
                        toastr.success('Produto excluído com sucesso!');
                        this.loadGrid();
                        this.loadStatistics();
                    } else {
                        toastr.error(response.message || 'Erro ao excluir produto');
                    }
                },
                error: (xhr) => {
                    console.error('Erro ao excluir produto:', xhr);
                    toastr.error('Erro ao excluir produto');
                }
            });
        }
    },

    // Gerenciar componentes (para produtos compostos)
    gerenciarComponentes: function(productId) {
        this.components.abrirGerenciamento(productId);
    },

    // Gerenciar grupo (para grupos de produtos)
    gerenciarGrupo: function(productId) {
        this.groupItems.abrirGerenciamento(productId);
    },

    // Validação de formulário
    validateForm: function(form) {
        let isValid = true;
        
        // Limpar erros anteriores
        form.find('.is-invalid').removeClass('is-invalid');
        form.find('.text-danger').empty();
        
        // Validar campos obrigatórios
        form.find('[required]').each(function() {
            if (!$(this).val().trim()) {
                $(this).addClass('is-invalid');
                isValid = false;
            }
        });
        
        // Validações específicas
        const price = parseFloat(form.find('#Price').val()) || 0;
        const cost = parseFloat(form.find('#Cost').val()) || 0;
        
        if (price > 0 && cost > 0 && price <= cost) {
            toastr.warning('Atenção: O preço de venda está menor ou igual ao custo!');
        }
        
        return isValid;
    },

    // Mostrar erros de validação
    showValidationErrors: function(errors) {
        if (errors) {
            Object.keys(errors).forEach(key => {
                const field = $(`[name="${key}"]`);
                if (field.length) {
                    field.addClass('is-invalid');
                    field.siblings('.text-danger').text(errors[key][0]);
                }
            });
        }
    },

    // Filtros rápidos
    filtrarPorStatus: function(status) {
        if (status === 'todos') {
            $('#filterStatus').val('');
        } else if (status === 'ativo') {
            $('#filterStatus').val('1');
        } else {
            $('#filterStatus').val('0');
        }
        this.aplicarFiltros();
    },

    filtrarPorTipo: function(tipo) {
        $('#filterProductType').val(tipo);
        this.aplicarFiltros();
    },

    filtrarEstoqueBaixo: function() {
        // Implementar filtro para estoque baixo
        console.log('Filtrar produtos com estoque baixo');
        // Por enquanto, apenas aplicar filtros existentes
        this.aplicarFiltros();
    },

    // Namespace para gerenciamento de componentes
    components: {
        // Carregar componentes
        carregarComponentes: function(productId) {
            $.ajax({
                url: `/ProductComponent/List/${productId}`,
                type: 'GET',
                success: (data) => {
                    $('#componentsContainer').html(data);
                },
                error: (xhr) => {
                    console.error('Erro ao carregar componentes:', xhr);
                    $('#componentsContainer').html('<div class="alert alert-danger">Erro ao carregar componentes</div>');
                }
            });
        },

        // Abrir gerenciamento de componentes
        abrirGerenciamento: function(productId) {
            // Implementar modal específico para componentes
            console.log('Gerenciar componentes do produto:', productId);
        },

        // Adicionar componente
        adicionarComponente: function(productId) {
            // Implementar adição de componente
        },

        // Editar componente
        editarComponente: function(componentId) {
            // Implementar edição de componente
        },

        // Remover componente
        removerComponente: function(componentId) {
            // Implementar remoção de componente
        }
    },

    // Namespace para gerenciamento de itens do grupo
    groupItems: {
        // Carregar itens do grupo
        carregarItens: function(productId) {
            $.ajax({
                url: `/ProductGroup/Items/${productId}`,
                type: 'GET',
                success: (data) => {
                    $('#groupItemsContainer').html(data);
                },
                error: (xhr) => {
                    console.error('Erro ao carregar itens do grupo:', xhr);
                    $('#groupItemsContainer').html('<div class="alert alert-danger">Erro ao carregar itens do grupo</div>');
                }
            });
        },

        // Abrir gerenciamento de grupo
        abrirGerenciamento: function(productId) {
            // Implementar modal específico para grupo
            console.log('Gerenciar grupo do produto:', productId);
        }
    },

    // Namespace para gerenciamento de opções do grupo
    groupOptions: {
        // Carregar opções
        carregarOpcoes: function(productId) {
            $.ajax({
                url: `/ProductGroup/Options/${productId}`,
                type: 'GET',
                success: (data) => {
                    $('#groupOptionsContainer').html(data);
                },
                error: (xhr) => {
                    console.error('Erro ao carregar opções:', xhr);
                    $('#groupOptionsContainer').html('<div class="alert alert-danger">Erro ao carregar opções</div>');
                }
            });
        }
    },

    // Namespace para gerenciamento de regras de troca
    exchangeRules: {
        // Carregar regras de troca
        carregarRegras: function(productId) {
            $.ajax({
                url: `/ProductGroup/ExchangeRules/${productId}`,
                type: 'GET',
                success: (data) => {
                    $('#exchangeRulesContainer').html(data);
                },
                error: (xhr) => {
                    console.error('Erro ao carregar regras:', xhr);
                    $('#exchangeRulesContainer').html('<div class="alert alert-danger">Erro ao carregar regras de troca</div>');
                }
            });
        }
    }
};

// Função utilitária para debounce
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    productManager.init();
    console.log('ProductManager inicializado:', productManager);
    console.log('Função novoProduct disponível:', typeof productManager.novoProduct);
}); 