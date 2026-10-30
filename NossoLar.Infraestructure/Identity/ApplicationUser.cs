using Microsoft.AspNetCore.Identity;

namespace NossoLar.Infraestructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
