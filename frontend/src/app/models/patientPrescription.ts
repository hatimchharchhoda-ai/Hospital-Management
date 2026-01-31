import { Patient } from "./patient_login";

export interface PatientPrescription {
  prescriptionId: number;
  doctorId: number;
  patientId: number;
  patient?: Patient | null; // comes in API
  description: string;
  imageUrl: string;
  imagePublicId: string;
}