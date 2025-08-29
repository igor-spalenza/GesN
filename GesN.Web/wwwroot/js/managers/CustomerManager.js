// ===================================
// CUSTOMER MANAGER - GesN (TypeScript)
// ===================================
import { ValidationError } from '../interfaces/common';
class CustomerManager {
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
    init() {
        this.configurarAutoInicializacao();
        this.configurarEventos();
        this.inicializarDataTable();
    }
    configurarAutoInicializacao() {
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
    configurarEventos() {
        // Delegação de eventos para elementos dinâmicos
        $(document).on('change', 'select[name="DocumentType"]', (event) => {
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
    async novoClienteModal() {
        try {
            const modalConfig = {
                selector: this.config.modalSelector,
                title: 'Novo Cliente',
                size: 'lg'
            };
            const formHtml = await this.request(`${this.config.baseUrl}/FormularioCriacao`);
            this.abrirModal(modalConfig, formHtml);
            this.aplicarMascaras($('#formNovoCliente'));
            this.state.currentEditingId = null;
            this.state.isFormDirty = false;
        }
        catch (error) {
            this.handleError('Erro ao carregar formulário de criação', error);
        }
    }
    async editarCliente(id) {
        if (!this.validarId(id)) {
            throw new ValidationError('ID de cliente inválido');
        }
        try {
            const modalConfig = {
                selector: this.config.modalSelector,
                title: 'Editar Cliente',
                size: 'lg'
            };
            const formHtml = await this.request(`${this.config.baseUrl}/FormularioEdicao/${id}`);
            this.abrirModal(modalConfig, formHtml);
            this.aplicarMascaras($('#formEditarCliente'));
            this.state.currentEditingId = id;
            this.state.isFormDirty = false;
        }
        catch (error) {
            this.handleError('Erro ao carregar formulário de edição', error);
        }
    }
    async verDetalhes(id) {
        if (!this.validarId(id)) {
            throw new ValidationError('ID de cliente inválido');
        }
        try {
            const modalConfig = {
                selector: this.config.modalSelector,
                title: 'Detalhes do Cliente',
                size: 'xl'
            };
            const detalhesHtml = await this.request(`${this.config.baseUrl}/DetalhesCliente/${id}`);
            this.abrirModal(modalConfig, detalhesHtml);
        }
        catch (error) {
            this.handleError('Erro ao carregar detalhes do cliente', error);
        }
    }
    abrirModal(config, content) {
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
    async salvarNovoCliente() {
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
            const response = await this.request(`${this.config.baseUrl}/SalvarNovoCliente`, {
                type: 'POST',
                data: new FormData($form.get(0)),
                processData: false,
                contentType: false
            });
            if (!response.success || !response.data) {
                throw new ValidationError(response.message || 'Erro ao salvar cliente', response.errors);
            }
            // Fechar modal e recarregar lista
            $(this.config.modalSelector).modal('hide');
            this.showSuccess(response.message || 'Cliente criado com sucesso!');
            await this.carregarListaClientes();
            return response.data;
        }
        catch (error) {
            this.handleError('Erro ao salvar cliente', error);
            throw error;
        }
        finally {
            const $btn = $form.find('button[onclick*="salvarNovoCliente"]');
            this.setButtonLoading($btn, false, '<i class="fas fa-save"></i> Criar Cliente');
        }
    }
    async salvarClienteEditado(clienteId) {
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
            const response = await this.request(`${this.config.baseUrl}/SalvarEdicaoCliente/${clienteId}`, {
                type: 'POST',
                data: new FormData($form.get(0)),
                processData: false,
                contentType: false
            });
            if (!response.success || !response.data) {
                throw new ValidationError(response.message || 'Erro ao atualizar cliente', response.errors);
            }
            $(this.config.modalSelector).modal('hide');
            this.showSuccess(response.message || 'Cliente atualizado com sucesso!');
            await this.carregarListaClientes();
            return response.data;
        }
        catch (error) {
            this.handleError('Erro ao salvar alterações', error);
            throw error;
        }
        finally {
            const $btn = $form.find('button[onclick*="salvarClienteEditado"]');
            this.setButtonLoading($btn, false, '<i class="fas fa-save"></i> Salvar Alterações');
        }
    }
    async excluirCliente(id, nome) {
        if (!this.validarId(id)) {
            throw new ValidationError('ID de cliente inválido');
        }
        const confirmacao = confirm(`Tem certeza que deseja excluir o cliente "${nome}"?`);
        if (!confirmacao) {
            return;
        }
        try {
            const response = await this.request(`${this.config.baseUrl}/ExcluirCliente/${id}`, {
                type: 'POST',
                headers: {
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });
            if (!response.success) {
                throw new ValidationError(response.message || 'Erro ao excluir cliente');
            }
            this.showSuccess(response.message || 'Cliente excluído com sucesso!');
            await this.carregarListaClientes();
        }
        catch (error) {
            this.handleError('Erro ao excluir cliente', error);
        }
    }
    // ===================================
    // LISTAGEM E FILTROS
    // ===================================
    async carregarListaClientes(filtros) {
        try {
            // Destruir DataTable existente
            if ($.fn.DataTable.isDataTable(this.config.gridSelector)) {
                $(this.config.gridSelector).DataTable().destroy();
            }
            const listaHtml = await this.request(`${this.config.baseUrl}/ListaClientes`, filtros ? { type: 'GET', data: filtros } : undefined);
            $('#lista-clientes-container').html(listaHtml);
            this.inicializarDataTable();
        }
        catch (error) {
            this.handleError('Erro ao carregar lista de clientes', error);
        }
    }
    async buscarClientes(termo) {
        if (!termo || termo.trim().length < 2) {
            await this.carregarListaClientes();
            return;
        }
        const filtros = {
            search: termo.trim()
        };
        await this.carregarListaClientes(filtros);
        this.showInfo(`Busca realizada para: "${termo}"`);
    }
    // ===================================
    // UTILITÁRIOS E HELPERS
    // ===================================
    async request(url, options = {}) {
        try {
            const response = await $.ajax({
                url,
                dataType: 'json',
                ...options
            });
            return response;
        }
        catch (error) {
            this.handleRequestError(error);
            throw error;
        }
    }
    extrairDadosFormulario($form) {
        const formArray = $form.serializeArray();
        const formData = {};
        formArray.forEach(field => {
            if (field.name === 'isActive') {
                formData[field.name] = field.value === 'true';
            }
            else {
                formData[field.name] = field.value;
            }
        });
        return formData;
    }
    validarFormulario(data) {
        this.state.validationErrors = {};
        let isValid = true;
        // Validação de nome
        if (!data.name || data.name.trim().length === 0) {
            this.state.validationErrors.name = ['Nome é obrigatório'];
            isValid = false;
        }
        else if (data.name.length > this.config.maxNameLength) {
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
    validarEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }
    validarId(id) {
        return Number.isInteger(id) && id > 0;
    }
    aplicarMascaras($container) {
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
    alterarTipoDocumento($select) {
        const tipoDocumento = $select.val();
        const $documentNumber = $select.closest('form').find('input[name="DocumentNumber"]');
        // Remove máscara anterior
        $documentNumber.unmask();
        const documentType = tipoDocumento === '0' || tipoDocumento === 'CPF' ? 'CPF' : 'CNPJ';
        const mask = this.config.documentMasks[documentType];
        $documentNumber.mask(mask, {
            placeholder: mask
        });
    }
    inicializarDataTable() {
        if (!$(this.config.gridSelector).length)
            return;
        const config = {
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
    setButtonLoading($button, loading, text) {
        if (loading) {
            $button.prop('disabled', true)
                .html(`<span class="spinner-border spinner-border-sm me-2"></span>${text}`);
        }
        else {
            $button.prop('disabled', false).html(text);
        }
    }
    getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val() || '';
    }
    handleError(message, error) {
        console.error(message, error);
        if (error instanceof ValidationError && error.errors) {
            this.displayValidationErrors(error.errors);
        }
        this.showError(error.message || message);
    }
    handleRequestError(error) {
        const response = error.responseJSON;
        if (response?.errors) {
            this.displayValidationErrors(response.errors);
        }
        const message = response?.message || 'Erro interno do servidor';
        this.showError(message);
    }
    displayValidationErrors(errors) {
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
    showSuccess(message) {
        if (typeof toastr !== 'undefined') {
            toastr.success(message);
        }
        else {
            alert(message);
        }
    }
    showError(message) {
        if (typeof toastr !== 'undefined') {
            toastr.error(message);
        }
        else {
            alert(message);
        }
    }
    showInfo(message) {
        if (typeof toastr !== 'undefined') {
            toastr.info(message);
        }
        else {
            alert(message);
        }
    }
    // ===================================
    // MÉTODOS PÚBLICOS (compatibilidade)
    // ===================================
    filtrarPorStatus(status) {
        const filtros = {
            status: status
        };
        this.carregarListaClientes(filtros);
    }
    filtrarPorTipo(tipo) {
        const filtros = {
            documentType: tipo
        };
        this.carregarListaClientes(filtros);
    }
    visualizarCliente(id) {
        this.verDetalhes(id);
    }
    confirmarExclusao(id, nome) {
        this.excluirCliente(id, nome);
    }
}
// Instância global para compatibilidade com código existente
const customerManager = new CustomerManager();
// Tornar disponível globalmente
window.customerManager = customerManager;
window.clientesManager = customerManager; // Alias para compatibilidade
// Auto-inicialização
$(function () {
    customerManager.init();
});
export default CustomerManager;
//# sourceMappingURL=CustomerManager.js.map