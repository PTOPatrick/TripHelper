using FluentAssertions;
using TestCommon.Members;
using TestCommon.TestConstants;

namespace TripHelper.Domain.UnitTests.Members;

public class MemberTests
{
    [Fact]
    public void Update_UpdateRole_ShouldPass()
    {
        // Arrange
        var member = MemberFactory.CreateMember(Constants.Member.UserId, Constants.Member.TripId);
    
        // Act
        member.Update(true);
    
        // Assert
        member.UserId.Should().Be(Constants.Member.UserId);
        member.TripId.Should().Be(Constants.Member.TripId);
        member.IsAdmin.Should().BeTrue();
    }
}