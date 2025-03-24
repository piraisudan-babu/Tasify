using System.Text.Json;

namespace Taskify_App.Repository
{
    public class TaskIDRepository
    {
        /// <summary>
        /// Function to write the project in the file
        /// </summary>
        /// <param name="projects">List of projects to be wrote in file.</param>
        /// <param name="emailID">Email ID of the user.</param>
        public void UpdateTaskID(long taskID)
        {
            string jsonString = JsonSerializer.Serialize(taskID, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText($"../../../TaskID.json", jsonString);
        }

        /// <summary>
        /// Function to fetch file data and return contents.
        /// </summary>
        /// <param name="emailID">Email ID of the user.</param>
        /// <returns>List of Projects.</returns>
        public long GetTaskID()
        {
            try
            {
                string jsonString = File.ReadAllText($"../../../TaskID.json");
                long taskID = JsonSerializer.Deserialize<long>(jsonString);
                return taskID;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
