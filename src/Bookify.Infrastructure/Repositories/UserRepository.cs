using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public override async Task AddAsync(User user)
    {
        foreach (Role role in user.Roles)
        {
            DbContext.Attach(role);
        }

        await DbContext.AddAsync(user);
    }

    public async Task<User?> GetUserByIdentityIdAsync(string identityId)
    {
        User? user = await DbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.IdentityId == identityId);

        return user;
    }
}
