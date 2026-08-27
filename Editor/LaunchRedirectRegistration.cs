using UnityEditor;
using Warlogic.LaunchIntercept;

namespace Warlogic.LaunchRedirect
{
    public static class LaunchRedirectRegistration
    {
        private const string RegistrationId = "warlogic.launch-redirect";

        [InitializeOnLoadMethod]
        private static void Register()
        {
            LaunchInterceptRegistration.ReplaceLaunchOperation(RegistrationId, new LaunchRedirectOperation());
        }
    }
}
