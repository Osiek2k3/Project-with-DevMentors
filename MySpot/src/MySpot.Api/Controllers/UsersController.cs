using Microsoft.AspNetCore.Mvc;
using MySpot.Application.Abstractions;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Queries;

namespace MySpot.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandHandler<SignUp> _signUpHandler;
        private readonly IQueryHandler<GetUsers, IEnumerable<UserDto>> _getUsersHandler;
        private readonly IQueryHandler<GetUser, UserDto> _getUserHandler;

        public UsersController(ICommandHandler<SignUp> signUpHandler,
            IQueryHandler<GetUsers, IEnumerable<UserDto>> getUsersHandler,
            IQueryHandler<GetUser, UserDto> getUserHandler)
        {
            _signUpHandler = signUpHandler;
            _getUsersHandler = getUsersHandler;
            _getUserHandler = getUserHandler;
        }

        [HttpPost]
        public async Task<ActionResult> Post(SignUp command)
        {
            command = command with { UserId = Guid.NewGuid() };
            await _signUpHandler.HandleAsync(command);
            return NoContent();
        }

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserDto>> Get(Guid userId)
        {
            var user = await _getUserHandler.HandleAsync(new GetUser { UserId = userId });
            if (user is null)
            {
                return NotFound();
            }

            return user;
        }
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Get()
        {
            if (string.IsNullOrWhiteSpace(User.Identity?.Name))
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.Identity?.Name);
            var user = await _getUserHandler.HandleAsync(new GetUser { UserId = userId });

            return user;
        }
        
    }
}
