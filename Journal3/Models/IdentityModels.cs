using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;

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

        public DbSet<UserInfo> UserInfoes { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<SpecialSchedule> SpecialSchedules { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Holiday> Holidays { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserInfo>()
                .HasKey(e => e.UserId);

            modelBuilder.Entity<ApplicationUser>()
                        .HasOptional(s => s.UserInfo)
                        .WithRequired(ad => ad.User)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserInfo>()
                        .HasOptional<WorkSchedule>(s => s.WorkSchedule) 
                        .WithMany(s => s.UserInfos)
                        .HasForeignKey(x => x.WorkScheduleId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Record>()
                        .HasRequired<WorkSchedule>(s => s.WorkSchedule)
                        .WithMany(s => s.Records)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<Record>()
                    .HasRequired<ApplicationUser>(s => s.User)
                    .WithMany(s => s.Records)
                    .WillCascadeOnDelete(true);

            modelBuilder.Entity<SpecialSchedule>()
                    .HasRequired<WorkSchedule>(s => s.WorkSchedule)
                    .WithMany(s => s.SpecialSchedules); 

            base.OnModelCreating(modelBuilder);
        }
    }
}