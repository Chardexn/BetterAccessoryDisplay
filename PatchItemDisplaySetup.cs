using Duckov.UI;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Linq;
using System.Reflection;
using ItemStatsSystem.Items;
using UnityEngine;

namespace BetterAccessoryDisplay
{
    /// Harmony Patch - 拦截 ItemDisplay.Setup 方法
    [HarmonyPatch(typeof(ItemDisplay), "Setup")]
    public class PatchItemDisplaySetup
    {
        // static void Replacement(ItemDisplay __instance, Item target)
        // {
        //     if (__instance == null || target == null) return;
        //     if (!IsGunItem(target)) return;
        //     
        //     __instance.UnregisterEvents();
        //     __instance.Target = target;
        //     __instance.Clear();
        //     __instance.slotIndicatorTemplate.gameObject.SetActive(false);
        //     if ((Object) target == (Object) null)
        //     {
        //         __instance.SetupEmpty();
        //     }
        //     else
        //     {
        //         __instance.icon.color = Color.white;
        //         __instance.icon.sprite = target.Icon;
        //         if ((Object) __instance.icon.sprite == (Object) null)
        //             __instance.icon.sprite = __instance.FallbackIcon;
        //         __instance.icon.gameObject.SetActive(true);
        //         (__instance.displayQualityShadow.OffsetDistance, __instance.displayQualityShadow.Color, __instance.displayQualityShadow.Inset) = GameplayDataSettings.UIStyle.GetShadowOffsetAndColorOfQuality(target.DisplayQuality);
        //         __instance.countGameObject.SetActive(__instance.Target.Stackable);
        //         __instance.nameText.text = __instance.Target.DisplayName;
        //         if ((Object) target.Slots != (Object) null)
        //         {
        //             foreach (Slot slot in target.Slots)
        //                 __instance.SlotIndicatorPool.Get().Setup(slot);
        //         }
        //     }
        //     __instance.Refresh();
        //     if (!__instance.isActiveAndEnabled)
        //         return;
        //     __instance.RegisterEvents();
        //     
        // }
        
        static void Prefix(ItemDisplay __instance, Item target)
        {
            if (__instance == null || target == null) return;
            if (!IsGunItem(target)) return;
            
            var oldIndicators = __instance.transform.GetComponentsInChildren<SlotIndicator>(true);
            Debug.Log($"Item[{target.DisplayName}] oldIndicators Num [{oldIndicators.Length}]");
            foreach (var old in oldIndicators)
            {
                Debug.Log($"Item[{target.DisplayName}] oldIndicators Name [{old.name}]");
                if (old != null && old.gameObject != null && old.name != "SlotIndicator")
                    UnityEngine.Object.Destroy(old.gameObject);
            }
        }
        /// Postfix 方法 - 在 ItemDisplay.Setup 执行后调用
        static void Postfix(ItemDisplay __instance, Item target)
        {
            // 参数检查
            if (__instance == null || target == null) return;
            if (!IsGunItem(target)) return;
            
            // 打印slot信息
            PrintSlotAndTagInfos(target);
            
            Debug.Log($"Item [{target.DisplayName}] ItemDisplay.Setup Postfix");
            
            // ---- Step 1: 删除旧的 SlotIndicator ----
            var oldIndicators = __instance.transform.GetComponentsInChildren<SlotIndicator>(true);
            Debug.Log($"Item[{target.DisplayName}] oldIndicators Num [{oldIndicators.Length}]");
            foreach (var old in oldIndicators)
            {
                Debug.Log($"Item[{target.DisplayName}] oldIndicators Name [{old.name}]");
                if (old != null && old.gameObject != null && old.name != "SlotIndicator")
                    UnityEngine.Object.Destroy(old.gameObject);
            }
            // Debug.Log($"Item [{target.DisplayName}] Delete pre Indicator");
            
            // ✅ 使用反射获取 private 字段 SlotIndicatorPool
            PropertyInfo slotIndicatorPoolProp = typeof(ItemDisplay).GetProperty(
                "SlotIndicatorPool", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            // Debug.Log($"PatchItemDisplaySetup Item [{target.DisplayName}] Get SlotIndicatorPool");

            if (slotIndicatorPoolProp != null)
            {
                // 获取该字段在 __instance 上的值（即 private GameObject slotIndicatorPoolObj）
                var slotIndicatorPoolObj = slotIndicatorPoolProp.GetValue(__instance) as PrefabPool<SlotIndicator>;
                if (slotIndicatorPoolObj != null)
                {
                    Debug.Log($"Item [{target.DisplayName}] Create SlotIndicator");
                    // ---- Step 2: 创建 6 个新的固定 SlotIndicator ----
                    for (int i = 0; i < 6; i++)
                    {
                        var slotName = Constants.GUN_SLOTS[i];
                        Debug.Log($"Slot [{slotName}] Checking");
                        
                        var curSlot = new Slot("TempSlot");
                        foreach (Slot slot in target.Slots)
                        {
                            if (slot.Key == slotName)
                            {
                                curSlot = slot;
                                Debug.Log($"Slot [{slotName}] Existing√！");
                            }
                        }
                        var SI = slotIndicatorPoolObj.Get();
                        SI.Setup(curSlot);
                        SI.name = $"SlotIndicator_{i}";
                    }
                }
                else
                {
                    Debug.LogWarning("PatchItemDisplaySetup slotIndicatorPoolObj是 null，请检查逻辑");
                }
            }

            
        }

        /// 检查物品是否为武器
        private static bool IsGunItem(Item item)
        {
            if (item == null) return false;

            try
            {
                // 使用 TagCollection.Contains(string) 方法检查是否有 "Key" 标签
                return item.Tags != null && item.Tags.Contains(Constants.GUN_TAG);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[{Constants.MOD_NAME}] Error checking if item is gun: {ex.Message}");
                return false;
            }
        }
        
        /// 打印槽位信息
        private static void PrintSlotAndTagInfos(Item target)
        {
            if (!Constants.DEBUG_MODE) return;
            if (target == null) return;
            
            if ((UnityEngine.Object) target.Slots != (UnityEngine.Object) null)
            {
                Debug.Log($"🔍 Item [{target.DisplayName}] 上的 Slot 数量: {target.Slots.Count}");
                foreach (Slot slot in target.Slots)
                    Debug.Log($"   → slot: {slot.Key}");
            }
            else
            {
                Debug.Log($"🔍 Item [{target.DisplayName}] 没有 Slot (Slot == null)");
            }
        }
    }
}

