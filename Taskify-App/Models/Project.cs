using Taskify_App.Enums;

namespace Taskify_App.Models
{
    public class Project
    {
        public string ProjectName { get; set; }
        public ProjectCategories ProjectCategory { get; set; }
        public string ProjectDescription { get; set; }
        public List<Activity> Activities { get; set; }

        public Project(string projectName, ProjectCategories projectCategory, string projectDescription, List<Activity> activities)
        {
            ProjectName = projectName;
            ProjectCategory = projectCategory;
            ProjectDescription = projectDescription;
            Activities = activities;
        }
    }
}