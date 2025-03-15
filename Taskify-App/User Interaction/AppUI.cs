using Taskify_App.App_Controller;
using Taskify_App.Enums;
using Taskify_App.Models;
using Taskify_App.Utilities;

namespace Taskify_App.User_Interaction
{
    public class AppUI
    {
        private ProjectService _projectService;
        public AppUI(ProjectService projectService)
        {
            _projectService = projectService;
        }

        public void DisplayOperations()
        {
            while (true)
            {

                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Task Manager", "Summary View", "Export", "Time Tracker", "Logout" }, "Select Task Operation"),
                    true, out TaskOperations taskOperations))
                {
                    switch (taskOperations)
                    {
                        case TaskOperations.TaskManager:
                            TaskManager();
                            break;

                        case TaskOperations.SummaryView:
                            break;

                        case TaskOperations.Export:
                            break;

                        case TaskOperations.TimeTracker:
                            break;

                        case TaskOperations.Logout:
                            Utils.DisplayInConsole("Logout Successfully...", ConsoleColor.Magenta);
                            _projectService.UpdateProjects();
                            return;

                        default:
                            Utils.DisplayInConsole(Constants.InvalidChoice, Constants.RedColor);
                            break;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice, Constants.RedColor);
                }
            }
        }

        private void TaskManager()
        {
            while (true)
            {
                if (Enum.TryParse(Utils.GetUserChoice(new[] { "Select Project", "Create Project", "Exit" }, "Project Choices"), true, out ProjectChoices projectChoices))
                {
                    switch(projectChoices)
                    {
                        case ProjectChoices.CreateProject:
                            Project? newProject = GetProjectDetails();
                            if (newProject == null)
                            {
                                Utils.DisplayInConsole("Project exist already!!", ConsoleColor.Red);
                                return;
                            }
                            Utils.DisplayInConsole("Project created successfully", ConsoleColor.Cyan);
                            TaskManagement();
                            break;

                        case ProjectChoices.SelectProject:
                            if (!SelectProject())
                            {
                                Utils.DisplayInConsole("Exited successfully!!", ConsoleColor.Magenta);
                            }
                            else
                            {
                                Utils.DisplayInConsole("Switched to the selected project.", ConsoleColor.Cyan);
                                TaskManagement();
                            }
                            break;

                        case ProjectChoices.Exit:
                            Utils.DisplayInConsole("Exited", ConsoleColor.Magenta);
                            return;

                        default:
                            Utils.DisplayInConsole(Constants.InvalidChoice, Constants.RedColor);
                            break;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice, Constants.RedColor);
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
            string projectDescription = Utils.GetDescription(projectName);
            Project newProject = new Project(projectName, projectCategory, projectDescription, new List<Activity>());
            _projectService.CreateProject(newProject);
            _projectService.SetTasks(projectName);
            return newProject;
        }

        private bool SelectProject()
        {
            List<string> avalaibleProjects = _projectService.GetProjectsName().ToList();
            avalaibleProjects.Add("Exit");
            string selectedProject = Utils.GetUserChoice(avalaibleProjects.ToArray(), "Select Project");
            if (selectedProject == "Exit")
            {
                return false;
            }
            _projectService.SetTasks(selectedProject);
            return true;
        }

        private void TaskManagement()
        {
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
                                Utils.DisplayInConsole("Task Added Successfully.", ConsoleColor.Green);
                            }
                            else
                            {
                                Utils.DisplayInConsole("Task Name already exist!! Pls Edit if necessary.", ConsoleColor.Red);
                            }
                            break;

                        case TaskManagementChoice.Edit:
                            if (EditTask())
                            {
                                Utils.DisplayInConsole("Edited successfully", ConsoleColor.Green);
                            }
                            else
                            {
                                Utils.DisplayInConsole("Exited Successfully", ConsoleColor.Magenta);
                            }
                            break;

                        case TaskManagementChoice.Delete:
                            if (DeleteTask())
                            {
                                Utils.DisplayInConsole("Task Deleted successfully.", ConsoleColor.Cyan);
                            }
                            else
                            {
                                Utils.DisplayInConsole("Exited successfully.", ConsoleColor.Red);
                            }
                            break;

                        case TaskManagementChoice.Exit:
                            Utils.DisplayInConsole("Exited successfully", ConsoleColor.Red);
                            _projectService.MergeTaskWithProject();
                            return;

                        default:
                            Utils.DisplayInConsole(Constants.InvalidChoice, Constants.RedColor);
                            break;
                    }
                }
                else
                {
                    Utils.DisplayInConsole(Constants.InvalidChoice, Constants.RedColor);
                }
            }
        }

        private bool AddTask()
        {
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
            List<string> activitiesName = _projectService.GetActivitiesName();
            activitiesName.Add("Exit");
            string activityToBeDeleted = Utils.GetUserChoice(activitiesName.ToArray(), "Choose the activity to be deleted");
            if (activityToBeDeleted == "Exit")
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
            List<string> activitiesName = _projectService.GetActivitiesName();
            activitiesName.Add("Exit");
            string activityToBeEdited = Utils.GetUserChoice(activitiesName.ToArray(), "Choose the activity to be Edited");
            if (activityToBeEdited == "Exit")
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
            string[] confirmationChoice = new string[] { "Yes", "No" };
            if (Utils.GetUserChoice(confirmationChoice, "Are you sure to delete") == "Yes")
            {
                return true;
            }
            return false;
        }
    }
}
