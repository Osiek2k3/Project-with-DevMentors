using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Queries;
using MySpot.Application.Security;
using MySpot.Infrastructure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MySpot.Api
{
    public static class UsersApi
    {
        private const string MeRoute = "me";

        public static void UseUsersApi(this WebApplication app)
        {
            app.MapGet("api", (IOptions<AppOptions> options) => Results.Ok(options.Value.Name));

            app.MapGet("api/users/me",GetMe)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization()
                .WithName("me");

            app.MapGet("api/users/{userId:guid}", GetById)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization("is-admin")
                .WithName("GetUserById");

            app.MapPost("api/users/signup", SignUp)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithName("SignUp");

            app.MapPost("api/users/signin", SignIn)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithName("SignIn");



        }

        private static async Task<IResult>  GetMe(HttpContext context, IQueryHandler<GetUser, UserDto> handler)
        {
            if (context.User.Identity?.Name == null)
            {
                return Results.Unauthorized();
            }
            var match = Regex.Match(context.User.Identity.Name, @"\b[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-\b[0-9a-fA-F]{12}\b");
            if (!match.Success)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(match.Value);
            var userDto = await handler.HandleAsync(new GetUser { UserId = userId });

            return userDto is null ? Results.NotFound() : Results.Ok(userDto);
        }

        private static async Task <IResult> GetById(Guid userId, IQueryHandler<GetUser, UserDto> handler)
        {
            var userDto = await handler.HandleAsync(new GetUser { UserId = userId });
            return userDto is null ? Results.NotFound() : Results.Ok(userDto);
        }

        private static async Task<IResult> SignUp(SignUp command, ICommandHandler<SignUp> handler)
        {
            command = command with { UserId = Guid.NewGuid() };
            await handler.HandleAsync(command);
            return Results.CreatedAtRoute(MeRoute, new { });
        }


        private static async Task<IResult> SignIn(SignIn command, ICommandHandler<SignIn> handler,
            ITokenStorage _tokenStorage)
        {
            await handler.HandleAsync(command);
            var jwt = _tokenStorage.Get();
            return Results.Ok(jwt);
        }

        
    }
}
