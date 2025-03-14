using Taskify_App.Models;
using Taskify_App.Repository;

namespace Taskify_App
{
    public class AuthenticatorServices
    {
        private UserRepository _userRepository;
        private List<User> Users;

        public AuthenticatorServices(UserRepository userRepository)
        {
            _userRepository = userRepository;
            List<User>? users = _userRepository.GetUsers();
            Users = (users == null) ? new List<User>() : users;
        }

        public bool IsEmailIDFound(string emailID)
        {
            return Users.Any(user => user.EmailID.Equals(emailID));
        }

        public bool IsLoginSuccessful(string emailID,  string password)
        {
            User user = Users.Where(user => user.EmailID.Equals(emailID)).First();
            if (IsSamePasswords(user.Password, password))
            {
                return true;
            }
            return false;
        }

        public void AddUser(string userName, string emailID,  string password)
        {
            User newUser = new User(userName, emailID, BCrypt.Net.BCrypt.HashPassword(password));
            Users.Add(newUser);
            _userRepository.UpdateUsers(Users);
        }

        private bool IsSamePasswords(string expectedPassword, string actualPassword)
        {
            return BCrypt.Net.BCrypt.Verify(actualPassword, expectedPassword);
        }
    }
}
