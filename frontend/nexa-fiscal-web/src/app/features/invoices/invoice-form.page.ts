import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { CatalogProduct } from '../../core/models/catalog-product.model';
import { ProductService } from '../../core/services/product.service';
import { BillingService } from '../../core/services/billing.service';

interface DraftInvoiceItem {
  productId: string;
  code: string;
  description: string;
  quantity: number;
  balance: number;
}

@Component({
  selector: 'app-invoice-form-page',
  standalone: true,
  imports: [AsyncPipe, ReactiveFormsModule, RouterLink],
  templateUrl: './invoice-form.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvoiceFormPage implements OnInit {
  readonly products$: Observable<CatalogProduct[]> = this.products.products$;
  readonly productControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required]
  });
  readonly quantityControl = new FormControl(1, {
    nonNullable: true,
    validators: [Validators.required, Validators.min(1)]
  });

  items: DraftInvoiceItem[] = [];
  processing = false;
  message = '';
  itemMessage = '';

  constructor(
    private readonly products: ProductService,
    private readonly billing: BillingService,
    private readonly router: Router,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.products.load();
  }

  selectedProduct(): CatalogProduct | undefined {
    return this.products.snapshot().find(product => product.id === this.productControl.value);
  }

  addItem(): void {
    this.itemMessage = '';

    if (this.productControl.invalid || this.quantityControl.invalid) {
      this.productControl.markAsTouched();
      this.quantityControl.markAsTouched();
      return;
    }

    const product = this.selectedProduct();
    if (!product) {
      this.itemMessage = 'Selecione um produto válido.';
      return;
    }

    if (this.items.some(item => item.productId === product.id)) {
      this.itemMessage = 'Este produto já foi adicionado à nota.';
      return;
    }

    this.items = [
      ...this.items,
      {
        productId: product.id,
        code: product.code,
        description: product.description,
        quantity: this.quantityControl.value,
        balance: product.balance
      }
    ];

    this.productControl.setValue('');
    this.quantityControl.setValue(1);
  }

  removeItem(productId: string): void {
    if (this.processing) return;
    this.items = this.items.filter(item => item.productId !== productId);
  }

  create(): void {
    if (this.processing) return;

    if (this.items.length === 0) {
      this.message = 'Adicione ao menos um produto à nota fiscal.';
      return;
    }

    this.processing = true;
    this.message = '';

    this.billing.createInvoice(
      this.items.map(item => ({
        productId: item.productId,
        quantity: item.quantity
      }))
    ).subscribe({
      next: invoice => this.router.navigate(['/invoices', invoice.id]),
      error: (error: Error) => {
        this.processing = false;
        this.message = error.message;
        this.cdr.markForCheck();
      }
    });
  }
}
