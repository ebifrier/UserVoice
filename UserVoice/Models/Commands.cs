using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace UserVoice.Models
{
    using UserVoice.Voice;

    /// <summary>
    /// コマンドの基本クラスです。
    /// </summary>
    public class CommandBase : ICommand
    {
        private List<WeakReference> canExecuteChanged = new List<WeakReference>();

        /// <summary>
        /// コマンドの実行可能/不可能状態が変わった
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (value == null)
                {
                    return;
                }

                this.canExecuteChanged.Add(new WeakReference(value));
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (value == null)
                {
                    return;
                }

                this.canExecuteChanged.Remove(new WeakReference(value));
                CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            for (var i = 0; i < this.canExecuteChanged.Count(); )
            {
                var weak = this.canExecuteChanged[i];
                var target = weak.Target as EventHandler;

                if (target != null)
                {
                    target(this, new EventArgs());
                    ++i;
                }
                else
                {
                    // iはインクリメントしません。
                    this.canExecuteChanged.RemoveAt(i);
                }
            }
        }

        protected virtual bool CanExecuteImpl(object parameter)
        {
            return true;
        }

        protected virtual void ExecuteImpl(object parameter)
        {
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteImpl(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteImpl(parameter);
        }
    }

    /// <summary>
    /// 選択されたユーザー声を削除します。
    /// </summary>
    public class RemoveUserVoiceCommand : CommandBase
    {
        protected override bool CanExecuteImpl(object parameter)
        {
            return (Global.ModelObject.CurrentUserVoiceInfoList.Count != 0);
        }

        protected override void ExecuteImpl(object parameter)
        {
            var currentVoiceInfoList = Global.ModelObject.CurrentUserVoiceInfoList;
            if (currentVoiceInfoList.Count == 0)
            {
                return;
            }

            // ユーザーの登録された声文字列を削除します。
            var voiceInfoList = Global.ModelObject.UserVoiceInfoList;

            // 操作の途中でCurrentUserVoiceInfoListが変わることがあるため、
            // 単純にforeeachすることはできません。
            for (var i = 0; i < currentVoiceInfoList.Count; )
            {
                var item = currentVoiceInfoList[i];

                if (voiceInfoList.Remove(item))
                {
                    // 削除された場合はiをインクリメントしません。
                }
                else
                {
                    ++i;
                }
            }

            // 設定された声を保存します。
            AppSettings.Save();
        }
    }

    /// <summary>
    /// 184ユーザーによる声を削除します。
    /// </summary>
    public class RemoveAnonymousUserVoiceCommand : CommandBase
    {
        protected override void ExecuteImpl(object parameter)
        {
            var voiceInfoList = Global.ModelObject.UserVoiceInfoList;

            // 操作を行う前に確認ダイアログを出します。
            var result = MessageBox.Show(
                "本当に削除してもよろしいですか？",
                "184ユーザーの削除確認",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.OK)
            {
                return;
            }

            // 184ユーザーによって登録された声文字列をすべて削除します。
            UserVoiceInfo info;
            while ((info = voiceInfoList.FirstOrDefault(
                info_ => info_.User.IsAnonymous)) != null)
            {
                voiceInfoList.Remove(info);
            }

            // 設定された声を保存します。
            AppSettings.Save();
        }
    }

    /// <summary>
    /// ウィンドウを閉じます。
    /// </summary>
    public class CloseCommand : CommandBase
    {
        protected override void ExecuteImpl(object parameter)
        {
            var window = Global.MainForm;
            if (window == null)
            {
                return;
            }

            window.Close();
        }
    }

    /// <summary>
    /// コマンド群を取得します。
    /// </summary>
    public static class Commands
    {
        /*public static readonly ICommand AddVoiceNameCommand =
            new AddVoiceNameCommand();

        public static readonly ICommand ReplaceVoiceNameCommand =
            new ReplaceVoiceNameCommand();

        public static readonly ICommand RemoveVoiceNameCommand =
            new RemoveVoiceNameCommand();*/

        public static readonly ICommand RemoveUserVoice =
            new RemoveUserVoiceCommand();

        public static readonly ICommand RemoveAnonymousUserVoice =
            new RemoveAnonymousUserVoiceCommand();

        public static readonly ICommand Close =
            new CloseCommand();
    }
}
