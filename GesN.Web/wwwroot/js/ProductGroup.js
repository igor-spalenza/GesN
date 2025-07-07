// ProductGroup Management JavaScript
const productGroupManager = {
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
                    const modalHtml = `
                        <div class="modal fade" id="createGroupItemModal" tabindex="-1">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"><i class="fas fa-plus"></i> Adicionar Item ao Grupo</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                        <button type="button" class="btn btn-primary" onclick="productGroupManager.items.save()">
                                            <i class="fas fa-save"></i> Salvar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#createGroupItemModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#createGroupItemModal').on('shown.bs.modal', function () {
                        productGroupManager.items.initializeForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#createGroupItemModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
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

            const formData = new FormData(form);

            // Disable submit button
            const submitBtn = $('#createGroupItemModal .btn-primary');
            const originalText = submitBtn.text();
            submitBtn.prop('disabled', true).text('Salvando...');

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
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
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
                    const errorMsg = xhr.responseJSON?.message || 'Erro ao salvar item do grupo';
                    toastr.error(errorMsg);
                },
                complete: function() {
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).text(originalText);
                }
            });
        },

        // Save edit group item
        saveEdit: function() {
            const form = $('#formEditGroupItem')[0];
            const formData = new FormData(form);
            const itemId = $('#formEditGroupItem input[name="Id"]').val();

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
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar edição:', xhr);
                    toastr.error('Erro ao salvar alterações do item');
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
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
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

        // Initialize ProductGroup Item form
        initializeForm: function() {
            // Initialize Select2 for products
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
                                    text: item.name + (item.sku ? ' - ' + item.sku : ''),
                                    data: item
                                };
                            })
                        };
                    },
                    cache: true
                }
            });

            // Sync default quantity with quantity when changed
            $('#Quantity').off('change.productGroup').on('change.productGroup', function() {
                const quantity = $(this).val();
                if (quantity && !$('#DefaultQuantity').val()) {
                    $('#DefaultQuantity').val(quantity);
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

    // Product Group Option Management
    options: {
        // Show create option modal
        showCreateModal: function(productGroupId) {
            $.ajax({
                url: `/ProductGroup/FormularioGroupOption/${productGroupId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="createGroupOptionModal" tabindex="-1">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"><i class="fas fa-plus"></i> Adicionar Opção de Configuração</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                        <button type="button" class="btn btn-primary" onclick="productGroupManager.options.save()">
                                            <i class="fas fa-save"></i> Salvar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#createGroupOptionModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#createGroupOptionModal').on('shown.bs.modal', function () {
                        productGroupManager.options.initializeForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#createGroupOptionModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de criação:', xhr);
                    toastr.error('Erro ao abrir formulário');
                }
            });
        },

        // Show edit option modal
        showEditModal: function(optionId) {
            $.ajax({
                url: `/ProductGroup/FormularioEdicaoGroupOption/${optionId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="editGroupOptionModal" tabindex="-1">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"><i class="fas fa-edit"></i> Editar Opção de Configuração</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                        <button type="button" class="btn btn-primary" onclick="productGroupManager.options.saveEdit()">
                                            <i class="fas fa-save"></i> Salvar Alterações
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#editGroupOptionModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#editGroupOptionModal').on('shown.bs.modal', function () {
                        productGroupManager.options.initializeForm();
                    });
                    
                    // Clean up when modal is closed
                    $('#editGroupOptionModal').on('hidden.bs.modal', function () {
                        $(this).remove();
                    });
                },
                error: function(xhr) {
                    console.error('Erro ao abrir modal de edição:', xhr);
                    toastr.error('Erro ao abrir formulário de edição');
                }
            });
        },

        // Save group option
        save: function() {
            const form = $('#formCreateGroupOption')[0];
            const formData = new FormData(form);

            $.ajax({
                url: '/ProductGroup/SalvarGroupOption',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#createGroupOptionModal').modal('hide');
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar opção:', xhr);
                    toastr.error('Erro ao salvar opção do grupo');
                }
            });
        },

        // Save edit group option
        saveEdit: function() {
            const form = $('#formEditGroupOption')[0];
            const formData = new FormData(form);
            const optionId = $('#formEditGroupOption input[name="Id"]').val();

            $.ajax({
                url: `/ProductGroup/SalvarEdicaoGroupOption/${optionId}`,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#editGroupOptionModal').modal('hide');
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar edição:', xhr);
                    toastr.error('Erro ao salvar alterações da opção');
                }
            });
        },

        // Confirm delete option
        confirmDelete: function(optionId, optionName) {
            if (confirm(`Tem certeza que deseja excluir a opção "${optionName}"?`)) {
                this.delete(optionId);
            }
        },

        // Delete group option
        delete: function(optionId) {
            // Get antiforgery token
            const token = $('input[name="__RequestVerificationToken"]').val();
            if (!token) {
                toastr.error('Token de segurança não encontrado');
                return;
            }

            $.ajax({
                url: `/ProductGroup/ExcluirGroupOption/${optionId}`,
                type: 'POST',
                data: { __RequestVerificationToken: token },
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
                    } else {
                        toastr.error(response.message || 'Erro ao excluir opção');
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao excluir opção:', xhr);
                    const errorMsg = xhr.responseJSON?.message || 'Erro ao excluir opção do grupo';
                    toastr.error(errorMsg);
                }
            });
        },

        // Initialize ProductGroup Option form
        initializeForm: function() {
            // Auto-generate display order if not provided
            if (!$('#DisplayOrder').val()) {
                // Find next available order
                const currentOptions = $('.product-group-section .table tbody tr').length;
                $('#DisplayOrder').val(currentOptions + 1);
            }
        }
    },

    // Product Group Exchange Rule Management
    exchangeRules: {
        // Show create exchange rule modal
        showCreateModal: function(productGroupId) {
            $.ajax({
                url: `/ProductGroup/FormularioGroupExchangeRule/${productGroupId}`,
                type: 'GET',
                success: function(data) {
                    const modalHtml = `
                        <div class="modal fade" id="createExchangeRuleModal" tabindex="-1">
                            <div class="modal-dialog modal-lg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"><i class="fas fa-plus"></i> Adicionar Regra de Troca</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                    </div>
                                    <div class="modal-body">
                                        ${data}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                        <button type="button" class="btn btn-primary" onclick="productGroupManager.exchangeRules.save()">
                                            <i class="fas fa-save"></i> Salvar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    $('body').append(modalHtml);
                    $('#createExchangeRuleModal').modal('show');
                    
                    // Initialize form after modal is shown
                    $('#createExchangeRuleModal').on('shown.bs.modal', function () {
                        productGroupManager.items.initializeForm(); // Reuse item form init (has Select2)
                    });
                    
                    // Clean up when modal is closed
                    $('#createExchangeRuleModal').on('hidden.bs.modal', function () {
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
            const formData = new FormData(form);

            $.ajax({
                url: '/ProductGroup/SalvarGroupExchangeRule',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    if (response.success) {
                        toastr.success(response.message);
                        $('#createExchangeRuleModal').modal('hide');
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar regra:', xhr);
                    toastr.error('Erro ao salvar regra de troca');
                }
            });
        },

        // Save edit exchange rule
        saveEdit: function() {
            const form = $('#formEditGroupExchangeRule')[0];
            const formData = new FormData(form);
            const ruleId = $('#formEditGroupExchangeRule input[name="Id"]').val();

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
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function(xhr) {
                    console.error('Erro ao salvar edição:', xhr);
                    toastr.error('Erro ao salvar alterações da regra');
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
                        // Dados são renderizados diretamente na view - não precisa recarregar via AJAX
                        // TODO: Implementar atualização local da lista se necessário
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

        // Toggle exchange rule status
        toggleStatus: function(ruleId, isActive) {
            // This would require a new endpoint in the controller
            console.log(`Toggle exchange rule ${ruleId} to ${isActive ? 'active' : 'inactive'}`);
            toastr.info('Funcionalidade de ativar/desativar em desenvolvimento');
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
                const errorText = Array.isArray(errors[field]) ? errors[field].join(', ') : errors[field];
                
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
    
    // Options
    showCreateOptionModal: (id) => productGroupManager.options.showCreateModal(id),
    showEditOptionModal: (id) => productGroupManager.options.showEditModal(id),
    saveGroupOption: () => productGroupManager.options.save(),
    saveEditGroupOption: () => productGroupManager.options.saveEdit(),
    confirmDeleteOption: (id, name) => productGroupManager.options.confirmDelete(id, name),
    deleteGroupOption: (id) => productGroupManager.options.delete(id),
    initializeGroupOptionForm: () => productGroupManager.options.initializeForm(),
    
    // Exchange Rules
    showCreateExchangeRuleModal: (id) => productGroupManager.exchangeRules.showCreateModal(id),
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
    // Auto-detectar e inicializar tabela de índice
    if ($('#groupsTable').length > 0) {
        productGroupManager.initializeIndex();
    }
    
    // Auto-detectar e inicializar formulário de criação
    if ($('#Price').length > 0 && $('#UnitPrice').length > 0) {
        productGroupManager.initializeCreate();
    }
}); 