import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DoctorList } from '../Component/doctor-list/doctor-list';
import { TodayAppointments } from '../Component/today-appointments/today-appointments';
import { AddDoctor } from '../Component/add-doctor/add-doctor';

@Component({
  selector: 'app-home',
  imports: [FormsModule, DoctorList, TodayAppointments, AddDoctor],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Phome {
  searchText = '';
  refreshDoctors = false;

  onDoctorAdded() {
    this.refreshDoctors = !this.refreshDoctors; // toggle
  }

  openAddDoctorModal() {
    const modal = document.getElementById('addDoctorModal');
    modal?.classList.add('show');
    modal!.style.display = 'block';
  }

  reloadDoctors() {
    // emitted to refresh doctor list
  }
}