import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { Invoice } from '../../core/models/invoice.model';
import { BillingService } from '../../core/services/billing.service';

@Component({
  selector: 'app-invoice-detail-page',
  standalone: true,
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './invoice-detail.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvoiceDetailPage implements OnInit {
  invoice$!: Observable<Invoice | undefined>;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly billing: BillingService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.invoice$ = this.billing.getById(id).pipe(catchError(() => of(undefined)));
  }
}
