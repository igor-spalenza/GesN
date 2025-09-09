"use strict";
// ===================================
// ORDER ITEM MANAGER - GesN (TypeScript)
// Gerenciamento completo de itens do pedido (OrderItem)
// ===================================
/// <reference path="../interfaces/common.ts" />
/// <reference path="../interfaces/product-catalog.ts" />
/// <reference path="../types/globals.d.ts" />
// ===================================
// ORDER ITEM MANAGER CLASS
// ===================================
class OrderItemManager {
    // ===================================
    // CONSTRUTOR E INICIALIZAÇÃO
    // ===================================
    constructor(events) {
        // ===================================
        // PROPRIEDADES E CONFIGURAÇÕES
        // ===================================
        this.orderId = '';
        this.items = [];
        this.isLoading = false;
        // Configuração de seletores e endpoints
        this.config = {
            containerSelector: '#orderItemsContainer',
            endpoints: {
                reloadItems: '/OrderItem/ReloadItems',
                addSimple: '/OrderItem/AddSimpleItem',
                addComposite: '/OrderItem/AddCompositeItem',
                addGroup: '/OrderItem/AddGroupItem',
                update: '/OrderItem/UpdateItem',
                remove: '/OrderItem/RemoveItem'
            }
        };
        // Sistema de eventos para comunicação com outros managers
        this.events = {};
        this.events = events || {};
        console.log('OrderItemManager inicializado');
    }
    /**
     * Inicializa o manager para um pedido específico
     */
    init(orderId) {
        try {
            this.orderId = orderId;
            this.isLoading = false;
            this.items = [];
            console.log(`OrderItemManager inicializado para pedido: ${orderId}`);
        }
        catch (error) {
            console.error('Erro ao inicializar OrderItemManager:', error);
        }
    }
    /**
     * Obtém o ID do pedido atual
     */
    getCurrentOrderId() {
        return this.orderId;
    }
    /**
     * Limpa o estado do manager
     */
    destroy() {
        this.orderId = '';
        this.items = [];
        this.isLoading = false;
    }
    // ===================================
    // MÉTODOS AJAX - EXTRAÍDOS DO PRODUCTCATALOGMANAGER
    // ===================================
    /**
     * Adiciona produto simples ao carrinho
     * EXTRAÍDO DE: ProductCatalogManager.makeAddSimpleItemRequest()
     */
    async addSimpleItem(productId, quantity, options) {
        try {
            const request = {
                orderId: this.orderId,
                productId: productId,
                quantity: quantity,
                discountAmount: options?.discountAmount || 0,
                taxAmount: options?.taxAmount || 0,
                notes: options?.notes || ''
            };
            const response = await this.makeAddSimpleItemRequest(request);
            if (response.success) {
                // Recarregar lista de itens automaticamente
                await this.reloadItems();
                // Disparar evento de sucesso
                this.events.onItemAdded?.(response.item);
            }
            return response;
        }
        catch (error) {
            console.error('Erro ao adicionar produto simples:', error);
            const errorResponse = {
                success: false,
                message: 'Erro ao adicionar produto ao carrinho'
            };
            this.events.onError?.(error);
            return errorResponse;
        }
    }
    /**
     * Adiciona produto composto ao carrinho
     * EXTRAÍDO DE: ProductCatalogManager.makeAddCompositeItemRequest()
     */
    async addCompositeItem(productId, quantity, componentConfigurations, options) {
        try {
            const request = {
                orderId: this.orderId,
                productId: productId,
                quantity: quantity,
                componentConfigurations: componentConfigurations,
                discountAmount: options?.discountAmount || 0,
                taxAmount: options?.taxAmount || 0,
                notes: options?.notes || ''
            };
            const response = await this.makeAddCompositeItemRequest(request);
            if (response.success) {
                // Recarregar lista de itens automaticamente
                await this.reloadItems();
                // Disparar evento de sucesso
                this.events.onItemAdded?.(response.item);
            }
            return response;
        }
        catch (error) {
            console.error('Erro ao adicionar produto composto:', error);
            const errorResponse = {
                success: false,
                message: 'Erro ao adicionar produto composto ao carrinho'
            };
            this.events.onError?.(error);
            return errorResponse;
        }
    }
    /**
     * Adiciona grupo de produtos ao carrinho
     * EXTRAÍDO DE: ProductCatalogManager.makeAddGroupItemRequest()
     */
    async addGroupItem(productId, quantity, groupConfigurations, options) {
        try {
            const request = {
                orderId: this.orderId,
                productId: productId,
                quantity: quantity,
                groupConfigurations: groupConfigurations,
                discountAmount: options?.discountAmount || 0,
                taxAmount: options?.taxAmount || 0,
                notes: options?.notes || `Grupo configurado com ${groupConfigurations.length} itens`
            };
            const response = await this.makeAddGroupItemRequest(request);
            if (response.success) {
                // Recarregar lista de itens automaticamente
                await this.reloadItems();
                // Disparar evento de sucesso
                this.events.onItemAdded?.(response.item);
            }
            return response;
        }
        catch (error) {
            console.error('Erro ao adicionar grupo de produtos:', error);
            const errorResponse = {
                success: false,
                message: 'Erro ao adicionar grupo de produtos ao carrinho'
            };
            this.events.onError?.(error);
            return errorResponse;
        }
    }
    // ===================================
    // MÉTODOS AJAX PRIVADOS - COMUNICAÇÃO COM BACKEND
    // ===================================
    /**
     * Faz requisição específica para adicionar produto simples ao carrinho
     * EXTRAÍDO DE: ProductCatalogManager.makeAddSimpleItemRequest()
     */
    async makeAddSimpleItemRequest(request) {
        return new Promise((resolve) => {
            $.ajax({
                url: this.config.endpoints.addSimple,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                success: (data) => {
                    resolve(data);
                },
                error: (xhr) => {
                    console.error('Erro na requisição de produto simples:', xhr);
                    let errorMessage = 'Erro na comunicação com o servidor';
                    try {
                        if (xhr.responseJSON && xhr.responseJSON.message) {
                            errorMessage = xhr.responseJSON.message;
                        }
                        else if (xhr.responseText) {
                            errorMessage = xhr.responseText;
                        }
                    }
                    catch (parseError) {
                        console.warn('Erro ao processar resposta de erro:', parseError);
                    }
                    resolve({
                        success: false,
                        message: errorMessage
                    });
                }
            });
        });
    }
    /**
     * Faz requisição para adicionar produto composto ao carrinho
     * EXTRAÍDO DE: ProductCatalogManager.makeAddCompositeItemRequest()
     */
    async makeAddCompositeItemRequest(request) {
        return new Promise((resolve) => {
            $.ajax({
                url: this.config.endpoints.addComposite,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                success: (data) => {
                    resolve(data);
                },
                error: (xhr) => {
                    console.error('Erro na requisição de produto composto:', xhr);
                    let errorMessage = 'Erro na comunicação com o servidor';
                    try {
                        if (xhr.responseJSON && xhr.responseJSON.message) {
                            errorMessage = xhr.responseJSON.message;
                        }
                        else if (xhr.responseText) {
                            errorMessage = xhr.responseText;
                        }
                    }
                    catch (parseError) {
                        console.warn('Erro ao processar resposta de erro:', parseError);
                    }
                    resolve({
                        success: false,
                        message: errorMessage
                    });
                }
            });
        });
    }
    /**
     * Faz requisição para adicionar grupo de produtos ao carrinho
     * EXTRAÍDO DE: ProductCatalogManager.makeAddGroupItemRequest()
     */
    async makeAddGroupItemRequest(request) {
        return new Promise((resolve) => {
            $.ajax({
                url: this.config.endpoints.addGroup,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),
                success: (data) => {
                    resolve(data);
                },
                error: (xhr) => {
                    console.error('Erro na requisição de grupo de produtos:', xhr);
                    let errorMessage = 'Erro na comunicação com o servidor';
                    try {
                        if (xhr.responseJSON && xhr.responseJSON.message) {
                            errorMessage = xhr.responseJSON.message;
                        }
                        else if (xhr.responseText) {
                            errorMessage = xhr.responseText;
                        }
                    }
                    catch (parseError) {
                        console.warn('Erro ao processar resposta de erro:', parseError);
                    }
                    resolve({
                        success: false,
                        message: errorMessage
                    });
                }
            });
        });
    }
    /**
     * Recarrega lista de OrderItems
     * EXTRAÍDO DE: ProductCatalogManager.reloadOrderItems()
     */
    async reloadItems() {
        return new Promise((resolve, reject) => {
            try {
                if (!this.orderId) {
                    console.warn('OrderItemManager: Sem orderId definido para reload');
                    resolve();
                    return;
                }
                this.isLoading = true;
                $.ajax({
                    url: this.config.endpoints.reloadItems,
                    type: 'GET',
                    data: { orderId: this.orderId },
                    dataType: 'html'
                })
                    .done((response) => {
                    // Atualizar container de OrderItems
                    $(this.config.containerSelector).html(response);
                    // Disparar evento de sucesso
                    this.events.onItemsReloaded?.();
                    console.log('OrderItems recarregados com sucesso');
                    this.isLoading = false;
                    resolve();
                })
                    .fail((xhr) => {
                    console.error('Erro ao recarregar OrderItems:', xhr);
                    const errorMessage = xhr.responseJSON?.message || 'Erro ao recarregar itens do pedido';
                    // Mostrar mensagem de erro no container
                    $(this.config.containerSelector).html(`
                        <div class="alert alert-danger" role="alert">
                            <i class="fas fa-exclamation-triangle"></i>
                            ${errorMessage}
                        </div>
                    `);
                    this.events.onError?.({
                        action: 'reloadItems',
                        message: errorMessage,
                        details: xhr,
                        timestamp: Date.now()
                    });
                    this.isLoading = false;
                    reject(new Error(errorMessage));
                });
            }
            catch (error) {
                console.error('Erro no reloadItems:', error);
                this.isLoading = false;
                reject(error);
            }
        });
    }
    // ===================================
    // MÉTODOS AUXILIARES
    // ===================================
    /**
     * Verifica se o manager está pronto para operações
     */
    isReady() {
        return !this.isLoading && !!this.orderId;
    }
    /**
     * Verifica se há itens no carrinho
     */
    hasItems() {
        return this.items.length > 0;
    }
    /**
     * Obtém lista atual de itens
     */
    getItems() {
        return [...this.items]; // Retorna cópia para evitar mutação
    }
}
// ===================================
// INSTÂNCIA GLOBAL
// ===================================
// Declaração global está em globals.d.ts
// Será inicializado pelo OrderManager
// window.orderItemManager será definido durante a inicialização das abas
//# sourceMappingURL=OrderItemManager.js.map