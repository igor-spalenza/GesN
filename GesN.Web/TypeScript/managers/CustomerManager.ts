// ===================================
// CUSTOMER MANAGER - GesN (TypeScript)
// ===================================

// Imports removidos - interfaces carregadas globalmente via script tags
// As interfaces estão definidas em arquivos separados que serão carregados antes

// ⚠️ Tipos removidos - já definidos em common.ts

class CustomerManager {
    private config: CustomerManagerConfig;
    private state: CustomerManagerState;

    constructor() {
        this.config = {
            baseUrl: '/Customer',
            modalSelector: '#clienteModal',
            gridSelector: '#customersTable',
            maxNameLength: 100,
            documentMasks: {
                'CPF': '000.000.000-00',
                'CNPJ': '00.000.000/0000-00'
            }
        };

        this.state = {
            currentEditingId: null,
            lastLoadedData: null,
            validationErrors: {},
            isFormDirty: false
        };
    }

    // ===================================
    // INICIALIZAÇÃO
    // ===================================
    public init(): void {
        this.configurarAutoInicializacao();
        this.configurarEventos();
        this.inicializarDataTable();
    }

    private configurarAutoInicializacao(): void {
        // Auto-detectar e inicializar formulários
        const $formNovo = $('#formNovoCliente');
        if ($formNovo.length > 0) {
            this.aplicarMascaras($formNovo);
        }

        const $formEditar = $('#formEditarCliente');
        if ($formEditar.length > 0) {
            this.aplicarMascaras($formEditar);
        }
    }

    private configurarEventos(): void {
        // Delegação de eventos para elementos dinâmicos
        $(document).on('change', 'select[name="DocumentType"]', (event: any) => {
            this.alterarTipoDocumento($(event.target));
        });

        // Detectar mudanças no formulário
        $(document).on('input change', '.customer-form input, .customer-form select', () => {
            this.state.isFormDirty = true;
        });
    }

    // ===================================
    // GERENCIAMENTO DE MODAL
    // ===================================
    public async novoClienteModal(): Promise<void> {
        try {
            const modalConfig: ModalConfig = {
                selector: this.config.modalSelector,
                title: 'Novo Cliente',
                size: 'lg'
            };

            const formHtml = await this.request<string>(
                `${this.config.baseUrl}/FormularioCriacao`
            );

            this.abrirModal(modalConfig, formHtml);
            this.aplicarMascaras($('#formNovoCliente'));
            this.state.currentEditingId = null;
            this.state.isFormDirty = false;

        } catch (error) {
            this.handleError('Erro ao carregar formulário de criação', error);
        }
    }

    public async editarCliente(id: number): Promise<void> {
        if (!this.validarId(id)) {
            throw new ValidationError('ID de cliente inválido');
        }

        try {
            const modalConfig: ModalConfig = {
                selector: this.config.modalSelector,
                title: 'Editar Cliente',
                size: 'lg'
            };

            const formHtml = await this.request<string>(
                `${this.config.baseUrl}/FormularioEdicao/${id}`
            );

            this.abrirModal(modalConfig, formHtml);
            this.aplicarMascaras($('#formEditarCliente'));
            this.state.currentEditingId = id;
            this.state.isFormDirty = false;

        } catch (error) {
            this.handleError('Erro ao carregar formulário de edição', error);
        }
    }

    public async verDetalhes(id: number): Promise<void> {
        if (!this.validarId(id)) {
            throw new ValidationError('ID de cliente inválido');
        }

        try {
            const modalConfig: ModalConfig = {
                selector: this.config.modalSelector,
                title: 'Detalhes do Cliente',
                size: 'xl'
            };

            const detalhesHtml = await this.request<string>(
                `${this.config.baseUrl}/DetalhesCliente/${id}`
            );

            this.abrirModal(modalConfig, detalhesHtml);

        } catch (error) {
            this.handleError('Erro ao carregar detalhes do cliente', error);
        }
    }

    private abrirModal(config: ModalConfig, content: string): void {
        const $modal = $(config.selector);
        if (!$modal.length) {
            throw new Error(`Modal ${config.selector} não encontrado`);
        }

        $modal.find('.modal-title').text(config.title);
        $modal.find('.modal-body').html(content);
        
        if (config.size) {
            $modal.find('.modal-dialog')
                .removeClass('modal-sm modal-md modal-lg modal-xl')
                .addClass(`modal-${config.size}`);
        }

        $modal.modal('show');
    }

    // ===================================
    // OPERAÇÕES CRUD
    // ===================================
    public async salvarNovoCliente(): Promise<CustomerSaveResponse> {
        const $form = $('#formNovoCliente');
        if (!$form.length) {
            throw new Error('Formulário não encontrado');
        }

        const formData = this.extrairDadosFormulario($form);
        
        if (!this.validarFormulario(formData)) {
            throw new ValidationError('Dados do formulário inválidos', this.state.validationErrors);
        }

        try {
            const $btn = $form.find('button[onclick*="salvarNovoCliente"]');
            this.setButtonLoading($btn, true, 'Salvando...');

            const response = await this.request<ApiResponse<CustomerSaveResponse>>(
                `${this.config.baseUrl}/SalvarNovoCliente`,
                {
                    type: 'POST',
                    data: new FormData($form.get(0) as HTMLFormElement),
                    processData: false,
                    contentType: false
                }
            );

            if (!response.success || !response.data) {
                throw new ValidationError(
                    response.message || 'Erro ao salvar cliente',
                    response.errors
                );
            }

            // Fechar modal e recarregar lista
            $(this.config.modalSelector).modal('hide');
            this.showSuccess(response.message || 'Cliente criado com sucesso!');
            await this.carregarListaClientes();

            return response.data;

        } catch (error) {
            this.handleError('Erro ao salvar cliente', error);
            throw error;
        } finally {
            const $btn = $form.find('button[onclick*="salvarNovoCliente"]');
            this.setButtonLoading($btn, false, '<i class="fas fa-save"></i> Criar Cliente');
        }
    }

    public async salvarClienteEditado(clienteId: number): Promise<CustomerSaveResponse> {
        if (!this.validarId(clienteId)) {
            throw new ValidationError('ID de cliente inválido');
        }

        const $form = $('#formEditarCliente');
        if (!$form.length) {
            throw new Error('Formulário não encontrado');
        }

        const formData = this.extrairDadosFormulario($form);
        
        if (!this.validarFormulario(formData)) {
            throw new ValidationError('Dados do formulário inválidos', this.state.validationErrors);
        }

        try {
            const $btn = $form.find('button[onclick*="salvarClienteEditado"]');
            this.setButtonLoading($btn, true, 'Salvando...');

            const response = await this.request<ApiResponse<CustomerSaveResponse>>(
                `${this.config.baseUrl}/SalvarEdicaoCliente/${clienteId}`,
                {
                    type: 'POST',
                    data: new FormData($form.get(0) as HTMLFormElement),
                    processData: false,
                    contentType: false
                }
            );

            if (!response.success || !response.data) {
                throw new ValidationError(
                    response.message || 'Erro ao atualizar cliente',
                    response.errors
                );
            }

            $(this.config.modalSelector).modal('hide');
            this.showSuccess(response.message || 'Cliente atualizado com sucesso!');
            await this.carregarListaClientes();

            return response.data;

        } catch (error) {
            this.handleError('Erro ao salvar alterações', error);
            throw error;
        } finally {
            const $btn = $form.find('button[onclick*="salvarClienteEditado"]');
            this.setButtonLoading($btn, false, '<i class="fas fa-save"></i> Salvar Alterações');
        }
    }

    public async excluirCliente(id: number, nome: string): Promise<void> {
        if (!this.validarId(id)) {
            throw new ValidationError('ID de cliente inválido');
        }

        const confirmacao = confirm(`Tem certeza que deseja excluir o cliente "${nome}"?`);
        if (!confirmacao) {
            return;
        }

        try {
            const response = await this.request<ApiResponse>(
                `${this.config.baseUrl}/ExcluirCliente/${id}`,
                {
                    type: 'POST',
                    headers: {
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    }
                }
            );

            if (!response.success) {
                throw new ValidationError(response.message || 'Erro ao excluir cliente');
            }

            this.showSuccess(response.message || 'Cliente excluído com sucesso!');
            await this.carregarListaClientes();

        } catch (error) {
            this.handleError('Erro ao excluir cliente', error);
        }
    }

    // ===================================
    // LISTAGEM E FILTROS
    // ===================================
    public async carregarListaClientes(filtros?: CustomerFilters): Promise<void> {
        try {
            // Destruir DataTable existente
            if ($.fn.DataTable.isDataTable(this.config.gridSelector)) {
                $(this.config.gridSelector).DataTable().destroy();
            }

            const listaHtml = await this.request<string>(
                `${this.config.baseUrl}/ListaClientes`,
                filtros ? { type: 'GET', data: filtros } : undefined
            );

            $('#lista-clientes-container').html(listaHtml);
            this.inicializarDataTable();

        } catch (error) {
            this.handleError('Erro ao carregar lista de clientes', error);
        }
    }

    public async buscarClientes(termo: string): Promise<void> {
        if (!termo || termo.trim().length < 2) {
            await this.carregarListaClientes();
            return;
        }

        const filtros: CustomerFilters = {
            search: termo.trim()
        };

        await this.carregarListaClientes(filtros);
        this.showInfo(`Busca realizada para: "${termo}"`);
    }

    // ===================================
    // UTILITÁRIOS E HELPERS
    // ===================================
    private async request<T>(url: string, options: AjaxOptions = {}): Promise<T> {
        try {
            const response = await $.ajax({
                url,
                dataType: 'json',
                ...options
            }) as T;

            return response;
        } catch (error) {
            this.handleRequestError(error as JQueryXHR);
            throw error;
        }
    }

    private extrairDadosFormulario($form: JQuery): CustomerFormData {
        const formArray = $form.serializeArray();
        const formData: Partial<CustomerFormData> = {};

        formArray.forEach(field => {
            if (field.name === 'isActive') {
                (formData as any)[field.name] = field.value === 'true';
            } else {
                (formData as any)[field.name] = field.value;
            }
        });

        return formData as CustomerFormData;
    }

    private validarFormulario(data: CustomerFormData): boolean {
        this.state.validationErrors = {};
        let isValid = true;

        // Validação de nome
        if (!data.name || data.name.trim().length === 0) {
            this.state.validationErrors.name = ['Nome é obrigatório'];
            isValid = false;
        } else if (data.name.length > this.config.maxNameLength) {
            this.state.validationErrors.name = [`Nome deve ter no máximo ${this.config.maxNameLength} caracteres`];
            isValid = false;
        }

        // Validação de documento
        if (!data.documentNumber || data.documentNumber.trim().length === 0) {
            this.state.validationErrors.documentNumber = ['Documento é obrigatório'];
            isValid = false;
        }

        // Validação de email (se fornecido)
        if (data.email && !this.validarEmail(data.email)) {
            this.state.validationErrors.email = ['Email inválido'];
            isValid = false;
        }

        return isValid;
    }

    private validarEmail(email: string): boolean {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    private validarId(id: number): boolean {
        return Number.isInteger(id) && id > 0;
    }

    private aplicarMascaras($container: JQuery): void {
        // Máscara para telefone
        $container.find('input[name="Phone"]').mask('(00) 00000-0000', {
            placeholder: '(00) 00000-0000'
        });

        // Aplicar máscara inicial do documento
        const $documentType = $container.find('select[name="DocumentType"]');
        if ($documentType.length > 0) {
            this.alterarTipoDocumento($documentType);
        }
    }

    private alterarTipoDocumento($select: JQuery): void {
        const tipoDocumento = $select.val() as string;
        const $documentNumber = $select.closest('form').find('input[name="DocumentNumber"]');

        // Remove máscara anterior
        $documentNumber.unmask();

        const documentType: 'CPF' | 'CNPJ' = tipoDocumento === '0' || tipoDocumento === 'CPF' ? 'CPF' : 'CNPJ';
        const mask = this.config.documentMasks[documentType as keyof typeof this.config.documentMasks];

        $documentNumber.mask(mask, {
            placeholder: mask
        });
    }

    private inicializarDataTable(): void {
        if (!$(this.config.gridSelector).length) return;

        const config: DataTableConfig = {
            pageLength: 10,
            lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]],
            order: [[0, "asc"]],
            responsive: true,
            language: {
                url: "//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json"
            },
            columnDefs: [
                {
                    targets: -1,
                    orderable: false,
                    searchable: false
                }
            ]
        };

        $(this.config.gridSelector).DataTable(config);
    }

    private setButtonLoading($button: JQuery, loading: boolean, text: string): void {
        if (loading) {
            $button.prop('disabled', true)
                   .html(`<span class="spinner-border spinner-border-sm me-2"></span>${text}`);
        } else {
            $button.prop('disabled', false).html(text);
        }
    }

    private getAntiForgeryToken(): string {
        return $('input[name="__RequestVerificationToken"]').val() as string || '';
    }

    private handleError(message: string, error: any): void {
        console.error(message, error);
        
        if (error instanceof ValidationError && error.errors) {
            this.displayValidationErrors(error.errors);
        }
        
        this.showError(error.message || message);
    }

    private handleRequestError(error: JQueryXHR): void {
        const response = error.responseJSON as ApiResponse;
        
        if (response?.errors) {
            this.displayValidationErrors(response.errors);
        }
        
        const message = response?.message || 'Erro interno do servidor';
        this.showError(message);
    }

    private displayValidationErrors(errors: Record<string, string[]>): void {
        Object.entries(errors).forEach(([field, messages]) => {
            const $field = $(`[name="${field}"]`);
            if ($field.length && messages.length > 0) {
                $field.addClass('is-invalid');
                
                let $errorContainer = $field.siblings('.invalid-feedback');
                if (!$errorContainer.length) {
                    $errorContainer = $('<div class="invalid-feedback"></div>');
                    $field.after($errorContainer);
                }
                
                $errorContainer.text(messages[0]);
            }
        });
    }

    private showSuccess(message: string): void {
        if (typeof toastr !== 'undefined') {
            toastr.success(message);
        } else {
            alert(message);
        }
    }

    private showError(message: string): void {
        if (typeof toastr !== 'undefined') {
            toastr.error(message);
        } else {
            alert(message);
        }
    }

    private showInfo(message: string): void {
        if (typeof toastr !== 'undefined') {
            toastr.info(message);
        } else {
            alert(message);
        }
    }

    // ===================================
    // MÉTODOS PÚBLICOS (compatibilidade)
    // ===================================
    public filtrarPorStatus(status: string): void {
        const filtros: CustomerFilters = {
            status: status as any
        };
        this.carregarListaClientes(filtros);
    }

    public filtrarPorTipo(tipo: string): void {
        const filtros: CustomerFilters = {
            documentType: tipo as any
        };
        this.carregarListaClientes(filtros);
    }

    public visualizarCliente(id: number): void {
        this.verDetalhes(id);
    }

    public confirmarExclusao(id: number, nome: string): void {
        this.excluirCliente(id, nome);
    }
}

// Instância global para compatibilidade com código existente
const customerManager = new CustomerManager();

// Tornar disponível globalmente
(window as any).customerManager = customerManager;
(window as any).clientesManager = customerManager; // Alias para compatibilidade

// Auto-inicialização
$(function() {
    customerManager.init();
});

// Export removido - usando disponibilização global
