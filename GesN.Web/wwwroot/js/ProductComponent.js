/**
 * ProductComponent.js
 * Gerencia as operações assíncronas de ProductComponent via modais
 */

const productComponentsManager = {
    contador: 0,
    qtdAbasAbertas: 0,
    
    carregarListaComponentes: function() {
        $('#lista-componentes-container').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        
        $.ajax({
            url: '/ProductComponent/Grid',
            type: 'GET',
            success: function(data) {
                $('#lista-componentes-container').html(data);
                // Inicializa DataTables após carregar o conteúdo
                productComponentsManager.inicializarDataTable();
            },
            error: function() {
                $('#lista-componentes-container').html('<div class="alert alert-danger">Erro ao carregar lista de componentes</div>');
            }
        });
    },

    inicializarDataTable: function() {
        // Verifica se a tabela existe antes de inicializar
        if ($('#components-table').length > 0) {
            // Aguarda um pouco para garantir que o DOM está completamente carregado
            setTimeout(function() {
                try {
                    // Destrói instância existente se houver
                    if ($.fn.DataTable.isDataTable('#components-table')) {
                        $('#components-table').DataTable().destroy();
                    }
                    
                    // Inicializa o DataTable
                    $('#components-table').DataTable({
                        language: {
                            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                        },
                        responsive: true,
                        pageLength: 10,
                        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
                        order: [[0, 'asc']], // Ordena por produto composto crescente
                        columnDefs: [
                            {
                                targets: [6], // Coluna de ações (última coluna - índice 6)
                                orderable: false,
                                searchable: false
                            },
                            {
                                targets: [4], // Coluna de opcional/obrigatório
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

    novoComponenteModal: function () {
        $('#componenteModal .modal-title').text('Novo Componente');
        $('#componenteModal .modal-dialog').removeClass('modal-xl').addClass('modal-lg');
        $('#componenteModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#componenteModal').modal('show');

        $.get('/ProductComponent/Create')
            .done(function (data) {
                $('#componenteModal .modal-body').html(data);
                
                // Inicializar autocomplete após carregar conteúdo
                productComponentsManager.inicializarAutocompleteHierarchy($('#componenteModal .modal-body'));
            })
            .fail(function () {
                $('#componenteModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar formulário</div>');
            });
    },

    salvarNovoModal: function () {
        const form = $('#componenteModal .modal-body form');
        if (form.length === 0) {
            console.error('Formulário não encontrado no modal');
            return;
        }

        // Validar dados do formulário
        const validation = productComponentsManager.validarDadosComponente(form);
        if (!validation.isValid) {
            toastr.error('Por favor, corrija os erros no formulário');
            return;
        }

        const formData = new FormData(form[0]);

        // Desabilita o botão de submit para evitar múltiplos envios
        const submitButton = form.find('button[type="button"]');
        const buttonText = submitButton.text();
        submitButton.prop('disabled', true).text('Salvando...');

        return $.ajax({
            url: '/ProductComponent/Create',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#componenteModal').modal('hide');
                    toastr.success(response.message || 'Componente criado com sucesso!');

                    // Recarrega a lista
                    productComponentsManager.carregarListaComponentes();
                } else {
                    // Tratar erros de validação do servidor
                    if (response.errors && typeof response.errors === 'object') {
                        Object.keys(response.errors).forEach(function(fieldName) {
                            const field = form.find(`input[name="${fieldName}"], select[name="${fieldName}"], textarea[name="${fieldName}"]`);
                            if (field.length) {
                                field.addClass('input-validation-error');
                                const validationSpan = field.next('.field-validation-valid');
                                if (validationSpan.length) {
                                    validationSpan.addClass('field-validation-error').text(response.errors[fieldName][0] || response.errors[fieldName]);
                                }
                            }
                        });
                    }
                    toastr.error(response.message || 'Não foi possível criar o componente');
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro ao salvar componente:', error);
                
                // Tratar erros de validação do servidor (400 Bad Request)
                if (xhr.status === 400 && xhr.responseJSON) {
                    if (xhr.responseJSON.errors) {
                        Object.keys(xhr.responseJSON.errors).forEach(function(fieldName) {
                            const field = form.find(`input[name="${fieldName}"], select[name="${fieldName}"], textarea[name="${fieldName}"]`);
                            if (field.length) {
                                field.addClass('input-validation-error');
                                const validationSpan = field.next('.field-validation-valid');
                                if (validationSpan.length) {
                                    validationSpan.addClass('field-validation-error').text(xhr.responseJSON.errors[fieldName][0] || xhr.responseJSON.errors[fieldName]);
                                }
                            }
                        });
                    }
                    toastr.error('Erro de validação. Verifique os campos destacados.');
                } else {
                    const errorMessage = xhr.responseJSON?.message || 'Ocorreu um erro ao salvar o componente. Por favor, tente novamente.';
                    toastr.error(errorMessage);
                }
            },
            complete: function () {
                // Reabilita o botão
                submitButton.prop('disabled', false).text(buttonText);
            }
        });
    },

    editarComponente: function (componenteId, productName = null) {
        // Verifica se a aba já existe usando o componenteId como identificador
        const existingTabId = `componente-${componenteId}`;
        const existingTab = $(`#${existingTabId}-tab`);
        
        if (existingTab.length > 0) {
            // Se a aba já existe, apenas ativa ela
            const tabTrigger = new bootstrap.Tab(document.getElementById(`${existingTabId}-tab`));
            tabTrigger.show();
            toastr.info('Componente já está aberto em outra aba');
            return;
        }

        // Se não existe, cria nova aba
        productComponentsManager.contador++;
        productComponentsManager.qtdAbasAbertas++;
        const tabId = existingTabId; // Usa o componenteId como base do ID
        
        // Se productName não foi fornecido, usa um placeholder que será atualizado
        const tabTitle = productName || 'Carregando...';
        
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab" data-componente-id="${componenteId}">
                    ${tabTitle}
                    <span class="btn-close ms-2" onclick="productComponentsManager.fecharAba('${tabId}')"></span>
                </button>
            </li>`;
        $('#componentesTabs').append(novaAba);
        
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
        $('#componenteTabsContent').append(novoConteudo);
        
        // Carrega o conteúdo da aba
        $.get(`/ProductComponent/Edit/${componenteId}`)
            .done(function (data) {
                $(`#conteudo-${tabId}`).html(data);
                
                // Inicializar autocomplete após carregar conteúdo
                productComponentsManager.inicializarAutocompleteHierarchy($(`#conteudo-${tabId}`));
                
                // Se productName não foi fornecido, extrai do conteúdo carregado
                if (!productName) {
                    const nameElement = $(`#conteudo-${tabId}`).find('[data-component-name]');
                    if (nameElement.length > 0) {
                        const extractedName = nameElement.data('component-name');
                        if (extractedName) {
                            $(`#${tabId}-tab`).html(`${extractedName} <span class="btn-close ms-2" onclick="productComponentsManager.fecharAba('${tabId}')"></span>`);
                        }
                    }
                }
            })
            .fail(function () {
                $(`#conteudo-${tabId}`).html('<div class="alert alert-danger">Erro ao carregar componente. Tente novamente.</div>');
            });
            
        // Ativa a nova aba
        const tabTrigger = new bootstrap.Tab(document.getElementById(`${tabId}-tab`));
        tabTrigger.show();
    },

    verDetalhes: function (componenteId) {
        $('#componenteModal .modal-title').text('Detalhes do Componente');
        $('#componenteModal .modal-dialog').removeClass('modal-lg').addClass('modal-xl');
        $('#componenteModal .modal-body').html('<div class="text-center"><div class="spinner-border" role="status"></div></div>');
        $('#componenteModal').modal('show');

        $.get(`/ProductComponent/Details/${componenteId}`)
            .done(function (data) {
                $('#componenteModal .modal-body').html(data);
            })
            .fail(function () {
                $('#componenteModal .modal-body').html('<div class="alert alert-danger">Erro ao carregar detalhes do componente</div>');
            });
    },

    fecharAba: function (tabId) {
        // Remove a aba e seu conteúdo
        $(`#${tabId}-tab`).parent().remove(); // Remove o <li> que contém o button
        $(`#${tabId}`).remove(); // Remove o conteúdo da aba
        
        productComponentsManager.qtdAbasAbertas--;
        
        // Se não há mais abas abertas, volta para a aba principal
        if (productComponentsManager.qtdAbasAbertas === 0) {
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab'));
            mainTab.show();
        } else {
            // Se ainda há abas abertas, ativa a última aba disponível
            const remainingTabs = $('#componentesTabs .nav-item button[data-componente-id]');
            if (remainingTabs.length > 0) {
                const lastTab = new bootstrap.Tab(remainingTabs.last()[0]);
                lastTab.show();
            }
        }
    },

    // Método para verificar se um componente já está aberto
    isComponenteAberto: function(componenteId) {
        return $(`#componente-${componenteId}-tab`).length > 0;
    },

    // Método para obter lista de componentes abertos
    getComponentesAbertos: function() {
        const abertos = [];
        $('#componentesTabs button[data-componente-id]').each(function() {
            abertos.push($(this).data('componente-id'));
        });
        return abertos;
    },

    excluirComponente: function(componenteId, componentName) {
        if (confirm(`Tem certeza que deseja excluir o componente "${componentName}"?\n\nEsta ação não pode ser desfeita.`)) {
            $.ajax({
                url: `/ProductComponent/Delete/${componenteId}`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function(response) {
                    if (response.success) {
                        toastr.success('Componente excluído com sucesso!');
                        // Recarrega a lista
                        productComponentsManager.carregarListaComponentes();
                        
                        // Fecha a aba se estiver aberta
                        const tabId = `componente-${componenteId}`;
                        if ($(`#${tabId}-tab`).length > 0) {
                            productComponentsManager.fecharAba(tabId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao excluir componente');
                    }
                },
                error: function(xhr, status, error) {
                    console.error('Erro ao excluir componente:', error);
                    const errorMessage = xhr.responseJSON?.message || 'Erro ao excluir componente. Tente novamente.';
                    toastr.error(errorMessage);
                }
            });
        }
    },

    salvarEdicaoComponente: function(componenteId) {
        const form = $(`#conteudo-componente-${componenteId} form`);
        if (form.length === 0) {
            console.error('Formulário não encontrado');
            return;
        }

        // Validar dados do formulário
        const validation = productComponentsManager.validarDadosComponente(form);
        if (!validation.isValid) {
            toastr.error('Por favor, corrija os erros no formulário');
            return;
        }

        const formData = new FormData(form[0]);
        const submitButton = form.find('button[type="button"]');
        const buttonText = submitButton.text();
        submitButton.prop('disabled', true).text('Salvando...');

        $.ajax({
            url: `/ProductComponent/Edit/${componenteId}`,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function(response) {
                if (response.success) {
                    toastr.success('Componente atualizado com sucesso!');
                    
                    // Recarrega a lista principal
                    productComponentsManager.carregarListaComponentes();
                    
                    // Atualiza o título da aba se necessário
                    if (response.componentName) {
                        const tabId = `componente-${componenteId}`;
                        $(`#${tabId}-tab`).html(`${response.componentName} <span class="btn-close ms-2" onclick="productComponentsManager.fecharAba('${tabId}')"></span>`);
                    }
                } else {
                    // Tratar erros de validação do servidor
                    if (response.errors && typeof response.errors === 'object') {
                        Object.keys(response.errors).forEach(function(fieldName) {
                            const field = form.find(`input[name="${fieldName}"], select[name="${fieldName}"], textarea[name="${fieldName}"]`);
                            if (field.length) {
                                field.addClass('input-validation-error');
                                const validationSpan = field.next('.field-validation-valid');
                                if (validationSpan.length) {
                                    validationSpan.addClass('field-validation-error').text(response.errors[fieldName][0] || response.errors[fieldName]);
                                }
                            }
                        });
                    }
                    toastr.error(response.message || 'Erro ao atualizar componente');
                }
            },
            error: function(xhr, status, error) {
                console.error('Erro ao salvar componente:', error);
                
                // Tratar erros de validação do servidor (400 Bad Request)
                if (xhr.status === 400 && xhr.responseJSON) {
                    if (xhr.responseJSON.errors) {
                        Object.keys(xhr.responseJSON.errors).forEach(function(fieldName) {
                            const field = form.find(`input[name="${fieldName}"], select[name="${fieldName}"], textarea[name="${fieldName}"]`);
                            if (field.length) {
                                field.addClass('input-validation-error');
                                const validationSpan = field.next('.field-validation-valid');
                                if (validationSpan.length) {
                                    validationSpan.addClass('field-validation-error').text(xhr.responseJSON.errors[fieldName][0] || xhr.responseJSON.errors[fieldName]);
                                }
                            }
                        });
                    }
                    toastr.error('Erro de validação. Verifique os campos destacados.');
                } else {
                    const errorMessage = xhr.responseJSON?.message || 'Erro ao salvar componente. Tente novamente.';
                    toastr.error(errorMessage);
                }
            },
            complete: function() {
                submitButton.prop('disabled', false).text(buttonText);
            }
        });
    },

    exportarComponentes: function() {
        // Implementar funcionalidade de exportação
        toastr.info('Funcionalidade de exportação em desenvolvimento...');
    },

    // Método para inicializar autocomplete de hierarquias em modais/abas
    inicializarAutocompleteHierarchy: function(container) {
        const hierarchyNameField = container.find('#ProductComponentHierarchyName');
        const hierarchyIdField = container.find('#ProductComponentHierarchyId');
        
        if (hierarchyNameField.length === 0) {
            return;
        }

        // Remove instância anterior se houver
        if (hierarchyNameField.data('aaAutocomplete')) {
            hierarchyNameField.autocomplete.destroy();
        }

        // Inicializar Algolia Autocomplete.js
        const autocompleteInstance = autocomplete(hierarchyNameField[0], {
            hint: false,
            debug: false,
            minLength: 2,
            openOnFocus: false,
            autoselect: true,
            appendTo: container[0] // Anexa ao container do modal/aba
        }, [{
            source: function(query, callback) {
                $.ajax({
                    url: '/ProductComponent/BuscarHierarchyAutocomplete',
                    type: 'GET',
                    dataType: 'json',
                    data: { termo: query },
                    success: function(data) {
                        const suggestions = $.map(data, function(item) {
                            return {
                                label: item.label,
                                value: item.value,
                                id: item.id,
                                description: item.description,
                                data: item
                            };
                        });
                        callback(suggestions);
                    },
                    error: function() {
                        callback([]);
                    }
                });
            },
            displayKey: 'label',
            templates: {
                suggestion: function(suggestion) {
                    return '<div class="autocomplete-suggestion">' +
                           '<div class="suggestion-title">' + (suggestion.data.name || suggestion.label) + '</div>' +
                           (suggestion.data.description ? '<div class="suggestion-subtitle">' + suggestion.data.description + '</div>' : '') +
                           '</div>';
                }
            }
        }]);

        // Handle selection for Hierarchy
        autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
            hierarchyIdField.val(suggestion.id);
            hierarchyNameField.val(suggestion.value);
            container.find('#displayHierarchyName').text(suggestion.value);
            
            // Atualizar display de custo (compatível com _EditComponent.cshtml)
            productComponentsManager.atualizarDisplayCusto(container);
        });

        // Limpar seleção se campo ficar vazio
        hierarchyNameField.on('blur', function() {
            if ($(this).val() === '') {
                hierarchyIdField.val('');
                container.find('#displayHierarchyName').text('-');
                productComponentsManager.atualizarDisplayCusto(container);
            }
        });
    },

    // Método para atualizar display de custo (compatível com _EditComponent.cshtml)
    atualizarDisplayCusto: function(container) {
        const additionalCost = parseFloat(container.find('#AdditionalCost').val() || 0);
        const hierarchyName = container.find('#ProductComponentHierarchyName').val();
        
        container.find('#displayAdditionalCost').text('R$ ' + additionalCost.toFixed(2).replace('.', ','));
        
        // Sempre mostrar o custo info na edição
        container.find('#componentCostInfo').removeClass('d-none');
    },

    // Método para validar dados do componente
    validarDadosComponente: function(form) {
        let isValid = true;
        const errors = [];

        // Limpar erros anteriores
        form.find('.field-validation-error').removeClass('field-validation-error').empty();
        form.find('.input-validation-error').removeClass('input-validation-error');

        // Validar nome
        const nameField = form.find('input[name="Name"]');
        if (!nameField.val() || nameField.val().trim() === '') {
            nameField.addClass('input-validation-error');
            const validationSpan = nameField.next('.field-validation-valid');
            if (validationSpan.length) {
                validationSpan.addClass('field-validation-error').text('O nome do componente é obrigatório');
            }
            errors.push('Nome é obrigatório');
            isValid = false;
        } else if (nameField.val().length > 100) {
            nameField.addClass('input-validation-error');
            const validationSpan = nameField.next('.field-validation-valid');
            if (validationSpan.length) {
                validationSpan.addClass('field-validation-error').text('O nome deve ter no máximo 100 caracteres');
            }
            errors.push('Nome muito longo');
            isValid = false;
        }

        // Validar hierarquia
        const hierarchyIdField = form.find('input[name="ProductComponentHierarchyId"]');
        if (!hierarchyIdField.val() || hierarchyIdField.val().trim() === '') {
            const hierarchyNameField = form.find('#ProductComponentHierarchyName');
            hierarchyNameField.addClass('input-validation-error');
            const validationSpan = hierarchyNameField.next('.field-validation-valid');
            if (validationSpan.length) {
                validationSpan.addClass('field-validation-error').text('A hierarquia de componentes é obrigatória');
            }
            errors.push('Hierarquia é obrigatória');
            isValid = false;
        }

        // Validar custo adicional
        const costField = form.find('input[name="AdditionalCost"]');
        if (costField.val() && parseFloat(costField.val()) < 0) {
            costField.addClass('input-validation-error');
            const validationSpan = costField.next('.field-validation-valid');
            if (validationSpan.length) {
                validationSpan.addClass('field-validation-error').text('O custo adicional deve ser maior ou igual a zero');
            }
            errors.push('Custo adicional inválido');
            isValid = false;
        }

        // Validar descrição
        const descriptionField = form.find('textarea[name="Description"]');
        if (descriptionField.val() && descriptionField.val().length > 500) {
            descriptionField.addClass('input-validation-error');
            const validationSpan = descriptionField.next('.field-validation-valid');
            if (validationSpan.length) {
                validationSpan.addClass('field-validation-error').text('A descrição deve ter no máximo 500 caracteres');
            }
            errors.push('Descrição muito longa');
            isValid = false;
        }

        return {
            isValid: isValid,
            errors: errors
        };
    },

    // Método helper para buscar componente por ID
    buscarComponentePorId: function(componentId) {
        return $.ajax({
            url: `/ProductComponent/Details/${componentId}`,
            type: 'GET',
            dataType: 'json'
        });
    },

    // Método para exportar componentes (placeholder)
    exportarComponentes: function() {
        toastr.info('Funcionalidade de exportação em desenvolvimento...');
    }
};

$(function() {
    // Se estivermos na página principal de componentes, carrega a lista
    if ($('#lista-componentes-container').length > 0) {
        productComponentsManager.carregarListaComponentes();
    }

    // Configura o modal para limpar conteúdo ao fechar
    $('#componenteModal').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').html('');
        $(this).find('.modal-title').text('Componente');
        $(this).find('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
    });
}); 