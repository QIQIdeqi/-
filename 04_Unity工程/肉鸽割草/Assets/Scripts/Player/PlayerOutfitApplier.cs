using System.Collections.Generic;
using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 玩家装扮应用器 - 在玩家身上显示所有装备的部件
    /// 将此组件添加到Player预制体上
    /// </summary>
    public class PlayerOutfitApplier : MonoBehaviour
    {
        [Header("【部件挂载点】")]
        [Tooltip("头部装饰的挂载点（蝴蝶结、帽子、眼镜等）")]
        [SerializeField] private Transform headAttachment;
        
        [Tooltip("身体装饰的挂载点（围巾、鞋子等）")]
        [SerializeField] private Transform bodyAttachment;
        
        [Tooltip("背部装饰的挂载点（背包等）")]
        [SerializeField] private Transform backAttachment;
        
        [Header("【部件容器】")]
        [Tooltip("所有部件的父容器（留空则使用当前物体）")]
        [SerializeField] private Transform outfitContainer;
        
        // 当前显示的部件
        private Dictionary<OutfitCategory, GameObject> activeParts = new Dictionary<OutfitCategory, GameObject>();
        
        private void Start()
        {
            // 如果没有指定容器，使用当前物体
            if (outfitContainer == null)
                outfitContainer = transform;
            
            // 如果没有指定挂载点，使用当前物体
            if (headAttachment == null)
                headAttachment = transform;
            if (bodyAttachment == null)
                bodyAttachment = transform;
            if (backAttachment == null)
                backAttachment = transform;
            
            // 监听装扮变化
            if (OutfitManager.Instance != null)
            {
                OutfitManager.Instance.OnOutfitChanged += OnOutfitChanged;
            }
            
            // 应用当前装扮
            if (OutfitManager.Instance != null)
            {
                ApplyOutfit(OutfitManager.Instance.GetAllEquippedParts());
            }
        }
        
        private void OnDestroy()
        {
            // 取消监听
            if (OutfitManager.Instance != null)
            {
                OutfitManager.Instance.OnOutfitChanged -= OnOutfitChanged;
            }
        }
        
        /// <summary>
        /// 装扮变化回调
        /// </summary>
        private void OnOutfitChanged()
        {
            if (OutfitManager.Instance != null)
            {
                ApplyOutfit(OutfitManager.Instance.GetAllEquippedParts());
            }
        }
        
        /// <summary>
        /// 应用装扮
        /// </summary>
        public void ApplyOutfit(Dictionary<OutfitCategory, OutfitPartData> parts)
        {
            // 清除旧部件
            ClearAllParts();
            
            // 创建新部件
            foreach (var kvp in parts)
            {
                CreatePart(kvp.Key, kvp.Value);
            }
        }
        
        /// <summary>
        /// 创建部件
        /// </summary>
        private void CreatePart(OutfitCategory category, OutfitPartData partData)
        {
            if (partData == null || partData.partSprite == null) return;
            
            // 确定挂载点
            Transform attachment = GetAttachmentPoint(category);
            
            // 创建部件GameObject
            GameObject partObj = new GameObject($"Outfit_{category}");
            partObj.transform.SetParent(attachment);
            partObj.transform.localPosition = partData.offset;
            partObj.transform.localRotation = Quaternion.Euler(0, 0, partData.rotation);
            partObj.transform.localScale = partData.scale;
            
            // 添加SpriteRenderer
            SpriteRenderer spriteRenderer = partObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = partData.partSprite;
            spriteRenderer.sortingOrder = 1; // 确保在玩家之上
            
            // 保存引用
            activeParts[category] = partObj;
        }
        
        /// <summary>
        /// 获取部件挂载点
        /// </summary>
        private Transform GetAttachmentPoint(OutfitCategory category)
        {
            switch (category)
            {
                case OutfitCategory.Bow:
                case OutfitCategory.Hat:
                case OutfitCategory.Glasses:
                    return headAttachment;
                case OutfitCategory.Scarf:
                    return bodyAttachment;
                case OutfitCategory.Backpack:
                    return backAttachment;
                case OutfitCategory.Shoes:
                    return bodyAttachment;
                case OutfitCategory.Special:
                    return bodyAttachment;
                default:
                    return outfitContainer;
            }
        }
        
        /// <summary>
        /// 清除所有部件
        /// </summary>
        private void ClearAllParts()
        {
            foreach (var kvp in activeParts)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
            }
            activeParts.Clear();
        }
    }
}
