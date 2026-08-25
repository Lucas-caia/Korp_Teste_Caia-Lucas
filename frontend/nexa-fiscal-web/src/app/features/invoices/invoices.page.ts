import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
import { BillingService } from '../../core/services/billing.service';
import { InvoiceStatus } from '../../core/models/invoice.model';

type InvoiceFilter = 'ALL' | InvoiceStatus;

@Component({
  selector: 'app-invoices-page',
  standalone: true,
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './invoices.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvoicesPage implements OnInit {
  private readonly filterSubject = new BehaviorSubject<InvoiceFilter>('ALL');
  readonly filter$ = this.filterSubject.asObservable();
  readonly invoices$ = combineLatest([this.billing.invoices$, this.filter$]).pipe(
    map(([invoices, filter]) => filter === 'ALL' ? invoices : invoices.filter(invoice => invoice.status === filter))
  );

  constructor(private readonly billing: BillingService) {}

  ngOnInit(): void {
    this.billing.load();
  }

  setFilter(filter: InvoiceFilter): void {
    this.filterSubject.next(filter);
  }
}
