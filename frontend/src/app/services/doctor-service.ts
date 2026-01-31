import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { PrescriptionModel } from '../models/prescriptionModel';
import { PatientPrescription } from '../models/patientPrescription';

@Injectable({
  providedIn: 'root',
})
export class DoctorService {
  private http = inject(HttpClient);

  // 🔹 inject API URL from provider
  constructor(@Inject('API_URL') private apiUrl: string) { }

  private get baseUrl() {
    return `${this.apiUrl}/Doctor`;
  }

  // 🔹 Get patients mapped to doctor
  getPatientsByDoctor(doctorId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.baseUrl}/${doctorId}/patients`
    );
  }

  // 🔹 Get today's appointments (Doctor Home)
  getTodayAppointments(doctorId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.baseUrl}/${doctorId}/today`
    );
  }

  // 🔹 Get upcoming appointments
  getUpcomingAppointments(doctorId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.baseUrl}/${doctorId}/appointments/upcoming`
    );
  }

  // 🔹 Update appointment (status etc.)
  doctorUpdateAppointment(payload: any) {
    return this.http.put(
      `${this.baseUrl}/appointments/update`,
      payload
    );
  }

  // 🔹 Get prescriptions by patient
  getPrescriptionsByPatient(doctorId: number, patientId: number): Observable<PatientPrescription[]> {
    return this.http.get<PatientPrescription[]>(
      `${this.baseUrl}/${doctorId}/prescriptions/${patientId}`
    );
  }

  // 🔹 Create new prescription
  createPrescription(formData: FormData): Observable<PatientPrescription> {
    return this.http.post<PatientPrescription>(
      `${this.baseUrl}/createPrescription`,
      formData
    );
  }

  // 🔹 Get all appointments for doctor (Dashboard - History)
  getAllAppointmentsByDoctor(doctorId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.baseUrl}/${doctorId}/appointments/all`
    );
  }
}
