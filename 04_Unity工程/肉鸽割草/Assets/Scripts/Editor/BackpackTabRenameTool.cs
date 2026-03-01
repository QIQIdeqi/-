using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 背包页面标签重命名工具
    /// 将"主角装备"改为"主角装扮"，"家园装扮"改为"家园布置"
    /// </summary>
    public class BackpackTabRenameTool : EditorWindow
    {
        [MenuItem("绒毛几何物语/工具/重命名背包标签")]
        public static void ShowWindow()
        {
            GetWindow<BackpackTabRenameTool>("重命名背包标签");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("📝 重命名背包页面标签", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "此工具会将：\n" +
                "• 『主角装备』→ 『主角装扮』\n" +
                "• 『家园装扮』→ 『家园布置』", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("重命名场景中的标签", GUILayout.Height(40)))
            {
                RenameTabsInScene();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("重命名预制体中的标签", GUILayout.Height(40)))
            {
                RenameTabsInPrefab();
            }
        }
        
        private void RenameTabsInScene()
        {
            int count = 0;
            
            // 查找所有 BackpackPanel
            var panels = FindObjectsOfType<BackpackPanel>(true);
            foreach (var panel in panels)
            {
                if (RenamePanelTabs(panel))
                    count++;
            }
            
            EditorUtility.DisplayDialog("完成", $"已重命名 {count} 个 BackpackPanel 的标签", "确定");
        }
        
        private void RenameTabsInPrefab()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab BackpackPanel");
            int count = 0;
            
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    BackpackPanel panel = prefab.GetComponent<BackpackPanel>();
                    if (panel != null && RenamePanelTabs(panel))
                    {
                        EditorUtility.SetDirty(prefab);
                        count++;
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("完成", $"已重命名 {count} 个 BackpackPanel 预制体的标签", "确定");
        }
        
        private bool RenamePanelTabs(BackpackPanel panel)
        {
            bool changed = false;
            
            // 修改主角装备标签
            if (panel.playerEquipTab != null)
            {
                Text text = panel.playerEquipTab.GetComponentInChildren<Text>(true);
                if (text != null && text.text.Contains("装备"))
                {
                    text.text = "主角装扮";
                    changed = true;
                    Debug.Log($"[BackpackTabRenameTool] 重命名: {panel.name} 的主角装备 → 主角装扮");
                }
            }
            
            // 修改家园装扮标签
            if (panel.homeOutfitTab != null)
            {
                Text text = panel.homeOutfitTab.GetComponentInChildren<Text>(true);
                if (text != null && text.text.Contains("装扮"))
                {
                    text.text = "家园布置";
                    changed = true;
                    Debug.Log($"[BackpackTabRenameTool] 重命名: {panel.name} 的家园装扮 → 家园布置");
                }
            }
            
            return changed;
        }
    }
}
