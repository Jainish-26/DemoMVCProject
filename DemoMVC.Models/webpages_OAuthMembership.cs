using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public partial class webpages_OAuthMembership
    {
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        [Key]
        public int UserId { get; set; }
    }
}
