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
    using System;
    using System.Collections.Generic;
    using global::FairyGUI; // 命名空间段 FairyGUI 遮蔽全局命名空间，必须 global:: 限定（C30 门面遮蔽教训的命名空间变体）
    using GameFrameX.OperationKeyboard.Runtime;
    using KeyboardFacade = GameFrameX.OperationKeyboard.Runtime.OperationKeyboard;

    /// <summary>
    /// FairyGUI GTextInput 键盘绑定器。
    /// </summary>
    /// <remarks>
    /// 聚焦 Attach / 失焦 Detach：订阅 <see cref="GTextInput"/> 的 onFocusIn / onFocusOut 事件把字段接入键盘门面；
    /// 绑定器从不拦截 GTextInput 原生事件——未注册渠道时门面 no-op，原生 TouchScreenKeyboard 路径完整保留；
    /// maxLength 恒 -1 由核心保证（键盘按拼音组合串计数，硬上限会掐断中文输入）。
    /// 通过 <see cref="Bind"/> 创建（同目标幂等，自动挂载器与手动模式共用注册表，不产生双绑定）；
    /// confirm / complete 终值落定后经 <see cref="TriggerEndEdit"/> 派发 onSubmit，
    /// 与原生 Enter 路径一致（InputTextField 键盘提交即 onSubmit）；不主动移动 Stage 焦点（多行 confirm 不等价失焦）。
    /// Focus attaches the GTextInput to the OperationKeyboard facade and blur detaches it; the binder never
    /// suppresses native FairyGUI events, and with no channel registered the facade is a no-op.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class GTextInputKeyboardBinder : IOperationKeyboardField
    {
        private static readonly Dictionary<GTextInput, GTextInputKeyboardBinder> Bindings =
            new Dictionary<GTextInput, GTextInputKeyboardBinder>();

        private readonly GTextInput _textInput;
        private bool _disposed;

        private GTextInputKeyboardBinder(GTextInput textInput)
        {
            _textInput = textInput;
            textInput.onFocusIn.Add(OnFocusIn);
            textInput.onFocusOut.Add(OnFocusOut);
        }

        /// <summary>
        /// 获取绑定目标。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public GTextInput Target
        {
            get { return _textInput; }
        }

        /// <summary>
        /// 绑定目标输入框（同目标幂等：已绑定时返回既有绑定器）。
        /// </summary>
        /// <param name="textInput">要绑定键盘的 GTextInput。</param>
        /// <returns>目标对应的绑定器。</returns>
        [UnityEngine.Scripting.Preserve]
        public static GTextInputKeyboardBinder Bind(GTextInput textInput)
        {
            if (textInput == null)
            {
                throw new ArgumentNullException("textInput");
            }

            GTextInputKeyboardBinder binder;
            if (!Bindings.TryGetValue(textInput, out binder) || binder._disposed)
            {
                binder = new GTextInputKeyboardBinder(textInput);
                Bindings[textInput] = binder;
            }

            return binder;
        }

        /// <summary>
        /// 获取指定目标已存在的绑定器（未绑定时返回 null）。
        /// </summary>
        /// <param name="textInput">查询的目标。</param>
        /// <returns>已存在的绑定器或 null。</returns>
        [UnityEngine.Scripting.Preserve]
        public static GTextInputKeyboardBinder Find(GTextInput textInput)
        {
            if (textInput == null)
            {
                return null;
            }

            GTextInputKeyboardBinder binder;
            return Bindings.TryGetValue(textInput, out binder) && !binder._disposed ? binder : null;
        }

        /// <summary>
        /// 重置绑定注册表（Domain Reload 关闭时避免残留已销毁目标的引用；不触碰键盘会话）。
        /// </summary>
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        [UnityEngine.Scripting.Preserve]
        public static void Reset()
        {
            Bindings.Clear();
        }

        /// <summary>
        /// 清扫并回收已销毁目标的绑定器（字段聚焦持有键盘会话时被销毁的兜底：解绑并收起键盘，防会话悬挂）。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        internal static void SweepDisposed()
        {
            List<GTextInput> disposed = null;
            foreach (KeyValuePair<GTextInput, GTextInputKeyboardBinder> pair in Bindings)
            {
                if (pair.Key.isDisposed)
                {
                    if (disposed == null)
                    {
                        disposed = new List<GTextInput>();
                    }

                    disposed.Add(pair.Key);
                }
            }

            if (disposed == null)
            {
                return;
            }

            foreach (GTextInput key in disposed)
            {
                if (Bindings.TryGetValue(key, out GTextInputKeyboardBinder binder))
                {
                    binder.Dispose();
                }
            }
        }

        /// <summary>
        /// 解绑并回收：退订事件、释放键盘会话（目标销毁或手动模式显式回收时调用；幂等）。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _textInput.onFocusIn.Remove(OnFocusIn);
            _textInput.onFocusOut.Remove(OnFocusOut);
            KeyboardFacade.Detach(this);
            if (Bindings.TryGetValue(_textInput, out GTextInputKeyboardBinder current) && ReferenceEquals(current, this))
            {
                Bindings.Remove(_textInput);
            }
        }

        /// <summary>
        /// 获取或设置字段当前文本。
        /// </summary>
        /// <remarks>
        /// 键盘 input 事件回灌时经此属性赋值（onChanged 照常触发，业务回调零改动）；目标已销毁时为安全 no-op。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public string Text
        {
            get { return !_textInput.isDisposed ? _textInput.text : string.Empty; }
            set
            {
                if (!_textInput.isDisposed)
                {
                    _textInput.text = value ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// 获取字段当前是否聚焦。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public bool IsFocused
        {
            get { return !_textInput.isDisposed && _textInput.focused; }
        }

        /// <summary>
        /// 获取是否多行输入。
        /// </summary>
        /// <remarks>
        /// 与 FairyGUI 单行语义对齐：GTextInput.singleLine 为真即单行。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public bool Multiline
        {
            get { return !_textInput.isDisposed && !_textInput.singleLine; }
        }

        /// <summary>
        /// 获取键盘确认按钮类型。
        /// </summary>
        /// <remarks>
        /// 与原内联桥一致固定 done；GTextInput.keyboardType（TouchScreenKeyboardType）到平台 confirmType 的映射为扩展点。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public string ConfirmType
        {
            get { return "done"; }
        }

        /// <summary>
        /// 将光标移动到文本末尾。
        /// </summary>
        /// <remarks>
        /// 平台键盘 input 事件携带整串值，赋值后光标须置尾（caretPosition 置 Length，即 SetSelection(len, 0)），避免光标停留在起始位置。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void MoveTextEnd()
        {
            if (!_textInput.isDisposed)
            {
                _textInput.caretPosition = _textInput.text.Length;
            }
        }

        /// <summary>
        /// 触发字段的结束编辑事件（等价于原生 Enter 提交后的 onSubmit）。
        /// </summary>
        /// <remarks>
        /// 门面在 confirm / complete 终值落定后调用； FairyGUI 无 onEndEdit，onSubmit 为提交事件挂点
        /// （原生 InputTextField Enter 路径即派发 onSubmit）。不主动移动 Stage 焦点：多行字段 confirm 不等价失焦，
        /// 失焦事件由字段自身 onFocusOut 链路产生。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void TriggerEndEdit()
        {
            if (!_textInput.isDisposed)
            {
                _textInput.onSubmit.Call();
            }
        }

        /// <summary>
        /// 目标已处于聚焦态时补发 Attach（自动挂载器发现即绑定，onFocusIn 已错过时由自动挂载器调用）。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        internal void HandleFocused()
        {
            KeyboardFacade.Attach(this);
        }

        private void OnFocusIn(EventContext context)
        {
            KeyboardFacade.Attach(this);
        }

        private void OnFocusOut(EventContext context)
        {
            KeyboardFacade.Detach(this);
        }
    }
}

#endif
