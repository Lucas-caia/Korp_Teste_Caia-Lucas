export interface CatalogProduct {
  id: string;
  code: string;
  description: string;
  balance: number;
  createdAt: string;
}

export interface CreateCatalogProductInput {
  code: string;
  description: string;
  balance: number;
}
