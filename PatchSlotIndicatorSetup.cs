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
    /// Harmony Patch - 拦截 SlotIndicator.Setup 方法
    [HarmonyPatch(typeof(SlotIndicator), "Setup")]
    public class PatchSlotIndicatorSetup
    {
        /// Postfix 方法 - 在 SlotIndicator.Setup 执行后调用
        static void Postfix(SlotIndicator __instance, Slot target)
        {
            if (__instance == null || target == null) return;

            Debug.Log("PatchSlotIndicatorSetup SlotIndicator.Setup Postfix");
            
            // ✅ 使用反射获取 private 字段 contentIndicator
            var fieldInfo = typeof(SlotIndicator).GetField(
                "contentIndicator",
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance
            );
            // Debug.Log("PatchSlotIndicatorSetup Get contentIndicator");
            
            if (fieldInfo != null)
            {
                // 获取该字段在 __instance 上的值（即 private GameObject contentIndicator）
                var contentIndicatorObj = fieldInfo.GetValue(__instance) as GameObject;
                if (contentIndicatorObj != null)
                {
                    Debug.Log($"PatchSlotIndicatorSetup Slot的Key名称: {target.Key ?? "NULL"}");

                    if ((bool)(UnityEngine.Object)target.Content)
                    {
                        Debug.Log($"Slot内的Item名称: {target.Content.name ??"NULL"}");
                    }
                    else
                    {
                        // Debug.Log("PatchSlotIndicatorSetup 该Slot内没有Item");
                        if (target.Key == "TempSlot")
                        {
                            // 检查是否是 Image
                            contentIndicatorObj.SetActive(true);
                            var img = contentIndicatorObj.GetComponent<UnityEngine.UI.Image>();
                            if (img != null)
                            {
                                img.color = Color.red; // 强制改成红色
                                Debug.Log("PatchSlotIndicatorSetup 已经将新增槽位设置为红色");
                            }
                        }
                    }
                    
                    
                    
                    // // 打印它有哪些组件
                    // var components = contentIndicatorObj.GetComponents<Component>();
                    // foreach (var c in components)
                    // {   
                    //     if (c != null)
                    //         // Debug.Log($"   → 组件: {c.GetType().Name}");
                    //         Debug.Log($"挂载的组件类名: {c.GetType().Name}, 完整类型: {c.GetType().FullName}\"");
                    // }
                }
            }
            else
            {
                Debug.LogWarning("⚠️ contentIndicator 是 null，请检查 Prefab 或 Setup 逻辑");
            }
        }
    }
}

