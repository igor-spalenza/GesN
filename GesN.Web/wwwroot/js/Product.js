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
                $('#total-produtos').text(data.totalProducts);
                $('#produtos-ativos').text(data.activeProducts);
                $('#produtos-simples').text(data.simpleProducts);
                $('#produtos-compostos').text(data.compositeProducts);
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
                
                // Inicializar floating labels após carregar o conteúdo
                setTimeout(() => {
                    const formContainer = document.getElementById('formNovoProduct');
                    if (formContainer && this.forms) {
                        this.forms.initFloatingLabels(formContainer);
                    }
                }, 100);
                
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
                    
                    // Limpar o formulário para próximas criações
                    if (this.forms) {
                        this.forms.resetFormState(form[0]);
                    }
                    
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

    // Editar produto - abrir como aba
    editarProduct: function(productId) {
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
        
        // Cria a aba com nome temporário que será atualizado
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab" data-product-id="${productId}">
                    <i class="fas fa-edit"></i> Carregando...
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
                
                // Extrai o nome do produto do conteúdo carregado para atualizar o título da aba
                const productNameElement = $(`#conteudo-${tabId}`).find('input[name="Name"]');
                if (productNameElement.length > 0) {
                    const productName = productNameElement.val();
                    $(`#${tabId}-tab`).html(`<i class="fas fa-edit"></i> ${productName} <span class="btn-close ms-2" onclick="productManager.fecharAba('${tabId}')"></span>`);
                }
                
                // Inicializar gerenciamento de sub-abas e floating labels
                setTimeout(() => {
                    this.initializeEditTabs(productId, tabId);
                    // Inicializar floating labels no conteúdo carregado
                    const container = document.getElementById(`conteudo-${tabId}`);
                    if (container) {
                        this.forms.initFloatingLabels(container);
                    }
                }, 100);
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
                        this.components.carregarComponentes(productId);
                    }
                    break;
                case '#group-items-tab-pane':
                    if ($(`${containerSelector} #groupItemsContainer .spinner-border`).length > 0) {
                        this.groupItems.carregarItens(productId);
                    }
                    break;
                case '#group-options-tab-pane':
                    if ($(`${containerSelector} #groupOptionsContainer .spinner-border`).length > 0) {
                        this.groupOptions.carregarOpcoes(productId);
                    }
                    break;
                case '#exchange-rules-tab-pane':
                    if ($(`${containerSelector} #exchangeRulesContainer .spinner-border`).length > 0) {
                        this.exchangeRules.carregarRegras(productId);
                    }
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

    // Gerenciar grupo (para grupos de produtos)
    gerenciarGrupo: function(productId) {
        this.groupItems.abrirGerenciamento(productId);
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

    // Mostrar erros de validação para aba específica
    showValidationErrors: function(errors, tabId = null) {
        if (errors) {
            // Determinar o container correto
            let container = '';
            if (tabId) {
                container = `#conteudo-${tabId}`;
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
    },

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
            
            // Ensure all textareas maintain floating state
            $(container).find('.floating-textarea').addClass('has-value');
            
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
                
                // Textareas should always have floating labels
                if ($input.hasClass('floating-textarea')) {
                    $input.addClass('has-value');
                    // Still add event listeners for validation and other behaviors
                    $input.on('focus blur input change', function() {
                        // Don't remove has-value class for textareas, but check for other behaviors
                        if (!$(this).hasClass('has-value')) {
                            $(this).addClass('has-value');
                        }
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