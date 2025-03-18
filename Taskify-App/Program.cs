using Taskify_App;
using Taskify_App.App_Controller;
using Taskify_App.Repository;
using Taskify_App.User_Interaction;

public class Program
{
    public static void Main()
    {
        UserRepository userRepository = new UserRepository();
        ProjectRepository projectRepository = new ProjectRepository();
        AppServices appService = new AppServices(projectRepository);
        AppUI appUI = new AppUI(appService);
        AuthenticatorServices authenticatorServices = new AuthenticatorServices(userRepository,appService);
        Authenticator authenticator = new Authenticator(authenticatorServices, appUI);
        authenticator.DisplayAuthenticationMenu();
    }
}