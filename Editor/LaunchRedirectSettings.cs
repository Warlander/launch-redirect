using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Warlogic.LaunchRedirect
{
    public static class LaunchRedirectSettings
    {
        private const string SettingsFilePath = "ProjectSettings/LaunchRedirectSettings.json";
        private const string IgnoreBlacklistKey = "Warlander_LaunchRedirect_IgnoreBlacklist";

        public static bool IgnoreBlacklistOnNextLaunch
        {
            get => SessionState.GetBool(IgnoreBlacklistKey, false);
            set => SessionState.SetBool(IgnoreBlacklistKey, value);
        }

        public static bool IsEnabled()
        {
            LaunchRedirectSettingsData data = LoadData();
            return data != null && data.enabled;
        }

        public static string LoadStartupScenePath()
        {
            LaunchRedirectSettingsData data = LoadData();
            return string.IsNullOrEmpty(data?.startupScenePath) ? null : data.startupScenePath;
        }

        public static bool IsRedirectOnEmptySceneEnabled()
        {
            LaunchRedirectSettingsData data = LoadData();
            return data != null && data.redirectOnEmptyScene;
        }

        public static bool IsSceneExcluded(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                return false;
            }

            LaunchRedirectSettingsData data = LoadData();
            if (data?.excludedScenes == null)
            {
                return false;
            }

            foreach (string excludedPath in data.excludedScenes)
            {
                if (string.IsNullOrEmpty(excludedPath))
                {
                    continue;
                }

                if (excludedPath.Equals(scenePath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                string normalizedExcluded = excludedPath.Replace('\\', '/');
                if (!normalizedExcluded.EndsWith("/"))
                {
                    normalizedExcluded += "/";
                }

                string normalizedScene = scenePath.Replace('\\', '/');
                if (normalizedScene.StartsWith(normalizedExcluded, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static LaunchRedirectSettingsData LoadData()
        {
            if (!File.Exists(SettingsFilePath))
            {
                return null;
            }

            string json = File.ReadAllText(SettingsFilePath);
            LaunchRedirectSettingsData data = JsonUtility.FromJson<LaunchRedirectSettingsData>(json);
            if (data == null)
            {
                return null;
            }

            if (data.excludedScenes == null)
            {
                data.excludedScenes = Array.Empty<string>();
            }

            return data;
        }

        private static void SaveData(LaunchRedirectSettingsData data)
        {
            if (data.excludedScenes == null)
            {
                data.excludedScenes = Array.Empty<string>();
            }

            File.WriteAllText(SettingsFilePath, JsonUtility.ToJson(data, true));
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Launch Redirect", SettingsScope.Project)
            {
                label = "Launch Redirect",
                guiHandler = _ =>
                {
                    LaunchRedirectSettingsData data = LoadData() ?? new LaunchRedirectSettingsData();

                    EditorGUI.BeginChangeCheck();

                    data.enabled = EditorGUILayout.Toggle(
                        new GUIContent("Enable Redirect",
                            "When enabled, pressing Play will redirect to the configured startup scene."),
                        data.enabled);

                    EditorGUILayout.Space(4);

                    SceneAsset currentScene = string.IsNullOrEmpty(data.startupScenePath)
                        ? null
                        : AssetDatabase.LoadAssetAtPath<SceneAsset>(data.startupScenePath);

                    SceneAsset newScene = (SceneAsset)EditorGUILayout.ObjectField(
                        new GUIContent("Startup Scene",
                            "Scene to redirect to when pressing Play. Leave empty to disable redirect."),
                        currentScene,
                        typeof(SceneAsset),
                        false);

                    if (newScene != null)
                    {
                        data.startupScenePath = AssetDatabase.GetAssetPath(newScene);
                    }
                    else
                    {
                        data.startupScenePath = "";
                    }

                    data.redirectOnEmptyScene = EditorGUILayout.Toggle(
                        new GUIContent("Redirect on Empty Scene",
                            "When enabled, redirect also triggers from untitled or unsaved scenes (e.g. straight after opening Unity)."),
                        data.redirectOnEmptyScene);

                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Excluded Scenes", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox(
                        "Add individual scenes or directories to exclude from launch redirect. " +
                        "Scenes inside excluded directories (including nested directories) are also excluded.",
                        MessageType.Info);

                    if (data.excludedScenes == null)
                    {
                        data.excludedScenes = Array.Empty<string>();
                    }

                    int toRemove = -1;
                    for (int i = 0; i < data.excludedScenes.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        UnityEngine.Object currentObj = string.IsNullOrEmpty(data.excludedScenes[i])
                            ? null
                            : AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(data.excludedScenes[i]);

                        UnityEngine.Object newObj = EditorGUILayout.ObjectField(
                            currentObj,
                            typeof(UnityEngine.Object),
                            false);

                        if (newObj != null)
                        {
                            string path = AssetDatabase.GetAssetPath(newObj);
                            bool isValid = newObj is SceneAsset || AssetDatabase.IsValidFolder(path);
                            data.excludedScenes[i] = isValid ? path : "";
                        }
                        else
                        {
                            data.excludedScenes[i] = "";
                        }

                        if (GUILayout.Button("-", GUILayout.Width(24)))
                        {
                            toRemove = i;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (toRemove >= 0)
                    {
                        List<string> list = data.excludedScenes.ToList();
                        list.RemoveAt(toRemove);
                        data.excludedScenes = list.ToArray();
                    }

                    if (GUILayout.Button("Add Excluded Scene or Directory"))
                    {
                        List<string> list = data.excludedScenes.ToList();
                        list.Add("");
                        data.excludedScenes = list.ToArray();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        SaveData(data);
                    }
                }
            };
        }
    }
}
