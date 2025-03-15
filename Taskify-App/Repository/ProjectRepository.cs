using System.Text.Json;
using Taskify_App.Models;

namespace Taskify_App.Repository
{
    public class ProjectRepository
    {
        public void UpdateProjects(List<Project> projects, string emailID)
        {
            string jsonString = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText($"../../../{emailID}.json", jsonString);
        }

        public List<Project>? GetProjects(string emailID)
        {
            try
            {
                string jsonString = File.ReadAllText($"../../../{emailID}.json");
                List<Project>? projects = JsonSerializer.Deserialize<List<Project>>(jsonString);
                return projects;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
