using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Tests.Integration.Controllers
{
    internal class TestUserRepository : IUserRepository
    {
        private readonly List<User> _users = new ();
        public Task AddAsync(User user)
        {
            _users.Add (user);
            return Task.CompletedTask;
        }

        public Task<User> GetByEmailAsync(Email email)
            => Task.FromResult(_users.SingleOrDefault(x => x.Email == email));

        public Task<User> GetByIdAsync(UserId id)
            => Task.FromResult(_users.SingleOrDefault(x => x.Id == id));

        public Task<User> GetByUsernameAsync(Username username)
            => Task.FromResult(_users.SingleOrDefault(x =>x.Username == username));
    }
}
