export interface Patient {
  patientId: number;       // DB primary key later
  doctorId: number;         // FK -> Doctor.doctorId
  name: string;
  mobile: string;
  isActive: boolean;
  createdAt: Date;
  otpCode: string;
  isUsed: boolean;
}
