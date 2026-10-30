import { Component, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth-service';
import { Router } from '@angular/router';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { LoginRequestDto } from '../../../shared/models/auth-models';

@Component({
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  selector: 'app-login',
  styleUrl: './login.scss',
  templateUrl: './login.html',
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(NonNullableFormBuilder);

  protected readonly loginForm = this.formBuilder.group({
    userName: ['', Validators.required],
    password: ['', Validators.required]
  });

  protected onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    // const userName = this.loginForm.controls.userName.value
    // const password = this.loginForm.controls.password.value
    const loginRequest = this.loginForm.getRawValue() as LoginRequestDto;

    this.authService
      .login(loginRequest)
      .subscribe(() => this.router.navigate(['/']));
  }
}
