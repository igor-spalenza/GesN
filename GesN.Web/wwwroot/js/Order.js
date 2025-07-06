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
                        responsive: false, // Desabilita responsive para funcionar com redimensionamento
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
                            
                            // Reaplica redimensionamento após redraw
                            ordersManager.aplicarRedimensionamento();
                        }
                    });
                    
                    // Inicializa o sistema de redimensionamento
                    ordersManager.inicializarRedimensionamento();
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

        // Remove instância anterior se houver
        if (field.data('aaAutocomplete')) {
            field.autocomplete.destroy();
        }

        // Inicializa Algolia Autocomplete.js
        const autocompleteInstance = autocomplete(field[0], {
            hint: false,
            debug: false,
            minLength: 2,
            openOnFocus: false,
            autoselect: true,
            appendTo: container[0] // Anexa ao container do modal
        }, [{
            source: function(query, callback) {
                $.ajax({
                    url: '/Customer/BuscaCustomerAutocomplete',
                    type: 'GET',
                    dataType: 'json',
                    data: { termo: query },
                    success: function(data) {
                        const suggestions = $.map(data, function(item) {
                            return {
                                label: item.label,
                                value: item.value,
                                id: item.id
                            };
                        });
                        callback(suggestions);
                    },
                    error: function() {
                        callback([]);
                    }
                });
            },
            displayKey: 'value',
            templates: {
                suggestion: function(suggestion) {
                    return '<div class="autocomplete-suggestion">' + suggestion.label + '</div>';
                }
            }
        }]);

        // Eventos
        autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
            container.find('#CustomerId').val(suggestion.id);
            field.val(suggestion.value);
        });

        autocompleteInstance.on('autocomplete:empty', function() {
            container.find('#CustomerId').val('');
        });

        // Adiciona placeholder e estilos
        field.attr('placeholder', 'Digite para buscar cliente...');
        field.addClass('form-control');
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
                        // Chama o método de edição passando também o numberSequence
                        ordersManager.abrirEdicao(response.id, response.numberSequence);
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

    abrirEdicao: function (orderId, numberSequence = null) {
        // Verifica se a aba já existe usando o orderId como identificador
        const existingTabId = `order-${orderId}`;
        const existingTab = $(`#${existingTabId}-tab`);
        
        if (existingTab.length > 0) {
            // Se a aba já existe, apenas ativa ela
            const tabTrigger = new bootstrap.Tab(document.getElementById(`${existingTabId}-tab`));
            tabTrigger.show();
            toastr.info('Pedido já está aberto em outra aba');
            return;
        }

        // Se não existe, cria nova aba
        ordersManager.contador++;
        ordersManager.qtdAbasAbertas++;
        const tabId = existingTabId; // Usa o orderId como base do ID
        
        // Se numberSequence não foi fornecido, usa um placeholder que será atualizado
        const tabTitle = numberSequence || 'Carregando...';
        
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab" data-order-id="${orderId}">
                    ${tabTitle}
                    <span class="btn-close ms-2" onclick="ordersManager.fecharAba('${tabId}')"></span>
                </button>
            </li>`;
        $('#orderTabs').append(novaAba);
        
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
        $('#orderTabsContent').append(novoConteudo);
        
        // Carrega o conteúdo da aba
        $.get(`/Order/EditPartial/${orderId}`)
            .done(function (data) {
                $(`#conteudo-${tabId}`).html(data);
                
                // Se numberSequence não foi fornecido, extrai do conteúdo carregado
                if (!numberSequence) {
                    const numberSequenceElement = $(`#conteudo-${tabId}`).find('[data-number-sequence]');
                    if (numberSequenceElement.length > 0) {
                        const extractedNumberSequence = numberSequenceElement.data('number-sequence');
                        if (extractedNumberSequence) {
                            $(`#${tabId}-tab`).html(`${extractedNumberSequence} <span class="btn-close ms-2" onclick="ordersManager.fecharAba('${tabId}')"></span>`);
                        }
                    }
                }
            })
            .fail(function () {
                $(`#conteudo-${tabId}`).html('<div class="alert alert-danger">Erro ao carregar pedido. Tente novamente.</div>');
            });
            
        // Ativa a nova aba
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
        // Remove a aba e seu conteúdo
        $(`#${tabId}-tab`).parent().remove(); // Remove o <li> que contém o button
        $(`#${tabId}`).remove(); // Remove o conteúdo da aba
        
        ordersManager.qtdAbasAbertas--;
        
        // Se não há mais abas abertas, volta para a aba principal
        if (ordersManager.qtdAbasAbertas === 0) {
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab'));
            mainTab.show();
        } else {
            // Se ainda há abas abertas, ativa a última aba disponível
            const remainingTabs = $('#orderTabs .nav-item button[data-order-id]');
            if (remainingTabs.length > 0) {
                const lastTab = new bootstrap.Tab(remainingTabs.last()[0]);
                lastTab.show();
            }
        }
    },

    // Método para verificar se um pedido já está aberto
    isPedidoAberto: function(orderId) {
        return $(`#order-${orderId}-tab`).length > 0;
    },

    // Método para obter lista de pedidos abertos
    getPedidosAbertos: function() {
        const abertos = [];
        $('#orderTabs button[data-order-id]').each(function() {
            abertos.push($(this).data('order-id'));
        });
        return abertos;
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
    },

    // ========================================
    // SISTEMA DE REDIMENSIONAMENTO DE COLUNAS
    // ========================================
    
    // Configurações de redimensionamento
    resizeConfig: {
        minWidths: {
            0: 80,   // Número
            1: 150,  // Cliente
            2: 90,   // Data
            3: 90,   // Entrega
            4: 70,   // Tipo
            5: 100,  // Valor Total
            6: 80,   // Status
            7: 120   // Ações
        },
        storageKey: 'ordersTable_column_widths',
        isDragging: false,
        currentColumn: null,
        startX: 0,
        startWidth: 0
    },

    inicializarRedimensionamento: function() {
        // Adiciona handles de redimensionamento aos headers
        $('#ordersTable thead th').each(function(index) {
            const $th = $(this);
            
            // Remove handles existentes
            $th.find('.resize-handle').remove();
            
            // Adiciona handle de redimensionamento
            const $handle = $('<div class="resize-handle"></div>');
            $th.append($handle);
            
            // Eventos para o handle
            $handle.on('mousedown', function(e) {
                e.preventDefault();
                ordersManager.iniciarRedimensionamento(index, e.pageX, $th.outerWidth());
            });
            
            // Duplo clique para auto-ajustar
            $handle.on('dblclick', function(e) {
                e.preventDefault();
                ordersManager.autoAjustarColuna(index);
            });
        });
        
        // Eventos globais para redimensionamento
        $(document).on('mousemove', ordersManager.redimensionarColuna);
        $(document).on('mouseup', ordersManager.finalizarRedimensionamento);
        
        // Restaura larguras salvas
        ordersManager.restaurarLargurasColunas();
    },

    iniciarRedimensionamento: function(columnIndex, startX, startWidth) {
        ordersManager.resizeConfig.isDragging = true;
        ordersManager.resizeConfig.currentColumn = columnIndex;
        ordersManager.resizeConfig.startX = startX;
        ordersManager.resizeConfig.startWidth = startWidth;
        
        // Adiciona classe visual ao header
        $(`#ordersTable thead th:eq(${columnIndex})`).addClass('resizing');
        
        // Desabilita seleção de texto
        $('body').addClass('user-select-none');
        
        // Cursor global
        $('body').css('cursor', 'col-resize');
    },

    redimensionarColuna: function(e) {
        if (!ordersManager.resizeConfig.isDragging) return;
        
        const columnIndex = ordersManager.resizeConfig.currentColumn;
        const deltaX = e.pageX - ordersManager.resizeConfig.startX;
        const newWidth = ordersManager.resizeConfig.startWidth + deltaX;
        const minWidth = ordersManager.resizeConfig.minWidths[columnIndex] || 50;
        
        // Aplica largura mínima
        const finalWidth = Math.max(newWidth, minWidth);
        
        // Aplica a nova largura
        $(`#ordersTable thead th:eq(${columnIndex})`).css('width', finalWidth + 'px');
        $(`#ordersTable tbody td:nth-child(${columnIndex + 1})`).css('width', finalWidth + 'px');
    },

    finalizarRedimensionamento: function() {
        if (!ordersManager.resizeConfig.isDragging) return;
        
        const columnIndex = ordersManager.resizeConfig.currentColumn;
        
        // Remove classe visual
        $(`#ordersTable thead th:eq(${columnIndex})`).removeClass('resizing');
        
        // Restaura cursor e seleção
        $('body').css('cursor', 'default').removeClass('user-select-none');
        
        // Salva larguras
        ordersManager.salvarLargurasColunas();
        
        // Reset das configurações
        ordersManager.resizeConfig.isDragging = false;
        ordersManager.resizeConfig.currentColumn = null;
    },

    autoAjustarColuna: function(columnIndex) {
        const $th = $(`#ordersTable thead th:eq(${columnIndex})`);
        const $cells = $(`#ordersTable tbody td:nth-child(${columnIndex + 1})`);
        
        // Calcula a largura máxima necessária
        let maxWidth = $th.text().length * 8 + 40; // Largura base do header
        
        $cells.each(function() {
            const cellContent = $(this).text().trim();
            const cellWidth = cellContent.length * 8 + 20; // Estimativa baseada no texto
            maxWidth = Math.max(maxWidth, cellWidth);
        });
        
        // Aplica largura mínima
        const minWidth = ordersManager.resizeConfig.minWidths[columnIndex] || 50;
        const finalWidth = Math.max(maxWidth, minWidth);
        
        // Aplica a nova largura
        $th.css('width', finalWidth + 'px');
        $cells.css('width', finalWidth + 'px');
        
        // Salva as larguras
        ordersManager.salvarLargurasColunas();
        
        toastr.success('Coluna ajustada automaticamente!');
    },

    salvarLargurasColunas: function() {
        const widths = {};
        
        $('#ordersTable thead th').each(function(index) {
            const width = $(this).outerWidth();
            widths[index] = width + 'px';
        });
        
        localStorage.setItem(ordersManager.resizeConfig.storageKey, JSON.stringify(widths));
    },

    restaurarLargurasColunas: function() {
        const savedWidths = localStorage.getItem(ordersManager.resizeConfig.storageKey);
        
        if (savedWidths) {
            try {
                const widths = JSON.parse(savedWidths);
                
                $('#ordersTable thead th').each(function(index) {
                    if (widths[index]) {
                        $(this).css('width', widths[index]);
                    }
                });
                
                // Aplica também nas células do tbody
                $('#ordersTable tbody tr').each(function() {
                    $(this).find('td').each(function(index) {
                        if (widths[index]) {
                            $(this).css('width', widths[index]);
                        }
                    });
                });
                
            } catch (e) {
                console.error('Erro ao restaurar larguras das colunas:', e);
            }
        }
    },

    aplicarRedimensionamento: function() {
        // Método chamado após DataTable redraw
        if ($('#ordersTable').length > 0) {
            // Reaplica larguras salvas após redraw
            ordersManager.restaurarLargurasColunas();
        }
    }
};

$(function() {
    ordersManager.carregarListaOrders();

    $('#orderModal').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');
        $(this).find('.modal-title').text('Pedido');
        $(this).find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
    });
}); 