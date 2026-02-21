using System.Collections.Generic;
using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 升级选项数据
    /// </summary>
    [System.Serializable]
    public class ElementUpgradeOption
    {
        public ElementType element;
        public int currentLevel;
        public bool isNewElement; // 是否是新获得属性
        public string title;
        public string description;
        public Sprite icon;
    }
    
    /// <summary>
    /// 属性升级系统 - 管理三选一升级
    /// </summary>
    public class ElementUpgradeSystem : MonoBehaviour
    {
        public static ElementUpgradeSystem Instance { get; private set; }
        
        [Header("当前属性状态")]
        public ElementType currentElement = ElementType.None;
        public int currentLevel = 0;
        
        [Header("升级配置")]
        public int maxLevel = 3;
        
        // 玩家当前拥有的属性等级
        private Dictionary<ElementType, int> elementLevels = new Dictionary<ElementType, int>();
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // 初始化
            Initialize();
        }
        
        void Initialize()
        {
            elementLevels[ElementType.Fire] = 0;
            elementLevels[ElementType.Ice] = 0;
            elementLevels[ElementType.Electric] = 0;
        }
        
        /// <summary>
        /// 生成三个升级选项
        /// </summary>
        public List<ElementUpgradeOption> GenerateOptions()
        {
            var options = new List<ElementUpgradeOption>();
            var availableElements = new List<ElementType> 
            { 
                ElementType.Fire, 
                ElementType.Ice, 
                ElementType.Electric 
            };
            
            // 如果已有属性且未满级，优先提供强化选项
            if (currentElement != ElementType.None && currentLevel < maxLevel)
            {
                // 40%几率出现当前属性强化
                if (Random.value < 0.4f)
                {
                    options.Add(CreateUpgradeOption(currentElement, currentLevel + 1, false));
                    availableElements.Remove(currentElement);
                }
            }
            
            // 填充剩余选项
            while (options.Count < 3 && availableElements.Count > 0)
            {
                int index = Random.Range(0, availableElements.Count);
                var element = availableElements[index];
                availableElements.RemoveAt(index);
                
                int level = elementLevels[element];
                bool isNew = (level == 0);
                
                // 已有属性且未满级，可能提供强化
                if (!isNew && level < maxLevel && Random.value < 0.5f)
                {
                    options.Add(CreateUpgradeOption(element, level + 1, false));
                }
                else if (isNew)
                {
                    options.Add(CreateUpgradeOption(element, 1, true));
                }
            }
            
            // 打乱顺序
            Shuffle(options);
            
            return options;
        }
        
        ElementUpgradeOption CreateUpgradeOption(ElementType element, int level, bool isNew)
        {
            var option = new ElementUpgradeOption
            {
                element = element,
                currentLevel = level,
                isNewElement = isNew,
                title = GetTitle(element, level, isNew),
                description = GetDescription(element, level)
            };
            
            return option;
        }
        
        string GetTitle(ElementType element, int level, bool isNew)
        {
            string elementName = GetElementName(element);
            if (isNew)
            {
                return $"觉醒：{elementName}之力";
            }
            return $"{elementName}强化 Lv.{level}";
        }
        
        string GetDescription(ElementType element, int level)
        {
            switch (element)
            {
                case ElementType.Fire:
                    return GetFireDescription(level);
                case ElementType.Ice:
                    return GetIceDescription(level);
                case ElementType.Electric:
                    return GetElectricDescription(level);
                default:
                    return "";
            }
        }
        
        string GetFireDescription(int level)
        {
            var effect = new FireEffect(10f, level);
            return effect.GetDescription();
        }
        
        string GetIceDescription(int level)
        {
            var effect = new IceEffect(level);
            return effect.GetDescription();
        }
        
        string GetElectricDescription(int level)
        {
            var effect = new ElectricEffect(level);
            return effect.GetDescription();
        }
        
        string GetElementName(ElementType type)
        {
            switch (type)
            {
                case ElementType.Fire: return "火焰";
                case ElementType.Ice: return "冰霜";
                case ElementType.Electric: return "雷电";
                default: return "无";
            }
        }
        
        /// <summary>
        /// 选择升级
        /// </summary>
        public void SelectUpgrade(ElementUpgradeOption option)
        {
            currentElement = option.element;
            currentLevel = option.currentLevel;
            elementLevels[option.element] = option.currentLevel;
            
            Debug.Log($"[Upgrade] 选择升级: {GetElementName(option.element)} Lv.{option.currentLevel}");
            
            // TODO: 应用升级效果到玩家
            ApplyUpgradeToPlayer(option);
        }
        
        void ApplyUpgradeToPlayer(ElementUpgradeOption option)
        {
            // 这里可以触发事件或调用玩家武器系统
            // EventManager.Instance.TriggerElementChanged(option.element, option.currentLevel);
        }
        
        void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
        
        /// <summary>
        /// 获取当前属性等级
        /// </summary>
        public int GetElementLevel(ElementType type)
        {
            if (elementLevels.ContainsKey(type))
                return elementLevels[type];
            return 0;
        }
    }
}
