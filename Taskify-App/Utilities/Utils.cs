using System.Net.Mail;
using Spectre.Console;
public static class Utils
{
    public static void DisplayInConsole(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static string GetUserChoice(string[] choices, string heading)
    {
        var userOption = AnsiConsole.Prompt(
         new SelectionPrompt<string>()
        .Title(heading)
        .PageSize(50)
        .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
        .AddChoices(choices));
        return userOption;
    }

    public static string GetUserName()
    {
        while (true)
        {
            Console.Write("Enter the User name: ");
            string? userName = Console.ReadLine();
            if (userName != null && userName.Trim() != "")
            {
                return userName;
            }
            Console.WriteLine("Invalid User Name!! User name can't be empty.");
        }
    }

    public static string GetEmailID()
    {
        while (true)
        {
            Console.Write("Enter Email ID : ");
            string emailID = Console.ReadLine()!;
            if (IsValidEmailID(emailID))
            {
                return emailID;
            }
            Console.WriteLine("Invalid Email ID!!");
        }
    }

    public static string GetPassword()
    {
        while (true)
        {
            Console.Write("Enter the password : ");
            string password = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            if (IsValidPassword(password))
            {
                return password;
            }

            Console.WriteLine("Invalid Password");
        }
    }

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
}