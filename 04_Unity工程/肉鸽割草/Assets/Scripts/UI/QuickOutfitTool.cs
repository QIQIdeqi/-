using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeometryWarrior;

/// <summary>
/// 一键换装工具 - 快速切换套装
/// 可以保存多套搭配，一键切换
/// </summary>
public class QuickOutfitTool : MonoBehaviour
{
    [System.Serializable]
    public class OutfitSet
    {
        public string setName;                          // 套装名称
        public Sprite setIcon;                          // 套装图标
        public List<OutfitPartData> parts = new List<OutfitPartData>(); // 部件列表
        
        [HideInInspector]
        public bool isUnlocked = true;                  // 是否已解锁
    }
    
    [Header("【套装配置】")]
    [Tooltip("预设套装列表")] public List<OutfitSet> presetOutfits = new List<OutfitSet>();
    
    [Header("【UI引用】")]
    [Tooltip("套装按钮容器")] public Transform setButtonContainer;
    [Tooltip("套装按钮预制体")] public GameObject setButtonPrefab;
    
    [Header("【动画】")]
    [Tooltip("换装特效预制体")] public GameObject equipEffectPrefab;
    [Tooltip("特效生成位置")] public Transform effectSpawnPoint;
    
    // 保存的自定义套装
    private List<OutfitSet> customOutfits = new List<OutfitSet>();
    private const string SAVE_KEY = "CustomOutfits";
    
    // 动画相关
    private bool useDOTween = false;
    
    void Start()
    {
        #if DG_TWEENING
        useDOTween = true;
        #endif
        
        LoadCustomOutfits();
        CreateSetButtons();
    }
    
    #region 套装按钮创建
    
    private void CreateSetButtons()
    {
        // 清除旧按钮
        foreach (Transform child in setButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 创建预设套装按钮
        for (int i = 0; i < presetOutfits.Count; i++)
        {
            CreateSetButton(presetOutfits[i], i, true);
        }
        
        // 创建自定义套装按钮
        for (int i = 0; i < customOutfits.Count; i++)
        {
            CreateSetButton(customOutfits[i], i + presetOutfits.Count, false);
        }
        
        // 创建"保存当前"按钮
        CreateSaveCurrentButton();
    }
    
    private void CreateSetButton(OutfitSet set, int index, bool isPreset)
    {
        var btnObj = Instantiate(setButtonPrefab, setButtonContainer);
        
        // 设置图标
        var icon = btnObj.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null && set.setIcon != null)
        {
            icon.sprite = set.setIcon;
        }
        
        // 设置名称
        var nameText = btnObj.transform.Find("Name")?.GetComponent<TMPro.TextMeshProUGUI>();
        if (nameText != null) nameText.text = set.setName;
        
        // 设置点击事件
        var button = btnObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ApplyOutfitSet(set, btnObj));
        }
        
        // 检查是否可用（所有部件都已解锁）
        bool canUse = CanApplyOutfitSet(set);
        var lockIcon = btnObj.transform.Find("Lock")?.gameObject;
        if (lockIcon != null) lockIcon.SetActive(!canUse);
        
        // 按钮入场动画
        StartCoroutine(ButtonEntryAnimation(btnObj, index));
    }
    
    private System.Collections.IEnumerator ButtonEntryAnimation(GameObject btnObj, int index)
    {
        btnObj.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(index * 0.05f);
        
        if (useDOTween)
        {
            #if DG_TWEENING
            btnObj.transform.DOScale(1f, 0.3f)
                .SetEase(DG.Tweening.Ease.OutBack);
            #endif
        }
        else
        {
            // 原生动画
            float timer = 0;
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                float t = timer / 0.3f;
                float scale = Mathf.Lerp(0, 1, Mathf.Sin(t * Mathf.PI * 0.5f));
                btnObj.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            btnObj.transform.localScale = Vector3.one;
        }
    }
    
    private void CreateSaveCurrentButton()
    {
        // 在末尾添加"+"保存按钮
        // 实现略...
    }
    
    #endregion
    
    #region 换装逻辑
    
    /// <summary>
    /// 应用整套换装
    /// </summary>
    public void ApplyOutfitSet(OutfitSet set, GameObject buttonObj = null)
    {
        if (!CanApplyOutfitSet(set))
        {
            // 播放错误动效 - 抖动
            if (buttonObj != null)
                StartCoroutine(ShakeAnimation(buttonObj));
            Debug.Log("该套装包含未解锁的部件！");
            return;
        }
        
        // 播放按钮动效
        if (buttonObj != null)
            StartCoroutine(ButtonPunchAnimation(buttonObj));
        
        // 播放换装特效
        PlayEquipEffect();
        
        // 依次装备每个部件（带延迟动画）
        StartCoroutine(ApplyOutfitWithAnimation(set));
    }
    
    private System.Collections.IEnumerator ApplyOutfitWithAnimation(OutfitSet set)
    {
        // 先卸下所有
        UnequipAllParts();
        yield return new WaitForSeconds(0.2f);
        
        // 依次装备
        foreach (var part in set.parts)
        {
            if (part != null)
            {
                EquipPart(part);
                
                // 播放单个部件装备特效
                PlayPartEquipEffect(part);
                
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        // 完成提示
        Debug.Log($"✨ 已换装：{set.setName}");
    }
    
    private bool CanApplyOutfitSet(OutfitSet set)
    {
        foreach (var part in set.parts)
        {
            if (part != null && !OutfitManager.Instance.IsPartUnlocked(part))
            {
                return false;
            }
        }
        return true;
    }
    
    private void EquipPart(OutfitPartData part)
    {
        OutfitManager.Instance?.EquipPart(part);
    }
    
    private void UnequipAllParts()
    {
        var categories = new[] { 
            OutfitCategory.Bow, 
            OutfitCategory.Hat, 
            OutfitCategory.Glasses,
            OutfitCategory.Scarf,
            OutfitCategory.Backpack 
        };
        
        foreach (var category in categories)
        {
            OutfitManager.Instance?.UnequipPart(category);
        }
    }
    
    private System.Collections.IEnumerator ShakeAnimation(GameObject target)
    {
        Vector3 originalPos = target.transform.position;
        float timer = 0;
        
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float offset = Mathf.Sin(timer * 50) * 5f;
            target.transform.position = originalPos + Vector3.right * offset;
            yield return null;
        }
        
        target.transform.position = originalPos;
    }
    
    private System.Collections.IEnumerator ButtonPunchAnimation(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;
        float timer = 0;
        
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.2f;
            float scale = 1 + Mathf.Sin(t * Mathf.PI) * 0.15f;
            target.transform.localScale = originalScale * scale;
            yield return null;
        }
        
        target.transform.localScale = originalScale;
    }
    
    #endregion
    
    #region 特效
    
    private void PlayEquipEffect()
    {
        if (equipEffectPrefab != null && effectSpawnPoint != null)
        {
            var effect = Instantiate(equipEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
    
    private void PlayPartEquipEffect(OutfitPartData part)
    {
        // 可以播放小星星、闪光等特效
        // 根据部件类型在不同位置播放
    }
    
    #endregion
    
    #region 保存/加载自定义套装
    
    /// <summary>
    /// 将当前装备保存为新套装
    /// </summary>
    public void SaveCurrentAsSet(string setName)
    {
        var newSet = new OutfitSet
        {
            setName = setName,
            parts = new List<OutfitPartData>()
        };
        
        // 获取当前所有装备
        var categories = new[] { 
            OutfitCategory.Bow, 
            OutfitCategory.Hat, 
            OutfitCategory.Glasses,
            OutfitCategory.Scarf,
            OutfitCategory.Backpack 
        };
        
        foreach (var category in categories)
        {
            var part = OutfitManager.Instance?.GetEquippedPart(category);
            if (part != null)
            {
                newSet.parts.Add(part);
            }
        }
        
        customOutfits.Add(newSet);
        SaveCustomOutfits();
        
        // 刷新按钮
        CreateSetButtons();
    }
    
    private void SaveCustomOutfits()
    {
        // 保存部件ID列表
        // 简化实现，实际应该用JSON
    }
    
    private void LoadCustomOutfits()
    {
        // 从 PlayerPrefs 加载
    }
    
    #endregion
}
