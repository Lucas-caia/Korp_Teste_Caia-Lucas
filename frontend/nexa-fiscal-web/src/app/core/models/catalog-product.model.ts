export interface CatalogProduct {
  id: string;
  code: string;
  description: string;
  createdAt: string;
}

export interface CreateCatalogProductInput {
  code: string;
  description: string;
}
