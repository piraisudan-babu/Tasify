namespace Taskify_App.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public string Password { get; set; }
        public User(string userName, string emailID, string password)
        {
            UserName = userName;
            EmailID = emailID;
            Password = password;
        }
    }
}
