using System;
using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler(IUsersRepository usersRepository) : IRequestHandler<GetUserQuery, ErrorOr<User>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        
        public async Task<ErrorOr<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetUserByIdAsync(request.Id);
            return user is not null ? user : Error.NotFound(description: $"User with id {request.Id} not found");
        }
    }
}