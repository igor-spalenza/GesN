/**
 * Gerenciador simples de operações CRUD para Clientes
 */
const clientesManager = {
    /**
     * Inicializa o gerenciador ao carregar a página
     */
    init: function() {
        this.configurarAutoInicializacao();
        this.configurarEventos();
        this.inicializarDataTable();
    },
    
    /**
     * Configura auto-inicialização baseada em elementos presentes na página
     */
    configurarAutoInicializacao: function() {
        // Auto-detectar e inicializar formulários de criação
        if ($('#formNovoCliente').length > 0) {
            this.aplicarMascaras($('#formNovoCliente'));
        }
        
        // Auto-detectar e inicializar formulários de edição
        if ($('#formEditarCliente').length > 0) {
            this.aplicarMascaras($('#formEditarCliente'));
        }
    },
    
    /**
     * Configura eventos globais
     */
    configurarEventos: function() {
        // Delegação de eventos para elementos dinâmicos
        $(document).on('change', 'select[name="DocumentType"]', function() {
            clientesManager.alterarTipoDocumento($(this));
        });
    },

    /**
     * Inicializa DataTable para a tabela de clientes
     */
    inicializarDataTable: function() {
        $(function() {
            if ($('#customersTable').length) {
                $('#customersTable').DataTable({
                    "language": {
                        "url": "//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json"
                    },
                    "responsive": true,
                    "pageLength": 10,
                    "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]],
                    "order": [[0, "asc"]],
                    "columnDefs": [
                        {
                            "targets": -1,
                            "orderable": false,
                            "searchable": false
                        }
                    ],
                    "dom": "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
                           "<'row'<'col-sm-12'tr>>" +
                           "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
                });
            }
        });
    },
    
    /**
     * Aplica máscaras nos campos do formulário
     */
    aplicarMascaras: function($container) {
        // Máscara para telefone
        $container.find('input[name="Phone"]').mask('(00) 00000-0000', {
            placeholder: '(00) 00000-0000'
        });
        
        // Aplicar máscara inicial do documento baseado no tipo selecionado
        const $documentType = $container.find('select[name="DocumentType"]');
        if ($documentType.length > 0) {
            this.alterarTipoDocumento($documentType);
        }
    },
    
    /**
     * Altera máscara do documento baseado no tipo selecionado
     */
    alterarTipoDocumento: function($select) {
        const tipoDocumento = $select.val();
        const $documentNumber = $select.closest('form').find('input[name="DocumentNumber"]');
        
        // Remove máscara anterior
        $documentNumber.unmask();
        
        if (tipoDocumento === '0' || tipoDocumento === 'CPF') { // CPF
            $documentNumber.mask('000.000.000-00', {
                placeholder: '000.000.000-00'
            });
        } else if (tipoDocumento === '1' || tipoDocumento === 'CNPJ') { // CNPJ
            $documentNumber.mask('00.000.000/0000-00', {
                placeholder: '00.000.000/0000-00'
            });
        }
    },
    
    /**
     * Abre modal para criar novo cliente
     */
    novoClienteModal: function() {
        $.ajax({
            url: '/Customer/FormularioCriacao',
            type: 'GET',
            success: function(data) {
                $('#clienteModal .modal-title').text('Novo Cliente');
                $('#clienteModal .modal-body').html(data);
                $('#clienteModal').modal('show');
                
                // Aplicar máscaras após carregar o formulário
                clientesManager.aplicarMascaras($('#formNovoCliente'));
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de criação');
            }
        });
    },

    /**
     * Abre modal para editar cliente
     */
    editarCliente: function(id) {
        $.ajax({
            url: `/Customer/FormularioEdicao/${id}`,
            type: 'GET',
            success: function(data) {
                $('#clienteModal .modal-title').text('Editar Cliente');
                $('#clienteModal .modal-body').html(data);
                $('#clienteModal').modal('show');
                
                // Aplicar máscaras após carregar o formulário
                clientesManager.aplicarMascaras($('#formEditarCliente'));
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de edição');
            }
        });
    },

    /**
     * Abre modal para ver detalhes do cliente
     */
    verDetalhes: function(id) {
        $.ajax({
            url: `/Customer/DetalhesCliente/${id}`,
            type: 'GET',
            success: function(data) {
                $('#clienteModal .modal-title').text('Detalhes do Cliente');
                $('#clienteModal .modal-body').html(data);
                $('#clienteModal').modal('show');
            },
            error: function() {
                toastr.error('Erro ao carregar detalhes do cliente');
            }
        });
    },

    /**
     * Carrega lista de clientes
     */
    carregarListaClientes: function() {
        $.ajax({
            url: '/Customer/ListaClientes',
            type: 'GET',
            success: function(data) {
                // Destruir DataTable existente se houver
                if ($.fn.DataTable.isDataTable('#customersTable')) {
                    $('#customersTable').DataTable().destroy();
                }
                
                $('#lista-clientes-container').html(data);
                
                // Reinicializar DataTable
                clientesManager.inicializarDataTable();
            },
            error: function() {
                toastr.error('Erro ao carregar lista de clientes');
            }
        });
    },

    /**
     * Filtra clientes por status
     */
    filtrarPorStatus: function(status) {
        $.ajax({
            url: '/Customer/ListaClientes',
            type: 'GET',
            data: { status: status },
            success: function(data) {
                // Destruir DataTable existente se houver
                if ($.fn.DataTable.isDataTable('#customersTable')) {
                    $('#customersTable').DataTable().destroy();
                }
                
                $('#lista-clientes-container').html(data);
                toastr.info(`Clientes filtrados por status: ${status}`);
                
                // Reinicializar DataTable
                clientesManager.inicializarDataTable();
            },
            error: function() {
                toastr.error('Erro ao filtrar clientes');
            }
        });
    },

    /**
     * Filtra clientes por tipo de documento
     */
    filtrarPorTipo: function(tipo) {
        $.ajax({
            url: '/Customer/ListaClientes',
            type: 'GET',
            data: { documentType: tipo },
            success: function(data) {
                // Destruir DataTable existente se houver
                if ($.fn.DataTable.isDataTable('#customersTable')) {
                    $('#customersTable').DataTable().destroy();
                }
                
                $('#lista-clientes-container').html(data);
                toastr.info(`Clientes filtrados por tipo: ${tipo}`);
                
                // Reinicializar DataTable
                clientesManager.inicializarDataTable();
            },
            error: function() {
                toastr.error('Erro ao filtrar clientes');
            }
        });
    },

    /**
     * Busca clientes baseado no termo digitado
     */
    buscarClientes: function() {
        const termo = $('#searchInput').val();
        
        $.ajax({
            url: '/Customer/BuscarClientes',
            type: 'GET',
            data: { termo: termo },
            success: function(data) {
                // Destruir DataTable existente se houver
                if ($.fn.DataTable.isDataTable('#customersTable')) {
                    $('#customersTable').DataTable().destroy();
                }
                
                $('#lista-clientes-container').html(data);
                toastr.info(`Busca realizada para: "${termo}"`);
                
                // Reinicializar DataTable
                clientesManager.inicializarDataTable();
            },
            error: function() {
                toastr.error('Erro ao buscar clientes');
            }
        });
    },

    /**
     * Visualizar detalhes do cliente
     */
    visualizarCliente: function(id) {
        this.verDetalhes(id);
    },

    /**
     * Exclui cliente (com confirmação)
     */
    excluirCliente: function(id, nome) {
        this.confirmarExclusao(id, nome);
    },

    /**
     * Exportar lista de clientes
     */
    exportarClientes: function() {
        // Funcionalidade para exportar - pode ser implementada depois
        toastr.info('Funcionalidade de exportação em desenvolvimento');
    },

    /**
     * Salva novo cliente
     */
    salvarNovoCliente: function() {
        const $form = $('#formNovoCliente');
        const $btn = $form.find('button[onclick*="salvarNovoCliente"]');
        
        if (!this.validarFormulario($form)) {
            return;
        }
        
        const formData = new FormData($form[0]);
        
        // Desabilitar botão durante salvamento
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2"></span>Salvando...');
        
        $.ajax({
            url: '/Customer/SalvarNovoCliente',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function(response) {
                if (response.success) {
                    toastr.success(response.message || 'Cliente criado com sucesso!');
                    $('#clienteModal').modal('hide');
                    clientesManager.carregarListaClientes();
                } else {
                    toastr.error(response.message || 'Erro ao salvar cliente');
                }
            },
            error: function(xhr, status, error) {
                console.error('Erro ao salvar cliente:', error);
                toastr.error('Erro interno do servidor');
            },
            complete: function() {
                $btn.prop('disabled', false).html('<i class="fas fa-save"></i> Criar Cliente');
            }
        });
    },

    /**
     * Salva cliente editado
     */
    salvarClienteEditado: function(clienteId) {
        const $form = $('#formEditarCliente');
        const $btn = $form.find('button[onclick*="salvarClienteEditado"]');
        
        if (!this.validarFormulario($form)) {
            return;
        }
        
        const formData = new FormData($form[0]);
        
        // Desabilitar botão durante salvamento
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2"></span>Salvando...');
        
        $.ajax({
            url: `/Customer/SalvarEdicaoCliente/${clienteId}`,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function(response) {
                if (response.success) {
                    toastr.success(response.message || 'Cliente atualizado com sucesso!');
                    $('#clienteModal').modal('hide');
                    clientesManager.carregarListaClientes();
                } else {
                    toastr.error(response.message || 'Erro ao atualizar cliente');
                }
            },
            error: function(xhr, status, error) {
                console.error('Erro ao salvar cliente:', error);
                toastr.error('Erro interno do servidor');
            },
            complete: function() {
                $btn.prop('disabled', false).html('<i class="fas fa-save"></i> Salvar Alterações');
            }
        });
    },
    
    /**
     * Valida formulário básico
     */
    validarFormulario: function($form) {
        let isValid = true;
        
        // Remover classes de erro anteriores
        $form.find('.is-invalid').removeClass('is-invalid');
        $form.find('.invalid-feedback').remove();
        
        // Validar campos obrigatórios
        $form.find('input[required], select[required]').each(function() {
            const $field = $(this);
            const value = $field.val();
            
            if (!value || value.trim() === '') {
                $field.addClass('is-invalid');
                $field.after('<div class="invalid-feedback">Este campo é obrigatório.</div>');
                isValid = false;
            }
        });
        
        // Validar email
        const $email = $form.find('input[name="Email"]');
        if ($email.length > 0 && $email.val()) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test($email.val())) {
                $email.addClass('is-invalid');
                $email.after('<div class="invalid-feedback">Email inválido.</div>');
                isValid = false;
            }
        }
        
        if (!isValid) {
            toastr.error('Por favor, corrija os erros no formulário');
        }
        
        return isValid;
    },
    
    /**
     * Confirma exclusão de cliente
     */
    confirmarExclusao: function(id, nome) {
        if (confirm(`Tem certeza que deseja excluir o cliente "${nome}"?`)) {
            this.excluirClienteDefinitivo(id);
        }
    },
    
    /**
     * Exclui cliente definitivamente
     */
    excluirClienteDefinitivo: function(id) {
        $.ajax({
            url: `/Customer/ExcluirCliente/${id}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response.success) {
                    toastr.success(response.message || 'Cliente excluído com sucesso!');
                    clientesManager.carregarListaClientes();
                } else {
                    toastr.error(response.message || 'Erro ao excluir cliente');
                }
            },
            error: function() {
                toastr.error('Erro interno do servidor');
            }
        });
    }
};

// Tornar clientesManager globalmente acessível
window.clientesManager = clientesManager;

// Auto-inicialização quando o DOM estiver pronto
$(function() {
    // Debug: verificar se o objeto clientesManager existe
    console.log('clientesManager carregado:', typeof clientesManager);
    console.log('novoClienteModal função existe:', typeof clientesManager.novoClienteModal);
    
    if (typeof clientesManager !== 'undefined') {
        console.log('Inicializando clientesManager...');
        clientesManager.init();
    } else {
        console.error('clientesManager não foi definido!');
    }
}); 

// Debug adicional - verificar se o objeto está acessível globalmente
window.debugClientesManager = function() {
    console.log('clientesManager global:', typeof window.clientesManager);
    console.log('clientesManager local:', typeof clientesManager);
    if (typeof clientesManager !== 'undefined') {
        console.log('Funções disponíveis:', Object.keys(clientesManager));
    }
}; 