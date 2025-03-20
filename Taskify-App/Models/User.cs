namespace Taskify_App.Models
{
    /// <summary>
    /// Class <c>User</c> to hold the user details.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Name of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email ID of the user.
        /// </summary>
        public string EmailID { get; set; }

        /// <summary>
        /// Password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Constructor to initialize user details.
        /// </summary>
        /// <param name="userName">Name of the user</param>
        /// <param name="emailID">Email ID of the user.</param>
        /// <param name="password">Password of the user.</param>
        public User(string userName, string emailID, string password)
        {
            UserName = userName;
            EmailID = emailID;
            Password = password;
        }
    }
}
