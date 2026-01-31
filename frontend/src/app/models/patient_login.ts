export interface Patient {
  patientId: number;
  name: string;
  mobile: string;
  password?: string; // optional, not used in UI
}