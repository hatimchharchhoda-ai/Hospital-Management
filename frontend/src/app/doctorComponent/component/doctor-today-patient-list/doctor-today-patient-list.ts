import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DoctorService } from '../../../services/doctor-service';
import { ToastService } from '../../../shareComponent/toast/toast-service';

@Component({
  selector: 'app-doctor-today-patient-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-today-patient-list.html',
  styleUrl: './doctor-today-patient-list.css',
})
export class DoctorTodayPatientList {
  @Input() appointments: any[] = [];
  @Input() searchText = '';
  @Output() updated = new EventEmitter<void>();

  constructor(
    private doctorService: DoctorService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef
  ) {}

  get filteredAppointments() {
    return this.appointments.filter((a) =>
      (a.patientName + a.startTime)
        .toLowerCase()
        .includes(this.searchText.toLowerCase())
    );
  }

  updateStatus(appointment: any, newStatus: number) {
    const payload = {
      appointmentId: appointment.appointmentId,
      appointmentDate: appointment.appointmentDate,
      startTime: appointment.startTime,
      status: Number(newStatus), // ✅ enum-safe
      doctorId: appointment.doctorId,
      patientId: appointment.patientId,
    };

    this.doctorService.doctorUpdateAppointment(payload).subscribe({
      next: () => {
        this.toast.success('Appointment status updated successfully');
        this.updated.emit();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.toast.error(
          err?.error?.message || 'Failed to update appointment'
        );
      },
    });
  }
}