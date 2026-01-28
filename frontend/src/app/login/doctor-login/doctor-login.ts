import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LoginDoctorDto } from '../../models/doctor_signup';
import { AuthService } from '../../services/auth-service';
import { ToastrService } from 'ngx-toastr';
import { TokenService } from '../../services/token-service';

@Component({
  selector: 'app-doctor-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './doctor-login.html',
  styleUrl: './doctor-login.css',
})
export class DoctorLogin {
   doctorLoginForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router,
    private tokenService: TokenService = inject(TokenService)
  ) {
    this.doctorLoginForm = this.fb.group({
      identifier: ['', Validators.required], // email or mobile
      password: ['', Validators.required]
    });
  }

  onLogin() {
    if (this.doctorLoginForm.invalid) {
      this.toastr.warning('Please fill all required fields');
      return;
    }

    this.isSubmitting = true;

    const dto: LoginDoctorDto = this.doctorLoginForm.value;

    this.authService.loginDoctor(dto).subscribe({
      next: (res) => {
        this.tokenService.setToken((res as any).token);
        this.toastr.success('Login successful', 'Welcome');
        this.router.navigate(['/doctor/home']);
      },
      error: (err) => {
        this.toastr.error(
          err.error?.message || 'Invalid credentials',
          'Login Failed'
        );
        this.isSubmitting = false;
      }
    });
  }
}