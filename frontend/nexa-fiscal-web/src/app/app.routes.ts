import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/dashboard/dashboard.page').then(m => m.DashboardPage)
  },
  {
    path: 'products',
    loadComponent: () => import('./features/products/products.page').then(m => m.ProductsPage)
  },
  {
    path: 'invoices',
    loadComponent: () => import('./features/invoices/invoices.page').then(m => m.InvoicesPage)
  },
  {
    path: 'invoices/new',
    loadComponent: () => import('./features/invoices/invoice-form.page').then(m => m.InvoiceFormPage)
  },
  {
    path: 'invoices/:id',
    loadComponent: () => import('./features/invoices/invoice-detail.page').then(m => m.InvoiceDetailPage)
  },
  { path: '**', redirectTo: '' }
];
