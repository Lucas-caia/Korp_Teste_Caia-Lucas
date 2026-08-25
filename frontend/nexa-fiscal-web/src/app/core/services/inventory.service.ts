import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, delay, map, of, throwError } from 'rxjs';
import { CreateProductInput, Product } from '../models/product.model';
import { InsufficientStockItem, InvoiceItem } from '../models/invoice.model';

export class InsufficientStockError extends Error {
  constructor(public readonly items: InsufficientStockItem[]) {
    super('Estoque insuficiente para concluir a nota.');
  }
}

const INITIAL_PRODUCTS: Product[] = [
  { id: 'p1', code: 'PRD-001', description: 'Café Premium 500g', balance: 47, minBalance: 10 },
  { id: 'p2', code: 'PRD-002', description: 'Açúcar Cristal 1kg', balance: 3, minBalance: 20 },
  { id: 'p3', code: 'PRD-003', description: 'Arroz Tipo 1 5kg', balance: 0, minBalance: 15 },
  { id: 'p4', code: 'PRD-004', description: 'Feijão Carioca 1kg', balance: 28, minBalance: 10 },
  { id: 'p5', code: 'PRD-005', description: 'Óleo de Soja 900ml', balance: 15, minBalance: 8 },
  { id: 'p6', code: 'PRD-006', description: 'Sal Refinado 1kg', balance: 5, minBalance: 12 },
  { id: 'p7', code: 'PRD-007', description: 'Macarrão Espaguete 500g', balance: 62, minBalance: 15 },
  { id: 'p8', code: 'PRD-008', description: 'Molho de Tomate 340g', balance: 4, minBalance: 18 }
];

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private readonly productsSubject = new BehaviorSubject<Product[]>(INITIAL_PRODUCTS);
  readonly products$ = this.productsSubject.asObservable();

  snapshot(): Product[] {
    return this.productsSubject.value.map(product => ({ ...product }));
  }

  addProduct(input: CreateProductInput): Observable<Product> {
    const normalizedCode = input.code.trim().toUpperCase();
    const duplicate = this.productsSubject.value.some(product => product.code === normalizedCode);

    if (duplicate) {
      return throwError(() => new Error('Já existe um produto com este código.'));
    }

    const product: Product = {
      id: `p-${Date.now()}`,
      code: normalizedCode,
      description: input.description.trim(),
      balance: Number(input.balance),
      minBalance: 10
    };

    return of(product).pipe(
      delay(250),
      map(created => {
        this.productsSubject.next([...this.productsSubject.value, created]);
        return created;
      })
    );
  }

  consume(items: InvoiceItem[]): Observable<void> {
    const products = this.snapshot();
    const insufficient: InsufficientStockItem[] = [];

    for (const item of items) {
      const product = products.find(current => current.id === item.productId);
      const available = product?.balance ?? 0;
      if (!product || available < item.quantity) {
        insufficient.push({
          productId: item.productId,
          code: item.productCode,
          description: item.productDescription,
          requested: item.quantity,
          available
        });
      }
    }

    if (insufficient.length > 0) {
      return of(null).pipe(
        delay(650),
        map(() => {
          throw new InsufficientStockError(insufficient);
        })
      );
    }

    return of(undefined).pipe(
      delay(900),
      map(() => {
        const updated = products.map(product => {
          const item = items.find(current => current.productId === product.id);
          return item ? { ...product, balance: product.balance - item.quantity } : product;
        });
        this.productsSubject.next(updated);
      })
    );
  }
}
