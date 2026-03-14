using Domain.Events;

namespace Core.Application.Events;

public interface IEventsRepository
{
    Task<IList<Event>> GetAllEvents();

    Task<Event?> GetByIdAsync(Guid id);

    Task AddAsync(Event eventToAdd);

    Task<bool> DeleteAsync(Guid id);

    Task<Event?> UpdateAsync(Event eventToUpdate);
}
