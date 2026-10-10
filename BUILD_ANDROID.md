# Building an Android APK without installing Unity on your device

The APK can be built on a remote CI runner, so Unity does not need to be installed on your personal computer. However, a Unity Android build still needs a complete Unity project and an authorized Unity build environment.

## Current status

**No APK has been built yet.** This repository currently contains starter scripts and setup notes under `Assets/`, but does not yet contain the full Unity project configuration, a configured gameplay scene, or a verified Android build. A remote build cannot produce a playable game from the scripts alone.

## What the remote build needs

1. A complete Unity project, including `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json`, and at least one scene included in the build.
2. A Unity version with Android Build Support.
3. An authorized Unity license for the CI runner. GameCI commonly uses GitHub repository secrets such as `UNITY_LICENSE`; the exact secret setup depends on the Unity license type and runner.
4. Android build configuration and a gameplay scene. The current main menu deliberately shows a message if no gameplay scene is included.

## Safe next build path

- Finish the playable scene and confirm all scripts compile in the chosen Unity LTS version.
- Configure a GitHub Actions workflow with a Unity build action (for example, GameCI's Unity Builder) and Android as the target.
- Add license credentials only through **GitHub repository Settings → Secrets and variables → Actions**. Never put license contents or passwords in a committed file.
- Run the workflow and download the generated APK from its workflow artifact.

Until those project files, scene, and license setup exist, claiming that an APK is ready would be misleading. The code changes are in the repository, but Unity compilation and device testing remain outstanding.
