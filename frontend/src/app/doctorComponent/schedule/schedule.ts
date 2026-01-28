import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { TokenService } from '../../services/token-service';
import { DoctorService } from '../../services/doctor-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../shareComponent/toast/toast-service';

@Component({
  selector: 'app-schedule',
  imports: [CommonModule, FormsModule],
  templateUrl: './schedule.html',
  styleUrl: './schedule.css',
})
export class Schedule implements OnInit {
  doctorId!: number;

  appointments: any[] = [];
  filteredAppointments: any[] = [];

  searchText = '';
  selectedDate = '';

  editAppointment: any = {
    appointmentId: 0,
    appointmentDate: '',
    startTime: '',
    status: 1,
    doctorId: 0,
    patientId: 0,
  };

  constructor(
    private doctorService: DoctorService,
    private tokenService: TokenService,
    private toast: ToastService,              // ✅ use your toaster
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.doctorId = this.tokenService.getUserId();

    if (!this.doctorId) {
      this.toast.error('Invalid doctor token');
      return;
    }

    this.loadAppointments();
  }

  loadAppointments() {
    this.doctorService.getUpcomingAppointments(this.doctorId).subscribe({
      next: (res) => {
        this.appointments = res;
        this.filteredAppointments = res;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Failed to load appointments');
      },
    });
  }

  applyFilter() {
    this.filteredAppointments = this.appointments.filter((a) => {
      const search = this.searchText.toLowerCase();

      const matchesSearch =
        a.patientName?.toLowerCase().includes(search) ||
        a.patientMobile?.includes(search) ||
        a.appointmentDate?.includes(search);

      const matchesDate =
        !this.selectedDate || a.appointmentDate === this.selectedDate;

      return matchesSearch && matchesDate;
    });

    this.cdr.detectChanges();
  }

  openEditModal(appointment: any) {
    this.editAppointment = {
      appointmentId: appointment.appointmentId,
      appointmentDate: appointment.appointmentDate,
      startTime: appointment.startTime,
      status: Number(appointment.status),
      doctorId: this.doctorId,
      patientId: appointment.patientId,
    };

    const modal = document.getElementById('editModal');
    modal?.classList.add('show');
    modal?.setAttribute('style', 'display:block');

    this.cdr.detectChanges();
  }

  closeModal() {
    const modal = document.getElementById('editModal');
    modal?.classList.remove('show');
    modal?.setAttribute('style', 'display:none');

    this.cdr.detectChanges();
  }

  updateAppointment() {
    const payload = {
      ...this.editAppointment,
      status: Number(this.editAppointment.status),
    };

    this.doctorService.doctorUpdateAppointment(payload).subscribe({
      next: () => {
        this.toast.success('Appointment updated successfully');
        this.closeModal();
        this.loadAppointments();
      },
      error: (err) => {
        this.toast.error(err.error?.message || 'Update failed');
      },
    });
  }
}