using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GeometryWarrior
{
    /// <summary>
    /// 主角装扮页面 - 显示当前装备的外观部件
    /// </summary>
    public class PlayerEquipPage : MonoBehaviour
    {
        [Header("【角色预览】")]
        [Tooltip("角色预览图片")] public Image characterPreview;
        [Tooltip("角色旋转按钮-左")] public Button rotateLeftBtn;
        [Tooltip("角色旋转按钮-右")] public Button rotateRightBtn;
        
        [Header("【装扮槽位】")]
        [Tooltip("装扮槽位容器")] public Transform slotContainer;
        [Tooltip("装扮槽位预制体")] public GameObject equipSlotPrefab;
        
        [Header("【描述区域】")]
        [Tooltip("装扮描述文本")] public Text descriptionText;
        
        [Header("【样式配置】")]
        [Tooltip("空槽位颜色")] public Color emptySlotColor = new Color(0.9f, 0.9f, 0.9f);
        [Tooltip("有装备的颜色")] public Color filledSlotColor = new Color(0.659f, 0.902f, 0.812f);
        
        // 装扮槽位数据
        private List<EquipSlot> equipSlots = new List<EquipSlot>();
        
        // 类别到名称的映射
        private Dictionary<OutfitCategory, string> categoryNames = new Dictionary<OutfitCategory, string>
        {
            { OutfitCategory.Bow, "头饰" },
            { OutfitCategory.Hat, "帽子" },
            { OutfitCategory.Glasses, "眼镜" },
            { OutfitCategory.Scarf, "围巾" },
            { OutfitCategory.Backpack, "背饰" }
        };
        
        // 类别顺序
        private OutfitCategory[] categoryOrder = new OutfitCategory[]
        {
            OutfitCategory.Bow,
            OutfitCategory.Hat,
            OutfitCategory.Glasses,
            OutfitCategory.Scarf,
            OutfitCategory.Backpack
        };
        
        private float currentRotation = 0f;
        private OutfitCategory? selectedCategory = null;
        
        void Start()
        {
            InitializeSlots();
            BindButtons();
            RefreshDisplay();
        }
        
        void OnEnable()
        {
            RefreshDisplay();
        }
        
        /// <summary>
        /// 初始化装扮槽位
        /// </summary>
        private void InitializeSlots()
        {
            equipSlots.Clear();
            
            // 查找现有的槽位
            if (slotContainer == null) return;
            
            for (int i = 0; i < slotContainer.childCount; i++)
            {
                Transform slotTransform = slotContainer.GetChild(i);
                if (slotTransform.name.StartsWith("Slot_"))
                {
                    int index = equipSlots.Count;
                    if (index < categoryOrder.Length)
                    {
                        SetupExistingSlot(slotTransform, categoryOrder[index]);
                    }
                }
            }
        }
        
        /// <summary>
        /// 设置已存在的槽位
        /// </summary>
        private void SetupExistingSlot(Transform slotTransform, OutfitCategory category)
        {
            EquipSlot slot = new EquipSlot
            {
                category = category,
                gameObject = slotTransform.gameObject,
                button = slotTransform.GetComponent<Button>()
            };
            
            // 查找子元素
            slot.iconImage = slotTransform.Find("IconArea/Icon")?.GetComponent<Image>();
            slot.nameText = slotTransform.Find("Name")?.GetComponent<Text>();
            slot.bgImage = slotTransform.GetComponent<Image>();
            
            // 绑定点击事件
            if (slot.button != null)
            {
                slot.button.onClick.RemoveAllListeners();
                var captureCategory = category;
                slot.button.onClick.AddListener(() => OnSlotClick(captureCategory));
            }
            
            equipSlots.Add(slot);
        }
        
        /// <summary>
        /// 绑定按钮事件
        /// </summary>
        private void BindButtons()
        {
            if (rotateLeftBtn != null)
                rotateLeftBtn.onClick.AddListener(() => RotateCharacter(-45f));
            
            if (rotateRightBtn != null)
                rotateRightBtn.onClick.AddListener(() => RotateCharacter(45f));
        }
        
        /// <summary>
        /// 刷新显示
        /// </summary>
        public void RefreshDisplay()
        {
            RefreshSlots();
            UpdateCharacterPreview();
            UpdateDescription();
        }
        
        /// <summary>
        /// 刷新所有槽位显示
        /// </summary>
        private void RefreshSlots()
        {
            for (int i = 0; i < equipSlots.Count && i < categoryOrder.Length; i++)
            {
                var slot = equipSlots[i];
                var category = categoryOrder[i];
                slot.category = category; // 确保类别正确
                
                var equippedPart = OutfitManager.Instance?.GetEquippedPart(category);
                
                if (equippedPart != null)
                {
                    // 已装备
                    if (slot.iconImage != null)
                        slot.iconImage.sprite = equippedPart.icon;
                    
                    if (slot.nameText != null)
                        slot.nameText.text = equippedPart.partName;
                    
                    if (slot.bgImage != null)
                        slot.bgImage.color = filledSlotColor;
                }
                else
                {
                    // 未装备 - 显示类别名
                    if (slot.iconImage != null)
                        slot.iconImage.sprite = null;
                    
                    if (slot.nameText != null)
                        slot.nameText.text = categoryNames[category];
                    
                    if (slot.bgImage != null)
                        slot.bgImage.color = emptySlotColor;
                }
            }
        }
        
        /// <summary>
        /// 更新角色预览
        /// </summary>
        private void UpdateCharacterPreview()
        {
            // 这里可以实现一个组合预览
            // 根据当前装备显示不同的预览图
            
            // TODO: 根据 equippedParts 组合显示角色形象
            // 可以叠加多个 Sprite 或者使用预制的组合图
        }
        
        /// <summary>
        /// 更新描述文本
        /// </summary>
        private void UpdateDescription()
        {
            if (descriptionText == null) return;
            
            // 如果有选中的槽位，显示该槽位装备/类别的描述
            if (selectedCategory.HasValue)
            {
                var equippedPart = OutfitManager.Instance?.GetEquippedPart(selectedCategory.Value);
                if (equippedPart != null)
                {
                    // 显示装备描述
                    descriptionText.text = $"【{equippedPart.partName}】\n{equippedPart.description}";
                }
                else
                {
                    // 显示类别提示
                    descriptionText.text = $"【{categoryNames[selectedCategory.Value]}】\n点击切换到装扮界面选择外观";
                }
            }
            else
            {
                // 默认提示
                int equippedCount = 0;
                foreach (var category in categoryOrder)
                {
                    if (OutfitManager.Instance?.GetEquippedPart(category) != null)
                        equippedCount++;
                }
                
                descriptionText.text = $"已装备 {equippedCount}/{categoryOrder.Length} 件装扮\n点击槽位查看详情或更换装扮";
            }
        }
        
        /// <summary>
        /// 点击槽位
        /// </summary>
        private void OnSlotClick(OutfitCategory category)
        {
            selectedCategory = category;
            UpdateDescription();
            
            // 可以双击或长按才切换页面，单击只显示描述
            // 暂时单击就切换
            if (BackpackPanel.Instance != null)
            {
                // 切换到主角装扮页（其实就是 OutfitPanelNew，在主角装扮页签里）
                // 这里需要打开换装界面
                Debug.Log($"点击了 {categoryNames[category]} 槽位");
            }
        }
        
        /// <summary>
        /// 旋转角色预览
        /// </summary>
        private void RotateCharacter(float angle)
        {
            currentRotation += angle;
            if (characterPreview != null)
            {
                characterPreview.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
            }
        }
        
        /// <summary>
        /// 装扮槽位数据
        /// </summary>
        private class EquipSlot
        {
            public OutfitCategory category;
            public GameObject gameObject;
            public Button button;
            public Image iconImage;
            public Text nameText;
            public Image bgImage;
        }
    }
}
