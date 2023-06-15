using Server.Data.Api;
using Server.Data.Api.Interfaces;
using Server.Data.ApiModels.User;
using Server.Data.User;

namespace Server.Core;

public static class ApiFormatHelper
{
    public static IApiResponse ToJsonApiFormat(this UserModel? user) =>
        new SingleApiResponse(user == null
            ? null
            : new ApiData(
                new UserApiAttributes(user),
                user.Role.ToRelationModel()));

    public static IApiResponse ToJsonApiFormat(this ICollection<UserModel> users) =>
        new ListApiResponse(!users.Any()
            ? null
            : users.Select(u => new ApiData(
                    new UserApiAttributes(u),
                    u.Role.ToRelationModel()))
                .ToList());

    public static IApiResponse ToJsonApiFormat(this ICollection<UserRole> userRoles) =>
        new ListApiResponse(!userRoles.Any()
            ? null
            : userRoles.Select(ur => new ApiData(
                new UserRoleApiAttributes(ur),
                ur.Users.ToRelationModel()))
                .ToList());

    private static IApiRelationship? ToRelationModel<T>(this ICollection<T>? collection) where T : IApiRelationshipModel
    {
        if (collection == null || !collection.Any())
        {
            return null;
        }

        return typeof(T) switch
        {
            not null when typeof(T) == typeof(UserModel) =>
                new UserApiRelationship(new ListApiResponse(
                    collection.Select(m => new ApiData(new UserApiAttributes(m as UserModel))).ToList())),
            not null when typeof(T) == typeof(UserRole) =>
                new UserRoleApiRelationship(new ListApiResponse(
                    collection.Select(m => new ApiData(new UserRoleApiAttributes(m as UserRole))).ToList())),
            _ => throw new NotImplementedException(typeof(T).ToString())
        };
    }
    
    private static IApiRelationship? ToRelationModel<T>(this T? model) where T : IApiRelationshipModel
    {
        if (model == null)
        {
            return null;
        }

        return typeof(T) switch
        {
            not null when typeof(T) == typeof(UserModel) => 
                new UserApiRelationship(
                    new SingleApiResponse(new ApiData(new UserApiAttributes(model as UserModel)))),
            not null when typeof(T) == typeof(UserRole) => 
                new UserRoleApiRelationship(
                    new SingleApiResponse(new ApiData(new UserRoleApiAttributes(model as UserRole)))),
            _ => throw new NotImplementedException(typeof(T).ToString())
        };
    }
}