using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI动效工具类 - 提供常用UI动画效果
/// 羊毛毡手作风格专用动效
/// 兼容DOTween和原生动画
/// </summary>
public static class UIAnimationHelper
{
    // 检测DOTween是否可用
    private static bool useDOTween = false;
    
    static UIAnimationHelper()
    {
        #if DG_TWEENING
        useDOTween = true;
        #endif
    }
    
    #region 按钮动效
    
    /// <summary>
    /// 羊毛毡按钮点击效果 - 按压回弹
    /// </summary>
    public static void PlayFluffyButtonClick(Transform button, float scaleAmount = 0.95f, float duration = 0.15f)
    {
        if (useDOTween)
        {
            #if DG_TWEENING
            button.DOScale(scaleAmount, duration)
                .SetEase(DG.Tweening.Ease.OutQuad)
                .OnComplete(() => {
                    button.DOScale(1f, duration * 1.5f)
                        .SetEase(DG.Tweening.Ease.OutBack, 1.5f);
                });
            #endif
        }
        else
        {
            // 原生动画简化版
            var mono = button.GetComponent<MonoBehaviour>();
            if (mono != null)
                mono.StartCoroutine(FluffyButtonClickNative(button, scaleAmount, duration));
        }
    }
    
    private static System.Collections.IEnumerator FluffyButtonClickNative(Transform button, float scaleAmount, float duration)
    {
        // 按下
        float timer = 0;
        Vector3 originalScale = button.localScale;
        Vector3 targetScale = originalScale * scaleAmount;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            button.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        button.localScale = targetScale;
        
        // 回弹
        timer = 0;
        float backDuration = duration * 1.5f;
        while (timer < backDuration)
        {
            timer += Time.deltaTime;
            float t = timer / backDuration;
            // 回弹效果
            float overshoot = 1.2f;
            t = Mathf.Sin(t * Mathf.PI * 0.5f) * overshoot;
            if (t > 1) t = 2 - t;
            button.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        button.localScale = originalScale;
    }
    
    /// <summary>
    /// 按钮悬停效果 - 轻微放大
    /// </summary>
    public static void PlayButtonHover(Transform button, bool isEnter, float targetScale = 1.1f)
    {
        if (useDOTween)
        {
            #if DG_TWEENING
            button.DOScale(isEnter ? targetScale : 1f, 0.2f)
                .SetEase(DG.Tweening.Ease.OutQuad);
            #endif
        }
        else
        {
            button.localScale = isEnter ? Vector3.one * targetScale : Vector3.one;
        }
    }
    
    /// <summary>
    /// 按钮禁用抖动 - 拒绝操作
    /// </summary>
    public static void PlayButtonReject(Transform button)
    {
        var mono = button.GetComponent<MonoBehaviour>();
        if (mono != null)
            mono.StartCoroutine(ShakeAnimationNative(button, 0.3f, 5f));
    }
    
    private static System.Collections.IEnumerator ShakeAnimationNative(Transform target, float duration, float strength)
    {
        Vector3 originalPos = target.position;
        float timer = 0;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float offset = Mathf.Sin(timer * 50) * strength;
            target.position = originalPos + Vector3.right * offset;
            yield return null;
        }
        
        target.position = originalPos;
    }
    
    #endregion
    
    #region 弹窗动效
    
    /// <summary>
    /// 弹窗打开动画 - 从底部滑入+弹性
    /// </summary>
    public static void PlayPopupOpen(RectTransform panel, CanvasGroup overlay, System.Action onComplete = null)
    {
        if (useDOTween)
        {
            #if DG_TWEENING
            // 初始状态
            panel.anchoredPosition = new Vector2(0, -Screen.height);
            overlay.alpha = 0f;
            
            // 遮罩渐显
            DG.Tweening.DOTween.To(() => overlay.alpha, x => overlay.alpha = x, 0.6f, 0.3f)
                .SetEase(DG.Tweening.Ease.OutQuad);
            
            // 面板滑入
            DG.Tweening.DOTween.To(() => panel.anchoredPosition.y, 
                y => panel.anchoredPosition = new Vector2(0, y), 0, 0.5f)
                .SetEase(DG.Tweening.Ease.OutBack)
                .OnComplete(() => onComplete?.Invoke());
            #endif
        }
        else
        {
            var mono = panel.GetComponent<MonoBehaviour>();
            if (mono != null)
                mono.StartCoroutine(PopupOpenNative(panel, overlay, onComplete));
        }
    }
    
    private static System.Collections.IEnumerator PopupOpenNative(RectTransform panel, CanvasGroup overlay, System.Action onComplete)
    {
        panel.anchoredPosition = new Vector2(0, -Screen.height);
        overlay.alpha = 0f;
        
        float timer = 0;
        float duration = 0.5f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            // 弹性缓出
            float elastic = Mathf.Sin(t * Mathf.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t;
            
            panel.anchoredPosition = new Vector2(0, Mathf.Lerp(-Screen.height, 0, elastic));
            overlay.alpha = Mathf.Lerp(0, 0.6f, t);
            
            yield return null;
        }
        
        panel.anchoredPosition = Vector2.zero;
        overlay.alpha = 0.6f;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 弹窗关闭动画 - 下滑消失
    /// </summary>
    public static void PlayPopupClose(RectTransform panel, CanvasGroup overlay, System.Action onComplete = null)
    {
        if (useDOTween)
        {
            #if DG_TWEENING
            DG.Tweening.DOTween.To(() => panel.anchoredPosition.y, 
                y => panel.anchoredPosition = new Vector2(0, y), -Screen.height, 0.3f)
                .SetEase(DG.Tweening.Ease.InQuad);
            
            DG.Tweening.DOTween.To(() => overlay.alpha, x => overlay.alpha = x, 0f, 0.3f)
                .SetEase(DG.Tweening.Ease.InQuad)
                .OnComplete(() => onComplete?.Invoke());
            #endif
        }
        else
        {
            var mono = panel.GetComponent<MonoBehaviour>();
            if (mono != null)
                mono.StartCoroutine(PopupCloseNative(panel, overlay, onComplete));
        }
    }
    
    private static System.Collections.IEnumerator PopupCloseNative(RectTransform panel, CanvasGroup overlay, System.Action onComplete)
    {
        float timer = 0;
        float duration = 0.3f;
        Vector2 startPos = panel.anchoredPosition;
        float startAlpha = overlay.alpha;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            panel.anchoredPosition = Vector2.Lerp(startPos, new Vector2(0, -Screen.height), t * t);
            overlay.alpha = Mathf.Lerp(startAlpha, 0, t);
            
            yield return null;
        }
        
        onComplete?.Invoke();
    }
    
    #endregion
    
    #region 列表项动效
    
    /// <summary>
    /// 列表项依次入场 - 弹性弹出
    /// </summary>
    public static void PlayListItemEntry(Transform item, int index, float delayPerItem = 0.05f)
    {
        var mono = item.GetComponent<MonoBehaviour>();
        if (mono != null)
            mono.StartCoroutine(ListItemEntryNative(item, index, delayPerItem));
    }
    
    private static System.Collections.IEnumerator ListItemEntryNative(Transform item, int index, float delayPerItem)
    {
        item.localScale = Vector3.zero;
        yield return new WaitForSeconds(index * delayPerItem);
        
        float timer = 0;
        float duration = 0.3f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            // 回弹效果
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.2f;
            if (scale > 1) scale = 2 - scale;
            item.localScale = Vector3.one * Mathf.Max(0, scale);
            yield return null;
        }
        
        item.localScale = Vector3.one;
    }
    
    #endregion
    
    #region 标签切换动效
    
    /// <summary>
    /// 标签切换效果 - 缩放+颜色
    /// </summary>
    public static void PlayTabSwitch(Image tabImage, bool isSelected, 
        Color selectedColor, Color unselectedColor)
    {
        if (useDOTween)
        {
            #if DG_TWEENING
            tabImage.DOColor(isSelected ? selectedColor : unselectedColor, 0.2f);
            tabImage.transform.DOScale(isSelected ? 1.1f : 1f, 0.2f)
                .SetEase(DG.Tweening.Ease.OutQuad);
            #endif
        }
        else
        {
            tabImage.color = isSelected ? selectedColor : unselectedColor;
            tabImage.transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
        }
    }
    
    #endregion
    
    #region 反馈动效
    
    /// <summary>
    /// 成功反馈 - 弹跳+放大
    /// </summary>
    public static void PlaySuccessFeedback(Transform target)
    {
        var mono = target.GetComponent<MonoBehaviour>();
        if (mono != null)
            mono.StartCoroutine(SuccessFeedbackNative(target));
    }
    
    private static System.Collections.IEnumerator SuccessFeedbackNative(Transform target)
    {
        Vector3 originalScale = target.localScale;
        float timer = 0;
        float duration = 0.4f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float scale = 1 + Mathf.Sin(t * Mathf.PI * 4) * 0.1f * (1 - t);
            target.localScale = originalScale * scale;
            yield return null;
        }
        
        target.localScale = originalScale;
    }
    
    /// <summary>
    /// 提示文字弹出 - 渐显上飘
    /// </summary>
    public static void PlayToastPopup(Transform toast, string message)
    {
        var text = toast.GetComponent<TextMeshProUGUI>();
        if (text != null) text.text = message;
        
        var mono = toast.GetComponent<MonoBehaviour>();
        if (mono != null)
            mono.StartCoroutine(ToastPopupNative(toast));
    }
    
    private static System.Collections.IEnumerator ToastPopupNative(Transform toast)
    {
        toast.localScale = Vector3.zero;
        var canvasGroup = toast.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = toast.gameObject.AddComponent<CanvasGroup>();
        
        canvasGroup.alpha = 1f;
        
        // 弹入
        float timer = 0;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.2f;
            toast.localScale = Vector3.one * Mathf.Sin(t * Mathf.PI * 0.5f);
            yield return null;
        }
        toast.localScale = Vector3.one;
        
        // 等待
        yield return new WaitForSeconds(1.5f);
        
        // 渐隐
        timer = 0;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.3f;
            canvasGroup.alpha = 1 - t;
            toast.localScale = Vector3.one * (1 - t * 0.2f);
            yield return null;
        }
        
        canvasGroup.alpha = 0;
    }
    
    /// <summary>
    /// 金币/货币增加动效 - 数字跳动
    /// </summary>
    public static void PlayCoinIncrease(TextMeshProUGUI coinText, int fromValue, int toValue, float duration = 0.8f)
    {
        var mono = coinText.GetComponent<MonoBehaviour>();
        if (mono != null)
            mono.StartCoroutine(CoinIncreaseNative(coinText, fromValue, toValue, duration));
    }
    
    private static System.Collections.IEnumerator CoinIncreaseNative(TextMeshProUGUI coinText, int fromValue, int toValue, float duration)
    {
        float timer = 0;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = 1 - (1 - t) * (1 - t); // EaseOutQuad
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(fromValue, toValue, t));
            coinText.text = $"💰 {currentValue}";
            yield return null;
        }
        
        coinText.text = $"💰 {toValue}";
        
        // 弹跳效果
        Vector3 originalScale = coinText.transform.localScale;
        timer = 0;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.3f;
            float scale = 1 + Mathf.Sin(t * Mathf.PI) * 0.2f;
            coinText.transform.localScale = originalScale * scale;
            yield return null;
        }
        coinText.transform.localScale = originalScale;
    }
    
    #endregion
    
    #region 羊毛毡风格特效
    
    /// <summary>
    /// 毛茸茸边框呼吸效果 - 轻微缩放
    /// </summary>
    public static void PlayFluffyBreath(Transform target, float scaleRange = 0.02f, float duration = 2f)
    {
        var mono = target.GetComponent<MonoBehaviour>();
        if (mono != null)
            mono.StartCoroutine(FluffyBreathNative(target, scaleRange, duration));
    }
    
    private static System.Collections.IEnumerator FluffyBreathNative(Transform target, float scaleRange, float duration)
    {
        Vector3 originalScale = target.localScale;
        float timer = 0;
        
        while (true)
        {
            timer += Time.deltaTime;
            float t = Mathf.Sin(timer / duration * Mathf.PI * 2) * 0.5f + 0.5f;
            float scale = 1 + t * scaleRange;
            target.localScale = originalScale * scale;
            yield return null;
        }
    }
    
    #endregion
}

/// <summary>
/// 按钮动效组件 - 直接挂载到按钮上使用
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonAnimation : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, 
    UnityEngine.EventSystems.IPointerUpHandler, UnityEngine.EventSystems.IPointerEnterHandler,
    UnityEngine.EventSystems.IPointerExitHandler
{
    [Header("【动效设置】")]
    [Tooltip("点击缩放比例")] public float clickScale = 0.95f;
    [Tooltip("悬停缩放比例")] public float hoverScale = 1.05f;
    [Tooltip("动画时长")] public float duration = 0.15f;
    
    private Button button;
    private Vector3 originalScale;
    
    void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }
    
    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!button.interactable) return;
        UIAnimationHelper.PlayFluffyButtonClick(transform, clickScale, duration);
    }
    
    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // 回弹在PlayFluffyButtonClick中处理
    }
    
    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!button.interactable) return;
        UIAnimationHelper.PlayButtonHover(transform, true, hoverScale);
    }
    
    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        UIAnimationHelper.PlayButtonHover(transform, false);
    }
}
