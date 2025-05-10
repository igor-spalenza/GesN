let abaContador = 1;
let qtdAbasAbertas = 0;

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
}

// Objeto para gerenciar os pedidos
const pedidosManager = {
    contador: 0,
    qtdAbasAbertas: 0,
    
    // Carrega a lista de pedidos na primeira aba
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
    
    // Cria uma nova aba para criar um pedido
    novoPedido: function() {
        this.contador++;
        this.qtdAbasAbertas++;
        const tabId = `tab-novo-pedido-${this.contador}`;
        const contentId = `conteudo-novo-pedido-${this.contador}`;
        
        this.adicionarAba(tabId, contentId, 'Novo Pedido');
        
        $(`#${contentId}`).html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: '/Pedido/NovoPedido',
            type: 'GET',
            success: function(data) {
                $(`#${contentId}`).html(data);
                pedidosManager.inicializarFormulario($(`#${contentId} form`), 'SalvarNovo');
            },
            error: function() {
                $(`#${contentId}`).html('<div class="alert alert-danger">Erro ao carregar o formulário de pedido.</div>');
            }
        });
        
        // Ativa a nova aba
        $(`#${tabId}`).tab('show');
    },
    
    // Cria uma nova aba para editar um pedido
    editarPedido: function(id) {
        this.contador++;
        this.qtdAbasAbertas++;
        const tabId = `tab-editar-pedido-${this.contador}`;
        const contentId = `conteudo-editar-pedido-${this.contador}`;
        
        this.adicionarAba(tabId, contentId, `Editar Pedido #${id}`);
        
        $(`#${contentId}`).html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: `/Pedido/Editar/${id}`,
            type: 'GET',
            success: function(data) {
                $(`#${contentId}`).html(data);
                pedidosManager.inicializarFormulario($(`#${contentId} form`), 'SalvarEdicao');
            },
            error: function() {
                $(`#${contentId}`).html('<div class="alert alert-danger">Erro ao carregar o pedido.</div>');
            }
        });
        
        $(`#${tabId}`).tab('show');
    },
    
    // Cria uma nova aba para visualizar os detalhes de um pedido
    visualizarPedido: function(id) {
        this.contador++;
        this.qtdAbasAbertas++;
        const tabId = `tab-detalhes-pedido-${this.contador}`;
        const contentId = `conteudo-detalhes-pedido-${this.contador}`;
        
        this.adicionarAba(tabId, contentId, `Pedido #${id}`);
        
        $(`#${contentId}`).html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: `/Pedido/Detalhes/${id}`,
            type: 'GET',
            success: function(data) {
                $(`#${contentId}`).html(data);
            },
            error: function() {
                $(`#${contentId}`).html('<div class="alert alert-danger">Erro ao carregar os detalhes do pedido.</div>');
            }
        });
        
        $(`#${tabId}`).tab('show');
    },
    
    adicionarAba: function(tabId, contentId, titulo) {
        const novaAba = `
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="${tabId}" data-bs-toggle="tab" href="#${contentId}" role="tab">
                    ${titulo}
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
    
    // Fecha uma aba
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
    
    // Inicializa um formulário para usar AJAX
    inicializarFormulario: function(form, actionName) {
        form.submit(function(e) {
            e.preventDefault();
            
            const formData = new FormData(form[0]);
            const pedidoId = formData.get('PedidoId');
            let url = `/Pedido/${actionName}`;
            
            if (actionName === 'SalvarEdicao' && pedidoId) {
                url += `/${pedidoId}`;
            }
            
            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        // Exibe mensagem de sucesso
                        alert(response.message);
                        
                        // Recarrega a lista de pedidos
                        pedidosManager.carregarListaPedidos();
                        
                        // Se for uma criação bem-sucedida e tiver um ID, abre a edição
                        if (actionName === 'SalvarNovo' && response.id) {
                            // Fecha a aba atual (último contador)
                            const tabIdAtual = `tab-novo-pedido-${pedidosManager.contador}`;
                            const contentIdAtual = `conteudo-novo-pedido-${pedidosManager.contador}`;
                            pedidosManager.fecharAba(tabIdAtual, contentIdAtual);
                            
                            // Abre a aba de edição
                            pedidosManager.editarPedido(response.id);
                        }
                    } else {
                        // Exibe mensagem de erro
                        alert('Erro: ' + response.message);
                    }
                },
                error: function() {
                    alert('Ocorreu um erro ao processar a requisição.');
                }
            });
        });
    },
    
    // Exclui um pedido
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
    // Carrega a lista de pedidos na aba principal
    pedidosManager.carregarListaPedidos();
});