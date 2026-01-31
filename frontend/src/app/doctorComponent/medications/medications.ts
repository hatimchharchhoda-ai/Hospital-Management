import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Patient } from '../../models/patient_login';
import { ToastService } from '../../shareComponent/toast/toast-service';
import { DoctorService } from '../../services/doctor-service';
import { TokenService } from '../../services/token-service';
import { PatientPrescription } from '../../models/patientPrescription';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-medications',
  imports: [FormsModule, CommonModule],
  templateUrl: './medications.html',
  styleUrl: './medications.css',
})
export class Medications implements OnInit {
  doctorId: number | null = null;

  patients: Patient[] = [];
  filteredPatients: Patient[] = [];

  expandedPatientId: number | null = null;
  prescriptionsMap = new Map<number, PatientPrescription[]>();
  loadingPatientId: number | null = null;

  searchText = '';

  // Image preview modal
  showImageModal = false;
  selectedImageUrl = '';
  selectedPrescriptionId: number | null = null;

  // Create prescription modal
  showCreateModal = false;
  selectedPatient: Patient | null = null;
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  isSubmitting = false;

  newPrescription = {
    description: '',
  };

  constructor(
    private doctorService: DoctorService,
    private tokenService: TokenService,
    private cdr: ChangeDetectorRef,
    private toast: ToastService,
  ) {}

 ngOnInit(): void {
  this.doctorId = this.tokenService.getUserId();

  if (!this.doctorId) {
    this.toast.error('Unable to identify doctor');
    return;
  }

  console.log('Doctor ID retrieved:', this.doctorId);
  this.loadPatients();
}


loadPatients(): void {
  if (!this.doctorId) return;

  this.doctorService.getPatientsByDoctor(this.doctorId).subscribe({
    next: (res: Patient[]) => {
      this.patients = res;
      this.filteredPatients = res;
      this.cdr.detectChanges();
    },
    error: () => {
      this.toast.error('Failed to load patients');
    }
  });
}

  filterPatients(): void {
    const q = this.searchText.toLowerCase().trim();

    if (!q) {
      this.filteredPatients = this.patients;
      return;
    }

    this.filteredPatients = this.patients.filter(p =>
      p.name.toLowerCase().includes(q) ||
      p.mobile.includes(q)
    );
  }

  togglePatient(patient: Patient): void {
    if (this.expandedPatientId === patient.patientId) {
      this.expandedPatientId = null;
      return;
    }

    this.expandedPatientId = patient.patientId;

    if (!this.prescriptionsMap.has(patient.patientId)) {
      this.loadPrescriptions(patient.patientId);
    }
  }

loadPrescriptions(patientId: number): void {
  if (!this.doctorId) return;
  const token = localStorage.getItem('jwt_token');
  if (token) {
    console.log(JSON.parse(atob(token.split('.')[1])));
  }

  this.loadingPatientId = patientId;

  this.doctorService
    .getPrescriptionsByPatient(this.doctorId, patientId)
    .subscribe({
      next: (res: PatientPrescription[]) => {
        this.prescriptionsMap.set(patientId, res ?? []);
        this.loadingPatientId = null;
        this.cdr.detectChanges();
        console.log(`Prescriptions loaded for patient ${patientId}:`, res);
      },
      error: () => {
        this.toast.error('Failed to load prescriptions');
      }
    });
}

  openCreateModal(patient: Patient): void {
    this.selectedPatient = patient;
    this.showCreateModal = true;
    this.resetForm();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
    this.selectedPatient = null;
    this.resetForm();
  }

  resetForm(): void {
    this.newPrescription = {
      description: '',
    };
    this.selectedFile = null;
    this.imagePreview = null;
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    
    if (!file) {
      return;
    }

    // Validate file type
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
    if (!allowedTypes.includes(file.type)) {
      this.toast.error('Please select a valid image file (JPG, PNG, GIF)');
      event.target.value = '';
      return;
    }

    // Validate file size (max 5MB)
    const maxSize = 5 * 1024 * 1024; // 5MB in bytes
    if (file.size > maxSize) {
      this.toast.error('File size must be less than 5MB');
      event.target.value = '';
      return;
    }

    this.selectedFile = file;

    // Create image preview
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.imagePreview = e.target.result;
      this.cdr.detectChanges();
    };
    reader.readAsDataURL(file);
  }

 createPrescription(): void {
  if (!this.selectedFile || !this.doctorId || !this.selectedPatient) {
    this.toast.error('Invalid data');
    return;
  }

  this.isSubmitting = true;

  const formData = new FormData();
  formData.append('DoctorId', this.doctorId.toString());
  formData.append('PatientId', this.selectedPatient.patientId.toString());
  formData.append('Description', this.newPrescription.description || '');
  formData.append('Image', this.selectedFile);
  formData.forEach((value, key) => {
  console.log(key, value);
});
  this.doctorService.createPrescription(formData).subscribe({
    next: () => {
      this.toast.success('Prescription created successfully');
      this.isSubmitting = false;
      this.loadPrescriptions(this.selectedPatient!.patientId);
    },
    error: () => {
      this.isSubmitting = false;
      this.toast.error('Failed to create prescription');
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

  getPrescriptions(patientId: number): PatientPrescription[] {
    return this.prescriptionsMap.get(patientId) || [];
  }
}
