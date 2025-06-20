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

        // Remover qualquer overlay existente
        form.find('.loading-overlay').remove();

        // Criar o HTML do overlay diretamente
        var loadingHtml = `
            <div class="loading-overlay" style="
                position: absolute;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(255, 255, 255, 0.7);
                z-index: 1000;
                display: flex;
                justify-content: center;
                align-items: center;
            ">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Carregando...</span>
                </div>
            </div>
        `;
        
        // Garantir que o contêiner do formulário tenha position: relative
        form.css('position', 'relative');
        
        // Adicionar o overlay ao formulário
        form.append(loadingHtml);
        
        // Armazenar referência ao overlay
        var loadingOverlay = form.find('.loading-overlay');
        
        // Mostrar spinner e desabilitar botão
        $(button).prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Salvando...');

        $.ajax({
            url: `/Pedido/SalvarEdicao/${pedidoId}`,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    // Atualizar os valores no formulário
                    form.find('input[name="ClienteId"]').val(response.pedido.clienteId);
                    form.find('input[name="DataPedido"]').val(response.pedido.dataPedido);
                    form.find('input[name="DataCadastro"]').val(response.pedido.dataCadastro);
                    form.find('input[name="DataModificacao"]').val(response.pedido.dataModificacao);
                    
                    // Remover overlay de loading após atualizar os campos
                    loadingOverlay.remove();
                    
                    // Exibir mensagem de sucesso
                    toastr.success(response.message || 'Pedido atualizado com sucesso!');
                } else {
                    // Remover overlay de loading em caso de erro
                    loadingOverlay.remove();
                    
                    // Exibir mensagem de erro
                    if (response.errors && response.errors.length > 0) {
                        // Erros de validação
                        var errorList = '<ul>';
                        response.errors.forEach(function(error) {
                            errorList += '<li>' + error + '</li>';
                        });
                        errorList += '</ul>';
                        
                        toastr.error(errorList, response.message || 'Falha na validação');
                    } else {
                        // Erro geral
                        toastr.error(response.message || 'Erro ao salvar o pedido');
                    }
                    
                    console.error('Erro ao salvar o pedido:', response.message);
                }
            },
            error: function (xhr, status, error) {
                // Remover overlay de loading em caso de erro na requisição
                loadingOverlay.remove();
                
                console.error('Erro na requisição:', error);
                
                // Verificar se há uma resposta JSON com mensagem de erro
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    toastr.error(xhr.responseJSON.message);
                } else {
                    toastr.error('Erro ao processar a requisição. Verifique os dados informados.');
                }
            },
            complete: function () {
                // Sempre restaurar o botão
                $(button).prop('disabled', false).html('Salvar Alterações');
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
    
    inicializarFormulario: function(form) {
        //form.submit(function(e) {
        //    e.preventDefault();
            
        //    const formData = new FormData(form[0]);
        //    const pedidoId = formData.get('PedidoId');

        //    $.ajax({
        //        url: `/Pedido/${pedidoId}`,
        //        type: 'POST',
        //        data: formData,
        //        processData: false,
        //        contentType: false,
        //        success: function(response) {
        //            if (response.success) {
        //                alert(response.message);
                        
        //                pedidosManager.carregarListaPedidos();
                        
        //                // Se for uma criação bem-sucedida e tiver um ID, abre a edição
        //                if (actionName === 'SalvarNovo' && response.id) {
        //                    // Fecha a aba atual (último contador)
        //                    const tabIdAtual = `tab-novo-pedido-${pedidosManager.contador}`;
        //                    const contentIdAtual = `conteudo-novo-pedido-${pedidosManager.contador}`;
        //                    pedidosManager.fecharAba(tabIdAtual, contentIdAtual);
                            
        //                    // Abre a aba de edição
        //                    pedidosManager.editarPedido(response.id);
        //                }
        //            } else {
        //                // Exibe mensagem de erro
        //                alert('Erro: ' + response.message);
        //            }
        //        },
        //        error: function() {
        //            alert('Ocorreu um erro ao processar a requisição.');
        //        }
        //    });
        //});
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
$(function() {
    pedidosManager.carregarListaPedidos();

    $('#pedidoModal').on('shown.bs.modal', function () {
        pedidosManager.inicializarAutocomplete($(this));
    });

    $('#pedidoModal').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');
    });

    $(document).on('submit', '.form-editar-pedido', function (e) {
        e.preventDefault();
        $(this).find('button[onclick^="pedidosManager.salvarEdicao"]').trigger('click');
    });

});