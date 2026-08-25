import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, delay, map, of, switchMap } from 'rxjs';
import { Invoice, InvoiceItem } from '../models/invoice.model';
import { InventoryService } from './inventory.service';

const INITIAL_INVOICES: Invoice[] = [
  {
    id: 'inv-001', number: 'NF-2026-001', status: 'CLOSED',
    createdAt: '2026-08-18T09:30:00', closedAt: '2026-08-18T10:10:00',
    items: [
      { productId: 'p1', productCode: 'PRD-001', productDescription: 'Café Premium 500g', quantity: 10, balanceAtAddition: 57 },
      { productId: 'p4', productCode: 'PRD-004', productDescription: 'Feijão Carioca 1kg', quantity: 8, balanceAtAddition: 36 }
    ]
  },
  {
    id: 'inv-002', number: 'NF-2026-002', status: 'OPEN',
    createdAt: '2026-08-20T14:00:00',
    items: [
      { productId: 'p5', productCode: 'PRD-005', productDescription: 'Óleo de Soja 900ml', quantity: 4, balanceAtAddition: 15 }
    ]
  },
  {
    id: 'inv-003', number: 'NF-2026-003', status: 'CLOSED',
    createdAt: '2026-08-21T11:15:00', closedAt: '2026-08-21T11:42:00',
    items: [
      { productId: 'p7', productCode: 'PRD-007', productDescription: 'Macarrão Espaguete 500g', quantity: 12, balanceAtAddition: 74 }
    ]
  },
  {
    id: 'inv-004', number: 'NF-2026-004', status: 'OPEN',
    createdAt: '2026-08-23T16:30:00',
    items: [
      { productId: 'p6', productCode: 'PRD-006', productDescription: 'Sal Refinado 1kg', quantity: 4, balanceAtAddition: 5 }
    ]
  }
];

@Injectable({ providedIn: 'root' })
export class BillingService {
  private readonly invoicesSubject = new BehaviorSubject<Invoice[]>(INITIAL_INVOICES);
  readonly invoices$ = this.invoicesSubject.asObservable();

  constructor(private readonly inventory: InventoryService) {}

  snapshot(): Invoice[] {
    return this.invoicesSubject.value.map(invoice => ({ ...invoice, items: invoice.items.map(item => ({ ...item })) }));
  }

  getById(id: string): Invoice | undefined {
    return this.snapshot().find(invoice => invoice.id === id);
  }

  createInvoice(items: InvoiceItem[]): Observable<Invoice> {
    const current = this.invoicesSubject.value;
    const invoice: Invoice = {
      id: `inv-${Date.now()}`,
      number: this.generateNumber(current),
      status: 'OPEN',
      createdAt: new Date().toISOString(),
      items: items.map(item => ({ ...item }))
    };

    return of(invoice).pipe(
      delay(300),
      map(created => {
        this.invoicesSubject.next([created, ...current]);
        return created;
      })
    );
  }

  closeInvoice(id: string): Observable<Invoice> {
    const invoice = this.getById(id);
    if (!invoice) throw new Error('Nota fiscal não encontrada.');
    if (invoice.status === 'CLOSED') return of(invoice);

    return this.inventory.consume(invoice.items).pipe(
      switchMap(() => {
        const closed: Invoice = { ...invoice, status: 'CLOSED', closedAt: new Date().toISOString() };
        const updated = this.invoicesSubject.value.map(current => current.id === id ? closed : current);
        this.invoicesSubject.next(updated);
        return of(closed);
      })
    );
  }

  private generateNumber(invoices: Invoice[]): string {
    const year = new Date().getFullYear();
    const numbers = invoices
      .filter(invoice => invoice.number.startsWith(`NF-${year}-`))
      .map(invoice => Number(invoice.number.split('-').at(-1) ?? 0));
    const next = Math.max(0, ...numbers) + 1;
    return `NF-${year}-${String(next).padStart(3, '0')}`;
  }
}
