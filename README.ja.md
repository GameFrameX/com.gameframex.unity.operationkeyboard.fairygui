<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (FairyGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（FairyGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（FairyGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（FairyGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(FairyGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディーゲーム開発のオールインワンソリューション · インディー開発者の夢を応援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## 目次

- [プロジェクト概要](#プロジェクト概要)
- [クイックスタート](#クイックスタート)
- [使用例](#使用例)
- [ドキュメントとリソース](#ドキュメントとリソース)
- [コミュニティとサポート](#コミュニティとサポート)
- [ライセンス](#ライセンス)

---

## プロジェクト概要

GameFrameX OperationKeyboard（FairyGUI バインディング）はキーボードファミリーの UI バインディングパッケージです：`IOperationKeyboardField` 実装を通じて FairyGUI の `GTextInput` をコアの `OperationKeyboard` ファサードにバインドし、Stage フォーカスをランタイムでポーリングする自動バインダーも同梱——パネル変更ゼロでミニゲームキーボードの自動適応を実現します。本パッケージは FairyGUI 6.0.0 の SDK 依存除去後の公式キーボード経路です（旧インライン WeChat / Douyin ブリッジは削除済み）。

### 主要機能

- **GTextInput バインダー** - `GTextInputKeyboardBinder` は onFocusIn / onFocusOut を購読してキーボードセッションを確立・解除；confirm / complete は onSubmit をディスパッチ（ネイティブの Enter 経路と同一）
- **自舉自動バインダー** - `DontDestroyOnLoad` で `Stage.inst.focus` を低頻度ポーリングし、フォーカスされた入力欄を発見次第バインド（パネルごとの設定不要）
- **ネイティブイベントフロー** - input イベントは `text` を代入（`onChanged` は通常どおり発火）しキャレットを末尾へ；confirm / complete は最終値を確定して `onSubmit` をディスパッチ；ブラー時の解除は重複発火なし
- **ツールキット探測** - `versionDefines` で `com.gameframex.unity.fairygui.unity` を探測；検出時のみ登録、未インストール時はバインダーコードがストリップされます

### 動作要件

- Unity 2019.4 以上
- `com.gameframex.unity.fairygui.unity` 6.0.0 以上（versionDefines で探測、ハード依存は宣言しません）

---

## クイックスタート

### インストール

以下のいずれかの方法を選択：

1. Unity プロジェクトの `Packages/manifest.json` を編集し、`scopedRegistries` セクションを追加：
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

   `scopes` はこのレジストリで解決されるパッケージを制御します。`com.gameframex` で始まるパッケージのみこのレジストリから取得されます。

2. `manifest.json` の `dependencies` ノードに直接追加：
   ```json
   {
      "com.gameframex.unity.operationkeyboard.fairygui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git"
   }
   ```
3. Unity の `Package Manager` で **Git URL** を使用：`https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git`
4. リポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンすると自動的に読み込まれます。

## 使用例

### 自動適応（パネル変更ゼロ）

本パッケージをプラットフォームチャネルパッケージ（例：`com.gameframex.unity.operationkeyboard.wechatminigame`）およびミニゲーム SDK と一緒にインストールします。WebGL ミニゲームビルドでは GTextInput をタップするとプラットフォームキーボードが立ち上がります。パネルへの変更は一切不要です。

```csharp
// ゲームコード側の変更は不要——自動バインダーが検出とバインドを担当。
// 業務コールバックはこれまでどおり動作します：
textInput.onChanged.Add(context => { /* キーボード入力時に発火 */ });
textInput.onSubmit.Add(context => { /* confirm / complete 時に発火 */ });
```

### 手動バインド（フォールバック）

明示的なバインドも利用可能（旧 FairyGUI インラインブリッジからの移行パス）：

```csharp
// 同一ターゲットに対して冪等：バインド済みなら既存のバインダーを返します
var binder = GameFrameX.OperationKeyboard.FairyGUI.Runtime.GTextInputKeyboardBinder.Bind(textInput);
// パネル破棄時に明示的に解放（自動バインダーも破棄済みターゲットをスイープします）
binder.Dispose();
```

### イベントフローのマッピング

| プラットフォームキーボードイベント | フィールドの動作 |
|---|---|
| input | `text = value`（`onChanged` 発火）+ キャレットを末尾へ |
| confirm / complete | 最終値の確定、キーボードを閉じる、`onSubmit` をディスパッチ |
| ブラ― / ウィンドウクローズ | キーボードを閉じる、セッション解除（重複発火なし） |

### 主要クラス

- **`GTextInputKeyboardBinder`**：`IOperationKeyboardField` 実装；フォーカスで Attach、ブラーで Detach、`Bind` はターゲットごとに冪等。
- **`OperationKeyboardFairyGUIAutoBinder`**：自舉自動バインダー；`DontDestroyOnLoad` + 低頻度 `Stage.inst.focus` ポーリング + 破棄済みターゲットのスイープ。
- **`GameFrameXOperationKeyboardFairyGUICroppingHelper`**：ビルド最適化用のコードクロッピングヘルパー。

### 注意事項

- チャネルパッケージ未登録（Editor / ネイティブプラットフォーム）ではファサードは no-op：イベントを一切拦截せず、ネイティブ `TouchScreenKeyboard` の経路は完全に保持されます。
- `maxLength` はコアから常に -1 を渡します（ミニゲームキーボードはピンイン組成文字列でカウント、ハード上限は中国語入力を断ち切ります）。
- 複数フィールドの並行はコアのフィールドセッション単一スロット調停により処理されます（新フィールドの Attach でキーボードを閉じ、旧フィールドに onSubmit をディスパッチ）。

---

## ドキュメントとリソース

- **完全なドキュメント**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **不具合報告**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/issues)

---

## コミュニティとサポート

- **QQ グループ**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **機能リクエスト**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/discussions)

---

## 依存関係
| パッケージ | 説明 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard コアパッケージ |
| com.gameframex.unity | GameFrameX コア |
| com.gameframex.unity.fairygui.unity | versionDefines で探測（6.0.0+） |

## 変更履歴

[Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases) を参照してください。

## ライセンス

詳細は [LICENSE.md](LICENSE.md) を参照してください。
