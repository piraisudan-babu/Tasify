using Taskify_App.App_Controller;
using Taskify_App.Models;
using Taskify_App.Repository;

namespace Taskify_App
{
    /// <summary>
    /// Class <c>AuthenticationServices</c> to hold the authentication functionalities.
    /// </summary>
    public class AuthenticatorServices
    {
        private UserRepository _userRepository;
        private List<User> Users;
        private AppServices _projectService;

        /// <summary>
        /// Constructor to initialize the dependencies and list of users.
        /// </summary>
        /// <param name="userRepository">UserRepository class object.</param>
        /// <param name="projectService">ProjectService class object.</param>
        public AuthenticatorServices(UserRepository userRepository, AppServices projectService)
        {
            _userRepository = userRepository;
            List<User>? users = _userRepository.GetUsers();
            Users = (users == null) ? new List<User>() : users;
            _projectService = projectService;
        }

        /// <summary>
        /// Function to check whether the given email id exist or not.
        /// </summary>
        /// <param name="emailID">Email ID to be searched.</param>
        /// <returns>True if email exist else false.</returns>
        public bool IsEmailIDFound(string emailID)
        {
            return Users.Any(user => user.EmailID.Equals(emailID));
        }

        /// <summary>
        /// Function for login functionality.
        /// </summary>
        /// <param name="emailID">Email ID of the user.</param>
        /// <param name="password">Password of the user.</param>
        /// <returns>True if login successful else false.</returns>
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

        /// <summary>
        /// Function to get user details and add to the existing user list.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="emailID">User email ID</param>
        /// <param name="password">User Password.</param>
        public void AddUser(string userName, string emailID,  string password)
        {
            User newUser = new User(userName, emailID, BCrypt.Net.BCrypt.HashPassword(password));
            Users.Add(newUser);
            _userRepository.UpdateUsers(Users);
            _projectService.SetProjects(emailID);
        }

        /// <summary>
        /// Function to check 2 password are same or not.
        /// </summary>
        /// <param name="expectedPassword">Password in the repository</param>
        /// <param name="actualPassword">Password given by user.</param>
        /// <returns></returns>
        private bool IsSamePasswords(string expectedPassword, string actualPassword)
        {
            return BCrypt.Net.BCrypt.Verify(actualPassword, expectedPassword);
        }
    }
}
