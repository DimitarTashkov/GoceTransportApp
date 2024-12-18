﻿// ReSharper disable VirtualMemberCallInConstructor
namespace GoceTransportApp.Data.Models
{
    using GoceTransportApp.Data.Common.Models;
    using Microsoft.AspNetCore.Identity;
    using System;

    public class ApplicationRole : IdentityRole<string>, IAuditInfo, IDeletableEntity
    {
        public ApplicationRole()
            : this(null)
        {
        }

        public ApplicationRole(string name)
            : base(name)
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
