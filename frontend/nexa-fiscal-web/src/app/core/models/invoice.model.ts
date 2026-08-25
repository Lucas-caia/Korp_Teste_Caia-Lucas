export type InvoiceStatus = 'OPEN' | 'CLOSED';

export interface InvoiceItem {
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
  balanceAtAddition: number;
}

export interface Invoice {
  id: string;
  number: string;
  status: InvoiceStatus;
  items: InvoiceItem[];
  createdAt: string;
  closedAt?: string;
}

export interface InsufficientStockItem {
  productId: string;
  code: string;
  description: string;
  requested: number;
  available: number;
}
