import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductDto } from './product';
import { AuthService } from './auth';

export interface CartItem {
    product: ProductDto;
    quantity: number;
}

export interface CreateSaleRequest {
    userId: string;
    items: {
        productId: number;
        quantity: number;
    }[];
}

export interface SaleResponse {
    id: number;
    saleDate: string;
    userId: string;
    totalAmount: number;
    items: any[];
}

@Injectable({
    providedIn: 'root',
})
export class CartService {
    private http = inject(HttpClient);
    private authService = inject(AuthService);
    private apiUrl = 'http://localhost:5277/api/Sales';

    // Cart state
    private cartItems = signal<CartItem[]>([]);

    // Computed values
    items = computed(() => this.cartItems());
    itemCount = computed(() =>
        this.cartItems().reduce((sum, item) => sum + item.quantity, 0)
    );
    totalPrice = computed(() =>
        this.cartItems().reduce((sum, item) => sum + (item.product.price * item.quantity), 0)
    );

    constructor() {
        // Load cart from localStorage on initialization
        this.loadCartFromStorage();
    }

    addToCart(product: ProductDto, quantity: number = 1): void {
        const currentItems = this.cartItems();
        const existingItemIndex = currentItems.findIndex(
            item => item.product.id === product.id
        );

        let updatedItems: CartItem[];
        if (existingItemIndex > -1) {
            // Update quantity of existing item
            updatedItems = currentItems.map((item, index) =>
                index === existingItemIndex
                    ? { ...item, quantity: item.quantity + quantity }
                    : item
            );
        } else {
            // Add new item
            updatedItems = [...currentItems, { product, quantity }];
        }

        this.cartItems.set(updatedItems);
        this.saveCartToStorage();
    }

    removeFromCart(productId: number): void {
        const updatedItems = this.cartItems().filter(
            item => item.product.id !== productId
        );
        this.cartItems.set(updatedItems);
        this.saveCartToStorage();
    }

    updateQuantity(productId: number, quantity: number): void {
        if (quantity <= 0) {
            this.removeFromCart(productId);
            return;
        }

        const updatedItems = this.cartItems().map(item =>
            item.product.id === productId
                ? { ...item, quantity }
                : item
        );
        this.cartItems.set(updatedItems);
        this.saveCartToStorage();
    }

    clearCart(): void {
        this.cartItems.set([]);
        this.saveCartToStorage();
    }

    checkout(): Observable<SaleResponse> {
        const userId = this.authService.getUserId();
        if (!userId) {
            throw new Error('Usuario no autenticado');
        }

        const request: CreateSaleRequest = {
            userId: userId,
            items: this.cartItems().map(item => ({
                productId: item.product.id,
                quantity: item.quantity
            }))
        };

        return this.http.post<SaleResponse>(this.apiUrl, request);
    }

    private saveCartToStorage(): void {
        localStorage.setItem('cart', JSON.stringify(this.cartItems()));
    }

    private loadCartFromStorage(): void {
        const cartStr = localStorage.getItem('cart');
        if (cartStr) {
            try {
                const items = JSON.parse(cartStr);
                this.cartItems.set(items);
            } catch (error) {
                console.error('Error loading cart from storage:', error);
                this.cartItems.set([]);
            }
        }
    }
}
