namespace Bookify.Domain.Reviews;

public interface IReviewRepository
{
    Task AddAsync(Review review);
}
