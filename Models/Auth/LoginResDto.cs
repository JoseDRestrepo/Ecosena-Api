namespace EcoSENA.Api.Models.Auth
{
    public class LoginResDto
    {
        public string? JWT { get; set; }
        public DateTime? LockedUntil { get; set; }
        public bool IsLocked { get; set; } = false;
        public int? RemainingAttempts { get; set; }
        public string? Message { get; set; }
    }
}
