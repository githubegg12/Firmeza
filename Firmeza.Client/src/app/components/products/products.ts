import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService, ProductDto } from '../../services/product';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-products',
  imports: [CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);

  products = signal<ProductDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  addedToCart = signal<number | null>(null);

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.loading.set(true);
    this.error.set(null);

    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error fetching products:', err);
        this.error.set('Error al cargar productos. Verifica que la API esté corriendo en http://localhost:5277');
        this.loading.set(false);
      }
    });
  }

  addToCart(product: ProductDto) {
    if (product.stock > 0) {
      this.cartService.addToCart(product, 1);
      this.addedToCart.set(product.id);

      // Clear the notification after 2 seconds
      setTimeout(() => {
        this.addedToCart.set(null);
      }, 2000);
    }
  }

  isOutOfStock(product: ProductDto): boolean {
    return product.stock === 0;
  }
}
