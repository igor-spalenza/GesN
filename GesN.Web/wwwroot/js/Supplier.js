// Supplier.js - Gerenciamento de Fornecedores
var suppliersManager = {
    qtdAbasAbertas: 0,
    
    // Inicialização
    init: function() {
        suppliersManager.carregarListaSuppliers();
        suppliersManager.configurarEventos();
    },

    configurarEventos: function() {
        // Configurar eventos globais se necessário
        $(document).on('hidden.bs.modal', '#supplierModal', function () {
            $(this).find('.modal-body').empty();
        });
    },

    // Carregar lista de fornecedores
    carregarListaSuppliers: function() {
        $('#lista-suppliers-container').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: '/Supplier/Grid',
            type: 'GET',
            success: function(data) {
                $('#lista-suppliers-container').html(data);
                suppliersManager.inicializarDataTable();
            },
            error: function() {
                $('#lista-suppliers-container').html('<div class="alert alert-danger">Erro ao carregar lista de fornecedores</div>');
            }
        });
    },

    // Inicializar DataTable
    inicializarDataTable: function() {
        if ($.fn.DataTable.isDataTable('#suppliersTable')) {
            $('#suppliersTable').DataTable().destroy();
        }
        
        $('#suppliersTable').DataTable({
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
            },
            responsive: true,
            pageLength: 10,
            order: [[0, 'asc']],
            columnDefs: [
                { targets: [-1], orderable: false }
            ]
        });
    },

    // Modal para novo fornecedor
    novoSupplierModal: function() {
        $.ajax({
            url: '/Supplier/CreatePartial',
            type: 'GET',
            success: function(data) {
                $('#supplierModal .modal-body').html(data);
                $('#supplierModal .modal-title').text('Novo Fornecedor');
                $('#supplierModal').modal('show');
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de criação');
            }
        });
    },

    // Salvar novo fornecedor
    salvarNovoSupplier: function(form) {
        if (!$(form).valid()) {
            return false;
        }

        var formData = $(form).serialize();
        
        $.ajax({
            url: '/Supplier/Create',
            type: 'POST',
            data: formData,
            success: function(response) {
                if (response.success) {
                    $('#supplierModal').modal('hide');
                    toastr.success('Fornecedor criado com sucesso!');
                    suppliersManager.carregarListaSuppliers();
                    
                    if (response.numberSequence) {
                        setTimeout(function() {
                            suppliersManager.abrirEdicao(response.id, response.numberSequence);
                        }, 500);
                    }
                } else {
                    toastr.error(response.message || 'Erro ao criar fornecedor');
                }
            },
            error: function() {
                toastr.error('Erro ao salvar fornecedor');
            }
        });
        
        return false;
    },

    // Ver detalhes do fornecedor
    verDetalhes: function(supplierId) {
        suppliersManager.abrirDetalhes(supplierId);
    },

    // Abrir detalhes em nova aba
    abrirDetalhes: function(supplierId) {
        var tabId = 'supplier-details-' + supplierId;
        
        // Verificar se a aba já está aberta
        if ($('#' + tabId).length > 0) {
            var tab = new bootstrap.Tab(document.getElementById('tab-' + tabId));
            tab.show();
            return;
        }

        $.ajax({
            url: '/Supplier/DetailsPartial/' + supplierId,
            type: 'GET',
            success: function(data) {
                suppliersManager.adicionarAba(tabId, 'Detalhes do Fornecedor', data, true);
            },
            error: function() {
                toastr.error('Erro ao carregar detalhes do fornecedor');
            }
        });
    },

    // Editar fornecedor
    editarSupplier: function(supplierId) {
        suppliersManager.abrirEdicao(supplierId, 'Fornecedor');
    },

    // Abrir edição em nova aba
    abrirEdicao: function(supplierId, supplierName) {
        var tabId = 'supplier-edit-' + supplierId;
        
        // Verificar se a aba já está aberta
        if ($('#' + tabId).length > 0) {
            var tab = new bootstrap.Tab(document.getElementById('tab-' + tabId));
            tab.show();
            return;
        }

        $.ajax({
            url: '/Supplier/EditPartial/' + supplierId,
            type: 'GET',
            success: function(data) {
                suppliersManager.adicionarAba(tabId, 'Editar: ' + supplierName, data, false);
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de edição');
            }
        });
    },

    // Salvar edição do fornecedor
    salvarEdicaoSupplier: function(form) {
        if (!$(form).valid()) {
            return false;
        }

        var formData = $(form).serialize();
        
        $.ajax({
            url: '/Supplier/Edit',
            type: 'POST',
            data: formData,
            success: function(response) {
                if (response.success) {
                    toastr.success('Fornecedor atualizado com sucesso!');
                    suppliersManager.carregarListaSuppliers();
                } else {
                    toastr.error(response.message || 'Erro ao atualizar fornecedor');
                }
            },
            error: function() {
                toastr.error('Erro ao salvar alterações');
            }
        });
        
        return false;
    },

    // Excluir fornecedor
    excluirSupplier: function(supplierId, supplierName) {
        if (confirm('Tem certeza que deseja excluir o fornecedor "' + supplierName + '"?')) {
            $.ajax({
                url: '/Supplier/Delete/' + supplierId,
                type: 'POST',
                success: function(response) {
                    if (response.success) {
                        toastr.success('Fornecedor excluído com sucesso!');
                        suppliersManager.carregarListaSuppliers();
                        
                        // Fechar aba se estiver aberta
                        var tabId = 'supplier-edit-' + supplierId;
                        suppliersManager.fecharAba(tabId);
                    } else {
                        toastr.error(response.message || 'Erro ao excluir fornecedor');
                    }
                },
                error: function() {
                    toastr.error('Erro ao excluir fornecedor');
                }
            });
        }
    },

    // Adicionar nova aba
    adicionarAba: function(tabId, titulo, conteudo, isReadOnly) {
        // Adicionar aba na navegação
        var tabNav = '<li class="nav-item" role="presentation">' +
                     '<button class="nav-link" id="tab-' + tabId + '" data-bs-toggle="tab" data-bs-target="#' + tabId + '" type="button" role="tab">' +
                     titulo +
                     (!isReadOnly ? ' <button type="button" class="btn btn-sm btn-outline-danger ms-2" onclick="suppliersManager.fecharAba(\'' + tabId + '\')" title="Fechar aba">' +
                     '<i class="bi bi-x"></i></button>' : '') +
                     '</button></li>';
        
        $('#suppliersTabs').append(tabNav);

        // Adicionar conteúdo da aba
        var tabContent = '<div class="tab-pane fade" id="' + tabId + '" role="tabpanel">' + conteudo + '</div>';
        $('#supplierTabsContent').append(tabContent);

        // Ativar a nova aba
        var tab = new bootstrap.Tab(document.getElementById('tab-' + tabId));
        tab.show();

        // Incrementar contador
        suppliersManager.qtdAbasAbertas++;
    },

    // Fechar aba
    fecharAba: function(tabId) {
        // Remover aba da navegação
        $('#tab-' + tabId).closest('li').remove();
        
        // Remover conteúdo da aba
        $('#' + tabId).remove();
        
        // Decrementar contador
        suppliersManager.qtdAbasAbertas--;

        // Se não há mais abas abertas, volta para a aba principal
        if (suppliersManager.qtdAbasAbertas === 0) {
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab'));
            mainTab.show();
        } else {
            // Ativar a primeira aba disponível
            var firstTab = $('#suppliersTabs .nav-link').not('#main-tab').first();
            if (firstTab.length > 0) {
                var tab = new bootstrap.Tab(firstTab[0]);
                tab.show();
            }
        }
    },

    // Obter abas abertas
    obterAbasAbertas: function() {
        var abertas = [];
        $('#suppliersTabs .nav-link').not('#main-tab').each(function() {
            var tabId = $(this).attr('data-bs-target').substring(1);
            abertas.push({
                id: tabId,
                titulo: $(this).text().trim()
            });
        });
        return abertas;
    },

    exportarSuppliers: function() {
        // Implementar funcionalidade de exportação
        toastr.info('Funcionalidade de exportação em desenvolvimento...');
    },

    // Busca por fornecedores
    buscarSuppliers: function(termo) {
        if (termo.length >= 2) {
            $.ajax({
                url: '/Supplier/Search',
                type: 'GET',
                data: { searchTerm: termo },
                success: function(data) {
                    $('#lista-suppliers-container').html(data);
                    suppliersManager.inicializarDataTable();
                },
                error: function() {
                    toastr.error('Erro ao buscar fornecedores');
                }
            });
        } else if (termo.length === 0) {
            suppliersManager.carregarListaSuppliers();
        }
    }
};

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    if (typeof suppliersManager !== 'undefined') {
        suppliersManager.init();
    }
}); 