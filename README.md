# BetterAccessoryDisplay

## 功能介绍

鸭科夫原有的武器item上会显示该武器可以安装的插槽，但是经常需要在不同武器之间交换配件，此时仅依靠item上显示的插槽不便于识别。

此mod强制让所有武器显示6个插槽，并将该武器没有的插槽显示为红色方便辨认。

可能之后会考虑给武器详情页也统一修改为6个槽位显示。

## 使用方法

### Steam 平台

1. 打开 Steam 创意工坊页面
2. 点击"订阅"按钮
3. 启动游戏
4. 在主菜单进入 Mods 界面
5. 开启本mod

## 工作原理

通过Harmony框架hook了游戏itemdisplay的setup函数

删除原有Item上的插槽，再统一添加6个插槽。

## 开发信息

### 技术栈

- C# (.NET Standard 2.1)
- Harmony (Mod 框架)
- Unity (游戏引擎)

### 编译方法

1. 打开 Visual Studio
2. 打开 `BetterAccessoryDisplay.sln`
3. 在csproj项目属性中设置 `DuckovPath` 为游戏安装目录
4. 选择 Release 配置
5. Build → Build Solution
6. DLL 文件将生成在 `bin/Release/` 目录

## 许可证

MIT License

## 致谢

感谢 Escape From Duckov 的开发者提供的 Mod 系统。

参考了以下项目：
- [Duckov Modding 示例](https://github.com/xvrsl/duckov_modding)
- [KeycardRecordedIndicator](https://github.com/Tonwed/KeycardRecordedIndicator)

**游戏愉快！**


