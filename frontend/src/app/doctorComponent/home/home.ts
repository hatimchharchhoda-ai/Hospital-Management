import { ChangeDetectorRef, Component } from '@angular/core';
import { DoctorService } from '../../services/doctor-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TokenService } from '../../services/token-service';
import { DoctorTodayPatientList } from '../component/doctor-today-patient-list/doctor-today-patient-list';
import { ToastService } from '../../shareComponent/toast/toast-service';

@Component({
  selector: 'app-home',
  imports: [CommonModule, FormsModule, DoctorTodayPatientList],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class DoctorHome {
  doctorId!: number;
  searchText = '';
  todayAppointments: any[] = [];

  constructor(
    private doctorService: DoctorService,
    private tokenService: TokenService,
    private toast: ToastService, // ✅ replaced ToastrService
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.initializeDoctor();
  }

  private initializeDoctor() {
    if (!this.tokenService.isLoggedIn()) {
      this.toast.error('Session expired. Please login again.');
      return;
    }

    const role = this.tokenService.getRole();
    if (role !== 'Doctor') {
      this.toast.error('Unauthorized access');
      return;
    }

    this.doctorId = this.tokenService.getUserId();

    if (!this.doctorId) {
      this.toast.error('Invalid doctor identity');
      return;
    }

    this.loadTodayAppointments();
  }

  loadTodayAppointments() {
    this.doctorService.getTodayAppointments(this.doctorId).subscribe({
      next: (res: any[]) => {
        console.log('Today Appointments:', res);
        this.todayAppointments = res;
        this.cdr.detectChanges(); // 🔄 force UI refresh
      },
      error: () => {
        this.toast.error('Failed to load appointments');
      },
    });
  }

  onAppointmentUpdated() {
    this.toast.success('Appointment updated successfully');
    this.loadTodayAppointments();
  }
}