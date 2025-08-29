// ===================================
// ORDER MANAGER - GesN (TypeScript)
// Migração 1:1 do Order.js mantendo compatibilidade total
// ===================================
class OrderManager {
    constructor() {
        // ===================================
        // PROPRIEDADES EXATAS DO JS ORIGINAL
        // ===================================
        this.contador = 0;
        this.qtdAbasAbertas = 0;
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
                0: 80, // Número
                1: 150, // Cliente
                2: 90, // Data
                3: 90, // Entrega
                4: 70, // Tipo
                5: 100, // Valor Total
                6: 80, // Status
                7: 120 // Ações
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
    }
    // ===================================
    // MÉTODOS PRINCIPAIS (ASSINATURA IDÊNTICA)
    // ===================================
    carregarListaOrders() {
        const $container = $(this.config.containerSelector);
        $container.html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $.ajax({
            url: `${this.config.baseUrl}/Grid`,
            type: 'GET',
            success: (data) => {
                $container.html(data);
                // Inicializa DataTables após carregar o conteúdo
                this.inicializarDataTable();
            },
            error: () => {
                $container.html('<div class="alert alert-danger">Erro ao carregar a lista de pedidos.</div>');
            }
        });
    }
    inicializarDataTable() {
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
                    const dataTableConfig = {
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
                }
                catch (error) {
                    console.error('Erro ao inicializar DataTable:', error);
                }
            }, 100);
        }
    }
    novoOrderModal() {
        const $modal = $(this.config.modalSelector);
        $modal.find('.modal-title').text('Novo Pedido');
        $modal.find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
        $modal.find('.modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $modal.modal('show');
        $.get(`${this.config.baseUrl}/CreatePartial`)
            .done((data) => {
            $modal.find('.modal-body').html(data);
            this.inicializarAutocompleteCustomer($modal);
            this.inicializarFloatingLabels($modal);
        })
            .fail(() => {
            $modal.find('.modal-body').html('<div class="alert alert-danger">Erro ao carregar formulário</div>');
        });
    }
    // ✅ MÉTODO REFATORADO: Autocomplete Customer seguindo padrão da referência
    inicializarAutocompleteCustomer(container) {
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
        const autocompleteInstance = autocomplete(customerNameField.get(0), {
            hint: this.autocompleteConfig.hint,
            debug: this.autocompleteConfig.debug,
            minLength: this.autocompleteConfig.minLength,
            openOnFocus: this.autocompleteConfig.openOnFocus,
            autoselect: this.autocompleteConfig.autoselect,
            appendTo: container.get(0) // ✅ CRUCIAL: Container correto
        }, [{
                source: (query, callback) => {
                    $.ajax({
                        url: this.autocompleteConfig.endpoint,
                        type: 'GET',
                        dataType: 'json',
                        data: { termo: query },
                        success: (data) => {
                            const suggestions = $.map(data, (item) => {
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
                    suggestion: (suggestion) => {
                        return '<div class="autocomplete-suggestion">' +
                            '<div class="suggestion-title">' + (suggestion.data.value || suggestion.value) + '</div>' +
                            (suggestion.data.phone ? '<div class="suggestion-subtitle">' + suggestion.data.phone + '</div>' : '') +
                            '</div>';
                    }
                }
            }]);
        // ✅ EVENT HANDLERS: Seleção
        autocompleteInstance.on('autocomplete:selected', (event, suggestion, dataset) => {
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
    inicializarFloatingLabels(container) {
        container.find(`.${this.floatingLabelConfig.inputClass}, .autocomplete-input`).each((index, element) => {
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
                }
                else {
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
    inicializarAutocomplete(container) {
        return this.inicializarAutocompleteCustomer(container);
    }
    salvarNovoModal() {
        const form = $(`${this.config.modalSelector} .modal-body form`);
        if (form.length === 0) {
            console.error('Formulário não encontrado no modal');
            return Promise.reject(new Error('Formulário não encontrado'));
        }
        const formData = new FormData(form.get(0));
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
                if (response.success && response.data) {
                    $(this.config.modalSelector).modal('hide');
                    this.showToast('success', response.message || 'Pedido criado com sucesso!');
                    if (response.data.id) {
                        // Chama o método de edição passando também o numberSequence
                        this.abrirEdicao(response.data.id, response.data.numberSequence);
                    }
                }
                else {
                    this.showToast('error', response.message || 'Não foi possível criar o pedido');
                }
            },
            error: (xhr, status, error) => {
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
    abrirEdicao(orderId, numberSequence) {
        // Verifica se a aba já existe usando o orderId como identificador
        const existingTabId = `order-${orderId}`;
        const existingTab = $(`#${existingTabId}-tab`);
        if (existingTab.length > 0) {
            // Se a aba já existe, apenas ativa ela
            const tabTrigger = new bootstrap.Tab(document.getElementById(`${existingTabId}-tab`));
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
            .done((data) => {
            $(`#conteudo-${tabId}`).html(data);
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
        const tabTrigger = new bootstrap.Tab(document.getElementById(`${tabId}-tab`));
        tabTrigger.show();
    }
    abrirDetalhes(orderId) {
        const $modal = $(this.config.modalSelector);
        $modal.find('.modal-title').text('Detalhes do Pedido');
        $modal.find('.modal-dialog').removeClass('modal-lg').addClass('modal-xl');
        $modal.find('.modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $modal.modal('show');
        $.get(`${this.config.baseUrl}/DetailsPartial/${orderId}`)
            .done((data) => {
            $modal.find('.modal-body').html(data);
        })
            .fail(() => {
            $modal.find('.modal-body').html('<div class="alert alert-danger">Erro ao carregar detalhes do pedido.</div>');
        });
    }
    fecharAba(tabId) {
        // Remove a aba e seu conteúdo
        $(`#${tabId}-tab`).parent().remove(); // Remove o <li> que contém o button
        $(`#${tabId}`).remove(); // Remove o conteúdo da aba
        this.qtdAbasAbertas--;
        // Se não há mais abas abertas, volta para a aba principal
        if (this.qtdAbasAbertas === 0) {
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab'));
            mainTab.show();
        }
        else {
            // Se ainda há abas abertas, ativa a última aba disponível
            const remainingTabs = $(`${this.config.tabsSelector} .nav-item button[data-order-id]`);
            if (remainingTabs.length > 0) {
                const lastTab = new bootstrap.Tab(remainingTabs.last().get(0));
                lastTab.show();
            }
        }
    }
    // Método para verificar se um pedido já está aberto
    isPedidoAberto(orderId) {
        return $(`#order-${orderId}-tab`).length > 0;
    }
    // Método para obter lista de pedidos abertos
    getPedidosAbertos() {
        const abertos = [];
        $(`${this.config.tabsSelector} button[data-order-id]`).each((index, element) => {
            const orderId = $(element).data('order-id');
            if (orderId) {
                abertos.push(orderId);
            }
        });
        return abertos;
    }
    exportarPedidos() {
        // Implementar funcionalidade de exportação
        this.showToast('info', 'Funcionalidade de exportação em desenvolvimento...');
    }
    excluirPedido(orderId, orderNumber) {
        if (confirm(`Tem certeza que deseja excluir o pedido ${orderNumber}?\n\nEsta ação não pode ser desfeita.`)) {
            $.ajax({
                url: `${this.config.baseUrl}/Delete/${orderId}`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                success: (response) => {
                    if (response.success) {
                        this.showToast('success', 'Pedido excluído com sucesso!');
                        // Recarrega a lista
                        this.carregarListaOrders();
                    }
                    else {
                        this.showToast('error', response.message || 'Erro ao excluir o pedido');
                    }
                },
                error: (xhr, status, error) => {
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
    inicializarRedimensionamento() {
        // Adiciona handles de redimensionamento aos headers
        $(`${this.config.gridSelector} thead th`).each((index, element) => {
            const $th = $(element);
            // Remove handles existentes
            $th.find('.resize-handle').remove();
            // Adiciona handle de redimensionamento
            const $handle = $('<div class="resize-handle"></div>');
            $th.append($handle);
            // Eventos para o handle
            $handle.on('mousedown', (e) => {
                e.preventDefault();
                this.iniciarRedimensionamento(index, e.pageX, $th.outerWidth() || 0);
            });
            // Duplo clique para auto-ajustar
            $handle.on('dblclick', (e) => {
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
    iniciarRedimensionamento(columnIndex, startX, startWidth) {
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
    redimensionarColuna(e) {
        if (!this.resizeConfig.isDragging || this.resizeConfig.currentColumn === null)
            return;
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
    finalizarRedimensionamento() {
        if (!this.resizeConfig.isDragging || this.resizeConfig.currentColumn === null)
            return;
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
    autoAjustarColuna(columnIndex) {
        const $th = $(`${this.config.gridSelector} thead th:eq(${columnIndex})`);
        const $cells = $(`${this.config.gridSelector} tbody td:nth-child(${columnIndex + 1})`);
        // Calcula a largura máxima necessária
        let maxWidth = ($th.text().length * 8) + 40; // Largura base do header
        $cells.each((index, element) => {
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
    salvarLargurasColunas() {
        const widths = {};
        $(`${this.config.gridSelector} thead th`).each((index, element) => {
            const width = $(element).outerWidth();
            if (width) {
                widths[index] = width + 'px';
            }
        });
        localStorage.setItem(this.resizeConfig.storageKey, JSON.stringify(widths));
    }
    restaurarLargurasColunas() {
        const savedWidths = localStorage.getItem(this.resizeConfig.storageKey);
        if (savedWidths) {
            try {
                const widths = JSON.parse(savedWidths);
                $(`${this.config.gridSelector} thead th`).each((index, element) => {
                    if (widths[index]) {
                        $(element).css('width', widths[index]);
                    }
                });
                // Aplica também nas células do tbody
                $(`${this.config.gridSelector} tbody tr`).each((trIndex, trElement) => {
                    $(trElement).find('td').each((tdIndex, tdElement) => {
                        if (widths[tdIndex]) {
                            $(tdElement).css('width', widths[tdIndex]);
                        }
                    });
                });
            }
            catch (e) {
                console.error('Erro ao restaurar larguras das colunas:', e);
            }
        }
    }
    aplicarRedimensionamento() {
        // Método chamado após DataTable redraw
        if ($(this.config.gridSelector).length > 0) {
            // Reaplica larguras salvas após redraw
            this.restaurarLargurasColunas();
        }
    }
    // ===================================
    // UTILITÁRIOS E HELPERS
    // ===================================
    showToast(type, message) {
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        }
        else {
            alert(message);
        }
    }
    getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val() || '';
    }
}
// ===================================
// COMPATIBILIDADE COM CÓDIGO EXISTENTE
// ===================================
// Instância global mantendo o mesmo nome do JS
const ordersManager = new OrderManager();
// Disponibilizar globalmente para manter compatibilidade
window.ordersManager = ordersManager;
// Auto-inicialização idêntica ao JS original
$(function () {
    ordersManager.carregarListaOrders();
    $(`${ordersManager['config'].modalSelector}`).on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');
        $(this).find('.modal-title').text('Pedido');
        $(this).find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
    });
});
export default OrderManager;
export { ordersManager };
//# sourceMappingURL=OrderManager.js.map