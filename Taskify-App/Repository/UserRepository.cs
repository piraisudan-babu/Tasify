using System.Text.Json;
using Taskify_App.Models;

namespace Taskify_App.Repository
{
    /// <summary>
    /// Class <c>UserRepository</c> to hold the functionalities of the operations on user file
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Function to write users.
        /// </summary>
        /// <param name="users">List of users.</param>
        public void UpdateUsers(List<User> users)
        {
            string jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("../../../Users.json", jsonString);
        }

        /// <summary>
        /// Function to fetch the users from file and return the content.
        /// </summary>
        /// <returns>List of users.</returns>
        public List<User>? GetUsers()
        {
            try
            {
                string jsonString = File.ReadAllText("../../../Users.json");
                List<User>? users = JsonSerializer.Deserialize<List<User>>(jsonString);
                return users;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
