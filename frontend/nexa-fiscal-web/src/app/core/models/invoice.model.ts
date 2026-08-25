export type InvoiceStatus = 'OPEN' | 'CLOSED';

export interface Invoice {
  id: string;
  number: string;
  status: InvoiceStatus;
  createdAt: string;
  closedAt?: string;
}

export interface InvoiceItem {
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
  balanceAtAddition: number;
}

export interface InsufficientStockItem {
  productId: string;
  code: string;
  description: string;
  requested: number;
  available: number;
}
