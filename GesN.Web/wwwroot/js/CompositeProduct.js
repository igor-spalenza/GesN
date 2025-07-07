// CompositeProduct and ProductComponent Management JavaScript
const compositeProductManager = {
    // Initialize composite product functionality
    init: function() {
        this.bindEvents();
        this.initializeComponentsList();
    },

    // Bind global events
    bindEvents: function() {
        // Bind events for component management
        $(document).off('click.compositeProduct', '#btnAddComponent').on('click.compositeProduct', '#btnAddComponent', () => {
            const productId = $('.product-edit-container').data('product-id');
            if (productId) {
                this.components.adicionarComponente(productId);
            }
        });

        $(document).off('click.compositeProduct', '#btnAddFirstComponent').on('click.compositeProduct', '#btnAddFirstComponent', () => {
            const productId = $('.product-edit-container').data('product-id');
            if (productId) {
                this.components.adicionarComponente(productId);
            }
        });

        $(document).off('click.compositeProduct', '#btnRefreshComponents').on('click.compositeProduct', '#btnRefreshComponents', () => {
            const productId = $('.product-edit-container').data('product-id');
            if (productId) {
                this.components.carregarComponentes(productId);
            }
        });

        // Global functions for onclick events in views
        window.editarComponente = (componentId) => {
            this.components.editarComponente(componentId);
        };

        window.removerComponente = (componentId) => {
            this.components.removerComponente(componentId);
        };
    },

    // Initialize components list (DataTable, etc.)
    initializeComponentsList: function() {
        // Initialize DataTable if components table exists
        const componentsTable = $('#componentsTable');
        
        if (componentsTable.length > 0) {
            // Verificar se DataTable já foi inicializado
            if ($.fn.DataTable.isDataTable('#componentsTable')) {
                componentsTable.DataTable().destroy();
            }
            
            componentsTable.DataTable({
                responsive: true,
                pageLength: 25,
                order: [[0, 'asc']], // Order by assembly order
                columnDefs: [
                    { targets: [-1], orderable: false }, // Actions column not orderable
                    { targets: [5], className: 'text-end' } // Cost column right-aligned
                ]
            });
        }

        // Initialize any other components-related functionality
        this.initializeComponentsStatistics();
    },

    // Initialize components statistics
    initializeComponentsStatistics: function() {
        // Update statistics if needed
        this.statistics.loadCompositeStatistics();
    },

    // Initialize components index page
    initializeComponentsIndex: function() {
        $('#componentsTable').DataTable({
            "language": {
                "url": "/lib/datatables.net/pt-br.json"
            },
            "order": [[0, "asc"]],
            "pageLength": 25
        });
    },

    // Configuration
    config: {
        baseUrl: '/Product',
        componentUrl: '/ProductComponent',
        modalId: '#productModal'
    },

    // Statistics methods
    statistics: {
        // Update composite products statistics
        updateCompositeProductStats: function(data) {
            if (data && data.compositeProducts !== undefined) {
                $('#produtos-compostos').text(data.compositeProducts);
            }
        },

        // Load statistics specific to composite products
        loadCompositeStatistics: function() {
            $.ajax({
                url: '/Product/ProductStatistics',
                type: 'GET',
                success: (data) => {
                    this.updateCompositeProductStats(data);
                },
                error: (xhr) => {
                    console.error('Erro ao carregar estatísticas de produtos compostos:', xhr);
                }
            });
        }
    },

    // ProductComponent Management
    components: {
        // Carregar componentes
        carregarComponentes: function(productId) {
            $.ajax({
                url: `/ProductComponent/List/${productId}`,
                type: 'GET',
                success: (data) => {
                    $('#componentsContainer').html(data);
                    
                    // Reinicializar DataTable após carregar novos componentes
                    setTimeout(() => {
                        this.initializeComponentsList();
                    }, 100);
                },
                error: (xhr) => {
                    $('#componentsContainer').html('<div class="alert alert-danger">Erro ao carregar componentes</div>');
                }
            });
        },

        // Abrir gerenciamento de componentes
        abrirGerenciamento: function(productId) {
            $.ajax({
                url: `/ProductComponent/GerenciarComponentes/${productId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="componentManagementModal" tabindex="-1">
                            <div class="modal-dialog modal-xl">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">
                                            <i class="fas fa-cogs"></i> Gerenciar Componentes do Produto
                                        </h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                            <i class="fas fa-times"></i> Fechar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#componentManagementModal').modal('show');
                    
                    // Clean up when modal is closed
                    $('#componentManagementModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    toastr.error('Erro ao abrir gerenciamento de componentes');
                }
            });
        },

        // Adicionar componente
        adicionarComponente: function(productId) {
            $.ajax({
                url: `/ProductComponent/FormularioComponente/${productId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="createComponentModal" tabindex="-1">
                            <div class="modal-dialog modal-lg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">
                                            <i class="fas fa-plus"></i> Adicionar Componente
                                        </h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                            Cancelar
                                        </button>
                                        <button type="button" class="btn btn-primary" onclick="compositeProductManager.components.salvarComponente()">
                                            <i class="fas fa-save"></i> Salvar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#createComponentModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#createComponentModal').on('shown.bs.modal', function () {
                        compositeProductManager.components.initializeForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#createComponentModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    toastr.error('Erro ao abrir formulário de componente');
                }
            });
        },

        // Editar componente
        editarComponente: function(componentId) {
            $.ajax({
                url: `/ProductComponent/FormularioEdicaoComponente/${componentId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="editComponentModal" tabindex="-1">
                            <div class="modal-dialog modal-lg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">
                                            <i class="fas fa-edit"></i> Editar Componente
                                        </h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                            Cancelar
                                        </button>
                                        <button type="button" class="btn btn-primary" onclick="compositeProductManager.components.salvarEdicaoComponente()">
                                            <i class="fas fa-save"></i> Salvar Alterações
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#editComponentModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#editComponentModal').on('shown.bs.modal', function () {
                        compositeProductManager.components.initializeForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#editComponentModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    toastr.error('Erro ao abrir formulário de edição');
                }
            });
        },

        // Salvar componente
        salvarComponente: function() {
            const form = $('#formCreateComponent')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            const formData = new FormData(form);

            // Disable submit button
            const submitBtn = $('#createComponentModal .btn-primary');
            const originalText = submitBtn.text();
            submitBtn.prop('disabled', true).text('Salvando...');

            $.ajax({
                url: '/ProductComponent/SalvarComponente',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#createComponentModal').modal('hide');
                        
                        // Recarregar lista de componentes
                        const productId = $('#formCreateComponent input[name="CompositeProductId"]').val();
                        if (productId) {
                            compositeProductManager.components.carregarComponentes(productId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao salvar componente');
                        // Show validation errors if present
                        if (response.errors) {
                            compositeProductManager.utils.showValidationErrors(response.errors, 'createComponentModal');
                        }
                    }
                },
                error: function(xhr) {
                    const errorMsg = xhr.responseJSON?.message || 'Erro ao salvar componente';
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).text(originalText);
                }
            });
        },

        // Salvar edição de componente
        salvarEdicaoComponente: function() {
            const form = $('#formEditComponent')[0];
            const formData = new FormData(form);
            const componentId = $('#formEditComponent input[name="Id"]').val();

            $.ajax({
                url: `/ProductComponent/SalvarEdicaoComponente/${componentId}`,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#editComponentModal').modal('hide');
                        
                        // Recarregar lista de componentes
                        const productId = $('#formEditComponent input[name="CompositeProductId"]').val();
                        if (productId) {
                            compositeProductManager.components.carregarComponentes(productId);
                        }
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function(xhr) {
                    toastr.error('Erro ao salvar alterações do componente');
                }
            });
        },

        // Confirmar remoção de componente
        confirmarRemocao: function(componentId, componentName) {
            if (confirm(`Tem certeza que deseja remover o componente "${componentName}"?`)) {
                this.removerComponente(componentId);
            }
        },

        // Remover componente
        removerComponente: function(componentId) {
            if (confirm('Tem certeza que deseja remover este componente?')) {
                $.ajax({
                    url: `/ProductComponent/RemoverComponente/${componentId}`,
                    type: 'DELETE',
                    success: function(response) {
                        if (response.success) {
                            toastr.success(response.message || 'Componente removido com sucesso');
                            
                            // Atualizar lista de componentes
                            const productId = $('.product-edit-container').data('product-id');
                            if (productId) {
                                compositeProductManager.components.carregarComponentes(productId);
                            }
                            
                            // Atualizar estatísticas
                            compositeProductManager.statistics.loadCompositeStatistics();
                        } else {
                            toastr.error(response.message || 'Erro ao remover componente');
                        }
                    },
                    error: function(xhr) {
                        toastr.error('Erro ao remover componente');
                    }
                });
            }
        },

        // Calcular total de componentes
        calcularTotalComponentes: function() {
            let total = 0;
            $('.component-quantity').each(function() {
                const quantity = parseFloat($(this).val()) || 0;
                const price = parseFloat($(this).data('price')) || 0;
                total += quantity * price;
            });
            
            $('.component-total').text('R$ ' + total.toFixed(2));
            return total;
        },

        // Inicializar formulário de componente
        initializeForm: function() {
            // Initialize Select2 for component products
            $('.select2-component').select2({
                placeholder: 'Selecione um produto',
                allowClear: true,
                ajax: {
                    url: '/Product/BuscaProductAutocomplete',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            termo: params.term,
                            page: params.page,
                            // Filtrar apenas produtos simples para evitar recursão
                            tipoProduct: 'Simple'
                        };
                    },
                    processResults: function (data, params) {
                        return {
                            results: data.map(function(item) {
                                return {
                                    id: item.id,
                                    text: item.name + (item.sku ? ' - ' + item.sku : ''),
                                    data: item
                                };
                            })
                        };
                    },
                    cache: true
                }
            });

            // Calcular valores automaticamente
            $('.component-quantity, .component-price').on('input change', function() {
                compositeProductManager.components.calcularTotalComponentes();
            });

            // Validar quantidade mínima
            $('.component-quantity').on('change', function() {
                const quantity = parseFloat($(this).val()) || 0;
                if (quantity <= 0) {
                    toastr.warning('A quantidade deve ser maior que zero');
                    $(this).focus();
                }
            });
        },

        // Inicializar formulário de criação de componente
        initializeCreateForm: function() {
            // Calcular custo quando produto ou quantidade mudar
            $('#ComponentProductId, #Quantity').on('change', function() {
                compositeProductManager.components.calculateComponentCost();
            });
        },

        // Inicializar formulário de edição de componente
        initializeEditForm: function() {
            // Calcular custo quando produto ou quantidade mudar
            $('#ComponentProductId, #Quantity').on('change', function() {
                compositeProductManager.components.calculateComponentCost();
            });
        },

        // Calcular custo do componente
        calculateComponentCost: function() {
            const productId = $('#ComponentProductId').val();
            const quantity = parseFloat($('#Quantity').val()) || 0;
            
            if (productId && quantity > 0) {
                $.ajax({
                    url: '/Product/GetProductCost/' + productId,
                    type: 'GET',
                    success: function(data) {
                        if (data.success) {
                            const unitCost = data.cost || 0;
                            const totalCost = unitCost * quantity;
                            
                            $('#unitCost').text('R$ ' + unitCost.toFixed(2));
                            $('#componentQuantity').text(quantity);
                            $('#totalCost').text('R$ ' + totalCost.toFixed(2));
                            $('#componentCostInfo').removeClass('d-none');
                        }
                    },
                    error: function() {
                        $('#componentCostInfo').addClass('d-none');
                    }
                });
            } else {
                $('#componentCostInfo').addClass('d-none');
            }
        },

        // Inicializar DataTable de componentes
        initializeComponentsTable: function() {
            // Inicializar DataTable se disponível
            if ($.fn.DataTable && $('#componentsTable').length > 0) {
                $('#componentsTable').DataTable({
                    "language": {
                        "url": "//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json"
                    },
                    "responsive": true,
                    "pageLength": 10,
                    "order": [[0, "asc"]], // Ordenar por ordem de montagem
                    "columnDefs": [
                        { "orderable": false, "targets": [7] } // Não ordenar coluna de ações
                    ]
                });
            }
        }
    },

    // Utility methods
    utils: {
        // Show validation errors
        showValidationErrors: function(errors, modalId) {
            const $modal = $(`#${modalId}`);
            
            // Clear previous errors
            $modal.find('.text-danger').text('');
            $modal.find('.is-invalid').removeClass('is-invalid');
            
            // Show new errors
            Object.keys(errors).forEach(field => {
                const $field = $modal.find(`[name="${field}"]`);
                const errorText = Array.isArray(errors[field]) ? errors[field].join(', ') : errors[field];
                
                if ($field.length) {
                    $field.addClass('is-invalid');
                    
                    let $errorContainer = $field.siblings('.text-danger');
                    if ($errorContainer.length === 0) {
                        $errorContainer = $('<span class="text-danger"></span>');
                        $field.parent().append($errorContainer);
                    }
                    
                    $errorContainer.text(errorText);
                }
            });
        }
    }
};

// Global aliases for backward compatibility
window.CompositeProduct = {
    // Statistics
    updateCompositeProductStats: (data) => compositeProductManager.statistics.updateCompositeProductStats(data),
    loadCompositeStatistics: () => compositeProductManager.statistics.loadCompositeStatistics(),
    
    // Components
    carregarComponentes: (id) => compositeProductManager.components.carregarComponentes(id),
    abrirGerenciamento: (id) => compositeProductManager.components.abrirGerenciamento(id),
    adicionarComponente: (id) => compositeProductManager.components.adicionarComponente(id),
    editarComponente: (id) => compositeProductManager.components.editarComponente(id),
    salvarComponente: () => compositeProductManager.components.salvarComponente(),
    salvarEdicaoComponente: () => compositeProductManager.components.salvarEdicaoComponente(),
    confirmarRemocao: (id, name) => compositeProductManager.components.confirmarRemocao(id, name),
    removerComponente: (id) => compositeProductManager.components.removerComponente(id),
    calcularTotalComponentes: () => compositeProductManager.components.calcularTotalComponentes(),
    initializeForm: () => compositeProductManager.components.initializeForm(),
    initializeCreateForm: () => compositeProductManager.components.initializeCreateForm(),
    initializeEditForm: () => compositeProductManager.components.initializeEditForm(),
    calculateComponentCost: () => compositeProductManager.components.calculateComponentCost(),
    initializeComponentsList: () => compositeProductManager.components.initializeComponentsList(),
    initializeComponentsTable: () => compositeProductManager.components.initializeComponentsTable()
};

// Global functions for onclick events in views
window.editarComponente = function(componentId) {
    compositeProductManager.components.editarComponente(componentId);
};

window.removerComponente = function(componentId) {
    compositeProductManager.components.removerComponente(componentId);
};

window.initializeComponentsTable = function() {
    compositeProductManager.components.initializeComponentsTable();
};

// Export for use in Product.js (for productManager.components compatibility)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = compositeProductManager;
}

// Auto-inicialização quando o DOM estiver pronto
$(function() {
    // Auto-detectar e inicializar tabela de componentes (página de índice)
    if ($('#componentsTable').length > 0) {
        compositeProductManager.components.initializeComponentsTable();
    }
    
    // Auto-detectar se há produtos compostos na página de edição
    if ($('.product-edit-container[data-is-composite="true"]').length > 0) {
        compositeProductManager.init();
    }
    
    // Detectar quando produtos compostos são adicionados dinamicamente
    if (typeof MutationObserver !== 'undefined') {
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeType === 1) { // Element node
                        const $node = $(node);
                        // Verificar se é um produto composto
                        if ($node.hasClass('product-edit-container') && $node.data('is-composite') === true) {
                            compositeProductManager.init();
                        }
                        // Verificar se contém um produto composto
                        else if ($node.find('.product-edit-container[data-is-composite="true"]').length > 0) {
                            compositeProductManager.init();
                        }
                        // Verificar se é uma tabela de componentes
                        else if ($node.find('#componentsTable').length > 0) {
                            setTimeout(() => {
                                compositeProductManager.initializeComponentsList();
                            }, 100);
                        }
                    }
                });
            });
        });
        
        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }
}); 