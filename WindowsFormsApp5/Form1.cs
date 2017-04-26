using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region == 1.Avoid async void 예제 ==
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //# 1.Avoid async void
                //# async void 메서드를 호출시 catch 블럭에서 예외가 잡히지 않음
                this.ThrowExceptionAsync();

                //# async Task 메서드를 호출시 catch 블러에서 예외가 잡힘
                //# - 시그니쳐를 async로 변경 필요
                //await this.ThrowExceptionExAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Message: {ex.Message}");
            }
        }

        private async void ThrowExceptionAsync()
        {
            await Task.Delay(1000);
            throw new InvalidOperationException();
        }

        private async Task ThrowExceptionExAsync()
        {
            await Task.Delay(1000);
            throw new InvalidOperationException();
        }

        #endregion

        #region == 2.Async All the Way 예제 ==
        private void button2_Click(object sender, EventArgs e)
        {
            //# 1.비동기 메서드를 Block(Wait)해서 발생하는 Deadlock 예제
            //# When the await completes, it attempts to execute the remainder of the async method within the captured context.
            //# But that context already has a thread in it, which is (synchronously) waiting for the async method to complete.
            //# They’re each waiting for the other, causing a deadlock.

            //# UI가 없는 ConsoleApp에서는 위 코드에서 Deaklock이 발생하지 않음
            //# ConsoleApp1 프로젝트 참조
            var deadlock = new DeadlockSample();
            deadlock.RunDeadlock();

            //# 2.해결책
            //# - 시그니쳐를 async로 변경 필요
            //var deadlock = new DeadlockSample();
            //await deadlock.RunDeadlockAsync();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //# 1.Unexpected blocking of context threads의 예제
            //# This method isn’t fully asynchronous. 
            //# It will immediately yield, returning an incomplete task, but when it resumes it will synchronously block whatever thread is running.
            //# If this method is called from a GUI context, it will block the GUI thread
            await Task.Yield();
            Thread.Sleep(5000);

            //# 해결책
            //await Task.Yield();
            //await Task.Delay(5000);
        }
        #endregion

        #region == 3.Configure Context 예제 ==
        private async void button4_Click(object sender, EventArgs e)
        {
            //# 1.ConfigureAwait() 예제
            //# Code here runs in the original context.
            Debug.WriteLine($"#1 ThreadId={Thread.CurrentThread.ManagedThreadId}, Context={SynchronizationContext.Current}");
            await Task.Delay(1000);

            //# Code here runs in the original context.
            Debug.WriteLine($"#2 ThreadId={Thread.CurrentThread.ManagedThreadId}, Context={SynchronizationContext.Current}");
            await Task.Delay(1000).ConfigureAwait(false);

            //# Code here runs without the original context.(in this case, on the ThreadPool)
            Debug.WriteLine($"#3 ThreadId={Thread.CurrentThread.ManagedThreadId}, Context={SynchronizationContext.Current}");
            Debug.WriteLine(string.Empty);
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            //# 2.Recall that the context is captured only if an incomplete Task is awaited;
            //# if the Task is already complete, then the context isn’t captured

            //# Code here runs in the original context.
            Debug.WriteLine($"#1 ThreadId={Thread.CurrentThread.ManagedThreadId}, Context={SynchronizationContext.Current}");
            await Task.FromResult(1);

            //# Code here runs in the original context.
            Debug.WriteLine($"#2 ThreadId={Thread.CurrentThread.ManagedThreadId}, Context={SynchronizationContext.Current}");
            await Task.FromResult(1).ConfigureAwait(false);

            //# Code here runs in the original context.
            Debug.WriteLine($"#3 ThreadId={Thread.CurrentThread.ManagedThreadId}, Context={SynchronizationContext.Current}");
            var random = new Random();
            await Task.Delay(random.Next(2)).ConfigureAwait(false);

            //# Code here might or might not run in the original context.
            Debug.WriteLine($"#4 ThreadId={Thread.CurrentThread.ManagedThreadId}, Context={SynchronizationContext.Current}");
            Debug.WriteLine(string.Empty);
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            //# 3.Each async method has its own context,
            //# so if one async method calls another async method, their contexts are independent
            button6.Enabled = false;
            await this.HandleClickAsync();
            button6.Enabled = true;
        }

        private async Task HandleClickAsync()
        {
            await Task.Delay(2000).ConfigureAwait(false);
        }
        #endregion
    }
}
