using UnityEngine;
using UnityEngine.UI;

namespace FluffyGeometry.Editor
{
    /// <summary>
    /// 背包系统编辑器菜单
    /// 通过菜单命令创建所有预制体
    /// </summary>
    public class BackpackSystemMenu : MonoBehaviour
    {
        [UnityEditor.MenuItem("绒毛几何物语/创建背包按钮预制体")]
        public static void CreateBackpackButtonPrefab()
        {
            // 创建根物体
            GameObject btnObj = new GameObject("BackpackButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            
            var rectTransform = btnObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(30, -30);
            rectTransform.sizeDelta = new Vector2(80, 80);
            
            var image = btnObj.GetComponent<Image>();
            image.color = new Color(0.8f, 0.6f, 0.4f);
            
            // 添加BackpackButton脚本
            var backpackBtn = btnObj.AddComponent<FluffyGeometry.UI.BackpackButton>();
            backpackBtn.backpackBtn = btnObj.GetComponent<Button>();
            backpackBtn.backpackIcon = image;
            
            // 创建新物品红点
            GameObject badgeObj = new GameObject("NewItemBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeObj.transform.SetParent(btnObj.transform);
            
            var badgeRect = badgeObj.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.anchoredPosition = new Vector2(-10, -10);
            badgeRect.sizeDelta = new Vector2(20, 20);
            
            var badgeImage = badgeObj.GetComponent<Image>();
            badgeImage.color = Color.red;
            
            backpackBtn.newItemBadge = badgeObj;
            badgeObj.SetActive(false);
            
            // 选中并高亮
            UnityEditor.Selection.activeGameObject = btnObj;
            UnityEditor.EditorGUIUtility.PingObject(btnObj);
            
            Debug.Log("[BackpackSystemMenu] 背包按钮已创建！请拖入Prefabs文件夹保存，并配置backpackPanelPrefab引用");
        }
        
        [UnityEditor.MenuItem("绒毛几何物语/创建家具数据")]
        public static void CreateFurnitureData()
        {
            var furnitureData = ScriptableObject.CreateInstance<FluffyGeometry.Home.FurnitureData>();
            furnitureData.furnitureId = "furniture_001";
            furnitureData.furnitureName = "新家具";
            furnitureData.defaultScale = 1f;
            furnitureData.minScale = 0.5f;
            furnitureData.maxScale = 2f;
            furnitureData.canFlip = true;
            furnitureData.isUnlocked = true;
            
            string path = "Assets/Resources/Furniture/NewFurniture.asset";
            
            // 确保目录存在
            System.IO.Directory.CreateDirectory("Assets/Resources/Furniture");
            
            UnityEditor.AssetDatabase.CreateAsset(furnitureData, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = furnitureData;
            
            Debug.Log($"[BackpackSystemMenu] 家具数据已创建：{path}");
        }
    }
}
