using backend.DTOs;
using backend.Models;

namespace backend.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<int> GetSlotCountAsync(int doctorId, DateOnly date, TimeOnly startTime);
        Task<bool> IsPatientAlreadyBookedAsync(int doctorId, int patientId, DateOnly date);
        Task AddAppointmentAsync(Appointment appointment);
        Task SaveChangesAsync();
        Task<List<AppointmentListDto>> GetTodayAppointmentsByDoctorAsync(int doctorId);
        Task<List<AppointmentListDto>> GetTodayAppointmentsByPatientAsync(int patientId);
        Task<List<AppointmentListDto>> GetTodayAndUpcomingByDoctorAsync(int doctorId);
        Task<List<AppointmentListDto>> GetTodayAndUpcomingByPatientAsync(int patientId);
        Task<Appointment?> GetByIdAsync(int appointmentId);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task<bool> IsPatientAlreadyBookedExcludingAppointmentAsync(int appointmentId,int doctorId,int patientId,DateOnly appointmentDate);
        Task<int> GetSlotCountExcludingAppointmentAsync(int appointmentId,int doctorId,DateOnly appointmentDate,TimeOnly startTime);
    }
}
