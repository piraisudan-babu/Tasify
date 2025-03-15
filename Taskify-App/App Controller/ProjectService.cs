using Taskify_App.Enums;
using Taskify_App.Models;
using Taskify_App.Repository;

namespace Taskify_App.App_Controller
{
    public class ProjectService
    {
        private List<Project> Projects;
        private ProjectRepository _projectRepository;
        private List<Activity> Activities;
        private string EmailID;
        private string ProjectName;

        public ProjectService(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public void SetProjects(string emailID)
        {
            List<Project>? projects = _projectRepository.GetProjects(emailID);
            Projects = (projects == null) ? new List<Project>() : projects;
            EmailID = emailID;
        }

        public void SetTasks(string projectName)
        {
            List<Activity> activities = Projects.Where(project => project.ProjectName == projectName).SelectMany(project => project.Activities).ToList();
            if (activities.Any())
            {
                Activities = activities;
            }
            else
            {
                Activities = new List<Activity>();
            }
            ProjectName = projectName;
        }

        public string[] GetProjectsName()
        {
            return Projects.Select(project => project.ProjectName).ToArray();
        } 

        public void CreateProject(Project project)
        {
            Projects.Add(project);
        }

        public bool IsProjectExist(string projectName)
        {
            return Projects.Any(project => project.ProjectName == projectName);
        }

        public void AddActivity(Activity activity)
        {
            Activities.Add(activity);
        }

        public void RemoveActivity(string activityName)
        {
            Activity activity = Activities.FirstOrDefault(activity => activity.TaskName == activityName)!;
            Activities.Remove(activity);
        }

        public bool IsActivityFound(string activityName)
        {
            return Activities.Any(activity => activity.TaskName == activityName);
        }

        public List<string> GetActivitiesName()
        {
            return Activities.Select(activity => activity.TaskName).ToList();
        }

        public void UpdateActivity(string activityName, string activityNameToBeUpdated, string activityDescriptionToUpdated, int activityTimeLimitToBeUpdated)
        {
            Activity activity = Activities.FirstOrDefault(activity => activity.TaskName == activityName)!;
            if (activityNameToBeUpdated != "")
            {
                activity.TaskName = activityNameToBeUpdated;
            }
            if (activityDescriptionToUpdated != "")
            {
                activity.TaskDescription = activityDescriptionToUpdated;
            }
            if (activityTimeLimitToBeUpdated != -1)
            {
                activity.TimeLimit = activityTimeLimitToBeUpdated;
            }
        }

        public void UpdateProjects()
        {
            _projectRepository.UpdateProjects(Projects, EmailID);
        }

        public void MergeTaskWithProject()
        {
            Project project = Projects.Where(project => project.ProjectName == ProjectName).FirstOrDefault()!;
            project.Activities = Activities;
        }

        public 
    }
}