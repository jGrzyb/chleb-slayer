using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class FontReplacer : EditorWindow
{
    private TMP_FontAsset newFont;
    private bool includePrefabs = true;

    [MenuItem("Tools/Font Replacer")]
    public static void Open()
    {
        GetWindow<FontReplacer>("Font Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("Podmień font we wszystkich TMP", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Nowy font", newFont, typeof(TMP_FontAsset), false);
        includePrefabs = EditorGUILayout.Toggle("Uwzględnij prefaby", includePrefabs);

        EditorGUILayout.Space();

        GUI.enabled = newFont != null;

        if (GUILayout.Button("Podmień w aktywnej scenie"))
            ReplaceInScene();

        if (includePrefabs && GUILayout.Button("Podmień w prefabach (Assets/)"))
            ReplaceInPrefabs();

        GUI.enabled = true;

        if (newFont == null)
            EditorGUILayout.HelpBox("Przeciągnij font TMP_FontAsset żeby zacząć.", MessageType.Info);
    }

    private void ReplaceInScene()
    {
        int count = 0;
        TMP_Text[] allTexts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TMP_Text tmp in allTexts)
        {
            Undo.RecordObject(tmp, "Replace TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
            count++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[FontReplacer] Podmieniono font w {count} obiektach w scenie.");
    }

    private void ReplaceInPrefabs()
    {
        int count = 0;
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            TMP_Text[] texts = prefab.GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length == 0) continue;

            bool changed = false;
            foreach (TMP_Text tmp in texts)
            {
                Undo.RecordObject(tmp, "Replace TMP Font in Prefab");
                tmp.font = newFont;
                EditorUtility.SetDirty(tmp);
                count++;
                changed = true;
            }

            if (changed)
                PrefabUtility.SavePrefabAsset(prefab);
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[FontReplacer] Podmieniono font w {count} obiektach w prefabach.");
    }
}
