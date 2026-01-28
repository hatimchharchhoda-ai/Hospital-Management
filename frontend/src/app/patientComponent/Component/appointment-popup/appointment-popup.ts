import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../services/patient-service';

@Component({
  selector: 'app-appointment-popup',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointment-popup.html',
  styleUrl: './appointment-popup.css',
})
export class AppointmentPopup {
  @Input() doctorId!: number;
  @Input() patientId!: number;
  @Input() date!: Date;
  @Output() close = new EventEmitter<void>();

  timeSlots = [
    '09:00', '09:30', '10:00', '10:30',
    '11:00', '11:30', '12:00', '12:30',
    '14:00', '14:30', '15:00', '15:30',
    '16:00', '16:30', '17:00', '17:30'
  ];

  selectedTime: string | null = null;
  loading = false;

  constructor(
    private patientService: PatientService,
    private toastr: ToastrService
  ) {}

  selectTime(time: string) {
    this.selectedTime = time;
  }

  bookAppointment() {
    if (!this.selectedTime) {
      this.toastr.warning('Please select a time slot');
      return;
    }

    this.patientService.bookAppointment({
      doctorId: this.doctorId,
      patientId: this.patientId,
      appointmentDate: this.date.toISOString().split('T')[0],
      startTime: this.selectedTime
    }).subscribe({
      next: () => {
        this.toastr.success('Appointment booked successfully');
        this.loading = false;
        this.close.emit();
      },
      error: err => {
        this.toastr.error(err.error?.message || 'Booking failed');
      }
    });
  }
}