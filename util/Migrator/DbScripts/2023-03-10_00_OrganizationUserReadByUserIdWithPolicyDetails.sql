SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[OrganizationUser_ReadByUserIdWithPolicyDetails]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON
SELECT
    OU.[Id] as OrganizationUserId,
    P.[OrganizationId],
    P.[Type] as PolicyType,
    P.[Enabled] as PolicyEnabled,
    P.[Data] as PolicyData,
    OU.[Type] as OrganizationUserType,
    OU.[Status] as OrganizationUserStatus,
    OU.[Permissions] as OrganizationUserPermissionsData,
    (CASE WHEN PUPO.[UserId] IS NULL THEN 0 ELSE 1 END) as IsProvider
FROM
    [dbo].[PolicyView] P
INNER JOIN
    [dbo].[OrganizationUserView] OU ON P.[OrganizationId] = OU.[OrganizationId]
LEFT JOIN
    (SELECT
        PU.UserId,
        PO.OrganizationId
    FROM
        [dbo].[ProviderUserView] PU
    INNER JOIN
        [ProviderOrganizationView] PO ON PO.[ProviderId] = PU.[ProviderId]) PUPO
    ON PUPO.UserId = OU.UserId
    AND PUPO.OrganizationId = P.OrganizationId
WHERE
    (
        (
            OU.[Status] != 0     -- OrgUsers who have accepted their invite and are linked to a UserId
            AND OU.[UserId] = @UserId
        )
        OR (
            OU.[Status] = 0     -- 'Invited' OrgUsers are not linked to a UserId yet, so we have to look up their email
            AND OU.[Email] IN (SELECT U.Email FROM [dbo].[UserView] U WHERE U.Id = @UserId)
        )
   )
END
GO