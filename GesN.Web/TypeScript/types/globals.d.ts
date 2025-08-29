// ===================================
// DECLARAÇÕES GLOBAIS - GesN
// ===================================

// Declarações para bibliotecas JavaScript existentes
declare var $: JQueryStatic;
declare var jQuery: JQueryStatic;

// Bootstrap
declare var bootstrap: {
    Tab: new (element: string | Element) => {
        show(): void;
    };
    Modal: new (element: string | Element) => {
        show(): void;
        hide(): void;
    };
};

// Toastr
declare var toastr: {
    success(message: string, title?: string): void;
    error(message: string, title?: string): void;
    info(message: string, title?: string): void;
    warning(message: string, title?: string): void;
};

// Autocomplete (Algolia)
declare function autocomplete(input: Element, options: any, datasets: any[]): {
    on(event: string, handler: Function): void;
    destroy(): void;
};

// Extensões completas do jQuery
interface JQuery {
    // Core jQuery methods
    find(selector: string): JQuery;
    closest(selector: string): JQuery;
    val(): string | number | string[];
    val(value: string | number): JQuery;
    text(): string;
    text(value: string): JQuery;
    html(): string;
    html(value: string): JQuery;
    prop(propertyName: string): any;
    prop(propertyName: string, value: any): JQuery;
    data(key: string): any;
    data(key: string, value: any): JQuery;
    addClass(className: string): JQuery;
    removeClass(className?: string): JQuery;
    hasClass(className: string): boolean;
    css(propertyName: string): string;
    css(propertyName: string, value: string | number): JQuery;
    attr(attributeName: string): string;
    attr(attributeName: string, value: string): JQuery;
    append(content: string | JQuery): JQuery;
    after(content: string | JQuery): JQuery;
    remove(): JQuery;
    length: number;
    outerWidth(): number | undefined;
    outerHeight(): number | undefined;
    get(index: number): Element;
    
    // Event methods
    on(events: string, handler: (event?: any) => void): JQuery;
    on(events: string, selector: string, handler: (event?: any) => void): JQuery;
    off(events?: string): JQuery;
    trigger(eventType: string, extraParameters?: any): JQuery;
    
    // Animation and effects
    show(): JQuery;
    hide(): JQuery;
    fadeIn(): JQuery;
    fadeOut(): JQuery;
    
    // Traversing
    each(func: (index: number, element: Element) => void): JQuery;
    parent(): JQuery;
    children(selector?: string): JQuery;
    siblings(selector?: string): JQuery;
    first(): JQuery;
    last(): JQuery;
    eq(index: number): JQuery;
    
    // jQuery Mask Plugin
    mask(pattern: string, options?: any): JQuery;
    unmask(): JQuery;
    
    // DataTables
    DataTable(options?: any): any;
    
    // Bootstrap plugins
    modal(action: 'show' | 'hide' | 'toggle'): JQuery;
    modal(options?: any): JQuery;
    tooltip(options?: any): JQuery;
    
    // Form methods
    serialize(): string;
    serializeArray(): Array<{name: string, value: string}>;
    
    // Custom autocomplete
    autocomplete: any;
}

// DataTables
declare namespace DataTables {
    interface Settings {}
    interface Api {
        destroy(): void;
    }
}

declare var DataTable: {
    isDataTable(selector: string | Element): boolean;
};

// jQuery static methods
interface JQueryStatic {
    (selector: string | Element | Document | (() => void)): JQuery;
    ajax(options: any): any;
    get(url: string): any;
    post(url: string, data?: any): any;
    map<T, U>(array: T[], callback: (item: T, index: number) => U): U[];
    each<T>(collection: T[], callback: (index: number, item: T) => void): void;
    fn: {
        DataTable: {
            isDataTable(selector: string): boolean;
        };
        tooltip: any;
    };
}

// jQuery event types
declare namespace JQuery {
    interface Event {
        type: string;
        target: Element;
        currentTarget: Element;
        preventDefault(): void;
        stopPropagation(): void;
        pageX: number;
        pageY: number;
    }
    
    interface MouseDownEvent extends Event {
        pageX: number;
        pageY: number;
    }
    
    interface MouseMoveEvent extends Event {
        pageX: number;
        pageY: number;
    }
    
    interface DoubleClickEvent extends Event {}
}

// Window global additions
interface Window {
    customerManager: any;
    clientesManager: any;
    productManager: any;
    ordersManager: any;
    toastr: typeof toastr;
    bootstrap: typeof bootstrap;
}

// Global functions que podem existir
declare function confirm(message?: string): boolean;
declare function alert(message?: string): void;
declare function debounce(func: Function, wait: number): Function;

// Se houver outras bibliotecas específicas do projeto
declare var MutationObserver: {
    new (callback: MutationCallback): MutationObserver;
};

interface MutationObserver {
    observe(target: Node, options: MutationObserverInit): void;
    disconnect(): void;
}

// Para controle de tipos mais específicos do projeto
type RequestVerificationToken = string;

// Helpers para ASP.NET Core
declare var __RequestVerificationToken: RequestVerificationToken;

// AJAX Error Response
interface JQueryXHR {
    responseJSON?: any;
    status: number;
    statusText: string;
    getAllResponseHeaders(): string;
    getResponseHeader(name: string): string | null;
}
