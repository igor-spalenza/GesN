// Ingredient.js - Gerenciamento de Ingredientes
var ingredientsManager = {
    qtdAbasAbertas: 0,
    
    // Inicialização
    init: function() {
        ingredientsManager.carregarListaIngredients();
        ingredientsManager.configurarEventos();
    },

    configurarEventos: function() {
        // Configurar eventos globais se necessário
        $(document).on('hidden.bs.modal', '#ingredientModal', function () {
            $(this).find('.modal-body').empty();
        });
    },

    // Carregar lista de ingredientes
    carregarListaIngredients: function() {
        $('#lista-ingredients-container').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: '/Ingredient/Grid',
            type: 'GET',
            success: function(data) {
                $('#lista-ingredients-container').html(data);
                ingredientsManager.inicializarDataTable();
            },
            error: function() {
                $('#lista-ingredients-container').html('<div class="alert alert-danger">Erro ao carregar lista de ingredientes</div>');
            }
        });
    },

    // Inicializar DataTable
    inicializarDataTable: function() {
        if ($.fn.DataTable.isDataTable('#ingredientsTable')) {
            $('#ingredientsTable').DataTable().destroy();
        }
        
        $('#ingredientsTable').DataTable({
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

    // Modal para novo ingrediente
    novoIngredientModal: function() {
        $.ajax({
            url: '/Ingredient/CreatePartial',
            type: 'GET',
            success: function(data) {
                $('#ingredientModal .modal-body').html(data);
                $('#ingredientModal .modal-title').text('Novo Ingrediente');
                $('#ingredientModal').modal('show');
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de criação');
            }
        });
    },

    // Salvar novo ingrediente
    salvarNovoIngredient: function(form) {
        if (!$(form).valid()) {
            return false;
        }

        var formData = $(form).serialize();
        
        $.ajax({
            url: '/Ingredient/Create',
            type: 'POST',
            data: formData,
            success: function(response) {
                if (response.success) {
                    $('#ingredientModal').modal('hide');
                    toastr.success('Ingrediente criado com sucesso!');
                    ingredientsManager.carregarListaIngredients();
                    
                    if (response.numberSequence) {
                        setTimeout(function() {
                            ingredientsManager.abrirEdicao(response.id, response.numberSequence);
                        }, 500);
                    }
                } else {
                    toastr.error(response.message || 'Erro ao criar ingrediente');
                }
            },
            error: function() {
                toastr.error('Erro ao salvar ingrediente');
            }
        });
        
        return false;
    },

    // Ver detalhes do ingrediente
    verDetalhes: function(ingredientId) {
        ingredientsManager.abrirDetalhes(ingredientId);
    },

    // Abrir detalhes em nova aba
    abrirDetalhes: function(ingredientId) {
        var tabId = 'ingredient-details-' + ingredientId;
        
        // Verificar se a aba já está aberta
        if ($('#' + tabId).length > 0) {
            var tab = new bootstrap.Tab(document.getElementById('tab-' + tabId));
            tab.show();
            return;
        }

        $.ajax({
            url: '/Ingredient/DetailsPartial/' + ingredientId,
            type: 'GET',
            success: function(data) {
                ingredientsManager.adicionarAba(tabId, 'Detalhes do Ingrediente', data, true);
            },
            error: function() {
                toastr.error('Erro ao carregar detalhes do ingrediente');
            }
        });
    },

    // Editar ingrediente
    editarIngredient: function(ingredientId) {
        ingredientsManager.abrirEdicao(ingredientId, 'Ingrediente');
    },

    // Abrir edição em nova aba
    abrirEdicao: function(ingredientId, ingredientName) {
        var tabId = 'ingredient-edit-' + ingredientId;
        
        // Verificar se a aba já está aberta
        if ($('#' + tabId).length > 0) {
            var tab = new bootstrap.Tab(document.getElementById('tab-' + tabId));
            tab.show();
            return;
        }

        $.ajax({
            url: '/Ingredient/EditPartial/' + ingredientId,
            type: 'GET',
            success: function(data) {
                ingredientsManager.adicionarAba(tabId, 'Editar: ' + ingredientName, data, false);
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de edição');
            }
        });
    },

    // Salvar edição do ingrediente
    salvarEdicaoIngredient: function(form) {
        if (!$(form).valid()) {
            return false;
        }

        var formData = $(form).serialize();
        
        $.ajax({
            url: '/Ingredient/Edit',
            type: 'POST',
            data: formData,
            success: function(response) {
                if (response.success) {
                    toastr.success('Ingrediente atualizado com sucesso!');
                    ingredientsManager.carregarListaIngredients();
                } else {
                    toastr.error(response.message || 'Erro ao atualizar ingrediente');
                }
            },
            error: function() {
                toastr.error('Erro ao salvar alterações');
            }
        });
        
        return false;
    },

    // Excluir ingrediente
    excluirIngredient: function(ingredientId, ingredientName) {
        if (confirm('Tem certeza que deseja excluir o ingrediente "' + ingredientName + '"?')) {
            $.ajax({
                url: '/Ingredient/Delete/' + ingredientId,
                type: 'POST',
                success: function(response) {
                    if (response.success) {
                        toastr.success('Ingrediente excluído com sucesso!');
                        ingredientsManager.carregarListaIngredients();
                        
                        // Fechar aba se estiver aberta
                        var tabId = 'ingredient-edit-' + ingredientId;
                        ingredientsManager.fecharAba(tabId);
                    } else {
                        toastr.error(response.message || 'Erro ao excluir ingrediente');
                    }
                },
                error: function() {
                    toastr.error('Erro ao excluir ingrediente');
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
                     (!isReadOnly ? ' <button type="button" class="btn btn-sm btn-outline-danger ms-2" onclick="ingredientsManager.fecharAba(\'' + tabId + '\')" title="Fechar aba">' +
                     '<i class="bi bi-x"></i></button>' : '') +
                     '</button></li>';
        
        $('#ingredientsTabs').append(tabNav);

        // Adicionar conteúdo da aba
        var tabContent = '<div class="tab-pane fade" id="' + tabId + '" role="tabpanel">' + conteudo + '</div>';
        $('#ingredientTabsContent').append(tabContent);

        // Ativar a nova aba
        var tab = new bootstrap.Tab(document.getElementById('tab-' + tabId));
        tab.show();

        // Incrementar contador
        ingredientsManager.qtdAbasAbertas++;
    },

    // Fechar aba
    fecharAba: function(tabId) {
        // Remover aba da navegação
        $('#tab-' + tabId).closest('li').remove();
        
        // Remover conteúdo da aba
        $('#' + tabId).remove();
        
        // Decrementar contador
        ingredientsManager.qtdAbasAbertas--;

        // Se não há mais abas abertas, volta para a aba principal
        if (ingredientsManager.qtdAbasAbertas === 0) {
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab'));
            mainTab.show();
        } else {
            // Ativar a primeira aba disponível
            var firstTab = $('#ingredientsTabs .nav-link').not('#main-tab').first();
            if (firstTab.length > 0) {
                var tab = new bootstrap.Tab(firstTab[0]);
                tab.show();
            }
        }
    },

    // Obter abas abertas
    obterAbasAbertas: function() {
        var abertas = [];
        $('#ingredientsTabs .nav-link').not('#main-tab').each(function() {
            var tabId = $(this).attr('data-bs-target').substring(1);
            abertas.push({
                id: tabId,
                titulo: $(this).text().trim()
            });
        });
        return abertas;
    },

    exportarIngredients: function() {
        // Implementar funcionalidade de exportação
        toastr.info('Funcionalidade de exportação em desenvolvimento...');
    },

    // Busca por ingredientes
    buscarIngredients: function(termo) {
        if (termo.length >= 2) {
            $.ajax({
                url: '/Ingredient/Search',
                type: 'GET',
                data: { searchTerm: termo },
                success: function(data) {
                    $('#lista-ingredients-container').html(data);
                    ingredientsManager.inicializarDataTable();
                },
                error: function() {
                    toastr.error('Erro ao buscar ingredientes');
                }
            });
        } else if (termo.length === 0) {
            ingredientsManager.carregarListaIngredients();
        }
    },

    // Funcionalidades específicas do Ingredient
    
    // Atualizar estoque
    atualizarEstoque: function(ingredientId) {
        $.ajax({
            url: '/Ingredient/UpdateStockPartial/' + ingredientId,
            type: 'GET',
            success: function(data) {
                $('#ingredientModal .modal-body').html(data);
                $('#ingredientModal .modal-title').text('Atualizar Estoque');
                $('#ingredientModal').modal('show');
            },
            error: function() {
                toastr.error('Erro ao carregar formulário de atualização de estoque');
            }
        });
    },

    // Salvar atualização de estoque
    salvarAtualizacaoEstoque: function(form) {
        if (!$(form).valid()) {
            return false;
        }

        var formData = $(form).serialize();
        
        $.ajax({
            url: '/Ingredient/UpdateStock',
            type: 'POST',
            data: formData,
            success: function(response) {
                if (response.success) {
                    $('#ingredientModal').modal('hide');
                    toastr.success('Estoque atualizado com sucesso!');
                    ingredientsManager.carregarListaIngredients();
                } else {
                    toastr.error(response.message || 'Erro ao atualizar estoque');
                }
            },
            error: function() {
                toastr.error('Erro ao salvar atualização de estoque');
            }
        });
        
        return false;
    },

    // Verificar ingredientes com estoque baixo
    verificarEstoqueBaixo: function() {
        $.ajax({
            url: '/Ingredient/LowStock',
            type: 'GET',
            success: function(data) {
                if (data.length > 0) {
                    var mensagem = 'Ingredientes com estoque baixo:\n';
                    data.forEach(function(item) {
                        mensagem += '- ' + item.name + ' (Atual: ' + item.currentStock + ', Mín: ' + item.minStock + ')\n';
                    });
                    alert(mensagem);
                } else {
                    toastr.success('Todos os ingredientes estão com estoque adequado!');
                }
            },
            error: function() {
                toastr.error('Erro ao verificar estoque baixo');
            }
        });
    }
};

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    if (typeof ingredientsManager !== 'undefined') {
        ingredientsManager.init();
    }
}); 