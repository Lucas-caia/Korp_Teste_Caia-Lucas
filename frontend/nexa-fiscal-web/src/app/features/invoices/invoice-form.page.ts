import { ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { BillingService } from '../../core/services/billing.service';

@Component({
  selector: 'app-invoice-form-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './invoice-form.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InvoiceFormPage {
  processing = false;
  message = '';

  constructor(
    private readonly billing: BillingService,
    private readonly router: Router,
    private readonly cdr: ChangeDetectorRef
  ) {}

  create(): void {
    if (this.processing) return;

    this.processing = true;
    this.message = '';

    this.billing.createInvoice().subscribe({
      next: invoice => this.router.navigate(['/invoices', invoice.id]),
      error: (error: Error) => {
        this.processing = false;
        this.message = error.message;
        this.cdr.markForCheck();
      }
    });
  }
}
