using Taskify_App.Enums;

namespace Taskify_App.Models
{
    /// <summary>
    /// Class <c>Project</c> to hold the attributes of the project.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Category of the project.
        /// </summary>
        public ProjectCategories ProjectCategory { get; set; }

        /// <summary>
        /// Description of the project.
        /// </summary>
        public string ProjectDescription { get; set; }

        /// <summary>
        /// List of activities for the project.
        /// </summary>
        public List<Activity> Activities { get; set; }

        /// <summary>
        /// Constructor to initialize the project details.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="projectCategory">Category of the project.</param>
        /// <param name="projectDescription">Description of the project.</param>
        /// <param name="activities">Activity list of the project.</param>
        public Project(string projectName, ProjectCategories projectCategory, string projectDescription, List<Activity> activities)
        {
            ProjectName = projectName;
            ProjectCategory = projectCategory;
            ProjectDescription = projectDescription;
            Activities = activities;
        }
    }
}