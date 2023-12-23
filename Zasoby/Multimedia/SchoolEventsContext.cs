using Microsoft.EntityFrameworkCore;
using Multimedia.Models;

public class SchoolEventsContext : DbContext
{
    public SchoolEventsContext(DbContextOptions<SchoolEventsContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }
    public DbSet<SentMessagesHistory> SentMessagesHistory { get; set; }

    public DbSet<EventUser> EventUser { get; set; }
    public ICollection<EventUser> SelectedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SentMessagesHistory>(entity =>
        {
            entity.HasKey(e => e.MessageID);

            entity.HasOne(e => e.User)
                .WithMany(u => u.SentMessagesHistory)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventUser>()
            .HasKey(eu => new { eu.EventID, eu.UserID });

        modelBuilder.Entity<EventUser>()
            .HasOne(eu => eu.Event)
            .WithMany(e => e.SelectedUsers)
            .HasForeignKey(eu => eu.EventID);

        modelBuilder.Entity<EventUser>()
            .HasOne(eu => eu.User)
            .WithMany(u => u.SelectedEvents)
            .HasForeignKey(eu => eu.UserID);


        base.OnModelCreating(modelBuilder);
    }

    public IQueryable<Event> GetEventsWithParticipants()
    {
        return Events
            .Include(e => e.SelectedUsers)
                .ThenInclude(ga => ga.User)
            .AsQueryable();
    }

}
