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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync(int userId, string role)
        {
            var allPayments = await _paymentRepository.GetAllAsync();

            if (role == "Admin")
            {
                return _mapper.Map<IEnumerable<PaymentDto>>(allPayments);
            }

            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient profile not found.");

            var myPayments = allPayments
                .Where(p => p.Appointment != null &&
                            p.Appointment.PatientId == patient.Id);

            return _mapper.Map<IEnumerable<PaymentDto>>(myPayments);
        }

        public async Task<PaymentDto?> GetByIdAsync(
            int id,
            int userId,
            string role)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
                return null;

            if (role == "Admin")
                return _mapper.Map<PaymentDto>(payment);

            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient profile not found.");

            if (payment.Appointment == null ||
                payment.Appointment.PatientId != patient.Id)
            {
                throw new BusinessException("You are not authorized.");
            }

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> CreateAsync(
            CreatePaymentDto dto,
            int userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient profile not found.");

            var appointment =
                await _appointmentRepository.GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
                throw new BusinessException("Appointment not found.");

            if (appointment.PatientId != patient.Id)
                throw new BusinessException("You are not allowed to pay for this appointment.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                throw new BusinessException("Cancelled appointments cannot be paid.");

            var existingPayment =
                await _paymentRepository.GetByAppointmentIdAsync(dto.AppointmentId);

            if (existingPayment != null)
                throw new BusinessException("Payment already exists.");

            var payment = _mapper.Map<Payment>(dto);

            payment.Status = PaymentStatus.Paid;
            payment.TransactionId = Guid.NewGuid().ToString("N").ToUpper();
            payment.PaymentDate = DateTime.UtcNow;

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            payment = await _paymentRepository.GetByIdAsync(payment.Id);

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task UpdateAsync(
            int id,
            UpdatePaymentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
                throw new BusinessException("Payment not found.");

            payment.Amount = dto.Amount;
            payment.PaymentMethod = dto.PaymentMethod;
            payment.Status = dto.Status;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
                throw new BusinessException("Payment not found.");

            await _paymentRepository.DeleteAsync(payment);
            await _paymentRepository.SaveChangesAsync();
        }
    }
}