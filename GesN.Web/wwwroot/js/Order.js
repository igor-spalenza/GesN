const ordersManager = {
    contador: 0,
    qtdAbasAbertas: 0,
    
    carregarListaOrders: function() {
        $('#lista-orders-container').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: '/Order/Grid',
            type: 'GET',
            success: function(data) {
                $('#lista-orders-container').html(data);
                // Inicializa DataTables após carregar o conteúdo
                ordersManager.inicializarDataTable();
            },
            error: function() {
                $('#lista-orders-container').html('<div class="alert alert-danger">Erro ao carregar a lista de pedidos.</div>');
            }
        });
    },

    inicializarDataTable: function() {
        // Verifica se a tabela existe antes de inicializar
        if ($('#ordersTable').length > 0) {
            // Aguarda um pouco para garantir que o DOM está completamente carregado
            setTimeout(function() {
                try {
                    // Destrói instância existente se houver
                    if ($.fn.DataTable.isDataTable('#ordersTable')) {
                        $('#ordersTable').DataTable().destroy();
                    }
                    
                    // Inicializa o DataTable
                    $('#ordersTable').DataTable({
                        language: {
                            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                        },
                        responsive: true,
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

    novoOrderModal: function () {
        $('#orderModal .modal-title').text('Novo Pedido');
        $('#orderModal .modal-dialog').removeClass('modal-xl').addClass('modal-lg');
        $('#orderModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#orderModal').modal('show');

        $.get('/Order/CreatePartial')
            .done(function (data) {
                $('#orderModal .modal-body').html(data);
                ordersManager.inicializarAutocomplete($('#orderModal'));
            })
            .fail(function () {
                $('#orderModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar formulário</div>');
            });
    },

    inicializarAutocomplete: function (container) {
        const field = container.find('#buscarCustomer');

        if (field.length === 0) {
            return;
        }

        if (field.data('ui-autocomplete')) {
            field.autocomplete('destroy');
        }

        field.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Customer/BuscaCustomerAutocomplete',
                    data: { termo: request.term },
                    dataType: 'json',
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.label,
                                value: item.value,
                                id: item.id
                            };
                        }));
                    },
                    error: function () {
                        response([]);
                    }
                });
            },
            minLength: 2,
            delay: 300,
            select: function (event, ui) {
                $(this).val(ui.item.value);
                container.find('#CustomerId').val(ui.item.id);
                return false;
            }
        });

        // Função para forçar fechamento (workaround para conflito)
        function forceCloseAutocomplete() {
            field.autocomplete('close');
            setTimeout(() => {
                $('.ui-autocomplete').hide(); // Force hide se ainda visível
            }, 50);
        }

        // Fechar ao clicar fora
        $(document).on('click.orderClose', function(e) {
            if (!$(e.target).is(field) && !$(e.target).closest('.ui-autocomplete').length) {
                forceCloseAutocomplete();
            }
        });

        // Fechar ao perder foco
        field.on('blur.orderClose', function() {
            setTimeout(forceCloseAutocomplete, 100);
        });

        // Fechar com ESC
        field.on('keydown.orderClose', function(e) {
            if (e.keyCode === 27) {
                forceCloseAutocomplete();
            }
        });
    },

    salvarNovoModal: function () {
        const form = $('#orderModal .modal-body form');
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
            url: '/Order/SalvarNovo',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#orderModal').modal('hide');
                    toastr.success(response.message || 'Pedido criado com sucesso!');

                    if (response.id) {
                        // Chama o método de edição
                        ordersManager.abrirEdicao(response.id);
                    }
                } else {
                    toastr.error(response.message || 'Não foi possível criar o pedido');
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro ao salvar pedido:', error);
                const errorMessage = xhr.responseJSON?.message || 'Ocorreu um erro ao salvar o pedido. Por favor, tente novamente.';
                toastr.error(errorMessage);
            },
            complete: function () {
                // Reabilita o botão
                submitButton.prop('disabled', false).text(buttonText);
            }
        });
    },

    abrirEdicao: function (orderId) {
        ordersManager.contador++;
        ordersManager.qtdAbasAbertas++;
        const tabId = `order${ordersManager.contador}`;
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab">
                    Pedido ${orderId}
                    <span class="btn-close ms-2" onclick="ordersManager.fecharAba('${tabId}')"></span>
                </button>
            </li>`;
        $('#orderTabs').append(novaAba);
        const novoConteudo = `
            <div class="main-div tab-pane fade" id="${tabId}" role="tabpanel">
                <div id="conteudo-${tabId}">Carregando...</div>
            </div>`;
        $('#orderTabsContent').append(novoConteudo);
        $.get(`/Order/EditPartial/${orderId}`)
            .done(function (data) {
                $(`#conteudo-${tabId}`).html(data);
            })
            .fail(function () {
                $(`#conteudo-${tabId}`).html('<div class="alert alert-danger">Erro ao carregar pedido.</div>');
            });
        const tabTrigger = new bootstrap.Tab(document.getElementById(`${tabId}-tab`));
        tabTrigger.show();
    },

    abrirDetalhes: function (orderId) {
        $('#orderModal .modal-title').text('Detalhes do Pedido');
        $('#orderModal .modal-dialog').removeClass('modal-lg').addClass('modal-xl');
        $('#orderModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#orderModal').modal('show');

        $.get(`/Order/DetailsPartial/${orderId}`)
            .done(function (data) {
                $('#orderModal .modal-body').html(data);
            })
            .fail(function () {
                $('#orderModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar detalhes do pedido.</div>');
            });
    },

    fecharAba: function (tabId) {
        $(`#${tabId}-tab`).remove();
        $(`#${tabId}`).remove();
        ordersManager.qtdAbasAbertas--;
        if (ordersManager.qtdAbasAbertas === 0) {
            $('#main-tab').addClass('active show');
        }
    },

    exportarPedidos: function() {
        // Implementar funcionalidade de exportação
        toastr.info('Funcionalidade de exportação em desenvolvimento...');
    },

    excluirPedido: function(orderId, orderNumber) {
        if (confirm(`Tem certeza que deseja excluir o pedido ${orderNumber}?\n\nEsta ação não pode ser desfeita.`)) {
            $.ajax({
                url: `/Order/Delete/${orderId}`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function(response) {
                    if (response.success) {
                        toastr.success('Pedido excluído com sucesso!');
                        // Recarrega a lista
                        ordersManager.carregarListaOrders();
                    } else {
                        toastr.error(response.message || 'Erro ao excluir o pedido');
                    }
                },
                error: function(xhr, status, error) {
                    console.error('Erro ao excluir pedido:', error);
                    const errorMessage = xhr.responseJSON?.message || 'Erro ao excluir pedido. Tente novamente.';
                    toastr.error(errorMessage);
                }
            });
        }
    }
};

$(document).ready(function() {
    ordersManager.carregarListaOrders();

    $('#orderModal').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');
        $(this).find('.modal-title').text('Pedido');
        $(this).find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
    });
}); 