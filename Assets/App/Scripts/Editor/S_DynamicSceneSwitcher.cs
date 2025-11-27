using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

public class S_DynamicSceneSwitcher : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private string[] scenePaths = new string[0];
    private string[] filteredScenes = new string[0];
    private string searchQuery = "";

    private GUIStyle searchFieldStyle = null;
    private GUIStyle buttonStyle = null;
    private GUIStyle buttonRefreshStyle = null;
    private float lastWidth = 0;

    [MenuItem("Tools/Dynamic Scenes Switcher")]
    public static void ShowWindow()
    {
        var window = GetWindow<S_DynamicSceneSwitcher>("Dynamic Scene Switcher");
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(1000, 1000);
        window.position = new Rect(100, 100, 400, 400);
    }

    private void OnEnable()
    {
        RefreshSceneList();
    }

    private void RefreshSceneList()
    {
        scenePaths = AssetDatabase.FindAssets("t:Scene")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => path.StartsWith("Assets/App/Scenes/"))
            .ToArray();

        UpdateFilteredScenes();
    }

    private void UpdateFilteredScenes()
    {
        filteredScenes = string.IsNullOrWhiteSpace(searchQuery) ? scenePaths : scenePaths
            .Where(p => Path.GetFileNameWithoutExtension(p)
            .IndexOf(searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0)
            .ToArray();
    }

    private void SetupStyles()
    {
        if (Mathf.Approximately(lastWidth, position.width)) return;

        lastWidth = position.width;

        float scale = Mathf.Clamp(position.width / 400f, 0.75f, 2f);

        searchFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 14,
            padding = new RectOffset(6, 6, 4, 4),
            fixedHeight = 25,
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(20, 20, 5, 5),
            padding = new RectOffset(10, 10, 8, 8),
            fontSize = Mathf.RoundToInt(14 * scale),
            alignment = TextAnchor.MiddleCenter,
        };

        buttonRefreshStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(5, 5, 5, 5),
            padding = new RectOffset(15, 15, 10, 10),
            fontSize = Mathf.RoundToInt(16 * scale),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter,
        };
    }

    private void OnGUI()
    {
        /// Setup Styles
        SetupStyles();

        /// Title
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Search Scenes:", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        /// Dummy Focus Control
        GUI.SetNextControlName("DummyFocus");
        GUILayout.Box("", GUIStyle.none, GUILayout.Width(0), GUILayout.Height(0));

        /// Search Field
        GUI.SetNextControlName("SearchField");
        string newQuery = EditorGUILayout.TextField(searchQuery, searchFieldStyle);

        /// Event Handling
        if (Event.current.type == EventType.MouseDown)
        {
            if (GUI.GetNameOfFocusedControl() == "SearchField")
            {
                Rect fieldRect = GUILayoutUtility.GetLastRect();
                if (!fieldRect.Contains(Event.current.mousePosition))
                {
                    GUI.FocusControl(null);
                    Repaint();
                }
            }
        }

        if (GUI.GetNameOfFocusedControl() == "SearchField" && Event.current.keyCode == KeyCode.Return)
        {
            GUI.FocusControl(null);
            Repaint();
        }

        /// Update Filtered Scenes
        if (newQuery != searchQuery)
        {
            searchQuery = newQuery;
            UpdateFilteredScenes();
            Repaint();
        }

        EditorGUILayout.Space(15);

        /// Scene Buttons
        if (filteredScenes.Length == 0)
        {
            EditorGUILayout.LabelField("No Scenes Found!", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            foreach (var scenePath in filteredScenes)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                if (GUILayout.Button(sceneName, buttonStyle) && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space(5);

        /// Refresh Button
        if (GUILayout.Button("Refresh Scenes", buttonRefreshStyle))
        {
            RefreshSceneList();
            Repaint();
        }
    }
}