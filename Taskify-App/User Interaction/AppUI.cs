using Taskify_App.App_Controller;
using Taskify_App.Enums;
using Taskify_App.Models;
using Taskify_App.Utilities;

namespace Taskify_App.User_Interaction
{
    public class AppUI
    {
        private AppServices _projectService;
        public AppUI(AppServices projectService)
        {
            _projectService = projectService;
        }

        public void DisplayOperations()
        {
            while (true)
            {
                Utils.ClearConsole();
                DisplayDashboard();

                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Task Manager", "Summary View", "Export", "Time Tracker", "Logout" }, "Select Task Operation"),
                    true, out TaskOperations taskOperations))
                {
                    switch (taskOperations)
                    {
                        case TaskOperations.TaskManager:
                            TaskManager();
                            break;

                        case TaskOperations.SummaryView:
                            SummaryView();
                            break;

                        case TaskOperations.Export:
                            ExportOptions();
                            break;

                        case TaskOperations.TimeTracker:
                            TimeTracker();
                            break;

                        case TaskOperations.Logout:
                            Utils.DisplayInConsole(Constants.LogoutMessage, ConsoleColor.Magenta);
                            _projectService.UpdateProjects();
                            return;

                        default:
                            Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
                            break;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
                }
            }
        }

        private void TaskManager()
        {
            while (true)
            {
                Utils.ClearConsole();
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Select Project", "Create Project", "Exit" }, "Project Choices"), true, out ProjectChoices projectChoices))
                {
                    switch(projectChoices)
                    {
                        case ProjectChoices.CreateProject:
                            Project? newProject = GetProjectDetails();
                            if (newProject == null)
                            {
                                Utils.DisplayInConsole(Constants.ProjectExistMessage, ConsoleColor.Red);
                                return;
                            }
                            Utils.DisplayInConsole(Constants.ProjectCreatedMessage, ConsoleColor.Cyan);
                            TaskManagement();
                            break;

                        case ProjectChoices.SelectProject:
                            if (!SelectProject())
                            {
                                Utils.DisplayInConsole(Constants.ExitMessage, ConsoleColor.Magenta);
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.SwitchProjectMessage, ConsoleColor.Cyan);
                                TaskManagement();
                            }
                            break;

                        case ProjectChoices.Exit:
                            return;

                        default:
                            Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
                            break;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
                }
            }
        }

        private Project? GetProjectDetails()
        {
            string projectName = Utils.GetTaskName("Project");
            if (_projectService.IsProjectExist(projectName))
            {
                return null;
            }
            ProjectCategories projectCategory = Utils.GetProjectCategory(projectName);
            Utils.DisplayInConsole("Project Category : " + projectCategory, ConsoleColor.White);
            string projectDescription = Utils.GetDescription(projectName);
            Project newProject = new Project(projectName, projectCategory, projectDescription, new List<Activity>());
            _projectService.CreateProject(newProject);
            _projectService.SetTasks(projectName);
            return newProject;
        }

        private bool SelectProject()
        {
            Utils.ClearConsole();
            List<string> avalaibleProjects = _projectService.GetProjectsName().ToList();
            avalaibleProjects.Add(Constants.Exit);
            string selectedProject = Utils.GetUserChoice(avalaibleProjects.ToArray(), "Select Project");
            if (selectedProject == Constants.Exit)
            {
                return false;
            }
            _projectService.SetTasks(selectedProject);
            return true;
        }

        private bool? SelectTask()
        {
            Utils.ClearConsole();
            List<string> availableTasks = _projectService.GetActivitiesName().ToList();
            availableTasks.Add(Constants.Exit);
            string selectedTask = Utils.GetUserChoice(availableTasks.ToArray(), "Select Task");
            if (selectedTask == Constants.Exit)
            {
                return false;
            }
            if (!_projectService.SetCurrentTask(selectedTask))
            {
                return null;
            }
            return true;
        }

        private void TaskManagement()
        {
            Utils.ClearConsole();
            while (true)
            {
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Add", "Edit", "Delete", "Exit" }, "Select option for Task Management."),
                    true, out TaskManagementChoice taskManagementChoice))
                {
                    switch (taskManagementChoice)
                    {
                        case TaskManagementChoice.Add:
                            if (AddTask())
                            {
                                Utils.DisplayInConsole(Constants.TaskAddedMessage, ConsoleColor.Green);
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.TaskExistMessage, ConsoleColor.Red);
                            }
                            break;

                        case TaskManagementChoice.Edit:
                            if (EditTask())
                            {
                                Utils.DisplayInConsole(Constants.TaskEditMessage, ConsoleColor.Green);
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.ExitMessage, ConsoleColor.Magenta);
                            }
                            break;

                        case TaskManagementChoice.Delete:
                            if (DeleteTask())
                            {
                                Utils.DisplayInConsole(Constants.TaskDeleteMessage, ConsoleColor.Cyan);
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.ExitMessage, ConsoleColor.Red);
                            }
                            break;

                        case TaskManagementChoice.Exit:
                            _projectService.MergeTaskWithProject();
                            return;

                        default:
                            Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
                            break;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
                }
            }
        }

        private bool AddTask()
        {
            Utils.ClearConsole();
            string activityName = Utils.GetTaskName("Activity Name");
            if (_projectService.IsActivityFound(activityName))
            {
                return false;
            }
            string activityDescription = Utils.GetDescription("Activity Name");
            int activityTimeLimit = Utils.GetTimeLimit();
            _projectService.AddActivity(new Activity(activityName, activityDescription, new List<DateTime[]>(), activityTimeLimit));
            return true;
        }

        private bool DeleteTask()
        {
            Utils.ClearConsole();
            List<string> activitiesName = _projectService.GetActivitiesName();
            activitiesName.Add(Constants.Exit);
            string activityToBeDeleted = Utils.GetUserChoice(activitiesName.ToArray(), "Choose the activity to be deleted");
            if (activityToBeDeleted == Constants.Exit)
            {
                return false;
            }
            if (ConfirmDelete())
            {
                _projectService.RemoveActivity(activityToBeDeleted);
                return true;
            }
            return false;
        }

        private bool EditTask()
        {
            Utils.ClearConsole();
            List<string> activitiesName = _projectService.GetActivitiesName();
            activitiesName.Add(Constants.Exit);
            string activityToBeEdited = Utils.GetUserChoice(activitiesName.ToArray(), "Choose the activity to be Edited");
            if (activityToBeEdited == Constants.Exit)
            {
                return false;
            }
            string activityNameToBeUpdated = Utils.GetActivityNameToUpdate();
            string activityDescriptionToUpdated = Utils.GetActivityDescriptionToUpdate();
            int activityTimeLimitToBeUpdated = Utils.GetActivityTimeLimitToUpdate();
            _projectService.UpdateActivity(activityToBeEdited, activityToBeEdited, activityDescriptionToUpdated, activityTimeLimitToBeUpdated);
            return true;
        }

        private bool ConfirmDelete()
        {
            Utils.ClearConsole();
            string[] confirmationChoice = new string[] { "Yes", "No" };
            if (Utils.GetUserChoice(confirmationChoice, "Are you sure to delete") == "Yes")
            {
                return true;
            }
            return false;
        }

        private void TimeTracker()
        {
            Utils.ClearConsole();
            if (!SelectProject())
            {
                return;
            }

            bool? isTaskSelected = SelectTask();
            if (isTaskSelected == null)
            {
                Utils.DisplayInConsole($"Already - {_projectService.GetCurrentProject()} is running in background Pls stop it to continue.", ConsoleColor.Green );
            }
            else if (isTaskSelected == true)
            {
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Start", "Stop", "Exit" }, "Select Status of the ."),
                    true, out ActivityStatus activityStatus))
                {
                    switch (activityStatus)
                    {
                        case ActivityStatus.Start:
                            _projectService.SetStartTime();
                            break;

                        case ActivityStatus.Stop:
                            if (!_projectService.SetStopTime())
                            {
                                Utils.DisplayInConsole(Constants.InvalidStopMessage, ConsoleColor.Green );
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.TimerStopMessage, ConsoleColor.Green);
                            }
                            break;

                        case ActivityStatus.Exit:
                            return;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
                }
            }
            else
            {
                Utils.DisplayInConsole(Constants.ExitMessage, ConsoleColor.Magenta);
            }
            Utils.WaitForUserInput();
        }

        private void DisplayDashboard()
        {
            var recentActivities = _projectService.GetRecentActivities();
            if (recentActivities == null || recentActivities.Count == 0)
            {
                Utils.DisplayInConsole("No Recent Activities Found", ConsoleColor.Green);
                return;
            }
            foreach(var activity in recentActivities)
            {
                if (activity == null)
                {
                    continue;
                }
                Utils.DisplayInConsole($"Activity Name : {activity.TaskName} {Environment.NewLine}" +
                                       $"Activity Description : {activity.TaskDescription}{Environment.NewLine}" +
                                       $"Activity Total time : {activity.TotalTimeTaken} {Environment.NewLine}" +
                                       $"{Environment.NewLine}Activity time stamps : ", ConsoleColor.Green);
                DisplayTimeStamps(activity);
            }
        }

        private void DisplayTimeStamps(Models.Activity activity)
        {
            foreach (var timeStamp in activity.TimeStamps)
            {
                Utils.DisplayInConsole($"Start : {timeStamp[0]} | Stop : {timeStamp[1]}", ConsoleColor.Green);
            }
            Utils.DisplayInConsole(Environment.NewLine, ConsoleColor.White);
        }

        private void ExportOptions()
        {
            if (Enum.TryParse(Utils.GetUserChoice(new[] { "Project", "All", "Exit" }, "Select what to export."),
                    true, out ExportChoices exportChoices))
            {
                List<string> projects = _projectService.GetProjectsName();
                projects.Add(Constants.Exit);

                switch (exportChoices)
                {
                    case ExportChoices.Project:
                        string selectedProject = Utils.GetUserChoice(projects.ToArray(), "Select a project.");
                        if (selectedProject == Constants.Exit)
                        {
                            break;
                        }
                        _projectService.ExportTasks(selectedProject);
                        break;

                    case ExportChoices.All:
                        _projectService.ExportAll();
                        break;

                    case ExportChoices.Exit:
                        return;
                }
            }
            else
            {
                Utils.DisplayInConsole(Constants.InvalidChoice, ConsoleColor.Red);
            }
        }

        private void SummaryView()
        {
            if (Enum.TryParse(Utils.GetUserChoice(new[] { "Today", "Weekly", "Monthly", "Sorted View", "Filter", "Exit" }, "Select how You want to view"), true, out SummaryViewChoice summaryViewChoice))
            {
                List<SummaryDetails> summaries;
                switch (summaryViewChoice)
                {
                    case SummaryViewChoice.Today:
                        summaries = _projectService.FilterWithDates(DateTime.Today, DateTime.Today);
                        DisplaySummary(summaries, "today");
                        Utils.WaitForUserInput();
                        break;

                    case SummaryViewChoice.Weekly:
                        summaries = _projectService.FilterWithDates(DateTime.Now.AddDays(-7), DateTime.Now);
                        DisplaySummary(summaries, "Weekly");
                        Utils.WaitForUserInput();
                        break;

                    case SummaryViewChoice .Monthly:
                        summaries = _projectService.FilterWithDates(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now);
                        DisplaySummary(summaries, "Monthly");
                        Utils.WaitForUserInput();
                        break;

                    case SummaryViewChoice.SortedView:
                        SortedView();
                        Utils.WaitForUserInput();
                        break;

                    case SummaryViewChoice.Filter:
                        DisplayFilterChoice();
                        break;

                    case SummaryViewChoice.Exit:
                        return;

                    default:
                        Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
                        break;
                }
            }
            else
            {
                Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
            }
        }

        private void DisplaySummary(List<SummaryDetails> summaryDetails, string type)
        {
            if (summaryDetails.Count == 0)
            {
                Utils.DisplayInConsole($"No Task found {type}", ConsoleColor.Red);
                return;
            }

            Utils.DisplayInConsole($"Summary of {type} activity", ConsoleColor.Magenta);
            for (int i = 0; i < summaryDetails.Count; i++)
            {
                Utils.DisplayInConsole("Project Name : " + summaryDetails[i].projectName, ConsoleColor.White);
                Utils.DisplayInConsole("Task Name : " + summaryDetails[i].taskName, ConsoleColor.White);
                Utils.DisplayInConsole($"Task Start DateTime : {summaryDetails[i].timeStamp[0]}, Task End DateTime : {summaryDetails[i].timeStamp[1]}", ConsoleColor.Blue);
                Utils.DisplayInConsole("Task Total time taken : " + summaryDetails[i].totalTimeTaken, ConsoleColor.White);
            }
        }

        private void SortedView()
        {
            if (Enum.TryParse(Utils.GetUserChoice(new[] {"Project Name", "Task Name", "Task Duration", "Exit"}, "Select the sorted order to display"), true, out SortedViewChoices sortedViewChoices))
            {
                List<SummaryDetails> summaries;
                switch (sortedViewChoices)
                {
                    case SortedViewChoices.ProjectName:
                        summaries = _projectService.SortTaskByProjectName();
                        DisplaySummary(summaries, "Sorted By Project Name");
                        break;

                    case SortedViewChoices.TaskName:
                        summaries = _projectService.SortTaskByTaskName();
                        DisplaySummary(summaries, "Sorted By Task Name");
                        break;

                    case SortedViewChoices.TaskDuration:
                        summaries = _projectService.SortTaskByTimeTaken();
                        DisplaySummary(summaries, "Sorted By Time Taken");
                        break;

                    case SortedViewChoices.Exit:
                        return;

                    default:
                        Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
                        break;
                }
            }
            else
            {
                Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
            }
        }

        private void DisplayFilterChoice()
        {
            if (Enum.TryParse(Utils.GetUserChoice(new[] {"Project Category", "Running Task", "Exit"}, "Select a Filter Option"), true, out FilterChoices filterChoices))
            {
                List<SummaryDetails> summaries;
                switch (filterChoices)
                {
                    case FilterChoices.ProjectCategory:
                        if (Enum.TryParse(Utils.GetUserChoice(new[] { "Personal", "Company" }, "Select the project for filtering"), true, out ProjectCategories projectCategories))
                        {
                            switch (projectCategories)
                            {
                                case ProjectCategories.Personal:
                                    summaries = _projectService.FilterByCategory(ProjectCategories.Personal);
                                    DisplaySummary(summaries, "Filter by project Category.");
                                    break;

                                case ProjectCategories.Company:
                                    summaries = _projectService.FilterByCategory(ProjectCategories.Company);
                                    DisplaySummary(summaries, "Filter by project Category.");
                                    break;

                                default:
                                    Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
                                    break;
                            }
                        }
                        else
                        {
                            Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
                        }
                        break;

                    case FilterChoices.RunningTask:
                        Utils.DisplayInConsole($"Current running project - {_projectService.GetCurrentTask}, Current running task - {_projectService.GetCurrentTask}", ConsoleColor.White);
                        break;

                    case FilterChoices.Exit:
                        return;

                    default:
                        Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
                        break;
                }
            }
            else
            {
                Utils.DisplayInConsole("Invalid Choice", ConsoleColor.Red);
            }
        }
    }
}
