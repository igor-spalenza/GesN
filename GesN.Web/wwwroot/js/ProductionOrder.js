/**
 * JavaScript para gerenciamento de Ordens de Produção
 */
const ProductionOrder = {
    // Inicialização
    init: function() {
        this.initializeDataTable();
        this.bindEvents();
    },

    // Inicializar DataTable
    initializeDataTable: function() {
        if ($('#ordersTable').length > 0) {
            this.dataTable = $('#ordersTable').DataTable({
                "language": {
                    "url": "/lib/datatables.net/pt-br.json"
                },
                "order": [[5, "asc"], [3, "asc"]],
                "pageLength": 25,
                "columnDefs": [
                    { "orderable": false, "targets": [7] }
                ]
            });
        }
    },

    // Bind de eventos
    bindEvents: function() {
        // Filtros
        $('#statusFilter, #priorityFilter').on('change', () => {
            this.applyFilters();
        });

        $('#searchInput').on('keyup', () => {
            if (this.dataTable) {
                this.dataTable.search($('#searchInput').val()).draw();
            }
        });

        $('#clearFilters').on('click', () => {
            this.clearFilters();
        });
    },

    // Aplicar filtros
    applyFilters: function() {
        if (!this.dataTable) return;
        
        const statusFilter = $('#statusFilter').val();
        const priorityFilter = $('#priorityFilter').val();
        
        this.dataTable.column(3).search(statusFilter).draw();
        this.dataTable.column(4).search(priorityFilter).draw();
    },

    // Limpar filtros
    clearFilters: function() {
        $('#statusFilter, #priorityFilter').val('');
        $('#searchInput').val('');
        
        if (this.dataTable) {
            this.dataTable.search('').columns().search('').draw();
        }
    },

    // Iniciar produção
    startProduction: function(orderId) {
        const assignedTo = prompt('Responsável pela produção:');
        if (assignedTo) {
            $.post('/ProductionOrder/StartProduction', {
                id: orderId,
                assignedTo: assignedTo
            }).done((result) => {
                if (result.success) {
                    this.showSuccess(result.message);
                    location.reload();
                } else {
                    this.showError(result.message);
                }
            }).fail(() => {
                this.showError('Erro ao iniciar produção');
            });
        }
    },

    // Completar produção
    completeProduction: function(orderId) {
        const actualTime = prompt('Tempo real gasto (horas):');
        $.post('/ProductionOrder/CompleteProduction', {
            id: orderId,
            actualTime: actualTime
        }).done((result) => {
            if (result.success) {
                this.showSuccess(result.message);
                location.reload();
            } else {
                this.showError(result.message);
            }
        }).fail(() => {
            this.showError('Erro ao completar produção');
        });
    },

    // Pausar produção
    pauseProduction: function(orderId) {
        const reason = prompt('Motivo da pausa:');
        if (reason) {
            $.post('/ProductionOrder/PauseProduction', {
                id: orderId,
                reason: reason
            }).done((result) => {
                if (result.success) {
                    this.showSuccess(result.message);
                    location.reload();
                } else {
                    this.showError(result.message);
                }
            }).fail(() => {
                this.showError('Erro ao pausar produção');
            });
        }
    },

    // Resumir produção
    resumeProduction: function(orderId) {
        $.post('/ProductionOrder/ResumeProduction', {
            id: orderId
        }).done((result) => {
            if (result.success) {
                this.showSuccess(result.message);
                location.reload();
            } else {
                this.showError(result.message);
            }
        }).fail(() => {
            this.showError('Erro ao resumir produção');
        });
    },

    // Mostrar mensagem de sucesso
    showSuccess: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.success(message);
        } else {
            alert(message);
        }
    },

    // Mostrar mensagem de erro
    showError: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.error(message);
        } else {
            alert(message);
        }
    }
};

// Funções globais para compatibilidade com onclick eventos
window.startProduction = function(orderId) {
    ProductionOrder.startProduction(orderId);
};

window.completeProduction = function(orderId) {
    ProductionOrder.completeProduction(orderId);
};

window.pauseProduction = function(orderId) {
    ProductionOrder.pauseProduction(orderId);
};

window.resumeProduction = function(orderId) {
    ProductionOrder.resumeProduction(orderId);
};

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    if (typeof window.ProductionOrder === 'undefined') {
        window.ProductionOrder = ProductionOrder;
    }
    
    // Auto-inicializar se estivermos na página de ordens de produção
    if ($('#ordersTable').length > 0) {
        ProductionOrder.init();
    }
}); 