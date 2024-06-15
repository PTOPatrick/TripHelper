using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace TripHelper.Api.Controllers;

[Route("api/[controller]")]
public class TripsController : ApiController
{
    private readonly ISender _sender;

    public TripsController(ISender sender)
    {
        _sender = sender;
    }
}