import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateInvoiceItemInput, Invoice } from '../models/invoice.model';

@Injectable({ providedIn: 'root' })
export class BillingService {
  private readonly invoicesSubject = new BehaviorSubject<Invoice[]>([]);
  readonly invoices$ = this.invoicesSubject.asObservable();
  private loaded = false;

  constructor(private readonly http: HttpClient) {}

  load(force = false): void {
    if (this.loaded && !force) return;

    this.http.get<Invoice[]>(`${environment.billingApiUrl}/invoices`).subscribe({
      next: invoices => {
        this.invoicesSubject.next(invoices);
        this.loaded = true;
      },
      error: () => {
        this.invoicesSubject.next([]);
        this.loaded = false;
      }
    });
  }

  getById(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${environment.billingApiUrl}/invoices/${id}`);
  }

  createInvoice(items: CreateInvoiceItemInput[]): Observable<Invoice> {
    return this.http.post<Invoice>(`${environment.billingApiUrl}/invoices`, { items }).pipe(
      tap(invoice => this.invoicesSubject.next([invoice, ...this.invoicesSubject.value])),
      catchError((error: HttpErrorResponse) => {
        const message = error.status === 0 || error.status >= 500
          ? 'O serviço de faturamento está temporariamente indisponível.'
          : error.error?.detail ?? 'Não foi possível criar a nota fiscal.';

        return throwError(() => new Error(message));
      })
    );
  }
}
