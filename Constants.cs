using UnityEngine;

namespace BetterAccessoryDisplay
{
    /// <summary>
    /// Mod 常量定义
    /// </summary>
    public static class Constants
    {
        // ==================== Mod 信息 ====================
        
        /// Mod 唯一标识符
        public const string MOD_ID = "BetterAccessoryDisplay";

        /// Mod 显示名称
        public const string MOD_NAME = "更好的配件展示";

        /// Mod 版本
        public const string MOD_VERSION = "1.0.0";

        // ==================== 调试 ====================

        /// 是否启用调试模式
        public const bool DEBUG_MODE = true;

        // ==================== 其他 ====================
        /// 武器Tag
        public const string GUN_TAG = "Gun";
        
        // 武器插槽名称
        public static string[] GUN_SLOTS = { "Scope", "Muzzle", "Grip", "Stock", "Tec", "Mag" };
    }
}

