﻿using Bit.Core.Auth.Entities;

namespace Bit.Core.Repositories;

public interface IAuthRequestRepository : IRepository<AuthRequest, Guid>
{
    Task<int> DeleteExpiredAsync();
    Task<ICollection<AuthRequest>> GetManyByUserIdAsync(Guid userId);
}
