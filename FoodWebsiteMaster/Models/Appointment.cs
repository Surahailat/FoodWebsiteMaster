using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Appointment
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string DoctorName { get; set; } = null!;

    public DateOnly AppointmentDate { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? DoctorPosition { get; set; }

    public string? Message { get; set; }

    public string Email { get; set; } = null!;
}
