using Duckov.UI;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Linq;
using ItemStatsSystem.Items;
using UnityEngine;

namespace BetterAccessoryDisplay
{
    /// slot内物品变更时会触发refresh，再设置一次颜色
    /// Harmony Patch - 拦截 SlotIndicator.Refresh 方法
    [HarmonyPatch(typeof(SlotIndicator), "Refresh")]
    public class SlotIndicatorRefresh
    {
        /// Postfix 方法 - 在 SlotIndicator.Refresh 执行后调用
        static void Postfix(SlotIndicator __instance)
        {
            if (__instance == null) return;

            // 检查 Target 是否为 null（可能在 OnEnable 时还未调用 Setup）
            if (__instance.Target == null)
            {
                Debug.LogWarning($"[{Constants.MOD_NAME}] SlotIndicator.Target 为 null，跳过颜色设置");
                return;
            }

            Debug.Log($"[{Constants.MOD_NAME}] SlotIndicatorRefresh SlotIndicator.Refresh Postfix");
            
            //  使用反射获取 private 字段 contentIndicator
            var fieldInfo = typeof(SlotIndicator).GetField(
                "contentIndicator",
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance
            );
            
            if (fieldInfo == null)
            {
                Debug.LogWarning($"[{Constants.MOD_NAME}] contentIndicator FieldInfo 是 null，请检查 Prefab 或 Setup 逻辑");
                return;
            }
            
            // 获取该字段在 __instance 上的值（即 private GameObject contentIndicator）
            var contentIndicatorObj = fieldInfo.GetValue(__instance) as GameObject;
            if (contentIndicatorObj != null)
            {
                if ((bool)(UnityEngine.Object)__instance.Target.Content)
                {
                    var img = contentIndicatorObj.GetComponent<UnityEngine.UI.Image>();
                    if (img != null)
                    {
                        img.color = Color.white; // 默认白色
                        Debug.Log($"[{Constants.MOD_NAME}] SlotIndicatorRefresh 已经将原有槽位设置为白色");
                    }
                    Debug.Log($"[{Constants.MOD_NAME}] Slot内的Item名称: {__instance.Target.Content?.name ?? "NULL"}");
                }
                else
                {
                    if (__instance.Target.Key == "TempSlot")
                    {
                        // 检查是否是 Image
                        contentIndicatorObj.SetActive(true);
                        var img = contentIndicatorObj.GetComponent<UnityEngine.UI.Image>();
                        if (img != null)
                        {
                            img.color = Color.red; // 强制改成红色
                            Debug.Log($"[{Constants.MOD_NAME}] SlotIndicatorRefresh 已经将新增槽位设置为红色");
                        }
                    }
                }
            }
        }
    }
}

