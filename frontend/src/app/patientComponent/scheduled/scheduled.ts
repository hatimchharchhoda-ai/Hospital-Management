import { ChangeDetectorRef, Component } from '@angular/core';
import { PatientService } from '../../services/patient-service';
import { TokenService } from '../../services/token-service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-scheduled',
  imports: [CommonModule, FormsModule],
  templateUrl: './scheduled.html',
  styleUrl: './scheduled.css',
})
export class Scheduled {
  appointments: any[] = [];
  selectedAppointment: any = null;

  timeSlots = [
    '09:00', '09:30', '10:00', '10:30',
    '11:00', '11:30', '12:00', '12:30',
    '14:00', '14:30', '15:00', '15:30',
    '16:00', '16:30', '17:00', '17:30'
  ];

  loading = false;

  constructor(
    private patientService: PatientService,
    private tokenService: TokenService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments() {
    const patientId = Number(this.tokenService.getUserId());

    this.patientService
      .getUpcomingAppointments(patientId)
      .subscribe({
        next: res => {
          this.appointments = res;
          this.cdr.detectChanges();
        },
        error: (err: { error: { message: any; }; }) => this.toastr.error(err.error?.message || 'Failed to load appointments')
      });
  }

  openEdit(appointment: any) {
    this.selectedAppointment = {
      ...appointment,
      appointmentDate: appointment.appointmentDate
    };
  }

  updateAppointment() {
    this.loading = true;

    const payload = {
      appointmentId: this.selectedAppointment.appointmentId,
      appointmentDate: this.selectedAppointment.appointmentDate,
      startTime: this.selectedAppointment.startTime,
      patientId: this.selectedAppointment.patientId,
      doctorId: this.selectedAppointment.doctorId
    };

    this.patientService.updateAppointment(payload).subscribe({
      next: () => {
        this.toastr.success('Appointment updated successfully');
        this.loading = false;
        this.selectedAppointment = null;
        this.loadAppointments();
        this.cdr.detectChanges();
      },
      error: (err: { error: { message: any; }; }) => {
        this.toastr.error(err.error?.message || 'Update failed');
      }
    });
  }
}
