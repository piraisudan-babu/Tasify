using Taskify_App.App_Controller;
using Taskify_App.Enums;
using Taskify_App.Models;
using Taskify_App.Utilities;

namespace Taskify_App.User_Interaction
{
    /// <summary>
    /// Class <c>AppUI</c> to hold the app user interactions functionalities.
    /// </summary>
    public class AppUI
    {
        private AppServices _appService;

        /// <summary>
        /// Constructor to initialize the AppService class object.
        /// </summary>
        /// <param name="appService">AppService class object.</param>
        public AppUI(AppServices appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Function to get the user choice for main menu.
        /// </summary>
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
                            ProjectManager();
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
                            _appService.SetStopTime();
                            _appService.UpdateProjects();
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
                Utils.WaitForUserInput();
            }
        }

        /// <summary>
        /// Function to get the user choice on task manager.
        /// </summary>
        private void ProjectManager()
        {
            while (true)
            {
                Utils.ClearConsole();
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Select Project", "Create Project", "Delete Project", "Exit" }, "Project Choices"), true, out ProjectChoices projectChoices))
                {
                    switch(projectChoices)
                    {
                        case ProjectChoices.CreateProject:
                            Project? newProject = GetProjectDetails();
                            if (newProject == null)
                            {
                                Utils.DisplayInConsole(Constants.ProjectExistMessage, ConsoleColor.Red);
                                Utils.WaitForUserInput();
                                return;
                            }
                            Utils.DisplayInConsole(Constants.ProjectCreatedMessage, ConsoleColor.Cyan);
                            Utils.WaitForUserInput();
                            TaskManager(newProject.ProjectName);
                            break;

                        case ProjectChoices.SelectProject:
                            string? selectedProject = SelectProject();
                            if (selectedProject == null)
                            {
                                Utils.DisplayInConsole("No Project found!!", ConsoleColor.Red);
                                return;
                            }
                            if (selectedProject == string.Empty)
                            {
                                Utils.DisplayInConsole(Constants.ExitMessage, ConsoleColor.Magenta);
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.SwitchProjectMessage, ConsoleColor.Cyan);
                                TaskManager(selectedProject);
                            }
                            break;

                        case ProjectChoices.DeleteProject:
                            string? projectToBeDeleted = SelectProject();
                            if (projectToBeDeleted == null)
                            {
                                Utils.DisplayInConsole("No project found!!", ConsoleColor.Red);
                                Utils.WaitForUserInput();
                                return;
                            }
                            if (projectToBeDeleted == string.Empty)
                            {
                                Utils.DisplayInConsole(Constants.ExitMessage, ConsoleColor.Magenta);
                            }
                            else
                            {
                                if (Utils.GetUserChoice(new[] { "Yes", "No" }, "Confirm delete") == "Yes")
                                {
                                    _appService.RemoveProject(projectToBeDeleted);
                                    Utils.DisplayInConsole("Project deleted successfully...", ConsoleColor.Magenta);
                                    Utils.WaitForUserInput();
                                }
                                else
                                {
                                    break;
                                }
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

        /// <summary>
        /// Function to create a project by user input.
        /// </summary>
        /// <returns>Project created.</returns>
        private Project? GetProjectDetails()
        {
            string projectName = Utils.GetTaskName("Project");
            if (_appService.IsProjectExist(projectName))
            {
                return null;
            }
            ProjectCategories projectCategory = Utils.GetProjectCategory(projectName);
            Utils.DisplayInConsole("Project Category : " + projectCategory + Environment.NewLine, ConsoleColor.White);
            string projectDescription = Utils.GetDescription(projectName);
            Project newProject = new Project(projectName, projectCategory, projectDescription, new List<Activity>());
            _appService.CreateProject(newProject);
            _appService.SetTasks(projectName);
            return newProject;
        }

        /// <summary>
        /// Function to display projects to user and make them to select one.
        /// </summary>
        /// <returns>True if project selected else False.</returns>
        private string? SelectProject()
        {
            Utils.ClearConsole();
            List<string> availableProjects = _appService.GetProjectsName().ToList();
            if (availableProjects.Count == 0)
            {
                return null; 
            }
            availableProjects.Add(Constants.Exit);
            string selectedProject = Utils.GetUserChoice(availableProjects.ToArray(), "Select Project");
            if (selectedProject == Constants.Exit)
            {
                return string.Empty;
            }
            _appService.SetTasks(selectedProject);
            return selectedProject;
        }

        /// <summary>
        /// Function to list out the tasks and get the user choice for it.
        /// </summary>
        /// <returns>True if any task is selected else false.</returns>
        private bool? SelectTask()
        {
            Utils.ClearConsole();
            string[] availableTasks = _appService.GetActivitiesDetails();
            if (availableTasks.Length == 0)
            {
                return false;
            }
            string selectedTask = Utils.GetUserChoice(availableTasks, "Select Task").Split(",")[0].Trim();
            if (selectedTask == Constants.Exit)
            {
                return false;
            }
            if (!_appService.SetCurrentTask(long.Parse(selectedTask)))
            {
                return null;
            }
            return true;
        }

        /// <summary>
        /// Function to display the task choices to user and make them to perform operations.
        /// </summary>
        private void TaskManager(string projectName)
        {
            while (true)
            {
                Utils.ClearConsole();
                Utils.DisplayInConsole($"You are in project {projectName}" + Environment.NewLine, ConsoleColor.Green);
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Add", "Edit", "Delete", "Exit" }, "Select option for Task Management."),
                    true, out TaskManagementChoice taskManagementChoice))
                {
                    switch (taskManagementChoice)
                    {
                        case TaskManagementChoice.Add:
                            AddTask();
                            Utils.DisplayInConsole(Constants.TaskAddedMessage + Environment.NewLine, ConsoleColor.Green);
                            Utils.WaitForUserInput();
                            break;

                        case TaskManagementChoice.Edit:
                            bool? editedResult = EditTask();
                            if (editedResult == null)
                            {
                                Utils.DisplayInConsole("No Projects available!!" + Environment.NewLine, ConsoleColor.Red);
                            }
                            else if (editedResult == true)
                            {
                                Utils.DisplayInConsole(Constants.TaskEditMessage + Environment.NewLine, ConsoleColor.Green);
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.ExitMessage + Environment.NewLine, ConsoleColor.Magenta);
                            }
                            Utils.WaitForUserInput();
                            break;

                        case TaskManagementChoice.Delete:
                            bool? deletedResult = DeleteTask();
                            if (deletedResult == null)
                            {
                                Utils.DisplayInConsole("No project available" + Environment.NewLine, ConsoleColor.Red);
                            }
                            else if (deletedResult == true)
                            {
                                Utils.DisplayInConsole(Constants.TaskDeleteMessage + Environment.NewLine, ConsoleColor.Cyan);
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.ExitMessage + Environment.NewLine, ConsoleColor.Red);
                            }
                            Utils.WaitForUserInput();
                            break;

                        case TaskManagementChoice.Exit:
                            _appService.MergeTaskWithProject();
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

        /// <summary>
        /// Function to add the task.
        /// </summary>
        /// <returns>True if added else false.</returns>
        private void AddTask()
        {
            Utils.ClearConsole();
            string activityName = Utils.GetTaskName("Activity");
            string activityDescription = Utils.GetDescription("Activity");
            float activityTimeLimit = Utils.GetTimeLimit();
            _appService.AddActivity(new Activity(Utils.taskID++, activityName, activityDescription, new List<DateTime[]>(), activityTimeLimit));
        }

        private bool? DeleteTask()
        {
            Utils.ClearConsole();
            string[] activitiesName = _appService.GetActivitiesDetails();
            if (activitiesName.Length == 0)
            {
                return null;
            } 
            string activityToBeDeleted = Utils.GetUserChoice(activitiesName.ToArray(), "Choose the activity to be deleted").Split(",")[0].Trim();
            if (activityToBeDeleted == Constants.Exit)
            {
                return false;
            }
            if (ConfirmDelete())
            {
                _appService.RemoveActivity(long.Parse(activityToBeDeleted));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function to edit task.
        /// </summary>
        /// <returns>True if edited successfully else false.</returns>
        private bool? EditTask()
        {
            Utils.ClearConsole();
            string[] activitiesName = _appService.GetActivitiesDetails();
            if (activitiesName.Length == 0)
            {
                return null;
            }
            string activityToBeEdited = Utils.GetUserChoice(activitiesName.ToArray(), "Choose the activity to be Edited").Split(",")[0].Trim();
            if (activityToBeEdited == Constants.Exit)
            {
                return false;
            }
            string activityNameToBeUpdated = Utils.GetActivityNameToUpdate();
            string activityDescriptionToUpdated = Utils.GetActivityDescriptionToUpdate();
            float activityTimeLimitToBeUpdated = Utils.GetActivityTimeLimitToUpdate();
            _appService.UpdateActivity(long.Parse(activityToBeEdited), activityToBeEdited, activityDescriptionToUpdated, activityTimeLimitToBeUpdated);
            return true;
        }

        /// <summary>
        /// Function to confirm the delete operation.
        /// </summary>
        /// <returns>True if yes else false</returns>
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

        /// <summary>
        /// Function to get user choice on timer.
        /// </summary>
        private void TimeTracker()
        {
            Utils.ClearConsole();
            string? isSelected = SelectProject();
            if (isSelected == null)
            {
                Utils.DisplayInConsole("No Project Found", ConsoleColor.Red);
                return;
            }
            if (isSelected == string.Empty)
            {
                return;
            }

            bool? isTaskSelected = SelectTask();
            if (isTaskSelected == null)
            {
                Utils.DisplayInConsole($"Already - {_appService.runningProject} project - {_appService.GetCurrentTask()} task is running in background Pls stop it to continue." + Environment.NewLine, ConsoleColor.Green );
            }
            else if (isTaskSelected == true)
            {
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Start", "Stop", "Exit" }, "Select Status of the ."),
                    true, out ActivityStatus activityStatus))
                {
                    switch (activityStatus)
                    {
                        case ActivityStatus.Start:
                            if (!_appService.SetStartTime())
                            {
                                Utils.DisplayInConsole("Already this task is running in background" + Environment.NewLine, ConsoleColor.Red);
                                return;
                            }
                            Utils.DisplayInConsole("Task started", ConsoleColor.Cyan);
                            break;

                        case ActivityStatus.Stop:
                            if (!_appService.SetStopTime())
                            {
                                Utils.DisplayInConsole(Constants.InvalidStopMessage + Environment.NewLine, ConsoleColor.Green );
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.TimerStopMessage + Environment.NewLine, ConsoleColor.Green);
                            }
                            break;

                        case ActivityStatus.Exit:
                            return;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice + Environment.NewLine, ConsoleColor.Red);
                }
            }
            else
            {
                Utils.DisplayInConsole("No task found" + Environment.NewLine, ConsoleColor.Magenta);
            }
        }

        /// <summary>
        /// Function to display the dashboard of the current user.
        /// </summary>
        private void DisplayDashboard()
        {
            var recentActivities = _appService.GetRecentActivities();
            if (recentActivities == null || recentActivities.Count == 0)
            {
                Utils.DisplayInConsole("No Recent Activities Found" + Environment.NewLine, ConsoleColor.Green);
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

        /// <summary>
        /// Function to display the time stamps of the given activity.
        /// </summary>
        /// <param name="activity">Activity to be displayed.</param>
        private void DisplayTimeStamps(Models.Activity activity)
        {
            foreach (var timeStamp in activity.TimeStamps)
            {
                Utils.DisplayInConsole($"Start : {timeStamp[0]} | Stop : {timeStamp[1]}" + Environment.NewLine, ConsoleColor.Green);
            }
            Utils.DisplayInConsole(Environment.NewLine, ConsoleColor.White);
        }

        /// <summary>
        /// Function to get the user choice on exporting csv file.
        /// </summary>
        private void ExportOptions()
        {
            List<string> projects = _appService.GetProjectsName();
            if (projects.Count == 0)
            {
                if (Utils.GetUserChoice(new[] {"Yes", "No"}, "No projects found!! Are you sure to continue") == "No")
                {
                    return;
                }

            }
            if (Enum.TryParse(Utils.GetUserChoice(new[] { "Project", "All", "Exit" }, "Select what to export."),
                    true, out ExportChoices exportChoices))
            {
                projects.Add(Constants.Exit);

                switch (exportChoices)
                {
                    case ExportChoices.Project:
                        string selectedProject = Utils.GetUserChoice(projects.ToArray(), "Select a project.");
                        if (selectedProject == Constants.Exit)
                        {
                            break;
                        }
                        _appService.ExportTasks(selectedProject);
                        Utils.DisplayInConsole("Exported successfully", ConsoleColor.Yellow);
                        break;

                    case ExportChoices.All:
                        _appService.ExportAll();
                        Utils.DisplayInConsole("Exported successfully!!", ConsoleColor.Yellow);
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

        /// <summary>
        /// Function to get summary view choices.
        /// </summary>
        private void SummaryView()
        {
            while (true)
            {
                Utils.ClearConsole();
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Today", "Weekly", "Monthly", "Sorted View", "Filter", "Exit" }, "Select how You want to view"), true, out SummaryViewChoice summaryViewChoice))
                {
                    List<SummaryDetails> summaries;
                    switch (summaryViewChoice)
                    {
                        case SummaryViewChoice.Today:
                            summaries = _appService.FilterWithDates(DateTime.Today, DateTime.Today);
                            DisplaySummary(summaries, "today");
                            Utils.WaitForUserInput();
                            break;

                        case SummaryViewChoice.Weekly:
                            summaries = _appService.FilterWithDates(DateTime.Now.AddDays(-7), DateTime.Now);
                            DisplaySummary(summaries, "Weekly");
                            Utils.WaitForUserInput();
                            break;

                        case SummaryViewChoice.Monthly:
                            summaries = _appService.FilterWithDates(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now);
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

        /// <summary>
        /// Function to display the summary.
        /// </summary>
        /// <param name="summaryDetails">List of summary list.</param>
        /// <param name="type">Type of the summary.</param>
        private void DisplaySummary(List<SummaryDetails> summaryDetails, string type)
        {
            if (summaryDetails.Count == 0)
            {
                Utils.DisplayInConsole($"No Task found {type}" + Environment.NewLine, ConsoleColor.Red);
                return;
            }

            Utils.DisplayInConsole($"Summary of {type} activity" + Environment.NewLine, ConsoleColor.Magenta);
            for (int i = 0; i < summaryDetails.Count; i++)
            {
                Utils.DisplayInConsole("Project Name : " + summaryDetails[i].projectName + Environment.NewLine, ConsoleColor.White);
                Utils.DisplayInConsole("Task Name : " + summaryDetails[i].taskName + Environment.NewLine, ConsoleColor.White);
                if (summaryDetails[i].timeStamp.Length == 0)
                {
                    Utils.DisplayInConsole("Task : []" + Environment.NewLine, ConsoleColor.Blue);
                }
                else
                {
                    Utils.DisplayInConsole($"Task Start DateTime : {summaryDetails[i].timeStamp[0]}, Task End DateTime : {summaryDetails[i].timeStamp[1]}" + Environment.NewLine, ConsoleColor.Blue);
                }
                Utils.DisplayInConsole("Task Total time taken : " + summaryDetails[i].totalTimeTaken + Environment.NewLine, ConsoleColor.White);
            }
        }

        /// <summary>
        /// Function to display the sorting choice to the users.
        /// </summary>
        private void SortedView()
        {
            if (Enum.TryParse(Utils.GetUserChoice(new[] {"Project Name", "Task Name", "Task Duration", "Exit"}, "Select the sorted order to display"), true, out SortedViewChoices sortedViewChoices))
            {
                List<SummaryDetails> summaries;
                switch (sortedViewChoices)
                {
                    case SortedViewChoices.ProjectName:
                        summaries = _appService.SortTaskByProjectName();
                        DisplaySummary(summaries, "Sorted By Project Name");
                        break;

                    case SortedViewChoices.TaskName:
                        summaries = _appService.SortTaskByTaskName();
                        DisplaySummary(summaries, "Sorted By Task Name");
                        break;

                    case SortedViewChoices.TaskDuration:
                        summaries = _appService.SortTaskByTimeTaken();
                        DisplaySummary(summaries, "Sorted By Time Taken");
                        break;

                    case SortedViewChoices.Exit:
                        return;

                    default:
                        Utils.DisplayInConsole(Constants.InvalidChoice + Environment.NewLine, ConsoleColor.Red);
                        break;
                }
            }
            else
            {
                Utils.DisplayInConsole(Constants.InvalidChoice + Environment.NewLine, ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Function to display the filtering choice to user.
        /// </summary>
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
                                    summaries = _appService.FilterByCategory(ProjectCategories.Personal);
                                    DisplaySummary(summaries, "Filter by project Category.");
                                    break;

                                case ProjectCategories.Company:
                                    summaries = _appService.FilterByCategory(ProjectCategories.Company);
                                    DisplaySummary(summaries, "Filter by project Category.");
                                    break;

                                default:
                                    Utils.DisplayInConsole(Constants.InvalidChoice + Environment.NewLine, ConsoleColor.Red);
                                    break;
                            }
                        }
                        else
                        {
                            Utils.DisplayInConsole(Constants.InvalidChoice + Environment.NewLine, ConsoleColor.Red);
                        }
                        break;

                    case FilterChoices.RunningTask:
                        long runningTask = _appService.GetCurrentTask();
                        if (runningTask == -1)
                        {
                            Utils.DisplayInConsole("No task running" + Environment.NewLine, ConsoleColor.Red);
                            break;
                        }
                        Utils.DisplayInConsole($"Current running project - {_appService.runningProject}, Current running task - {runningTask}" + Environment.NewLine, ConsoleColor.White);
                        break;

                    case FilterChoices.Exit:
                        return;

                    default:
                        Utils.DisplayInConsole(Constants.InvalidChoice + Environment.NewLine, ConsoleColor.Red);
                        break;
                }
            }
            else
            {
                Utils.DisplayInConsole(Constants.InvalidChoice + Environment.NewLine, ConsoleColor.Red);
            }
            Utils.WaitForUserInput();
        }
    }
}
