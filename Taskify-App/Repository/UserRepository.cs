using System.Text.Json;
using Taskify_App.Models;

namespace Taskify_App.Repository
{
    public class UserRepository
    {
        public void UpdateUsers(List<User> users)
        {
            string jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("../../../Users.json", jsonString);
        }

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
