export interface PrescriptionModel {
  prescriptionId: number;

  doctorId: number;
  patientId: number;

  description: string;
  imageUrl: string;
  imagePublicId: string;

  doctor?: {
    doctorId: number;
    fullName: string;
    email: string;
    mobile: string;
    specialization: string;
  };

  patient?: {
    patientId: number;
    name: string;
    mobile: string;
  } | null;
}