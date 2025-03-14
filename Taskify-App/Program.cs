using Taskify_App;
using Taskify_App.Models;
using Taskify_App.Repository;

public class Program
{
    public static void Main()
    {
        UserRepository userRepository = new UserRepository();
        AuthenticatorServices authenticatorServices = new AuthenticatorServices(userRepository);
        Authenticator authenticator = new Authenticator(authenticatorServices);
        authenticator.DisplayAuthenticationMenu();
    }
}