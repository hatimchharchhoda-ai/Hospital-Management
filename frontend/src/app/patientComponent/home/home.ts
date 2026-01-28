import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DoctorList } from '../Component/doctor-list/doctor-list';
import { TodayAppointments } from '../Component/today-appointments/today-appointments';
import { AddDoctor } from '../Component/add-doctor/add-doctor';
import { ToastService } from '../../shareComponent/toast/toast-service';

@Component({
  selector: 'app-home',
  imports: [FormsModule, DoctorList, TodayAppointments, AddDoctor],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Phome {
  searchText = '';
  refreshDoctors = false;

  constructor(private toast: ToastService) {} // ✅ inject

  onDoctorAdded() {
    this.refreshDoctors = !this.refreshDoctors; // toggle refresh
    this.toast.success('Doctor added successfully'); // ✅ toast
  }

  openAddDoctorModal() {
    const modal = document.getElementById('addDoctorModal');
    modal?.classList.add('show');
    modal!.style.display = 'block';

    // optional UX feedback
    this.toast.info('Fill doctor details to add');
  }

  reloadDoctors() {
    this.refreshDoctors = !this.refreshDoctors;
    this.toast.info('Doctor list refreshed');
  }
}