import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth-service';
import { TokenService } from '../../services/token-service';

@Component({
  selector: 'app-patient-login',
  standalone: true,
  imports: [CommonModule, HttpClientModule, ReactiveFormsModule, RouterLink],
  templateUrl: './patient-login.html',
  styleUrl: './patient-login.css',
})
export class PatientLogin {
  private fb = inject(FormBuilder);
  private AuthService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private tokenService: TokenService = inject(TokenService);

  loginForm = this.fb.group({
    mobile: ['', [Validators.required, Validators.maxLength(10)]],
    password: ['', [Validators.required, Validators.maxLength(10)]],
  });

  async login() {
    if (this.loginForm.invalid) {
      this.toastr.error('Please enter valid mobile and password');
      return;
    }

    try {
      // ✅ Call backend login
      const response = await firstValueFrom(
        this.AuthService.loginPatient(this.loginForm.value as {
          mobile: string;
          password: string;
        })
      );

      // ✅ Store JWT token
      this.tokenService.setToken((response as any).token);

      this.toastr.success('Login successful');

      // ✅ Navigate to patient home page
      this.router.navigate(['/patient/home']);

    } catch (err: any) {
      if (err.status === 401) {
        this.toastr.error('Invalid mobile or password');
      } else {
        this.toastr.error('Login failed');
      }
    }
  }
}
