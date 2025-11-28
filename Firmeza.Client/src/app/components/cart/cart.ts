import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { CartService, CartItem } from '../../services/cart.service';
import { AuthService } from '../../services/auth';

@Component({
    selector: 'app-cart',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './cart.html',
    styleUrl: './cart.css'
})
export class Cart implements OnInit {
    private cartService = inject(CartService);
    private authService = inject(AuthService);
    private router = inject(Router);

    items = this.cartService.items;
    itemCount = this.cartService.itemCount;
    totalPrice = this.cartService.totalPrice;

    checkingOut = signal(false);
    checkoutError = signal<string | null>(null);
    checkoutSuccess = signal(false);

    ngOnInit() {
        // Check if user is authenticated
        if (!this.authService.isAuthenticated()) {
            this.router.navigate(['/login'], {
                queryParams: { returnUrl: '/cart' }
            });
        }
    }

    updateQuantity(productId: number, newQuantity: number) {
        this.cartService.updateQuantity(productId, newQuantity);
    }

    removeItem(productId: number) {
        this.cartService.removeFromCart(productId);
    }

    checkout() {
        if (!this.authService.isAuthenticated()) {
            this.router.navigate(['/login'], {
                queryParams: { returnUrl: '/cart' }
            });
            return;
        }

        this.checkingOut.set(true);
        this.checkoutError.set(null);

        this.cartService.checkout().subscribe({
            next: (response) => {
                console.log('Compra exitosa:', response);
                this.checkoutSuccess.set(true);
                this.cartService.clearCart();

                // Redirect to products after 2 seconds
                setTimeout(() => {
                    this.router.navigate(['/products']);
                }, 2000);
            },
            error: (err) => {
                console.error('Error en checkout:', err);
                let errorMessage = 'Error al procesar la compra. Por favor intenta de nuevo.';

                if (err.error?.message) {
                    errorMessage = err.error.message;
                } else if (err.error) {
                    errorMessage = typeof err.error === 'string' ? err.error : errorMessage;
                }

                this.checkoutError.set(errorMessage);
                this.checkingOut.set(false);
            },
            complete: () => {
                this.checkingOut.set(false);
            }
        });
    }

    continueShopping() {
        this.router.navigate(['/products']);
    }
}
