using Microsoft.EntityFrameworkCore;

namespace CustomerService.EF
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext()
        {

        }

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Subscriber> Subscriber { get; set; }
        public virtual DbSet<SubscriptionFilter> SubscriptionFilter { get; set; }
        public virtual DbSet<CustomerAuthorization> CustomerAuthorization { get; set; }
        public virtual DbSet<ClientRateLimitPolicyRule> ClientRateLimitPolicyRule { get; set; }
        public virtual DbSet<RateLimitPolicy> RateLimitPolicy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscriber>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.SubscriptionId)
                    .IsRequired();

                entity.HasIndex(e => new { e.CustomerCode })
                    .HasDatabaseName("IX_Subscriber_MessageType");
            });

            modelBuilder.Entity<SubscriptionFilter>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.SubscriberId)
                    .IsRequired();

                entity.Property(e => e.Rules)
                    .IsRequired();

                entity.HasIndex(e => new { e.SubscriberId })
                    .HasDatabaseName("IX_SubscriberFilter_SubscriptionId");
            });

            modelBuilder.Entity<CustomerAuthorization>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsRequired();

                entity.HasMany(e => e.ClientRateLimitPolicyRules)
                      .WithOne(e => e.CustomerAuthorization);

                entity.HasIndex(e => new { e.ClientId })
                    .HasDatabaseName("IX_CustomerAuthorization_ClientId")
                    .IsUnique();
            });

            modelBuilder.Entity<RateLimitPolicy>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.HasMany(e => e.ClientRateLimitPolicyRules)
                      .WithOne(e => e.RateLimitPolicy);
            });

            modelBuilder.Entity<ClientRateLimitPolicyRule>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.CustomerAuthorizationId })
                    .HasDatabaseName("IX_ClientRateLimitPolicyRule_CustomerAuthorizationId");

                entity.HasIndex(e => new { e.RateLimitPolicyId })
                 .HasDatabaseName("[IX_ClientRateLimitPolicyRule_RateLimitPolicyId]");
            });
        }
    }
}