var claimsManager = {
    currentView: 'table', // 'cards' ou 'table'
    
    init: function () {

        // Inicializar eventos dos modais
        $(document).on('shown.bs.modal', '#modalContainer', function () {
            console.log('ClaimsManager: Modal container mostrado');
            claimsManager.initializeForm();
        });

        $(document).on('hidden.bs.modal', '#modalContainer', function () {
            // Destruir instâncias do Select2 antes de limpar o modal
            $('.form-select[multiple]').each(function() {
                if ($(this).hasClass('select2-hidden-accessible')) {
                    $(this).select2('destroy');
                }
            });
            $(this).find('.modal-content').html('');
        });

        // Configurar toastr
        toastr.options = {
            "closeButton": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "timeOut": "3000"
        };

        // Inicializar funcionalidades da grid
        claimsManager.initializeGrid();
        
        // Inicializar observer para detectar novos selects múltiplos
        claimsManager.initializeDOMObserver();
        
        console.log('ClaimsManager: Inicialização completa');
    },

    initializeGrid: function() {
        console.log('ClaimsManager: Inicializando grid...');
        
        // Busca em tempo real
        $('#searchInput').on('input', function() {
            claimsManager.filterClaims();
        });

        // Filtros
        $('#userCountFilter, #roleCountFilter').on('change', function() {
            claimsManager.filterClaims();
        });

        // Toggle de visualização
        $('#btnCardView').on('click', function() {
            claimsManager.switchView('cards');
        });

        $('#btnTableView').on('click', function() {
            claimsManager.switchView('table');
        });

        // Inicializar view padrão
        claimsManager.switchView('cards');
    },

    switchView: function(viewType) {
        console.log('ClaimsManager: Mudando view para:', viewType);
        claimsManager.currentView = viewType;
        
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

    filterClaims: function() {
        const searchTerm = $('#searchInput').val().toLowerCase();
        const userCountFilter = $('#userCountFilter').val();
        const roleCountFilter = $('#roleCountFilter').val();

        let visibleCount = 0;

        $('.claim-item').each(function() {
            const $item = $(this);
            const searchData = $item.data('search') || '';
            const userCount = parseInt($item.data('user-count')) || 0;
            const roleCount = parseInt($item.data('role-count')) || 0;

            let showItem = true;

            // Filtro de busca
            if (searchTerm && !searchData.includes(searchTerm)) {
                showItem = false;
            }

            // Filtro de contagem de usuários
            if (userCountFilter && !claimsManager.matchCountFilter(userCount, userCountFilter)) {
                showItem = false;
            }

            // Filtro de contagem de roles
            if (roleCountFilter && !claimsManager.matchCountFilter(roleCount, roleCountFilter)) {
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
            if (claimsManager.currentView === 'cards') {
                $('#cardsView').show();
            } else {
                $('#tableView').show();
            }
        }
    },

    matchCountFilter: function(count, filter) {
        switch (filter) {
            case '0':
                return count === 0;
            case '1-5':
                return count >= 1 && count <= 5;
            case '6-20':
                return count >= 6 && count <= 20;
            case '20+':
                return count > 20;
            case '1-3':
                return count >= 1 && count <= 3;
            case '4-10':
                return count >= 4 && count <= 10;
            case '10+':
                return count > 10;
            default:
                return true;
        }
    },

    clearFilters: function() {
        $('#searchInput').val('');
        $('#userCountFilter').val('');
        $('#roleCountFilter').val('');
        claimsManager.filterClaims();
    },

    applyFilters: function() {
        // Fechar o collapse de filtros avançados
        $('#advancedFilters').collapse('hide');
        claimsManager.filterClaims();
    },

    atualizarGrid: function() {
        console.log('ClaimsManager: Atualizando grid...');
        $.ajax({
            url: '/Admin/Claims/GridPartial',
            type: 'GET',
            success: function(data) {
                console.log('ClaimsManager: Grid atualizada com sucesso');
                $('#gridContainer').html(data);
                // Reinicializar a view atual após recarregar a grid
                claimsManager.switchView(claimsManager.currentView);
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro ao carregar grid:', {xhr, status, error});
                console.error('Response text:', xhr.responseText);
                toastr.error('Erro ao atualizar a lista de claims: ' + (xhr.responseText || error));
            }
        });
    },

    visualizarClaimModal: function(claimType, claimValue) {
        console.log('ClaimsManager: Abrindo modal de detalhes da claim:', {claimType, claimValue});
        
        if (!claimType || !claimValue) {
            console.error('ClaimsManager: Parâmetros de claim inválidos:', {claimType, claimValue});
            toastr.error('Parâmetros da claim são inválidos.');
            return;
        }
        
        const $modal = $('#modalContainer');
        
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        const url = `/Admin/Claims/DetailsPartial?type=${encodeURIComponent(claimType)}&value=${encodeURIComponent(claimValue)}`;
        console.log('ClaimsManager: URL da requisição de detalhes:', url);
        
        $.ajax({
            url: url,
            type: 'GET',
            success: function(data) {
                console.log('ClaimsManager: Detalhes carregados com sucesso');
                $modal.find('.modal-content').html(data);
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro ao carregar detalhes da claim:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                toastr.error('Erro ao carregar os detalhes da claim.');
                $modal.modal('hide');
            }
        });
    },

    novoClaimModal: function() {
        console.log('ClaimsManager: Abrindo modal de nova claim');
        
        const $modal = $('#modalContainer');
        
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        $.ajax({
            url: '/Admin/Claims/CreatePartial',
            type: 'GET',
            success: function(data) {
                console.log('ClaimsManager: Formulário de criação carregado com sucesso');
                $modal.find('.modal-content').html(data);
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro ao carregar formulário de criação:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                toastr.error('Erro ao carregar o formulário de criação.');
                $modal.modal('hide');
            }
        });
    },

    editarClaimModal: function(claimType, claimValue) {
        console.log('ClaimsManager: Abrindo modal de edição da claim:', {claimType, claimValue});
        
        if (!claimType || !claimValue) {
            console.error('ClaimsManager: Parâmetros de claim inválidos para edição:', {claimType, claimValue});
            toastr.error('Parâmetros da claim são inválidos.');
            return;
        }
        
        const $modal = $('#modalContainer');
        
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        const url = `/Admin/Claims/Edit?type=${encodeURIComponent(claimType)}&value=${encodeURIComponent(claimValue)}`;
        console.log('ClaimsManager: URL da requisição de edição:', url);
        
        $.ajax({
            url: url,
            type: 'GET',
            success: function(data) {
                console.log('ClaimsManager: Formulário de edição carregado com sucesso');
                $modal.find('.modal-content').html(data);
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro ao carregar formulário de edição:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                toastr.error('Erro ao carregar o formulário de edição.');
                $modal.modal('hide');
            }
        });
    },

    excluirClaimModal: function(claimType, claimValue) {
        console.log('ClaimsManager: Abrindo modal de exclusão da claim:', {claimType, claimValue});
        
        if (!claimType || !claimValue) {
            console.error('ClaimsManager: Parâmetros de claim inválidos para exclusão:', {claimType, claimValue});
            toastr.error('Parâmetros da claim são inválidos.');
            return;
        }
        
        const $modal = $('#modalContainer');
        
        $modal.find('.modal-content').html('<div class="d-flex justify-content-center my-5"><div class="spinner-border" role="status"><span class="visually-hidden">Carregando...</span></div></div>');
        $modal.modal('show');
        
        const url = `/Admin/Claims/Delete?type=${encodeURIComponent(claimType)}&value=${encodeURIComponent(claimValue)}`;
        console.log('ClaimsManager: URL da requisição de exclusão:', url);
        
        $.ajax({
            url: url,
            type: 'GET',
            success: function(data) {
                console.log('ClaimsManager: Formulário de exclusão carregado com sucesso');
                $modal.find('.modal-content').html(data);
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro ao carregar formulário de exclusão:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                toastr.error('Erro ao carregar o formulário de exclusão.');
                $modal.modal('hide');
            }
        });
    },

    salvarNovo: function(form) {
        console.log('ClaimsManager: salvarNovo chamado');
        console.log('Form element:', form);
        console.log('Form jQuery object:', $(form));
        
        // Prevenir múltiplos submits
        if ($(form).data('submitting')) {
            console.log('ClaimsManager: Já está enviando claim, cancelando');
            return false;
        }

        // Verificar validação
        if (!$(form).valid()) {
            console.log('ClaimsManager: Formulário inválido na criação de claim');
            console.log('Erros de validação:', $(form).find('.field-validation-error').map(function() {
                return $(this).text();
            }).get());
            return false;
        }

        // Regra: precisa ter pelo menos um usuário OU uma role selecionada
        const selectedUsers = $('select[name="SelectedUsers"]').val() || [];
        const selectedRoles = $('select[name="SelectedRoles"]').val() || [];
        
        // Garantir que são arrays
        const usersArray = Array.isArray(selectedUsers) ? selectedUsers : [];
        const rolesArray = Array.isArray(selectedRoles) ? selectedRoles : [];
        
        if (usersArray.length === 0 && rolesArray.length === 0) {
            toastr.warning('Selecione pelo menos um usuário ou uma role para atribuir a claim.');
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        console.log('ClaimsManager: Iniciando envio da nova claim...');
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
                console.log('ClaimsManager: Resposta da criação de claim recebida:', response);
                console.log('Tipo da resposta:', typeof response);
                
                if (response && response.success) {
                    console.log('ClaimsManager: Claim criada com sucesso, fechando modal e atualizando grid');
                    $('#modalContainer').modal('hide');
                    claimsManager.atualizarGrid();
                    toastr.success('Claim criada com sucesso!');
                } else {
                    console.log('ClaimsManager: Resposta indica erro ou formulário com validação');
                    if (typeof response === 'string') {
                        console.log('ClaimsManager: Atualizando modal com novo HTML');
                        $('#modalContainer .modal-content').html(response);
                        claimsManager.initializeForm();
                    } else {
                        console.log('ClaimsManager: Mostrando mensagem de erro JSON');
                        toastr.error(response.message || 'Erro ao criar claim. Verifique os dados informados.');
                    }
                }
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro na requisição de criação de claim:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                console.error('Response headers:', xhr.getAllResponseHeaders());
                toastr.error(xhr.responseText || 'Erro ao salvar a claim.');
            },
            complete: function() {
                console.log('ClaimsManager: Requisição de criação de claim completa');
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
            console.log('ClaimsManager: Já está enviando edição de claim, cancelando');
            return false;
        }

        // Verificar validação
        if (!$(form).valid()) {
            console.log('ClaimsManager: Formulário inválido na edição de claim');
            console.log('Erros de validação:', $(form).find('.field-validation-error').map(function() {
                return $(this).text();
            }).get());
            return false;
        }

        // Regra: precisa ter pelo menos um usuário OU uma role selecionada
        const selectedUsers = $('select[name="SelectedUserIds"]').val() || [];
        const selectedRoles = $('select[name="SelectedRoleIds"]').val() || [];
        
        // Garantir que são arrays
        const usersArray = Array.isArray(selectedUsers) ? selectedUsers : [];
        const rolesArray = Array.isArray(selectedRoles) ? selectedRoles : [];
        
        if (usersArray.length === 0 && rolesArray.length === 0) {
            toastr.warning('Selecione pelo menos um usuário ou uma role para atribuir a claim.');
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        console.log('ClaimsManager: Iniciando envio da edição de claim...');
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
                console.log('ClaimsManager: Resposta da edição de claim recebida:', response);
                console.log('Tipo da resposta:', typeof response);
                
                if (response && response.success) {
                    console.log('ClaimsManager: Claim atualizada com sucesso, fechando modal e atualizando grid');
                    $('#modalContainer').modal('hide');
                    claimsManager.atualizarGrid();
                    toastr.success('Claim atualizada com sucesso!');
                } else {
                    console.log('ClaimsManager: Resposta indica erro ou formulário com validação');
                    if (typeof response === 'string') {
                        console.log('ClaimsManager: Atualizando modal com novo HTML');
                        $('#modalContainer .modal-content').html(response);
                        claimsManager.initializeForm();
                    } else {
                        console.log('ClaimsManager: Mostrando mensagem de erro JSON');
                        toastr.error(response.message || 'Erro ao atualizar claim. Verifique os dados informados.');
                    }
                }
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro na requisição de edição de claim:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                console.error('Response headers:', xhr.getAllResponseHeaders());
                toastr.error(xhr.responseText || 'Erro ao atualizar a claim.');
            },
            complete: function() {
                console.log('ClaimsManager: Requisição de edição de claim completa');
                $form.data('submitting', false);
                submitButton.prop('disabled', false);
                loadingSpinner.addClass('d-none');
            }
        });

        return false;
    },

    confirmarExclusao: function(form) {
        console.log('ClaimsManager: confirmarExclusao chamado');
        console.log('Form element:', form);
        console.log('Form jQuery object:', $(form));
        
        // Prevenir múltiplos submits
        if ($(form).data('submitting')) {
            console.log('ClaimsManager: Já está enviando exclusão de claim, cancelando');
            return false;
        }

        const $form = $(form);
        const submitButton = $form.find('button[type="submit"]');
        const loadingSpinner = $form.find('.spinner-border');
        
        console.log('ClaimsManager: Iniciando exclusão de claim...');
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
                console.log('ClaimsManager: Resposta da exclusão de claim recebida:', response);
                console.log('Tipo da resposta:', typeof response);
                
                if (response && response.success) {
                    console.log('ClaimsManager: Claim excluída com sucesso, fechando modal e atualizando grid');
                    $('#modalContainer').modal('hide');
                    claimsManager.atualizarGrid();
                    toastr.success('Claim excluída com sucesso!');
                } else {
                    console.log('ClaimsManager: Resposta indica erro na exclusão');
                    toastr.error(response.message || 'Erro ao excluir claim.');
                }
            },
            error: function(xhr, status, error) {
                console.error('ClaimsManager: Erro na requisição de exclusão de claim:', {xhr, status, error});
                console.error('Status code:', xhr.status);
                console.error('Response text:', xhr.responseText);
                console.error('Response headers:', xhr.getAllResponseHeaders());
                toastr.error(xhr.responseText || 'Erro ao excluir a claim.');
            },
            complete: function() {
                console.log('ClaimsManager: Requisição de exclusão de claim completa');
                $form.data('submitting', false);
                submitButton.prop('disabled', false);
                loadingSpinner.addClass('d-none');
            }
        });

        return false;
    },

    setClaimType: function(claimType) {
        const $claimTypeSelect = $('#claimTypeSelect');
        if ($claimTypeSelect.length > 0) {
            $claimTypeSelect.val(claimType).trigger('change');
            console.log('ClaimsManager: Tipo de claim definido:', claimType);
        }
    },

    initializeForm: function() {
        console.log('ClaimsManager: Inicializando formulário do modal...');
        
        // Aguardar um pouco para garantir que o DOM foi carregado
        setTimeout(function() {
            // Validação unobtrusive
            if ($('#editClaimForm').length) {
                $.validator.unobtrusive.parse('#editClaimForm');
            }
            if ($('#createClaimForm').length) {
                $.validator.unobtrusive.parse('#createClaimForm');
            }
            
            // Inicializar Select2
            claimsManager.initializeSelect2();
            
            console.log('ClaimsManager: Formulário inicializado');
        }, 150);
    },

    // Método para reinicializar Select2 quando conteúdo é carregado dinamicamente
    reinitializeSelect2: function() {
        claimsManager.initializeSelect2();
    },

    // Observer para detectar automaticamente novos selects múltiplos
    initializeDOMObserver: function() {
        // Usar MutationObserver para detectar mudanças no DOM
        if (typeof MutationObserver !== 'undefined') {
            const observer = new MutationObserver(function(mutations) {
                mutations.forEach(function(mutation) {
                    if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                        // Verificar se foram adicionados novos selects múltiplos
                        mutation.addedNodes.forEach(function(node) {
                            if (node.nodeType === 1) { // Element node
                                const $node = $(node);
                                // Verificar se o próprio node é um select múltiplo ou contém um
                                const $newSelects = $node.is('select[multiple]') ? 
                                    $node : $node.find('select[multiple]');
                                
                                if ($newSelects.length > 0) {
                                    setTimeout(function() {
                                        claimsManager.initializeSelect2();
                                    }, 100);
                                }
                            }
                        });
                    }
                });
            });

            // Observar mudanças no container do modal
            const modalContainer = document.getElementById('modalContainer');
            if (modalContainer) {
                observer.observe(modalContainer, {
                    childList: true,
                    subtree: true
                });
            }
        }
    },

    initializeSelect2: function() {
        try {
            // Verificar se existe algum select múltiplo que ainda não foi inicializado
            const $selects = $('.form-select[multiple]').not('.select2-hidden-accessible');
            
            if ($selects.length > 0) {
                $selects.select2({
                    theme: 'bootstrap-5',
                    language: 'pt-BR',
                    width: '100%',
                    placeholder: function() {
                        const id = $(this).attr('id');
                        if (id === 'SelectedUsers') {
                            return 'Selecione os usuários';
                        } else if (id === 'SelectedRoles') {
                            return 'Selecione as roles';
                        }
                        return 'Selecione as opções';
                    },
                    dropdownParent: $('#modalContainer'),
                    allowClear: true,
                    closeOnSelect: false
                });
                
                console.log('ClaimsManager: Select2 inicializado para', $selects.length, 'elementos');
            }
            
            // Inicializar Select2 para selects simples (tipo de claim)
            const $singleSelects = $('.form-select').not('[multiple]').not('.select2-hidden-accessible');
            if ($singleSelects.length > 0) {
                $singleSelects.select2({
                    theme: 'bootstrap-5',
                    language: 'pt-BR',
                    width: '100%',
                    placeholder: 'Selecione uma opção',
                    dropdownParent: $('#modalContainer'),
                    allowClear: true
                });
                
                console.log('ClaimsManager: Select2 inicializado para', $singleSelects.length, 'selects simples');
            }
        } catch (error) {
            console.log('ClaimsManager: Erro ao inicializar Select2:', error);
        }
    }
};

// Inicializar quando o documento estiver pronto
$(document).ready(function() {
    console.log('Document ready: Inicializando ClaimsManager');
    claimsManager.init();
    console.log('Document ready: ClaimsManager inicializado');
}); 