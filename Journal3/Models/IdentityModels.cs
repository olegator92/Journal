﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Journal3.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Records = new HashSet<Record>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual UserInfo UserInfo { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("JournalCS")
        {
            Configuration.ProxyCreationEnabled = false;
        }

        //public DbSet<UserInfo> UserInfoes { get; set; }
        public DbSet<Record> Records { get; set; }
        //public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                        .HasOptional(s => s.UserInfo)
                        .WithRequired(ad => ad.User)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserInfo>()
                        .HasOptional(s => s.WorkSchedule) 
                        .WithRequired(ad => ad.UserInfo)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<Record>()
                    .HasRequired<ApplicationUser>(s => s.User)
                    .WithMany(s => s.Records)
                    .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}