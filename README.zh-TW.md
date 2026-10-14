<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (FairyGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（FairyGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（FairyGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（FairyGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(FairyGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文件](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 目錄導航

- [項目簡介](#項目簡介)
- [快速開始](#快速開始)
- [使用示例](#使用示例)
- [文件與資源](#文件與資源)
- [社區與支持](#社區與支持)
- [開源協議](#開源協議)

---

## 項目簡介

GameFrameX OperationKeyboard（FairyGUI 綁定）是鍵盤家族的 UI 綁定套件：透過 `IOperationKeyboardField` 實作把 FairyGUI `GTextInput` 綁定到核心 `OperationKeyboard` 門面，並自帶執行時輪詢 Stage 焦點的自動掛載器——零面板改動實現小遊戲鍵盤自動適配。本套件為 FairyGUI 6.0.0 去 SDK 化後的官方鍵盤接入路徑（原內連微信 / 抖音鍵盤橋已移除）。

### 核心特性

- **GTextInput 綁定器** - `GTextInputKeyboardBinder` 訂閱 onFocusIn / onFocusOut 建立與解除鍵盤會話，confirm / complete 經 onSubmit 派發（與原生 Enter 路徑一致）
- **自舉自動掛載器** - `DontDestroyOnLoad` 自舉低頻輪詢 `Stage.inst.focus`，發現聚焦輸入框即綁定（無需逐面板配置）
- **原生事件流** - input 事件賦值 `text`（`onChanged` 照常觸發）並置尾游標；confirm / complete 終值落定並派發 `onSubmit`；失焦解綁且不重複觸發
- **工具集探測** - `versionDefines` 探測 `com.gameframex.unity.fairygui.unity`；探測到即註冊，未安裝時綁定器程式碼被裁剪

### 系統要求

- Unity 2019.4 或更高版本
- `com.gameframex.unity.fairygui.unity` 6.0.0 或更高版本（經 versionDefines 探測，不宣告硬依賴）

---

## 快速開始

### 安裝

選擇以下任一方式：

1. 編輯 Unity 專案的 `Packages/manifest.json`，新增 `scopedRegistries` 部分：
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

   `scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。

2. 直接在 `manifest.json` 的 `dependencies` 節點下新增以下內容：
   ```json
   {
      "com.gameframex.unity.operationkeyboard.fairygui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式新增庫，地址為： `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git`
4. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動載入識別。

## 使用示例

### 自動適配（零面板改動）

本套件與平台渠道包（如 `com.gameframex.unity.operationkeyboard.wechatminigame`）及小遊戲 SDK 一起安裝。WebGL 小遊戲構建下，點按 GTextInput 即拉起平台鍵盤；任何面板都無需改動。

```csharp
// 遊戲程式碼無需任何改動——自動掛載器負責發現與綁定。
// 業務回呼照常工作：
textInput.onChanged.Add(context => { /* 鍵盤輸入時觸發 */ });
textInput.onSubmit.Add(context => { /* confirm / complete 時觸發 */ });
```

### 手動綁定（兜底）

顯式綁定保留可用（遷移路徑：原 FairyGUI 內連橋工程平遷）：

```csharp
// 同一目標冪等：已綁定時返回既有綁定器
var binder = GameFrameX.OperationKeyboard.FairyGUI.Runtime.GTextInputKeyboardBinder.Bind(textInput);
// 面板銷毀時顯式回收（自動掛載器亦會兜底清掃已銷毀目標）
binder.Dispose();
```

### 事件流映射

| 平台鍵盤事件 | 欄位行為 |
|---|---|
| input | `text = value`（`onChanged` 觸發）+ 游標置尾 |
| confirm / complete | 終值落定、鍵盤收起、派發 `onSubmit` |
| 失焦 / 視窗關閉 | 鍵盤收起、會話解綁（不重複觸發） |

### 主要類

- **`GTextInputKeyboardBinder`**：`IOperationKeyboardField` 實作；聚焦 Attach、失焦 Detach、`Bind` 同一目標冪等。
- **`OperationKeyboardFairyGUIAutoBinder`**：自舉自動掛載器；`DontDestroyOnLoad` + 低頻 `Stage.inst.focus` 輪詢 + 已銷毀目標清掃。
- **`GameFrameXOperationKeyboardFairyGUICroppingHelper`**：構建裁剪最佳化用的防裁剪助手。

### 注意事項

- 未註冊渠道包（Editor / 原生平台）時門面 no-op：不攔截任何事件，原生 `TouchScreenKeyboard` 路徑完整保留。
- `maxLength` 由核心恆傳 -1（小遊戲鍵盤按拼音組合串計數，硬上限會掐斷中文輸入）。
- 多欄位並發由核心欄位會話單槽仲裁（新欄位 Attach 自動收起鍵盤並對舊欄位派發 onSubmit）。

---

## 文件與資源

- **完整文件**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **問題回報**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/issues)

---

## 社區與支持

- **QQ 討論群**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **功能建議**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/discussions)

---

## 依賴
| 套件 | 說明 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard 核心套件 |
| com.gameframex.unity | GameFrameX 核心 |
| com.gameframex.unity.fairygui.unity | 經 versionDefines 探測（6.0.0+） |

## 更新日誌

查看 [Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases) 了解更新日誌。

## 開源協議

詳見 [LICENSE.md](LICENSE.md) 文件。
