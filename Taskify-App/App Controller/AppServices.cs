using System;
using System.Diagnostics;
using Taskify_App.Enums;
using Taskify_App.Models;
using Taskify_App.Repository;

namespace Taskify_App.App_Controller
{
    /// <summary>
    /// Class <c>AppServices</c> to hold the app related functionalities. 
    /// </summary>
    public class AppServices
    {
        private List<Project> Projects;
        private ProjectRepository _projectRepository;
        private List<Models.Activity> Activities;
        private string EmailID;
        private string ProjectName;
        private long selectedTaskID = -1;
        private long runningTaskID = -1;
        public string runningProject { get; set; } = string.Empty;
        private DateTime TaskStartTime = DateTime.MinValue;
        private DateTime TaskEndTime = DateTime.MinValue;
        private long TaskID { get; set; }

        /// <summary>
        /// Constructor to initialize the ProjectRepository object.
        /// </summary>
        /// <param name="projectRepository"></param>
        public AppServices(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Function to set the current working project.
        /// </summary>
        /// <param name="emailID">User Email ID</param>
        public void SetProjects(string emailID)
        {
            List<Project>? projects = _projectRepository.GetProjects(emailID);
            Projects = (projects == null) ? new List<Project>() : projects;
            EmailID = emailID;
        }

        /// <summary>
        /// Function to set the current working tasks.
        /// </summary>
        /// <param name="projectName">Project name</param>
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

        /// <summary>
        /// Function to set the current working task.
        /// </summary>
        /// <param name="taskName">Current working task name.</param>
        /// <returns>True if task is set else false.</returns>
        public bool SetCurrentTask(long taskID)
        {
            if (selectedTaskID == -1 && runningTaskID == -1 || taskID == runningTaskID)
            {
                selectedTaskID = taskID;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function to set the start time of task.
        /// </summary>
        /// <returns>True if the task is started else false.</returns>
        public bool SetStartTime()
        {
            if (runningTaskID !=  -1)
            {
                return false;
            }
            TaskStartTime = DateTime.Now;
            runningTaskID = selectedTaskID;
            runningProject = ProjectName;
            return true;
        }

        /// <summary>
        /// Function to set the stop time of the task.
        /// </summary>
        /// <returns>True if the task is stopped else false.</returns>
        public bool SetStopTime()
        {
            if (runningTaskID == -1)
            {
                selectedTaskID = -1;
                return false;
            }
            TaskEndTime = DateTime.Now;
            Models.Activity activity = Activities.Where(activity => activity.TaskID == runningTaskID).FirstOrDefault()!;
            activity.TotalTimeTaken += (float)(TaskEndTime - TaskStartTime).Seconds/3600;
            activity.TimeStamps.Add(new[] {TaskStartTime, TaskEndTime});
            TaskStartTime = DateTime.MinValue;
            TaskEndTime = DateTime.MinValue;
            runningTaskID = -1;
            selectedTaskID = -1;
            runningProject = string.Empty;
            return true;
        }

        /// <summary>
        /// Function to get the current task.
        /// </summary>
        /// <returns>Current task name</returns>
        public long GetCurrentTask()
        {
            return selectedTaskID;
        }

        /// <summary>
        /// Function to get the current project.
        /// </summary>
        /// <returns>Current project name.</returns>
        public string GetCurrentProject()
        {
            return ProjectName;
        }

        /// <summary>
        /// Function to get the all available projects name.
        /// </summary>
        /// <returns>List of project names.</returns>
        public List<string> GetProjectsName()
        {
            return Projects.Select(project => project.ProjectName).ToList();
        } 

        /// <summary>
        /// Function to add a project to the project list.
        /// </summary>
        /// <param name="project">Project that need to be added.</param>
        public void CreateProject(Project project)
        {
            Projects.Add(project);
        }

        /// <summary>
        /// Function to check whether the project exist or not.
        /// </summary>
        /// <param name="projectName">Project name to be checked whether exists.</param>
        /// <returns>True if project exist else false.</returns>
        public bool IsProjectExist(string projectName)
        {
            return Projects.Any(project => project.ProjectName == projectName);
        }

        /// <summary>
        /// Function to add activity to the activity list.
        /// </summary>
        /// <param name="activity">Activity to be added.</param>
        public void AddActivity(Models.Activity activity)
        {
            Activities.Add(activity);
        }

        /// <summary>
        /// Function to remove an activity from the activity list.
        /// </summary>
        /// <param name="activityName">Activity Name.</param>
        public void RemoveActivity(long activityID)
        {
            Models.Activity activity = Activities.FirstOrDefault(activity => activity.TaskID == activityID)!;
            Activities.Remove(activity);
        }

        public void RemoveProject(string projectName)
        {
            Project projectToBeDeleted = Projects.FirstOrDefault(project => project.ProjectName == projectName)!;
            Projects.Remove(projectToBeDeleted);
        }

        /// <summary>
        /// Function to check whether the activity is in the activity list or not.
        /// </summary>
        /// <param name="activityName">Activity Name.</param>
        /// <returns>True if activity is found else false.</returns>
        public bool IsActivityFound(string activityName)
        {
            return Activities.Any(activity => activity.TaskName == activityName);
        }

        /// <summary>
        /// Function to get all activities names.
        /// </summary>
        /// <returns>List of activity names.</returns>
        public string[] GetActivitiesDetails()
        {
            string[] activityDetails = new string[Activities.Count + 1];
            int count = 0;
            foreach(var activity in Activities)
            {
                string detail = activity.TaskID + " , " + activity.TaskName + " , ";
                if (activity.TaskDescription.Length <= 20)
                    detail += activity.TaskDescription;
                else 
                    detail += activity.TaskDescription.Substring(0,20) + "...";
                activityDetails[count++] = detail;
            }
            activityDetails[count] = "Exit";
            return activityDetails;
        }

        /// <summary>
        /// Function to edit the activity.
        /// </summary>
        /// <param name="activityName">Name of the activity.</param>
        /// <param name="activityNameToBeUpdated">Name to be updated.</param>
        /// <param name="activityDescriptionToUpdated">Description to be updated.</param>
        /// <param name="activityTimeLimitToBeUpdated">Time limit to be updated.</param>
        public void UpdateActivity(long activityID, string activityNameToBeUpdated, string activityDescriptionToUpdated, float activityTimeLimitToBeUpdated)
        {
            Models.Activity activity = Activities.FirstOrDefault(activity => activity.TaskID == activityID)!;
            if (activityNameToBeUpdated != string.Empty)
            {
                activity.TaskName = activityNameToBeUpdated;
            }
            if (activityDescriptionToUpdated != string.Empty)
            {
                activity.TaskDescription = activityDescriptionToUpdated;
            }
            if (activityTimeLimitToBeUpdated != -1)
            {
                activity.TimeLimit = activityTimeLimitToBeUpdated;
            }
        }

        /// <summary>
        /// Function to update the projects.
        /// </summary>
        public void UpdateProjects()
        {
            _projectRepository.UpdateProjects(Projects, EmailID);
        }

        /// <summary>
        /// Function to combine and update the task in the current project.
        /// </summary>
        public void MergeTaskWithProject()
        {
            Project project = Projects.Where(project => project.ProjectName == ProjectName).FirstOrDefault()!;
            project.Activities = Activities;
        }

        /// <summary>
        /// Function to compute the recent tasks.
        /// </summary>
        /// <returns>List of recent task.</returns>
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
                        continue;
                    }
                    else if (activity.TimeStamps.Count == 0)
                    {
                        continue;
                    }
                    else if (recentActivities[0].TimeStamps[recentActivities[0].TimeStamps.Count - 1][1] < activity.TimeStamps[activity.TimeStamps.Count - 1][1])
                    {
                        recentActivities[1] = recentActivities[0];
                        recentActivities[0] = activity;
                        continue;
                    }
                    Console.WriteLine(activity.TimeStamps[activity.TimeStamps.Count - 1][1]);
                    if (recentActivities[1] == null || recentActivities[0].TimeStamps[recentActivities[0].TimeStamps.Count - 1][1] >= activity.TimeStamps[activity.TimeStamps.Count - 1][1] && recentActivities[1].TimeStamps[recentActivities[0].TimeStamps.Count - 1][1] < activity.TimeStamps[activity.TimeStamps.Count - 1][1])
                    {
                        recentActivities[1] = activity;
                    }
                }
            }
            return recentActivities;
        }

        /// <summary>
        /// Function to export the tasks.
        /// </summary>
        /// <param name="projectName">Project to be exported.</param>
        public void ExportTasks(string projectName)
        {
            string filePath = $"../../../{projectName}";
            Project project = Projects.Where(project => project.ProjectName ==  projectName).FirstOrDefault()!;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Activity Name, Activity Description, Activity TimeStamps(start), Activity TimeStamps(stop), Activity Time Limit");

                foreach(Models.Activity activity in project.Activities)
                {
                    writer.WriteLine($"{activity.TaskName}, {activity.TaskDescription}, {activity.TimeStamps[0]}, {activity.TimeStamps[1]}, {activity.TimeLimit}");
                }
            }
        }

        /// <summary>
        /// Function to export all
        /// </summary>
        public void ExportAll()
        {
            string filePath = $"../../../{EmailID}";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Project Name, Project Category, Activity Name, Activity Description, Time Stamp(start), Time Stamp(stop)");
                foreach (Project project in Projects) 
                {
                    if (project.Activities.Count == 0)
                    {
                        writer.WriteLine($"{project.ProjectName}, {project.ProjectCategory}, - , -, -, -");
                    }
                    foreach (Models.Activity activity in project.Activities)
                    {
                        if (activity.TimeStamps.Count == 0)
                        {
                            writer.WriteLine($"{project.ProjectName}, {project.ProjectCategory}, {activity.TaskName}, {activity.TaskDescription}, -, -");
                        }
                        foreach (DateTime[] timestamp in activity.TimeStamps)
                        {
                            writer.WriteLine($"{project.ProjectName}, {project.ProjectCategory}, {activity.TaskName}, {activity.TaskDescription}, {timestamp[0]}, {timestamp[1]}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function to filter the tasks with dates.
        /// </summary>
        /// <param name="startDate">Starting date.</param>
        /// <param name="endDate">Ending date.</param>
        /// <returns>List of summary details</returns>
        public List<SummaryDetails> FilterWithDates(DateTime startDate, DateTime endDate)
        {
            List<SummaryDetails> filteredData = new List<SummaryDetails>();
            startDate = DateTime.Parse(startDate.ToString("yyyy-MM-dd"));
            endDate = DateTime.Parse(endDate.ToString("yyyy-MM-dd"));
            foreach (Project project in Projects)
            {
                foreach (Models.Activity activity in project.Activities)
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

        /// <summary>
        /// Function to sort the records using project name.
        /// </summary>
        /// <returns>List of summary details</returns>
        public List<SummaryDetails> SortTaskByProjectName()
        {
            List<SummaryDetails> summaryDetails = GetAllTaskSummary();
            summaryDetails.Sort((element1, element2) => element1.projectName.CompareTo(element2.projectName));
            return summaryDetails;
        }

        /// <summary>
        /// Function to sort the records using task name.
        /// </summary>
        /// <returns>List of summary details</returns>
        public List<SummaryDetails> SortTaskByTaskName()
        {
            List<SummaryDetails> summaryDetails = GetAllTaskSummary();
            summaryDetails.Sort((element1, element2) => element1.taskName.CompareTo(element2.taskName));
            return summaryDetails;
        }

        /// <summary>
        /// Function to sort the records using time taken.
        /// </summary>
        /// <returns>List of Summary details</returns>
        public List<SummaryDetails> SortTaskByTimeTaken()
        {
            List<SummaryDetails> summaryDetails = GetAllTaskSummary();
            summaryDetails.Sort((element1, element2) => element1.totalTimeTaken.CompareTo(element2.totalTimeTaken));
            return summaryDetails;
        }

        /// <summary>
        /// Function to get all tasks.
        /// </summary>
        /// <returns>List of task summaries.</returns>
        private List<SummaryDetails> GetAllTaskSummary()
        {
            List<SummaryDetails> summaryDetails = new List<SummaryDetails>();
            foreach (Project project in Projects)
            {
                foreach (Models.Activity activity in project.Activities)
                {
                    if (activity.TimeStamps.Count == 0)
                    {
                        summaryDetails.Add(new SummaryDetails { projectName = project.ProjectName, taskName = activity.TaskName, timeStamp = [], totalTimeTaken = activity.TotalTimeTaken });
                        continue;
                    }
                    foreach (DateTime[] dateTime in activity.TimeStamps)
                    {
                        summaryDetails.Add(new SummaryDetails { projectName = project.ProjectName, taskName = activity.TaskName, timeStamp = dateTime, totalTimeTaken = activity.TotalTimeTaken });
                    }
                }
            }
            return summaryDetails;
        }

        /// <summary>
        /// Function to filter the records using category.
        /// </summary>
        /// <param name="projectCategory">Project category.</param>
        /// <returns>List of summary details.</returns>
        public List<SummaryDetails> FilterByCategory(ProjectCategories projectCategory)
        {
            List<SummaryDetails> filteredSummaries = new List<SummaryDetails>();
            foreach (Project project in Projects)
            {
                if (project.ProjectCategory != projectCategory)
                {
                    continue;
                }

                foreach (Models.Activity activity in project.Activities)
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