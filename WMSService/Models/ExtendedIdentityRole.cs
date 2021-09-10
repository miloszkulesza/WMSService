using Microsoft.AspNetCore.Identity;

namespace WMSService.Models
{
    public class ExtendedIdentityRole : IdentityRole<string>
    {
        public string Description { get; set; }
    }
}
