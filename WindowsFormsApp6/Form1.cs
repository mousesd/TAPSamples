using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region == async/await를 사용한 샘플 ==
        private async void button1_Click(object sender, EventArgs e)
        {
            string result = await this.GetButtonLabelAsync();
            button1.Text = result;
        }

        private async Task<string> GetButtonLabelAsync()
        {
            string label = await this.LongRunningAsync();
            return label;
        }
        #endregion

        #region == 컴파일러가 생성하는 코드 샘플 ==
        private void button2_Click(object sender, EventArgs e)
        {
            //# button1_Click() 이벤트 핸들러에 대응하는 코드
            int state = -1;
            Action moveNext = null;
            TaskAwaiter<string> awaiter;
            var builder = new AsyncVoidMethodBuilder();

            moveNext = delegate
            {
                try
                {
                    if (state != 0)
                    {
                        awaiter = this.GetButtonLabelAsync().GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            state = 0;
                            awaiter.OnCompleted(moveNext);
                            return;
                        }
                    }
                    button2.Text = awaiter.GetResult();
                    builder.SetResult();
                }
                catch (Exception ex)
                {
                    builder.SetException(ex);
                }
            };
            moveNext();
        }

        private Task<string> GetButtonLabel()
        {
            //# GetButtonLabelAsync() 메서드에 대응하는 코드
            int state = -1;
            Action moveNext = null;
            TaskAwaiter<string> awaiter;
            var builder = new AsyncTaskMethodBuilder<string>();

            moveNext = delegate
            {
                try
                {
                    if (state != 0)
                    {
                        awaiter = this.LongRunningAsync().GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            state = 0;
                            awaiter.OnCompleted(moveNext);
                            return;
                        }
                    }
                    builder.SetResult(awaiter.GetResult());
                }
                catch (Exception ex)
                {
                    builder.SetException(ex);
                }
            };

            moveNext();
            return builder.Task;
        }
        #endregion

        #region == ConfigureAwait() 메서드 사용 샘플 ==
        private async void button3_Click(object sender, EventArgs e)
        {
            //# ConfigureAwait(true)를 사용하는 경우의 예제
            button3.Text = await this.LongRunningAsync() + await this.ShortRunningAsync();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            //# ConfigureAwait(false)를 사용하는 경우의 예제
            //# - LongRunningAsync(), ShortRunningAsync() 메서드가 서로다른 스레드에서 실행 됨
            //# - 위 두 메서드가 동시에 실행되는건 아니고 첫번째 메서드 실행이 완료된후 두번째 메서드가 실행 됨
            Func<Task<string>> RunningAsync = 
                async () => await this.LongRunningAsync().ConfigureAwait(false) + await this.ShortRunningAsync().ConfigureAwait(false);
            button4.Text = await RunningAsync();
        } 
        #endregion

        private Task<string> LongRunningAsync()
        {
            Debug.WriteLine($"ThreadId #{Thread.CurrentThread.ManagedThreadId}, LongRunning: {DateTime.Now}");
            return Task.Run(async () =>
            {
                await Task.Delay(3000);
                return "Asynchronous";
            });
        }

        private Task<string> ShortRunningAsync()
        {
            Debug.WriteLine($"ThreadId #{Thread.CurrentThread.ManagedThreadId}, ShortRunning: {DateTime.Now}");
            return Task.Run(async () =>
            {
                await Task.Delay(1000);
                return " Programming";
            });
        }
    }
}
