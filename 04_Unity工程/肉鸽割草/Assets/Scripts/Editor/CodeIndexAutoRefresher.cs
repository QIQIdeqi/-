using UnityEngine;
using UnityEditor;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 代码索引自动刷新器
    /// 在编辑器启动或脚本编译后自动刷新索引
    /// </summary>
    [InitializeOnLoad]
    public static class CodeIndexAutoRefresher
    {
        private const string INDEX_PATH = "Assets/Documentation/CodeIndex.md";
        private const string AUTO_REFRESH_KEY = "CodeIndexer_AutoRefresh";
        private const string LAST_REFRESH_KEY = "CodeIndexer_LastRefresh";
        
        // 启动时刷新
        static CodeIndexAutoRefresher()
        {
            // 延迟执行，等待编辑器完全加载
            EditorApplication.delayCall += () =>
            {
                if (ShouldAutoRefresh())
                {
                    RefreshIndex();
                }
            };
            
            // 监听编译完成事件
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // 退出Play模式时刷新（开发时常用）
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (ShouldAutoRefresh())
                {
                    // 延迟一点，确保所有脚本已加载
                    EditorApplication.delayCall += () =>
                    {
                        RefreshIndex();
                    };
                }
            }
        }
        
        /// <summary>
        /// 检查是否需要自动刷新
        /// </summary>
        private static bool ShouldAutoRefresh()
        {
            // 检查是否启用了自动刷新
            if (!EditorPrefs.GetBool(AUTO_REFRESH_KEY, true))
                return false;
            
            // 检查索引文件是否存在
            string fullPath = Path.Combine(Application.dataPath, "..", INDEX_PATH);
            if (!File.Exists(fullPath))
                return true; // 不存在则创建
            
            // 检查最后刷新时间（超过30分钟刷新一次）
            string lastRefreshStr = EditorPrefs.GetString(LAST_REFRESH_KEY, "");
            if (System.DateTime.TryParse(lastRefreshStr, out System.DateTime lastRefresh))
            {
                if ((System.DateTime.Now - lastRefresh).TotalMinutes > 30)
                    return true;
            }
            else
            {
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 手动刷新索引（供外部调用）
        /// </summary>
        [MenuItem("绒毛几何物语/工具/立即刷新代码索引 _F5")]
        public static void RefreshIndex()
        {
            // 直接调用 CodeIndexer 的静态方法
            CodeIndexer.GenerateIndexStatic();
            
            // 更新时间戳
            EditorPrefs.SetString(LAST_REFRESH_KEY, System.DateTime.Now.ToString());
            
            Debug.Log("[CodeIndexAutoRefresher] 代码索引已刷新");
        }
        
        /// <summary>
        /// 设置是否启用自动刷新
        /// </summary>
        [MenuItem("绒毛几何物语/工具/自动刷新设置", priority = 100)]
        public static void ToggleAutoRefresh()
        {
            bool current = EditorPrefs.GetBool(AUTO_REFRESH_KEY, true);
            EditorPrefs.SetBool(AUTO_REFRESH_KEY, !current);
            
            Menu.SetChecked("绒毛几何物语/工具/自动刷新设置", !current);
            
            Debug.Log($"[CodeIndexAutoRefresher] 自动刷新: {(!current ? "启用" : "禁用")}");
        }
        
        [MenuItem("绒毛几何物语/工具/自动刷新设置", validate = true)]
        private static bool ValidateToggleAutoRefresh()
        {
            bool current = EditorPrefs.GetBool(AUTO_REFRESH_KEY, true);
            Menu.SetChecked("绒毛几何物语/工具/自动刷新设置", current);
            return true;
        }
        
        /// <summary>
        /// 打开索引文档
        /// </summary>
        [MenuItem("绒毛几何物语/工具/查看代码索引 _F6")]
        public static void OpenIndex()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", INDEX_PATH);
            if (File.Exists(fullPath))
            {
                EditorUtility.OpenWithDefaultApp(fullPath);
            }
            else
            {
                if (EditorUtility.DisplayDialog("提示", "索引文档不存在，是否立即生成？", "生成", "取消"))
                {
                    RefreshIndex();
                }
            }
        }
    }
}
