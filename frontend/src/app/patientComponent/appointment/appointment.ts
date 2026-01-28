import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TokenService } from '../../services/token-service';
import { CommonModule } from '@angular/common';
import { AppointmentPopup } from '../Component/appointment-popup/appointment-popup';

@Component({
  selector: 'app-appointment',
  standalone: true,
  imports: [CommonModule, AppointmentPopup],
  templateUrl: './appointment.html',
  styleUrl: './appointment.css',
})

export class Appointment implements OnInit {
  doctorId!: number;
  patientId!: number;

  weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  calendarDays: (Date | null)[] = [];

  selectedDate?: Date;
  showPopup = false;

  constructor(
    private route: ActivatedRoute,
    private tokenService: TokenService
  ) {}

  ngOnInit() {
    this.doctorId = Number(this.route.snapshot.paramMap.get('doctorId'));
    this.patientId = Number(this.tokenService.getUserId());
    this.generateNext30DaysCalendar();
  }

  generateNext30DaysCalendar() {
    const today = new Date();
    const startDayIndex = today.getDay(); // weekday of today (0–6)

    this.calendarDays = [];

    // 🔹 Empty cells BEFORE today (for weekday alignment)
    for (let i = 0; i < startDayIndex; i++) {
      this.calendarDays.push(null);
    }

    // 🔹 Add next 30 days (including today)
    for (let i = 0; i < 30; i++) {
      const d = new Date(today);
      d.setDate(today.getDate() + i);
      this.calendarDays.push(d);
    }
  }

  openSlots(date: Date | null) {
    if (!date) return;

    this.selectedDate = date;
    this.showPopup = true;
  }

  closePopup() {
    this.showPopup = false;
  }
}