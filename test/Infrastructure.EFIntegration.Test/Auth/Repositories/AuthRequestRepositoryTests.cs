﻿using Bit.Core.Auth.Entities;
using Bit.Core.Entities;
using Bit.Core.Test.AutoFixture.Attributes;
using Bit.Infrastructure.EFIntegration.Test.Auth.AutoFixture;
using Bit.Infrastructure.EFIntegration.Test.Auth.Repositories.EqualityComparers;
using Xunit;
using EfAuthRepo = Bit.Infrastructure.EntityFramework.Auth.Repositories;
using EfRepo = Bit.Infrastructure.EntityFramework.Repositories;
using SqlAuthRepo = Bit.Infrastructure.Dapper.Auth.Repositories;
using SqlRepo = Bit.Infrastructure.Dapper.Repositories;

namespace Bit.Infrastructure.EFIntegration.Test.Auth.Repositories;

public class AuthRequestRepositoryTests
{
    [CiSkippedTheory, EfAuthRequestAutoData]
    public async void CreateAsync_Works_DataMatches(
        AuthRequest authRequest,
        AuthRequestCompare equalityComparer,
        List<EfAuthRepo.AuthRequestRepository> suts,
        SqlAuthRepo.AuthRequestRepository sqlAuthRequestRepo,
        User user,
        List<EfRepo.UserRepository> efUserRepos,
        SqlRepo.UserRepository sqlUserRepo
        )
    {
        authRequest.ResponseDeviceId = null;
        var savedAuthRequests = new List<AuthRequest>();
        foreach (var sut in suts)
        {
            var i = suts.IndexOf(sut);

            var efUser = await efUserRepos[i].CreateAsync(user);
            sut.ClearChangeTracking();
            authRequest.UserId = efUser.Id;

            var postEfAuthRequest = await sut.CreateAsync(authRequest);
            sut.ClearChangeTracking();

            var savedAuthRequest = await sut.GetByIdAsync(postEfAuthRequest.Id);
            savedAuthRequests.Add(savedAuthRequest);
        }

        var sqlUser = await sqlUserRepo.CreateAsync(user);
        authRequest.UserId = sqlUser.Id;
        var sqlAuthRequest = await sqlAuthRequestRepo.CreateAsync(authRequest);
        var savedSqlAuthRequest = await sqlAuthRequestRepo.GetByIdAsync(sqlAuthRequest.Id);
        savedAuthRequests.Add(savedSqlAuthRequest);

        var distinctItems = savedAuthRequests.Distinct(equalityComparer);
        Assert.True(!distinctItems.Skip(1).Any());
    }
}
