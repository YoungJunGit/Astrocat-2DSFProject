using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace SnapShotPro
{
    public class SnapShotProWindow : EditorWindow
    {
        private int currentTap = 0;
        private string[] tabs = new string[] { "Rendering", "Export" };

        public enum ImageFormat { PNG, JPEG, EXR }

        [System.Serializable]
        public class ScreenshotSetting
        {
            public string name = "Screenshot";
            public int resolutionIndex = 1;
            public Vector2 customResolution = new Vector2(800, 600);
            public int viewIndex = 0;
            public bool transparentBackground = false;
            public ImageFormat imageFormat = ImageFormat.JPEG;
            public int jpegQuality = 100;
        }

        [System.Serializable]
        public class ScreenshotData
        {
            public Texture2D texture;
            public ScreenshotSetting setting;
        }

        private List<ScreenshotSetting> settingsList = new List<ScreenshotSetting>();
        private int selectedSettingIndex = 0;
        private List<ScreenshotData> screenshotsMemory = new List<ScreenshotData>();

        private Vector2 cardsScrollPos;
        private Vector2 galleryScrollPos;
        private string exportPath = "Assets/Screenshots/";

        [MenuItem("Tools/SnapShot Pro/SnapShot Pro Window")]
        public static void ShowWindow() => GetWindow<SnapShotProWindow>("SnapShot Pro");

        [MenuItem("Tools/SnapShot Pro/Take Screenshot &#S")] // Alt+Shift+S
        private static void TakeAllScreenshotsShortcut()
        {
            var window = GetWindow<SnapShotProWindow>("SnapShot Pro");
            window.TakeAllScreenshots();
        }

        private void OnEnable()
        {
            if (settingsList.Count == 0)
                settingsList.Add(new ScreenshotSetting());
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            currentTap = GUILayout.Toolbar(currentTap, tabs);
            GUILayout.Space(15);

            switch (currentTap)
            {
                case 0: DrawRendering(); break;
                case 1: DrawExport(); break;
            }
        }

        private void DrawRendering()
        {
            float cardWidth = 100f;
            float cardHeight = 30f;
            float cardSpacing = 10f;

            cardsScrollPos = EditorGUILayout.BeginScrollView(cardsScrollPos, false, false, GUILayout.Height(cardHeight + 20));
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < settingsList.Count; i++)
            {
                var setting = settingsList[i];
                GUIStyle cardStyle = new GUIStyle("box") { alignment = TextAnchor.MiddleCenter };
                Rect cardRect = GUILayoutUtility.GetRect(cardWidth, cardHeight, GUILayout.Width(cardWidth), GUILayout.Height(cardHeight));

                if (i == selectedSettingIndex)
                    EditorGUI.DrawRect(cardRect, new Color(0f, 1f, 0f, 0.25f));

                if (GUI.Button(cardRect, setting.name, cardStyle))
                    selectedSettingIndex = i;

                if (i < settingsList.Count - 1)
                    GUILayout.Space(cardSpacing);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            if (selectedSettingIndex >= 0 && selectedSettingIndex < settingsList.Count)
            {
                EditorGUILayout.BeginVertical("box");
                var setting = settingsList[selectedSettingIndex];

                // File Name
                setting.name = EditorGUILayout.TextField(
                    new GUIContent("File Name", "Name of the screenshot file"),
                    setting.name);

                EditorGUILayout.Space();

                // Resolution
                string[] resolutions = { "Screen Size", "1920 x 1080", "2560 x 1440", "3840 x 2160", "1080 x 1080", "Custom" };
                setting.resolutionIndex = EditorGUILayout.Popup(
                    new GUIContent("Resolution", "Choose the resolution for the screenshot"),
                    setting.resolutionIndex, resolutions);

                if (setting.resolutionIndex == resolutions.Length - 1)
                    setting.customResolution = EditorGUILayout.Vector2Field(
                        new GUIContent("Custom Size", "Enter custom width and height"),
                        setting.customResolution);

                // Capture From
                string[] viewOptions = { "Game View", "Scene View" };
                setting.viewIndex = EditorGUILayout.Popup(
                    new GUIContent("Capture From", "Choose which camera to capture from"),
                    setting.viewIndex, viewOptions);

                EditorGUILayout.Space();

                // Image Format
                setting.imageFormat = (ImageFormat)EditorGUILayout.EnumPopup(
                    new GUIContent("Image Format", "Select the image format (PNG, JPEG, EXR)"),
                    setting.imageFormat);

                if (setting.imageFormat == ImageFormat.JPEG)
                    setting.jpegQuality = EditorGUILayout.IntSlider(
                        new GUIContent("JPEG Quality", "Set the JPEG compression quality"),
                        setting.jpegQuality, 1, 100);
                else
                    setting.transparentBackground = EditorGUILayout.Toggle(
                        new GUIContent("Transparent Background", "Enable transparent background"),
                        setting.transparentBackground);

                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10);

            // Add / Remove Settings
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button(
                new GUIContent("Remove Setting", "Remove the currently selected screenshot setting"),
                GUILayout.Height(30)) && settingsList.Count > 0)
            {
                settingsList.RemoveAt(selectedSettingIndex);
                selectedSettingIndex = Mathf.Clamp(selectedSettingIndex - 1, 0, settingsList.Count - 1);
            }

            if (GUILayout.Button(
                new GUIContent("Add New Setting", "Add a new screenshot setting"),
                GUILayout.Height(30)))
            {
                var newCard = new ScreenshotSetting();
                newCard.name += "_" + settingsList.Count;
                settingsList.Add(newCard);
                selectedSettingIndex = settingsList.Count - 1;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            if (GUILayout.Button(
                new GUIContent("📷 Take All Screenshots", "Capture screenshots for all defined settings"),
                GUILayout.Height(40)))
                TakeAllScreenshots();

            EditorGUILayout.HelpBox("Tip: You can take screenshots quickly using the shortcut Alt+Shift+S.", MessageType.Info);
        }

        private void DrawExport()
        {
            EditorGUILayout.BeginVertical("box");

            float thumbWidth = 100f;
            float thumbHeight = 100f;
            float spacing = 10f;
            float totalWidth = screenshotsMemory.Count * (thumbWidth + spacing) - spacing;

            Rect scrollViewRect = GUILayoutUtility.GetRect(0, 120, GUILayout.ExpandWidth(true));
            galleryScrollPos = GUI.BeginScrollView(scrollViewRect, galleryScrollPos, new Rect(0, 0, totalWidth, thumbHeight));

            float xOffset = 0f;
            for (int i = 0; i < screenshotsMemory.Count; i++)
            {
                var data = screenshotsMemory[i];
                if (data.texture == null) continue;

                Rect thumbRect = new Rect(xOffset, 0, thumbWidth, thumbHeight);
                GUI.DrawTexture(thumbRect, data.texture, ScaleMode.StretchToFill, true);

                float buttonSize = 18f;
                Rect deleteButtonRect = new Rect(
                    xOffset + thumbWidth - buttonSize,
                    0,
                    buttonSize,
                    buttonSize
                );

                GUIStyle deleteStyle = new GUIStyle(GUI.skin.button)
                {
                    normal = { textColor = Color.white, background = MakeColorTexture(new Color(0.8f, 0.2f, 0.2f)) },
                    hover = { textColor = Color.white, background = MakeColorTexture(new Color(0.9f, 0.3f, 0.3f)) },
                    fontSize = 10,
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(0, 0, 0, 0)
                };

                if (GUI.Button(deleteButtonRect, "×", deleteStyle))
                {
                    screenshotsMemory.RemoveAt(i);
                    i--;
                    Repaint();
                }

                xOffset += thumbWidth + spacing;
            }

            GUI.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            exportPath = EditorGUILayout.TextField(
                new GUIContent("Export Path", "Folder where screenshots will be saved"),
                exportPath);

            if (GUILayout.Button("Browse", GUILayout.MaxWidth(80)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Export Folder", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                    exportPath = path;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (GUILayout.Button(
                new GUIContent("Export All", "Export all captured screenshots to the selected folder"),
                GUILayout.Height(30)))
                ExportScreenshots();

            EditorGUILayout.EndVertical();
        }

        private Texture2D CaptureScreenshot(ScreenshotSetting setting)
        {
            try
            {
                Camera cam = (setting.viewIndex == 0 ? Camera.main ?? SceneView.lastActiveSceneView?.camera :
                                                      SceneView.lastActiveSceneView?.camera);

                if (cam == null)
                {
                    Debug.LogError($"No camera found to capture screenshot! named {setting.name}");
                    return null;
                }

                int width = 1920;
                int height = 1080;
                string[] presetResolutions = { "Screen Size", "1920 x 1080", "2560 x 1440", "3840 x 2160", "1080 x 1080" };

                if (setting.resolutionIndex == 0) { width = Screen.width; height = Screen.height; }
                else if (setting.resolutionIndex > 0 && setting.resolutionIndex < presetResolutions.Length)
                {
                    var split = presetResolutions[setting.resolutionIndex].Split('x');
                    width = int.Parse(split[0].Trim());
                    height = int.Parse(split[1].Trim());
                }
                else { width = (int)setting.customResolution.x; height = (int)setting.customResolution.y; }

                RenderTexture rt = new RenderTexture(width, height, 24);
                var originalClearFlags = cam.clearFlags;
                var originalBGColor = cam.backgroundColor;

                if (setting.transparentBackground)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = Color.clear;
                }

                cam.targetTexture = rt;
                cam.Render();

                RenderTexture.active = rt;
                Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGBA32, false);
                screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                screenshot.Apply();

                cam.clearFlags = originalClearFlags;
                cam.backgroundColor = originalBGColor;

                cam.targetTexture = null;
                RenderTexture.active = null;
                DestroyImmediate(rt);

                return screenshot;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to capture screenshot: " + ex.Message);
                return null;
            }
        }

        private void ExportScreenshots()
        {
            if (screenshotsMemory.Count == 0)
            {
                EditorUtility.DisplayDialog("No Screenshots", "There are no screenshots to export.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(exportPath))
            {
                EditorUtility.DisplayDialog("Invalid Path", "Please select a valid export path.", "OK");
                return;
            }

            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            foreach (var data in screenshotsMemory)
            {
                if (data.texture == null || data.setting == null) continue;

                try
                {
                    string ext = data.setting.imageFormat.ToString().ToLower();
                    string filename = Path.Combine(exportPath, $"{data.setting.name}.{ext}");
                    byte[] bytes = null;

                    switch (data.setting.imageFormat)
                    {
                        case ImageFormat.PNG: bytes = data.texture.EncodeToPNG(); break;
                        case ImageFormat.JPEG: bytes = data.texture.EncodeToJPG(data.setting.jpegQuality); break;
                        case ImageFormat.EXR: bytes = data.texture.EncodeToEXR(); break;
                    }

                    File.WriteAllBytes(filename, bytes);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to export screenshot '{data.setting.name}': {ex.Message}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"✅ Exported {screenshotsMemory.Count} screenshots to {exportPath}");
        }

        private void TakeAllScreenshots()
        {
            if (settingsList.Count == 0)
            {
                Debug.LogWarning("No screenshot settings defined.");
                return;
            }

            foreach (var setting in settingsList)
            {
                var tex = CaptureScreenshot(setting);
                if (tex != null)
                {
                    string uniqueName = setting.name + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + screenshotsMemory.Count;

                    screenshotsMemory.Add(new ScreenshotData
                    {
                        texture = tex,
                        setting = new ScreenshotSetting
                        {
                            name = uniqueName,
                            resolutionIndex = setting.resolutionIndex,
                            customResolution = setting.customResolution,
                            viewIndex = setting.viewIndex,
                            transparentBackground = setting.transparentBackground,
                            imageFormat = setting.imageFormat,
                            jpegQuality = setting.jpegQuality
                        }
                    });
                }
            }

            Debug.Log($"✅ Taken {screenshotsMemory.Count} screenshots (including previous ones)");
        }

        private Texture2D MakeColorTexture(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.Apply();
            return tex;
        }
    }
}