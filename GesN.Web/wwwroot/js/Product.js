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
        if ($('#btnNovoProduct').length) {
            $('#btnNovoProduct').on('click', () => this.novoProduct());
        }
        
        // Filtros - apenas vincula se existirem
        if ($('#filterProductType').length) {
            $('#filterProductType').on('change', () => this.aplicarFiltros());
        }
        if ($('#filterCategory').length) {
            $('#filterCategory').on('change', () => this.aplicarFiltros());
        }
        if ($('#filterStatus').length) {
            $('#filterStatus').on('change', () => this.aplicarFiltros());
        }
        if ($('#searchProduct').length) {
            $('#searchProduct').on('input', debounce(() => this.aplicarFiltros(), 300));
        }
        
        // Botão limpar filtros
        if ($('#btnLimparFiltros').length) {
            $('#btnLimparFiltros').on('click', () => this.limparFiltros());
        }
        
        // Event handlers globais para modals e abas
        this.setupGlobalEventHandlers();
    },

    // Setup global event handlers for modals and tabs
    setupGlobalEventHandlers: function() {
        // Inicializar componentes quando modal é mostrado
        $(document).off('shown.bs.modal.productManager').on('shown.bs.modal.productManager', '.modal', (e) => {
            this.initializeForm(e.target);
        });
        
        // Limpeza quando modal é fechado
        $(document).off('hidden.bs.modal.productManager').on('hidden.bs.modal.productManager', '.modal', (e) => {
            this.cleanupForm(e.target);
        });
        
        // Inicializar componentes quando abas são mostradas (para o _Edit.cshtml)
        $(document).off('shown.bs.tab.productManager').on('shown.bs.tab.productManager', 'button[data-bs-toggle="tab"]', (e) => {
            const targetPane = $(e.target.getAttribute('data-bs-target'));
            if (targetPane.length) {
                // Delay para garantir que o conteúdo foi carregado
                setTimeout(() => {
                    this.initializeForm(targetPane[0]);
                }, 150);

                // As abas ProductGroup agora são renderizadas diretamente via @await Html.PartialAsync()
                // Não precisamos mais das chamadas AJAX para loadGroupItems, loadGroupOptions, loadGroupExchangeRules
                // Apenas inicializar floating labels nos forms que podem ter sido renderizados
                const tabId = e.target.id;
                if (tabId === 'group-items-tab' || tabId === 'group-options-tab' || tabId === 'exchange-rules-tab') {
                    setTimeout(() => {
                        if (this.forms) {
                            this.forms.initFloatingLabels(targetPane[0]);
                        }
                    }, 200);
                }
            }
        });
        
        // Detectar quando conteúdo AJAX é inserido no DOM (para abas de edição)
        $(document).off('DOMNodeInserted.productManager').on('DOMNodeInserted.productManager', (e) => {
            if ($(e.target).find('.select2-category, input[name="Price"], input[name="Cost"]').length) {
                setTimeout(() => {
                    this.initializeForm(e.target);
                }, 100);
            }
        });
    },

    // Carregar grid principal
    loadGrid: function() {
        $.ajax({
            url: `${this.config.baseUrl}/_Grid`,
            type: 'GET',
            data: this.getFilterData(),
            success: (data) => {
                $('#lista-produtos-container').html(data);
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
            //language: {
            //    url: '/lib/datatables.net/pt-.json'
            //},
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
                $('#total-produtos').text(data.totalProducts);
                $('#produtos-ativos').text(data.activeProducts);
                $('#produtos-simples').text(data.simpleProducts);
                // CompositeProduct statistics handled in CompositeProduct.js
                $('#grupos-produtos').text(data.groupProducts);
                
                // Atualizar badges de alerta se existirem
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
            productType: $('#filterProductType').length ? $('#filterProductType').val() : null,
            categoryId: $('#filterCategory').length ? $('#filterCategory').val() : null,
            status: $('#filterStatus').length ? $('#filterStatus').val() : null,
            search: $('#searchProduct').length ? $('#searchProduct').val() : null
        };
    },

    // Aplicar filtros
    aplicarFiltros: function() {
        this.loadGrid();
        this.loadStatistics();
    },

    // Limpar filtros
    limparFiltros: function() {
        if ($('#filterProductType').length) $('#filterProductType').val('');
        if ($('#filterCategory').length) $('#filterCategory').val('');
        if ($('#filterStatus').length) $('#filterStatus').val('');
        if ($('#searchProduct').length) $('#searchProduct').val('');
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
                
                // Inicializar componentes do formulário (Select2, cálculos, etc.)
                setTimeout(() => {
                    const formContainer = document.getElementById('formNovoProduct');
                    if (formContainer) {
                        this.initializeForm(formContainer);
                    }
                }, 150);
                
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
        if (form.length === 0) {
            return;
        }

        if (!this.validateForm(form)) {
            return;
        }

        const formData = new FormData(form[0]);

        // Desabilita o botão de submit para evitar múltiplos envios
        const submitButton = form.find('button[type="button"]');
        const buttonText = submitButton.text();
        submitButton.prop('disabled', true).text('Salvando...');

        return $.ajax({
            url: `${this.config.baseUrl}/SalvarNovo`,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: (response) => {
                if (response.success) {
                    $(this.config.modalId).modal('hide');
                    toastr.success(response.message || 'Produto criado com sucesso!');
                    this.loadGrid();
                    this.loadStatistics();
                    
                    if (response.productId) {
                        // Chama o método de edição diretamente
                        this.editarProduct(response.productId);
                    }
                } else {
                    toastr.error(response.message || 'Não foi possível criar o produto');
                    this.showValidationErrors(response.errors);
                }
            },
            error: (xhr, status, error) => {
                const errorMessage = xhr.responseJSON?.message || 'Ocorreu um erro ao salvar o produto. Por favor, tente novamente.';
                toastr.error(errorMessage);
            },
            complete: () => {
                // Reabilita o botão
                submitButton.prop('disabled', false).text(buttonText);
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

    // Editar produto - abrir como aba
    editarProduct: function(productId, productName = null) {
        // Verificar se já existe uma aba para este produto
        const existingTabId = `product-${productId}`;
        const existingTab = $(`#${existingTabId}-tab`);
        
        if (existingTab.length > 0) {
            // Se a aba já existe, apenas ativa ela
            const tabTrigger = new bootstrap.Tab(document.getElementById(`${existingTabId}-tab`));
            tabTrigger.show();
            toastr.info('Produto já está aberto em outra aba');
            return;
        }

        // Se não existe, cria nova aba
        const tabId = existingTabId;
        this.config.currentProductId = productId;
        this.config.currentEditMode = 'edit';
        
        // Se productName não foi fornecido, usa um placeholder que será atualizado
        const tabTitle = productName || 'Carregando...';
        
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab" data-product-id="${productId}">
                    <i class="fas fa-edit"></i> ${tabTitle}
                    <span class="btn-close ms-2" onclick="productManager.fecharAba('${tabId}')"></span>
                </button>
            </li>`;
        $('#productTabs').append(novaAba);
        
        const novoConteudo = `
            <div class="tab-pane fade" id="${tabId}" role="tabpanel">
                <div id="conteudo-${tabId}">
                    <div class="d-flex justify-content-center my-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Carregando...</span>
                        </div>
                    </div>
                </div>
            </div>`;
        $('#productTabsContent').append(novoConteudo);
        
        // Carrega o conteúdo da aba
        $.get(`${this.config.baseUrl}/FormularioEdicao/${productId}`)
            .done((data) => {
                $(`#conteudo-${tabId}`).html(data);
                
                // Se productName não foi fornecido, extrai do conteúdo carregado
                if (!productName) {
                    const productNameElement = $(`#conteudo-${tabId}`).find('input[name="Name"]');
                    if (productNameElement.length > 0) {
                        const extractedProductName = productNameElement.val();
                        if (extractedProductName) {
                            $(`#${tabId}-tab`).html(`<i class="fas fa-edit"></i> ${extractedProductName} <span class="btn-close ms-2" onclick="productManager.fecharAba('${tabId}')"></span>`);
                        }
                    }
                }
                
                // Inicializar gerenciamento de sub-abas e componentes do formulário
                setTimeout(() => {
                    this.initializeEditTabs(productId, tabId);
                    const container = document.getElementById(`conteudo-${tabId}`);
                    if (container) {
                        this.initializeForm(container);
                    }
                }, 150);
            })
            .fail(() => {
                $(`#conteudo-${tabId}`).html('<div class="alert alert-danger">Erro ao carregar produto. Tente novamente.</div>');
            });
            
        // Ativa a nova aba
        const tabTrigger = new bootstrap.Tab(document.getElementById(`${tabId}-tab`));
        tabTrigger.show();
    },

    // Fechar aba
    fecharAba: function(tabId) {
        // Remove a aba e seu conteúdo
        $(`#${tabId}-tab`).parent().remove();
        $(`#${tabId}`).remove();
        
        // Volta para a aba principal de lista
        const mainTab = new bootstrap.Tab(document.getElementById('tab-lista-produtos'));
        mainTab.show();
        
        // Limpar referências
        this.config.currentProductId = null;
        this.config.currentEditMode = null;
    },

    // Inicializar gerenciamento de sub-abas do formulário de edição
    initializeEditTabs: function(productId, tabId) {
        const containerSelector = `#conteudo-${tabId}`;
        
        // Remover eventos anteriores para evitar duplicatas
        $(`${containerSelector} #productEditTabs button[data-bs-toggle="tab"]`).off('shown.bs.tab.productEdit');
        
        // Configurar carregamento lazy das sub-abas
        $(`${containerSelector} #productEditTabs button[data-bs-toggle="tab"]`).on('shown.bs.tab.productEdit', (e) => {
            const targetTab = $(e.target).attr('data-bs-target');
            
            switch(targetTab) {
                case '#components-tab-pane':
                    if ($(`${containerSelector} #componentsContainer .spinner-border`).length > 0) {
                        // TODO: Implementar carregamento de componentes
                        console.log('Carregamento de componentes não implementado ainda');
                    }
                    break;
                case '#group-items-tab-pane':
                case '#group-options-tab-pane':
                case '#exchange-rules-tab-pane':
                    // As abas ProductGroup agora são renderizadas diretamente via @await Html.PartialAsync()
                    // Não precisamos recarregar via AJAX
                    break;
            }
        });
    },

    // Salvar edição
    salvarEdicao: function() {
        const currentProductId = this.config.currentProductId;
        if (!currentProductId) {
            toastr.error('Nenhum produto selecionado para edição');
            return;
        }

        const tabId = `product-${currentProductId}`;
        const form = $(`#conteudo-${tabId} #formEditProduct`);
        if (form.length === 0) {
            toastr.error('Formulário de edição não encontrado');
            return;
        }

        const formData = form.serialize();

        if (!this.validateForm(form)) {
            return;
        }

        $.ajax({
            url: `${this.config.baseUrl}/SalvarEdicaoProduct/${currentProductId}`,
            type: 'POST',
            data: formData,
            success: (response) => {
                if (response.success) {
                    toastr.success('Produto atualizado com sucesso!');
                    this.loadGrid();
                    this.loadStatistics();
                    
                    // Opcional: fechar aba após salvar
                    // this.fecharAba(tabId);
                } else {
                    toastr.error(response.message || 'Erro ao atualizar produto');
                    this.showValidationErrors(response.errors, tabId);
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
        const currentProductId = this.config.currentProductId;
        if (currentProductId) {
            const tabId = `product-${currentProductId}`;
            this.fecharAba(tabId);
        }
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



    // Validação de formulário
    validateForm: function(form) {
        // Se o formulário tem floating labels, use o sistema de validação específico
        if (form.find('.floating-input, .floating-select, .floating-textarea').length > 0) {
            return this.forms.validateForm(form[0]);
        }
        
        // Fallback para validação tradicional
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

    // Mostrar erros de validação para aba específica ou modal
    showValidationErrors: function(errors, tabId = null) {
        if (errors) {
            // Determinar o container correto
            let container = '';
            if (tabId) {
                // Check if it's a modal
                if (tabId.includes('Modal')) {
                    container = `#${tabId}`;
                } else {
                    container = `#conteudo-${tabId}`;
                }
            } else if (this.config.currentEditMode === 'create') {
                container = '#formNovoProduct';
            }
            
            Object.keys(errors).forEach(key => {
                const field = $(`${container} [name="${key}"]`);
                if (field.length) {
                    // Se está usando floating labels, use o sistema específico
                    if (field.hasClass('floating-input') || field.hasClass('floating-select') || field.hasClass('floating-textarea')) {
                        if (this.forms) {
                            this.forms.showValidation(field, errors[key][0]);
                        }
                    } else {
                        // Fallback para validação tradicional
                        field.addClass('is-invalid');
                        field.siblings('.text-danger').text(errors[key][0]);
                    }
                }
            });
        }
    },

    // Filtros rápidos
    filtrarPorStatus: function(status) {
        if ($('#filterStatus').length) {
            if (status === 'todos') {
                $('#filterStatus').val('');
            } else if (status === 'ativo') {
                $('#filterStatus').val('1');
            } else {
                $('#filterStatus').val('0');
            }
        }
        this.aplicarFiltros();
    },

    filtrarPorTipo: function(tipo) {
        if ($('#filterProductType').length) {
            $('#filterProductType').val(tipo);
        }
        this.aplicarFiltros();
    },

    filtrarEstoqueBaixo: function() {
        // Implementar filtro para estoque baixo
        console.log('Filtrar produtos com estoque baixo');
        // Por enquanto, apenas aplicar filtros existentes
        this.aplicarFiltros();
    },

    // CompositeProduct functionality moved to CompositeProduct.js
    // Include CompositeProduct.js in your view to use Component methods



    // Namespace para gerenciamento de formulários com floating labels
    forms: {
        // Inicializar floating labels
        initFloatingLabels: function(container = document) {
            this.setupFloatingLabels(container);
            this.setupFormValidation(container);
            this.setupCalculations(container);
            this.setupAccessibility(container);
            
            // Force initial state for special cases
            this.forceInitialStates(container);
        },
        
        // Force initial states for edge cases
        forceInitialStates: function(container) {
            // Ensure all addon inputs maintain floating state
            $(container).find('.floating-input-group-addon .floating-input').addClass('has-value');
            
            // Check textareas for initial values (don't force floating)
            $(container).find('.floating-textarea').each(function() {
                const $textarea = $(this);
                productManager.forms.checkInputValue($textarea);
            });
            
            // Ensure selects with values maintain floating state
            $(container).find('.floating-select').each(function() {
                const $select = $(this);
                if ($select.val() && $select.val() !== '') {
                    $select.addClass('has-value');
                }
            });
            
            // Force number inputs with values to maintain floating state
            $(container).find('.floating-input[type="number"]').each(function() {
                const $input = $(this);
                productManager.forms.checkInputValue($input);
            });
            
            // Ensure disabled/readonly inputs maintain floating state
            $(container).find('.floating-input:disabled, .floating-input[readonly]').addClass('has-value');
        },

        // Setup floating label behavior
        setupFloatingLabels: function(container) {
            // Handle floating inputs (excluding those with addons which are always floating)
            $(container).find('.floating-input, .floating-select, .floating-textarea').each(function() {
                const $input = $(this);
                const $parent = $input.closest('.floating-input-group');
                
                // Skip inputs with addons as they are always in floating state
                if ($parent.hasClass('floating-input-group-addon')) {
                    // For addon inputs, always ensure they have the floating state
                    $input.addClass('has-value');
                    return;
                }
                
                // Textareas should behave like regular inputs
                if ($input.hasClass('floating-textarea')) {
                    // Check initial value and add class if needed
                    productManager.forms.checkInputValue($input);
                    
                    // Add event listeners for textareas
                    $input.on('focus blur input change', function() {
                        productManager.forms.checkInputValue($(this));
                    });
                    return;
                }
                
                // Check initial value and add class if needed for regular inputs
                productManager.forms.checkInputValue($input);
                
                // Add event listeners for regular inputs
                $input.on('focus blur input change', function() {
                    productManager.forms.checkInputValue($(this));
                });
            });
            
            // Special handling for selects
            $(container).find('.floating-select').on('change', function() {
                const $select = $(this);
                if ($select.val() && $select.val() !== '') {
                    $select.addClass('has-value');
                } else {
                    $select.removeClass('has-value');
                }
            });
        },

        // Check if input has value and toggle floating state
        checkInputValue: function($input) {
            const value = $input.val();
            const inputType = $input.attr('type');
            let hasValue = false;
            
            // For number inputs, treat 0 as a valid value
            if (inputType === 'number') {
                hasValue = value !== null && value !== undefined && value !== '';
            } else {
                // For text inputs, check for non-empty trimmed value
                hasValue = value && value.trim() !== '';
            }
            
            if (hasValue) {
                $input.addClass('has-value');
            } else {
                $input.removeClass('has-value');
            }
        },

        // Setup form validation
        setupFormValidation: function(container) {
            // Real-time validation
            $(container).find('.floating-input, .floating-select, .floating-textarea').on('blur', function() {
                const $input = $(this);
                productManager.forms.validateField($input);
            });

            // Clear validation on input
            $(container).find('.floating-input, .floating-select, .floating-textarea').on('input change', function() {
                const $input = $(this);
                if ($input.hasClass('is-invalid')) {
                    productManager.forms.clearValidation($input);
                }
            });
        },

        // Validate individual field
        validateField: function($input) {
            const value = $input.val();
            const isRequired = $input.prop('required') || $input.data('required');
            let isValid = true;
            let errorMessage = '';

            // Required validation
            if (isRequired && (!value || value.trim() === '')) {
                isValid = false;
                errorMessage = 'Este campo é obrigatório';
            }

            // Email validation
            if (value && $input.attr('type') === 'email') {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(value)) {
                    isValid = false;
                    errorMessage = 'Por favor, insira um email válido';
                }
            }

            // URL validation
            if (value && $input.attr('type') === 'url') {
                try {
                    new URL(value);
                } catch {
                    isValid = false;
                    errorMessage = 'Por favor, insira uma URL válida';
                }
            }

            // Number validation
            if (value && $input.attr('type') === 'number') {
                const numValue = parseFloat(value);
                const min = parseFloat($input.attr('min'));
                const max = parseFloat($input.attr('max'));

                if (isNaN(numValue)) {
                    isValid = false;
                    errorMessage = 'Por favor, insira um número válido';
                } else if (!isNaN(min) && numValue < min) {
                    isValid = false;
                    errorMessage = `O valor deve ser maior ou igual a ${min}`;
                } else if (!isNaN(max) && numValue > max) {
                    isValid = false;
                    errorMessage = `O valor deve ser menor ou igual a ${max}`;
                }
            }

            // Apply validation state
            if (isValid) {
                this.clearValidation($input);
            } else {
                this.showValidation($input, errorMessage);
            }

            return isValid;
        },

        // Show validation error
        showValidation: function($input, message) {
            $input.addClass('is-invalid');
            
            let $errorContainer = $input.siblings('.text-danger');
            if ($errorContainer.length === 0) {
                $errorContainer = $('<span class="text-danger"></span>');
                $input.parent().append($errorContainer);
            }
            
            $errorContainer.text(message);
        },

        // Clear validation error
        clearValidation: function($input) {
            $input.removeClass('is-invalid');
            $input.siblings('.text-danger').text('');
        },

        // Setup automatic calculations
        setupCalculations: function(container) {
            // Profit margin calculation
            $(container).find('#Price, #Cost').on('input', function() {
                productManager.forms.calculateProfitMargin();
            });
        },

        // Calculate profit margin
        calculateProfitMargin: function() {
            const price = parseFloat($('#Price').val()) || 0;
            const cost = parseFloat($('#Cost').val()) || 0;
            
            // Find margin display element (could be in create or edit form)
            const $marginDisplay = $('#marginDisplay');
            const $marginSection = $('#marginSection');
            
            if (price > 0 && cost > 0) {
                const margin = ((price - cost) / cost) * 100;
                $marginDisplay.text(margin.toFixed(1) + '%');
                
                // Show margin section (for create form)
                if ($marginSection.length > 0) {
                    $marginSection.show();
                }
                
                // Show margin parent (for edit form)
                $marginDisplay.parent().removeClass('d-none').show();
                
                // Add color coding
                const $alertContainer = $marginDisplay.parent();
                if (margin < 10) {
                    $alertContainer.removeClass('alert-info alert-success').addClass('alert-warning');
                } else if (margin < 20) {
                    $alertContainer.removeClass('alert-warning alert-success').addClass('alert-info');
                } else {
                    $alertContainer.removeClass('alert-warning alert-info').addClass('alert-success');
                }
            } else {
                // Hide margin displays
                if ($marginSection.length > 0) {
                    $marginSection.hide();
                }
                $marginDisplay.parent().hide();
            }
        },

        // Validate entire form
        validateForm: function(container) {
            let isValid = true;
            
            $(container).find('.floating-input, .floating-select, .floating-textarea').each(function() {
                if (!productManager.forms.validateField($(this))) {
                    isValid = false;
                }
            });

            // Custom business logic validations
            const price = parseFloat($(container).find('#Price').val()) || 0;
            const cost = parseFloat($(container).find('#Cost').val()) || 0;
            
            if (price > 0 && cost > 0 && price <= cost) {
                toastr.warning('Atenção: O preço de venda está menor ou igual ao custo!');
            }

            return isValid;
        },

        // Reset form to initial state
        resetFormState: function(container) {
            $(container).find('.floating-input, .floating-select, .floating-textarea').each(function() {
                const $input = $(this);
                $input.removeClass('has-value is-invalid');
                productManager.forms.clearValidation($input);
            });
            
            $(container).find('#marginDisplay').parent().hide();
        },

        // Accessibility improvements
        setupAccessibility: function(container) {
            // Add ARIA attributes
            $(container).find('.floating-input, .floating-select, .floating-textarea').each(function() {
                const $input = $(this);
                const $label = $input.siblings('.floating-label');
                
                if ($label.length > 0) {
                    const labelText = $label.text();
                    $input.attr('aria-label', labelText);
                    
                    // Link label with input
                    const inputId = $input.attr('id') || 'input_' + Math.random().toString(36).substr(2, 9);
                    const labelId = 'label_' + inputId;
                    
                    $input.attr('id', inputId).attr('aria-labelledby', labelId);
                    $label.attr('id', labelId);
                }
            });

            // Keyboard navigation improvements
            $(container).find('.floating-input, .floating-select, .floating-textarea').on('keydown', function(e) {
                // Enter key should move to next field (except textarea)
                if (e.key === 'Enter' && !$(this).is('textarea')) {
                    e.preventDefault();
                    const $inputs = $(container).find('.floating-input, .floating-select, .floating-textarea');
                    const currentIndex = $inputs.index(this);
                    const $nextInput = $inputs.eq(currentIndex + 1);
                    
                    if ($nextInput.length > 0) {
                        $nextInput.focus();
                    }
                }
            });
        }
    },

    // Select2 Management for Category Fields
    select2: {
        // Initialize all Select2 instances in container
        initializeAll: function(container) {
            const $container = container ? $(container) : $(document);
            
            // Initialize category selects
            $container.find('.select2-category').each(function() {
                productManager.select2.initializeCategorySelect($(this));
            });
        },

        // Initialize a single category select
        initializeCategorySelect: function($select) {
            if (!$select.length) return;
            
            // Generate unique ID if not present
            const selectId = $select.attr('id') || 'select2-category-' + Math.random().toString(36).substr(2, 9);
            $select.attr('id', selectId);
            
            // Skip if already initialized and working
            if ($select.hasClass('select2-hidden-accessible') && $select.next('.select2-container').length) {
                return;
            }
            
            // Destroy previous instance
            if ($select.hasClass('select2-hidden-accessible')) {
                $select.select2('destroy');
            }
            
            // Find correct dropdown parent (modal if exists, otherwise body)
            const $modal = $select.closest('.modal');
            const dropdownParent = $modal.length ? $modal : $('body');
            
            $select.select2({
                placeholder: 'Selecione uma categoria',
                allowClear: true,
                language: 'pt-BR',
                dropdownParent: dropdownParent,
                width: '100%',
                ajax: {
                    url: '/ProductCategory/BuscaProductCategoryAutocomplete',
                    dataType: 'json',
                    delay: 300,
                    data: function (params) {
                        return {
                            termo: params.term || ''
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data.map(function(item) {
                                return {
                                    id: item.id,
                                    text: item.text,
                                    description: item.description
                                };
                            })
                        };
                    },
                    cache: true
                },
                templateResult: function(category) {
                    if (category.loading) {
                        return 'Buscando...';
                    }
                    
                    if (!category.id) {
                        return category.text;
                    }
                    
                    const $result = $(
                        '<div class="select2-result-category">' +
                            '<div class="select2-result-category__title">' + category.text + '</div>' +
                            (category.description ? '<div class="select2-result-category__description">' + category.description + '</div>' : '') +
                        '</div>'
                    );
                    
                    return $result;
                },
                templateSelection: function(category) {
                    return category.text || category.id;
                },
                minimumInputLength: 0
            });
            
            // Handle floating label integration
            $select.on('select2:open select2:close select2:select select2:unselect', function() {
                const $container = $(this).next('.select2-container');
                if ($(this).val()) {
                    $container.addClass('has-value');
                } else {
                    $container.removeClass('has-value');
                }
            });
            
            // Set initial state for floating label
            if ($select.val()) {
                $select.next('.select2-container').addClass('has-value');
            }
        },

        // Destroy all Select2 instances in container
        destroyAll: function(container) {
            const $container = container ? $(container) : $(document);
            $container.find('.select2-hidden-accessible').each(function() {
                const $select = $(this);
                if ($select.data('select2')) {
                    $select.select2('destroy');
                }
            });
        }
    },

    // Form initialization and setup
    initializeForm: function(container) {
        const $container = $(container);
        
        // Initialize Select2 components
        this.select2.initializeAll($container);
        
        // Setup form calculations
        this.setupFormCalculations($container);
        
        // Initialize floating labels if forms module exists
        if (this.forms) {
            this.forms.initFloatingLabels(container);
        }
    },

    // Setup form calculations (margin, etc.)
    setupFormCalculations: function(container) {
        const $container = $(container);
        
        // Setup margin calculation for price/cost fields
        $container.find('input[name="Price"], input[name="Cost"]').off('input.marginCalc').on('input.marginCalc', function() {
            productManager.calculateMargin($container);
        });
        
        // Calculate initial margin
        setTimeout(() => {
            productManager.calculateMargin($container);
        }, 100);
    },

    // Calculate profit margin
    calculateMargin: function(container) {
        const $container = $(container);
        const $priceInput = $container.find('input[name="Price"]');
        const $costInput = $container.find('input[name="Cost"]');
        const $marginDisplay = $container.find('#marginDisplay');
        const $marginSection = $container.find('#marginSection');
        
        if (!$priceInput.length || !$costInput.length || !$marginDisplay.length) {
            return;
        }
        
        const price = parseFloat($priceInput.val()) || 0;
        const cost = parseFloat($costInput.val()) || 0;
        
        if (price > 0 && cost > 0) {
            const margin = ((price - cost) / price * 100);
            $marginDisplay.text(margin.toFixed(2) + '%');
            
            // Show margin section (for create form)
            if ($marginSection.length) {
                $marginSection.show();
            }
            
            // Show margin parent (for edit form)
            const $marginParent = $marginDisplay.parent();
            $marginParent.removeClass('d-none').show();
            
            // Add color coding based on margin percentage
            if (margin < 10) {
                $marginParent.removeClass('alert-info alert-success').addClass('alert-warning');
            } else if (margin < 30) {
                $marginParent.removeClass('alert-warning alert-success').addClass('alert-info');
            } else {
                $marginParent.removeClass('alert-warning alert-info').addClass('alert-success');
            }
        } else {
            // Hide margin displays when no valid calculation
            if ($marginSection.length) {
                $marginSection.hide();
            }
            $marginDisplay.parent().hide();
        }
    },

    // Clean up form when modal is closed
    cleanupForm: function(container) {
        const $container = $(container);
        
        // Destroy Select2 instances
        this.select2.destroyAll($container);
        
        // Remove event handlers
        $container.find('input[name="Price"], input[name="Cost"]').off('input.marginCalc');
    },

    // Get current product ID (for editing context)
    getCurrentProductId: function() {
        // First try to get from global config (most reliable in tab context)
        if (this.config.currentProductId) {
            return this.config.currentProductId;
        }

        // Try to get from current edit form
        const editForm = $('form[id*="EditProduct"], form[id*="editProduct"]');
        if (editForm.length) {
            const productId = editForm.find('input[name="Id"]').val();
            if (productId) return productId;
        }

        // Try to get from product-edit-container data attribute
        const productContainer = $('.product-edit-container');
        if (productContainer.length) {
            const productId = productContainer.data('product-id') || productContainer.attr('data-product-id');
            if (productId) return productId;
        }

        // Try to get from active tab data attribute
        const activeTab = $('.nav-link.active[data-product-id]');
        if (activeTab.length) {
            const productId = activeTab.data('product-id');
            if (productId) return productId;
        }

        // Try to get from URL if we're on edit page
        const urlParts = window.location.pathname.split('/');
        const editIndex = urlParts.indexOf('FormularioEdicao');
        if (editIndex > -1 && urlParts.length > editIndex + 1) {
            return urlParts[editIndex + 1];
        }

        return null;
    },

    // ProductGroup functionality moved to ProductGroup.js
    // Include ProductGroup.js in your view to use ProductGroup methods
};

// ProductGroup functions now available in ProductGroup.js

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
$(function() {
    productManager.init();
}); 