using Taskify_App.App_Controller;
using Taskify_App.Models;
using Taskify_App.Repository;

namespace Taskify_App
{
    public class AuthenticatorServices
    {
        private UserRepository _userRepository;
        private List<User> Users;
        private AppServices _projectService;

        public AuthenticatorServices(UserRepository userRepository, AppServices projectService)
        {
            _userRepository = userRepository;
            List<User>? users = _userRepository.GetUsers();
            Users = (users == null) ? new List<User>() : users;
            _projectService = projectService;
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
                _projectService.SetProjects(emailID);
                return true;
            }
            return false;
        }

        public void AddUser(string userName, string emailID,  string password)
        {
            User newUser = new User(userName, emailID, BCrypt.Net.BCrypt.HashPassword(password));
            Users.Add(newUser);
            _userRepository.UpdateUsers(Users);
            _projectService.SetProjects(emailID);
        }

        private bool IsSamePasswords(string expectedPassword, string actualPassword)
        {
            return BCrypt.Net.BCrypt.Verify(actualPassword, expectedPassword);
        }
    }
}
