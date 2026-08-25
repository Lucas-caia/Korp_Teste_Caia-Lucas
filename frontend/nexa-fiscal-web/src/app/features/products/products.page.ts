import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { combineLatest, debounceTime, map, Observable, startWith } from 'rxjs';
import { Product, getStockStatus } from '../../core/models/product.model';
import { InventoryService } from '../../core/services/inventory.service';

@Component({
  selector: 'app-products-page',
  standalone: true,
  imports: [AsyncPipe, ReactiveFormsModule],
  templateUrl: './products.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductsPage implements OnInit {
  readonly search = new FormControl('', { nonNullable: true });
  readonly productForm = new FormGroup({
    code: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    description: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    balance: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.min(0)] })
  });

  filteredProducts$!: Observable<Product[]>;
  createOpen = false;
  saving = false;
  formError = '';
  successMessage = '';

  constructor(private readonly inventory: InventoryService) {}

  ngOnInit(): void {
    const search$ = this.search.valueChanges.pipe(startWith(''), debounceTime(120));
    this.filteredProducts$ = combineLatest([this.inventory.products$, search$]).pipe(
      map(([products, term]) => {
        const query = term.trim().toLowerCase();
        if (!query) return products;
        return products.filter(product =>
          product.code.toLowerCase().includes(query) || product.description.toLowerCase().includes(query)
        );
      })
    );
  }

  statusClass(product: Product): string {
    return `stock-${getStockStatus(product)}`;
  }

  statusLabel(product: Product): string {
    const status = getStockStatus(product);
    if (status === 'empty') return 'Sem estoque';
    if (status === 'low') return 'Estoque baixo';
    return 'Normal';
  }

  openCreate(): void {
    this.formError = '';
    this.productForm.reset({ code: '', description: '', balance: 0 });
    this.createOpen = true;
  }

  closeCreate(): void {
    if (!this.saving) this.createOpen = false;
  }

  saveProduct(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.formError = '';
    this.inventory.addProduct(this.productForm.getRawValue()).subscribe({
      next: product => {
        this.saving = false;
        this.createOpen = false;
        this.successMessage = `${product.description} cadastrado com sucesso.`;
        setTimeout(() => this.successMessage = '', 3500);
      },
      error: (error: Error) => {
        this.saving = false;
        this.formError = error.message;
      }
    });
  }
}
