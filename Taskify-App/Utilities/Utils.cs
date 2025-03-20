using System.Net.Mail;
using Spectre.Console;
using Taskify_App.Enums;
using Taskify_App.Utilities;

/// <summary>
/// Class <c>Utils</c> is a static helper class to hold the validation, User inputs.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Function to display the message in console.
    /// </summary>
    /// <param name="message">Message to be displayed in console.</param>
    /// <param name="color">Color of the message.</param>
    public static void DisplayInConsole(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    /// <summary>
    /// Function to get user choice in a drop down format.
    /// </summary>
    /// <param name="choices">Choices to be displayed.</param>
    /// <param name="heading">Heading of the choice.</param>
    /// <returns>Choice selected by user.</returns>
    public static string GetUserChoice(string[] choices, string heading)
    {
        var userOption = AnsiConsole.Prompt(
         new SelectionPrompt<string>()
        .Title(heading)
        .PageSize(50)
        .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
        .AddChoices(choices));
        return userOption.Replace(" ", string.Empty);
    }

    /// <summary>
    /// Function to get the project category in form of drop down from user.
    /// </summary>
    /// <param name="heading">Heading of the drop down.</param>
    /// <returns>Project Category selected by user.</returns>
    public static ProjectCategories GetProjectCategory(string heading)
    {
        var projectCategoriesOption = AnsiConsole.Prompt(
         new SelectionPrompt<ProjectCategories>()
        .Title(heading)
        .PageSize(50)
        .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
        .AddChoices((IEnumerable<ProjectCategories>)Enum.GetValues(typeof(ProjectCategories))));
        return projectCategoriesOption;
    }

    /// <summary>
    /// Function to get the user name.
    /// </summary>
    /// <returns>User Name</returns>
    public static string GetUserName()
    {
        while (true)
        {
            DisplayInConsole("Enter the UserName : ", ConsoleColor.White);
            string? userName = Console.ReadLine();
            if (userName != null)
            {
                userName = userName.Trim();
                if (userName != string.Empty && userName.All(letter => Char.IsLetter(letter)))
                {
                    return userName;
                }
            }
            DisplayInConsole(Constants.InvalidUserName + Environment.NewLine, ConsoleColor.Red);
        }
    }

    /// <summary>
    /// Function to get the Email ID.
    /// </summary>
    /// <returns>Email ID</returns>
    public static string GetEmailID()
    {
        while (true)
        {
            DisplayInConsole("Enter Email ID : ", ConsoleColor.White);
            string? emailID = Console.ReadLine();
            if (!string.IsNullOrEmpty(emailID) && IsValidEmailID(emailID))
            {
                return emailID;
            }
            DisplayInConsole(Constants.InvalidEmailID + Environment.NewLine, ConsoleColor.White);
        }
    }

    /// <summary>
    /// Function of get the password.
    /// </summary>
    /// <returns>Password</returns>
    public static string GetPassword()
    {
        while (true)
        {
            DisplayInConsole("Enter the password : ", ConsoleColor.White);
            string password = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    DisplayInConsole("\b \b", ConsoleColor.White);
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    DisplayInConsole("*", ConsoleColor.White);
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            if (IsValidPassword(password))
            {
                return password;
            }

            DisplayInConsole(Constants.InvalidPassword + Environment.NewLine, ConsoleColor.White);
        }
    }

    /// <summary>
    /// Function to get password for login purpose.
    /// </summary>
    /// <returns>Password</returns>
    public static string GetPasswordForLogin()
    {
        DisplayInConsole("Enter the password : ", ConsoleColor.White);
        string password = string.Empty;
        ConsoleKey key;

        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
            {
                DisplayInConsole("\b \b", ConsoleColor.White);
                password = password[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                DisplayInConsole("*", ConsoleColor.White);
                password += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        return password;
    }

    /// <summary>
    /// Function to check whether the given password is valid or not.
    /// </summary>
    /// <param name="password">Password to be validated.</param>
    /// <returns>True if password is valid else false.</returns>
    private static bool IsValidPassword(string password)
    {
        if (password.Length < 8)
        {
            return false;
        }

        bool uppercase = false, lowercase = false, numeric = false, specialCharacter = false;
        foreach (char letter in password)
        {
            if (char.IsUpper(letter))
            {
                uppercase = true;
            }
            else if (char.IsLower(letter))
            {
                lowercase = true;
            }
            else if (char.IsDigit(letter))
            {
                numeric = true;
            }
            else
            {
                specialCharacter = true;
            }
        }
        return new[] { uppercase, lowercase, numeric, specialCharacter }.All(element => element);
    }

    /// <summary>
    /// Function to validate the email ID.
    /// </summary>
    /// <param name="emailID">Email ID to be validated.</param>
    /// <returns>True if email ID is valid else false.</returns>
    private static bool IsValidEmailID(string? emailID)
    {
        if (emailID == null)
        {
            return false;
        }

        if (emailID.Any(letter => char.IsUpper(letter)))
        {
            return false;
        }

        try
        {
            var mailAddress = new MailAddress(emailID);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>
    /// Function to get the task/project name.
    /// </summary>
    /// <param name="taskName">Task/Project</param>
    /// <returns>Task Name</returns>
    public static string GetTaskName(string taskName)
    {
        while (true)
        {
            DisplayInConsole($"Enter the {taskName} name: ", ConsoleColor.White);
            string? name = Console.ReadLine();
            if (name != null)
            {
                name = name.Trim();
                if (name != string.Empty && name.All(letter => char.IsLetter(letter) || char.IsDigit(letter)))
                {
                    return name;
                }
            }
            DisplayInConsole(Constants.InvalidName + Environment.NewLine, ConsoleColor.Red);
        }
    }

    /// <summary>
    /// Function to get description from user.
    /// </summary>
    /// <param name="taskName">Project/Task</param>
    /// <returns>description.</returns>
    public static string GetDescription(string taskName)
    {
        DisplayInConsole($"Enter the {taskName} Description : ", ConsoleColor.White);
        string? description = Console.ReadLine();
        if (description == null || description.Trim() == string.Empty)
        {
            return "-";
        }
        return description;
    }

    /// <summary>
    /// Function to get the time limit for the task.
    /// </summary>
    /// <returns>Time limit of the task</returns>
    public static float GetTimeLimit()
    {
        while (true)
        {
            DisplayInConsole("Enter the time limit in Hrs: ", ConsoleColor.White);
            string input = Console.ReadLine()!;
            var floatParts = input.Split(".");
            if (float.TryParse(input, out var timeLimit) && timeLimit > 0 && timeLimit < 100 && (floatParts.Length == 1 || floatParts.Length > 1 && floatParts[1].Length <= 5))
            {
                return timeLimit;
            }
            DisplayInConsole(Constants.InvalidTimeLimit + Environment.NewLine, ConsoleColor.Red);
        }
    }

    /// <summary>
    /// Function to get the activity name for editing.
    /// </summary>
    /// <returns>Activity Name.</returns>
    public static string GetActivityNameToUpdate()
    {
        DisplayInConsole("Enter the Activity name to be updated : ", ConsoleColor.White);
        string? activityName = Console.ReadLine();
        return (activityName == null) ? string.Empty : activityName;
    }

    /// <summary>
    /// Function to get the activity description for editing.
    /// </summary>
    /// <returns>Activity description.</returns>
    public static string GetActivityDescriptionToUpdate()
    {
        DisplayInConsole("Enter the Activity Description to be updated : ", ConsoleColor.White);
        string? activityDescription = Console.ReadLine();
        return (activityDescription == null) ? string.Empty : activityDescription;
    }

    /// <summary>
    /// Function to get the activity time limit for editing.
    /// </summary>
    /// <returns>activity time limit</returns>
    public static float GetActivityTimeLimitToUpdate()
    {
        while (true)
        {
            DisplayInConsole("Enter the time limit in Hrs to be update: ", ConsoleColor.White);
            string input = Console.ReadLine()!;
            var floatParts = input.Split(".");
            if (float.TryParse(input, out var timeLimit) && timeLimit > 0 && timeLimit < 100 && (floatParts.Length == 1 || floatParts.Length > 1 && floatParts[1].Length <= 5))
            {
                return timeLimit;
            }
            DisplayInConsole(Constants.InvalidTimeLimit + Environment.NewLine, ConsoleColor.Red);
        }
    }

    /// <summary>
    /// Function to clear the console.
    /// </summary>
    public static void ClearConsole()
    {
        Console.Clear();
    }

    /// <summary>
    /// Function to pause until user press any key.
    /// </summary>
    public static void WaitForUserInput()
    {
        DisplayInConsole(Environment.NewLine + Constants.ContinueMessage, ConsoleColor.Cyan);
        Console.ReadKey();
    }
}