# Launch Redirect

Redirects to a configured startup scene when pressing Play in the Unity Editor, so multi-scene projects always initialize from the correct entry point.

# Installation

## Via Git URL

Open **Window → Package Manager**, click **+**, and choose **Add package from git URL**.

To install the latest version:
```
https://github.com/Warlander/launch-redirect.git
```

To install a specific release, append the tag:
```
https://github.com/Warlander/launch-redirect.git#1.0.4
```

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
    "com.warlogic.launchredirect": "1.0.4"
  }
}
```

Alternatively, open **Window → Package Manager**, click **+**, choose **Add package by name**, and enter `com.warlogic.launchredirect`.

# Setup

1. Open **Edit → Project Settings → Launch Redirect**.
2. Assign the **Startup Scene** field to the scene that should always run first (e.g. a loading/bootstrapper scene).
3. Leave the field empty to disable redirection — pressing Play will behave normally.

The setting is stored in `ProjectSettings/LaunchRedirectSettings.json` and should be committed to source control so all team members share the same startup scene.

# Usage

When you press Play from any scene other than the configured startup scene, the editor will:

1. Cancel the current play request.
2. Open the startup scene.
3. Resume playing from the startup scene.

When play mode exits, the editor automatically reopens the scene you were editing before the redirect.

No code changes are required — the redirect is driven entirely by the Project Settings entry.
