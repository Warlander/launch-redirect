# Launch Redirect

Launch Redirect replaces Launch Intercept's default launch operation and uses Unity's Play Mode start scene to launch from the configured startup scene. The scene open for editing remains unchanged. Unsaved scenes enter Play without redirection unless **Redirect on Empty Scene** is enabled.

# Installation

## Via Git URL

Open **Window → Package Manager**, click **+**, and choose **Add package from git URL**.

To install the latest version:
```
https://github.com/Warlander/launch-redirect.git
```

To install a specific release, append the tag:
```
https://github.com/Warlander/launch-redirect.git#2.0.0
```

## Via Registry Browser

If you have [Registry Browser](https://github.com/Warlander/registry-browser) in the project, make sure you have tracked registry added:

```
Scope Prefix: com.warlogic
Registry URL: https://upm.maciejcyranowicz.com
```

Then open **Window > Warlander > Registry Browser** and add `com.warlogic.launchredirect` to the project.

## Via Scoped Registry

Add the Warlogic registry to your `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "Warlogic",
      "url": "https://upm.maciejcyranowicz.com",
      "scopes": ["com.warlogic"]
    }
  ],
  "dependencies": {
    "com.warlogic.launchredirect": "2.1.0"
  }
}
```

Then open **Window > Package Manager** and look for `com.warlogic.launchredirect`.

# Setup

1. Open **Edit → Project Settings → Launch Redirect**.
2. Enable the **Enable Redirect** toggle to turn redirection on.
3. Assign the **Startup Scene** field to the scene that should always run first (e.g. a loading/bootstrapper scene).
4. *(Optional)* Enable **Redirect on Empty Scene** to also redirect when pressing Play from an untitled or unsaved scene (e.g. straight after opening Unity).
5. *(Optional)* Add scenes or directories to **Excluded Scenes** to prevent redirect from triggering on them.

The setting is stored in `ProjectSettings/LaunchRedirectSettings.json` and should be committed to source control so all team members share the same startup scene.

# Usage

When you press Play, Launch Intercept runs registered preparation operations in priority order. Launch Redirect then:

1. Checks whether the current scene should redirect.
2. Configures Unity's `playModeStartScene` when redirection applies.
3. Enters Play Mode.

The editing scene is never replaced, so Unity returns to it automatically when Play Mode exits. If the configured startup scene cannot be loaded, Launch Redirect logs an error and remains in Edit mode.

Redirect can be temporarily disabled with the **Enable Redirect** toggle, or bypassed per-scene (or per-directory) using **Excluded Scenes**. Empty-scene bypass can be lifted with **Redirect on Empty Scene**.

No code changes are required — the redirect is driven entirely by the Project Settings entry.
