using Taskify_App.Enums;
using Taskify_App.Repository;
using Taskify_App.User_Interaction;
using Taskify_App.Utilities;

namespace Taskify_App
{
    public class Authenticator
    {
        private AuthenticatorServices _authenticatorServices;
        private AppUI _appUI;
        public Authenticator(AuthenticatorServices authenticatorServices, AppUI appUI)
        {
            _authenticatorServices = authenticatorServices;
            _appUI = appUI;
        }

        public void DisplayAuthenticationMenu()
        {
            while (true)
            {
                if (Enum.TryParse(Utils.GetUserChoice(new[] { Constants.Login, Constants.Register, Constants.Exit },
                Constants.SelectAuthenticationChoice), true, out AuthenticationChoice authenticationChoice))
                {
                    switch (authenticationChoice)
                    {
                        case AuthenticationChoice.Login:
                            if (LoginUser())
                            {
                                Utils.DisplayInConsole("Login successfully!!!", Constants.YellowColor);
                                _appUI.DisplayOperations();
                            }
                            else
                            {
                                Utils.DisplayInConsole("Try again!!", Constants.RedColor);
                            }
                            break;

                        case AuthenticationChoice.Register:
                            if (RegisterUser())
                            {
                                Utils.DisplayInConsole("User register successfully!!!", Constants.YellowColor);
                                _appUI.DisplayOperations();
                            }
                            else
                            {
                                Utils.DisplayInConsole("Try again!!", Constants.RedColor);
                            }
                            break;

                        case AuthenticationChoice.Exit:
                            Utils.DisplayInConsole(Constants.ExitMessage, Constants.YellowColor);
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

        private bool RegisterUser()
        {
            string userName = Utils.GetUserName();
            string emailID = Utils.GetEmailID();
            if (_authenticatorServices.IsEmailIDFound(emailID))
            {
                Utils.DisplayInConsole("Email ID found already!!", Constants.RedColor);
                return false;
            }
            string userPassword = Utils.GetPassword();
            _authenticatorServices.AddUser(userName, emailID, userPassword);
            return true;
        }

        private bool LoginUser()
        {
            string emailID = Utils.GetEmailID();
            if (!_authenticatorServices.IsEmailIDFound(emailID))
            {
                Utils.DisplayInConsole("Email ID not found!!", Constants.RedColor);
                return false;
            }
            string userPassword = Utils.GetPassword();
            if (_authenticatorServices.IsLoginSuccessful(emailID, userPassword))
            {
                return true;
            }
            Utils.DisplayInConsole("Incorrect Password!!", Constants.RedColor);
            return false;
        }
    }
}
