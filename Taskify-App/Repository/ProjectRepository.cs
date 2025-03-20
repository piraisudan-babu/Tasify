using System.Text.Json;
using Taskify_App.Models;

namespace Taskify_App.Repository
{
    /// <summary>
    /// Class <c>ProjectRepository</c> to hold the functionalities of interaction with project file.
    /// </summary>
    public class ProjectRepository
    {
        /// <summary>
        /// Function to write the project in the file
        /// </summary>
        /// <param name="projects">List of projects to be wrote in file.</param>
        /// <param name="emailID">Email ID of the user.</param>
        public void UpdateProjects(List<Project> projects, string emailID)
        {
            string jsonString = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText($"../../../{emailID}.json", jsonString);
        }

        /// <summary>
        /// Function to fetch file data and return contents.
        /// </summary>
        /// <param name="emailID">Email ID of the user.</param>
        /// <returns>List of Projects.</returns>
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
