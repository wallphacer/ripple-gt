using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

public class EventManagementDbContextFactory : IDesignTimeDbContextFactory<EventManagementDbContext>
{
    public EventManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventManagementDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=eventmanagement;Username=test;Password=test");

        return new EventManagementDbContext(optionsBuilder.Options);
    }
}