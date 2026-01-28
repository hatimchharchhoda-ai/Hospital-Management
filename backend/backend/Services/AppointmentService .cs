using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointmentAsync(int doctorId, int patientId, DateOnly date, TimeOnly startTime);
        Task<List<AppointmentListDto>> GetTodayAppointmentsForDoctorAsync(int doctorId);
        Task<List<AppointmentListDto>> GetTodayAppointmentsForPatientAsync(int patientId);
        Task<List<AppointmentListDto>> GetUpcomingAppointmentsForDoctorAsync(int doctorId);
        Task<List<AppointmentListDto>> GetUpcomingAppointmentsForPatientAsync(int patientId);
        Task UpdateAppointmentForPatientAsync(UpdateAppointmentRequest request);
        Task DoctorUpdateAppointmentAsync(DoctorUpdateAppointmentRequest request);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;

        public AppointmentService(IAppointmentRepository appointmentRepo)
        {
            _appointmentRepo = appointmentRepo;
        }

        public async Task<Appointment> CreateAppointmentAsync(int doctorId, int patientId, DateOnly date, TimeOnly startTime)
            {
                // 1️⃣ Check if patient already booked same doctor same day
                if (await _appointmentRepo.IsPatientAlreadyBookedAsync(doctorId, patientId, date))
                {
                    throw new Exception("You have already booked this doctor on the same day.");
                }

                // 2️⃣ Check if the slot has less than 6 patients
                int slotCount = await _appointmentRepo.GetSlotCountAsync(doctorId, date, startTime);
                if (slotCount >= 6)
                {
                    throw new Exception("This time slot is already full. Please choose another slot.");
                }

                // 3️⃣ Create appointment
                var appointment = new Appointment
                {
                    DoctorId = doctorId,
                    PatientId = patientId,
                    AppointmentDate = date,
                    StartTime = startTime,
                    Status = AppointmentStatus.Booked
                };

                await _appointmentRepo.AddAppointmentAsync(appointment);
                await _appointmentRepo.SaveChangesAsync();

                return appointment;
            }

        public async Task<List<AppointmentListDto>> GetTodayAppointmentsForDoctorAsync(int doctorId)
        {
            return await _appointmentRepo.GetTodayAppointmentsByDoctorAsync(doctorId);
        }

        public async Task<List<AppointmentListDto>> GetTodayAppointmentsForPatientAsync(int patientId)
        {
            return await _appointmentRepo.GetTodayAppointmentsByPatientAsync(patientId);
        }

        public async Task<List<AppointmentListDto>> GetUpcomingAppointmentsForDoctorAsync(int doctorId)
        {
            return await _appointmentRepo.GetTodayAndUpcomingByDoctorAsync(doctorId);
        }

        public async Task<List<AppointmentListDto>> GetUpcomingAppointmentsForPatientAsync(int patientId)
        {
            return await _appointmentRepo.GetTodayAndUpcomingByPatientAsync(patientId);
        }
        public async Task UpdateAppointmentForPatientAsync(UpdateAppointmentRequest request)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(request.AppointmentId);

            if (await _appointmentRepo.IsPatientAlreadyBookedExcludingAppointmentAsync(request.AppointmentId, request.DoctorId, request.PatientId, request.AppointmentDate))
            {
                throw new Exception("You have already booked this doctor on the same day.");
            }

            int slotCount = await _appointmentRepo.GetSlotCountExcludingAppointmentAsync(request.AppointmentId, request.DoctorId, request.AppointmentDate, request.StartTime);
            if (slotCount >= 6)
            {
                throw new Exception("This time slot is already full. Please choose another slot.");
            }

            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Booked)
                throw new Exception("Only booked appointments can be updated.");

            // ✅ Update fields
            appointment.AppointmentDate = request.AppointmentDate;
            appointment.StartTime = request.StartTime;

            await _appointmentRepo.UpdateAppointmentAsync(appointment);
            await _appointmentRepo.SaveChangesAsync();
        }
        public async Task DoctorUpdateAppointmentAsync(DoctorUpdateAppointmentRequest request)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(request.AppointmentId);

            if (await _appointmentRepo.IsPatientAlreadyBookedExcludingAppointmentAsync(request.AppointmentId, request.DoctorId, request.PatientId, request.AppointmentDate))
            {
                throw new Exception("You have already booked this doctor on the same day.");
            }

            int slotCount = await _appointmentRepo.GetSlotCountExcludingAppointmentAsync(request.AppointmentId, request.DoctorId, request.AppointmentDate, request.StartTime);
            if (slotCount >= 6)
            {
                throw new Exception("This time slot is already full. Please choose another slot.");
            }
            if (appointment == null)
                throw new Exception("Appointment not found or access denied.");

            if (appointment.Status != AppointmentStatus.Booked)
                throw new Exception("Only booked appointments can be updated.");

            if (request.Status != AppointmentStatus.Booked &&
                request.Status != AppointmentStatus.Completed &&
                request.Status != AppointmentStatus.Cancelled)
                throw new Exception("Invalid appointment status.");

            // ✅ Apply updates
            appointment.AppointmentDate = request.AppointmentDate;
            appointment.StartTime = request.StartTime;
            appointment.Status = request.Status;

            await _appointmentRepo.UpdateAppointmentAsync(appointment);
            await _appointmentRepo.SaveChangesAsync();
        }

    }
}
