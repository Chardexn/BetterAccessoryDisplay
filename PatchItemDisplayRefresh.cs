using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem.Items;
using System.Reflection;
using BetterAccessoryDisplay;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovModDemo
{
    /// Harmony Patch - 拦截 ItemDisplay.Refresh 方法
    /// 关键修复：确保在 Refresh 时重新创建固定插槽，避免点击后插槽被隐藏的问题
    [HarmonyPatch(typeof(ItemDisplay), "Refresh")]
    public class PatchItemDisplayRefresh
    {
        /// Postfix 方法 - 在 ItemDisplay.Refresh 执行后调用
        /// 当 Refresh 被调用时（例如点击物品选择时），重新确保插槽正确显示
        static void Postfix(ItemDisplay __instance)
        {
            // 参数检查
            if (__instance == null)
            {
                return;
            }

            // 获取 Target 属性
            var targetProperty = typeof(ItemDisplay).GetProperty("Target", BindingFlags.Public | BindingFlags.Instance);
            if (targetProperty == null)
            {
                return;
            }

            var target = targetProperty.GetValue(__instance) as Item;
            if (target == null)
            {
                return;
            }

            // 只为武器类型处理
            if (!IsGunItem(target))
            {
                return;
            }

            // 关键修复：Refresh 后重新创建插槽，避免插槽被隐藏 by Cursor
            try
            {
                // 确保插槽容器是激活的
                var slotContainerField = typeof(ItemDisplay).GetField(
                    "slotIndicatorContainer",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                if (slotContainerField != null)
                {
                    var container = slotContainerField.GetValue(__instance) as GameObject;
                    if (container != null && !container.activeSelf)
                    {
                        // 未搜索时不显示
                        if (__instance.Target.NeedInspection)
                        {
                            container.SetActive(false);
                            return;
                        }
                        else
                        {
                            container.SetActive(true);
                        }
                        Debug.Log($"[{Constants.MOD_NAME}] Refresh后重新激活插槽容器");
                    }
                }

                // 重新创建插槽（关键修复：确保插槽数量正确）
                PatchItemDisplaySetup.RecreateFixedSlots(__instance, target);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[{Constants.MOD_NAME}] Refresh Postfix 处理时出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// 检查物品是否为武器
        private static bool IsGunItem(Item item)
        {
            if (item == null) return false;

            try
            {
                return item.Tags != null && item.Tags.Contains(Constants.GUN_TAG);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[{Constants.MOD_NAME}] Error checking if item is gun: {ex.Message}");
                return false;
            }
        }
    }
}
