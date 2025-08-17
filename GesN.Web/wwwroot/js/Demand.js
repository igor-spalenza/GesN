/**
 * JavaScript para gerenciamento de Demandas
 */
const Demand = {
    // Inicialização
    init: function() {
        this.bindEvents();
        this.initializeDataTables();
    },

    // Bind de eventos
    bindEvents: function() {
        $(document).on('click', '.demand-edit-btn', this.showEditModal);
        $(document).on('click', '.demand-delete-btn', this.confirmDelete);
        $(document).on('click', '.demand-confirm-btn', this.confirmDemand);
        $(document).on('click', '.demand-produced-btn', this.markAsProduced);
        $(document).on('click', '.demand-ending-btn', this.markAsEnding);
        $(document).on('click', '.demand-delivered-btn', this.markAsDelivered);
        $(document).on('submit', '#createDemandForm', this.handleCreateSubmit);
        $(document).on('submit', '#editDemandForm', this.handleEditSubmit);
    },

    // Inicializar DataTables
    initializeDataTables: function() {
        if ($('#demandsTable').length > 0) {
            $('#demandsTable').DataTable({
                responsive: true,
                pageLength: 25,
                order: [[3, 'asc']], // Ordenar por data prevista
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                },
                columnDefs: [
                    {
                        targets: [-1], // Última coluna (ações)
                        orderable: false,
                        searchable: false
                    }
                ]
            });
        }
    },

    // Modal de criação
    showCreateModal: function() {
        $.get('/Demand/Create')
            .done(function(data) {
                $('#genericModalLabel').text('Nova Demanda');
                $('#genericModalBody').html(data);
                Demand.initializeForm();
                $('#genericModal').modal('show');
            })
            .fail(function() {
                toastr.error('Erro ao carregar formulário de criação');
            });
    },

    // Modal de edição
    showEditModal: function() {
        const demandId = $(this).data('id');
        
        $.get('/Demand/Edit/' + demandId)
            .done(function(data) {
                $('#genericModalLabel').text('Editar Demanda');
                $('#genericModalBody').html(data);
                Demand.initializeForm();
                $('#genericModal').modal('show');
            })
            .fail(function() {
                toastr.error('Erro ao carregar dados da demanda');
            });
    },

    // Inicializar formulário
    initializeForm: function() {
        // Máscaras
        $('.date-mask').mask('00/00/0000');
        $('.quantity-mask').mask('000.000.000,00', {reverse: true});

        // Select2 para dropdowns
        if ($.fn.select2) {
            $('.form-select').select2({
                dropdownParent: $('#genericModal'),
                theme: 'bootstrap-5'
            });
        }

        // Validação de formulário
        if ($.fn.validate) {
            $('#createDemandForm, #editDemandForm').validate({
                rules: {
                    OrderItemId: { required: true },
                    ProductId: { required: true },
                    Quantity: { required: true }
                },
                messages: {
                    OrderItemId: 'Item do pedido é obrigatório',
                    ProductId: 'Produto é obrigatório',
                    Quantity: 'Quantidade é obrigatória'
                },
                errorClass: 'is-invalid',
                validClass: 'is-valid',
                errorPlacement: function(error, element) {
                    error.addClass('invalid-feedback');
                    error.insertAfter(element);
                }
            });
        }
    },

    // Submit do formulário de criação
    handleCreateSubmit: function(e) {
        e.preventDefault();
        
        if ($(this).valid()) {
            const formData = $(this).serialize();
            
            $.post('/Demand/Create', formData)
                .done(function(response) {
                    if (response.success) {
                        $('#genericModal').modal('hide');
                        toastr.success(response.message);
                        Demand.refreshGrid();
                    } else {
                        toastr.error(response.message || 'Erro ao criar demanda');
                    }
                })
                .fail(function() {
                    toastr.error('Erro ao criar demanda');
                });
        }
    },

    // Submit do formulário de edição
    handleEditSubmit: function(e) {
        e.preventDefault();
        
        if ($(this).valid()) {
            const formData = $(this).serialize();
            
            $.post('/Demand/Edit', formData)
                .done(function(response) {
                    if (response.success) {
                        $('#genericModal').modal('hide');
                        toastr.success(response.message);
                        Demand.refreshGrid();
                    } else {
                        toastr.error(response.message || 'Erro ao atualizar demanda');
                    }
                })
                .fail(function() {
                    toastr.error('Erro ao atualizar demanda');
                });
        }
    },

    // Confirmar exclusão
    confirmDelete: function() {
        const demandId = $(this).data('id');
        const demandName = $(this).data('name');
        
        if (confirm(`Tem certeza que deseja excluir a demanda "${demandName}"?`)) {
            Demand.deleteDemand(demandId);
        }
    },

    // Excluir demanda
    deleteDemand: function(demandId) {
        $.post('/Demand/Delete', { id: demandId })
            .done(function(response) {
                if (response.success) {
                    toastr.success(response.message);
                    Demand.refreshGrid();
                } else {
                    toastr.error(response.message || 'Erro ao excluir demanda');
                }
            })
            .fail(function() {
                toastr.error('Erro ao excluir demanda');
            });
    },

    // Confirmar demanda
    confirmDemand: function() {
        const demandId = $(this).data('id');
        Demand.changeStatus(demandId, '/Demand/Confirm', 'Confirmar demanda');
    },

    // Marcar como produzido
    markAsProduced: function() {
        const demandId = $(this).data('id');
        Demand.changeStatus(demandId, '/Demand/MarkAsProduced', 'Marcar como produzido');
    },

    // Marcar como finalizando
    markAsEnding: function() {
        const demandId = $(this).data('id');
        Demand.changeStatus(demandId, '/Demand/MarkAsEnding', 'Marcar como finalizando');
    },

    // Marcar como entregue
    markAsDelivered: function() {
        const demandId = $(this).data('id');
        Demand.changeStatus(demandId, '/Demand/MarkAsDelivered', 'Marcar como entregue');
    },

    // Alterar status genérico
    changeStatus: function(demandId, url, action) {
        if (confirm(`Tem certeza que deseja ${action}?`)) {
            $.post(url, { id: demandId })
                .done(function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        Demand.refreshGrid();
                        Demand.updateDashboardStats();
                    } else {
                        toastr.error(response.message || `Erro ao ${action}`);
                    }
                })
                .fail(function() {
                    toastr.error(`Erro ao ${action}`);
                });
        }
    },

    // Atualizar data prevista
    updateExpectedDate: function(demandId, newDate) {
        $.post('/Demand/UpdateExpectedDate', { 
            id: demandId, 
            newExpectedDate: newDate 
        })
        .done(function(response) {
            if (response.success) {
                toastr.success(response.message);
                Demand.refreshGrid();
            } else {
                toastr.error(response.message || 'Erro ao atualizar data prevista');
            }
        })
        .fail(function() {
            toastr.error('Erro ao atualizar data prevista');
        });
    },

    // Atribuir a ordem de produção
    assignToProductionOrder: function(demandId, productionOrderId) {
        $.post('/Demand/AssignToProductionOrder', { 
            id: demandId, 
            productionOrderId: productionOrderId 
        })
        .done(function(response) {
            if (response.success) {
                toastr.success(response.message);
                Demand.refreshGrid();
            } else {
                toastr.error(response.message || 'Erro ao atribuir ordem de produção');
            }
        })
        .fail(function() {
            toastr.error('Erro ao atribuir ordem de produção');
        });
    },

    // Atualizar estatísticas do dashboard
    updateDashboardStats: function() {
        $.get('/Demand/GetDashboardStats')
            .done(function(data) {
                if (!data.error) {
                    $('.dashboard-pending').text(data.pending);
                    $('.dashboard-confirmed').text(data.confirmed);
                    $('.dashboard-produced').text(data.produced);
                    $('.dashboard-ending').text(data.ending);
                    $('.dashboard-delivered').text(data.delivered);
                    $('.dashboard-overdue').text(data.overdue);
                }
            });
    },

    // Atualizar grid
    refreshGrid: function() {
        if ($('#demandsTable').length > 0) {
            $('#demandsTable').DataTable().ajax.reload();
        } else {
            // Recarregar a partial view do grid
            $.get(window.location.href)
                .done(function(data) {
                    const newGrid = $(data).find('#demands-grid').html();
                    $('#demands-grid').html(newGrid);
                    Demand.initializeDataTables();
                });
        }
    },

    // Buscar demandas para autocomplete
    searchDemands: function(term, callback) {
        $.get('/Demand/Search', { term: term })
            .done(function(data) {
                callback(data);
            })
            .fail(function() {
                callback([]);
            });
    },

    // Utilidades
    formatDate: function(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('pt-BR');
    },

    formatDateTime: function(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleString('pt-BR');
    },

    getStatusBadgeClass: function(status) {
        const statusClasses = {
            'Pending': 'badge-warning',
            'Confirmed': 'badge-info',
            'Produced': 'badge-primary',
            'Ending': 'badge-secondary',
            'Delivered': 'badge-success'
        };
        return statusClasses[status] || 'badge-light';
    },

    // Alternar visibilidade dos filtros
    toggleFilters: function() {
        $('#filters-card').toggle();
    }
};

// Funções globais para compatibilidade com onclick eventos
window.toggleFilters = function() {
    Demand.toggleFilters();
};

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    if (typeof window.Demand === 'undefined') {
        window.Demand = Demand;
    }
    
    // Auto-inicializar se estivermos na página de demandas
    if ($('#demandsTable').length > 0) {
        Demand.init();
    }
}); 