import { Routes } from '@angular/router';
import { PatientLogin } from './login/patient-login/patient-login';
import { DoctorLogin } from './login/doctor-login/doctor-login';
import { DoctorSignup } from './login/doctor-signup/doctor-signup';
import { PatientSignup } from './login/patient-signup/patient-signup';
import { Phome } from './patientComponent/home/home';
import { Dhome } from './doctorComponent/home/home';
import { Appointment } from './patientComponent/appointment/appointment';
import { History } from './doctorComponent/history/history';
import { Schedule } from './doctorComponent/schedule/schedule';
import { authGuard } from './guards/auth.guard';
import { Unauthorized } from './unauthorized/unauthorized';
import { DoctorNavbar } from './layout/doctor-navbar/doctor-navbar';
import { PatientNavbar } from './layout/patient-navbar/patient-navbar';
import { AuthNavbar } from './layout/auth-navbar/auth-navbar';
import { Scheduled } from './patientComponent/scheduled/scheduled';

export const routes: Routes = [
  // 🔐 Auth routes (NO NAVBAR)
  {
    path: '',
    component: AuthNavbar,
    children: [
      { path: '', component: PatientSignup },
      { path: 'patient-login', component: PatientLogin },
      { path: 'doctor-login', component: DoctorLogin },
      { path: 'doctor-signup', component: DoctorSignup },
      { path: 'unauthorized', component: Unauthorized },
    ]
  },

  // 🧑‍⚕️ Patient routes (PATIENT NAVBAR)
  {
    path: 'patient',
    component: PatientNavbar,
    // canActivate: [authGuard],
    data: { role: 'Patient' },
    children: [
      { path: 'home', component: Phome },
      {
        path: ':doctorId/appointment',
        component: Appointment
      },
      { path: 'scheduled', component: Scheduled }
    ]
  },

  // 👨‍⚕️ Doctor routes (DOCTOR NAVBAR)
  {
    path: 'doctor',
    component: DoctorNavbar,
    // canActivate: [authGuard],
    data: { role: 'Doctor' },
    children: [
      { path: 'home', component: Dhome },
      { path: 'history', component: History },
      { path: 'schedule', component: Schedule },
    ]
  },

  // fallback
  { path: '**', component: Unauthorized }
];