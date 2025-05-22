
const pedidosManager = {
    contador: 0,
    qtdAbasAbertas: 0,
    
    carregarListaPedidos: function() {
        $('#lista-pedidos-container').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: '/Pedido/ListaPedidos',
            type: 'GET',
            success: function(data) {
                $('#lista-pedidos-container').html(data);
            },
            error: function() {
                $('#lista-pedidos-container').html('<div class="alert alert-danger">Erro ao carregar a lista de pedidos.</div>');
            }
        });
    },
    novoPedidoModal: function () {
        $('#pedidoModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#pedidoModal').modal('show');

        $.get('/Pedido/NovoPedido', function (data) {
            $('#pedidoModal .modal-body').html(data);
            pedidosManager.inicializarAutocomplete($('#pedidoModal'));
        }).fail(function () {
            $('#pedidoModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar formulário</div>');
        });
    },

    visualizarPedidoModal: function (id) {
        $.get('/Pedido/DetailsPartialView/' + id, function (data) {
            $('#pedidoModal .modal-body').html(data);
            $('#pedidoModal').modal('show');
        });
    },

    inicializarAutocomplete: function (container) {
        const buscarClienteField = container.find('#buscarCliente');

        if (buscarClienteField.length === 0) {
            console.error('Campo #buscarCliente não encontrado');
            return;
        }

        if (buscarClienteField.data('ui-autocomplete')) {
            buscarClienteField.autocomplete('destroy');
        }

        buscarClienteField.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Pedido/BuscarClienteNomeTel',
                    data: { termo: request.term },
                    dataType: 'json',
                    success: function (data) {
                        console.log('Dados recebidos:', data);
                        response($.map(data, function (item) {
                            return {
                                label: item.nome + ' - ' + item.telefonePrincipal,
                                value: item.nome,
                                id: item.id
                            };
                        }));
                    },
                    error: function (xhr, status, error) {
                        console.error('Erro na requisição:', error);
                        response([]);
                    }
                });
            },
            minLength: 2,
            delay: 300,
            appendTo: container, // Anexa ao container fornecido (modal)
            select: function (event, ui) {
                container.find('#ClienteId').val(ui.item.id);
                $(this).autocomplete('close');
                return true;
            }
        });
    },

    salvarNovoModal: function () {
        const form = $('#pedidoModal .modal-body form');
        if (form.length === 0) {
            console.error('Formulário não encontrado no modal');
            return;
        }
        const formData = new FormData(form[0]);

        // Desabilita o botão de submit para evitar múltiplos envios
        const submitButton = form.find('button[type="submit"]');
        const buttonText = submitButton.text();
        submitButton.prop('disabled', true).text('Salvando...');

        return $.ajax({
            url: '/Pedido/SalvarNovo',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#pedidoModal').modal('hide');
                    toastr.success('Pedido atualizado com sucesso!');

                    if (response.id) {
                        // Chama o método editarPedido, se estiver usando o gerenciador de pedidos
                        if (typeof pedidosManager !== 'undefined') {
                            pedidosManager.editarPedido(response.id);
                        } else if (typeof EditarPedido === 'function') {
                            // Usa a função global EditarPedido como fallback
                            EditarPedido(response.id, true);
                        } else {
                            console.error('Não foi possível encontrar uma função para editar o pedido');
                        }
                    }
                } else {
                    alert('Erro: ' + (response.message || 'Não foi possível criar o pedido'));
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro ao salvar pedido:', error);
                alert('Ocorreu um erro ao salvar o pedido. Por favor, tente novamente.');
            }
        });
    },

    editarPedido: function(id) {
        this.contador++;
        this.qtdAbasAbertas++;
        const tabId = `tab-pedido-${id}`;
        const contentId = `conteudo-pedido-${id}`;
        
        this.adicionarAba(tabId, contentId, `Pedido #${id}`);
        
        $(`#${contentId}`).html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: `/Pedido/Editar/${id}`,
            type: 'GET',
            success: function(data) {
                $(`#${contentId}`).html(data);
            },
            error: function() {
                $(`#${contentId}`).html('<div class="alert alert-danger">Erro ao carregar o pedido.</div>');
            },
            complete: function () {
                var nome = $(`#${contentId}`).find('input[name="NomeCliente"]').val();
                $(`#${tabId} .nome-pedido`).text(nome + ` #${id}`);
            }
        });

        $(`#${tabId}`).tab('show');
    },

    salvarEdicao: function (button) {
        var form = $(button).closest('form');
        var formData = form.serialize();
        var pedidoId = form.find('input[name="PedidoId"]').val();
        //var abaContainer = form.closest('.tab-pane');

        $(button).prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Salvando...');

        $.ajax({
            url: `/Pedido/SalvarEdicao/${pedidoId}`,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success === true) {
                    form.find('input[name="ClienteId"]').val(response.pedido.clienteId);
                    form.find('input[name="ColaboradorId"]').val(response.pedido.colaboradorId);
                    form.find('input[name="DataPedido"]').val(response.pedido.dataPedido);
                    form.find('input[name="DataCadastro"]').val(response.pedido.dataCadastro);
                    form.find('input[name="DataModificacao"]').val(response.pedido.dataModificacao);
                    toastr.success('Pedido atualizado com sucesso!');
                    $(button).prop('disabled', false).html('Salvar Alterações');
                }
                else if (response.success === false) {
                    toastr.error('Erro ao salvar o pedido. ' + response.message);
                    $(button).prop('disabled', false).html('Salvar Alterações');
                }

            },
            error: function () {
                $(button).prop('disabled', false).html('Salvar Alterações');
                toastr.error('Erro ao salvar o pedido.');
            }
        });
    },

    adicionarAba: function(tabId, contentId, titulo) {
        const novaAba = `
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="${tabId}" data-bs-toggle="tab" href="#${contentId}" role="tab">
                    <span class="nome-pedido">${titulo}</span>
                    <button type="button" class="btn-close ms-2" onclick="pedidosManager.fecharAba('${tabId}', '${contentId}')"></button>
                </a>
            </li>
        `;
        
        // Adiciona a aba na barra de navegação
        $('#pedidosTabs').append(novaAba);
        
        // Cria o conteúdo da aba
        const novoConteudo = `
            <div class="tab-pane fade" id="${contentId}" role="tabpanel">
                <!-- O conteúdo será carregado via AJAX -->
            </div>
        `;
        
        $('#pedidoTabsContent').append(novoConteudo);
    },
    
    fecharAba: function(tabId, contentId) {
        // Verifica se a aba a ser fechada está ativa
        const estaAtiva = $(`#${tabId}`).hasClass('active');
        
        // Remove a aba e seu conteúdo
        $(`#${tabId}`).parent().remove();
        $(`#${contentId}`).remove();
        
        this.qtdAbasAbertas--;
        
        // Se a aba fechada estava ativa, ativa a primeira aba
        if (estaAtiva) {
            $('#tab-lista-pedidos').tab('show');
        }
    },
    
    excluirPedido: function(id) {
        if (confirm('Tem certeza que deseja excluir este pedido?')) {
            $.ajax({
                url: `/Pedido/Excluir/${id}`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function(response) {
                    if (response.success) {
                        alert(response.message);
                        pedidosManager.carregarListaPedidos();
                    } else {
                        alert('Erro: ' + response.message);
                    }
                },
                error: function() {
                    alert('Ocorreu um erro ao processar a requisição.');
                }
            });
        }
    }
};

// Inicializa o gerenciador de pedidos quando o documento estiver pronto
$(document).ready(function() {
    pedidosManager.carregarListaPedidos();

    $('#pedidoModal').on('shown.bs.modal', function () {
        pedidosManager.inicializarAutocomplete($(this));
    });

    $('#pedidoModal').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');
    });

    $(document).on('submit', '.form-editar-pedido', function (e) {
        e.preventDefault();
        $(this).find('button[onclick^="pedidosManager.salvarEdicao"]').click();
    });

});