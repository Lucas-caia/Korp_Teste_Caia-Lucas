export type InvoiceStatus = 'OPEN' | 'CLOSED';

export interface StoredInvoiceItem {
  productId: string;
  quantity: number;
}

export interface Invoice {
  id: string;
  number: string;
  status: InvoiceStatus;
  items: StoredInvoiceItem[];
  createdAt: string;
  closedAt: string | null;
}

export interface CreateInvoiceItemInput {
  productId: string;
  quantity: number;
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
