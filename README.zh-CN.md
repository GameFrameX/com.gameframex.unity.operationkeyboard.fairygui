<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (FairyGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（FairyGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（FairyGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（FairyGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(FairyGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 目录导航

- [项目简介](#项目简介)
- [快速开始](#快速开始)
- [使用示例](#使用示例)
- [文档与资源](#文档与资源)
- [社区与支持](#社区与支持)
- [开源协议](#开源协议)

---

## 项目简介

GameFrameX OperationKeyboard（FairyGUI 绑定）是键盘家族的 UI 绑定包：通过 `IOperationKeyboardField` 实现把 FairyGUI `GTextInput` 绑定到核心 `OperationKeyboard` 门面，并自带运行时轮询 Stage 焦点的自动挂载器——零面板改动实现小游戏键盘自动适配。本包为 FairyGUI 6.0.0 去 SDK 化后的官方键盘接入路径（原内联微信 / 抖音键盘桥已移除）。

### 核心特性

- **GTextInput 绑定器** - `GTextInputKeyboardBinder` 订阅 onFocusIn / onFocusOut 建立与解除键盘会话，confirm / complete 经 onSubmit 派发（与原生 Enter 路径一致）
- **自举自动挂载器** - `DontDestroyOnLoad` 自举低频轮询 `Stage.inst.focus`，发现聚焦输入框即绑定（无需逐面板配置）
- **原生事件流** - input 事件赋值 `text`（`onChanged` 照常触发）并置尾光标；confirm / complete 终值落定并派发 `onSubmit`；失焦解绑且不重复触发
- **工具集探测** - `versionDefines` 探测 `com.gameframex.unity.fairygui.unity`；探测到即注册，未安装时绑定器代码被裁剪

### 系统要求

- Unity 2019.4 或更高版本
- `com.gameframex.unity.fairygui.unity` 6.0.0 或更高版本（经 versionDefines 探测，不声明硬依赖）

---

## 快速开始

### 安装

选择以下任一方式：

1. 编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
   ```json
   {
     "scopedRegistries": [
       {
         "name": "GameFrameX",
         "url": "https://gameframex.upm.alianblank.uk",
         "scopes": [
           "com.gameframex"
         ]
       }
     ],
     "dependencies": {
       "com.gameframex.unity.operationkeyboard.fairygui": "1.0.0"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

2. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {
      "com.gameframex.unity.operationkeyboard.fairygui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为： `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

## 使用示例

### 自动适配（零面板改动）

本包与平台渠道包（如 `com.gameframex.unity.operationkeyboard.wechatminigame`）及小游戏 SDK 一起安装。WebGL 小游戏构建下，点按 GTextInput 即拉起平台键盘；任何面板都无需改动。

```csharp
// 游戏代码无需任何改动——自动挂载器负责发现与绑定。
// 业务回调照常工作：
textInput.onChanged.Add(context => { /* 键盘输入时触发 */ });
textInput.onSubmit.Add(context => { /* confirm / complete 时触发 */ });
```

### 手动绑定（兜底）

显式绑定保留可用（迁移路径：原 FairyGUI 内联桥工程平迁）：

```csharp
// 同一目标幂等：已绑定时返回既有绑定器
var binder = GameFrameX.OperationKeyboard.FairyGUI.Runtime.GTextInputKeyboardBinder.Bind(textInput);
// 面板销毁时显式回收（自动挂载器亦会兜底清扫已销毁目标）
binder.Dispose();
```

### 事件流映射

| 平台键盘事件 | 字段行为 |
|---|---|
| input | `text = value`（`onChanged` 触发）+ 光标置尾 |
| confirm / complete | 终值落定、键盘收起、派发 `onSubmit` |
| 失焦 / 窗口关闭 | 键盘收起、会话解绑（不重复触发） |

### 主要类

- **`GTextInputKeyboardBinder`**：`IOperationKeyboardField` 实现；聚焦 Attach、失焦 Detach、`Bind` 同目标幂等。
- **`OperationKeyboardFairyGUIAutoBinder`**：自举自动挂载器；`DontDestroyOnLoad` + 低频 `Stage.inst.focus` 轮询 + 已销毁目标清扫。
- **`GameFrameXOperationKeyboardFairyGUICroppingHelper`**：构建裁剪优化用的防裁剪助手。

### 注意事项

- 未注册渠道包（Editor / 原生平台）时门面 no-op：不拦截任何事件，原生 `TouchScreenKeyboard` 路径完整保留。
- `maxLength` 由核心恒传 -1（小游戏键盘按拼音组合串计数，硬上限会掐断中文输入）。
- 多字段并发由核心字段会话单槽仲裁（新字段 Attach 自动收起键盘并对旧字段派发 onSubmit）。

---

## 文档与资源

- **完整文档**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **问题反馈**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/issues)

---

## 社区与支持

- **QQ 讨论群**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **功能建议**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/discussions)

---

## 依赖
| 包 | 说明 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard 核心包 |
| com.gameframex.unity | GameFrameX 核心 |
| com.gameframex.unity.fairygui.unity | 经 versionDefines 探测（6.0.0+） |

## 更新日志

查看 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases) 了解更新日志。

## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。
