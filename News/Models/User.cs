using Microsoft.AspNetCore.Identity;
using News.DbModels;

namespace News.Models
{
    public class User : IdentityUser<int>
    {
        public virtual DbImageFile? Logo { get; set; }
        public string Name { get; set; }
    }
}
