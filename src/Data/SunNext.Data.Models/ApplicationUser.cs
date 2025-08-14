// ReSharper disable VirtualMemberCallInConstructor
namespace SunNext.Data.Models
{
    using System;
    using System.Collections.Generic;

    using SunNext.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.SolarSystems = new HashSet<SolarSystem>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public string WalletId { get; set; }
        public virtual VirtualWallet Wallet { get; set; } = null!;

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
        public virtual ICollection<SolarSystem> SolarSystems { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
    }
}
