using Microsoft.AspNetCore.Mvc;
using MediatR;
using TripHelper.Contracts.Trips;
using TripHelper.Application.Trips.Commands.CreateTrip;
using TripHelper.Application.Trips.Queries.GetTrip;
using TripHelper.Application.Trips.Queries.GetTrips;
using TripHelper.Application.Trips.Commands.UpdateTrip;
using TripHelper.Application.Trips.Commands.DeleteTrip;
using Microsoft.AspNetCore.Authorization;

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
        var query = new GetTripsQuery(1); // TODO: Get user id from token

        var result = await _sender.Send(query);

        return result.Match(
            trips => Ok(trips.Select(trip => new TripResponse(trip.Id, trip.Name, trip.StartDate, trip.EndDate, trip.Description, trip.Location, trip.ImageUrl, trip.CreatorUserId)).ToList()),
            Problem);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTrip(UpdateTripRequest request)
    {
        var command = new UpdateTripCommand(request.TripId, request.Name, request.StartDate, request.EndDate, request.Description, request.Location, request.ImageUrl);

        var result = await _sender.Send(command);

        return result.Match(
            trip => Ok(new TripResponse(trip.Id, trip.Name, trip.StartDate, trip.EndDate, trip.Description, trip.Location, trip.ImageUrl, trip.CreatorUserId)),
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
}