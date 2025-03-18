using Taskify_App.Enums;
using Taskify_App.Models;
using Taskify_App.Repository;

namespace Taskify_App.App_Controller
{
    public struct SummaryDetails
    {
        public string projectName { get; set; }
        public string taskName { get; set; }
        public DateTime[] timeStamp { get; set; }
        public float totalTimeTaken { get; set; }
    }
    public class AppServices
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

        public AppServices(ProjectRepository projectRepository)
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
            activity.TotalTimeTaken = (float)(TaskEndTime - TaskStartTime).Seconds/3600;
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

        public string GetCurrentProject()
        {
            return ProjectName;
        }

        public List<string> GetProjectsName()
        {
            return Projects.Select(project => project.ProjectName).ToList();
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

        public List<Models.Activity> GetRecentActivities()
        {
            if (Projects.Count == 0)
            {
                return [];
            }
            List<Models.Activity> recentActivities = new List<Models.Activity>() { null, null};
            foreach(Project project in Projects)
            {
                foreach(Models.Activity activity in project.Activities)
                {
                    if (recentActivities[0] == null)
                    {
                        recentActivities[0] = activity;
                    }
                    else if (activity.TimeStamps.Count == 0)
                    {
                        continue;
                    }
                    else if (recentActivities[0].TimeStamps[recentActivities[0].TimeStamps.Count - 1][1] < activity.TimeStamps[activity.TimeStamps.Count - 1][1])
                    {
                        recentActivities[1] = recentActivities[0];
                        recentActivities[0] = activity;
                    }
                    else if (recentActivities[0].TimeStamps[recentActivities[0].TimeStamps.Count - 1][1] >= activity.TimeStamps[activity.TimeStamps.Count - 1][1] && recentActivities[1].TimeStamps[recentActivities[0].TimeStamps.Count - 1][1] < activity.TimeStamps[activity.TimeStamps.Count - 1][1])
                    {
                        recentActivities[1] = activity;
                    }
                }
            }
            return recentActivities;
        }

        public void ExportTasks(string projectName)
        {
            string filePath = $"../../../{projectName}";
            Project project = Projects.Where(project => project.ProjectName ==  projectName).FirstOrDefault()!;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Activity Name, Activity Description, Activity TimeStamps, Activity Time Limit");

                foreach(Models.Activity activity in project.Activities)
                {
                    writer.WriteLine($"{activity.TaskName}, {activity.TaskDescription}, {activity.TimeStamps}, {activity.TimeLimit}");
                }
            }
        }

        public void ExportAll()
        {
            string filePath = $"../../../{EmailID}";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Project Name, Project Category, Activity Name, Activity Description");
                foreach (Project project in Projects) 
                {
                    foreach (Models.Activity activity in project.Activities)
                    {
                        writer.WriteLine($"{project.ProjectName}, {project.ProjectCategory}, {activity.TaskName}, {activity.TaskDescription}");
                    }
                }
            }
        }

        public List<SummaryDetails> FilterWithDates(DateTime startDate, DateTime endDate)
        {
            List<SummaryDetails> filteredData = new List<SummaryDetails>();
            startDate = DateTime.Parse(startDate.ToString("yyyy-MM-dd"));
            endDate = DateTime.Parse(endDate.ToString("yyyy-MM-dd"));
            foreach (Project project in Projects)
            {
                foreach (Activity activity in project.Activities)
                {
                    foreach (DateTime[] timeStamp in activity.TimeStamps)
                    {
                        DateTime[] dateTime = new DateTime[] { timeStamp[0].Date, timeStamp[1].Date };
                        if (dateTime[0] >= startDate && dateTime[0] <= endDate || dateTime[1] >= startDate && dateTime[1] <= endDate)
                        {
                            filteredData.Add(new SummaryDetails {projectName = project.ProjectName, taskName = activity.TaskName, timeStamp = timeStamp, totalTimeTaken = activity.TotalTimeTaken});
                        }
                    }
                }
            }
            return filteredData;
        }

        public List<SummaryDetails> SortTaskByProjectName()
        {
            List<SummaryDetails> summaryDetails = GetAllTaskSummary();
            summaryDetails.Sort((element1, element2) => element1.projectName.CompareTo(element2.projectName));
            return summaryDetails;
        }

        public List<SummaryDetails> SortTaskByTaskName()
        {
            List<SummaryDetails> summaryDetails = GetAllTaskSummary();
            summaryDetails.Sort((element1, element2) => element1.taskName.CompareTo(element2.taskName));
            return summaryDetails;
        }

        public List<SummaryDetails> SortTaskByTimeTaken()
        {
            List<SummaryDetails> summaryDetails = GetAllTaskSummary();
            summaryDetails.Sort((element1, element2) => element1.totalTimeTaken.CompareTo(element2.totalTimeTaken));
            return summaryDetails;
        }

        private List<SummaryDetails> GetAllTaskSummary()
        {
            List<SummaryDetails> summaryDetails = new List<SummaryDetails>();
            foreach (Project project in Projects)
            {
                foreach (Activity activity in project.Activities)
                {
                    foreach (DateTime[] dateTime in activity.TimeStamps)
                    {
                        summaryDetails.Add(new SummaryDetails { projectName = project.ProjectName, taskName = activity.TaskName, timeStamp = dateTime, totalTimeTaken = activity.TotalTimeTaken });
                    }
                }
            }
            return summaryDetails;
        }

        public List<SummaryDetails> FilterByCategory(ProjectCategories projectCategory)
        {
            List<SummaryDetails> filteredSummaries = new List<SummaryDetails>();
            foreach (Project project in Projects)
            {
                if (project.ProjectCategory != projectCategory)
                {
                    continue;
                }

                foreach (Activity activity in project.Activities)
                {
                    foreach (DateTime[] dateTime in activity.TimeStamps)
                    {
                        filteredSummaries.Add(new SummaryDetails { projectName = project.ProjectName, taskName = activity.TaskName, timeStamp = dateTime, totalTimeTaken = activity.TotalTimeTaken});
                    }
                }
            }
            return filteredSummaries;
        }
    }
}