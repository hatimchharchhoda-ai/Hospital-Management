import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Doctor } from '../../models/doctor_signup';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientService } from '../../services/patient-service';
import { TokenService } from '../../services/token-service';
import { PrescriptionModel } from '../../models/prescriptionModel';
import { ToastService } from '../../shareComponent/toast/toast-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-prescription',
  imports: [CommonModule, FormsModule],
  standalone: true, 
  templateUrl: './prescription.html',
  styleUrl: './prescription.css',
})
export class PatientPrescriptionComponent implements OnInit {
  patientId: number | null = null;

  doctors: Doctor[] = [];
  filteredDoctors: Doctor[] = [];

  expandedDoctorId: number | null = null;
  prescriptionsMap = new Map<number, PrescriptionModel[]>();
  loadingDoctorId: number | null = null;

  searchText = '';

  // Image preview modal
  showImageModal = false;
  selectedImageUrl = '';
  selectedPrescriptionId: number | null = null;

  constructor(
    private patientService: PatientService,
    private tokenService: TokenService,
    private cdr: ChangeDetectorRef,
    private toast: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Wait for browser to be ready
    if (!this.tokenService.isBrowser()) {
      console.warn('Not in browser environment');
      return;
    }

    // Check if user is logged in
    if (!this.tokenService.isLoggedIn()) {
      console.error('User is not logged in or token expired');
      this.toast.error('Session expired. Please login again.');
      this.router.navigate(['/login']);
      return;
    }

    // Get patient ID
    this.patientId = this.tokenService.getUserId();

    if (!this.patientId) {
      console.error('Patient ID not found in token');
      this.toast.error('Unable to retrieve user information. Please login again.');
      this.router.navigate(['/login']);
      return;
    }

    console.log('Patient ID retrieved:', this.patientId);
    this.loadDoctors();
  }

  loadDoctors(): void {
    if (!this.patientId) {
      this.toast.error('Invalid patient ID');
      return;
    }

    this.patientService
      .getDoctorsByPatient(this.patientId)
      .subscribe({
        next: (res) => {
          this.doctors = res;
          this.filteredDoctors = res;
          this.cdr.detectChanges();
          
          if (res.length === 0) {
            this.toast.info('No doctors found');
          } else {
            this.toast.success(`Loaded ${res.length} doctor(s)`);
          }
        },
        error: (err) => {
          console.error('Error loading doctors:', err);
          this.toast.error('Failed to load doctors. Please try again.');
        }
      });
  }

  filterDoctors(): void {
    const q = this.searchText.toLowerCase().trim();
    
    if (!q) {
      this.filteredDoctors = this.doctors;
      return;
    }

    this.filteredDoctors = this.doctors.filter(d =>
      d.fullName.toLowerCase().includes(q) ||
      d.email.toLowerCase().includes(q) ||
      d.mobile.includes(q)
    );
  }

  toggleDoctor(doctor: Doctor): void {
    if (this.expandedDoctorId === doctor.doctorId) {
      this.expandedDoctorId = null;
      return;
    }

    this.expandedDoctorId = doctor.doctorId;

    if (!this.prescriptionsMap.has(doctor.doctorId)) {
      this.loadPrescriptions(doctor.doctorId);
    }
  }

  loadPrescriptions(doctorId: number): void {
    if (!this.patientId) {
      this.toast.error('Invalid patient ID');
      return;
    }

    this.loadingDoctorId = doctorId;

    this.patientService
      .getPrescriptions(this.patientId, doctorId)
      .subscribe({
        next: (res) => {
          this.prescriptionsMap.set(doctorId, res);
          this.loadingDoctorId = null;
          this.cdr.detectChanges();

          if (res.length === 0) {
            this.toast.info('No prescriptions found for this doctor');
          } else {
            this.toast.success(`Loaded ${res.length} prescription(s)`);
          }
        },
        error: (err) => {
          console.error('Error loading prescriptions:', err);
          this.loadingDoctorId = null;
          this.toast.error('Failed to load prescriptions. Please try again.');
          this.cdr.detectChanges();
        }
      });
  }

  downloadPrescription(imageUrl: string, prescriptionId: number): void {
    if (!imageUrl) {
      this.toast.error('No image available for download');
      return;
    }

    const extension = imageUrl.split('.').pop()?.toLowerCase() || 'jpg';

    this.toast.info('Downloading prescription...');

    fetch(imageUrl)
      .then(res => {
        if (!res.ok) {
          throw new Error('Failed to fetch image');
        }
        return res.blob();
      })
      .then(blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `prescription-${prescriptionId}.${extension}`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        
        this.toast.success('Prescription downloaded successfully');
      })
      .catch((err) => {
        console.error('Error downloading prescription:', err);
        this.toast.error('Failed to download. Opening in new tab...');
        window.open(imageUrl, '_blank');
      });
  }

  previewImage(imageUrl: string, prescriptionId: number): void {
    if (!imageUrl) {
      this.toast.error('No image available for preview');
      return;
    }

    this.selectedImageUrl = imageUrl;
    this.selectedPrescriptionId = prescriptionId;
    this.showImageModal = true;
  }

  closeImageModal(): void {
    this.showImageModal = false;
    this.selectedImageUrl = '';
    this.selectedPrescriptionId = null;
  }

  getPrescriptions(doctorId: number): PrescriptionModel[] {
    return this.prescriptionsMap.get(doctorId) || [];
  }
}