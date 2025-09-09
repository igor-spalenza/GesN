// ===================================
// ORDER MANAGER - GesN (TypeScript)
// Migração 1:1 do Order.js mantendo compatibilidade total
// ===================================

// Imports removidos - interfaces carregadas globalmente via script tags
// As interfaces estão definidas em arquivos separados que serão carregados antes

/// <reference path="../types/globals.d.ts" />

// ⚠️ Tipos removidos - já definidos em common.ts

class OrderManager {
    // ===================================
    // PROPRIEDADES EXATAS DO JS ORIGINAL
    // ===================================
    public contador: number = 0;
    public qtdAbasAbertas: number = 0;

    private config: OrderManagerConfig;
    private resizeConfig: ResizeConfig;
    private autocompleteConfig: AutocompleteConfig;
    private floatingLabelConfig: FloatingLabelConfig;

    constructor() {
        // Configuração idêntica ao JS original
        this.config = {
            baseUrl: '/Order',
            modalSelector: '#orderModal',
            gridSelector: '#ordersTable',
            containerSelector: '#lista-orders-container',
            tabsSelector: '#orderTabs',
            tabsContentSelector: '#orderTabsContent'
        };

        // Configuração de redimensionamento (exata do JS)
        this.resizeConfig = {
            minWidths: {
                0: 80,   // Número
                1: 150,  // Cliente
                2: 90,   // Data
                3: 90,   // Entrega
                4: 70,   // Tipo
                5: 100,  // Valor Total
                6: 80,   // Status
                7: 120   // Ações
            },
            storageKey: 'ordersTable_column_widths',
            isDragging: false,
            currentColumn: null,
            startX: 0,
            startWidth: 0
        };

        this.autocompleteConfig = {
            minLength: 2,
            endpoint: '/Customer/BuscaCustomerAutocomplete',
            hint: false,
            debug: false,
            openOnFocus: false,
            autoselect: true
        };

        this.floatingLabelConfig = {
            containerClass: 'floating-input-group',
            inputClass: 'floating-input',
            labelClass: 'floating-label',
            activeClass: 'has-value',
            errorClass: 'is-invalid'
        };

        // Inicializar managers
        this.initializeManagers();
        
        // Configurar event listeners para controle do catálogo
        this.setupCatalogIntegration();
    }

    /**
     * Inicializa managers necessários para o funcionamento do sistema
     */
    private initializeManagers(): void {
        try {
            // Inicializar OrderItemManager globalmente
            if (!window.orderItemManager) {
                window.orderItemManager = new OrderItemManager({
                    onItemAdded: (item: any) => {
                        console.log('✅ Item adicionado ao carrinho:', item);
                        // Atualizar display de totais se necessário
                    },
                    onItemsReloaded: () => {
                        console.log('🔄 Lista de itens recarregada');
                    },
                    onError: (error: any) => {
                        console.error('❌ Erro no OrderItemManager:', error);
                        // Mostrar toast de erro global
                        if (window.toastr) {
                            window.toastr.error(error.message || 'Erro no gerenciamento de itens');
                        }
                    }
                });
                console.log('✅ OrderItemManager inicializado');
            }
            
            // Configurar comunicação entre managers
            this.setupManagersCommunication();
            
            console.log('🏗️ Managers preparados e comunicação configurada');
            
        } catch (error) {
            console.error('❌ Erro ao inicializar managers:', error);
        }
    }

    /**
     * Configura comunicação entre ProductCatalogManager e OrderItemManager
     */
    private setupManagersCommunication(): void {
        try {
            // Aguardar ProductCatalogManager estar disponível
            if (typeof window.productCatalogManager !== 'undefined') {
                this.configureEventsBetweenManagers();
            } else {
                // Tentar novamente após um pequeno delay
                setTimeout(() => {
                    if (typeof window.productCatalogManager !== 'undefined') {
                        this.configureEventsBetweenManagers();
                    } else {
                        console.warn('⚠️ ProductCatalogManager não disponível para configurar eventos');
                    }
                }, 100);
            }
        } catch (error) {
            console.error('❌ Erro ao configurar comunicação entre managers:', error);
        }
    }

    /**
     * Configura eventos específicos entre os managers
     */
    private configureEventsBetweenManagers(): void {
        try {
            const catalogManager = window.productCatalogManager;
            const itemManager = window.orderItemManager;

            if (!catalogManager || !itemManager) {
                console.warn('⚠️ Managers não disponíveis para configurar eventos');
                return;
            }

            // ProductCatalogManager → OrderItemManager
            catalogManager.events.onSimpleProductSelected = async (productId: string, quantity: number, notes?: string) => {
                try {
                    const response = await itemManager.addSimpleItem(productId, quantity, { notes });
                    if (!response.success) {
                        console.error('Erro ao adicionar produto simples:', response.message);
                        toastr.error(response.message || 'Erro ao adicionar produto ao carrinho');
                    } else {
                        toastr.success('Produto adicionado ao carrinho com sucesso!');
                        // Recarregar lista de itens
                        await itemManager.reloadItems(itemManager.getCurrentOrderId());
                    }
                } catch (error) {
                    console.error('Erro no evento onSimpleProductSelected:', error);
                    toastr.error('Erro ao adicionar produto ao carrinho');
                }
            };

            catalogManager.events.onCompositeProductConfigured = async (productId: string, config: CompositeItemConfiguration[]) => {
                try {
                    const response = await itemManager.addCompositeItem(productId, 1, config);
                    if (!response.success) {
                        console.error('Erro ao adicionar produto composto:', response.message);
                    }
                } catch (error) {
                    console.error('Erro no evento onCompositeProductConfigured:', error);
                }
            };

            catalogManager.events.onGroupProductConfigured = async (productId: string, config: GroupItemConfiguration[]) => {
                try {
                    const response = await itemManager.addGroupItem(productId, 1, config);
                    if (!response.success) {
                        console.error('Erro ao adicionar grupo de produtos:', response.message);
                    }
                } catch (error) {
                    console.error('Erro no evento onGroupProductConfigured:', error);
                }
            };

            console.log('🔗 Comunicação entre managers configurada com sucesso');
            
        } catch (error) {
            console.error('❌ Erro ao configurar eventos entre managers:', error);
        }
    }

    /**
     * Configura integração com o catálogo lateral
     */
    private setupCatalogIntegration(): void {
        // Event listener para mudança de abas
        $(document).on('shown.bs.tab', 'button[data-bs-toggle="tab"]', async (e) => {
            const target = $(e.target);
            const targetId = target.data('bs-target') || target.attr('href');
            const orderId = target.data('order-id');
            
            if (typeof window.productCatalogManager !== 'undefined') {
                if (targetId === '#lista-orders') {
                    // Aba principal - esconder catálogo e botão toggle
                    window.productCatalogManager.hideCatalog();
                    $('#catalogToggleBtn').addClass('hidden');
                    console.log('🏠 Página inicial ativa - catálogo escondido');
                } else if (orderId) {
                    // Aba de edição - trocar contexto do catálogo e inicializar OrderItemManager
                    window.productCatalogManager.switchContext(orderId).then(() => {
                        console.log('📝 Contexto do catálogo alterado para pedido:', orderId);
                    }).catch((error: any) => {
                        console.error('❌ Erro ao trocar contexto:', error);
                    });
                    
                    // Inicializar OrderItemManager para este pedido
                    if (typeof window.orderItemManager !== 'undefined') {
                        window.orderItemManager.init(orderId);
                        console.log('🛒 OrderItemManager inicializado para pedido:', orderId);
                    }
                    
                    $('#catalogToggleBtn').removeClass('hidden');
                }
            }
        });

        // Event listener para escape key (fechar catálogo)
        $(document).on('keydown', (e) => {
            if (e.key === 'Escape' && typeof window.productCatalogManager !== 'undefined') {
                window.productCatalogManager.hideCatalog();
            }
        });

        console.log('🔗 Integração com catálogo configurada');
    }

    // ===================================
    // MÉTODOS PRINCIPAIS (ASSINATURA IDÊNTICA)
    // ===================================

    public carregarListaOrders(): void {
        const $container = $(this.config.containerSelector);
        $container.html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: `${this.config.baseUrl}/Grid`,
            type: 'GET',
            success: (data: string) => {
                $container.html(data);
                // Inicializa DataTables após carregar o conteúdo
                this.inicializarDataTable();
            },
            error: () => {
                $container.html('<div class="alert alert-danger">Erro ao carregar a lista de pedidos.</div>');
            }
        });
    }

    public inicializarDataTable(): void {
        // Verifica se a tabela existe antes de inicializar
        if ($(this.config.gridSelector).length > 0) {
            // Aguarda um pouco para garantir que o DOM está completamente carregado
            setTimeout(() => {
                try {
                    // Destrói instância existente se houver
                    if ($.fn.DataTable.isDataTable(this.config.gridSelector)) {
                        $(this.config.gridSelector).DataTable().destroy();
                    }
                    
                    // Configuração idêntica ao JS original
                    const dataTableConfig: OrderDataTableConfig = {
                        language: {
                            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                        },
                        responsive: false, // Desabilita responsive para funcionar com redimensionamento
                        pageLength: 25,
                        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
                        order: [[0, 'desc']], // Ordena por número do pedido decrescente
                        columnDefs: [
                            {
                                targets: [7], // Coluna de ações
                                orderable: false,
                                searchable: false
                            },
                            {
                                targets: [4, 6], // Colunas de tipo e status
                                searchable: true,
                                orderable: true
                            }
                        ],
                        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rtip',
                        drawCallback: () => {
                            // Reaplica tooltips após redraw da tabela
                            if (typeof $.fn.tooltip !== 'undefined') {
                                $('[title]').tooltip();
                            }
                            
                            // Reaplica redimensionamento após redraw
                            this.aplicarRedimensionamento();
                        }
                    };

                    // Inicializa o DataTable
                    $(this.config.gridSelector).DataTable(dataTableConfig);
                    
                    // Inicializa o sistema de redimensionamento
                    this.inicializarRedimensionamento();
                } catch (error) {
                    console.error('Erro ao inicializar DataTable:', error);
                }
            }, 100);
        }
    }

    public novoOrderModal(): void {
        const $modal = $(this.config.modalSelector);
        $modal.find('.modal-title').text('Novo Pedido');
        $modal.find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
        $modal.find('.modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $modal.modal('show');

        $.get(`${this.config.baseUrl}/CreatePartial`)
            .done((data: string) => {
                $modal.find('.modal-body').html(data);
                this.inicializarAutocompleteCustomer($modal);
                this.inicializarFloatingLabels($modal);
            })
            .fail(() => {
                $modal.find('.modal-body').html('<div class="alert alert-danger">Erro ao carregar formulário</div>');
            });
    }

    // ✅ MÉTODO REFATORADO: Autocomplete Customer seguindo padrão da referência
    public inicializarAutocompleteCustomer(container: JQuery): void {
        const customerNameField = container.find('#CustomerName');
        const customerIdField = container.find('#CustomerId');
        
        // ✅ VALIDAÇÃO: Verificar existência dos campos
        if (customerNameField.length === 0) {
            return;
        }

        // ✅ CLEANUP: Remove instância anterior se houver
        if (customerNameField.data('aaAutocomplete')) {
            customerNameField.autocomplete.destroy();
        }

        // ✅ ALGOLIA CONFIG: Configuração padrão
        const autocompleteInstance = autocomplete(customerNameField.get(0) as Element, {
            hint: this.autocompleteConfig.hint,
            debug: this.autocompleteConfig.debug,
            minLength: this.autocompleteConfig.minLength,
            openOnFocus: this.autocompleteConfig.openOnFocus,
            autoselect: this.autocompleteConfig.autoselect,
            appendTo: container.get(0) as Element // ✅ CRUCIAL: Container correto
        }, [{
            source: (query: string, callback: (suggestions: CustomerAutocompleteItem[]) => void) => {
                $.ajax({
                    url: this.autocompleteConfig.endpoint,
                    type: 'GET',
                    dataType: 'json',
                    data: { termo: query },
                    success: (data: any[]) => {
                        const suggestions: CustomerAutocompleteItem[] = $.map(data, (item: any) => {
                            return {
                                label: item.label,
                                value: item.value,
                                id: item.id,
                                phone: item.phone || '',
                                email: item.email || '',
                                data: item
                            };
                        });
                        callback(suggestions);
                    },
                    error: () => {
                        callback([]);
                    }
                });
            },
            displayKey: 'label',
            templates: {
                suggestion: (suggestion: CustomerAutocompleteItem): string => {
                    return '<div class="autocomplete-suggestion">' +
                           '<div class="suggestion-title">' + (suggestion.data?.value || suggestion.value) + '</div>' +
                           (suggestion.data?.phone ? '<div class="suggestion-subtitle">' + suggestion.data.phone + '</div>' : '') +
                           '</div>';
                }
            }
        }]);

        // ✅ EVENT HANDLERS: Seleção
        autocompleteInstance.on('autocomplete:selected', (event: any, suggestion: CustomerAutocompleteItem, dataset: any) => {
            customerIdField.val(suggestion.id);
            customerNameField.val(suggestion.value);
            
            // ✅ UI UPDATES: Atualizar floating label
            const $container = customerNameField.closest(`.${this.floatingLabelConfig.containerClass}`);
            if ($container.length) {
                $container.addClass(this.floatingLabelConfig.activeClass);
            }
            
            // Trigger change para validação
            customerIdField.trigger('change');
        });

        // ✅ VALIDATION: Limpar seleção se campo ficar vazio
        customerNameField.on('blur', () => {
            if (customerNameField.val() === '') {
                customerIdField.val('');
                const $container = customerNameField.closest(`.${this.floatingLabelConfig.containerClass}`);
                if ($container.length) {
                    $container.removeClass(this.floatingLabelConfig.activeClass);
                }
                customerIdField.trigger('change');
            }
        });

        // ✅ INTEGRATION: Se já tem valor, marcar container como preenchido
        if (customerNameField.val()) {
            const $container = customerNameField.closest(`.${this.floatingLabelConfig.containerClass}`);
            if ($container.length) {
                $container.addClass(this.floatingLabelConfig.activeClass);
            }
        }

        // ✅ FLOATING LABEL: Inicializar comportamento das floating labels
        this.inicializarFloatingLabels(container);
    }

    // ✅ FLOATING LABELS: Inicializar comportamento
    public inicializarFloatingLabels(container: JQuery): void {
        container.find(`.${this.floatingLabelConfig.inputClass}, .autocomplete-input`).each((index: number, element: Element) => {
            const $input = $(element);
            const $container = $input.closest(`.${this.floatingLabelConfig.containerClass}`);
            
            // Marcar como preenchido se já tem valor
            if ($input.val() && String($input.val()).trim() !== '') {
                $input.addClass(this.floatingLabelConfig.activeClass);
                $container.addClass(this.floatingLabelConfig.activeClass);
            }

            // Event listeners para controlar a classe has-value
            $input.on('input blur', () => {
                if ($input.val() && String($input.val()).trim() !== '') {
                    $input.addClass(this.floatingLabelConfig.activeClass);
                    $container.addClass(this.floatingLabelConfig.activeClass);
                } else {
                    $input.removeClass(this.floatingLabelConfig.activeClass);  
                    $container.removeClass(this.floatingLabelConfig.activeClass);
                }
            });

            $input.on('focus', () => {
                $container.addClass('focused');
            });

            $input.on('blur', () => {
                $container.removeClass('focused');
            });
        });
    }

    // ✅ COMPATIBILIDADE: Alias para manter compatibilidade com código existente
    public inicializarAutocomplete(container: JQuery): void {
        return this.inicializarAutocompleteCustomer(container);
    }

    public salvarNovoModal(): Promise<void> {
        const form = $(`${this.config.modalSelector} .modal-body form`);
        if (form.length === 0) {
            console.error('Formulário não encontrado no modal');
            return Promise.reject(new Error('Formulário não encontrado'));
        }

        const formData = new FormData(form.get(0) as HTMLFormElement);

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
                    success: async (response: OrderSaveResponse) => {
            // 🔍 DEBUG: Vamos analisar a resposta real
            console.log('=== DEBUG RESPONSE ===');
            console.log('Resposta completa:', response);
            console.log('response.success:', response.success, typeof response.success);
            console.log('response.id:', response.id);
            console.log('response.numberSequence:', response.numberSequence);
            console.log('response.message:', response.message);
            console.log('=====================');
            
            if (response.success) {
                $(this.config.modalSelector).modal('hide');
                this.showToast('success', response.message || 'Pedido criado com sucesso!');

                console.log('✅ Abrindo edição com:', response.id, response.numberSequence);
                
                if (response.id) {
                    // Chama o método de edição passando ID e numberSequence
                    this.abrirEdicao(response.id, response.numberSequence);
                } else {
                    console.log('❌ ID não encontrado no response');
                }
            } else {
                console.log('❌ Success = false - mostrando erro');
                this.showToast('error', response.message || 'Não foi possível criar o pedido');
            }
            },
            error: (xhr: JQueryXHR, status: string, error: string) => {
                console.error('Erro ao salvar pedido:', error);
                const errorMessage = xhr.responseJSON?.message || 'Ocorreu um erro ao salvar o pedido. Por favor, tente novamente.';
                this.showToast('error', errorMessage);
            },
            complete: () => {
                // Reabilita o botão
                submitButton.prop('disabled', false).text(buttonText);
            }
        });
    }

    // Wrapper público para uso em onclick HTML (não async)
    public abrirEdicao(orderId: string | number, numberSequence?: string): void {
        this.abrirEdicaoAsync(orderId, numberSequence).catch(error => {
            console.error('❌ Erro ao abrir edição:', error);
            this.showToast('error', 'Erro ao abrir edição do pedido');
        });
    }
    
    // Implementação async interna
    public async abrirEdicaoAsync(orderId: string | number, numberSequence?: string): Promise<void> {
        // Verifica se a aba já existe usando o orderId como identificador
        const existingTabId = `order-${orderId}`;
        const existingTab = $(`#${existingTabId}-tab`);
        
        if (existingTab.length > 0) {
            // Se a aba já existe, apenas ativa ela
            const tabTrigger = new bootstrap.Tab(document.getElementById(`${existingTabId}-tab`)!);
            tabTrigger.show();
            this.showToast('info', 'Pedido já está aberto em outra aba');
            return;
        }

        // Se não existe, cria nova aba
        this.contador++;
        this.qtdAbasAbertas++;
        const tabId = existingTabId; // Usa o orderId como base do ID
        
        // Se numberSequence não foi fornecido, usa um placeholder que será atualizado
        const tabTitle = numberSequence || 'Carregando...';
        
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab" data-order-id="${orderId}">
                    ${tabTitle}
                    <span class="btn-close ms-2" onclick="ordersManager.fecharAba('${tabId}')"></span>
                </button>
            </li>`;
        $(this.config.tabsSelector).append(novaAba);
        
        const novoConteudo = `
            <div class="main-div tab-pane fade" id="${tabId}" role="tabpanel">
                <div id="conteudo-${tabId}">
                    <div class="d-flex justify-content-center my-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Carregando...</span>
                        </div>
                    </div>
                </div>
            </div>`;
        $(this.config.tabsContentSelector).append(novoConteudo);
        
        // Carrega o conteúdo da aba
        $.get(`${this.config.baseUrl}/EditPartial/${orderId}`)
            .done(async (data: string) => {
                $(`#conteudo-${tabId}`).html(data);
                
                // Mostrar botão toggle do catálogo (contexto já foi configurado no event listener)
                $('#catalogToggleBtn').removeClass('hidden');
                console.log('Aba carregada para pedido:', orderId);
                
                // Inicializar OrderItemManager para este pedido
                if (typeof window.orderItemManager !== 'undefined') {
                    window.orderItemManager.init(orderId);
                    console.log('🛒 OrderItemManager inicializado para pedido:', orderId);
                }
                
                // Se numberSequence não foi fornecido, extrai do conteúdo carregado
                if (!numberSequence) {
                    const numberSequenceElement = $(`#conteudo-${tabId}`).find('[data-number-sequence]');
                    if (numberSequenceElement.length > 0) {
                        const extractedNumberSequence = numberSequenceElement.data('number-sequence');
                        if (extractedNumberSequence) {
                            $(`#${tabId}-tab`).html(`${extractedNumberSequence} <span class="btn-close ms-2" onclick="ordersManager.fecharAba('${tabId}')"></span>`);
                        }
                    }
                }
            })
            .fail(() => {
                $(`#conteudo-${tabId}`).html('<div class="alert alert-danger">Erro ao carregar pedido. Tente novamente.</div>');
            });
            
        // Ativa a nova aba
        const tabTrigger = new bootstrap.Tab(document.getElementById(`${tabId}-tab`)!);
        tabTrigger.show();
    }

    public abrirDetalhes(orderId: string | number): void {
        const $modal = $(this.config.modalSelector);
        $modal.find('.modal-title').text('Detalhes do Pedido');
        $modal.find('.modal-dialog').removeClass('modal-lg').addClass('modal-xl');
        $modal.find('.modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $modal.modal('show');

        $.get(`${this.config.baseUrl}/DetailsPartial/${orderId}`)
            .done((data: string) => {
                $modal.find('.modal-body').html(data);
            })
            .fail(() => {
                $modal.find('.modal-body').html('<div class="alert alert-danger">Erro ao carregar detalhes do pedido.</div>');
            });
    }

    public fecharAba(tabId: string): void {
        // Extrair orderId do tabId para limpeza de estado
        const orderId = tabId.replace('order-', '');
        
        // Limpar estado salvo do catálogo para esta aba
        if (typeof window.productCatalogManager !== 'undefined' && orderId) {
            window.productCatalogManager.clearStateForOrder(orderId);
            console.log('Estado do catálogo limpo para pedido:', orderId);
        }
        
        // Remove a aba e seu conteúdo
        $(`#${tabId}-tab`).parent().remove(); // Remove o <li> que contém o button
        $(`#${tabId}`).remove(); // Remove o conteúdo da aba
        
        this.qtdAbasAbertas--;
        
        // Se não há mais abas abertas, volta para a aba principal e esconde catálogo
        if (this.qtdAbasAbertas === 0) {
            // Esconder catálogo
            if (typeof window.productCatalogManager !== 'undefined') {
                window.productCatalogManager.hideCatalog();
            }
            
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab')!);
            mainTab.show();
        } else {
            // Se ainda há abas abertas, ativa a última aba disponível
            const remainingTabs = $(`${this.config.tabsSelector} .nav-item button[data-order-id]`);
            if (remainingTabs.length > 0) {
                const lastTab = new bootstrap.Tab(remainingTabs.last().get(0) as Element);
                lastTab.show();
            }
        }
    }

    // Método para verificar se um pedido já está aberto
    public isPedidoAberto(orderId: string | number): boolean {
        return $(`#order-${orderId}-tab`).length > 0;
    }

    // Método para obter lista de pedidos abertos
    public getPedidosAbertos(): (string | number)[] {
        const abertos: (string | number)[] = [];
        $(`${this.config.tabsSelector} button[data-order-id]`).each((index: number, element: Element) => {
            const orderId = $(element).data('order-id') as (string | number);
            if (orderId) {
                abertos.push(orderId);
            }
        });
        return abertos;
    }

    public exportarPedidos(): void {
        // Implementar funcionalidade de exportação
        this.showToast('info', 'Funcionalidade de exportação em desenvolvimento...');
    }

    public excluirPedido(orderId: string | number, orderNumber: string): void {
        if (confirm(`Tem certeza que deseja excluir o pedido ${orderNumber}?\n\nEsta ação não pode ser desfeita.`)) {
            $.ajax({
                url: `${this.config.baseUrl}/Delete/${orderId}`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                success: (response: ApiResponse) => {
                    if (response.success) {
                        this.showToast('success', 'Pedido excluído com sucesso!');
                        // Recarrega a lista
                        this.carregarListaOrders();
                    } else {
                        this.showToast('error', response.message || 'Erro ao excluir o pedido');
                    }
                },
                error: (xhr: JQueryXHR, status: string, error: string) => {
                    console.error('Erro ao excluir pedido:', error);
                    const errorMessage = xhr.responseJSON?.message || 'Erro ao excluir pedido. Tente novamente.';
                    this.showToast('error', errorMessage);
                }
            });
        }
    }

    // ========================================
    // SISTEMA DE REDIMENSIONAMENTO DE COLUNAS
    // (Mantido exatamente igual ao JS original)
    // ========================================

    public inicializarRedimensionamento(): void {
        // Adiciona handles de redimensionamento aos headers
        $(`${this.config.gridSelector} thead th`).each((index: number, element: Element) => {
            const $th = $(element);
            
            // Remove handles existentes
            $th.find('.resize-handle').remove();
            
            // Adiciona handle de redimensionamento
            const $handle = $('<div class="resize-handle"></div>');
            $th.append($handle);
            
            // Eventos para o handle
            $handle.on('mousedown', (e: any) => {
                e.preventDefault();
                this.iniciarRedimensionamento(index, e.pageX, $th.outerWidth() || 0);
            });
            
            // Duplo clique para auto-ajustar
            $handle.on('dblclick', (e: any) => {
                e.preventDefault();
                this.autoAjustarColuna(index);
            });
        });
        
        // Eventos globais para redimensionamento
        $(document).on('mousemove', this.redimensionarColuna.bind(this));
        $(document).on('mouseup', this.finalizarRedimensionamento.bind(this));
        
        // Restaura larguras salvas
        this.restaurarLargurasColunas();
    }

    public iniciarRedimensionamento(columnIndex: number, startX: number, startWidth: number): void {
        this.resizeConfig.isDragging = true;
        this.resizeConfig.currentColumn = columnIndex;
        this.resizeConfig.startX = startX;
        this.resizeConfig.startWidth = startWidth;
        
        // Adiciona classe visual ao header
        $(`${this.config.gridSelector} thead th:eq(${columnIndex})`).addClass('resizing');
        
        // Desabilita seleção de texto
        $('body').addClass('user-select-none');
        
        // Cursor global
        $('body').css('cursor', 'col-resize');
    }

    public redimensionarColuna(e: any): void {
        if (!this.resizeConfig.isDragging || this.resizeConfig.currentColumn === null) return;
        
        const columnIndex = this.resizeConfig.currentColumn;
        const deltaX = e.pageX - this.resizeConfig.startX;
        const newWidth = this.resizeConfig.startWidth + deltaX;
        const minWidth = this.resizeConfig.minWidths[columnIndex] || 50;
        
        // Aplica largura mínima
        const finalWidth = Math.max(newWidth, minWidth);
        
        // Aplica a nova largura
        $(`${this.config.gridSelector} thead th:eq(${columnIndex})`).css('width', finalWidth + 'px');
        $(`${this.config.gridSelector} tbody td:nth-child(${columnIndex + 1})`).css('width', finalWidth + 'px');
    }

    public finalizarRedimensionamento(): void {
        if (!this.resizeConfig.isDragging || this.resizeConfig.currentColumn === null) return;
        
        const columnIndex = this.resizeConfig.currentColumn;
        
        // Remove classe visual
        $(`${this.config.gridSelector} thead th:eq(${columnIndex})`).removeClass('resizing');
        
        // Restaura cursor e seleção
        $('body').css('cursor', 'default').removeClass('user-select-none');
        
        // Salva larguras
        this.salvarLargurasColunas();
        
        // Reset das configurações
        this.resizeConfig.isDragging = false;
        this.resizeConfig.currentColumn = null;
    }

    public autoAjustarColuna(columnIndex: number): void {
        const $th = $(`${this.config.gridSelector} thead th:eq(${columnIndex})`);
        const $cells = $(`${this.config.gridSelector} tbody td:nth-child(${columnIndex + 1})`);
        
        // Calcula a largura máxima necessária
        let maxWidth = ($th.text().length * 8) + 40; // Largura base do header
        
        $cells.each((index: number, element: Element) => {
            const cellContent = $(element).text().trim();
            const cellWidth = (cellContent.length * 8) + 20; // Estimativa baseada no texto
            maxWidth = Math.max(maxWidth, cellWidth);
        });
        
        // Aplica largura mínima
        const minWidth = this.resizeConfig.minWidths[columnIndex] || 50;
        const finalWidth = Math.max(maxWidth, minWidth);
        
        // Aplica a nova largura
        $th.css('width', finalWidth + 'px');
        $cells.css('width', finalWidth + 'px');
        
        // Salva as larguras
        this.salvarLargurasColunas();
        
        this.showToast('success', 'Coluna ajustada automaticamente!');
    }

    public salvarLargurasColunas(): void {
        const widths: ColumnWidths = {};
        
        $(`${this.config.gridSelector} thead th`).each((index: number, element: Element) => {
            const width = $(element).outerWidth();
            if (width) {
                widths[index] = width + 'px';
            }
        });
        
        localStorage.setItem(this.resizeConfig.storageKey, JSON.stringify(widths));
    }

    public restaurarLargurasColunas(): void {
        const savedWidths = localStorage.getItem(this.resizeConfig.storageKey);
        
        if (savedWidths) {
            try {
                const widths: ColumnWidths = JSON.parse(savedWidths);
                
                $(`${this.config.gridSelector} thead th`).each((index: number, element: Element) => {
                    if (widths[index]) {
                        $(element).css('width', widths[index]);
                    }
                });
                
                // Aplica também nas células do tbody
                $(`${this.config.gridSelector} tbody tr`).each((trIndex: number, trElement: Element) => {
                    $(trElement).find('td').each((tdIndex: number, tdElement: Element) => {
                        if (widths[tdIndex]) {
                            $(tdElement).css('width', widths[tdIndex]);
                        }
                    });
                });
                
            } catch (e) {
                console.error('Erro ao restaurar larguras das colunas:', e);
            }
        }
    }

    public aplicarRedimensionamento(): void {
        // Método chamado após DataTable redraw
        if ($(this.config.gridSelector).length > 0) {
            // Reaplica larguras salvas após redraw
            this.restaurarLargurasColunas();
        }
    }

    // ===================================
    // UTILITÁRIOS E HELPERS
    // ===================================

    private showToast(type: 'success' | 'error' | 'info' | 'warning', message: string): void {
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        } else {
            alert(message);
        }
    }

    private getAntiForgeryToken(): string {
        return $('input[name="__RequestVerificationToken"]').val() as string || '';
    }
}

// ===================================
// COMPATIBILIDADE COM CÓDIGO EXISTENTE
// ===================================

// Instância global mantendo o mesmo nome do JS
const ordersManager = new OrderManager();

// Disponibilizar globalmente para manter compatibilidade
(window as any).ordersManager = ordersManager;

// Auto-inicialização idêntica ao JS original
$(function() {
    ordersManager.carregarListaOrders();

    $(`${ordersManager['config'].modalSelector}`).on('hidden.bs.modal', function (this: Element) {
        $(this).find('.modal-body').html('');
        $(this).find('.modal-title').text('Pedido');
        $(this).find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
    });
});

// Exports removidos - usando disponibilização global
