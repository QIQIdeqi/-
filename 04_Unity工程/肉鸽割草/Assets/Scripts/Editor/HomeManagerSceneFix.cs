using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 修复 HomeManager 场景名引用
    /// </summary>
    public class HomeManagerSceneFix : EditorWindow
    {
        private string newSceneName = "GameScene";
        
        [MenuItem("绒毛几何物语/工具/修复HomeManager场景名")]
        public static void ShowWindow()
        {
            GetWindow<HomeManagerSceneFix>("修复场景名");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("修复 HomeManager 主菜单场景名", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            newSceneName = EditorGUILayout.TextField("主菜单场景名:", newSceneName);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("更新所有 HomeManager", GUILayout.Height(40)))
            {
                UpdateAllHomeManagers();
            }
        }
        
        private void UpdateAllHomeManagers()
        {
            int count = 0;
            
            // 查找所有场景中的 HomeManager
            HomeManager[] managers = FindObjectsOfType<HomeManager>();
            foreach (var manager in managers)
            {
                SerializedObject so = new SerializedObject(manager);
                SerializedProperty prop = so.FindProperty("mainMenuSceneName");
                
                if (prop != null)
                {
                    string oldValue = prop.stringValue;
                    prop.stringValue = newSceneName;
                    so.ApplyModifiedProperties();
                    
                    Debug.Log($"[HomeManagerSceneFix] {manager.name}: {oldValue} → {newSceneName}");
                    count++;
                }
            }
            
            // 也更新预制体
            string[] guids = AssetDatabase.FindAssets("t:Prefab HomeManager");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    HomeManager manager = prefab.GetComponent<HomeManager>();
                    if (manager != null)
                    {
                        SerializedObject so = new SerializedObject(manager);
                        SerializedProperty prop = so.FindProperty("mainMenuSceneName");
                        
                        if (prop != null && prop.stringValue != newSceneName)
                        {
                            prop.stringValue = newSceneName;
                            so.ApplyModifiedProperties();
                            
                            Debug.Log($"[HomeManagerSceneFix] 预制体 {path}: → {newSceneName}");
                            count++;
                        }
                    }
                }
            }
            
            EditorUtility.DisplayDialog("完成", $"更新了 {count} 个 HomeManager", "确定");
        }
    }
}
