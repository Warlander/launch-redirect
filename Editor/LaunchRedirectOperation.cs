using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Warlogic.LaunchIntercept;

namespace Warlogic.LaunchRedirect
{
    internal sealed class LaunchRedirectOperation : ILaunchOperation
    {
        public void Launch()
        {
            bool ignoreBlacklist = LaunchRedirectSettings.IgnoreBlacklistOnNextLaunch;
            LaunchRedirectSettings.IgnoreBlacklistOnNextLaunch = false;

            string startupScenePath = LaunchRedirectSettings.LoadStartupScenePath();
            string currentScenePath = SceneManager.GetActiveScene().path;
            bool redirect = LaunchRedirectSettings.IsEnabled() &&
                            !string.IsNullOrEmpty(startupScenePath) &&
                            !string.IsNullOrEmpty(currentScenePath) &&
                            currentScenePath != startupScenePath &&
                            (!LaunchRedirectSettings.IsSceneExcluded(currentScenePath) || ignoreBlacklist);

            if (redirect)
            {
                SceneAsset startupScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(startupScenePath);
                if (startupScene == null)
                {
                    EditorSceneManager.playModeStartScene = null;
                    Debug.LogError(
                        $"Launch Redirect could not load the configured startup scene '{startupScenePath}'. " +
                        "Unity remains in Edit mode.");
                    return;
                }

                EditorSceneManager.playModeStartScene = startupScene;
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }

            EditorApplication.isPlaying = true;
        }
    }
}
