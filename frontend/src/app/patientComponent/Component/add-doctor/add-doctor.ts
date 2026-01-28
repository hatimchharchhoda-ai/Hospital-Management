import { Component, EventEmitter, Output } from '@angular/core';
import { PatientService } from '../../../services/patient-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TokenService } from '../../../services/token-service';

@Component({
  selector: 'app-add-doctor',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-doctor.html',
  styleUrl: './add-doctor.css',
})
export class AddDoctor {
  @Output() doctorAdded = new EventEmitter<void>();

  doctorIdentifier = '';
  patientId!: number;

  constructor(private patientService: PatientService, private tokenService: TokenService) {
    this.patientId = this.tokenService.getPatientId();
  }

  addDoctor() {
    this.patientService.addDoctor({
      patientId: this.patientId,
      doctorIdentifier: this.doctorIdentifier
    }).subscribe(() => {
      this.close();
      this.doctorAdded.emit();
    });
  }

  close() {
    const modal = document.getElementById('addDoctorModal');
    modal?.classList.remove('show');
    modal!.style.display = 'none';
    this.doctorIdentifier = '';
  }
}
