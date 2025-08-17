// CompositeProduct and ProductComponent Management JavaScript
// Proteção contra redeclaração quando script é carregado dinamicamente
if (typeof window.compositeProductManager !== 'undefined') {
    console.warn('CompositeProduct.js já foi carregado, pulando redeclaração');
} else {

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

        // Bind tab events for dynamic content loading
        this.bindTabEvents();
    },

    // Bind tab events
    bindTabEvents: function() {
        // Event listeners para as abas (hierarquias e demandas) - usando delegação de eventos
        $(document).off('shown.bs.tab', '[id^="hierarchies-tab-"]').on('shown.bs.tab', '[id^="hierarchies-tab-"]', (e) => {
            // Extrair o productId do ID do botão da aba (formato: hierarchies-tab-{productId})
            const buttonId = $(e.target).attr('id');
            const productId = buttonId.replace('hierarchies-tab-', '');
            
            if (productId) {
                this.hierarchies.loadHierarchies(productId);
            }
        });

        $(document).off('shown.bs.tab', '[id^="demands-tab-"]').on('shown.bs.tab', '[id^="demands-tab-"]', (e) => {
            // Extrair o productId do ID do botão da aba (formato: demands-tab-{productId})
            const buttonId = $(e.target).attr('id');
            const productId = buttonId.replace('demands-tab-', '');
            
            if (productId) {
                this.demands.loadDemands(productId);
            }
        });
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

    // Statistics methods (removed - endpoint não existe)
    statistics: {
        // Statistics functionality removed due to non-existent endpoint
    },

    // ProductComponent Management
    components: {
        // Carregar componentes integrado
        carregarComponentes: function(productId) {
            
            if (!productId) {
                console.error('ProductId não fornecido para carregar componentes');
                return;
            }

            // Show loading indicator
            const container = $('#componentsContainer');
            if (container.length === 0) {
                console.warn('Container #componentsContainer não encontrado');
                return;
            }

            const originalContent = container.html();
            container.html('<div class="text-center p-4"><i class="fas fa-spinner fa-spin"></i> Carregando...</div>');

            $.ajax({
                url: `/ProductComponent/ProductComponents/${productId}`,
                type: 'GET',
                success: (data) => {
                    container.html(data);
                },
                error: (xhr) => {
                    console.error('Erro ao carregar componentes:', xhr);
                    container.html(originalContent);
                    toastr.error('Erro ao carregar componentes');
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

        // Show create component modal integrado
        showCreateComponentModal: function(productId) {

            $.ajax({
                url: `/ProductComponent/FormularioComponente/${productId}`,
                type: 'GET',
                success: function(data) {
                    // Remove modal if it exists
                    if ($('#createComponentModal').length > 0) {
                        $('#createComponentModal').remove();
                    }
                    
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
                                        <button type="button" class="btn btn-primary" id="btnSaveComponent">
                                            <i class="fas fa-save"></i> Salvar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    
                    // Initialize form after modal is shown
                    $('#createComponentModal').on('shown.bs.modal', function () {
                
                        
                        // Ensure CompositeProductId is set correctly
                        const compositeProductIdField = $('#CompositeProductId');
                        if (compositeProductIdField.length) {
                            if (!compositeProductIdField.val() || compositeProductIdField.val() === '') {
                                compositeProductIdField.val(productId);
                    
                            }
                        }
                        
                        compositeProductManager.components.initializeForm();
                        
                        // Clear any previous validation errors
                        $(this).find('.text-danger').text('');
                        $(this).find('.is-invalid').removeClass('is-invalid');
                    });
                    
                    // Handle save button click
                    $('#createComponentModal').off('click', '#btnSaveComponent').on('click', '#btnSaveComponent', function() {
                        compositeProductManager.components.save();
                    });
                    
                    // Clean up when modal is closed
                    $('#createComponentModal').on('hidden.bs.modal', function () {
                        $(this).find('form')[0]?.reset();
                        $(this).remove();
                    });
                    
                    // Show modal
                    $('#createComponentModal').modal('show');
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de criação:', xhr);
                    toastr.error('Erro ao abrir formulário de componente');
                }
            });
        },

        // Manter compatibilidade com nome antigo
        adicionarComponente: function(productId) {
            this.showCreateComponentModal(productId);
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

        // Save component integrado
        save: function() {
            const form = $('#formCreateComponent')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            const formData = new FormData(form);


            
            // Validate required fields before sending
            const compositeProductId = formData.get('CompositeProductId');
            const componentProductId = formData.get('ComponentProductId');
            const quantity = formData.get('Quantity');
            
            
            
            if (!compositeProductId || compositeProductId.trim() === '') {
                toastr.error('ID do produto composto não foi definido. Recarregue a página e tente novamente.');
                return;
            }

            // Disable submit button
            const submitBtn = $('#btnSaveComponent');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

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
                        if (compositeProductId) {
                            compositeProductManager.components.carregarComponentes(compositeProductId);
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
                    console.error('Erro ao salvar componente:', xhr);
                    let errorMsg = 'Erro ao salvar componente';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        compositeProductManager.utils.showValidationErrors(xhr.responseJSON.errors, 'createComponentModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            });
        },

        // Manter compatibilidade com nome antigo
        salvarComponente: function() {
            this.save();
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

    // ProductComponentHierarchy Management
    hierarchies: {
        // ✅ Templates HTML para geração dinâmica de conteúdo
        templates: {
            // Template para linha individual da tabela
            rowTemplate: function(data) {
                const maxQtyDisplay = data.maxQuantity === 0 ? 'Ilimitado' : data.maxQuantity;
                const optionalBadge = data.isOptional ? 'bg-secondary' : 'bg-primary';
                const optionalText = data.isOptional ? 'Opcional' : 'Obrigatório';
                
                return `
                    <tr data-relation-id="${data.id}" class="hierarchy-row">
                        <td class="hierarchy-name">${this.escapeHtml(data.hierarchyDescription || data.hierarchyName || '')}</td>
                        <td class="text-center min-qty">${data.minQuantity}</td>
                        <td class="text-center max-qty">${maxQtyDisplay}</td>
                        <td class="text-center assembly-order">${data.assemblyOrder}</td>
                        <td class="text-center optional-status">
                            <span class="badge ${optionalBadge}">
                                ${optionalText}
                            </span>
                        </td>
                        <td class="text-center actions">
                            ${this.actionButtonsTemplate(data.id, data.hierarchyDescription || data.hierarchyName || '')}
                        </td>
                    </tr>
                `;
            },

            // Template para botões de ação
            actionButtonsTemplate: function(relationId, hierarchyName) {
                const escapedName = this.escapeHtml(hierarchyName);
                return `
                    <div class="btn-group btn-group-sm" role="group">
                        <button type="button" class="btn btn-outline-info btn-sm" 
                                onclick="compositeProductManager.hierarchies.showRelationDetailsModal(${relationId})"
                                title="Ver detalhes">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button type="button" class="btn btn-outline-warning btn-sm"
                                onclick="compositeProductManager.hierarchies.showEditRelationModal(${relationId})"
                                title="Editar">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button type="button" class="btn btn-outline-danger btn-sm"
                                onclick="compositeProductManager.hierarchies.confirmRemoveRelation(${relationId}, '${escapedName}')"
                                title="Remover">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                `;
            },

            // Template para estado vazio (quando não há registros)
            emptyStateTemplate: function() {
                return `
                    <tr class="no-data-row">
                        <td colspan="6" class="text-center text-muted py-4">
                            <i class="fas fa-inbox fa-2x mb-2"></i><br/>
                            <span>Nenhuma hierarquia associada a este produto.</span><br/>
                            <small class="text-muted">Clique em "Nova Hierarquia" para começar.</small>
                        </td>
                    </tr>
                `;
            },

            // Template para loading state
            loadingTemplate: function(message = 'Carregando hierarquias...') {
                return `
                    <div class="text-center p-4">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Carregando...</span>
                        </div>
                        <div class="mt-2 text-muted">${this.escapeHtml(message)}</div>
                    </div>
                `;
            },

            // Template para atualizar uma linha existente
            updateRowContent: function(data, $existingRow) {
                const maxQtyDisplay = data.maxQuantity === 0 ? 'Ilimitado' : data.maxQuantity;
                const optionalBadge = data.isOptional ? 'bg-secondary' : 'bg-primary';
                const optionalText = data.isOptional ? 'Opcional' : 'Obrigatório';
                
                // Atualizar células individuais
                $existingRow.find('.hierarchy-name').text(data.hierarchyDescription || data.hierarchyName || '');
                $existingRow.find('.min-qty').text(data.minQuantity);
                $existingRow.find('.max-qty').text(maxQtyDisplay);
                $existingRow.find('.assembly-order').text(data.assemblyOrder);
                
                // Atualizar badge de status opcional
                const $statusBadge = $existingRow.find('.optional-status .badge');
                $statusBadge.removeClass('bg-secondary bg-primary').addClass(optionalBadge).text(optionalText);
                
                // Atualizar botões de ação
                $existingRow.find('.actions').html(this.actionButtonsTemplate(data.id, data.hierarchyDescription || data.hierarchyName || ''));
            },

            // Escape HTML para segurança
            escapeHtml: function(text) {
                if (!text) return '';
                const div = document.createElement('div');
                div.textContent = text;
                return div.innerHTML;
            }
        },

        // ✅ Utilitários para manipulação de DOM
        utils: {
            // Encontrar container da tabela para um produto específico
            getTableContainer: function(productId) {
                return $(`#hierarchiesContainer-${productId} table tbody`);
            },

            // Verificar se existe linha de "sem dados"
            hasEmptyState: function(productId) {
                return $(`#hierarchiesContainer-${productId} .no-data-row`).length > 0;
            },

            // Remover linha de "sem dados"
            removeEmptyState: function(productId) {
                $(`#hierarchiesContainer-${productId} .no-data-row`).remove();
            },

            // Adicionar linha de "sem dados" se necessário
            addEmptyStateIfNeeded: function(productId) {
                const $tableBody = this.getTableContainer(productId);
                if ($tableBody.find('tr').length === 0) {
                    $tableBody.append(compositeProductManager.hierarchies.templates.emptyStateTemplate());
                }
            },

            // Contar total de registros
            getTotalCount: function(productId) {
                const $tableBody = this.getTableContainer(productId);
                return $tableBody.find('tr.hierarchy-row').length;
            },

            // Atualizar contador visual (se existir)
            updateCounter: function(productId, delta = 0) {
                const $counter = $(`#hierarchies-count-${productId}`);
                if ($counter.length) {
                    const current = parseInt($counter.text()) || 0;
                    $counter.text(Math.max(0, current + delta));
                }
            }
        },

        // Carregar hierarquias integrado (fallback/manual refresh)
        carregarHierarquias: function(productId) {

            
            if (!productId) {
                console.error('ProductId não fornecido para carregar hierarquias');
                return;
            }

            // Encontrar container usando ID dinâmico
            const container = $(`#hierarchiesContainer-${productId}`);
            if (container.length === 0) {
                console.warn(`Container #hierarchiesContainer-${productId} não encontrado`);
                return;
            }

            // Salvar conteúdo original para rollback em caso de erro
            const originalContent = container.html();
            
            // ✅ Usar template de loading
            container.html(this.templates.loadingTemplate('Recarregando hierarquias...'));

            $.ajax({
                url: `/ProductComponentHierarchy/ProductHierarchies/${productId}`,
                type: 'GET',
                timeout: 10000, // 10 segundos timeout
                success: (data) => {
                    container.html(data);
    
                },
                error: (xhr) => {
                    console.error('❌ Erro ao recarregar hierarquias via fallback:', xhr);
                    
                    // Rollback para conteúdo original
                    container.html(originalContent);
                    
                    let errorMessage = 'Erro ao carregar hierarquias';
                    if (xhr.status === 404) {
                        errorMessage = 'Produto não encontrado';
                    } else if (xhr.status === 500) {
                        errorMessage = 'Erro interno do servidor';
                    } else if (xhr.statusText === 'timeout') {
                        errorMessage = 'Tempo esgotado - tente novamente';
                    }
                    
                    toastr.error(errorMessage);
                }
            });
        },

        // ✅ FUNÇÃO PRINCIPAL: Atualização granular DOM com fallback
        updateHierarchyDisplay: function(productId, operation, data, relationId = null) {

            
            try {
                switch(operation) {
                    case 'CREATE':
                        return this.addHierarchyRowOptimized(productId, data);
                    case 'UPDATE':
                        return this.updateHierarchyRowOptimized(productId, data);
                    case 'DELETE':
                        return this.removeHierarchyRowOptimized(productId, relationId);
                    default:
                        console.warn('Operação não reconhecida:', operation);
                        return false;
                }
            } catch (error) {
                console.warn('⚠️ Atualização granular falhou, usando fallback:', error);
                
                // Fallback: recarregamento completo
                this.carregarHierarquias(productId);
                return true; // Considera sucesso via fallback
            }
        },

        // ✅ Adicionar nova linha (CREATE)
        addHierarchyRowOptimized: function(productId, data) {
            if (!data || !data.id) {
                throw new Error('Dados inválidos para criação de linha');
            }

            const $tableBody = this.utils.getTableContainer(productId);
            if ($tableBody.length === 0) {
                throw new Error(`Tabela não encontrada para produto ${productId}`);
            }

            // Remover estado vazio se existir
            this.utils.removeEmptyState(productId);

            // Gerar HTML da nova linha
            const newRowHtml = this.templates.rowTemplate(data);
            const $newRow = $(newRowHtml);

            // Adicionar na tabela
            $tableBody.append($newRow);

            // Animação suave
            $newRow.hide().fadeIn(500);

            // Atualizar contador
            this.utils.updateCounter(productId, +1);

            
            return true;
        },

        // ✅ Atualizar linha existente (UPDATE)  
        updateHierarchyRowOptimized: function(productId, data) {
            if (!data || !data.id) {
                throw new Error('Dados inválidos para atualização de linha');
            }

            const $existingRow = $(`#hierarchiesContainer-${productId} tr[data-relation-id="${data.id}"]`);
            if ($existingRow.length === 0) {
                throw new Error(`Linha não encontrada para relação ${data.id}`);
            }

            // Usar template para atualizar conteúdo
            this.templates.updateRowContent(data, $existingRow);

            // Efeito visual de atualização
            $existingRow.addClass('table-warning');
            setTimeout(() => {
                $existingRow.removeClass('table-warning');
            }, 2000);

            
            return true;
        },

        // ✅ Remover linha existente (DELETE)
        removeHierarchyRowOptimized: function(productId, relationId) {
            if (!relationId) {
                throw new Error('ID da relação não fornecido para remoção');
            }

            const $rowToDelete = $(`#hierarchiesContainer-${productId} tr[data-relation-id="${relationId}"]`);
            if ($rowToDelete.length === 0) {
                throw new Error(`Linha não encontrada para relação ${relationId}`);
            }

            // Animação de remoção
            $rowToDelete.addClass('table-danger').fadeOut(500, () => {
                $rowToDelete.remove();
                
                // Verificar se precisa adicionar estado vazio
                this.utils.addEmptyStateIfNeeded(productId);
            });

            // Atualizar contador
            this.utils.updateCounter(productId, -1);

            
            return true;
        },

        // Carregar hierarquias (método alternativo para compatibilidade)
        loadHierarchies: function(productId) {
            $('#hierarchies-grid').load(`/ProductComponentHierarchy/GetByProduct/${productId}`);
        },

        // Mostrar modal de criação de hierarquia (método alternativo para compatibilidade)
        showCreateModal: function(productId) {
            window.location.href = `/ProductComponentHierarchy/Create?productId=${productId}`;
        },

        // Show create CompositeProductXHierarchy modal
        showCreateHierarchyModal: function(productId) {
            // Limpar espaços e verificar se o productId é válido
            productId = (productId || '').toString().trim();
            
            if (!productId || productId === 'undefined' || productId === 'null') {
                toastr.error('ID do produto não foi informado ou é inválido');
                return;
            }

            $.ajax({
                url: `/ProductComponentHierarchy/FormularioCompositeProductXHierarchy/${productId}`,
                type: 'GET',
                success: function(data, textStatus, xhr) {
                    // Check if response is JSON error
                    if (xhr.getResponseHeader('Content-Type')?.includes('application/json')) {
                        try {
                            const jsonResponse = typeof data === 'string' ? JSON.parse(data) : data;
                            if (jsonResponse.success === false) {
                                toastr.error(jsonResponse.message || 'Erro ao carregar formulário de hierarquia');
                                return;
                            }
                        } catch (parseError) {
                            // Continue with normal processing if JSON parse fails
                        }
                    }

                    // Remove modal if it exists
                    if ($('#createCompositeProductXHierarchyModal').length > 0) {
                        $('#createCompositeProductXHierarchyModal').remove();
                    }
                    
                    $('body').append(data);
                    
                    // Check if modal was actually created
                    if ($('#createCompositeProductXHierarchyModal').length === 0) {
                        toastr.error('Erro ao criar formulário de hierarquia');
                        return;
                    }
                    
                    // Initialize modal
                    const modal = new bootstrap.Modal(document.getElementById('createCompositeProductXHierarchyModal'));
                    modal.show();
                    
                    // Remove any existing event listeners before adding new ones
                    $('#createCompositeProductXHierarchyModal').off('shown.bs.modal').on('shown.bs.modal', function() {
                        compositeProductManager.hierarchies.initializeCreateCompositeProductXHierarchyForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#createCompositeProductXHierarchyModal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                        $(this).find('form')[0]?.reset();
                        $(this).remove();
                    });
                    
                                         // Modal is already shown by Bootstrap Modal constructor above
                },
                error: function(xhr) {
                    let errorMsg = 'Erro ao abrir formulário de hierarquia';
                    
                    // Try to extract error message from JSON response
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseText) {
                        try {
                            const errorResponse = JSON.parse(xhr.responseText);
                            if (errorResponse.message) {
                                errorMsg = errorResponse.message;
                            }
                        } catch (parseError) {
                            // Use default error message
                        }
                    }
                    
                    toastr.error(errorMsg);
                }
            });
        },

        // Show assign hierarchy modal integrado
        showAssignHierarchyModal: function(productId) {
    

            $.ajax({
                url: `/ProductComponentHierarchy/FormularioAssociarHierarquia/${productId}`,
                type: 'GET',
                success: function(data) {
                    // Remove modal if it exists
                    if ($('#assignHierarchyModal').length > 0) {
                        $('#assignHierarchyModal').remove();
                    }
                    
                    const modalHtml = `
                        <div class="modal fade" id="assignHierarchyModal" tabindex="-1">
                            <div class="modal-dialog modal-lg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">
                                            <i class="fas fa-link"></i> Associar Hierarquia Existente
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
                                        <button type="button" class="btn btn-primary" id="btnAssignHierarchy">
                                            <i class="fas fa-link"></i> Associar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    
                    // Handle assign button click
                    $('#assignHierarchyModal').off('click', '#btnAssignHierarchy').on('click', '#btnAssignHierarchy', function() {
                        compositeProductManager.hierarchies.assignHierarchy();
                    });
                    
                    // Clean up when modal is closed
                    $('#assignHierarchyModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                    
                    // Show modal
                    $('#assignHierarchyModal').modal('show');
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de associação de hierarquia:', xhr);
                    toastr.error('Erro ao abrir formulário de associação');
                }
            });
        },

        // Show hierarchy details modal
        showHierarchyDetailsModal: function(hierarchyId, productId = null) {
    

            const url = productId ? 
                `/ProductComponentHierarchy/DetalhesHierarquia/${hierarchyId}?productId=${productId}` :
                `/ProductComponentHierarchy/DetalhesHierarquia/${hierarchyId}`;

            $.ajax({
                url: url,
                type: 'GET',
                success: function(data) {
                    // Remove modal if it exists
                    if ($('#hierarchyDetailsModal').length > 0) {
                        $('#hierarchyDetailsModal').remove();
                    }
                    
                    const modalHtml = `
                        <div class="modal fade" id="hierarchyDetailsModal" tabindex="-1">
                            <div class="modal-dialog modal-xl">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">
                                            <i class="fas fa-sitemap"></i> Detalhes da Hierarquia
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
                    
                    // Clean up when modal is closed
                    $('#hierarchyDetailsModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                    
                    // Show modal
                    $('#hierarchyDetailsModal').modal('show');
                },
                error: function(xhr) {
                    console.error('Erro ao abrir detalhes da hierarquia:', xhr);
                    toastr.error('Erro ao carregar detalhes da hierarquia');
                }
            });
        },

        // Show manage hierarchy components modal
        showManageHierarchyComponentsModal: function(hierarchyId, productId = null) {
    

            const url = productId ? 
                `/ProductComponentHierarchy/GerenciarComponentesHierarquia/${hierarchyId}?productId=${productId}` :
                `/ProductComponentHierarchy/GerenciarComponentesHierarquia/${hierarchyId}`;

            $.ajax({
                url: url,
                type: 'GET',
                success: function(data) {
                    // Remove modal if it exists
                    if ($('#manageHierarchyComponentsModal').length > 0) {
                        $('#manageHierarchyComponentsModal').remove();
                    }
                    
                    const modalHtml = `
                        <div class="modal fade" id="manageHierarchyComponentsModal" tabindex="-1">
                            <div class="modal-dialog modal-xl">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">
                                            <i class="fas fa-cogs"></i> Gerenciar Componentes da Hierarquia
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
                    
                    // Clean up when modal is closed
                    $('#manageHierarchyComponentsModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                    
                    // Show modal
                    $('#manageHierarchyComponentsModal').modal('show');
                },
                error: function(xhr) {
                    console.error('Erro ao abrir gerenciamento de componentes:', xhr);
                    toastr.error('Erro ao carregar gerenciamento de componentes');
                }
            });
        },

        // Save hierarchy integrado
        saveHierarchy: function() {
            const form = $('#formCreateHierarchy')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            const formData = new FormData(form);

            // Disable submit button
            const submitBtn = $('#btnSaveHierarchy');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

            $.ajax({
                url: '/ProductComponentHierarchy/SalvarHierarquia',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#createHierarchyModal').modal('hide');
                        
                        // Recarregar lista de hierarquias
                        const productId = $('.product-edit-container').data('product-id');
                        if (productId) {
                            compositeProductManager.hierarchies.carregarHierarquias(productId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao salvar hierarquia');
                        if (response.errors) {
                            compositeProductManager.utils.showValidationErrors(response.errors, 'createHierarchyModal');
                        }
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar hierarquia:', xhr);
                    let errorMsg = 'Erro ao salvar hierarquia';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        compositeProductManager.utils.showValidationErrors(xhr.responseJSON.errors, 'createHierarchyModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            });
        },

        // Save CompositeProductXHierarchy relation
        saveCompositeProductXHierarchy: function(form) {
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            // Verificar se já está sendo submetido
            const $form = $(form);
            if ($form.data('submitting')) {
    
                return;
            }

            // Marcar como sendo submetido
            $form.data('submitting', true);


            
            const formData = new FormData(form);
            


            // Disable submit button
            const submitBtn = $form.find('button[type="submit"]');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

            $.ajax({
                url: '/ProductComponentHierarchy/SalvarCompositeProductXHierarchy',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#createCompositeProductXHierarchyModal').modal('hide');
                        
                        // ✅ NOVA LÓGICA: Atualização granular com fallback automático
                        const productId = $('.product-edit-container').data('product-id') || $('#ProductId').val();
                        if (productId) {
                            // Tentar atualização granular primeiro
                            if (response.data && response.operation === 'CREATE') {
                                // Usar a nova função de atualização granular
                                compositeProductManager.hierarchies.updateHierarchyDisplay(
                                    productId, 
                                    'CREATE', 
                                    response.data
                                );
                            } else {
                                // Se não tem dados estruturados, usar fallback
                                console.warn('⚠️ Response não contém dados estruturados, usando fallback');
                                compositeProductManager.hierarchies.carregarHierarquias(productId);
                            }
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao criar relação');
                        if (response.errors) {
                            compositeProductManager.utils.showValidationErrors(response.errors, 'createCompositeProductXHierarchyModal');
                        }
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar relação CompositeProductXHierarchy:', xhr);
                    let errorMsg = 'Erro ao criar relação';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        compositeProductManager.utils.showValidationErrors(xhr.responseJSON.errors, 'createCompositeProductXHierarchyModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button e limpar flag
                    submitBtn.prop('disabled', false).html(originalText);
                    $form.removeData('submitting');
                }
            });
        },

        // ✅ REFATORADO: Inicialização robusta seguindo padrão da referência com TRY-CATCH
        initializeHierarchyAutocomplete: function(container) {
            const hierarchyNameField = container.find('#ProductComponentHierarchyName');
            const hierarchyIdField = container.find('#ProductComponentHierarchyId');
            
            if (hierarchyNameField.length === 0) {
                return;
            }

            // Remove instância anterior se houver
            if (hierarchyNameField.data('aaAutocomplete')) {
                hierarchyNameField.autocomplete.destroy();
            }

            try {
                // Get productId from data attribute
                const productId = hierarchyNameField.data('product-id') || '';

                // Inicializar Algolia Autocomplete.js (PADRÃO ROBUSTO)
                const autocompleteInstance = autocomplete(hierarchyNameField[0], {
                    hint: false,
                    debug: false,
                    minLength: 2,
                    openOnFocus: false,
                    autoselect: true,
                    appendTo: container[0] // ✅ CRUCIAL: Container correto para modais
                }, [{
                    source: function(query, callback) {
                        $.ajax({
                            url: '/ProductComponentHierarchy/BuscarHierarchiaDisponivel',
                            type: 'GET',
                            dataType: 'json',
                            data: { 
                                termo: query,
                                productId: productId
                            },
                            success: function(data) {
                                const suggestions = $.map(data, function(item) {
                                    return {
                                        label: item.label || item.name,
                                        value: item.value || item.name,
                                        id: item.id,
                                        description: item.description,
                                        data: item
                                    };
                                });
                                callback(suggestions);
                            },
                            error: function() {
                                callback([]);
                            }
                        });
                    },
                    displayKey: 'label',
                    templates: {
                        suggestion: function(suggestion) {
                            return '<div class="autocomplete-suggestion">' +
                                   '<div class="suggestion-title">' + (suggestion.data.name || suggestion.label) + '</div>' +
                                   (suggestion.data.description ? '<div class="suggestion-subtitle">' + suggestion.data.description + '</div>' : '') +
                                   '</div>';
                        },
                        empty: '<div class="aa-empty">Nenhuma hierarquia encontrada</div>'
                    }
                }]);

                // ✅ EVENT HANDLERS: Seleção robusta (seguindo padrão de referência)
                autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
                    hierarchyIdField.val(suggestion.id);
                    hierarchyNameField.val(suggestion.value);
                    container.find('#displayHierarchyName').text(suggestion.value);
                    
                    // Show selected hierarchy info
                    container.find('#selectedHierarchyInfo').removeClass('d-none');
                    
                    // Trigger validation
                    hierarchyIdField.trigger('change');
                });

                // ✅ VALIDATION: Limpar seleção se campo ficar vazio (seguindo padrão de referência)
                hierarchyNameField.on('blur', function() {
                    if ($(this).val() === '') {
                        hierarchyIdField.val('');
                        container.find('#displayHierarchyName').text('-');
                        container.find('#selectedHierarchyInfo').addClass('d-none');
                        hierarchyIdField.trigger('change');
                    }
                });

            } catch (error) {
                console.error('❌ Erro ao inicializar hierarchy autocomplete:', error);
                
                // ✅ FALLBACK: Mostrar mensagem de erro amigável e permitir input manual
                const $container = hierarchyNameField.closest('.floating-input-group');
                if ($container.length && !$container.find('.alert-warning').length) {
                    const warningHtml = `
                        <div class="alert alert-warning alert-sm mt-2">
                            <i class="fas fa-exclamation-triangle"></i>
                            <small>Autocomplete temporariamente indisponível. Digite o nome da hierarquia manualmente.</small>
                        </div>
                    `;
                    $container.append(warningHtml);
                }
            }
        },

        // Initialize CreateCompositeProductXHierarchy form
        initializeCreateCompositeProductXHierarchyForm: function() {
            const form = $('#createCompositeProductXHierarchyForm');
            
            // Se o formulário não existir, retornar
            if (form.length === 0) {
                console.warn('Formulário createCompositeProductXHierarchyForm não encontrado');
                return;
            }

            // Limpar validações existentes para prevenir duplicação
            if (form.data('validator')) {
                form.removeData('validator');
                form.off('submit.validate');
            }
            
            // Remover TODOS os event listeners do form de forma mais abrangente
            form.off('submit.compositeHierarchy submit');
            $('#MaxQuantity, #MinQuantity, #AssemblyOrder').off('input.customValidation change.customValidation');
            
            // Limpar flag de submitting se existir
            form.removeData('submitting');
            
            // Inicializar autocomplete para hierarquias (padrão robusto)
            this.initializeHierarchyAutocomplete($('#createCompositeProductXHierarchyModal'));

            // Inicializar validação do formulário
            form.validate({
                rules: {
                    ProductComponentHierarchyId: {
                        required: true
                    },
                    MinQuantity: {
                        required: true,
                        min: 1
                    },
                    MaxQuantity: {
                        min: 0
                    },
                    AssemblyOrder: {
                        required: true,
                        min: 1
                    }
                },
                messages: {
                    ProductComponentHierarchyId: {
                        required: "Selecione uma hierarquia"
                    },
                    MinQuantity: {
                        required: "Informe a quantidade mínima",
                        min: "Quantidade mínima deve ser maior que zero"
                    },
                    MaxQuantity: {
                        min: "Quantidade máxima não pode ser negativa"
                    },
                    AssemblyOrder: {
                        required: "Informe a ordem de montagem",
                        min: "Ordem de montagem deve ser maior que zero"
                    }
                },
                submitHandler: function(formElement) {
                    // Prevenir múltiplas submissões pelo validate
                    if ($(formElement).data('submitting')) {
                        return false;
                    }
                    compositeProductManager.hierarchies.saveCompositeProductXHierarchy(formElement);
                    return false;
                }
            });

            // Validação custom para MaxQuantity
            $('#MaxQuantity').on('input.customValidation', function() {
                var maxQty = parseInt($(this).val()) || 0;
                var minQty = parseInt($('#MinQuantity').val()) || 1;
                
                if (maxQty > 0 && maxQty < minQty) {
                    $(this).addClass('is-invalid');
                    var errorMsg = $(this).siblings('.field-validation-valid, .field-validation-error');
                    errorMsg.addClass('field-validation-error').removeClass('field-validation-valid')
                           .text('Quantidade máxima deve ser maior ou igual à mínima');
                } else {
                    $(this).removeClass('is-invalid');
                    var errorMsg = $(this).siblings('.field-validation-error');
                    errorMsg.addClass('field-validation-valid').removeClass('field-validation-error').text('');
                }
            });

            // Handle form submission via button click - APENAS se o validate não funcionou
            form.on('submit.compositeHierarchy', function(e) {
                e.preventDefault();
                
                // Prevenir múltiplas submissões
                if ($(this).data('submitting')) {
                    return false;
                }
                
                if ($(this).valid()) {
                    compositeProductManager.hierarchies.saveCompositeProductXHierarchy(this);
                }
                return false;
            });
        },

        // Initialize EditCompositeProductXHierarchy form
        initializeEditCompositeProductXHierarchyForm: function() {
            const form = $('#editCompositeProductXHierarchyForm');
            
            // Se o formulário não existir, retornar
            if (form.length === 0) {
                console.warn('Formulário editCompositeProductXHierarchyForm não encontrado');
                return;
            }

            // Limpar validações existentes para prevenir duplicação
            if (form.data('validator')) {
                form.removeData('validator');
                form.off('submit.validate');
            }
            
            // Remover TODOS os event listeners do form de forma mais abrangente
            form.off('submit.editCompositeHierarchy submit');
            $('#MaxQuantity, #MinQuantity, #AssemblyOrder, #IsOptionalEdit').off('input.editFormValidation change.editFormValidation');
            
            // Limpar flag de submitting se existir
            form.removeData('submitting');
            
            // Inicializar validação do formulário
            form.validate({
                rules: {
                    MinQuantity: {
                        required: true,
                        number: true,
                        min: 1
                    },
                    MaxQuantity: {
                        number: true,
                        min: 0
                    },
                    AssemblyOrder: {
                        required: true,
                        number: true,
                        min: 1
                    }
                },
                messages: {
                    MinQuantity: {
                        required: "A quantidade mínima é obrigatória",
                        number: "Informe um número válido",
                        min: "A quantidade mínima deve ser pelo menos 1"
                    },
                    MaxQuantity: {
                        number: "Informe um número válido",
                        min: "A quantidade máxima não pode ser negativa"
                    },
                    AssemblyOrder: {
                        required: "A ordem de montagem é obrigatória",
                        number: "Informe um número válido",
                        min: "A ordem de montagem deve ser pelo menos 1"
                    }
                },
                errorClass: 'is-invalid',
                validClass: 'is-valid',
                errorPlacement: function(error, element) {
                    error.addClass('invalid-feedback');
                    element.closest('.floating-input-group').append(error);
                },
                highlight: function(element) {
                    $(element).addClass('is-invalid').removeClass('is-valid');
                },
                unhighlight: function(element) {
                    $(element).removeClass('is-invalid').addClass('is-valid');
                },
                submitHandler: function(formElement) {
                    // Prevenir múltiplas submissões pelo validate
                    if ($(formElement).data('submitting')) {
                        return false;
                    }
                    compositeProductManager.hierarchies.updateCompositeProductXHierarchy(formElement);
                    return false;
                }
            });

            // Validação custom para MaxQuantity
            $('#MaxQuantity').on('input.editFormValidation', function() {
                var maxQty = parseInt($(this).val()) || 0;
                var minQty = parseInt($('#MinQuantity').val()) || 1;
                
                if (maxQty > 0 && maxQty < minQty) {
                    $(this).addClass('is-invalid');
                    var errorMsg = $(this).siblings('.field-validation-valid, .field-validation-error');
                    errorMsg.addClass('field-validation-error').removeClass('field-validation-valid')
                           .text('Quantidade máxima deve ser maior ou igual à mínima');
                } else {
                    $(this).removeClass('is-invalid');
                    var errorMsg = $(this).siblings('.field-validation-error');
                    errorMsg.addClass('field-validation-valid').removeClass('field-validation-error').text('');
                }
                compositeProductManager.hierarchies.updateEditPreview();
            });

            // Event listeners para atualizar o preview
            $('#MinQuantity, #AssemblyOrder').on('input.editFormValidation', function() {
                compositeProductManager.hierarchies.updateEditPreview();
            });

            $('#IsOptionalEdit').on('change.editFormValidation', function() {
                compositeProductManager.hierarchies.updateEditPreview();
            });

            // Atualizar preview inicial
            compositeProductManager.hierarchies.updateEditPreview();

            // Handle form submission via button click - APENAS se o validate não funcionou
            form.on('submit.editCompositeHierarchy', function(e) {
                e.preventDefault();
                
                // Prevenir múltiplas submissões
                if ($(this).data('submitting')) {
                    return false;
                }
                
                if ($(this).valid()) {
                    compositeProductManager.hierarchies.updateCompositeProductXHierarchy(this);
                }
                return false;
            });
        },

        // Update edit preview
        updateEditPreview: function() {
            const minQty = parseInt($('#MinQuantity').val()) || 0;
            const maxQty = parseInt($('#MaxQuantity').val()) || 0;
            const order = parseInt($('#AssemblyOrder').val()) || 0;
            const isOptional = $('#IsOptionalEdit').is(':checked');

            // Atualizar quantidade
            let quantityText = minQty.toString();
            if (maxQty > 0) {
                quantityText += ` - ${maxQty}`;
            } else {
                quantityText += '+';
            }
            $('#quantityPreview').text(quantityText);

            // Atualizar tipo
            $('#typePreview').text(isOptional ? 'Opcional' : 'Obrigatória');

            // Atualizar ordem
            $('#orderPreview').text(order ? `${order}º` : '-');
        },

        // Assign hierarchy to product
        assignHierarchy: function() {
            const form = $('#formAssignHierarchy')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            const formData = new FormData(form);

            // Disable submit button
            const submitBtn = $('#btnAssignHierarchy');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Associando...');

            $.ajax({
                url: '/ProductComponentHierarchy/AssociarHierarquia',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#assignHierarchyModal').modal('hide');
                        
                        // Recarregar lista de hierarquias
                        if (response.productId) {
                            compositeProductManager.hierarchies.carregarHierarquias(response.productId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao associar hierarquia');
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao associar hierarquia:', xhr);
                    toastr.error('Erro ao associar hierarquia');
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            });
        },

        // Confirm unassign hierarchy
        confirmUnassignHierarchy: function(productId, hierarchyId, hierarchyName) {
            if (confirm(`Tem certeza que deseja desassociar a hierarquia "${hierarchyName}" deste produto?`)) {
                this.unassignHierarchy(productId, hierarchyId);
            }
        },

        // Unassign hierarchy from product
        unassignHierarchy: function(productId, hierarchyId) {
            $.ajax({
                url: '/ProductComponentHierarchy/DesassociarHierarquia',
                type: 'POST',
                data: {
                    productId: productId,
                    hierarchyId: hierarchyId,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message || 'Hierarquia desassociada com sucesso');
                        
                        // Recarregar lista de hierarquias
                        if (response.productId) {
                            compositeProductManager.hierarchies.carregarHierarquias(response.productId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao desassociar hierarquia');
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao desassociar hierarquia:', xhr);
                    toastr.error('Erro ao desassociar hierarquia');
                }
            });
        },

        // Show edit hierarchy modal (implementar se necessário)
        showEditHierarchyModal: function(hierarchyId) {
            // Redirecionar para a página de edição padrão ou implementar modal
            window.open(`/ProductComponentHierarchy/Edit/${hierarchyId}`, '_blank');
        },

        // CompositeProductXHierarchy Relations - Edit & Management methods
        showRelationDetailsModal: function(relationId) {
            // Validar relationId
            if (!relationId || relationId === 'undefined' || relationId === 'null') {
                toastr.error('ID da relação não foi informado ou é inválido');
                return;
            }

            $.ajax({
                url: `/ProductComponentHierarchy/DetalhesCompositeProductXHierarchy/${relationId}`,
                type: 'GET',
                success: function(data, textStatus, xhr) {
                    // Check if response is JSON error
                    if (xhr.getResponseHeader('Content-Type')?.includes('application/json')) {
                        try {
                            const jsonResponse = typeof data === 'string' ? JSON.parse(data) : data;
                            if (jsonResponse.success === false) {
                                toastr.error(jsonResponse.message || 'Erro ao carregar detalhes da relação');
                                return;
                            }
                        } catch (parseError) {
                            // Continue with normal processing if JSON parse fails
                        }
                    }

                    // Remove modal if it exists
                    if ($('#detailsCompositeProductXHierarchyModal').length > 0) {
                        $('#detailsCompositeProductXHierarchyModal').remove();
                    }
                    
                    $('body').append(data);
                    
                    // Check if modal was actually created
                    if ($('#detailsCompositeProductXHierarchyModal').length === 0) {
                        toastr.error('Erro ao criar modal de detalhes');
                        return;
                    }

                    // Initialize modal
                    const modal = new bootstrap.Modal(document.getElementById('detailsCompositeProductXHierarchyModal'));
                    modal.show();
                    
                    // Clean up when modal is closed
                    $('#detailsCompositeProductXHierarchyModal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao carregar detalhes da relação:', xhr);
                    let errorMsg = 'Erro ao carregar detalhes da relação';
                    
                    if (xhr.status === 404) {
                        errorMsg = 'Relação não encontrada';
                    } else if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.status === 500) {
                        errorMsg = 'Erro interno do servidor';
                    }
                    
                    toastr.error(errorMsg);
                }
            });
        },

        showEditRelationModal: function(relationId) {
            // Validar relationId
            if (!relationId || relationId === 'undefined' || relationId === 'null') {
                toastr.error('ID da relação não foi informado ou é inválido');
                return;
            }

            $.ajax({
                url: `/ProductComponentHierarchy/EditarCompositeProductXHierarchy/${relationId}`,
                type: 'GET',
                success: function(data, textStatus, xhr) {
                    // Check if response is JSON error
                    if (xhr.getResponseHeader('Content-Type')?.includes('application/json')) {
                        try {
                            const jsonResponse = typeof data === 'string' ? JSON.parse(data) : data;
                            if (jsonResponse.success === false) {
                                toastr.error(jsonResponse.message || 'Erro ao carregar formulário de edição');
                                return;
                            }
                        } catch (parseError) {
                            // Continue with normal processing if JSON parse fails
                        }
                    }

                    // Remove modal if it exists
                    if ($('#editCompositeProductXHierarchyModal').length > 0) {
                        $('#editCompositeProductXHierarchyModal').remove();
                    }
                    
                    $('body').append(data);
                    
                    // Check if modal was actually created
                    if ($('#editCompositeProductXHierarchyModal').length === 0) {
                        toastr.error('Erro ao criar formulário de edição');
                        return;
                    }

                    // Initialize modal
                    const modal = new bootstrap.Modal(document.getElementById('editCompositeProductXHierarchyModal'));
                    modal.show();
                    
                    // Remove any existing event listeners before adding new ones
                    $('#editCompositeProductXHierarchyModal').off('shown.bs.modal').on('shown.bs.modal', function() {
                        compositeProductManager.hierarchies.initializeEditCompositeProductXHierarchyForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#editCompositeProductXHierarchyModal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                        $(this).find('form')[0]?.reset();
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao carregar formulário de edição:', xhr);
                    let errorMsg = 'Erro ao carregar formulário de edição';
                    
                    if (xhr.status === 404) {
                        errorMsg = 'Relação não encontrada';
                    } else if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.status === 500) {
                        errorMsg = 'Erro interno do servidor';
                    }
                    
                    toastr.error(errorMsg);
                }
            });
        },

        moveHierarchyUp: function(relationId) {
            toastr.info('Funcionalidade de reordenação será implementada em breve');
        },

        moveHierarchyDown: function(relationId) {
            toastr.info('Funcionalidade de reordenação será implementada em breve');
        },

        confirmRemoveRelation: function(relationId, hierarchyName) {
            if (confirm(`Tem certeza que deseja remover a relação com a hierarquia "${hierarchyName}"?`)) {
                this.removeRelation(relationId);
            }
        },

        updateEditPreview: function() {
            const minQty = parseInt($('#MinQuantity').val()) || 0;
            const maxQty = parseInt($('#MaxQuantity').val()) || 0;
            const isOptional = $('#IsOptionalEdit').is(':checked');
            const assemblyOrder = parseInt($('#AssemblyOrder').val()) || 0;

            // Atualizar preview de quantidade
            let quantityText = maxQty === 0 ? `${minQty}+` : `${minQty}-${maxQty}`;
            $('#quantityPreview').text(quantityText);

            // Atualizar preview de tipo
            $('#typePreview').text(isOptional ? 'Opcional' : 'Obrigatório');

            // Atualizar preview de ordem
            $('#orderPreview').text(assemblyOrder > 0 ? `${assemblyOrder}º` : '-');
        },

        updateCompositeProductXHierarchy: function(form) {
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            // Verificar se já está sendo submetido
            const $form = $(form);
            if ($form.data('submitting')) {
    
                return;
            }

            // Marcar como sendo submetido
            $form.data('submitting', true);

            const formData = new FormData(form);

            // Disable submit button
            const submitBtn = $form.find('button[type="submit"]');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

            $.ajax({
                url: '/ProductComponentHierarchy/AtualizarCompositeProductXHierarchy',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#editCompositeProductXHierarchyModal').modal('hide');
                        
                        // Recarregar lista de hierarquias do produto
                        const productId = $('.product-edit-container').data('product-id') || $('#ProductId').val();
                        if (productId) {
                            compositeProductManager.hierarchies.carregarHierarquias(productId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao atualizar relação');
                        if (response.errors) {
                            compositeProductManager.utils.showValidationErrors(response.errors, 'editCompositeProductXHierarchyModal');
                        }
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao atualizar relação CompositeProductXHierarchy:', xhr);
                    let errorMsg = 'Erro ao atualizar relação';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        compositeProductManager.utils.showValidationErrors(xhr.responseJSON.errors, 'editCompositeProductXHierarchyModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button e limpar flag
                    submitBtn.prop('disabled', false).html(originalText);
                    $form.removeData('submitting');
                }
            });
        },

        removeRelation: function(relationId) {
            // Validar relationId
            if (!relationId || relationId === 'undefined' || relationId === 'null') {
                toastr.error('ID da relação não foi informado ou é inválido');
                return;
            }

            // Criar FormData com anti-forgery token
            const formData = new FormData();
            formData.append('__RequestVerificationToken', $('input[name="__RequestVerificationToken"]').val());

            $.ajax({
                url: `/ProductComponentHierarchy/DeletarCompositeProductXHierarchy/${relationId}`,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message || 'Relação removida com sucesso!');
                        
                        // Recarregar lista de hierarquias do produto
                        const productId = $('.product-edit-container').data('product-id') || $('#ProductId').val();
                        if (productId) {
                            compositeProductManager.hierarchies.carregarHierarquias(productId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao remover relação');
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao remover relação CompositeProductXHierarchy:', xhr);
                    let errorMsg = 'Erro ao remover relação';
                    
                    if (xhr.status === 404) {
                        errorMsg = 'Relação não encontrada';
                    } else if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.status === 500) {
                        errorMsg = 'Erro interno do servidor';
                    }
                    
                    toastr.error(errorMsg);
                }
            });
        }
    },

    // Demand Management
    demands: {
        // Mostrar modal de criação de demanda
        showCreateModal: function(productId) {
            window.location.href = `/Demand/Create?productId=${productId}`;
        },
        
        // Carregar demandas
        loadDemands: function(productId) {
            $('#demands-grid').load(`/Demand/GetByProduct/${productId}`);
            this.loadStats(productId);
        },
        
        // Carregar estatísticas das demandas
        loadStats: function(productId) {
            $.get(`/Demand/GetStatsByProduct/${productId}`)
                .done(function(data) {
                    $('#demands-pending-count').text(data.pending || 0);
                    $('#demands-confirmed-count').text(data.confirmed || 0);
                    $('#demands-produced-count').text(data.produced || 0);
                    $('#demands-delivered-count').text(data.delivered || 0);
                    $('#demands-overdue-count').text(data.overdue || 0);
                })
                .fail(function() {
                    console.error('Erro ao carregar estatísticas das demandas');
                });
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
    // Statistics (removed - endpoint não existe)
    
    // Components - Integrated methods
    showCreateComponentModal: (id) => compositeProductManager.components.showCreateComponentModal(id),
    showEditComponentModal: (id) => compositeProductManager.components.editarComponente(id),
    confirmDeleteComponent: (id, name) => compositeProductManager.components.confirmarRemocao(id, name),
    refreshComponentsList: (id) => compositeProductManager.components.carregarComponentes(id),
    
    // Components - Legacy compatibility
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
    initializeComponentsTable: () => compositeProductManager.components.initializeComponentsTable(),
    
    // Hierarchies - Implemented methods
    showCreateHierarchyModal: (id) => compositeProductManager.hierarchies.showCreateHierarchyModal(id),
    showAssignHierarchyModal: (id) => compositeProductManager.hierarchies.showAssignHierarchyModal(id),
    showHierarchyDetailsModal: (hierarchyId, productId) => compositeProductManager.hierarchies.showHierarchyDetailsModal(hierarchyId, productId),
    showEditHierarchyModal: (id) => compositeProductManager.hierarchies.showEditHierarchyModal(id),
    initializeCreateCompositeProductXHierarchyForm: () => compositeProductManager.hierarchies.initializeCreateCompositeProductXHierarchyForm(),
    initializeEditCompositeProductXHierarchyForm: () => compositeProductManager.hierarchies.initializeEditCompositeProductXHierarchyForm(),
    showManageHierarchyComponentsModal: (hierarchyId, productId) => compositeProductManager.hierarchies.showManageHierarchyComponentsModal(hierarchyId, productId),
    confirmUnassignHierarchy: (productId, hierarchyId, name) => compositeProductManager.hierarchies.confirmUnassignHierarchy(productId, hierarchyId, name),
    refreshHierarchiesList: (id) => compositeProductManager.hierarchies.carregarHierarquias(id),
    
    // CompositeProductXHierarchy Relations - New methods
    showRelationDetailsModal: (relationId) => compositeProductManager.hierarchies.showRelationDetailsModal(relationId),
    showEditRelationModal: (relationId) => compositeProductManager.hierarchies.showEditRelationModal(relationId),
    moveHierarchyUp: (relationId) => compositeProductManager.hierarchies.moveHierarchyUp(relationId),
    moveHierarchyDown: (relationId) => compositeProductManager.hierarchies.moveHierarchyDown(relationId),
    confirmRemoveRelation: (relationId, hierarchyName) => compositeProductManager.hierarchies.confirmRemoveRelation(relationId, hierarchyName)
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

// Expor globalmente para acesso em outros scripts
window.compositeProductManager = compositeProductManager;

} // Fim da proteção contra redeclaração 