using Microsoft.AspNetCore.Mvc;
using MediatR;
using TripHelper.Contracts.Trips;
using TripHelper.Application.Trips.Commands.CreateTrip;
using TripHelper.Application.Trips.Queries.GetTrip;
using TripHelper.Application.Trips.Queries.GetTrips;
using TripHelper.Application.Trips.Commands.UpdateTrip;
using TripHelper.Application.Trips.Commands.DeleteTrip;
using Microsoft.AspNetCore.Authorization;
using TripHelper.Application.TripItems.Commands.AddTripItem;
using TripHelper.Application.TripItems.Queries.GetTripItems;
using TripHelper.Application.TripItems.Queries.GetTripItem;
using TripHelper.Application.TripItems.Commands.UpdateTripItem;
using TripHelper.Application.TripItems.Commands.DeleteTripItem;
using TripHelper.Application.Members.Queries.GetTripMembers;
using TripHelper.Contracts.Members;

namespace TripHelper.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TripsController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateTrip(CreateTripRequest request)
    {
        var command = new CreateTripCommand(request.Name, request.StartDate, request.EndDate, request.Description, request.Location, request.ImageUrl);
        
        var result = await _sender.Send(command);

        return result.Match(
            trip => Ok(new TripResponse(trip.Id, trip.Name, trip.StartDate, trip.EndDate, trip.Description, trip.Location, trip.ImageUrl, trip.CreatorUserId)),
            Problem);
    }

    [HttpPost("{tripId:int}")]
    public async Task<IActionResult> CreateTripItem(int tripId, AddTripItemRequest request)
    {
        var command = new CreateTripItemCommand(request.Name, tripId, request.Amount, request.MemberId);

        var result = await _sender.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpGet("{tripId:int}")]
    public async Task<IActionResult> GetTrip(int tripId)
    {
        var query = new GetTripQuery(tripId);

        var result = await _sender.Send(query);

        return result.Match(
            trip => Ok(new TripDetailResponse(trip.Id, trip.Name, trip.StartDate, trip.EndDate, trip.Description, trip.Location, trip.ImageUrl, trip.CreatorUserId, trip.Emails)),
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        var query = new GetTripsQuery();

        var result = await _sender.Send(query);

        return result.Match(
            trips => Ok(trips.Select(trip => new TripResponse(trip.Id, trip.Name, trip.StartDate, trip.EndDate, trip.Description, trip.Location, trip.ImageUrl, trip.CreatorUserId)).ToList()),
            Problem);
    }

    [HttpGet("{tripId:int}/items")]
    public async Task<IActionResult> GetTripItems(int tripId)
    {
        var query = new GetTripItemsQuery(tripId);

        var result = await _sender.Send(query);

        return result.Match(
            items => Ok(items.Select(i => new TripItemResponse(i.Id, i.Name, i.Amount, i.MemberId, i.Email)).ToList()),
            Problem);
    }

    [HttpGet("{tripId:int}/items/{itemId:int}")]
    public async Task<IActionResult> GetTripItem(int tripId, int itemId)
    {
        var query = new GetTripItemQuery(tripId, itemId);

        var result = await _sender.Send(query);

        return result.Match(
            item => Ok(new TripItemResponse(item.Id, item.Name, item.Amount, item.MemberId, item.Email)),
            Problem);
    }

    [HttpGet("{tripId:int}/members")]
    public async Task<IActionResult> GetTripMembers(int tripId)
    {
        var query = new GetTripMembersQuery(tripId);

        var result = await _sender.Send(query);

        return result.Match(
            members => Ok(members.Select(m => new MemberResponse(m.Id, m.UserId, m.TripId, m.IsAdmin, m.Email)).ToList()),
            Problem);
    }

    [HttpPut("{tripId:int}")]
    public async Task<IActionResult> UpdateTrip(int tripId, UpdateTripRequest request)
    {
        var command = new UpdateTripCommand(tripId, request.Name, request.StartDate, request.EndDate, request.Description, request.Location, request.ImageUrl);

        var result = await _sender.Send(command);

        return result.Match(
            trip => Ok(new TripResponse(trip.Id, trip.Name, trip.StartDate, trip.EndDate, trip.Description, trip.Location, trip.ImageUrl, trip.CreatorUserId)),
            Problem);
    }

    [HttpPut("{tripId:int}/items/{tripItemId:int}")]
    public async Task<IActionResult> UpdateTripItem(int tripId, int tripItemId, UpdateTripItemRequest request)
    {
        var command = new UpdateTripItemCommand(tripId, tripItemId, request.Name, request.Amount, request.MemberId);

        var result = await _sender.Send(command);

        return result.Match(
            item => Ok(new TripItemResponse(item.Id, item.Name, item.Amount, item.MemberId, item.Email)),
            Problem);
    }

    [HttpDelete("{tripId:int}")]
    public async Task<IActionResult> DeleteTrip(int tripId)
    {
        var command = new DeleteTripCommand(tripId);

        var result = await _sender.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpDelete("{tripId:int}/items/{tripItemId:int}")]
    public async Task<IActionResult> DeleteTripItem(int tripId, int tripItemId)
    {
        var command = new DeleteTripItemCommand(tripId, tripItemId);

        var result = await _sender.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}