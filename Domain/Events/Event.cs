using System;
using Domain.Shared;

namespace Domain.Events;

public class Event
{
    public Guid Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public int TotalCapacity { get; set; }

    private IList<PricingTier> _pricingTiers = new List<PricingTier>();

    public ICollection<PricingTier> PricingTiers => _pricingTiers.AsReadOnly();

    public static Event Create(string name, string description, string venue, DateTime eventDate, int totalCapacity)
    {
        return new Event
        {
            Name = name,
            Description = description,
            Venue = venue,
            EventDate = eventDate,
            TotalCapacity = totalCapacity
        };
    }

    public Result<PricingTier> AddPricingTier(string name, int costInCents, int capacity)
    {
        var currentTotal = _pricingTiers.Sum(tier => tier.Capacity);

        if (currentTotal + capacity <= TotalCapacity)
        {
            var pricingTier = PricingTier.Create(name, costInCents, capacity, this);

            _pricingTiers.Add(pricingTier);
            return Result<PricingTier>.Success(pricingTier);
        }

        return Result<PricingTier>.Fail(
            $"This event has Total Capacity of {TotalCapacity}. New Tier with name {name} makes Total Capacity = {currentTotal + capacity}");
    }
}