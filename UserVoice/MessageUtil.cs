using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UserVoice
{
    /// <summary>
    /// エラーメッセージなどを表示します。
    /// </summary>
    public static class MessageUtil
    {
        public static void ErrorMessage(string message)
        {
            MessageBox.Show(
                message,
                "エラー発生！！！",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static void Message(string caption, string message)
        {
            MessageBox.Show(
                message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
