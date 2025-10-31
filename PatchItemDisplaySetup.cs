using Duckov.UI;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Linq;
using System.Reflection;
using BetterAccessoryDisplay;
using ItemStatsSystem.Items;
using UnityEngine;

namespace DuckovModDemo
{
    /// Harmony Patch - 拦截 ItemDisplay.Setup 方法
    [HarmonyPatch(typeof(ItemDisplay), "Setup")]
    public class PatchItemDisplaySetup
    {
        /// Postfix 方法 - 在 ItemDisplay.Setup 执行后调用
        static void Postfix(ItemDisplay __instance, Item target)
        {
            // 参数检查
            if (__instance == null || target == null)
            {
                return;
            }

            // 只为武器类型处理
            if (!IsGunItem(target))
            {
                return;
            }

            Debug.Log($"[{Constants.MOD_NAME}] 处理武器物品: {target.DisplayName}");

            // 创建固定插槽
            RecreateFixedSlots(__instance, target);
        }

        /// 公共方法：重新创建固定插槽（供 Refresh 调用）
        public static void RecreateFixedSlots(ItemDisplay instance, Item target)
        {
            // 删除原有插槽
            ClearAllSlotIndicators(instance);

            // 创建6个固定插槽
            CreateFixedSlotIndicators(instance, target);
        }

        /// 清空所有插槽指示器
        private static void ClearAllSlotIndicators(ItemDisplay instance)
        {
            try
            {
                // 使用反射获取 SlotIndicatorPool 属性
                var poolProperty = typeof(ItemDisplay).GetProperty(
                    "SlotIndicatorPool",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                if (poolProperty != null)
                {
                    PrefabPool<SlotIndicator> pool = poolProperty.GetValue(instance) as PrefabPool<SlotIndicator>;
                    pool.ReleaseAll();
                    Debug.Log($"[{Constants.MOD_NAME}] 已清空所有插槽指示器");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Constants.MOD_NAME}] 清空插槽时出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// 创建6个固定插槽指示器
        private static void CreateFixedSlotIndicators(ItemDisplay instance, Item target)
        {
            try
            {
                // 使用反射获取 SlotIndicatorPool 属性
                var poolProperty = typeof(ItemDisplay).GetProperty(
                    "SlotIndicatorPool",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                if (poolProperty == null)
                {
                    Debug.LogError($"[{Constants.MOD_NAME}] 无法获取 SlotIndicatorPool 属性");
                    return;
                }

                PrefabPool<SlotIndicator> pool = poolProperty.GetValue(instance) as PrefabPool<SlotIndicator>;
                if (pool == null)
                {
                    Debug.LogError($"[{Constants.MOD_NAME}] SlotIndicatorPool 为 null");
                    return;
                }

                // 获取物品实际拥有的插槽列表
                var itemSlots = target.Slots;
                int itemSlotCount = itemSlots != null ? itemSlots.Count : 0;

                // 创建6个固定插槽
                const int FIXED_SLOT_COUNT = 6;
                for (int i = 0; i < FIXED_SLOT_COUNT; i++)
                {
                    // 从池中获取 SlotIndicator
                    var slotIndicator = pool.Get() as SlotIndicator;
                    if (slotIndicator == null)
                    {
                        Debug.LogWarning($"[{Constants.MOD_NAME}] 无法从池中获取 SlotIndicator (索引: {i})");
                        continue;
                    }

                    // 创建一个临时插槽
                    Slot slotToUse = null;

                    var slotName = Constants.GUN_SLOTS[i];
                    bool bIsExist = false;
                    
                    // 判断这是物品拥有的插槽还是额外的插槽
                    foreach (Slot slot in target.Slots)
                    {
                        if (slot.Key == slotName)
                        {
                            slotToUse = slot;
                            bIsExist = true;
                        }
                    }

                    if (!bIsExist)
                    {
                        slotToUse = new Slot("TempSlot");
                    }

                    // 设置插槽指示器
                    if (slotToUse != null)
                    {
                        slotIndicator.Setup(slotToUse);
                        Debug.Log($"[{Constants.MOD_NAME}] 创建插槽 {i + 1}/{FIXED_SLOT_COUNT}: {(bIsExist ? "原有" : "新建")}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Constants.MOD_NAME}] 创建插槽时出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// 检查物品是否为武器（设为public供其他类使用）
        public static bool IsGunItem(Item item)
        {
            if (item == null) return false;

            try
            {
                return item.Tags != null && item.Tags.Contains(Constants.GUN_TAG);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[{Constants.MOD_NAME}] Error checking if item is gun: {ex.Message}");
                return false;
            }
        }
    }
}

