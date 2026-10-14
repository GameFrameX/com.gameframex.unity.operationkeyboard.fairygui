<div align="center">
<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# {'en': 'GameFrameX OperationKeyboard (FairyGUI Binding)', 'zh-CN': 'GameFrameX OperationKeyboard 键盘组件（FairyGUI 绑定）', 'zh-TW': 'GameFrameX OperationKeyboard 鍵盤組件（FairyGUI 綁定）', 'ja': 'GameFrameX OperationKeyboard（FairyGUI バインディング）', 'ko': 'GameFrameX OperationKeyboard(FairyGUI 바인딩)'}

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발의 올인원 솔루션 · 인디 개발자의 꿈을 응원

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ 그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 목차

- [프로젝트 소개](#프로젝트-소개)
- [빠른 시작](#빠른-시작)
- [사용 예시](#사용-예시)
- [문서 및 리소스](#문서-및-리소스)
- [커뮤니티 및 지원](#커뮤니티-및-지원)
- [라이선스](#라이선스)

---

## 프로젝트 소개

GameFrameX OperationKeyboard(FairyGUI 바인딩)는 키보드 패밀리의 UI 바인딩 패키지입니다: `IOperationKeyboardField` 구현을 통해 FairyGUI `GTextInput`을 코어 `OperationKeyboard` 퍼사드에 바인딩하고, Stage 포커스를 런타임에 폴링하는 자동 바인더를 동봉——패널 변경 없이 미니 게임 키보드 자동 적응을 구현합니다. 이 패키지는 FairyGUI 6.0.0의 SDK 의존 제거 이후 공식 키보드 경로입니다(기존 인라인 WeChat / Douyin 브리지는 제거됨).

### 주요 기능

- **GTextInput 바인더** - `GTextInputKeyboardBinder`가 onFocusIn / onFocusOut을 구독하여 키보드 세션을 수립·해제; confirm / complete는 onSubmit을 디스패치(네이티브 Enter 경로와 동일)
- **자가 부팅 자동 바인더** - `DontDestroyOnLoad`로 `Stage.inst.focus`를 저빈도 폴링하여 포커스된 입력창을 발견하면 즉시 바인딩(패널별 설정 불필요)
- **네이티브 이벤트 흐름** - input 이벤트는 `text`에 대입(`onChanged`는 평소대로 발화)하고 캐럿을 끝으로 이동; confirm / complete는 최종 값을 확정하고 `onSubmit`을 디스패치; 블러 시 해제되며 중복 발화 없음
- **툴킷 탐지** - `versionDefines`로 `com.gameframex.unity.fairygui.unity`를 탐지; 감지 시 등록, 미설치 시 바인더 코드는 스트립됩니다

### 시스템 요구 사항

- Unity 2019.4 이상
- `com.gameframex.unity.fairygui.unity` 6.0.0 이상(versionDefines로 탐지, 하드 의존성은 선언하지 않음)

---

## 빠른 시작

### 설치

다음 방법 중 하나를 선택:

1. Unity 프로젝트의 `Packages/manifest.json`을 편집하여 `scopedRegistries` 섹션 추가:
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

   `scopes`는 이 레지스트리로 해석되는 패키지를 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies` 노드에 직접 추가:
   ```json
   {
      "com.gameframex.unity.operationkeyboard.fairygui": "https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git"
   }
   ```
3. Unity의 `Package Manager`에서 **Git URL** 사용: `https://github.com/gameframex/com.gameframex.unity.operationkeyboard.fairygui.git`
4. 저장소를 Unity 프로젝트의 `Packages` 디렉토리에 클론하면 자동으로 로드됩니다.

## 사용 예시

### 자동 적응 (패널 변경 없음)

이 패키지를 플랫폼 채널 패키지(예: `com.gameframex.unity.operationkeyboard.wechatminigame`) 및 미니 게임 SDK와 함께 설치합니다. WebGL 미니 게임 빌드에서 GTextInput을 탭하면 플랫폼 키보드가 열립니다. 패널에는 어떤 변경도 필요 없습니다.

```csharp
// 게임 코드의 변경은 전혀 불필요——자동 바인더가 탐지와 바인딩을 담당.
// 비즈니스 콜백은 기존처럼 동작:
textInput.onChanged.Add(context => { /* 키보드 입력 시 발화 */ });
textInput.onSubmit.Add(context => { /* confirm / complete 시 발화 */ });
```

### 수동 바인딩 (폴백)

명시적 바인딩도 사용 가능(기존 FairyGUI 인라인 브리지의 마이그레이션 경로):

```csharp
// 동일 대상에 대해 멱등: 이미 바인딩된 경우 기존 바인더를 반환
var binder = GameFrameX.OperationKeyboard.FairyGUI.Runtime.GTextInputKeyboardBinder.Bind(textInput);
// 패널 파괴 시 명시적으로 해제(자동 바인더도 파괴된 대상을 스윕)
binder.Dispose();
```

### 이벤트 흐름 매핑

| 플랫폼 키보드 이벤트 | 필드 동작 |
|---|---|
| input | `text = value`(`onChanged` 발화) + 캐럿을 끝으로 |
| confirm / complete | 최종 값 확정, 키보드 닫기, `onSubmit` 디스패치 |
| 블러 / 창 닫기 | 키보드 닫기, 세션 해제(중복 발화 없음) |

### 주요 클래스

- **`GTextInputKeyboardBinder`**: `IOperationKeyboardField` 구현; 포커스 시 Attach, 블러 시 Detach, `Bind`는 대상별 멱등.
- **`OperationKeyboardFairyGUIAutoBinder`**: 자가 부팅 자동 바인더; `DontDestroyOnLoad` + 저빈도 `Stage.inst.focus` 폴링 + 파괴된 대상 스윕.
- **`GameFrameXOperationKeyboardFairyGUICroppingHelper`**: 빌드 최적화용 코드 크로핑 헬퍼.

### 주의 사항

- 채널 패키지 미등록(Editor / 네이티브 플랫폼) 시 퍼사드는 no-op: 이벤트를 가로채지 않으며 네이티브 `TouchScreenKeyboard` 경로가 온전히 유지됩니다.
- `maxLength`는 코어가 항상 -1을 전달합니다(미니 게임 키보드는 병음 조합 문자열로 카운트, 하드 상한은 중국어 입력을 끊습니다).
- 다중 필드 동시 사용은 코어의 필드 세션 단일 슬롯 중재로 처리됩니다(새 필드 Attach 시 키보드를 닫고 이전 필드에 onSubmit을 디스패치).

---

## 문서 및 리소스

- **전체 문서**: [https://gameframex.doc.alianblank.com](https://gameframex.doc.alianblank.com)
- **버그 신고**: [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/issues)

---

## 커뮤니티 및 지원

- **QQ 그룹**: [467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)
- **기능 제안**: [GitHub Discussions](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/discussions)

---

## 의존성
| 패키지 | 설명 |
|----|------|
| com.gameframex.unity.operationkeyboard | GameFrameX OperationKeyboard 코어 패키지 |
| com.gameframex.unity | GameFrameX 코어 |
| com.gameframex.unity.fairygui.unity | versionDefines로 탐지 (6.0.0+) |

## 업데이트 로그

[Releases](https://github.com/GameFrameX/com.gameframex.unity.operationkeyboard.fairygui/releases)를 참조하세요.

## 라이선스

자세한 내용은 [LICENSE.md](LICENSE.md)를 참조하세요.
