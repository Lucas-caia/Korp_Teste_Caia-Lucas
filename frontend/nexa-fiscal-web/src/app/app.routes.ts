import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login.page').then(m => m.LoginPage)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/register.page').then(m => m.RegisterPage)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/shell/shell.component').then(m => m.ShellComponent),
    children: [
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
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
