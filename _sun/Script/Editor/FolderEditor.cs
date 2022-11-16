using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sun.Editor
{

    /// <summary>
    /// 快速創建專案資料夾 v1.0
    /// </summary>
    public class FolderEditor : EditorWindow
    {
        static string folderName = "_Main";
        public string[] componentFolder = {
        "Animations",
        "Audio",
        "Fonts",
        "Materials",
        "Models",
        "Prefabs",
        "Scenes",
        "Script",
        "Shaders",
        "Standard Assets",
        "Textures"
    };

        [MenuItem("Sun/Fast Create Folder")]
        static void Init()
        {

            EditorWindow.GetWindow(typeof(FolderEditor), false, "Folder Editor");

        }

        void OnGUI()
        {
            Rect r = position;

            //r.y = EditorGUI.GetPropertyHeight(element) + EditorGUIUtility.standardVerticalSpacing;

            GUILayout.Label("Folder Settings", EditorStyles.boldLabel);
            folderName = EditorGUILayout.TextField("Main Folder Name", folderName);

            ScriptableObject target = this;
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty stringsProperty = serializedObject.FindProperty("componentFolder");

            EditorGUILayout.PropertyField(stringsProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Add Folder"))
            {
                CreateFolder();
            }
        }

        void CreateFolder()
        {

            if (!AssetDatabase.IsValidFolder("Assets/" + folderName))
            {
                AssetDatabase.CreateFolder("Assets", folderName);
            }
            for (int i = 0; i < componentFolder.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder("Assets/" + folderName + "/" + componentFolder[i]))
                {
                    AssetDatabase.CreateFolder("Assets/" + folderName, componentFolder[i]);
                }
            }

            if (!AssetDatabase.IsValidFolder("Assets/" + folderName + "/Script/" + "Editor"))
            {
                AssetDatabase.CreateFolder("Assets/" + folderName + "/Script", "Editor");

            }
        }
    }
}