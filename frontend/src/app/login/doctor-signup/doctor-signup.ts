import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DoctorRegisterDto } from '../../models/doctor_signup';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth-service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-doctor-signup',
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule, RouterLink],
  templateUrl: './doctor-signup.html',
  styleUrl: './doctor-signup.css',
})
export class DoctorSignup {
  doctorSignupForm: FormGroup;
  isSubmitting = false;
  toastr = inject(ToastrService);
  
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
  ) {
    this.doctorSignupForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      mobile: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      specialization: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSignup() {
    if (this.doctorSignupForm.invalid) return;

    this.isSubmitting = true;

    const doctor: DoctorRegisterDto = this.doctorSignupForm.value;

    this.authService.registerDoctor(doctor).subscribe({
      next: () => {
        this.toastr.success('Doctor registered successfully', 'Success');
        this.router.navigate(['/doctor-login']);
      },
      error: (err: { error: { message: any; }; }) => {
        this.toastr.error(
          err.error?.message || 'Registration failed',
          'Error'
        );
        this.isSubmitting = false;
      }
    });
  }
}