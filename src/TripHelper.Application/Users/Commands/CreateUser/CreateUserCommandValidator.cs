using FluentValidation;

namespace TripHelper.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Firstname).NotEmpty();

        RuleFor(x => x.Lastname).NotEmpty();
        
        RuleFor(x => x.Password).NotEmpty();
        
        RuleFor(x => x.Email).EmailAddress();
    }
}