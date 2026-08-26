using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Models;

namespace HospitalAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // =================================================
            // DEPARTMENT
            // =================================================

            CreateMap<Department, DepartmentDto>();

            CreateMap<CreateDepartmentDto, Department>();

            CreateMap<UpdateDepartmentDto, Department>();


            // =================================================
            // DOCTOR
            // =================================================

            CreateMap<Doctor, DoctorDto>()

                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(
                        src => src.User != null
                            ? src.User.Email
                            : string.Empty))

                .ForMember(
                    dest => dest.DepartmentName,
                    opt => opt.MapFrom(
                        src => src.Department != null
                            ? src.Department.Name
                            : string.Empty));


            CreateMap<CreateDoctorDto, Doctor>();

            // =================================================
            // PATIENT
            // =================================================

            CreateMap<Patient, PatientDto>()

                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(
                        src => src.User != null
                            ? src.User.Email
                            : string.Empty));


            CreateMap<CreatePatientDto, Patient>();


            CreateMap<UpdatePatientDto, Patient>();


            // =================================================
            // APPOINTMENT
            // =================================================

            CreateMap<Appointment, AppointmentDto>()

                .ForMember(
                    dest => dest.DoctorName,
                    opt => opt.MapFrom(
                        src => src.Doctor != null
                            ? src.Doctor.FullName
                            : string.Empty))

                .ForMember(
                    dest => dest.PatientName,
                    opt => opt.MapFrom(
                        src => src.Patient != null
                            ? src.Patient.FullName
                            : string.Empty));


            CreateMap<CreateAppointmentDto, Appointment>();

            CreateMap<UpdateAppointmentDto, Appointment>();


            // =================================================
            // PRESCRIPTION
            // =================================================

            CreateMap<CreatePrescriptionDto, Prescription>();

            CreateMap<Prescription, PrescriptionDto>()

                .ForMember(
                    dest => dest.DoctorName,
                    opt => opt.MapFrom(
                        src => src.Appointment!.Doctor!.FullName))

                .ForMember(
                    dest => dest.PatientName,
                    opt => opt.MapFrom(
                        src => src.Appointment!.Patient!.FullName));


            // =================================================
            // PAYMENT
            // =================================================

            CreateMap<CreatePaymentDto, Payment>();

            CreateMap<UpdatePaymentDto, Payment>();

            CreateMap<Payment, PaymentDto>()

                .ForMember(
                    dest => dest.PatientName,
                    opt => opt.MapFrom(
                        src => src.Appointment!.Patient!.FullName))

                .ForMember(
                    dest => dest.DoctorName,
                    opt => opt.MapFrom(
                        src => src.Appointment!.Doctor!.FullName));


            // =================================================
            // DOCTOR LEAVE
            // =================================================

            CreateMap<DoctorLeave, DoctorLeaveDto>()

                .ForMember(
                    dest => dest.DoctorName,
                    opt => opt.MapFrom(
                        src => src.Doctor.FullName));


            CreateMap<CreateLeaveDto, DoctorLeave>();
        }
    }
}