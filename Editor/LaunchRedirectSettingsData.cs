using System;

namespace Warlogic.LaunchRedirect
{
    [Serializable]
    internal sealed class LaunchRedirectSettingsData
    {
        public bool enabled = true;
        public string startupScenePath = "";
        public bool redirectOnEmptyScene = false;
        public string[] excludedScenes = Array.Empty<string>();
    }
}
