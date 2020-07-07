using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFApp_Test.ViewModels
{
    internal class DelegateCommand : ICommand
    {
        /// <summary>
        /// コマンド実行時の処理内容を保持します。
        /// </summary>
        private Action<object> _execute;

        /// <summary>
        /// コマンド実行可能判別の処理内容を保持します。
        /// </summary>
        private Func<object, bool> _canExecute;

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンド実行処理を指定します。</param>
        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンド実行処理を指定します。</param>
        /// <param name="canExecute">コマンド実行可能判別処理を指定します。</param>
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        #region ICommand の実装
        /// <summary>
        /// コマンドが実行可能かどうかの判別処理をおこないます。
        /// </summary>
        /// <param name="parameter">判別処理に対するパラメータを指定します。</param>
        /// <returns>実行可能な場合に true を返します。</returns>
        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        /// <summary>
        /// 実行可能かどうかの判別処理に関する状態が変更されたときに発生します。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// CanExecuteChanged イベントを発行します。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var h = this.CanExecuteChanged;
            if (h != null) h(this, EventArgs.Empty);
        }
        /// <summary>
        /// コマンドが実行されたときの処理をおこないます。
        /// </summary>
        /// <param name="parameter">コマンドに対するパラメータを指定します。</param>
        public void Execute(object parameter)
        {
            if (this._execute != null) this._execute(parameter);
        }
        #endregion ICommand の実装 }
    }

    internal abstract class NotificationObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged の実装
        /// <summary>
        /// プロパティに変更があった場合に発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChanged イベントを発行します。
        /// </summary>
        /// <param name="propertyName">プロパティ値に変更があったプロパティ名を指定します。</param>
        protected void RaiseProeprtyChanged([CallerMemberName] string propertyName = null)
        {
            var h = this.PropertyChanged;
            if (h != null) h(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティ値を変更するヘルパです。
        /// </summary>
        /// <typeparam name="T">プロパティの型を表します。</typeparam>
        /// <param name="target">変更するプロパティの実体を ref 指定します。</param>
        /// <param name="value">変更後の値を指定します。</param>
        /// <param name="propertyName">プロパティ名を指定します。
        /// </param>
        /// <returns>プロパティ値に変更があった場合に true を返します。</returns>
        protected bool SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(target, value))
                return false;
            target = value;
            RaiseProeprtyChanged(propertyName);
            return true;
        }
        #endregion INotifyPropertyChanged の実装
    }

    internal class MainViewModel : NotificationObject
    {
        private string _upperString;
        /// <summary>
        /// すべて大文字に変換した文字列を取得します。
        /// </summary> 
        public string UpperString
        {
            get { return this._upperString; }
            private set { SetProperty(ref this._upperString, value); }
        }
        private string _inputString;
        /// <summary>
        /// 入力文字列を取得または設定します。
        /// </summary>
        public string InputString
        {
            get { return this._inputString; }
            set
            {
                if (SetProperty(ref this._inputString, value))
                {
                    this._inputString = value;
                    // 入力された文字列を大文字に変換します
                    this.UpperString = this._inputString.ToUpper();
                    // コマンドの実行可能判別結果に影響を与えているので変更通知をおこないます
                    this.ClearCommand.RaiseCanExecuteChanged();

                    // 出力ウィンドウに結果を表示します
                    System.Diagnostics.Debug.WriteLine("UpperString=" + this.UpperString);
                }
            }
        }

        private DelegateCommand _clearCommand;
        /// <summary>
        /// クリアコマンドを取得します。
        /// </summary>
        public DelegateCommand ClearCommand
        {
            get
            {
                if (this._clearCommand == null)
                {
                    this._clearCommand = new DelegateCommand(_ => { this.InputString = ""; },
                                                             _ => !string.IsNullOrEmpty(this.InputString));
                }
                return this._clearCommand;
            }
        }
    }
}
