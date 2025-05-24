var usersManager = {
    currentView: 'cards', // 'cards' ou 'table'
    
    init: function() {
        // Inicializar eventos dos modais
        $(document).on('shown.bs.modal', '#modalContainer', function () {
            usersManager.initializeForm();
            
            // Inicializar select múltiplo com Select2
            $('.form-select[multiple]').select2({
                theme: 'bootstrap-5',
                width: '100%',
                placeholder: 'Selecione as funções'
            });

            // Aguardar um pouco para garantir que tudo foi carregado
            setTimeout(function() {
                // Inicializar os selects de claims existentes
                $('.claim-type').each(function() {
                    usersManager.updateClaimValues(this);
                });
            }, 200);
        });

        $(document).on('hidden.bs.modal', '#modalContainer', function () {
            $(this).find('.modal-content').html('');
        });

        // Configurar toastr
        toastr.options = {
            "closeButton": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "timeOut": "3000"
        };

        // Configurar eventos para claims
        $(document).on('change', '.claim-type', function() {
            usersManager.updateClaimValues(this);
        });

        // Inicializar funcionalidades da grid
        usersManager.initializeGrid();
    },

    initializeGrid: function() {
        // Busca em tempo real
        $('#searchInput').on('input', function() {
            usersManager.filterUsers();
        });

        // Filtros
        $('#roleFilter, #statusFilter').on('change', function() {
            usersManager.filterUsers();
        });

        // Toggle de visualização
        $('#btnCardView').on('click', function() {
            usersManager.switchView('cards');
        });

        $('#btnTableView').on('click', function() {
            usersManager.switchView('table');
        });

        // Inicializar view padrão
        usersManager.switchView('cards');
    },

    switchView: function(viewType) {
        usersManager.currentView = viewType;
        
        // Atualizar botões
        $('.view-toggle .btn').removeClass('active');
        if (viewType === 'cards') {
            $('#btnCardView').addClass('active');
            $('#cardsView').removeClass('d-none');
            $('#tableView').addClass('d-none');
        } else {
            $('#btnTableView').addClass('active');
            $('#cardsView').addClass('d-none');
            $('#tableView').removeClass('d-none');
        }
    },

    filterUsers: function() {
        const searchTerm = $('#searchInput').val().toLowerCase();
        const roleFilter = $('#roleFilter').val().toLowerCase();
        const statusFilter = $('#statusFilter').val().toLowerCase();

        let visibleCount = 0;

        $('.user-item').each(function() {
            const $item = $(this);
            const searchData = $item.data('search') || '';
            const rolesData = $item.data('roles') || '';
            const statusData = $item.data('status') || '';

            let showItem = true;

            // Filtro de busca
            if (searchTerm && !searchData.includes(searchTerm)) {
                showItem = false;
            }

            // Filtro de role
            if (roleFilter && !rolesData.includes(roleFilter)) {
                showItem = false;
            }

            // Filtro de status
            if (statusFilter && statusData !== statusFilter) {
                showItem = false;
            }

            if (showItem) {
                $item.show();
                visibleCount++;
            } else {
                $item.hide();
            }
        });

        // Mostrar/ocultar empty state
        if (visibleCount === 0) {
            $('#emptyState').show();
            $('#cardsView, #tableView').hide();
        } else {
            $('#emptyState').hide();
            if (usersManager.currentView === 'cards') {
                $('#cardsView').show();
            } else {
                $('#tableView').show();
            }
        }
    },

    clearFilters: function() {
        $('#searchInput').val('');
        $('#roleFilter').val('');
        $('#statusFilter').val('');
        usersManager.filterUsers();
    },

    applyFilters: function() {
        // Fechar o collapse de filtros avançados
        $('#advancedFilters').collapse('hide');
        usersManager.filterUsers();
    },

    atualizarGrid: function() {
        $.ajax({
            url: '/Admin/Users/GridPartial',
            type: 'GET',
            success: function(data) {
                $('#gridContainer').html(data);
                // Reinicializar a view atual após recarregar a grid
                usersManager.switchView(usersManager.currentView);
            },
            error: function(xhr, status, error) {
                console.error('Erro ao carregar grid:', xhr.responseText);
                toastr.error('Erro ao atualizar a lista de usuários: ' + (xhr.responseText || error));
            }
        });
    },

    visualizarUserModal: function(userId) {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Users/DetailsPartial/' + userId,
            type: 'GET',
            success: function(data) {
                $modal.find('.modal-content').html(data);
            },
            error: function() {
                toastr.error('Erro ao carregar os detalhes do usuário.');
                $modal.modal('hide');
            }
        });
    },

    novoUserModal: function() {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Users/CreatePartial',
            type: 'GET',
            success: function(data) {
                $modal.find('.modal-content').html(data);
            },
            error: function() {
                toastr.error('Erro ao carregar o formulário de criação.');
                $modal.modal('hide');
            }
        });
    },

    editarUserModal: function(userId) {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Users/Edit/' + userId,
            type: 'GET',
            success: function(data) {
                $modal.find('.modal-content').html(data);
            },
            error: function() {
                toastr.error('Erro ao carregar o formulário de edição.');
                $modal.modal('hide');
            }
        });
    },

    excluirUserModal: function(userId) {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Users/Delete/' + userId,
            type: 'GET',
            success: function(data) {
                $modal.find('.modal-content').html(data);
            },
            error: function() {
                toastr.error('Erro ao carregar o formulário de exclusão.');
                $modal.modal('hide');
            }
        });
    },

    salvarNovo: function(form) {
        if (!$(form).valid()) {
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        submitButton.prop('disabled', true);
        loadingSpinner.removeClass('d-none');

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function(response) {
                if (response.success) {
                    $('#modalContainer').modal('hide');
                    usersManager.atualizarGrid();
                    toastr.success('Usuário criado com sucesso!');
                } else {
                    if (typeof response === 'string') {
                        $('#modalContainer .modal-content').html(response);
                        usersManager.initializeForm();
                    }
                    toastr.error('Erro ao criar usuário. Verifique os dados informados.');
                }
            },
            error: function(xhr) {
                toastr.error(xhr.responseText || 'Erro ao salvar o usuário.');
            },
            complete: function() {
                submitButton.prop('disabled', false);
                loadingSpinner.addClass('d-none');
            }
        });

        return false;
    },

    salvarEdicao: function(form) {
        if (!$(form).valid()) {
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        submitButton.prop('disabled', true);
        loadingSpinner.removeClass('d-none');

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function(response) {
                if (response.success) {
                    $('#modalContainer').modal('hide');
                    usersManager.atualizarGrid();
                    toastr.success('Usuário atualizado com sucesso!');
                } else {
                    if (typeof response === 'string') {
                        $('#modalContainer .modal-content').html(response);
                        usersManager.initializeForm();
                    }
                    toastr.error('Erro ao atualizar usuário. Verifique os dados informados.');
                }
            },
            error: function(xhr) {
                toastr.error(xhr.responseText || 'Erro ao atualizar o usuário.');
            },
            complete: function() {
                submitButton.prop('disabled', false);
                loadingSpinner.addClass('d-none');
            }
        });

        return false;
    },

    confirmarExclusao: function(form) {
        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        submitButton.prop('disabled', true);
        loadingSpinner.removeClass('d-none');

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function(response) {
                if (response.success) {
                    $('#modalContainer').modal('hide');
                    usersManager.atualizarGrid();
                    toastr.success('Usuário excluído com sucesso!');
                } else {
                    toastr.error(response.message || 'Erro ao excluir usuário.');
                }
            },
            error: function(xhr) {
                toastr.error(xhr.responseText || 'Erro ao excluir o usuário.');
            },
            complete: function() {
                submitButton.prop('disabled', false);
                loadingSpinner.addClass('d-none');
            }
        });

        return false;
    },

    addClaim: function() {
        const claimCount = $('.claim-row').length;
        const claimHtml = `
            <div class="claim-row row mb-2">
                <div class="col-5">
                    <select name="Claims[${claimCount}].Type" class="form-select claim-type" required>
                        <option value="">Selecione o tipo</option>
                        <option value="Permission">Permissão</option>
                        <option value="Department">Departamento</option>
                        <option value="AccessLevel">Nível de Acesso</option>
                    </select>
                </div>
                <div class="col-5">
                    <select name="Claims[${claimCount}].Value" class="form-select claim-value" required>
                        <option value="">Selecione o valor</option>
                    </select>
                </div>
                <div class="col-2">
                    <button type="button" class="btn btn-danger btn-sm" onclick="usersManager.removeClaim(this)">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </div>`;
        
        const $newRow = $(claimHtml);
        $('#claimsContainer').append($newRow);
        
        // Inicializar os eventos para a nova linha
        const $typeSelect = $newRow.find('.claim-type');
        $typeSelect.on('change', function() {
            usersManager.updateClaimValues(this);
        });
    },

    removeClaim: function(button) {
        $(button).closest('.claim-row').remove();
        // Reindexar os campos
        $('.claim-row').each(function(index) {
            $(this).find('select[name*=".Type"]').attr('name', `Claims[${index}].Type`);
            $(this).find('select[name*=".Value"]').attr('name', `Claims[${index}].Value`);
        });
    },

    updateClaimValues: function(selectType) {
        const typeValue = $(selectType).val();
        const valueSelect = $(selectType).closest('.claim-row').find('.claim-value');
        const currentValue = valueSelect.val(); // Guardar o valor atual
        
        valueSelect.empty().append('<option value="">Selecione o valor</option>');

        if (typeValue && window.availableClaimValues && window.availableClaimValues[typeValue]) {
            window.availableClaimValues[typeValue].forEach(value => {
                const option = new Option(value, value, false, value === currentValue);
                valueSelect.append(option);
            });
        }
    },

    initializeForm: function() {
        if ($('#editUserForm').length) {
            $.validator.unobtrusive.parse('#editUserForm');
        }
        if ($('#createUserForm').length) {
            $.validator.unobtrusive.parse('#createUserForm');
        }
        
        // Aguardar um pouco para garantir que o DOM foi carregado
        setTimeout(function() {
            // Inicializar claims existentes após carregar o form
            $('.claim-type').each(function() {
                usersManager.updateClaimValues(this);
            });
        }, 100);
    }
};

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    usersManager.init();
}); 