export interface Doctor {
  doctorId: number;        // PK later
  fullName: string;
  email: string;
  mobile: string;
  specialization: string;
  password: string;        // later -> hashed
  isActive: boolean;
  createdAt: Date;
}

export interface DoctorRegisterDto {
  fullName: string;
  email: string;
  mobile: string;
  specialization: string;
  password: string;
}

export interface LoginDoctorDto {
  identifier: string; // email OR mobile
  password: string;
}
