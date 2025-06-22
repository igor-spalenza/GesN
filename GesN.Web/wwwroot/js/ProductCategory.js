const productCategoriesManager = {
    contador: 0,
    qtdAbasAbertas: 0,
    
    carregarListaCategorias: function() {
        $('#lista-categorias-container').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: '/ProductCategory/Grid',
            type: 'GET',
            success: function(data) {
                $('#lista-categorias-container').html(data);
                // Inicializa DataTables após carregar o conteúdo
                productCategoriesManager.inicializarDataTable();
            },
            error: function() {
                $('#lista-categorias-container').html('<div class="alert alert-danger">Erro ao carregar lista de categorias</div>');
            }
        });
    },

    inicializarDataTable: function() {
        // Verifica se a tabela existe antes de inicializar
        if ($('#categoriesTable').length > 0) {
            // Aguarda um pouco para garantir que o DOM está completamente carregado
            setTimeout(function() {
                try {
                    // Destrói instância existente se houver
                    if ($.fn.DataTable.isDataTable('#categoriesTable')) {
                        $('#categoriesTable').DataTable().destroy();
                    }
                    
                    // Inicializa o DataTable
                    $('#categoriesTable').DataTable({
                        language: {
                            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                        },
                        responsive: true,
                        pageLength: 10,
                        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
                        order: [[0, 'asc']], // Ordena por nome crescente
                        columnDefs: [
                            {
                                targets: [4], // Coluna de ações (última coluna - índice 4)
                                orderable: false,
                                searchable: false
                            },
                            {
                                targets: [2], // Coluna de status
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

    novaCategoriaModal: function () {
        $('#categoriaModal .modal-title').text('Nova Categoria');
        $('#categoriaModal .modal-dialog').removeClass('modal-xl').addClass('modal-lg');
        $('#categoriaModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#categoriaModal').modal('show');

        $.get('/ProductCategory/CreatePartial')
            .done(function (data) {
                $('#categoriaModal .modal-body').html(data);
            })
            .fail(function () {
                $('#categoriaModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar formulário</div>');
            });
    },

    salvarNovoModal: function () {
        const form = $('#categoriaModal .modal-body form');
        if (form.length === 0) {
            console.error('Formulário não encontrado no modal');
            return;
        }
        const formData = new FormData(form[0]);

        // Desabilita o botão de submit para evitar múltiplos envios
        const submitButton = form.find('button[type="button"]');
        const buttonText = submitButton.text();
        submitButton.prop('disabled', true).text('Salvando...');

        return $.ajax({
            url: '/ProductCategory/SalvarNovo',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#categoriaModal').modal('hide');
                    toastr.success(response.message || 'Categoria criada com sucesso!');

                    // Recarrega a lista
                    productCategoriesManager.carregarListaCategorias();

                    if (response.id) {
                        // Chama o método de edição passando também o numberSequence
                        productCategoriesManager.editarCategoria(response.id, response.numberSequence);
                    }
                } else {
                    toastr.error(response.message || 'Não foi possível criar a categoria');
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro ao salvar categoria:', error);
                const errorMessage = xhr.responseJSON?.message || 'Ocorreu um erro ao salvar a categoria. Por favor, tente novamente.';
                toastr.error(errorMessage);
            },
            complete: function () {
                // Reabilita o botão
                submitButton.prop('disabled', false).text(buttonText);
            }
        });
    },

    editarCategoria: function (categoriaId, numberSequence = null) {
        // Verifica se a aba já existe usando o categoriaId como identificador
        const existingTabId = `categoria-${categoriaId}`;
        const existingTab = $(`#${existingTabId}-tab`);
        
        if (existingTab.length > 0) {
            // Se a aba já existe, apenas ativa ela
            const tabTrigger = new bootstrap.Tab(document.getElementById(`${existingTabId}-tab`));
            tabTrigger.show();
            toastr.info('Categoria já está aberta em outra aba');
            return;
        }

        // Se não existe, cria nova aba
        productCategoriesManager.contador++;
        productCategoriesManager.qtdAbasAbertas++;
        const tabId = existingTabId; // Usa o categoriaId como base do ID
        
        // Se numberSequence não foi fornecido, usa um placeholder que será atualizado
        const tabTitle = numberSequence || 'Carregando...';
        
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab" data-categoria-id="${categoriaId}">
                    ${tabTitle}
                    <span class="btn-close ms-2" onclick="productCategoriesManager.fecharAba('${tabId}')"></span>
                </button>
            </li>`;
        $('#categoriasTabs').append(novaAba);
        
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
        $('#categoriaTabsContent').append(novoConteudo);
        
        // Carrega o conteúdo da aba
        $.get(`/ProductCategory/EditPartial/${categoriaId}`)
            .done(function (data) {
                $(`#conteudo-${tabId}`).html(data);
                
                // Se numberSequence não foi fornecido, extrai do conteúdo carregado
                if (!numberSequence) {
                    const nameElement = $(`#conteudo-${tabId}`).find('[data-category-name]');
                    if (nameElement.length > 0) {
                        const extractedName = nameElement.data('category-name');
                        if (extractedName) {
                            $(`#${tabId}-tab`).html(`${extractedName} <span class="btn-close ms-2" onclick="productCategoriesManager.fecharAba('${tabId}')"></span>`);
                        }
                    }
                }
            })
            .fail(function () {
                $(`#conteudo-${tabId}`).html('<div class="alert alert-danger">Erro ao carregar categoria. Tente novamente.</div>');
            });
            
        // Ativa a nova aba
        const tabTrigger = new bootstrap.Tab(document.getElementById(`${tabId}-tab`));
        tabTrigger.show();
    },

    verDetalhes: function (categoriaId) {
        $('#categoriaModal .modal-title').text('Detalhes da Categoria');
        $('#categoriaModal .modal-dialog').removeClass('modal-lg').addClass('modal-xl');
        $('#categoriaModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#categoriaModal').modal('show');

        $.get(`/ProductCategory/DetailsPartial/${categoriaId}`)
            .done(function (data) {
                $('#categoriaModal .modal-body').html(data);
            })
            .fail(function () {
                $('#categoriaModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar detalhes da categoria</div>');
            });
    },

    fecharAba: function (tabId) {
        // Remove a aba e seu conteúdo
        $(`#${tabId}-tab`).parent().remove(); // Remove o <li> que contém o button
        $(`#${tabId}`).remove(); // Remove o conteúdo da aba
        
        productCategoriesManager.qtdAbasAbertas--;
        
        // Se não há mais abas abertas, volta para a aba principal
        if (productCategoriesManager.qtdAbasAbertas === 0) {
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab'));
            mainTab.show();
        } else {
            // Se ainda há abas abertas, ativa a última aba disponível
            const remainingTabs = $('#categoriasTabs .nav-item button[data-categoria-id]');
            if (remainingTabs.length > 0) {
                const lastTab = new bootstrap.Tab(remainingTabs.last()[0]);
                lastTab.show();
            }
        }
    },

    // Método para verificar se uma categoria já está aberta
    isCategoriaAberta: function(categoriaId) {
        return $(`#categoria-${categoriaId}-tab`).length > 0;
    },

    // Método para obter lista de categorias abertas
    getCategoriasAbertas: function() {
        const abertas = [];
        $('#categoriasTabs button[data-categoria-id]').each(function() {
            abertas.push($(this).data('categoria-id'));
        });
        return abertas;
    },

    exportarCategorias: function() {
        // Implementar funcionalidade de exportação
        toastr.info('Funcionalidade de exportação em desenvolvimento...');
    },



    excluirCategoria: function(categoriaId, categoryName) {
        if (confirm(`Tem certeza que deseja excluir a categoria "${categoryName}"?\n\nEsta ação não pode ser desfeita.`)) {
            $.ajax({
                url: `/ProductCategory/Delete/${categoriaId}`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function(response) {
                    if (response.success) {
                        toastr.success('Categoria excluída com sucesso!');
                        // Recarrega a lista
                        productCategoriesManager.carregarListaCategorias();
                        
                        // Fecha a aba se estiver aberta
                        const tabId = `categoria-${categoriaId}`;
                        if ($(`#${tabId}-tab`).length > 0) {
                            productCategoriesManager.fecharAba(tabId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao excluir categoria');
                    }
                },
                error: function(xhr, status, error) {
                    console.error('Erro ao excluir categoria:', error);
                    const errorMessage = xhr.responseJSON?.message || 'Erro ao excluir categoria. Tente novamente.';
                    toastr.error(errorMessage);
                }
            });
        }
    }
};

$(document).ready(function() {
    productCategoriesManager.carregarListaCategorias();

    $('#categoriaModal').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');
        $(this).find('.modal-title').text('Categoria');
        $(this).find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
    });
});
