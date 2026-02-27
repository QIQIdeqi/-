using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// 装扮部件列表项UI
    /// </summary>
    public class OutfitItemUI : MonoBehaviour
    {
        [Tooltip("显示部件图标的Image组件")]
        [SerializeField] private Image iconImage;
        
        [Tooltip("锁定图标（未解锁时显示）")]
        [SerializeField] private Image lockImage;
        
        [Tooltip("已装备标识（选中时显示）")]
        [SerializeField] private GameObject equippedIndicator;
        
        [Tooltip("点击此按钮选中部件")]
        [SerializeField] private Button button;
        
        public OutfitPartData PartData { get; private set; }
        
        private System.Action<OutfitPartData> onClickCallback;
        
        private void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }
        
        /// <summary>
        /// 设置部件项
        /// </summary>
        public void Setup(OutfitPartData part, bool isUnlocked, bool isEquipped, System.Action<OutfitPartData> callback)
        {
            PartData = part;
            onClickCallback = callback;
            
            // 调试信息
            Debug.Log($"[OutfitItemUI] Setup: {part.partName}, iconImage={(iconImage!=null)}, icon={(part.icon!=null)}, partSprite={(part.partSprite!=null)}");
            
            // 设置图标
            if (iconImage != null)
            {
                Sprite spriteToShow = part.icon != null ? part.icon : part.partSprite;
                
                if (spriteToShow != null)
                {
                    iconImage.sprite = spriteToShow;
                    iconImage.color = isUnlocked ? Color.white : Color.gray;
                    Debug.Log($"[OutfitItemUI] 设置图标: {spriteToShow.name}");
                }
                else
                {
                    Debug.LogWarning($"[OutfitItemUI] {part.partName} 没有图标！请在 OutfitPartData 中设置 icon 或 partSprite");
                    iconImage.color = new Color(1, 1, 1, 0.2f); // 半透明白色表示缺失
                }
            }
            else
            {
                Debug.LogError("[OutfitItemUI] iconImage 未绑定！请在预制体上配置 Icon Image 字段");
            }
            
            // 显示/隐藏锁定图标
            if (lockImage != null)
            {
                lockImage.gameObject.SetActive(!isUnlocked);
            }
            
            // 显示/隐藏已装备标识
            if (equippedIndicator != null)
            {
                equippedIndicator.SetActive(isEquipped);
            }
        }
        
        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            // 可以添加选中效果，如边框高亮
            Image bgImage = GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = selected ? new Color(1f, 0.8f, 0.4f) : Color.white;
            }
        }
        
        private void OnClick()
        {
            onClickCallback?.Invoke(PartData);
        }
    }
}
