public class UpdateDoctorDto
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; }

    public string Specialization { get; set; } = string.Empty;

    public string Qualification { get; set; } = string.Empty;

    public int Experience { get; set; }

    public decimal ConsultationFee { get; set; }

    public string AvailableFrom { get; set; } = string.Empty;

    public string AvailableTo { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
}