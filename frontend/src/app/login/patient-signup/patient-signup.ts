import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../services/patient-service';
import { firstValueFrom } from 'rxjs';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../services/auth-service';  

@Component({
  selector: 'app-patient-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink ],
  templateUrl: './patient-signup.html',
  styleUrl: './patient-signup.css', 
})
export class PatientSignup {
  patientForm: FormGroup;
  patientService = inject(PatientService);
  toastr = inject(ToastrService);
  AuthService = inject(AuthService);

  constructor(private fb: FormBuilder, private router: Router) {
    this.patientForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      mobile: ['', [Validators.required, Validators.maxLength(10)]],
      password: ['', [Validators.required, Validators.maxLength(10)]],
    });
  }

  async checkAndRegister() {
  if (this.patientForm.invalid) {
    this.toastr.error('Please fill all required fields correctly');
    return;
  }

  const { name, mobile, password } = this.patientForm.value;

  try {
    // 1️⃣ Check if mobile already exists
    const mobileCheck = await firstValueFrom(this.patientService.checkMobile(mobile));
    if (mobileCheck!.exists) {
      this.toastr.error('Mobile number already registered');
      return;
    }

    // 2️⃣ Register patient
    const res = await firstValueFrom(
      this.AuthService.registerPatient({ name, mobile, password })
    );

    this.toastr.success('Patient registered successfully!');
    this.patientForm.reset();
    this.router.navigate(['/patient-login']);

  } catch (err: any) {
    console.error(err);
    this.toastr.error(err?.message || 'Registration failed');
  }
}

}
