/**
 * Manager para Hierarquias de Componentes de Produto
 * Segue padrão manager object para operações CRUD assíncronas
 */
const hierarchyManager = {
    contador: 0,
    qtdAbasAbertas: 0,
    
    /**
     * Carrega a lista de hierarquias via AJAX
     */
    carregarListaHierarchies: function() {
        const container = $('#lista-hierarchies-container');
        
        $.get('/ProductComponentHierarchy/Grid')
            .done(function(data) {
                container.html(data);
            })
            .fail(function(xhr, status, error) {
                console.error('Erro ao carregar hierarquias:', error);
                container.html(`
                    <div class="alert alert-danger">
                        <i class="fas fa-exclamation-triangle"></i>
                        Erro ao carregar a lista de hierarquias. Tente novamente.
                        <button type="button" class="btn btn-sm btn-outline-danger ms-2" onclick="hierarchyManager.carregarListaHierarchies()">
                            <i class="fas fa-redo"></i> Tentar Novamente
                        </button>
                    </div>
                `);
            });
    },

    /**
     * Abre modal para criar nova hierarquia
     */
    novoHierarchyModal: function() {
        $.get('/ProductComponentHierarchy/Create')
            .done(function(data) {
                // Usar modal específico
                $('#hierarchyModalLabel').text('Nova Hierarquia de Componentes');
                $('#hierarchyModal .modal-body').html(data);
                
                // Inicializar formulário
                hierarchyManager.inicializarFormulario('#hierarchyModal .modal-body');
                
                $('#hierarchyModal').modal('show');
            })
            .fail(function() {
                toastr.error('Erro ao carregar formulário de criação');
            });
    },

    /**
     * Abre nova aba para editar hierarquia
     */
    editarHierarchy: function(id, name) {
        if (!id) {
            toastr.error('ID da hierarquia não informado');
            return;
        }

        // Verifica se a aba já existe usando o id como identificador
        const existingTabId = `hierarchy-${id}`;
        const existingTab = $(`#${existingTabId}-tab`);
        
        if (existingTab.length > 0) {
            // Se a aba já existe, apenas ativa ela
            const tabTrigger = new bootstrap.Tab(document.getElementById(`${existingTabId}-tab`));
            tabTrigger.show();
            toastr.info('Hierarquia já está aberta em outra aba');
            return;
        }

        // Se não existe, cria nova aba
        hierarchyManager.contador++;
        hierarchyManager.qtdAbasAbertas++;
        const tabId = existingTabId;
        
        // Usa o nome fornecido ou placeholder
        const tabTitle = name || 'Carregando...';
        
        const novaAba = `
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="${tabId}-tab" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab" data-hierarchy-id="${id}">
                    ${tabTitle}
                    <span class="btn-close ms-2" onclick="hierarchyManager.fecharAba('${tabId}')"></span>
                </button>
            </li>`;
        $('#hierarchyTabs').append(novaAba);
        
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
        $('#hierarchyTabsContent').append(novoConteudo);
        
        // Carrega o conteúdo da aba
        $.get(`/ProductComponentHierarchy/Edit/${id}`)
            .done(function(data) {
                $(`#conteudo-${tabId}`).html(data);
                
                // Inicializar formulário
                hierarchyManager.inicializarFormulario(`#conteudo-${tabId}`);
                
                // Se name não foi fornecido, extrai do conteúdo carregado
                if (!name) {
                    const nameElement = $(`#conteudo-${tabId}`).find('[data-hierarchy-name]');
                    if (nameElement.length > 0) {
                        const extractedName = nameElement.data('hierarchy-name');
                        if (extractedName) {
                            $(`#${tabId}-tab`).html(`${extractedName} <span class="btn-close ms-2" onclick="hierarchyManager.fecharAba('${tabId}')"></span>`);
                        }
                    }
                }
            })
            .fail(function() {
                $(`#conteudo-${tabId}`).html('<div class="alert alert-danger">Erro ao carregar hierarquia. Tente novamente.</div>');
            });
            
        // Ativa a nova aba
        const tabTrigger = new bootstrap.Tab(document.getElementById(`${tabId}-tab`));
        tabTrigger.show();
    },

    /**
     * Fecha uma aba específica
     */
    fecharAba: function(tabId) {
        // Remove a aba e seu conteúdo
        $(`#${tabId}-tab`).parent().remove(); // Remove o <li> que contém o button
        $(`#${tabId}`).remove(); // Remove o conteúdo da aba
        
        hierarchyManager.qtdAbasAbertas--;
        
        // Se não há mais abas abertas, volta para a aba principal
        if (hierarchyManager.qtdAbasAbertas === 0) {
            const mainTab = new bootstrap.Tab(document.getElementById('main-tab'));
            mainTab.show();
        } else {
            // Se ainda há abas abertas, ativa a última aba disponível
            const remainingTabs = $('#hierarchyTabs .nav-item button[data-hierarchy-id]');
            if (remainingTabs.length > 0) {
                const lastTab = new bootstrap.Tab(remainingTabs.last()[0]);
                lastTab.show();
            }
        }
    },

    /**
     * Verifica se uma hierarquia já está aberta
     */
    isHierarchyAberta: function(hierarchyId) {
        return $(`#hierarchy-${hierarchyId}-tab`).length > 0;
    },

    /**
     * Obtém lista de hierarquias abertas
     */
    getHierarchyAbertas: function() {
        const abertas = [];
        $('#hierarchyTabs button[data-hierarchy-id]').each(function() {
            abertas.push($(this).data('hierarchy-id'));
        });
        return abertas;
    },

    /**
     * Abre modal de detalhes da hierarquia
     */
    verDetalhes: function(id) {
        if (!id) {
            toastr.error('ID da hierarquia não informado');
            return;
        }

        $.get('/ProductComponentHierarchy/Details/' + id)
            .done(function(data) {
                $('#hierarchyModalLabel').text('Detalhes da Hierarquia');
                $('#hierarchyModal .modal-body').html(data);
                $('#hierarchyModal').modal('show');
            })
            .fail(function() {
                toastr.error('Erro ao carregar detalhes da hierarquia');
            });
    },

    /**
     * Confirma e executa exclusão da hierarquia
     */
    excluirHierarchy: function(id, name) {
        if (!id) {
            toastr.error('ID da hierarquia não informado');
            return;
        }

        // Confirmation dialog
        if (!confirm(`Tem certeza que deseja excluir a hierarquia "${name}"?\n\nEsta ação não pode ser desfeita.`)) {
            return;
        }

        // Mostrar loading
        const button = event.target.closest('button');
        const originalHtml = button.innerHTML;
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
        button.disabled = true;

        $.ajax({
            url: '/ProductComponentHierarchy/Delete',
            type: 'POST',
            data: { 
                id: id,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response.success) {
                    toastr.success(response.message || 'Hierarquia excluída com sucesso!');
                    
                    // Fechar aba se estiver aberta
                    const tabId = `hierarchy-${id}`;
                    if ($(`#${tabId}-tab`).length > 0) {
                        hierarchyManager.fecharAba(tabId);
                    }
                    
                    // Recarregar lista principal se estiver visível
                    if ($('#main').hasClass('active')) {
                        hierarchyManager.carregarListaHierarchies();
                    }
                } else {
                    toastr.error(response.message || 'Erro ao excluir hierarquia');
                }
            },
            error: function() {
                toastr.error('Erro ao excluir hierarquia');
            },
            complete: function() {
                button.innerHTML = originalHtml;
                button.disabled = false;
            }
        });
    },

    /**
     * Salva nova hierarquia via modal
     */
    salvarNovoModal: function() {
        const form = $('#createHierarchyForm');
        
        if (!hierarchyManager.validarDadosHierarchy(form)) {
            return false;
        }

        const formData = form.serialize();
        
        $.ajax({
            url: '/ProductComponentHierarchy/Create',
            type: 'POST',
            data: formData,
            success: function(response) {
                if (response.success) {
                    $('#hierarchyModal').modal('hide');
                    toastr.success(response.message || 'Hierarquia criada com sucesso!');
                    
                    // Recarregar apenas a lista se estiver visível
                    if ($('#main').hasClass('active')) {
                        hierarchyManager.carregarListaHierarchies();
                    }
                } else {
                    toastr.error(response.message || 'Erro ao criar hierarquia');
                }
            },
            error: function(xhr) {
                if (xhr.responseText) {
                    // Se retornou HTML com erros de validação
                    $('#hierarchyModal .modal-body').html(xhr.responseText);
                    hierarchyManager.inicializarFormulario('#hierarchyModal .modal-body');
                } else {
                    toastr.error('Erro ao criar hierarquia');
                }
            }
        });

        return false;
    },

    /**
     * Salva edição da hierarquia via aba
     */
    salvarEdicaoHierarchy: function() {
        // Encontra o formulário na aba ativa
        const activeTab = $('.tab-pane.active');
        const form = activeTab.find('#editHierarchyForm');
        
        if (form.length === 0) {
            toastr.error('Formulário não encontrado');
            return false;
        }
        
        if (!hierarchyManager.validarDadosHierarchy(form)) {
            return false;
        }

        const formData = form.serialize();
        const hierarchyId = form.find('input[name="Id"]').val();
        
        $.ajax({
            url: '/ProductComponentHierarchy/Edit',
            type: 'POST',
            data: formData,
            success: function(response) {
                if (response.success) {
                    toastr.success(response.message || 'Hierarquia atualizada com sucesso!');
                    
                    // Atualizar o nome da aba se mudou
                    const newName = form.find('input[name="Name"]').val();
                    if (newName) {
                        const tabId = `hierarchy-${hierarchyId}`;
                        $(`#${tabId}-tab`).html(`${newName} <span class="btn-close ms-2" onclick="hierarchyManager.fecharAba('${tabId}')"></span>`);
                    }
                    
                    // Recarregar a lista principal se estiver visível
                    if ($('#main').hasClass('active')) {
                        hierarchyManager.carregarListaHierarchies();
                    }
                } else {
                    toastr.error(response.message || 'Erro ao atualizar hierarquia');
                }
            },
            error: function(xhr) {
                if (xhr.responseText) {
                    // Se retornou HTML com erros de validação, recarrega o conteúdo da aba
                    const tabId = activeTab.attr('id');
                    $(`#conteudo-${tabId}`).html(xhr.responseText);
                    hierarchyManager.inicializarFormulario(`#conteudo-${tabId}`);
                } else {
                    toastr.error('Erro ao atualizar hierarquia');
                }
            }
        });

        return false;
    },

    /**
     * Validação client-side dos dados da hierarquia
     */
    validarDadosHierarchy: function(form) {
        let isValid = true;
        
        // Limpar erros anteriores
        form.find('.is-invalid').removeClass('is-invalid');
        form.find('.invalid-feedback').remove();
        
        // Validar nome (obrigatório)
        const nome = form.find('input[name="Name"]');
        if (!nome.val() || nome.val().trim().length === 0) {
            hierarchyManager.mostrarErroValidacao(nome, 'Nome da hierarquia é obrigatório');
            isValid = false;
        } else if (nome.val().trim().length > 200) {
            hierarchyManager.mostrarErroValidacao(nome, 'Nome deve ter no máximo 200 caracteres');
            isValid = false;
        }
        
        // Validar descrição (opcional, mas com limite)
        const descricao = form.find('textarea[name="Description"]');
        if (descricao.val() && descricao.val().length > 1000) {
            hierarchyManager.mostrarErroValidacao(descricao, 'Descrição deve ter no máximo 1000 caracteres');
            isValid = false;
        }
        
        // Validar observações (opcional, mas com limite)
        const notes = form.find('textarea[name="Notes"]');
        if (notes.val() && notes.val().length > 2000) {
            hierarchyManager.mostrarErroValidacao(notes, 'Observações devem ter no máximo 2000 caracteres');
            isValid = false;
        }
        
        return isValid;
    },

    /**
     * Mostra erro de validação em um campo
     */
    mostrarErroValidacao: function(field, message) {
        field.addClass('is-invalid');
        field.after(`<div class="invalid-feedback">${message}</div>`);
    },

    /**
     * Inicializa formulários dentro de um container
     */
    inicializarFormulario: function(container) {
        const $container = $(container);
        
        // Bind de eventos de submit
        $container.find('#createHierarchyForm').off('submit').on('submit', function(e) {
            e.preventDefault();
            hierarchyManager.salvarNovoModal();
        });
        
        $container.find('#editHierarchyForm').off('submit').on('submit', function(e) {
            e.preventDefault();
            hierarchyManager.salvarEdicaoHierarchy();
        });
        
        // Inicializar Select2 se disponível
        if (typeof $.fn.select2 !== 'undefined') {
            // Para abas, usar o body como parent ao invés do modal
            $container.find('.form-select').select2({
                dropdownParent: $('body'),
                theme: 'bootstrap-5'
            });
        }
        
        // Foco no primeiro campo
        setTimeout(function() {
            $container.find('input:visible:first').focus();
        }, 100);
    },

    /**
     * Inicializar formulário de relação hierárquica composta
     */
    initializeCompositeHierarchyRelationForm: function() {
        // Initialize Select2 for hierarchy selection
        $('.select2-hierarchy').select2({
            placeholder: "Selecione uma hierarquia...",
            allowClear: true,
            width: '100%'
        });

        // Auto-calculate next assembly order if not provided
        if ($('#AssemblyOrder').val() == '0' || $('#AssemblyOrder').val() == '') {
            // This would typically be set by the server, but we can adjust if needed
        }

        // Update preview for create form or current display for edit form
        const updateDisplay = () => {
            const hierarchyName = $('.select2-hierarchy option:selected').text();
            const minQty = $('#MinQuantity').val() || '1';
            const maxQty = $('#MaxQuantity').val() || '0';
            const isOptional = $('#IsOptional').is(':checked');
            
            // For create form preview
            if ($('#configurationPreview').length) {
                if (hierarchyName && hierarchyName !== 'Selecione uma hierarquia...') {
                    $('#previewHierarchyName').text(hierarchyName);
                    $('#previewQuantity').text(minQty + ' - ' + (maxQty === '0' ? '∞' : maxQty));
                    $('#previewType').html(isOptional ? 
                        '<span class="badge bg-warning text-dark">Opcional</span>' : 
                        '<span class="badge bg-danger">Obrigatória</span>'
                    );
                    $('#configurationPreview').show();
                } else {
                    $('#configurationPreview').hide();
                }
            }
            
            // For edit form current display
            if ($('#currentQuantity').length) {
                $('#currentQuantity').html(
                    '<span class="badge bg-primary">' + minQty + '</span> - ' +
                    '<span class="badge bg-primary">' + (maxQty === '0' ? '∞' : maxQty) + '</span>'
                );
                
                $('#currentType').html(isOptional ? 
                    '<span class="badge bg-warning text-dark">Opcional</span>' : 
                    '<span class="badge bg-danger">Obrigatória</span>'
                );
            }
        };

        // Bind events for display update
        $('.select2-hierarchy, #MinQuantity, #MaxQuantity, #IsOptional').off('change.hierarchyPreview').on('change.hierarchyPreview', updateDisplay);

        // Validate quantity configuration
        $('#MaxQuantity').off('change.hierarchyValidation').on('change.hierarchyValidation', function() {
            const minQty = parseInt($('#MinQuantity').val()) || 1;
            const maxQty = parseInt($(this).val()) || 0;
            
            if (maxQty > 0 && maxQty < minQty) {
                $(this).addClass('is-invalid');
                $(this).siblings('.field-validation-valid').text('A quantidade máxima deve ser maior ou igual à mínima').addClass('field-validation-error');
            } else {
                $(this).removeClass('is-invalid');
                $(this).siblings('.field-validation-error').text('').removeClass('field-validation-error').addClass('field-validation-valid');
            }
        });

        // Initialize form validation
        if (typeof $.fn.validate !== 'undefined') {
            $('#formCreateCompositeHierarchyRelation, #formEditCompositeHierarchyRelation').validate({
                rules: {
                    ProductComponentHierarchyId: { required: true },
                    MinQuantity: { required: true, min: 1 },
                    MaxQuantity: { min: 0 },
                    AssemblyOrder: { required: true, min: 0 },
                    Weight: { min: 0 },
                    AdditionalCost: { min: 0 },
                    AdditionalProcessingTime: { min: 0 }
                },
                messages: {
                    ProductComponentHierarchyId: "Selecione uma hierarquia de componentes",
                    MinQuantity: "Informe uma quantidade mínima válida",
                    AssemblyOrder: "Informe uma ordem de montagem válida"
                },
                errorElement: 'span',
                errorClass: 'field-validation-error text-danger',
                highlight: function(element) {
                    $(element).addClass('is-invalid');
                },
                unhighlight: function(element) {
                    $(element).removeClass('is-invalid');
                }
            });
        }

        // Handle hierarchy change warning for edit form
        $('#ProductComponentHierarchyId').off('change.hierarchyWarning').on('change.hierarchyWarning', function() {
            if ($(this).data('original-value') && $(this).val() !== $(this).data('original-value')) {
                if (!$('#hierarchy-change-warning').length) {
                    $(this).after(`
                        <div id="hierarchy-change-warning" class="alert alert-warning mt-2">
                            <i class="fas fa-exclamation-triangle"></i>
                            <strong>Atenção:</strong> Alterar a hierarquia pode afetar os componentes e cálculos existentes.
                        </div>
                    `);
                }
            } else {
                $('#hierarchy-change-warning').remove();
            }
        });

        // Store original value for comparison (edit form)
        if ($('#ProductComponentHierarchyId').val()) {
            $('#ProductComponentHierarchyId').data('original-value', $('#ProductComponentHierarchyId').val());
        }

        // Initial display update
        updateDisplay();
    },

    // Autocomplete functionality for hierarchy selection
    initializeHierarchyAutocomplete: function(inputId, hiddenId) {
        const inputField = $('#' + inputId);
        const hiddenField = $('#' + hiddenId);
        
        if (!inputField.length || !hiddenField.length) {
            console.warn('Campos de autocomplete não encontrados:', inputId, hiddenId);
            return;
        }

        // Get productId from data attribute
        const productId = inputField.data('product-id') || '';

        // Remove previous instance if exists
        if (inputField.data('aaAutocomplete')) {
            inputField.autocomplete.destroy();
        }

        // Initialize Algolia Autocomplete.js
        const autocompleteInstance = autocomplete(inputField[0], {
            hint: false,
            debug: false,
            minLength: 2,
            openOnFocus: false,
            autoselect: false,
            appendTo: inputField.closest('.modal-body, .tab-pane, body')[0]
        }, [{
            source: function(query, callback) {
                $.ajax({
                    url: '/ProductComponentHierarchy/BuscarHierarchiaDisponivel',
                    type: 'GET',
                    dataType: 'json',
                    data: { 
                        termo: query,
                        productId: productId
                    },
                    success: function(data) {
                        const suggestions = $.map(data, function(item) {
                            return {
                                label: item.label,
                                value: item.name,
                                id: item.id,
                                name: item.name,
                                description: item.description,
                                data: item
                            };
                        });
                        callback(suggestions);
                    },
                    error: function() {
                        console.error('Erro ao buscar hierarquias');
                        callback([]);
                    }
                });
            },
            displayKey: 'label',
            templates: {
                suggestion: function(suggestion) {
                    return '<div class="hierarchy-suggestion">' +
                        '<div class="hierarchy-name">' + suggestion.name + '</div>' +
                        (suggestion.description ? '<div class="hierarchy-description">' + suggestion.description + '</div>' : '') +
                        '</div>';
                },
                empty: function(query) {
                    return '<div class="aa-empty">Nenhuma hierarquia encontrada para "' + query.query + '"</div>';
                }
            }
        }]);

        // Handle selection
        autocompleteInstance.on('autocomplete:selected', function(event, suggestion) {
            // Set hidden field with ID
            hiddenField.val(suggestion.id);
            
            // Set display field with name
            inputField.val(suggestion.name);
            
            // Trigger validation
            hiddenField.trigger('change');
            inputField.removeClass('is-invalid').addClass('is-valid');
            
            // Update floating label
            const label = inputField.siblings('.floating-label');
            if (label.length) {
                label.addClass('active');
            }
            
            console.log('Hierarquia selecionada:', suggestion.name, 'ID:', suggestion.id);
        });

        // Handle clearing
        autocompleteInstance.on('autocomplete:empty', function() {
            hiddenField.val('');
            hiddenField.trigger('change');
        });

        // Handle input clearing
        inputField.on('input', function() {
            const currentValue = $(this).val();
            if (currentValue === '') {
                hiddenField.val('');
                hiddenField.trigger('change');
                $(this).removeClass('is-valid is-invalid');
                
                // Update floating label
                const label = $(this).siblings('.floating-label');
                if (label.length) {
                    label.removeClass('active');
                }
            }
        });

        // Handle manual validation on blur
        inputField.on('blur', function() {
            const currentValue = $(this).val();
            const hiddenValue = hiddenField.val();
            
            if (currentValue && !hiddenValue) {
                // User typed something but didn't select from autocomplete
                $(this).addClass('is-invalid');
                const errorSpan = $(this).siblings('.field-validation-valid, .field-validation-error');
                errorSpan.text('Selecione uma opção da lista de sugestões').removeClass('field-validation-valid').addClass('field-validation-error');
            } else if (currentValue && hiddenValue) {
                // Valid selection
                $(this).removeClass('is-invalid').addClass('is-valid');
                const errorSpan = $(this).siblings('.field-validation-error');
                errorSpan.text('').removeClass('field-validation-error').addClass('field-validation-valid');
            }
        });

        return autocompleteInstance;
    }
};

// Funções globais para compatibilidade
window.initializeCompositeHierarchyRelationForm = function() {
    hierarchyManager.initializeCompositeHierarchyRelationForm();
};

// Global object for ProductComponentHierarchy namespace
window.ProductComponentHierarchy = {
    initializeHierarchyAutocomplete: function(inputId, hiddenId) {
        return hierarchyManager.initializeHierarchyAutocomplete(inputId, hiddenId);
    },
    manager: hierarchyManager
};

// Inicialização quando o DOM estiver pronto
$(document).ready(function() {
    // Manager está pronto para uso
    console.log('HierarchyManager carregado');
    
    // Auto-inicializar formulários de relação hierárquica se presentes
    if ($('#formCreateCompositeHierarchyRelation, #formEditCompositeHierarchyRelation').length > 0) {
        hierarchyManager.initializeCompositeHierarchyRelationForm();
    }
}); 