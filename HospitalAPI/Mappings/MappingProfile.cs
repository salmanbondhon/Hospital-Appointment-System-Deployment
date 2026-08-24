using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Models;

namespace HospitalAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Department
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            // Doctor
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department!.Name));

            CreateMap<CreateDoctorDto, Doctor>();

            CreateMap<UpdateDoctorDto, Doctor>();
            CreateMap<Doctor, DoctorDto>();


            // Patient
            CreateMap<Patient, PatientDto>();

            CreateMap<CreatePatientDto, Patient>();

            CreateMap<UpdatePatientDto, Patient>();


            // Appointment
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty));


            // Prescription
            CreateMap<CreatePrescriptionDto, Prescription>();

            CreateMap<Prescription, PrescriptionDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Appointment!.Doctor!.FullName))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Appointment!.Patient!.FullName));


            //Payment
            CreateMap<CreatePaymentDto, Payment>();

            CreateMap<UpdatePaymentDto, Payment>();

            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Appointment!.Patient!.FullName))
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Appointment!.Doctor!.FullName));


            CreateMap<CreateAppointmentDto, Appointment>();

            CreateMap<UpdateAppointmentDto, Appointment>();

            CreateMap<DoctorLeave, DoctorLeaveDto>()
    .ForMember(dest => dest.DoctorName,
        opt => opt.MapFrom(src => src.Doctor.FullName));

            CreateMap<CreateLeaveDto, DoctorLeave>();
        }
    }
}
