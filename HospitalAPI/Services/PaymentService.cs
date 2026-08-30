using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Enums;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        private readonly IAppointmentRepository
            _appointmentRepository;

        private readonly IPatientRepository
            _patientRepository;

        private readonly IMapper _mapper;


        public PaymentService(
            IPaymentRepository paymentRepository,
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;

            _appointmentRepository =
                appointmentRepository;

            _patientRepository =
                patientRepository;

            _mapper = mapper;
        }


        // =================================================
        // GET ALL PAYMENTS
        // =================================================

        public async Task<IEnumerable<PaymentDto>> GetAllAsync(
            int userId,
            string role)
        {
            var allPayments =
                await _paymentRepository.GetAllAsync();


            // =================================================
            // ADMIN
            // =================================================

            if (role == "Admin")
            {
                return _mapper.Map<
                    IEnumerable<PaymentDto>>(
                    allPayments);
            }


            // =================================================
            // PATIENT
            // =================================================

            var patient =
                await _patientRepository
                    .GetByUserIdAsync(userId);


            if (patient == null)
            {
                throw new BusinessException(
                    "Patient profile not found.");
            }


            var myPayments =
                allPayments.Where(
                    p =>
                        p.Appointment != null &&
                        p.Appointment.PatientId ==
                        patient.Id);


            return _mapper.Map<
                IEnumerable<PaymentDto>>(
                myPayments);
        }


        // =================================================
        // GET PAYMENT BY ID
        // =================================================

        public async Task<PaymentDto?> GetByIdAsync(
            int id,
            int userId,
            string role)
        {
            var payment =
                await _paymentRepository
                    .GetByIdAsync(id);


            if (payment == null)
            {
                return null;
            }


            // =================================================
            // ADMIN
            // =================================================

            if (role == "Admin")
            {
                return _mapper.Map<PaymentDto>(
                    payment);
            }


            // =================================================
            // PATIENT
            // =================================================

            var patient =
                await _patientRepository
                    .GetByUserIdAsync(userId);


            if (patient == null)
            {
                throw new BusinessException(
                    "Patient profile not found.");
            }


            if (
                payment.Appointment == null ||
                payment.Appointment.PatientId !=
                patient.Id)
            {
                throw new BusinessException(
                    "You are not authorized.");
            }


            return _mapper.Map<PaymentDto>(
                payment);
        }


        // =================================================
        // CREATE PAYMENT
        // =================================================
        //
        // Patient sends:
        //
        // AppointmentId
        // PaymentMethod
        //
        // Amount is calculated automatically
        // from Doctor.ConsultationFee
        // =================================================

        public async Task<PaymentDto> CreateAsync(
     CreatePaymentDto dto,
     int userId)
        {
            // =================================================
            // GET CURRENT PATIENT
            // =================================================

            var patient =
                await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException(
                    "Patient profile not found.");


            // =================================================
            // GET APPOINTMENT
            // =================================================

            var appointment =
                await _appointmentRepository
                    .GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
                throw new BusinessException(
                    "Appointment not found.");


            // =================================================
            // CHECK PATIENT OWNS APPOINTMENT
            // =================================================

            if (appointment.PatientId != patient.Id)
                throw new BusinessException(
                    "You are not allowed to pay for this appointment.");


            // =================================================
            // CHECK DOCTOR
            // =================================================

            if (appointment.Doctor == null)
                throw new BusinessException(
                    "Doctor information not found.");


            // =================================================
            // CHECK CANCELLED APPOINTMENT
            // =================================================

            if (appointment.Status == AppointmentStatus.Cancelled)
                throw new BusinessException(
                    "Cancelled appointments cannot be paid.");


            // =================================================
            // CHECK EXISTING PAYMENT
            // =================================================

            var existingPayment =
                await _paymentRepository
                    .GetByAppointmentIdAsync(dto.AppointmentId);

            if (existingPayment != null)
                throw new BusinessException(
                    "Payment already exists for this appointment.");


            // =================================================
            // AUTOMATICALLY CALCULATE PAYMENT
            // =================================================

            decimal amount =
                appointment.Doctor.ConsultationFee;


            if (amount <= 0)
                throw new BusinessException(
                    "Doctor consultation fee is not configured.");


            // =================================================
            // CREATE PAYMENT
            // =================================================

            var payment = new Payment
            {
                AppointmentId = appointment.Id,

                Amount = amount,

                PaymentMethod =
                    dto.PaymentMethod,

                Status =
                    PaymentStatus.Paid,

                TransactionId =
                    Guid.NewGuid()
                        .ToString("N")
                        .ToUpper(),

                PaymentDate =
                    DateTime.UtcNow
            };


            // =================================================
            // SAVE
            // =================================================

            await _paymentRepository.AddAsync(payment);

            await _paymentRepository.SaveChangesAsync();


            // =================================================
            // GET SAVED PAYMENT
            // =================================================

            payment =
                await _paymentRepository
                    .GetByIdAsync(payment.Id);


            return _mapper.Map<PaymentDto>(payment);
        }

        // =================================================
        // UPDATE PAYMENT
        // =================================================

        public async Task UpdateAsync(
            int id,
            UpdatePaymentDto dto)
        {
            var payment =
                await _paymentRepository
                    .GetByIdAsync(id);


            if (payment == null)
            {
                throw new BusinessException(
                    "Payment not found.");
            }


            payment.Amount =
                dto.Amount;

            payment.PaymentMethod =
                dto.PaymentMethod;

            payment.Status =
                dto.Status;


            await _paymentRepository
                .UpdateAsync(payment);

            await _paymentRepository
                .SaveChangesAsync();
        }


        // =================================================
        // DELETE PAYMENT
        // =================================================

        public async Task DeleteAsync(
            int id)
        {
            var payment =
                await _paymentRepository
                    .GetByIdAsync(id);


            if (payment == null)
            {
                throw new BusinessException(
                    "Payment not found.");
            }


            await _paymentRepository
                .DeleteAsync(payment);

            await _paymentRepository
                .SaveChangesAsync();
        }
    }
}