import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { combineLatest, debounceTime, map, Observable, startWith } from 'rxjs';
import { CatalogProduct } from '../../core/models/catalog-product.model';
import { ProductService } from '../../core/services/product.service';

@Component({
  selector: 'app-products-page',
  standalone: true,
  imports: [AsyncPipe, DatePipe, ReactiveFormsModule],
  templateUrl: './products.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductsPage implements OnInit {
  readonly search = new FormControl('', { nonNullable: true });
  readonly productForm = new FormGroup({
    code: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(40)] }),
    description: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(160)] })
  });

  filteredProducts$!: Observable<CatalogProduct[]>;
  createOpen = false;
  saving = false;
  formError = '';
  successMessage = '';

  constructor(private readonly products: ProductService) {}

  ngOnInit(): void {
    this.products.load();
    const search$ = this.search.valueChanges.pipe(startWith(''), debounceTime(120));

    this.filteredProducts$ = combineLatest([this.products.products$, search$]).pipe(
      map(([products, term]) => {
        const query = term.trim().toLowerCase();
        if (!query) return products;
        return products.filter(product =>
          product.code.toLowerCase().includes(query) || product.description.toLowerCase().includes(query)
        );
      })
    );
  }

  openCreate(): void {
    this.formError = '';
    this.productForm.reset({ code: '', description: '' });
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

    this.products.create(this.productForm.getRawValue()).subscribe({
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
