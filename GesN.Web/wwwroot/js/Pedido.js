
/*document.getElementById('btnAbrirPedido').addEventListener('click', () => {
    abaContador++;
    qtdAbasAbertas++;
    const pedidoId = `pedido${abaContador}`;

    // Criar a aba dinamicamente
    const novaAba = `
        <li class="nav-item" role="presentation">
            <button class="nav-link"
                    id="${pedidoId}-tab"
                    data-bs-toggle="tab"
                    data-bs-target="#${pedidoId}"
                    type="button"
                    role="tab">
                Novo Pedido
                <span class="btn-close ms-2" onclick="fecharAba('${pedidoId}')"></span>
            </button>
        </li>`;
    document.getElementById('pedidoTabs').insertAdjacentHTML('beforeend', novaAba);

    // Criar o conteúdo da aba e carregar a Partial View via AJAX
    const novoConteudo = `
        <div class="main-div tab-pane fade ${abaContador === 1 ? 'show active' : ''}"
             id="${pedidoId}"
             role="tabpanel">
            <div id="conteudo-${pedidoId}">Carregando...</div>
        </div>`;
    document.getElementById('pedidoTabsContent').insertAdjacentHTML('beforeend', novoConteudo);

    // Chamada AJAX para buscar a Partial View
    fetch('/Pedido/NovoPedido')
        .then(response => response.text())
        .then(html => {
            document.getElementById(`conteudo-${pedidoId}`).innerHTML = html;
        })
        .catch(error => {
            document.getElementById(`conteudo-${pedidoId}`).innerHTML = '<p>Erro ao carregar o pedido.</p>';
            console.error('Erro ao carregar a partial view:', error);
        });

    // Ativar a nova aba automaticamente
    const tabTrigger = new bootstrap.Tab(document.getElementById(`${pedidoId}-tab`));
    tabTrigger.show();
});*/
/*
document.getElementById('btnHomeVendas').addEventListener('click', () => {
    // 1. Remover as classes "active" e "show" de todas as abas ativas
    document.querySelectorAll('.tab-pane.active, .nav-link.active').forEach(element => {
        element.classList.remove('active', 'show');
    });

    // 2. Adicionar as classes "active" e "show" à aba principal
    const mainTab = document.getElementById('main-tab');
    mainTab.classList.add('active', 'show');

    // 3. Ativar a aba principal usando o Bootstrap Tab
    const tabTrigger = new bootstrap.Tab(document.querySelector('[data-bs-target="#main-tab"]'));
    tabTrigger.show();
});

function fecharAba(pedidoId) {
    const aba = document.getElementById(`${pedidoId}-tab`);
    const conteudo = document.getElementById(pedidoId);

    aba.remove();
    conteudo.remove();
    qtdAbasAbertas--;
    if (qtdAbasAbertas == 0) {
        const mainTab = document.getElementById('main-tab');
        mainTab.classList.add('active', 'show');
    }
}

function EditarPedido(IdPedido, editing) {
    abaContador++;
    qtdAbasAbertas++;
    const pedidoId = `pedido${abaContador}`;

    // Criar a aba dinamicamente
    const novaAba = `
        <li class="nav-item" role="presentation">
            <button class="nav-link"
                    id="${pedidoId}-tab"
                    data-bs-toggle="tab"
                    data-bs-target="#${pedidoId}"
                    type="button"
                    role="tab">
                Novo Pedido
                <span class="btn-close ms-2" onclick="fecharAba('${pedidoId}')"></span>
            </button>
        </li>`;
    document.getElementById('pedidoTabs').insertAdjacentHTML('beforeend', novaAba);

    // Criar o conteúdo da aba e carregar a Partial View via AJAX
    const novoConteudo = `
        <div class="main-div tab-pane fade ${abaContador === 1 ? 'show active' : ''}"
             id="${pedidoId}"
             role="tabpanel">
            <div id="conteudo-${pedidoId}">Carregando...</div>
        </div>`;
    document.getElementById('pedidoTabsContent').insertAdjacentHTML('beforeend', novoConteudo);
    let action = (editing == true ? "EditPartial" : "DetailsPartialView");

    // Chamada AJAX para buscar a Partial View de EDIT ou DETAILS
    fetch('/Pedido/' + action + '/' + IdPedido)
        .then(response => response.text())
        .then(html => {
            document.getElementById(`conteudo-${pedidoId}`).innerHTML = html;
        })
        .catch(error => {
            document.getElementById(`conteudo-${pedidoId}`).innerHTML = '<p>Erro ao carregar o pedido.</p>';
            console.error('Erro ao carregar a partial view:', error);
        });
    
    // Ativar a nova aba automaticamente
    const tabTrigger = new bootstrap.Tab(document.getElementById(`${pedidoId}-tab`));
    tabTrigger.show();
}

function CriarPedido(IdPedido, editing) {
    abaContador++;
    qtdAbasAbertas++;
    const pedidoId = `pedido${abaContador}`;

    // Criar a aba dinamicamente
    const novaAba = `
        <li class="nav-item" role="presentation">
            <button class="nav-link"
                    id="${pedidoId}-tab"
                    data-bs-toggle="tab"
                    data-bs-target="#${pedidoId}"
                    type="button"
                    role="tab">
                Novo Pedido
                <span class="btn-close ms-2" onclick="fecharAba('${pedidoId}')"></span>
            </button>
        </li>`;
    document.getElementById('pedidoTabs').insertAdjacentHTML('beforeend', novaAba);

    // Criar o conteúdo da aba e carregar a Partial View via AJAX

    const novoConteudo = `
        <div class="main-div tab-pane fade ${abaContador === 1 ? 'show active' : ''}"
             id="${pedidoId}"
             role="tabpanel">
            <div id="conteudo-${pedidoId}">Carregando...</div>
        </div>`;
    document.getElementById('pedidoTabsContent').insertAdjacentHTML('beforeend', novoConteudo);
    let action = (editing == true ? "EditPartial" : "DetailsPartialView");

    // Chamada AJAX para buscar a Partial View de EDIT ou DETAILS
    fetch('/Pedido/' + action + '/' + IdPedido)
        .then(response => response.text())
        .then(html => {
            document.getElementById(`conteudo-${pedidoId}`).innerHTML = html;
        })
        .catch(error => {
            document.getElementById(`conteudo-${pedidoId}`).innerHTML = '<p>Erro ao carregar o pedido.</p>';
            console.error('Erro ao carregar a partial view:', error);
        });

    // Ativar a nova aba automaticamente
    const tabTrigger = new bootstrap.Tab(document.getElementById(`${pedidoId}-tab`));
    tabTrigger.show();
}*/



//
// NOVO Objeto para gerenciar os pedidos
// 
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
                if (response.success) {
                    form.find('input[name="ClienteId"]').val(response.pedido.clienteId);
                    form.find('input[name="ColaboradorId"]').val(response.pedido.colaboradorId);
                    form.find('input[name="DataPedido"]').val(response.pedido.dataPedido);
                    form.find('input[name="DataCadastro"]').val(response.pedido.dataCadastro);
                    form.find('input[name="DataModificacao"]').val(response.pedido.dataModificacao);
                }
                //abaContainer.empty().html(partialHtml);
                toastr.success('Pedido atualizado com sucesso!');
                $(button).prop('disabled', false).html('Salvar Alterações');
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