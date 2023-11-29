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
}
