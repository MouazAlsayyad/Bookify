namespace Bookify.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdentityIdAsync(string identityId);
    Task AddAsync(User user);
}
