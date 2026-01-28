import { Component, OnInit } from '@angular/core';
import { PatientService } from '../../../services/patient-service';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../../services/token-service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-today-appointments',
  imports: [CommonModule],
  templateUrl: './today-appointments.html',
  styleUrl: './today-appointments.css',
})
export class TodayAppointments implements OnInit{
  appointments: any[] = [];
  patientId!: number;

  constructor(
    private patientService: PatientService, 
    private tokenService: TokenService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.patientId = this.tokenService.getPatientId();

    this.patientService.getTodayAppointments(this.patientId)
      .subscribe(res => {
        this.appointments = res;
        this.cdr.detectChanges();
      });
  }
}
