import { HttpClient } from '@angular/common/http';
import { Inject, inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PatientService {
  private http = inject(HttpClient);

  // 🔹 inject API URL from provider
  constructor(@Inject('API_URL') private apiUrl: string) { }

  private get baseUrl() {
    return `${this.apiUrl}/Patient`;
  }

  // Check if mobile already exists
  checkMobile(mobile: string) {
    return this.http.get<{ exists: boolean }>(`${this.baseUrl}/check-mobile/${mobile}`);
  }

  // Get doctors associated with a patient
  getDoctorsByPatient(id: number) {
    return this.http.get<any[]>(`${this.baseUrl}/${id}/doctors`);
  }

  // Add a doctor to a patient
  addDoctor(data: any) {
    return this.http.post(`${this.baseUrl}/add-doctor`, data);
  }

  // Get today's appointments for a patient
  getTodayAppointments(id: number) {
    return this.http.get<any[]>(`${this.baseUrl}/${id}/today`);
  }

  // Book an appointment
  bookAppointment(data: {
    doctorId: number;
    patientId: number;
    appointmentDate: string;
    startTime: string;
  }) {
    return this.http.post(`${this.baseUrl}/bookAppointment`, data);
  }

  // Get upcoming appointments for a patient
  getUpcomingAppointments(patientId: number) {
    return this.http.get<any[]>(
      `${this.baseUrl}/${patientId}/appointments/upcoming`
    );
  }

  // Update an appointment
  updateAppointment(payload: any) {
    return this.http.put(
      `${this.baseUrl}/appointments/update`,
      payload
    );
  }
}