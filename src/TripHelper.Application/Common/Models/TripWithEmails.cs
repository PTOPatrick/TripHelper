using TripHelper.Domain.Trips;

namespace TripHelper.Application.Common.Models;

public class TripWithEmails(int id, string name, DateTime? startDate, DateTime? endDate, string? description, string? location, string? imageUrl, int creatorUserId, List<string> emails)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public DateTime? StartDate { get; } = startDate;
    public DateTime? EndDate { get; } = endDate;
    public string? Description { get; } = description;
    public string? Location { get; } = location;
    public string? ImageUrl { get; } = imageUrl;
    public int CreatorUserId { get; } = creatorUserId;
    public List<string> Emails { get; } = emails;

    public static TripWithEmails FromTrip(Trip trip, List<string> emails)
    {
        return new TripWithEmails(trip.Id, trip.Name, trip.StartDate, trip.EndDate, trip.Description, trip.Location, trip.ImageUrl, trip.CreatorUserId, emails);
    }
}