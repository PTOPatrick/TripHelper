using ErrorOr;
using TripHelper.Domain.Common;

namespace TripHelper.Domain.TripItems;

public class TripItem(string name, int tripId, int memberId) : Entity
{
    public string Name { get; private set; } = name;
    public int TripId { get; private set; } = tripId;
    public decimal Amount { get; private set; }
    public int MemberId { get; private set; } = memberId;

    public ErrorOr<Success> AssignAmount(decimal amount)
    {
        if (amount <= 0)
            return TripItemErrors.AmountMustBePositive;

        Amount = amount;

        return Result.Success;
    }

    public ErrorOr<Success> Update(string name, decimal amount, int memberId)
    {
        Name = name;
        MemberId = memberId;

        return AssignAmount(amount);
    }
}