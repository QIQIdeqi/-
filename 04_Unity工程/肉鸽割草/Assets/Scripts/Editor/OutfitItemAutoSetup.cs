using UnityEngine;
using UnityEditor;

namespace FluffyGeometry.Editor
{
    /// <summary>
    /// 主角装扮项自动配置工具
    /// </summary>
    public class OutfitItemAutoSetup : MonoBehaviour
    {
        [Header("【自动配置设置】")]
        [Tooltip("点击此按钮自动生成装扮项预制体")]
        public bool autoSetup = false;
        
        private void OnValidate()
        {
            if (autoSetup)
            {
                autoSetup = false;
                SetupOutfitItem();
            }
        }
        
        private void SetupOutfitItem()
        {
            GameObject itemObj = new GameObject("OutfitItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            itemObj.transform.SetParent(transform);
            
            var rect = itemObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
            
            var bgImage = itemObj.GetComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(1f, 1f, 1f);
            
            // 添加脚本
            var itemUI = itemObj.AddComponent<GeometryWarrior.OutfitItemUI>();
            
            // 图标
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            iconObj.transform.SetParent(itemObj.transform);
            var iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(5, 5);
            iconRect.offsetMax = new Vector2(-5, -5);
            itemUI.GetType().GetField("iconImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(itemUI, iconObj.GetComponent<UnityEngine.UI.Image>());
            
            // 锁定图标
            GameObject lockObj = new GameObject("LockIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            lockObj.transform.SetParent(itemObj.transform);
            var lockRect = lockObj.GetComponent<RectTransform>();
            lockRect.anchorMin = new Vector2(1, 1);
            lockRect.anchorMax = new Vector2(1, 1);
            lockRect.pivot = new Vector2(1, 1);
            lockRect.anchoredPosition = new Vector2(-5, -5);
            lockRect.sizeDelta = new Vector2(30, 30);
            lockObj.GetComponent<UnityEngine.UI.Image>().color = Color.gray;
            lockObj.SetActive(false);
            itemUI.GetType().GetField("lockImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(itemUI, lockObj.GetComponent<UnityEngine.UI.Image>());
            
            // 已装备标识
            GameObject equippedObj = new GameObject("EquippedIndicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            equippedObj.transform.SetParent(itemObj.transform);
            var equippedRect = equippedObj.GetComponent<RectTransform>();
            equippedRect.anchorMin = new Vector2(0, 0);
            equippedRect.anchorMax = new Vector2(0, 0);
            equippedRect.pivot = new Vector2(0, 0);
            equippedRect.anchoredPosition = new Vector2(5, 5);
            equippedRect.sizeDelta = new Vector2(20, 20);
            equippedObj.GetComponent<UnityEngine.UI.Image>().color = new Color(0.4f, 0.8f, 0.4f);
            equippedObj.SetActive(false);
            itemUI.GetType().GetField("equippedIndicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(itemUI, equippedObj);
            
            // 按钮
            var btn = itemObj.AddComponent<UnityEngine.UI.Button>();
            
            UnityEditor.Selection.activeGameObject = itemObj;
            Debug.Log("[OutfitItemAutoSetup] 装扮项预制体已创建！");
        }
    }
}
