import { AsyncPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Observable, catchError, combineLatest, map, of } from 'rxjs';
import { Invoice } from '../../core/models/invoice.model';
import { BillingService } from '../../core/services/billing.service';
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
  vm$!: Observable<InvoiceDetailVm | undefined>;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly billing: BillingService,
    private readonly products: ProductService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.products.load();

    const invoice$ = this.billing.getById(id).pipe(
      catchError(() => of(undefined))
    );

    this.vm$ = combineLatest([invoice$, this.products.products$]).pipe(
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
  }
}
