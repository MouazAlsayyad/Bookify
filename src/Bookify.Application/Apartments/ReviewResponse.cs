namespace Bookify.Application.Apartments;
public sealed class ReviewResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; }
    public DateTime CreatedOnUtc { get; init; }

    public ReviewResponse() { }

    public ReviewResponse(Guid id, Guid userId, string firstName, string lastName, int rating, string comment, DateTime createdOnUtc)
    {
        Id = id;
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Rating = rating;
        Comment = comment;
        CreatedOnUtc = createdOnUtc;
    }
}
