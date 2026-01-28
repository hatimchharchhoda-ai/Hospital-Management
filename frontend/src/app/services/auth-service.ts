import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private http: HttpClient,
    @Inject('API_URL') private apiUrl: string
  ) { }

  // 🔐 Patient
  loginPatient(data: { mobile: string; password: string }) {
    return this.http.post<{ token: string }>(
      `${this.apiUrl}/Auth/patient/login`,
      data
    );
  }

  registerPatient(data: { name: string; mobile: string; password: string }) {
    return this.http.post(
      `${this.apiUrl}/Auth/patient/register`,
      data
    );
  }

  // 🔐 Doctor (future)
  loginDoctor(data: any) {
    return this.http.post(
      `${this.apiUrl}/Auth/doctor/login`,
      data
    );
  }

  registerDoctor(data: any) {
    return this.http.post(
      `${this.apiUrl}/Auth/doctor/register`,
      data
    );
  }
}
