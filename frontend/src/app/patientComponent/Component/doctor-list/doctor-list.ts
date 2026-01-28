import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { PatientService } from '../../../services/patient-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../../services/token-service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-doctor-list',
  imports: [CommonModule],
  templateUrl: './doctor-list.html',
  styleUrl: './doctor-list.css',
})
export class DoctorList implements OnInit, OnChanges {
  @Input() searchText = '';

  doctors: any[] = [];
  filteredDoctors: any[] = [];
  patientId!: number;

  constructor(
    private patientService: PatientService,
    private router: Router,
    private tokenService: TokenService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.patientId = Number(this.tokenService.getUserId());
    this.loadDoctors();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['searchText'] && this.doctors.length) {
      this.applyFilter();
    }
  }

  loadDoctors() {
    if (!this.patientId) return;

    this.patientService.getDoctorsByPatient(this.patientId)
      .subscribe(res => {
        this.doctors = [...res];          // ✅ NEW reference
        this.filteredDoctors = [...res]; // ✅ INIT safely
        this.cdr.detectChanges();
      });
  }

  applyFilter() {
    const text = this.searchText.toLowerCase();

    this.filteredDoctors = this.doctors.filter(d =>
      (d.fullName + d.email + d.mobile)
        .toLowerCase()
        .includes(text)
    );
  }

  goToDoctor(doctorId: number) {
    this.router.navigate(['/patient', doctorId, 'appointment']);
  }
}
