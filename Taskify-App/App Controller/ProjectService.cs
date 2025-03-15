using System.Diagnostics;
using Taskify_App.Enums;
using Taskify_App.Models;
using Taskify_App.Repository;

namespace Taskify_App.App_Controller
{
    public class ProjectService
    {
        private List<Project> Projects;
        private ProjectRepository _projectRepository;
        private List<Models.Activity> Activities;
        private string EmailID;
        private string ProjectName;
        private string selectedTaskName = "";
        private string runningTaskName = "";
        private DateTime TaskStartTime = DateTime.MinValue;
        private DateTime TaskEndTime = DateTime.MinValue;

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
            List<Models.Activity> activities = Projects.Where(project => project.ProjectName == projectName).SelectMany(project => project.Activities).ToList();
            if (activities.Any())
            {
                Activities = activities;
            }
            else
            {
                Activities = new List<Models.Activity>();
            }
            ProjectName = projectName;
        }

        public bool SetCurrentTask(string taskName)
        {
            if (selectedTaskName == "" && runningTaskName =="" || taskName == runningTaskName)
            {
                selectedTaskName = taskName;
                return true;
            }
            return false;
        }

        public bool SetStartTime()
        {
            TaskStartTime = DateTime.Now;
            runningTaskName = selectedTaskName;
            return true;
        }

        public bool SetStopTime()
        {
            if (runningTaskName == "")
            {
                selectedTaskName = "";
                return false;
            }
            TaskEndTime = DateTime.Now;
            Models.Activity activity = Activities.Where(activity => activity.TaskName == runningTaskName).FirstOrDefault()!;
            activity.TotalTimeTaken = (TaskEndTime - TaskStartTime).Hours;
            activity.TimeStamps.Add(new[] {TaskStartTime, TaskEndTime});
            TaskStartTime = DateTime.MinValue;
            TaskEndTime = DateTime.MinValue;
            runningTaskName = "";
            selectedTaskName = "";
            return true;
        }

        public string GetCurrentTask()
        {
            return selectedTaskName;
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

        public void AddActivity(Models.Activity activity)
        {
            Activities.Add(activity);
        }

        public void RemoveActivity(string activityName)
        {
            Models.Activity activity = Activities.FirstOrDefault(activity => activity.TaskName == activityName)!;
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
            Models.Activity activity = Activities.FirstOrDefault(activity => activity.TaskName == activityName)!;
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
    }
}