// ProductGroup Management JavaScript
const productGroupManager = {
    // Initialize floating labels for all form inputs
    initializeFloatingLabels: function() {
        // Detectar valores preenchidos em inputs
        $('.floating-input, .floating-textarea, .floating-select').each(function() {
            const $input = $(this);
            if ($input.val() && $input.val().length > 0) {
                $input.addClass('has-value');
            }
        });

        // Event listeners para floating labels
        $('.floating-input, .floating-textarea').on('input blur', function() {
            const $input = $(this);
            if ($input.val() && $input.val().length > 0) {
                $input.addClass('has-value');
            } else {
                $input.removeClass('has-value');
            }
        });

        // Event listeners para selects
        $('.floating-select').on('change', function() {
            const $select = $(this);
            if ($select.val() && $select.val().length > 0) {
                $select.addClass('has-value');
            } else {
                $select.removeClass('has-value');
            }
        });
    },

    // Index page initialization
    initializeIndex: function() {
        // Inicializar DataTable
        var table = $('#groupsTable').DataTable({
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
            },
            responsive: true,
            order: [[1, 'asc']],
            columnDefs: [
                { orderable: false, targets: -1 }
            ]
        });

        // Pesquisa personalizada
        $('#searchInput').on('keyup', function() {
            table.search(this.value).draw();
        });

        $('#searchBtn').on('click', function() {
            table.search($('#searchInput').val()).draw();
        });

        // Filtros
        $('#statusFilter').on('change', function() {
            var value = this.value;
            if (value === '') {
                table.column(5).search('').draw();
            } else {
                var statusText = value === '1' ? 'Ativo' : 'Inativo';
                table.column(5).search(statusText).draw();
            }
        });

        $('#categoryFilter').on('change', function() {
            table.column(2).search(this.value).draw();
        });

        // Tooltip
        $('[title]').tooltip();
    },

    // Create page initialization
    initializeCreate: function() {
        // Máscara para preços
        $('#Price').mask('000.000.000,00', {reverse: true});
        $('#UnitPrice').mask('000.000.000,00', {reverse: true});
    },

    // Product Group Item Management
    items: {
        // Show create item modal
        showCreateModal: function(productGroupId) {
            $.ajax({
                url: `/ProductGroup/FormularioGroupItem/${productGroupId}`,
                type: 'GET',
                success: function(data) {
                    // Remove modal if it exists
                    if ($('#createGroupItemModal').length > 0) {
                        $('#createGroupItemModal').remove();
                    }
                    
                    // Add modal to body
                    $('body').append(data);
                    
                    // Initialize form after modal is shown
                    $('#createGroupItemModal').on('shown.bs.modal', function () {
                        console.log('Modal shown, productGroupId parameter:', productGroupId);
                        
                        // Debug: Check all hidden fields
                        $(this).find('input[type="hidden"]').each(function() {
                            console.log('Hidden field:', $(this).attr('name'), '=', $(this).val());
                        });
                        
                        // Ensure ProductGroupId is set correctly
                        const productGroupIdField = $('#ProductGroupId');
                        console.log('ProductGroupId field found:', productGroupIdField.length > 0);
                        console.log('Current ProductGroupId value:', productGroupIdField.val());
                        
                        if (productGroupIdField.length) {
                            if (!productGroupIdField.val() || productGroupIdField.val() === '') {
                                productGroupIdField.val(productGroupId);
                                console.log('ProductGroupId set to:', productGroupId);
                            } else {
                                console.log('ProductGroupId already has value:', productGroupIdField.val());
                            }
                        }
                        
                        productGroupManager.items.initializeForm();
                        
                        // Clear any previous validation errors
                        $(this).find('.text-danger').text('');
                        $(this).find('.is-invalid').removeClass('is-invalid');
                    });
                    
                    // Handle save button click
                    $('#createGroupItemModal').off('click', '#btnSaveGroupItem').on('click', '#btnSaveGroupItem', function() {
                        productGroupManager.items.save();
                    });
                    
                    // Clean up when modal is hidden
                    $('#createGroupItemModal').on('hidden.bs.modal', function () {
                        $(this).find('form')[0].reset();
                        $(this).find('.text-danger').text('');
                        $(this).find('.is-invalid').removeClass('is-invalid');
                        $(this).find('.floating-input').removeClass('has-value');
                        $(this).remove();
                    });
                    
                    // Show modal
                    $('#createGroupItemModal').modal('show');
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de criação:', xhr);
                    toastr.error('Erro ao abrir formulário');
                }
            });
        },

        // Show edit item modal
        showEditModal: function(itemId) {
            $.ajax({
                url: `/ProductGroup/FormularioEdicaoGroupItem/${itemId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="editGroupItemModal" tabindex="-1">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"><i class="fas fa-edit"></i> Editar Item do Grupo</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                        <button type="button" class="btn btn-primary" onclick="productGroupManager.items.saveEdit()">
                                            <i class="fas fa-save"></i> Salvar Alterações
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#editGroupItemModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#editGroupItemModal').on('shown.bs.modal', function () {
                        productGroupManager.items.initializeForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#editGroupItemModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de edição:', xhr);
                    toastr.error('Erro ao abrir formulário de edição');
                }
            });
        },

        // Save group item
        save: function() {
            const form = $('#formCreateGroupItem')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            // Debug: Log form data before sending
            const formData = new FormData(form);
            console.log('Form data being sent:');
            for (let pair of formData.entries()) {
                console.log(pair[0] + ': ' + pair[1]);
            }
            
            // Validate required fields before sending
            const productGroupId = formData.get('ProductGroupId');
            const productId = formData.get('ProductId');
            const quantity = formData.get('Quantity');
            
            console.log('Validation check - ProductGroupId:', productGroupId);
            console.log('Validation check - ProductId:', productId);
            console.log('Validation check - Quantity:', quantity);
            
            if (!productGroupId || productGroupId.trim() === '') {
                toastr.error('ID do grupo de produtos não foi definido. Recarregue a página e tente novamente.');
                return;
            }

            // Disable submit button
            const submitBtn = $('#btnSaveGroupItem');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

            $.ajax({
                url: '/ProductGroup/SalvarGroupItem',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#createGroupItemModal').modal('hide');
                        
                        // Atualizar apenas a partial view dos itens do grupo ao invés de recarregar a página
                        productGroupManager.items.refreshGroupItemsList(productGroupId);
                    } else {
                        toastr.error(response.message || 'Erro ao salvar item');
                        // Show validation errors if present
                        if (response.errors) {
                            productGroupManager.utils.showValidationErrors(response.errors, 'createGroupItemModal');
                        }
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar item:', xhr);
                    let errorMsg = 'Erro ao salvar item do grupo';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        productGroupManager.utils.showValidationErrors(xhr.responseJSON.errors, 'createGroupItemModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            });
        },

        // Save edit group item
        saveEdit: function() {
            const form = $('#formEditGroupItem')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            const formData = new FormData(form);
            const itemId = $('#formEditGroupItem input[name="Id"]').val();
            const productGroupId = $('#formEditGroupItem input[name="ProductGroupId"]').val();

            // Disable submit button
            const submitBtn = $('#editGroupItemModal .btn-primary');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

            $.ajax({
                url: `/ProductGroup/SalvarEdicaoGroupItem/${itemId}`,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#editGroupItemModal').modal('hide');
                        
                        // Atualizar apenas a partial view dos itens do grupo
                        if (productGroupId) {
                            productGroupManager.items.refreshGroupItemsList(productGroupId);
                        }
                    } else {
                        toastr.error(response.message);
                        // Show validation errors if present
                        if (response.errors) {
                            productGroupManager.utils.showValidationErrors(response.errors, 'editGroupItemModal');
                        }
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar edição:', xhr);
                    let errorMsg = 'Erro ao salvar alterações do item';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        productGroupManager.utils.showValidationErrors(xhr.responseJSON.errors, 'editGroupItemModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            });
        },

        // Confirm delete item
        confirmDelete: function(itemId, productName) {
            if (confirm(`Tem certeza que deseja excluir o item "${productName}" do grupo?`)) {
                this.delete(itemId);
            }
        },

        // Delete group item
        delete: function(itemId) {
            // Get antiforgery token
            const token = $('input[name="__RequestVerificationToken"]').val();
            if (!token) {
                toastr.error('Token de segurança não encontrado');
                return;
            }

            $.ajax({
                url: `/ProductGroup/ExcluirGroupItem/${itemId}`,
                type: 'POST',
                data: { __RequestVerificationToken: token },
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        
                        // Atualizar a lista de itens após exclusão
                        const productGroupId = $('.product-edit-container').data('product-id');
                        if (productGroupId) {
                            productGroupManager.items.refreshGroupItemsList(productGroupId);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao excluir item');
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao excluir item:', xhr);
                    const errorMsg = xhr.responseJSON?.message || 'Erro ao excluir item do grupo';
                    toastr.error(errorMsg);
                }
            });
        },

        // Refresh group items list (partial view update)
        refreshGroupItemsList: function(productGroupId) {
            console.log('Refreshing group items list for productGroupId:', productGroupId);
            
            if (!productGroupId) {
                console.error('ProductGroupId not provided for refresh');
                return;
            }

            // Show loading indicator - usando ID dinâmico baseado no productGroupId (ProductId)
            const container = $(`#groupItemsContainer-${productGroupId}`);
            if (container.length === 0) {
                console.warn(`Container #groupItemsContainer-${productGroupId} not found`);
                return;
            }

            const originalContent = container.html();
            container.html('<div class="text-center p-4"><i class="fas fa-spinner fa-spin"></i> Carregando...</div>');

            $.ajax({
                url: `/ProductGroup/ProductGroupItems/${productGroupId}`,
                type: 'GET',
                success: function(data) {
                    container.html(data);
                    console.log('Group items list refreshed successfully');
                    
                    // Auto-load exchange rules info for all items after refresh
                    setTimeout(() => {
                        $('[id^="exchange-rules-info-"]').each(function() {
                            const itemId = $(this).attr('id').replace('exchange-rules-info-', '');
                            if (itemId) {
                                productGroupManager.exchangeRules.loadExchangeRulesInfo(itemId);
                            }
                        });
                    }, 100);
                },
                error: function(xhr) {
                    console.error('Erro ao atualizar lista de itens:', xhr);
                    container.html(originalContent);
                    toastr.error('Erro ao atualizar lista de itens');
                }
            });
        },

        // Initialize ProductGroup Item form
        initializeForm: function() {
            // Initialize floating labels first
            productGroupManager.initializeFloatingLabels();
            
            // Initialize group item form functionality
            productGroupManager.exchangeRules.initializeGroupItemForm();
            
            // Initialize autocomplete for ProductName field using Algolia autocomplete.js
            const productNameField = $('#ProductName');
            const productIdField = $('#ProductId');
            
            if (productNameField.length) {
                // Remove previous instance if exists
                if (productNameField.data('aaAutocomplete')) {
                    productNameField.autocomplete.destroy();
                }

                // Initialize Algolia Autocomplete.js
                const autocompleteInstance = autocomplete(productNameField[0], {
                    hint: false,
                    debug: false,
                    minLength: 2,
                    openOnFocus: false,
                    autoselect: true,
                    appendTo: productNameField.closest('.modal-body, .tab-pane, body')[0]
                }, [{
                    source: function(query, callback) {
                        $.ajax({
                            url: '/Product/BuscaProductAutocomplete',
                            type: 'GET',
                            dataType: 'json',
                            data: { termo: query },
                            success: function(data) {
                                const suggestions = $.map(data, function(item) {
                                    return {
                                        label: item.label || (item.value + (item.sku ? ' - ' + item.sku : '')),
                                        value: item.label || (item.value + (item.sku ? ' - ' + item.sku : '')),
                                        id: item.id,
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
                    displayKey: 'value',
                    templates: {
                        suggestion: function(suggestion) {
                            return '<div class="autocomplete-suggestion">' + suggestion.label + '</div>';
                        }
                    }
                }]);

                // Events
                autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
                    productIdField.val(suggestion.id);
                    productNameField.val(suggestion.value);
                    productNameField.addClass('has-value');
                    productNameField.trigger('change');
                });

                autocompleteInstance.on('autocomplete:empty', function() {
                    productIdField.val('');
                    productNameField.removeClass('has-value');
                });

                // Add placeholder and styles
                productNameField.attr('placeholder', ' ');
                productNameField.addClass('form-control');

                // Handle floating label behavior
                productNameField.on('focus blur keyup change input', function() {
                    if ($(this).val() !== '') {
                        $(this).addClass('has-value');
                    } else {
                        $(this).removeClass('has-value');
                    }
                });

                // Initialize floating label state
                if (productNameField.val() !== '') {
                    productNameField.addClass('has-value');
                }
            }

            // Initialize Select2 for any remaining select2-product elements (for edit forms)
            $('.select2-product').select2({
                placeholder: 'Selecione um produto',
                allowClear: true,
                ajax: {
                    url: '/Product/BuscaProductAutocomplete',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            termo: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        return {
                            results: data.map(function(item) {
                                return {
                                    id: item.id,
                                    text: item.label || (item.value + (item.sku ? ' - ' + item.sku : '')),
                                    data: item
                                };
                            })
                        };
                    },
                    cache: true
                }
            });

            // Handle floating labels for input fields
            $('.floating-input').on('focus blur keyup change input', function() {
                if ($(this).val() !== '' && $(this).val() !== null) {
                    $(this).addClass('has-value');
                } else {
                    $(this).removeClass('has-value');
                }
            });

            // Initialize floating labels on page load
            $('.floating-input').each(function() {
                if ($(this).val() !== '' && $(this).val() !== null) {
                    $(this).addClass('has-value');
                }
            });

            // Special handling for number inputs
            $('.floating-input[type="number"]').on('change', function() {
                if ($(this).val() !== '' && $(this).val() !== null && $(this).val() !== '0') {
                    $(this).addClass('has-value');
                } else {
                    $(this).removeClass('has-value');
                }
            });

            // Sync default quantity with quantity when changed
            $('#Quantity').off('change.productGroup').on('change.productGroup', function() {
                const quantity = $(this).val();
                if (quantity && !$('#DefaultQuantity').val()) {
                    $('#DefaultQuantity').val(quantity).addClass('has-value');
                }
            });

            // Validate min vs max quantity
            $('#MinQuantity, #MaxQuantity').off('change.productGroup').on('change.productGroup', function() {
                const min = parseInt($('#MinQuantity').val()) || 0;
                const max = parseInt($('#MaxQuantity').val()) || 0;
                
                if (max > 0 && min > max) {
                    toastr.warning('A quantidade mínima não pode ser maior que a máxima');
                    $(this).focus();
                }
            });

            // Initialize default values
            if ($('#Quantity').val() && !$('#DefaultQuantity').val()) {
                $('#DefaultQuantity').val($('#Quantity').val()).addClass('has-value');
            }

            // Set default values for optional fields
            if (!$('#MinQuantity').val() || $('#MinQuantity').val() == '0') {
                $('#MinQuantity').val(1).addClass('has-value');
            }
            if (!$('#DefaultQuantity').val() || $('#DefaultQuantity').val() == '0') {
                $('#DefaultQuantity').val(1).addClass('has-value');
            }
            if (!$('#Quantity').val() || $('#Quantity').val() == '0') {
                $('#Quantity').val(1).addClass('has-value');
            }
        },

        // Initialize Group Item Create Form
        initializeCreateForm: function() {
            // Reuse common form initialization
            this.initializeForm();
        },

        // Initialize Group Item Edit Form
        initializeEditForm: function() {
            // Reuse common form initialization
            this.initializeForm();
        }
    },



    // Product Group Exchange Rule Management
    exchangeRules: {
        // Show item exchange rules modal (reusing existing view)
        showItemExchangeRulesModal: function(productGroupId, itemId) {
            $.ajax({
                url: `/ProductGroup/ItemExchangeRules/${productGroupId}/${itemId}`,
                type: 'GET',
                success: function(data) {
                    // Criar modal genérico
                    var modalHtml = `
                        <div class="modal fade" id="itemExchangeRulesModal" tabindex="-1" aria-labelledby="itemExchangeRulesModalLabel" aria-hidden="true">
                            <div class="modal-dialog modal-xl">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title" id="itemExchangeRulesModalLabel">
                                            <i class="fas fa-exchange-alt"></i> Regras de Troca do Item
                                        </h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
                    
                    // Remove modal existente se houver
                    $('#itemExchangeRulesModal').remove();
                    
                    // Adicionar modal ao DOM
                    $('body').append(modalHtml);
                    
                    // Mostrar modal
                    $('#itemExchangeRulesModal').modal('show');
                    
                    // Clean up when modal is closed
                    $('#itemExchangeRulesModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de regras de troca do item:', xhr);
                    toastr.error('Erro ao carregar regras de troca do item');
                }
            });
        },

        // Load exchange rules info for item in grid
        loadExchangeRulesInfo: function(itemId) {
            $.ajax({
                url: `/ProductGroup/ExchangeRulesInfo/${itemId}`,
                type: 'GET',
                success: function(response) {
                    if (response.success) {
                        $(`#exchange-rules-info-${itemId}`).html(response.html);
                    } else {
                        $(`#exchange-rules-info-${itemId}`).html('<span class="text-muted">Erro</span>');
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao carregar informações de regras de troca:', xhr);
                    $(`#exchange-rules-info-${itemId}`).html('<span class="text-muted">Erro</span>');
                }
            });
        },

        // Show create exchange rule modal
        showCreateModal: function(productGroupId, itemId, type) {
            // Construir URL com ou sem sourceItemId
            var url = `/ProductGroup/FormularioGroupExchangeRule/${productGroupId}`;
            if (itemId) {
                url += `?sourceItemId=${itemId}`;
            }
            
            $.ajax({
                url: url,
                type: 'GET',
                success: function(data) {
                    // Remove modal existente se houver
                    $('#createGroupExchangeRuleModal').remove();
                    
                    // Adicionar modal ao DOM
                    $('body').append(data);
                    
                    // Configurar evento do botão salvar
                    $('#btnSaveGroupExchangeRule').off('click').on('click', function() {
                        productGroupManager.exchangeRules.save();
                    });
                    
                    // Mostrar modal
                    $('#createGroupExchangeRuleModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#createGroupExchangeRuleModal').on('shown.bs.modal', function () {
                        productGroupManager.exchangeRules.initializeForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#createGroupExchangeRuleModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de criação:', xhr);
                    toastr.error('Erro ao abrir formulário');
                }
            });
        },

        // Show edit exchange rule modal
        showEditModal: function(ruleId) {
            $.ajax({
                url: `/ProductGroup/FormularioEdicaoGroupExchangeRule/${ruleId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="editExchangeRuleModal" tabindex="-1">
                            <div class="modal-dialog modal-lg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"><i class="fas fa-edit"></i> Editar Regra de Troca</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                        <button type="button" class="btn btn-primary" onclick="productGroupManager.exchangeRules.saveEdit()">
                                            <i class="fas fa-save"></i> Salvar Alterações
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#editExchangeRuleModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#editExchangeRuleModal').on('shown.bs.modal', function () {
                        productGroupManager.items.initializeForm(); // Reuse item form init (has Select2)
                    });
                    
                    // Clean up when modal is closed
                    $('#editExchangeRuleModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de edição:', xhr);
                    toastr.error('Erro ao abrir formulário de edição');
                }
            });
        },

        // Save exchange rule
        save: function() {
            const form = $('#formCreateGroupExchangeRule')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            const formData = new FormData(form);
            const productGroupId = formData.get('ProductGroupId');

            // Disable submit button
            const submitBtn = $('#btnSaveGroupExchangeRule');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

            $.ajax({
                url: '/ProductGroup/SalvarGroupExchangeRule',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#createGroupExchangeRuleModal').modal('hide');
                        
                        // Atualizar apenas a partial view das regras de troca
                        if (productGroupId) {
                            // Refresh exchange rules list if it exists (legacy)
                            productGroupManager.exchangeRules.refreshExchangeRulesList(productGroupId);
                            
                            // Update exchange rules info in items grid (new approach)
                            setTimeout(() => {
                                $('[id^="exchange-rules-info-"]').each(function() {
                                    const itemId = $(this).attr('id').replace('exchange-rules-info-', '');
                                    if (itemId) {
                                        productGroupManager.exchangeRules.loadExchangeRulesInfo(itemId);
                                    }
                                });
                            }, 100);
                        }
                    } else {
                        toastr.error(response.message);
                        // Show validation errors if present
                        if (response.errors) {
                            productGroupManager.utils.showValidationErrors(response.errors, 'createGroupExchangeRuleModal');
                        }
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar regra:', xhr);
                    let errorMsg = 'Erro ao salvar regra de troca';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        productGroupManager.utils.showValidationErrors(xhr.responseJSON.errors, 'createGroupExchangeRuleModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            });
        },

        // Save edit exchange rule
        saveEdit: function() {
            const form = $('#formEditGroupExchangeRule')[0];
            if (!form) {
                toastr.error('Formulário não encontrado');
                return;
            }

            const formData = new FormData(form);
            const ruleId = $('#formEditGroupExchangeRule input[name="Id"]').val();
            const productGroupId = $('#formEditGroupExchangeRule input[name="ProductGroupId"]').val();

            // Disable submit button
            const submitBtn = $('#editExchangeRuleModal .btn-primary');
            const originalText = submitBtn.html();
            submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Salvando...');

            $.ajax({
                url: `/ProductGroup/SalvarEdicaoGroupExchangeRule/${ruleId}`,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#editExchangeRuleModal').modal('hide');
                        
                        // Atualizar apenas a partial view das regras de troca
                        if (productGroupId) {
                            // Refresh exchange rules list if it exists (legacy)
                            productGroupManager.exchangeRules.refreshExchangeRulesList(productGroupId);
                            
                            // Update exchange rules info in items grid (new approach)
                            setTimeout(() => {
                                $('[id^="exchange-rules-info-"]').each(function() {
                                    const itemId = $(this).attr('id').replace('exchange-rules-info-', '');
                                    if (itemId) {
                                        productGroupManager.exchangeRules.loadExchangeRulesInfo(itemId);
                                    }
                                });
                            }, 100);
                        }
                    } else {
                        toastr.error(response.message);
                        // Show validation errors if present
                        if (response.errors) {
                            productGroupManager.utils.showValidationErrors(response.errors, 'editExchangeRuleModal');
                        }
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar edição:', xhr);
                    let errorMsg = 'Erro ao salvar alterações da regra';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    } else if (xhr.responseJSON && xhr.responseJSON.errors) {
                        productGroupManager.utils.showValidationErrors(xhr.responseJSON.errors, 'editExchangeRuleModal');
                        errorMsg = 'Verifique os dados informados';
                    }
                    
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            });
        },

        // Confirm delete exchange rule
        confirmDelete: function(ruleId, originalProduct, exchangeProduct) {
            if (confirm(`Tem certeza que deseja excluir a regra de troca de "${originalProduct}" por "${exchangeProduct}"?`)) {
                this.delete(ruleId);
            }
        },

        // Delete exchange rule
        delete: function(ruleId) {
            // Get antiforgery token
            const token = $('input[name="__RequestVerificationToken"]').val();
            if (!token) {
                toastr.error('Token de segurança não encontrado');
                return;
            }

            $.ajax({
                url: `/ProductGroup/ExcluirGroupExchangeRule/${ruleId}`,
                type: 'POST',
                data: { __RequestVerificationToken: token },
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        
                        // Atualizar a lista de regras de troca após exclusão
                        const productGroupId = $('.product-edit-container').data('product-id');
                        if (productGroupId) {
                            // Refresh exchange rules list if it exists (legacy)
                            productGroupManager.exchangeRules.refreshExchangeRulesList(productGroupId);
                            
                            // Update exchange rules info in items grid (new approach)
                            setTimeout(() => {
                                $('[id^="exchange-rules-info-"]').each(function() {
                                    const itemId = $(this).attr('id').replace('exchange-rules-info-', '');
                                    if (itemId) {
                                        productGroupManager.exchangeRules.loadExchangeRulesInfo(itemId);
                                    }
                                });
                            }, 100);
                        }
                    } else {
                        toastr.error(response.message || 'Erro ao excluir regra');
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao excluir regra:', xhr);
                    const errorMsg = xhr.responseJSON?.message || 'Erro ao excluir regra de troca';
                    toastr.error(errorMsg);
                }
            });
        },

        // Refresh exchange rules list (partial view update)
        refreshExchangeRulesList: function(productGroupId) {
            console.log('Refreshing exchange rules list for productGroupId:', productGroupId);
            
            if (!productGroupId) {
                console.error('ProductGroupId not provided for refresh');
                return;
            }

            // Show loading indicator
            const container = $('#exchangeRulesContainer');
            if (container.length === 0) {
                console.warn('Container #exchangeRulesContainer not found');
                return;
            }

            const originalContent = container.html();
            container.html('<div class="text-center p-4"><i class="fas fa-spinner fa-spin"></i> Carregando...</div>');

            $.ajax({
                url: `/ProductGroup/ProductGroupExchangeRules/${productGroupId}`,
                type: 'GET',
                success: function(data) {
                    container.html(data);
                    console.log('Exchange rules list refreshed successfully');
                },
                error: function(xhr) {
                    console.error('Erro ao atualizar lista de regras de troca:', xhr);
                    container.html(originalContent);
                    toastr.error('Erro ao atualizar lista de regras de troca');
                }
            });
        },

        // Toggle exchange rule status
        toggleStatus: function(ruleId, isActive) {
            // This would require a new endpoint in the controller
            console.log(`Toggle exchange rule ${ruleId} to ${isActive ? 'active' : 'inactive'}`);
            toastr.info('Funcionalidade de ativar/desativar em desenvolvimento');
        },

        // Initialize ProductGroup Exchange Rule form
        initializeForm: function() {
            // Initialize floating labels first
            productGroupManager.initializeFloatingLabels();
            
            // Initialize exchange rule form functionality
            this.initializeExchangeRuleForm();
            
            // Initialize autocomplete for Original Product field
            const originalProductField = $('#OriginalProductId');
            if (originalProductField.length) {
                // Remove any existing select2 or autocomplete
                if (originalProductField.hasClass('select2-hidden-accessible')) {
                    originalProductField.select2('destroy');
                }

                // Convert to autocomplete input
                const originalWrapper = originalProductField.closest('.floating-input-group');
                originalWrapper.html(`
                    <input type="hidden" id="OriginalProductId" name="OriginalProductId" />
                    <input id="OriginalProductName" type="text" class="floating-input" placeholder=" " required autocomplete="off" />
                    <label for="OriginalProductName" class="floating-label">Produto Original</label>
                    <span class="text-danger"></span>
                `);

                this.setupProductAutocomplete('#OriginalProductName', '#OriginalProductId');
            }

            // Initialize autocomplete for Exchange Product field
            const exchangeProductField = $('#ExchangeProductId');
            if (exchangeProductField.length) {
                // Remove any existing select2 or autocomplete
                if (exchangeProductField.hasClass('select2-hidden-accessible')) {
                    exchangeProductField.select2('destroy');
                }

                // Convert to autocomplete input
                const exchangeWrapper = exchangeProductField.closest('.floating-input-group');
                exchangeWrapper.html(`
                    <input type="hidden" id="ExchangeProductId" name="ExchangeProductId" />
                    <input id="ExchangeProductName" type="text" class="floating-input" placeholder=" " required autocomplete="off" />
                    <label for="ExchangeProductName" class="floating-label">Produto de Troca</label>
                    <span class="text-danger"></span>
                `);

                this.setupProductAutocomplete('#ExchangeProductName', '#ExchangeProductId');
            }

            // Handle floating labels for input fields
            $('.floating-input').on('focus blur keyup change input', function() {
                if ($(this).val() !== '' && $(this).val() !== null) {
                    $(this).addClass('has-value');
                } else {
                    $(this).removeClass('has-value');
                }
            });

            // Initialize floating labels on page load
            $('.floating-input').each(function() {
                if ($(this).val() !== '' && $(this).val() !== null) {
                    $(this).addClass('has-value');
                }
            });

            // Special handling for number inputs
            $('.floating-input[type="number"]').on('change', function() {
                if ($(this).val() !== '' && $(this).val() !== null && $(this).val() !== '0') {
                    $(this).addClass('has-value');
                } else {
                    $(this).removeClass('has-value');
                }
            });

            // Set default values
            if (!$('#ExchangeRatio').val()) {
                $('#ExchangeRatio').val(1.00).addClass('has-value');
            }
            if (!$('#AdditionalCost').val()) {
                $('#AdditionalCost').val(0.00).addClass('has-value');
            }
        },

        // Setup product autocomplete for a specific field
        setupProductAutocomplete: function(nameFieldSelector, idFieldSelector) {
            const nameField = $(nameFieldSelector);
            const idField = $(idFieldSelector);
            
            if (nameField.length) {
                // Remove previous instance if exists
                if (nameField.data('aaAutocomplete')) {
                    nameField.autocomplete.destroy();
                }

                // Initialize Algolia Autocomplete.js
                const autocompleteInstance = autocomplete(nameField[0], {
                    hint: false,
                    debug: false,
                    minLength: 2,
                    openOnFocus: false,
                    autoselect: true,
                    appendTo: nameField.closest('.modal-body, .tab-pane, body')[0]
                }, [{
                    source: function(query, callback) {
                        $.ajax({
                            url: '/Product/BuscaProductAutocomplete',
                            type: 'GET',
                            dataType: 'json',
                            data: { termo: query },
                            success: function(data) {
                                const suggestions = $.map(data, function(item) {
                                    return {
                                        label: item.label || (item.value + (item.sku ? ' - ' + item.sku : '')),
                                        value: item.label || (item.value + (item.sku ? ' - ' + item.sku : '')),
                                        id: item.id,
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
                    displayKey: 'value',
                    templates: {
                        suggestion: function(suggestion) {
                            return '<div class="autocomplete-suggestion">' + suggestion.label + '</div>';
                        }
                    }
                }]);

                // Events
                autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
                    idField.val(suggestion.id);
                    nameField.val(suggestion.value);
                    nameField.addClass('has-value');
                    nameField.trigger('change');
                });

                autocompleteInstance.on('autocomplete:empty', function() {
                    idField.val('');
                    nameField.removeClass('has-value');
                });

                // Add placeholder and styles
                nameField.attr('placeholder', ' ');
                nameField.addClass('form-control');

                // Handle floating label behavior
                nameField.on('focus blur keyup change input', function() {
                    if ($(this).val() !== '') {
                        $(this).addClass('has-value');
                    } else {
                        $(this).removeClass('has-value');
                    }
                });

                // Initialize floating label state
                if (nameField.val() !== '') {
                    nameField.addClass('has-value');
                }
            }
        },

        // Setup category autocomplete for a specific field
        setupCategoryAutocomplete: function(nameFieldSelector, idFieldSelector) {
            const nameField = $(nameFieldSelector);
            const idField = $(idFieldSelector);
            
            if (nameField.length === 0) return;
            
            // Destroy existing autocomplete instance if any
            if (nameField.data('aaAutocomplete')) {
                nameField.autocomplete.destroy();
            }
            
            // Initialize Algolia Autocomplete.js for Product Category
            const autocompleteInstance = autocomplete(nameField[0], {
                hint: false,
                debug: false,
                minLength: 2,
                openOnFocus: false,
                autoselect: true,
                appendTo: nameField.closest('.modal-body, .tab-pane, body')[0]
            }, [{
                source: function(query, callback) {
                    $.ajax({
                        url: '/ProductCategory/BuscaProductCategoryAutocomplete',
                        type: 'GET',
                        dataType: 'json',
                        data: { termo: query },
                        success: function(data) {
                            const suggestions = $.map(data, function(item) {
                                return {
                                    label: item.name || item.label,
                                    value: item.name || item.label,
                                    id: item.id,
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

            // Handle selection for Category
            autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
                if (suggestion && suggestion.id) {
                    idField.val(suggestion.id);
                    nameField.addClass('has-value');
                    nameField.trigger('change');
                }
            });

            // Clear hidden field when input is cleared
            nameField.on('input', function() {
                if ($(this).val().trim() === '') {
                    idField.val('');
                    $(this).removeClass('has-value');
                }
            });
        },

        // Initialize group item form (for _CreateGroupItem and _EditGroupItem)
        initializeGroupItemForm: function() {
            // Handle item type change
            $('input[name="itemType"]').off('change.groupItem').on('change.groupItem', function() {
                const selectedType = $(this).val();
                
                if (selectedType === 'Produto') {
                    $('#productSelection').show();
                    $('#categorySelection').hide();
                    $('#ProductId').val('');
                    $('#ProductName').val('').removeClass('has-value');
                    $('#ProductCategoryId').val('');
                    $('#ProductCategoryName').val('').removeClass('has-value');
                } else if (selectedType === 'Categoria') {
                    $('#productSelection').hide();
                    $('#categorySelection').show();
                    $('#ProductId').val('');
                    $('#ProductName').val('').removeClass('has-value');
                    $('#ProductCategoryId').val('');
                    $('#ProductCategoryName').val('').removeClass('has-value');
                }
            });

            // Initialize ProductCategory autocomplete
            this.setupCategoryAutocomplete('#ProductCategoryName', '#ProductCategoryId');
        },

        // Setup group item autocomplete for exchange rules
        setupGroupItemAutocomplete: function(nameFieldSelector, idFieldSelector) {
            const nameField = $(nameFieldSelector);
            const idField = $(idFieldSelector);
            
            if (nameField.length === 0 || nameField.prop('readonly')) return;
            
            // Destroy existing autocomplete instance if any
            if (nameField.data('aaAutocomplete')) {
                nameField.autocomplete.destroy();
            }
            
            // Initialize Algolia Autocomplete.js for Group Item
            const autocompleteInstance = autocomplete(nameField[0], {
                hint: false,
                debug: false,
                minLength: 2,
                openOnFocus: false,
                autoselect: true,
                appendTo: nameField.closest('.modal-body, .tab-pane, body')[0]
            }, [{
                source: function(query, callback) {
                    const productGroupId = $('#ProductGroupId').val();
                    $.ajax({
                        url: '/ProductGroup/BuscaGroupItemAutocomplete',
                        type: 'GET',
                        dataType: 'json',
                        data: { termo: query, productGroupId: productGroupId },
                        success: function(data) {
                            const suggestions = $.map(data, function(item) {
                                return {
                                    label: item.label,
                                    value: item.value,
                                    id: item.id,
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
                               '<div class="suggestion-title">' + suggestion.data.productName + '</div>' +
                               (suggestion.data.productSKU ? '<div class="suggestion-subtitle">SKU: ' + suggestion.data.productSKU + '</div>' : '') +
                               '<div class="suggestion-subtitle">Quantidade: ' + suggestion.data.weight + '</div>' +
                               '</div>';
                    }
                }
            }]);

            // Handle selection
            autocompleteInstance.on('autocomplete:selected', function(event, suggestion, dataset) {
                if (suggestion && suggestion.id) {
                    idField.val(suggestion.id);
                    nameField.addClass('has-value');
                    nameField.trigger('change');
                }
            });

            // Clear hidden field when input is cleared
            nameField.on('input', function() {
                if ($(this).val().trim() === '') {
                    idField.val('');
                    $(this).removeClass('has-value');
                }
            });
        },

        // Initialize exchange rule form (for _CreateGroupExchangeRule and _EditGroupExchangeRule)
        initializeExchangeRuleForm: function() {
            // ✅ CORREÇÃO: Verificar se SourceGroupItemName é readonly antes de aplicar autocomplete
            const sourceItemField = $('#SourceGroupItemName');
            if (sourceItemField.length && !sourceItemField.prop('readonly')) {
                // Initialize autocomplete apenas se não for readonly (item predefinido)
                this.setupGroupItemAutocomplete('#SourceGroupItemName', '#SourceGroupItemId');
            } else if (sourceItemField.length && sourceItemField.prop('readonly')) {
                // ✅ GARANTIR: Se é readonly, marcar como has-value para floating label
                sourceItemField.addClass('has-value');
                sourceItemField.closest('.floating-input-group').addClass('has-value');
            }
            
            // Initialize autocomplete for target group item (sempre necessário)
            this.setupGroupItemAutocomplete('#TargetGroupItemName', '#TargetGroupItemId');
        },

        // Initialize Exchange Rule Create Form
        initializeCreateForm: function() {
            // Inicializar Select2 para produto original
            $('.select2-original-product').select2({
                placeholder: 'Selecione o produto original',
                allowClear: true,
                ajax: {
                    url: '/Product/BuscaProductAutocomplete',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            termo: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        return {
                            results: data.map(function(item) {
                                return {
                                    id: item.id,
                                    text: item.name + (item.sku ? ' - ' + item.sku : ''),
                                    data: item
                                };
                            })
                        };
                    },
                    cache: true
                }
            });

            // Inicializar Select2 para produto de troca
            $('.select2-exchange-product').select2({
                placeholder: 'Selecione o produto de troca',
                allowClear: true,
                ajax: {
                    url: '/Product/BuscaProductAutocomplete',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            termo: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        return {
                            results: data.map(function(item) {
                                return {
                                    id: item.id,
                                    text: item.name + (item.sku ? ' - ' + item.sku : ''),
                                    data: item
                                };
                            })
                        };
                    },
                    cache: true
                }
            });

            // Validar que produtos são diferentes
            $('#OriginalProductId, #ExchangeProductId').on('change', function() {
                const originalId = $('#OriginalProductId').val();
                const exchangeId = $('#ExchangeProductId').val();
                
                if (originalId && exchangeId && originalId === exchangeId) {
                    toastr.warning('O produto original e o produto de troca devem ser diferentes');
                    $(this).val('').trigger('change');
                }
            });

            // Valor padrão para proporção
            if (!$('#ExchangeRatio').val()) {
                $('#ExchangeRatio').val('1.00');
            }

            // Valor padrão para custo adicional
            if (!$('#AdditionalCost').val()) {
                $('#AdditionalCost').val('0.00');
            }
        },

        // Initialize Exchange Rule Edit Form
        initializeEditForm: function() {
            // Reuse create form initialization
            this.initializeCreateForm();
        }
    },

    // Utility methods
    utils: {
        // Show validation errors
        showValidationErrors: function(errors, modalId) {
            const $modal = $(`#${modalId}`);
            
            // Clear previous errors
            $modal.find('.text-danger').text('');
            $modal.find('.is-invalid').removeClass('is-invalid');
            
            // Show new errors
            Object.keys(errors).forEach(field => {
                const $field = $modal.find(`[name="${field}"]`);
                
                // Handle different error formats
                let errorText = '';
                const errorValue = errors[field];
                
                if (Array.isArray(errorValue)) {
                    // Array of error messages
                    errorText = errorValue.join(', ');
                } else if (typeof errorValue === 'object' && errorValue !== null) {
                    // Object with error information (ModelState format)
                    if (errorValue.errors && Array.isArray(errorValue.errors)) {
                        errorText = errorValue.errors.join(', ');
                    } else if (errorValue.errorMessage) {
                        errorText = errorValue.errorMessage;
                    } else {
                        errorText = 'Erro de validação';
                    }
                } else if (typeof errorValue === 'string') {
                    // Simple string error
                    errorText = errorValue;
                } else {
                    // Fallback for unknown formats
                    errorText = 'Erro de validação';
                }
                
                if ($field.length) {
                    $field.addClass('is-invalid');
                    
                    let $errorContainer = $field.siblings('.text-danger');
                    if ($errorContainer.length === 0) {
                        $errorContainer = $('<span class="text-danger"></span>');
                        $field.parent().append($errorContainer);
                    }
                    
                    $errorContainer.text(errorText);
                }
            });
        }
    }
};

    // Global aliases for backward compatibility
window.ProductGroup = {
    // Index
    initializeIndex: () => productGroupManager.initializeIndex(),
    
    // Create
    initializeCreate: () => productGroupManager.initializeCreate(),
    
    // Items
    showCreateItemModal: (id) => productGroupManager.items.showCreateModal(id),
    showEditItemModal: (id) => productGroupManager.items.showEditModal(id),
    saveGroupItem: () => productGroupManager.items.save(),
    saveEditGroupItem: () => productGroupManager.items.saveEdit(),
    confirmDeleteItem: (id, name) => productGroupManager.items.confirmDelete(id, name),
    deleteGroupItem: (id) => productGroupManager.items.delete(id),
    initializeGroupItemForm: () => productGroupManager.items.initializeForm(),
    initializeGroupItemCreateForm: () => productGroupManager.items.initializeCreateForm(),
    initializeGroupItemEditForm: () => productGroupManager.items.initializeEditForm(),
    
    // Exchange Rules - New item-based approach
    showItemExchangeRulesModal: (productGroupId, itemId) => productGroupManager.exchangeRules.showItemExchangeRulesModal(productGroupId, itemId),
    loadExchangeRulesInfo: (itemId) => productGroupManager.exchangeRules.loadExchangeRulesInfo(itemId),
    
    // Exchange Rules - Legacy methods (kept for backward compatibility)
    showCreateExchangeRuleModal: (id, itemId, type) => productGroupManager.exchangeRules.showCreateModal(id, itemId, type),
    showEditExchangeRuleModal: (id) => productGroupManager.exchangeRules.showEditModal(id),
    saveExchangeRule: () => productGroupManager.exchangeRules.save(),
    saveEditExchangeRule: () => productGroupManager.exchangeRules.saveEdit(),
    confirmDeleteExchangeRule: (id, orig, exch) => productGroupManager.exchangeRules.confirmDelete(id, orig, exch),
    deleteExchangeRule: (id) => productGroupManager.exchangeRules.delete(id),
    toggleExchangeRuleStatus: (id, active) => productGroupManager.exchangeRules.toggleStatus(id, active),
    initializeExchangeRuleCreateForm: () => productGroupManager.exchangeRules.initializeCreateForm(),
    initializeExchangeRuleEditForm: () => productGroupManager.exchangeRules.initializeEditForm()
};

// Export for use in Product.js (for productManager.productGroup compatibility)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = productGroupManager;
}

// Auto-inicialização quando o DOM estiver pronto
$(function() {
    // Inicializar floating labels globalmente
    productGroupManager.initializeFloatingLabels();
    
    // Auto-detectar e inicializar tabela de índice
    if ($('#groupsTable').length > 0) {
        productGroupManager.initializeIndex();
    }
    
    // Auto-detectar e inicializar formulário de criação
    if ($('#Price').length > 0 && $('#UnitPrice').length > 0) {
        productGroupManager.initializeCreate();
    }
}); 