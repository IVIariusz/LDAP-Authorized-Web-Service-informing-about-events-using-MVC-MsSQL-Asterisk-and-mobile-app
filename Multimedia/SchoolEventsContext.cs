using Microsoft.EntityFrameworkCore;
using Multimedia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


        base.OnModelCreating(modelBuilder);
    }
}