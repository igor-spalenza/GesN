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
        // Verifica se a tabela existe antes de inicializar
        if ($('#suppliersTable').length > 0) {
            // Aguarda um pouco para garantir que o DOM está completamente carregado
            setTimeout(function() {
                try {
                    // Destrói instância existente se houver
                    if ($.fn.DataTable.isDataTable('#suppliersTable')) {
                        $('#suppliersTable').DataTable().destroy();
                    }
                    
                    $('#suppliersTable').DataTable({
                        language: {
                            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                        },
                        responsive: true,
                        pageLength: 10,
                        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
                        order: [[0, 'asc']],
                        columnDefs: [
                            {
                                targets: [6], // Coluna de ações (última coluna - índice 6)
                                orderable: false,
                                searchable: false
                            },
                            {
                                targets: [4], // Coluna de status
                                searchable: true,
                                orderable: true
                            }
                        ],
                        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rtip',
                        drawCallback: function() {
                            // Reaplica tooltips após redraw da tabela
                            if (typeof $.fn.tooltip !== 'undefined') {
                                $('[title]').tooltip();
                            }
                        }
                    });
                } catch (error) {
                    console.error('Erro ao inicializar DataTable:', error);
                }
            }, 100);
        }
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
                
                // Inicializar máscaras após o modal ser exibido
                $('#supplierModal').on('shown.bs.modal', function () {
                    suppliersManager.initializeFormMasks();
                });
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de criação');
            }
        });
    },

    // Salvar novo fornecedor
    salvarNovoSupplier: function(form) {

        
        // Validação simples dos campos obrigatórios
        var name = $(form).find('#Name').val();
        
        if (!name) {

            toastr.error('Por favor, preencha o nome do fornecedor');
            return false;
        }

        var formData = $(form).serialize();

        
        $.ajax({
            url: '/Supplier/Create',
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
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
            error: function(xhr, status, error) {
                console.error('AJAX error:', xhr, status, error);
                console.error('Response text:', xhr.responseText);
                toastr.error('Erro ao salvar fornecedor');
            }
        });
        
        return false;
    },

    // Ver detalhes do fornecedor
    verDetalhes: function(supplierId) {
        $('#supplierModal .modal-title').text('Detalhes do Fornecedor');
        $('#supplierModal .modal-dialog').removeClass('modal-lg').addClass('modal-xl');
        $('#supplierModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#supplierModal').modal('show');

        $.get(`/Supplier/DetailsPartial/${supplierId}`)
            .done(function (data) {
                $('#supplierModal .modal-body').html(data);
            })
            .fail(function () {
                $('#supplierModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar detalhes do fornecedor</div>');
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
                
                // Inicializar máscaras após adicionar a aba
                setTimeout(function() {
                    suppliersManager.initializeFormMasks();
                }, 100);
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
                     (!isReadOnly ? ' <span class="btn-close ms-2" onclick="suppliersManager.fecharAba(\'' + tabId + '\')" title="Fechar aba"></span>' : '') +
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
    },

    // Inicializar máscaras e validações de formulário
    initializeFormMasks: function() {
        // Máscara para telefone
        $('#Phone').mask('(00) 00000-0000');
        
        // Máscara para documento baseada no tipo
        $('#DocumentType').off('change.supplierMask').on('change.supplierMask', function() {
            var docType = $(this).val();
            var docInput = $('#DocumentNumber');
            
            docInput.unmask();
            
            if (docType === 'CPF') {
                docInput.mask('000.000.000-00');
            } else if (docType === 'CNPJ') {
                docInput.mask('00.000.000/0000-00');
            }
        });
        
        // Aplicar máscara inicial se já tiver tipo selecionado
        $('#DocumentType').trigger('change');
    }
};

// Inicializar quando o documento estiver pronto
$(function() {
    if (typeof suppliersManager !== 'undefined') {
        suppliersManager.init();
    }
}); 