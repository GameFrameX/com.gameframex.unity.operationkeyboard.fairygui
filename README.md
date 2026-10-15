<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (FairyGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（FairyGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（FairyGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（FairyGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(FairyGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Table of Contents

- [Project Overview](#project-overview)
- [Quick Start](#quick-start)
- [Usage Examples](#usage-examples)
- [Documentation & Resources](#documentation--resources)
- [Community & Support](#community--support)
- [License](#license)

---

## Project Overview

GameFrameX OperationKeyboard (FairyGUI Binding) is the UI binding package of the keyboard family for FairyGUI: it binds `GTextInput` to the core `OperationKeyboard` facade through an `IOperationKeyboardField` implementation, and ships an auto-binder bootstrap that polls the FairyGUI stage focus at runtime - zero panel changes for automatic mini-game keyboard adaptation. This package is the official keyboard path after FairyGUI 6.0.0 de-SDK (the former inline WeChat / Douyin bridges were removed).

### Key Features

- **GTextInput Binder** - `GTextInputKeyboardBinder` subscribes onFocusIn / onFocusOut to establish and release the keyboard session; confirm / complete dispatch onSubmit (same as the native Enter path)
- **Auto-Binder Bootstrap** - a `DontDestroyOnLoad` bootstrap polls `Stage.inst.focus` at a low frequency, discovers focused input fields and binds them automatically (no per-panel setup); once the `[GameFrameX]` runtime host is up, the bootstrap node relocates under `[GameFrameX]/UI/Keyboard`
- **Native Event Flow** - input events assign `text` (`onChanged` fires as usual) and move the caret to the end; confirm / complete finalize the value and dispatch `onSubmit`; blur detaches without duplication
- **Toolkit Probing** - `versionDefines` probe `com.gameframex.unity.fairygui.unity`; the binder compiles only when the toolkit is present (discovery means registration)

### System Requirements

- Unity 2019.4 or higher
- `com.gameframex.unity.fairygui.unity` 6.0.0 or higher (probed via versionDefines, not a hard dependency)

---

## Quick Start

### Installation

Choose one of the following methods:

1. Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:
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

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.operationkeyboard.fairygui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.

## Usage Examples

### Automatic Adaptation (Zero Panel Changes)

Install this package together with a platform channel package (e.g. `com.gameframex.unity.operationkeyboard.wechatminigame`) and the mini-game SDK. On WebGL mini-game builds, tapping a `GTextInput` now opens the platform keyboard; no panel needs any change.

```csharp
// Nothing to do in game code - the auto-binder handles discovery and binding.
// Business callbacks keep working unchanged:
textInput.onChanged.Add(context => { /* fires on keyboard input */ });
textInput.onSubmit.Add(context => { /* fires on confirm / complete */ });
```

### Manual Binding (Fallback)

Explicit binding stays available (migration path from the former FairyGUI inline bridges):

```csharp
// Idempotent per target: returns the existing binder when already bound
var binder = GameFrameX.OperationKeyboard.FairyGUI.Runtime.GTextInputKeyboardBinder.Bind(textInput);
// Release explicitly on panel teardown (the auto-binder also sweeps disposed targets)
binder.Dispose();
```

### Event Flow Mapping

| Platform keyboard event | Field behavior |
|---|---|
| input | `text = value` (`onChanged` fires) + caret to end |
| confirm / complete | final value assigned, keyboard hidden, `onSubmit` dispatched |
| blur / window close | keyboard hidden, session detached (no duplicated event) |

### Main Classes

- **`GTextInputKeyboardBinder`**: `IOperationKeyboardField` implementation; attach on focus, detach on blur, `Bind` is idempotent per target.
- **`OperationKeyboardFairyGUIAutoBinder`**: bootstrap auto-binder; `DontDestroyOnLoad` + low-frequency `Stage.inst.focus` polling + disposed-target sweeping; relocates under the `[GameFrameX]/UI/Keyboard` group node once the runtime host exists.
- **`GameFrameXOperationKeyboardFairyGUICroppingHelper`**: code cropping helper for build optimization.

### Notes

- With no channel package registered (Editor / native platforms) the facade is a no-op: no event is intercepted and the native `TouchScreenKeyboard` path is fully preserved.
- `maxLength` is always passed as -1 by the core (mini-game keyboards count the pinyin composition string; a hard cap breaks Chinese input).
- Concurrent fields are arbitrated by the core single-slot session (attaching a new field hides the keyboard and dispatches onSubmit on the previous field).

---

## Documentation & Resources

- **Documentation**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **Issue Tracker**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/issues)

---

## Community & Support

- **QQ Group**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **Feature Requests**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/discussions)

---

## Dependencies
| Package | Description |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard core |
| com.gameframex.unity | GameFrameX core |
| com.gameframex.unity.fairygui.unity | probed via versionDefines (6.0.0+) |

## Changelog

See [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases) for changelog.

## License

See [LICENSE.md](LICENSE.md) for license information.
