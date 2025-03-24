using Taskify_App;
using Taskify_App.App_Controller;
using Taskify_App.Repository;
using Taskify_App.User_Interaction;

/// <summary>
/// Class <c>Program</c> to hold Main function.
/// </summary>
public class Program
{
    /// <summary>
    /// Function to inject the dependency and create the objects for all classes.
    /// </summary>
    public static void Main()
    {
        UserRepository userRepository = new UserRepository();
        ProjectRepository projectRepository = new ProjectRepository();
        TaskIDRepository taskIDRepository = new TaskIDRepository();
        AppServices appService = new AppServices(projectRepository);
        AppUI appUI = new AppUI(appService);
        AuthenticatorServices authenticatorServices = new AuthenticatorServices(userRepository,appService);
        Authenticator authenticator = new Authenticator(authenticatorServices, appUI, taskIDRepository);
        authenticator.DisplayAuthenticationMenu();
    }
}