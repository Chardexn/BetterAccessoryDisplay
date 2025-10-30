using Duckov.Modding;
using HarmonyLib;
using System;
using System.Reflection;
using Duckov.MasterKeys;
using UnityEngine;

namespace BetterAccessoryDisplay
{
    /// 负责初始化 Harmony Patch 和事件订阅
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony? harmony;

        /// Mod 加载时调用
        void Awake()
        {
            Debug.Log($"[{Constants.MOD_NAME}] Loaded!!!");
        }

        /// Mod 启用时调用
        /// 初始化 Harmony Patch 和事件订阅
        void OnEnable()
        {
            Debug.Log($"[{Constants.MOD_NAME}] OnEnable");

            try
            {
                // 创建 Harmony 实例
                harmony = new Harmony(Constants.MOD_ID);

                // 注册所有 Patch
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log($"[{Constants.MOD_NAME}] Harmony patches applied successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Constants.MOD_NAME}] Error in OnEnable: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// Mod 禁用时调用
        /// 卸载 Harmony Patch
        void OnDisable()
        {
            Debug.Log($"[{Constants.MOD_NAME}] OnDisable");

            try
            {
                if (harmony != null)
                {
                    harmony.UnpatchAll(Constants.MOD_ID);
                    Debug.Log($"[{Constants.MOD_NAME}] Harmony patches removed successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{Constants.MOD_NAME}] Error in OnDisable: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// Mod 销毁时调用
        void OnDestroy()
        {
            Debug.Log($"[{Constants.MOD_NAME}] OnDestroy");
        }
    }
}

