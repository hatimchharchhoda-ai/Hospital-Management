import { Routes } from '@angular/router';
import { PatientLogin } from './login/patient-login/patient-login';
import { DoctorLogin } from './login/doctor-login/doctor-login';
import { DoctorSignup } from './login/doctor-signup/doctor-signup';
import { PatientSignup } from './login/patient-signup/patient-signup';
import { Phome } from './patientComponent/home/home';
import { DoctorHome } from './doctorComponent/home/home';
import { Appointment } from './patientComponent/appointment/appointment';
import { Dashboard } from './doctorComponent/dashboard/dashboard';
import { Schedule } from './doctorComponent/schedule/schedule';
import { authGuard } from './guards/auth.guard';
import { Unauthorized } from './unauthorized/unauthorized';
import { DoctorNavbar } from './layout/doctor-navbar/doctor-navbar';
import { PatientNavbar } from './layout/patient-navbar/patient-navbar';
import { AuthNavbar } from './layout/auth-navbar/auth-navbar';
import { Scheduled } from './patientComponent/scheduled/scheduled';
import { PatientPrescriptionComponent } from './patientComponent/prescription/prescription';
import { Medications } from './doctorComponent/medications/medications';
import { Chat } from './shareComponent/chat/chat';

export const routes: Routes = [
  // 🔐 Auth routes (NO NAVBAR)
  {
    path: '',
    component: AuthNavbar,
    children: [
      { path: '', component: PatientSignup },
      { path: 'patient-login', component: PatientLogin, canActivate: [authGuard] },
      { path: 'doctor-login', component: DoctorLogin, canActivate: [authGuard] },
      { path: 'doctor-signup', component: DoctorSignup },
      { path: 'unauthorized', component: Unauthorized },
    ]
  },

  // 🧑‍⚕️ Patient routes (PATIENT NAVBAR)
  {
    path: 'patient',
    component: PatientNavbar,
    canActivateChild: [authGuard],
    data: { role: 'Patient' },
    children: [
      { path: 'home', component: Phome },
      {
        path: ':doctorId/appointment',
        component: Appointment
      },
      { path: 'scheduled', component: Scheduled },
      { path: 'prescription', component: PatientPrescriptionComponent },
      { path: 'chat', component: Chat}
    ]
  },

  // 👨‍⚕️ Doctor routes (DOCTOR NAVBAR)
  {
    path: 'doctor',
    component: DoctorNavbar,
    canActivateChild: [authGuard],
    data: { role: 'Doctor' },
    children: [
      { path: 'home', component: DoctorHome },
      { path: 'dashboard', component: Dashboard },
      { path: 'schedule', component: Schedule },
      { path: 'medications', component: Medications },
      { path: 'chat', component: Chat}
    ]
  },

  // fallback
  { path: '**', component: Unauthorized }
];