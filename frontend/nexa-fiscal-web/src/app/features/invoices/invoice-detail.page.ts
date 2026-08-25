import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BehaviorSubject, combineLatest, map, Observable, of } from 'rxjs';
import { Invoice } from '../../core/models/invoice.model';
import { BillingService } from '../../core/services/billing.service';
import { InventoryService } from '../../core/services/inventory.service';
import { InsufficientStockError } from '../../core/services/inventory.service';

@Component({
  selector: 'app-invoice-detail-page',
  standalone: true,
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './invoice-detail.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvoiceDetailPage implements OnInit {
  private readonly refreshSubject = new BehaviorSubject(0);
  invoice$!: Observable<Invoice | undefined>;
  insight = '';
  processing = false;
  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly billing: BillingService,
    private readonly inventory: InventoryService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.invoice$ = combineLatest([this.billing.invoices$, this.refreshSubject]).pipe(
      map(([invoices]) => invoices.find(invoice => invoice.id === id))
    );

    if (this.route.snapshot.queryParamMap.get('print') === '1') {
      setTimeout(() => window.print(), 650);
    }
  }

  close(invoice: Invoice): void {
    this.processing = true;
    this.errorMessage = '';
    this.billing.closeInvoice(invoice.id).subscribe({
      next: () => {
        this.processing = false;
        this.refreshSubject.next(this.refreshSubject.value + 1);
        this.cdr.markForCheck();
      },
      error: (error: unknown) => {
        this.processing = false;
        this.errorMessage = error instanceof InsufficientStockError
          ? 'Estoque insuficiente. Ajuste os itens antes de tentar novamente.'
          : 'Não foi possível validar o estoque. Sua nota permanece aberta.';
        this.cdr.markForCheck();
      }
    });
  }


  totalUnits(invoice: Invoice): number {
    return invoice.items.reduce((total, item) => total + item.quantity, 0);
  }

  generateInsight(invoice: Invoice): void {
    const products = this.inventory.snapshot();
    const critical = invoice.items
      .map(item => ({ item, product: products.find(product => product.id === item.productId) }))
      .filter(pair => pair.product && pair.product.balance <= pair.product.minBalance);

    if (critical.length === 0) {
      this.insight = 'O impacto desta nota está dentro de uma faixa confortável de estoque para os produtos analisados.';
      return;
    }

    const names = critical.slice(0, 2).map(pair => pair.item.productDescription).join(' e ');
    this.insight = `${critical.length} produto(s) merecem atenção após esta movimentação. Destaque para ${names}, que estão próximos ou abaixo do estoque mínimo definido.`;
  }

  print(): void {
    window.print();
  }
}
