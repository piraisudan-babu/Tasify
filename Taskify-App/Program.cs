using Taskify_App;
using Taskify_App.App_Controller;
using Taskify_App.Models;
using Taskify_App.Repository;
using Taskify_App.User_Interaction;

public class Program
{
    public static void Main()
    {
        UserRepository userRepository = new UserRepository();
        ProjectRepository projectRepository = new ProjectRepository();
        ProjectService projectService = new ProjectService(projectRepository);
        AppUI appUI = new AppUI(projectService);
        AuthenticatorServices authenticatorServices = new AuthenticatorServices(userRepository,projectService);
        Authenticator authenticator = new Authenticator(authenticatorServices, appUI);
        authenticator.DisplayAuthenticationMenu();
    }
}