import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateInvoiceItemInput,
  InsufficientStockItem,
  Invoice
} from '../models/invoice.model';

export type InvoiceCloseErrorCode =
  | 'INSUFFICIENT_STOCK'
  | 'INVENTORY_UNAVAILABLE'
  | 'ALREADY_CLOSED'
  | 'NOT_FOUND'
  | 'UNKNOWN';

export class InvoiceCloseError extends Error {
  constructor(
    public readonly code: InvoiceCloseErrorCode,
    message: string,
    public readonly items: InsufficientStockItem[] = []
  ) {
    super(message);
  }
}

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

  closeInvoice(id: string): Observable<Invoice> {
    return this.http.post<Invoice>(`${environment.billingApiUrl}/invoices/${id}/close`, {}).pipe(
      tap(invoice => {
        const current = this.invoicesSubject.value;
        const exists = current.some(item => item.id === invoice.id);

        this.invoicesSubject.next(
          exists
            ? current.map(item => item.id === invoice.id ? invoice : item)
            : [invoice, ...current]
        );
      }),
      catchError((error: HttpErrorResponse) => {
        const stockItems = Array.isArray(error.error?.items)
          ? error.error.items as InsufficientStockItem[]
          : [];

        if (error.status === 409 && stockItems.length > 0) {
          return throwError(() => new InvoiceCloseError(
            'INSUFFICIENT_STOCK',
            'Estoque insuficiente para concluir a nota.',
            stockItems
          ));
        }

        if (error.status === 409) {
          return throwError(() => new InvoiceCloseError(
            'ALREADY_CLOSED',
            error.error?.detail ?? 'Esta nota fiscal já foi fechada.'
          ));
        }

        if (error.status === 404) {
          return throwError(() => new InvoiceCloseError(
            'NOT_FOUND',
            'A nota fiscal não foi encontrada.'
          ));
        }

        if (error.status === 0 || error.status === 503 || error.status >= 500) {
          return throwError(() => new InvoiceCloseError(
            'INVENTORY_UNAVAILABLE',
            'O serviço de estoque está temporariamente indisponível. Sua nota permanece aberta e nenhuma alteração foi confirmada.'
          ));
        }

        return throwError(() => new InvoiceCloseError(
          'UNKNOWN',
          error.error?.detail ?? 'Não foi possível fechar a nota fiscal.'
        ));
      })
    );
  }
}
