using Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class EventManagementDbContext : DbContext
{
    public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventManagementDbContext).Assembly);
    }
}