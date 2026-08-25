import { ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { InvoiceItem, InsufficientStockItem } from '../../core/models/invoice.model';
import { Product } from '../../core/models/product.model';
import { InventoryService, InsufficientStockError } from '../../core/services/inventory.service';
import { BillingService } from '../../core/services/billing.service';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-invoice-form-page',
  standalone: true,
  imports: [AsyncPipe, FormsModule, RouterLink],
  templateUrl: './invoice-form.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvoiceFormPage {
  readonly products$ = this.inventory.products$;
  items: InvoiceItem[] = [];
  selectedProductId = '';
  quantity = 1;
  processing = false;
  message = '';
  insufficientItems: InsufficientStockItem[] = [];

  constructor(
    private readonly inventory: InventoryService,
    private readonly billing: BillingService,
    private readonly router: Router,
    private readonly cdr: ChangeDetectorRef
  ) {}

  selectedProduct(): Product | undefined {
    return this.inventory.snapshot().find(product => product.id === this.selectedProductId);
  }

  addItem(): void {
    this.message = '';
    const product = this.selectedProduct();
    if (!product) {
      this.message = 'Selecione um produto.';
      return;
    }
    if (!Number.isInteger(this.quantity) || this.quantity <= 0) {
      this.message = 'Informe uma quantidade inteira maior que zero.';
      return;
    }

    const existing = this.items.find(item => item.productId === product.id);
    if (existing) {
      existing.quantity += this.quantity;
      this.items = [...this.items];
    } else {
      this.items = [...this.items, {
        productId: product.id,
        productCode: product.code,
        productDescription: product.description,
        quantity: this.quantity,
        balanceAtAddition: product.balance
      }];
    }

    this.selectedProductId = '';
    this.quantity = 1;
  }

  removeItem(productId: string): void {
    this.items = this.items.filter(item => item.productId !== productId);
  }

  totalUnits(): number {
    return this.items.reduce((total, item) => total + item.quantity, 0);
  }

  save(): void {
    if (!this.validateItems()) return;
    this.processing = true;
    this.billing.createInvoice(this.items).subscribe({
      next: invoice => this.router.navigate(['/invoices', invoice.id]),
      error: () => this.stopWithMessage('Não foi possível salvar a nota.')
    });
  }

  closeAndPrint(): void {
    if (!this.validateItems()) return;
    this.processing = true;
    this.insufficientItems = [];
    this.message = '';

    this.billing.createInvoice(this.items).pipe(
      switchMap(invoice => this.billing.closeInvoice(invoice.id))
    ).subscribe({
      next: invoice => this.router.navigate(['/invoices', invoice.id], { queryParams: { print: '1' } }),
      error: (error: unknown) => {
        this.processing = false;
        if (error instanceof InsufficientStockError) {
          this.insufficientItems = error.items;
          this.message = 'Estoque insuficiente para concluir a nota.';
        } else {
          this.message = 'Não foi possível concluir a operação. Tente novamente.';
        }
        this.cdr.markForCheck();
      }
    });
  }

  private validateItems(): boolean {
    if (this.items.length === 0) {
      this.message = 'Adicione pelo menos um produto à nota.';
      return false;
    }
    this.message = '';
    return true;
  }

  private stopWithMessage(message: string): void {
    this.processing = false;
    this.message = message;
    this.cdr.markForCheck();
  }
}
