using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}