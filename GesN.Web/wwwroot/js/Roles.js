var rolesManager = {
    currentView: 'cards', // 'cards' ou 'table'
    
    init: function() {
        // Inicializar eventos dos modais
        $(document).on('shown.bs.modal', '#modalContainer', function () {
            rolesManager.initializeForm();
            
            // Inicializar select múltiplo com Select2
            $('.form-select[multiple]').select2({
                theme: 'bootstrap-5',
                width: '100%',
                placeholder: 'Selecione as opções'
            });

            // Aguardar um pouco para garantir que tudo foi carregado
            setTimeout(function() {
                // Inicializar os selects de claims existentes
                $('.claim-type').each(function() {
                    rolesManager.updateClaimValues(this);
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
            rolesManager.updateClaimValues(this);
        });

        // Inicializar funcionalidades da grid
        rolesManager.initializeGrid();
    },

    initializeGrid: function() {
        // Busca em tempo real
        $('#searchInput').on('input', function() {
            rolesManager.filterRoles();
        });

        // Filtros
        $('#userCountFilter, #claimFilter').on('change', function() {
            rolesManager.filterRoles();
        });

        // Toggle de visualização
        $('#btnCardView').on('click', function() {
            rolesManager.switchView('cards');
        });

        $('#btnTableView').on('click', function() {
            rolesManager.switchView('table');
        });

        // Inicializar view padrão
        rolesManager.switchView('cards');
    },

    switchView: function(viewType) {
        rolesManager.currentView = viewType;
        
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

    filterRoles: function() {
        const searchTerm = $('#searchInput').val().toLowerCase();
        const userCountFilter = $('#userCountFilter').val();
        const claimFilter = $('#claimFilter').val();

        let visibleCount = 0;

        $('.role-item').each(function() {
            const $item = $(this);
            const searchData = $item.data('search') || '';
            const userCount = parseInt($item.data('user-count')) || 0;
            const hasClaims = $item.data('has-claims') === 'true';

            let showItem = true;

            // Filtro de busca
            if (searchTerm && !searchData.includes(searchTerm)) {
                showItem = false;
            }

            // Filtro de contagem de usuários
            if (userCountFilter && !rolesManager.matchUserCountFilter(userCount, userCountFilter)) {
                showItem = false;
            }

            // Filtro de claims
            if (claimFilter === 'true' && !hasClaims) {
                showItem = false;
            } else if (claimFilter === 'false' && hasClaims) {
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
            if (rolesManager.currentView === 'cards') {
                $('#cardsView').show();
            } else {
                $('#tableView').show();
            }
        }
    },

    matchUserCountFilter: function(userCount, filter) {
        switch (filter) {
            case '0':
                return userCount === 0;
            case '1-5':
                return userCount >= 1 && userCount <= 5;
            case '6-20':
                return userCount >= 6 && userCount <= 20;
            case '20+':
                return userCount > 20;
            default:
                return true;
        }
    },

    clearFilters: function() {
        $('#searchInput').val('');
        $('#userCountFilter').val('');
        $('#claimFilter').val('');
        rolesManager.filterRoles();
    },

    applyFilters: function() {
        // Fechar o collapse de filtros avançados
        $('#advancedFilters').collapse('hide');
        rolesManager.filterRoles();
    },

    atualizarGrid: function() {
        $.ajax({
            url: '/Admin/Roles/GridPartial',
            type: 'GET',
            success: function(data) {
                $('#gridContainer').html(data);
                // Reinicializar a view atual após recarregar a grid
                rolesManager.switchView(rolesManager.currentView);
            },
            error: function(xhr, status, error) {
                console.error('Erro ao carregar grid:', xhr.responseText);
                toastr.error('Erro ao atualizar a lista de roles: ' + (xhr.responseText || error));
            }
        });
    },

    visualizarRoleModal: function(roleId) {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Roles/DetailsPartial/' + roleId,
            type: 'GET',
            success: function(data) {
                $modal.find('.modal-content').html(data);
            },
            error: function() {
                toastr.error('Erro ao carregar os detalhes da role.');
                $modal.modal('hide');
            }
        });
    },

    novoRoleModal: function() {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Roles/CreatePartial',
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

    editarRoleModal: function(roleId) {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Roles/Edit/' + roleId,
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

    excluirRoleModal: function(roleId) {
        const $modal = $('#modalContainer');
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Roles/DeletePartial/' + roleId,
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
        console.log('RolesManager: salvarNovo chamado');
        console.log('Form element:', form);
        console.log('Form jQuery object:', $(form));
        
        // Prevenir múltiplos submits
        if ($(form).data('submitting')) {
            console.log('RolesManager: Já está enviando role, cancelando');
            return false;
        }

        // Remover claims vazias antes da validação
        rolesManager.removeEmptyClaims();

        // Verificar validação padrão
        if (!$(form).valid()) {
            console.log('RolesManager: Formulário inválido na criação de role');
            console.log('Erros de validação:', $(form).find('.field-validation-error').map(function() {
                return $(this).text();
            }).get());
            return false;
        }

        // Regra de negócio: precisa ter pelo menos 1 claim válida (igual ao Claims)
        const validClaims = rolesManager.getClaimsData();
        
        if (validClaims.length === 0) {
            toastr.warning('Uma role deve ter pelo menos uma claim associada. Adicione pelo menos uma claim antes de salvar.');
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        console.log('RolesManager: Iniciando envio da nova role...');
        console.log('URL:', $form.attr('action'));
        console.log('Dados serializados:', $form.serialize());
        console.log('Claims válidas encontradas:', validClaims);
        console.log('Submit button encontrado:', submitButton.length);
        console.log('Loading spinner encontrado:', loadingSpinner.length);
        
        // Marcar como enviando
        $form.data('submitting', true);
        
        submitButton.prop('disabled', true);
        loadingSpinner.removeClass('d-none');

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function(response) {
                console.log('RolesManager: Resposta da criação de role recebida:', response);
                console.log('Tipo da resposta:', typeof response);
                
                if (response && response.success) {
                    console.log('RolesManager: Role criada com sucesso, fechando modal e atualizando grid');
                    $('#modalContainer').modal('hide');
                    rolesManager.atualizarGrid();
                    toastr.success('Role criada com sucesso!');
                } else {
                    console.log('RolesManager: Resposta indica erro ou formulário com validação');
                    if (typeof response === 'string') {
                        console.log('RolesManager: Atualizando modal com novo HTML');
                        $('#modalContainer .modal-content').html(response);
                        rolesManager.initializeForm();
                    } else {
                        console.log('RolesManager: Mostrando mensagem de erro JSON');
                        const errorMessage = response && response.message ? response.message : 'Erro ao criar role. Verifique os dados informados.';
                        toastr.error(errorMessage);
                        if (response.errors && response.errors.length > 0) {
                            response.errors.forEach(error => {
                                console.error('Erro específico:', error);
                            });
                        }
                    }
                }
            },
            error: function(xhr, status, error) {
                console.error('RolesManager: Erro na requisição de criação de role:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                console.error('Response headers:', xhr.getAllResponseHeaders());
                toastr.error(xhr.responseText || 'Erro ao salvar a role.');
            },
            complete: function() {
                console.log('RolesManager: Requisição de criação de role completa');
                $form.data('submitting', false);
                submitButton.prop('disabled', false);
                loadingSpinner.addClass('d-none');
            }
        });

        return false;
    },

    salvarEdicao: function(form) {
        console.log('ClaimsManager: salvarEdicao chamado');
        console.log('Form element:', form);
        console.log('Form jQuery object:', $(form));
        
        // Prevenir múltiplos submits
        if ($(form).data('submitting')) {
            console.log('ClaimsManager: Já está enviando edição de role, cancelando');
            return false;
        }

        // Verificar validação
        if (!$(form).valid()) {
            console.log('ClaimsManager: Formulário inválido na edição de role');
            console.log('Erros de validação:', $(form).find('.field-validation-error').map(function() {
                return $(this).text();
            }).get());
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        console.log('ClaimsManager: Iniciando envio da edição de role...');
        console.log('URL:', $form.attr('action'));
        console.log('Dados serializados:', $form.serialize());
        console.log('Submit button encontrado:', submitButton.length);
        console.log('Loading spinner encontrado:', loadingSpinner.length);
        
        // Marcar como enviando
        $form.data('submitting', true);
        
        submitButton.prop('disabled', true);
        loadingSpinner.removeClass('d-none');

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function(response) {
                console.log('ClaimsManager: Resposta da edição de role recebida:', response);
                console.log('Tipo da resposta:', typeof response);
                console.log('Response.success:', response ? response.success : 'undefined');
                console.log('É string?', typeof response === 'string');
                console.log('Conteúdo completo da response:', JSON.stringify(response));
                
                if (response && response.success === true) {
                    console.log('ClaimsManager: Role atualizada com sucesso, fechando modal e atualizando grid');
                    $('#modalContainer').modal('hide');
                    rolesManager.atualizarGrid();
                    toastr.success('Role atualizada com sucesso!');
                } else {
                    console.log('ClaimsManager: Resposta indica erro ou formulário com validação');
                    if (typeof response === 'string') {
                        console.log('ClaimsManager: Atualizando modal com novo HTML');
                        $('#modalContainer .modal-content').html(response);
                        rolesManager.initializeForm();
                    } else {
                        console.log('ClaimsManager: Mostrando mensagem de erro JSON');
                        const errorMessage = response && response.message ? response.message : 'Erro ao atualizar role. Verifique os dados informados.';
                        console.log('Mensagem de erro:', errorMessage);
                        toastr.error(errorMessage);
                    }
                }
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro na requisição de edição de role:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                console.error('Response headers:', xhr.getAllResponseHeaders());
                toastr.error(xhr.responseText || 'Erro ao atualizar a role.');
            },
            complete: function() {
                console.log('ClaimsManager: Requisição de edição de role completa');
                $form.data('submitting', false);
                submitButton.prop('disabled', false);
                loadingSpinner.addClass('d-none');
            }
        });

        return false;
    },

    confirmarExclusao: function(form) {
        console.log('confirmarExclusao chamado');
        
        // Prevenir múltiplos submits
        if ($(form).data('submitting')) {
            console.log('Já está enviando exclusão, cancelando');
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        console.log('Iniciando exclusão...');
        console.log('URL:', $form.attr('action'));
        console.log('Dados:', $form.serialize());
        
        // Marcar como enviando
        $form.data('submitting', true);
        
        submitButton.prop('disabled', true);
        loadingSpinner.removeClass('d-none');

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function(response) {
                console.log('Resposta da exclusão recebida:', response);
                if (response.success) {
                    $('#modalContainer').modal('hide');
                    rolesManager.atualizarGrid();
                    toastr.success('Role excluída com sucesso!');
                } else {
                    toastr.error(response.message || 'Erro ao excluir role.');
                }
            },
            error: function(xhr, status, error) {
                console.error('Erro na requisição de exclusão:', {xhr, status, error});
                console.error('Response text:', xhr.responseText);
                toastr.error(xhr.responseText || 'Erro ao excluir a role.');
            },
            complete: function() {
                console.log('Requisição de exclusão completa');
                $form.data('submitting', false);
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
                        ${rolesManager.generateClaimTypeOptions()}
                    </select>
                </div>
                <div class="col-5">
                    <input name="Claims[${claimCount}].Value" class="form-control" placeholder="Valor da claim..." required />
                </div>
                <div class="col-2">
                    <button type="button" class="btn btn-danger btn-sm" onclick="rolesManager.removeClaim(this)">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </div>`;
        
        const $newRow = $(claimHtml);
        $('#claimsContainer').append($newRow);
        
        // Atualizar feedback visual
        rolesManager.updateClaimsValidationFeedback();
        
        // Focar no select de tipo da nova claim
        $newRow.find('.claim-type').focus();
    },

    removeClaim: function(button) {
        $(button).closest('.claim-row').remove();
        
        // Reindexar os campos
        $('.claim-row').each(function(index) {
            $(this).find('select[name*=".Type"]').attr('name', `Claims[${index}].Type`);
            $(this).find('input[name*=".Value"]').attr('name', `Claims[${index}].Value`);
        });
        
        // Atualizar feedback visual
        rolesManager.updateClaimsValidationFeedback();
    },

    updateClaimsValidationFeedback: function() {
        const claimCount = $('.claim-row').length;
        const $emptyMessage = $('#emptyClaimsMessage');
        
        if (claimCount === 0) {
            $emptyMessage.show();
            $emptyMessage.removeClass('text-muted border-muted')
                        .addClass('text-warning border-warning');
        } else {
            $emptyMessage.hide();
        }
        
        console.log(`RolesManager: Claims count atualizada: ${claimCount}`);
    },

    addClaimFromSuggestion: function(claimType) {
        rolesManager.addClaim();
        
        // Definir o tipo da claim no último elemento adicionado
        const $lastRow = $('.claim-row').last();
        $lastRow.find('.claim-type').val(claimType);
        
        // Focar no campo de valor
        $lastRow.find('input[name*=".Value"]').focus();
    },

    generateClaimTypeOptions: function() {
        if (typeof window.availableClaimTypes !== 'undefined') {
            return window.availableClaimTypes.map(type => 
                `<option value="${type}">${type}</option>`
            ).join('');
        }
        
        // Fallback para tipos básicos
        const basicTypes = [
            'permission.users.read', 'permission.users.create', 'permission.users.update', 'permission.users.delete',
            'permission.roles.read', 'permission.roles.create', 'permission.roles.update', 'permission.roles.delete',
            'permission.claims.read', 'permission.claims.create', 'permission.claims.update', 'permission.claims.delete',
            'permission.admin.access'
        ];
        
        return basicTypes.map(type => 
            `<option value="${type}">${type}</option>`
        ).join('');
    },

    updateClaimValues: function(selectType) {
        // Para roles, mantemos o valor como texto livre
        // Esta função pode ser expandida no futuro se necessário
    },

    initializeForm: function() {
        if ($('#editRoleForm').length) {
            $.validator.unobtrusive.parse('#editRoleForm');
        }
        if ($('#createRoleForm').length) {
            $.validator.unobtrusive.parse('#createRoleForm');
        }
        
        // Aguardar um pouco para garantir que o DOM foi carregado
        setTimeout(function() {
            // Inicializar claims existentes após carregar o form
            $('.claim-type').each(function() {
                rolesManager.updateClaimValues(this);
            });
        }, 100);
    },

    removeEmptyClaims: function() {
        console.log('Removendo claims vazias...');
        $('.claim-row').each(function() {
            const $row = $(this);
            const type = $row.find('.claim-type').val();
            const value = $row.find('input[name*="Value"]').val();
            
            // Se ambos estão vazios, remover a linha
            if (!type || !value) {
                console.log('Removendo claim vazia:', {type, value});
                $row.remove();
            }
        });
        
        // Atualizar índices dos campos restantes
        rolesManager.updateClaimIndexes();
    },

    getClaimsData: function() {
        const claims = [];
        $('.claim-row').each(function() {
            const $row = $(this);
            const type = $row.find('.claim-type').val();
            const value = $row.find('input[name*="Value"]').val();
            
            if (type && value) {
                claims.push({ type: type, value: value });
            }
        });
        return claims;
    },

    updateClaimIndexes: function() {
        console.log('Atualizando índices das claims...');
        $('.claim-row').each(function(index) {
            const $row = $(this);
            
            // Atualizar name attributes
            $row.find('.claim-type').attr('name', `Claims[${index}].Type`);
            $row.find('input[name*="Value"]').attr('name', `Claims[${index}].Value`);
            
            // Atualizar IDs se necessário
            $row.find('.claim-type').attr('id', `Claims_${index}__Type`);
            $row.find('input[name*="Value"]').attr('id', `Claims_${index}__Value`);
        });
    }
};

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    rolesManager.init();
}); 