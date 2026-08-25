import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CatalogProduct, CreateCatalogProductInput } from '../models/catalog-product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly productsSubject = new BehaviorSubject<CatalogProduct[]>([]);
  readonly products$ = this.productsSubject.asObservable();
  private loaded = false;

  constructor(private readonly http: HttpClient) {}

  load(force = false): void {
    if (this.loaded && !force) return;

    this.http.get<CatalogProduct[]>(`${environment.inventoryApiUrl}/products`).subscribe({
      next: products => {
        this.productsSubject.next(products);
        this.loaded = true;
      },
      error: () => {
        this.productsSubject.next([]);
        this.loaded = false;
      }
    });
  }

  create(input: CreateCatalogProductInput): Observable<CatalogProduct> {
    return this.http.post<CatalogProduct>(`${environment.inventoryApiUrl}/products`, input).pipe(
      tap(product => {
        this.productsSubject.next(
          [...this.productsSubject.value, product]
            .sort((a, b) => a.description.localeCompare(b.description))
        );
      }),
      catchError((error: HttpErrorResponse) => {
        const message = error.status === 409
          ? 'Já existe um produto com este código.'
          : 'Não foi possível cadastrar o produto.';
        return throwError(() => new Error(message));
      })
    );
  }
}
