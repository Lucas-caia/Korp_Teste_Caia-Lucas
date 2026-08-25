import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { combineLatest, map, Observable } from 'rxjs';
import { BillingService } from '../../core/services/billing.service';
import { ProductService } from '../../core/services/product.service';
import { CatalogProduct } from '../../core/models/catalog-product.model';
import { Invoice } from '../../core/models/invoice.model';

interface DashboardVm {
  productCount: number;
  lowStockCount: number;
  openInvoices: number;
  closedInvoices: number;
  recentInvoices: Invoice[];
  attentionProducts: CatalogProduct[];
}

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './dashboard.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardPage implements OnInit {
  vm$!: Observable<DashboardVm>;

  constructor(
    private readonly products: ProductService,
    private readonly billing: BillingService
  ) {}

  ngOnInit(): void {
    this.products.load();
    this.billing.load();

    this.vm$ = combineLatest([this.products.products$, this.billing.invoices$]).pipe(
      map(([products, invoices]) => ({
        productCount: products.length,
        lowStockCount: products.filter(product => product.balance <= 5).length,
        openInvoices: invoices.filter(invoice => invoice.status === 'OPEN').length,
        closedInvoices: invoices.filter(invoice => invoice.status === 'CLOSED').length,
        recentInvoices: invoices.slice(0, 5),
        attentionProducts: products.filter(product => product.balance <= 5).slice(0, 4)
      }))
    );
  }

  stockLabel(product: CatalogProduct): string {
    return product.balance === 0 ? 'Sem estoque' : 'Estoque baixo';
  }
}
