using Taskify_App.Enums;
using Taskify_App.Repository;
using Taskify_App.User_Interaction;
using Taskify_App.Utilities;

namespace Taskify_App
{
    /// <summary>
    /// Class <c>Authenticator</c> to hold the authentication functionalities.
    /// </summary>
    public class Authenticator
    {
        private AuthenticatorServices _authenticatorServices;
        private AppUI _appUI;
        private TaskIDRepository _taskIDRepository;

        /// <summary>
        /// Constructor to initialize the AuthenticatorService and AppUI objects.
        /// </summary>
        /// <param name="authenticatorServices">AuthenticatorService object</param>
        /// <param name="appUI">AppUI object.</param>
        public Authenticator(AuthenticatorServices authenticatorServices, AppUI appUI, TaskIDRepository taskIDRepository)
        {
            _authenticatorServices = authenticatorServices;
            _appUI = appUI;
            _taskIDRepository = taskIDRepository;
            Utils.taskID = _taskIDRepository.GetTaskID();
        }

        /// <summary>
        /// Function to get the user choice on authentication.
        /// </summary>
        public void DisplayAuthenticationMenu()
        {
            while (true)
            {
                Utils.ClearConsole();
                if (Enum.TryParse(Utils.GetUserChoice(new[] { Constants.Login, Constants.Register, Constants.Exit },
                Constants.SelectAuthenticationChoice), true, out AuthenticationChoice authenticationChoice))
                {
                    switch (authenticationChoice)
                    {
                        case AuthenticationChoice.Login:
                            if (LoginUser())
                            {
                                Utils.DisplayInConsole(Environment.NewLine + Constants.LoginMessage + Environment.NewLine, ConsoleColor.Yellow);
                                Utils.WaitForUserInput();
                                _appUI.DisplayOperations();
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.TryAgainMessage + Environment.NewLine, ConsoleColor.Red);
                            }
                            break;

                        case AuthenticationChoice.Register:
                            if (RegisterUser())
                            {
                                Utils.DisplayInConsole(Environment.NewLine + Constants.RegisterMessage + Environment.NewLine, ConsoleColor.Yellow);
                                Utils.WaitForUserInput();
                                _appUI.DisplayOperations();
                            }
                            else
                            {
                                Utils.DisplayInConsole(Constants.TryAgainMessage + Environment.NewLine, ConsoleColor.Red);
                            }
                            break;

                        case AuthenticationChoice.Exit:
                            Utils.DisplayInConsole(Constants.ExitMessage + Environment.NewLine, ConsoleColor.Yellow);
                            _taskIDRepository.UpdateTaskID(Utils.taskID);
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

        /// <summary>
        /// Function to registration.
        /// </summary>
        /// <returns>True if registration successful else false.</returns>
        private bool RegisterUser()
        {
            string userName = Utils.GetUserName();
            string emailID = Utils.GetEmailID();
            if (_authenticatorServices.IsEmailIDFound(emailID))
            {
                Utils.DisplayInConsole(Constants.EmailFound + Environment.NewLine, ConsoleColor.Red);
                return false;
            }
            string userPassword = Utils.GetPassword();
            _authenticatorServices.AddUser(userName, emailID, userPassword);
            return true;
        }

        /// <summary>
        /// Function to login user.
        /// </summary>
        /// <returns>True if login successful else false.</returns>
        private bool LoginUser()
        {
            string emailID = Utils.GetEmailID();
            if (!_authenticatorServices.IsEmailIDFound(emailID))
            {
                Utils.DisplayInConsole(Constants.EmailNotFound + Environment.NewLine, ConsoleColor.Red);
                return false;
            }
            string userPassword = Utils.GetPasswordForLogin();
            if (_authenticatorServices.IsLoginSuccessful(emailID, userPassword))
            {
                return true;
            }
            Utils.DisplayInConsole(Environment.NewLine + Constants.IncorrectPassword + Environment.NewLine, ConsoleColor.Red);
            return false;
        }
    }
}
