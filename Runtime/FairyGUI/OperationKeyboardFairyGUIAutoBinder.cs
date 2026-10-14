#if ENABLE_FAIRYGUI_OPERATIONKEYBOARD

// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相关法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root of the source tree for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any disputes or liabilities arising from secondary development based on this project
//   本项目组织及贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository:  https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:   https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:     https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

namespace GameFrameX.OperationKeyboard.FairyGUI.Runtime
{
    using global::FairyGUI; // 命名空间段 FairyGUI 遮蔽全局命名空间，必须 global:: 限定（C30 门面遮蔽教训的命名空间变体）
    using UnityEngine;

    /// <summary>
    /// FairyGUI 键盘自动挂载器。
    /// </summary>
    /// <remarks>
    /// BeforeSceneLoad 阶段自举创建常驻 GameObject，以 0.1 秒低频轮询 <see cref="Stage.inst"/>.focus：
    /// 焦点为 InputTextField 且 gOwner 为 GTextInput 且未绑定时创建绑定器并补发 Attach
    /// （Stage 焦点在绑定器创建前已落定，onFocusIn 已错过），零面板改动自动适配；
    /// 同轮清扫已销毁目标的绑定器（聚焦中被销毁的字段兜底解绑，防键盘会话悬挂）。
    /// 手动模式（<see cref="GTextInputKeyboardBinder.Bind"/>）始终可用，挂载器经共用注册表对已绑定字段不重复干预。
    /// Bootstrap polls the FairyGUI stage focus and auto-attaches the matching binder so existing panels need
    /// zero changes; manual binding stays available and shares the same registry.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    internal sealed class OperationKeyboardFairyGUIAutoBinder : MonoBehaviour
    {
        /// <summary>
        /// 轮询间隔（秒，按 unscaled 时间累计）。
        /// </summary>
        private const float PollIntervalSeconds = 0.1f;

        private float _nextPollTime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [UnityEngine.Scripting.Preserve]
        private static void Bootstrap()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var host = new GameObject("OperationKeyboardFairyGUIAutoBinder");
            DontDestroyOnLoad(host);
            host.AddComponent<OperationKeyboardFairyGUIAutoBinder>();
        }

        [UnityEngine.Scripting.Preserve]
        private void Update()
        {
            if (Time.unscaledTime < _nextPollTime)
            {
                return;
            }

            _nextPollTime = Time.unscaledTime + PollIntervalSeconds;

            // 清扫已销毁目标的绑定器（含聚焦中销毁字段的会话兜底解绑）。
            GTextInputKeyboardBinder.SweepDisposed();

            var focus = Stage.inst.focus;
            if (focus is InputTextField inputTextField && inputTextField.gOwner is GTextInput gTextInput && !gTextInput.isDisposed)
            {
                if (GTextInputKeyboardBinder.Find(gTextInput) == null)
                {
                    GTextInputKeyboardBinder.Bind(gTextInput).HandleFocused();
                }
            }
        }
    }
}

#endif
