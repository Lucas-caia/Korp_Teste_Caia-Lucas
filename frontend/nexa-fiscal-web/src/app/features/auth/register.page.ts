import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegisterPage {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);

  loading = false;
  errorMessage = '';

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  });

  submit(): void {
    if (this.form.invalid || this.loading) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    if (value.password !== value.confirmPassword) {
      this.errorMessage = 'As senhas informadas não coincidem.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.auth.register({ name: value.name, email: value.email, password: value.password })
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: () => void this.router.navigateByUrl('/'),
        error: error => {
          this.errorMessage = error.error?.detail ?? 'Não foi possível criar sua conta. Tente novamente.';
        }
      });
  }
}
