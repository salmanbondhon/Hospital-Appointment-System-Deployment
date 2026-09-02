using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Enums;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;
using HospitalAPI.Data;

namespace HospitalAPI.Services
{
    public class DoctorLeaveService : IDoctorLeaveService
    {
        private readonly IDoctorLeaveRepository _leaveRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DoctorLeaveService(
    IDoctorLeaveRepository leaveRepository,
    IDoctorRepository doctorRepository,
    INotificationService notificationService,
    IMapper mapper,
    ApplicationDbContext context)
        {
            _leaveRepository = leaveRepository;
            _doctorRepository = doctorRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<DoctorLeaveDto>> GetAllAsync()
        {
            var leaves = await _leaveRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<DoctorLeaveDto>>(leaves);
        }

        public async Task<IEnumerable<DoctorLeaveDto>> GetMyLeavesAsync(int userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
                throw new BusinessException("Doctor profile not found.");

            var leaves = await _leaveRepository.GetByDoctorIdAsync(doctor.Id);

            return _mapper.Map<IEnumerable<DoctorLeaveDto>>(leaves);
        }

        public async Task<DoctorLeaveDto> CreateLeaveAsync(
    CreateLeaveDto dto,
    int userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
                throw new BusinessException("Doctor profile not found.");

            if (dto.StartDate.Date < DateTime.Today)
                throw new BusinessException("Leave cannot start in the past.");

            if (dto.EndDate.Date < dto.StartDate.Date)
                throw new BusinessException("End date must be after start date.");


            bool hasOverlap = await _leaveRepository
    .HasOverlappingLeaveAsync(
        doctor.Id,
        dto.StartDate,
        dto.EndDate);

            if (hasOverlap)
            {
                throw new BusinessException(
                    "Leave overlaps with an existing leave request.");
            }
            var leave = _mapper.Map<DoctorLeave>(dto);

            leave.DoctorId = doctor.Id;

            // Waiting for admin approval
            leave.IsApproved = false;

            await _leaveRepository.AddAsync(leave);
            await _leaveRepository.SaveChangesAsync();

            var adminUsers = await _context.Users
    .Where(u => u.Role == UserRole.Admin)
    .ToListAsync();

            foreach (var admin in adminUsers)
            {
                await _notificationService.CreateNotificationAsync(
                    admin.Id,
                    "Doctor Leave Request",
                    $"Dr. {doctor.FullName} has requested leave from " +
                    $"{leave.StartDate:dd MMM yyyy} to " +
                    $"{leave.EndDate:dd MMM yyyy}."
                );
            }

            leave = await _leaveRepository.GetByIdAsync(leave.Id);

            return _mapper.Map<DoctorLeaveDto>(leave);
        }

        public async Task ApproveLeaveAsync(int leaveId)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);

            if (leave == null)
                throw new BusinessException("Leave not found.");

            if (leave.IsApproved)
                throw new BusinessException("Leave already approved.");

            leave.IsApproved = true;

            await _leaveRepository.UpdateAsync(leave);
            await _leaveRepository.SaveChangesAsync();

            // Notify Doctor
            await _notificationService.CreateNotificationAsync(
                leave.Doctor.UserId,
                "Leave Approved",
                $"Your leave request from {leave.StartDate:dd MMM yyyy} " +
                $"to {leave.EndDate:dd MMM yyyy} has been approved."
            );
        }


        public async Task DeleteLeaveAsync(
    int leaveId,
    int userId,
    string role)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);

            if (leave == null)
                throw new BusinessException("Leave not found.");

            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (leave.DoctorId != doctor.Id)
                    throw new BusinessException("You are not authorized.");
            }

            await _leaveRepository.DeleteAsync(leave);
            await _leaveRepository.SaveChangesAsync();
        }


    }
}