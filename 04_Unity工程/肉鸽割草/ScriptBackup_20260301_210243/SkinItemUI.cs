using UnityEngine;
using UnityEngine.UI;
using System;

namespace GeometryWarrior
{
    /// <summary>
    /// 皮肤列表项 UI
    /// </summary>
    public class SkinItemUI : MonoBehaviour
    {
        [Header("[UI组件]")]
        [SerializeField] private Image skinIcon;           // 皮肤图标
        [SerializeField] private Image lockIcon;           // 锁定图标
        [SerializeField] private Image equippedIcon;       // 已装备图标
        [SerializeField] private Image selectedBorder;     // 选中边框
        [SerializeField] private Button button;            // 按钮
        
        public SkinData SkinData { get; private set; }
        
        private Action<SkinData> onClickCallback;
        
        /// <summary>
        /// 设置皮肤项
        /// </summary>
        public void Setup(SkinData skin, bool isUnlocked, bool isEquipped, Action<SkinData> onClick)
        {
            SkinData = skin;
            onClickCallback = onClick;
            
            // 设置图标
            if (skinIcon != null && skin.icon != null)
            {
                skinIcon.sprite = skin.icon;
                skinIcon.color = isUnlocked ? Color.white : Color.gray;
            }
            
            // 锁定图标
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!isUnlocked);
            }
            
            // 已装备图标
            if (equippedIcon != null)
            {
                equippedIcon.gameObject.SetActive(isEquipped);
            }
            
            // 选中边框（默认关闭）
            if (selectedBorder != null)
            {
                selectedBorder.gameObject.SetActive(false);
            }
            
            // 绑定按钮点击
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }
        }
        
        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (selectedBorder != null)
            {
                selectedBorder.gameObject.SetActive(selected);
            }
        }
        
        /// <summary>
        /// 点击回调
        /// </summary>
        private void OnClick()
        {
            onClickCallback?.Invoke(SkinData);
        }
    }
}
