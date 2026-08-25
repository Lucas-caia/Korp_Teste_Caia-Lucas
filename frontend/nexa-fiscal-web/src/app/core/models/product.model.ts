export type StockStatus = 'normal' | 'low' | 'empty';

export interface Product {
  id: string;
  code: string;
  description: string;
  balance: number;
  minBalance: number;
}

export interface CreateProductInput {
  code: string;
  description: string;
  balance: number;
}

export function getStockStatus(product: Product): StockStatus {
  if (product.balance === 0) return 'empty';
  if (product.balance < product.minBalance) return 'low';
  return 'normal';
}
