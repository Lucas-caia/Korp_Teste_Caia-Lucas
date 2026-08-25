import { AsyncPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BehaviorSubject, Observable, combineLatest, map } from 'rxjs';
import { InsufficientStockItem, Invoice } from '../../core/models/invoice.model';
import {
  BillingService,
  InvoiceCloseError
} from '../../core/services/billing.service';
import { ProductService } from '../../core/services/product.service';

interface InvoiceDetailItemVm {
  productId: string;
  code: string;
  description: string;
  quantity: number;
}

interface InvoiceDetailVm {
  invoice: Invoice;
  items: InvoiceDetailItemVm[];
}

@Component({
  selector: 'app-invoice-detail-page',
  standalone: true,
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './invoice-detail.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvoiceDetailPage implements OnInit {
  private readonly invoiceSubject = new BehaviorSubject<Invoice | undefined>(undefined);

  vm$!: Observable<InvoiceDetailVm | undefined>;
  loading = true;
  processing = false;
  loadError = '';
  closeError = '';
  insufficientItems: InsufficientStockItem[] = [];

  constructor(
    private readonly route: ActivatedRoute,
    private readonly billing: BillingService,
    private readonly products: ProductService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.products.load();

    this.vm$ = combineLatest([
      this.invoiceSubject,
      this.products.products$
    ]).pipe(
      map(([invoice, products]) => {
        if (!invoice) return undefined;

        return {
          invoice,
          items: invoice.items.map(item => {
            const product = products.find(current => current.id === item.productId);

            return {
              productId: item.productId,
              code: product?.code ?? '—',
              description: product?.description ?? 'Produto não encontrado no catálogo',
              quantity: item.quantity
            };
          })
        };
      })
    );

    this.billing.getById(id).subscribe({
      next: invoice => {
        this.invoiceSubject.next(invoice);
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.loading = false;
        this.loadError = 'Nota fiscal não encontrada.';
        this.cdr.markForCheck();
      }
    });
  }

  closeAndPrint(invoice: Invoice): void {
    if (this.processing || invoice.status !== 'OPEN') return;

    this.processing = true;
    this.closeError = '';
    this.insufficientItems = [];
    this.cdr.markForCheck();

    this.billing.closeInvoice(invoice.id).subscribe({
      next: closedInvoice => {
        this.invoiceSubject.next(closedInvoice);
        this.products.load(true);
        this.processing = false;
        this.cdr.markForCheck();

        setTimeout(() => window.print(), 150);
      },
      error: (error: InvoiceCloseError) => {
        this.processing = false;

        if (error.code === 'INSUFFICIENT_STOCK') {
          this.insufficientItems = error.items;
        } else {
          this.closeError = error.message;
        }

        this.cdr.markForCheck();
      }
    });
  }
}
