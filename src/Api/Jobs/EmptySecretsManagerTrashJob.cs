﻿using Bit.Core.Jobs;
using Bit.Core.SecretsManager.Repositories;
using Quartz;

namespace Bit.Api.Jobs;

public class EmptySecretsManagerTrashJob : BaseJob
{
    private ISecretRepository _secretRepository;
    private const uint DeleteAfterThisNumberOfDays = 30;

    public EmptySecretsManagerTrashJob(ISecretRepository secretRepository, ILogger<AliveJob> logger) : base(logger)
    {
        _secretRepository = secretRepository;
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        await _secretRepository.EmptyTrash(DateTime.UtcNow, DeleteAfterThisNumberOfDays);
    }
}
