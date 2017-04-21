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

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //# 1.ExecutionContext.Capture(), Run() 샘플
            Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}, SynchronizationContext is {SynchronizationContext.Current}");
            var context = ExecutionContext.Capture();
            ThreadPool.QueueUserWorkItem(delegate
            {
                Debug.WriteLine($"#1. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}, SynchronizationContext is {SynchronizationContext.Current}");
                Thread.Sleep(1500);
                ExecutionContext.Run(context, delegate
                {
                    Debug.WriteLine($"#2. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}, SynchronizationContext is {SynchronizationContext.Current}");

                    //# ExecutionContext.Run()을 실행하면 
                    //# SynchronizationContext.Current 속성을 Capture한 SynchronizationContext로 변경
                    SynchronizationContext.Current.Post(delegate
                    {
                        Debug.WriteLine($"#3. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
                        textBox1.Text = "ExecutionContext";
                    }, null);
                }, null);
            });

            //# 2.아래 코드는 WindowsForms에서만 사용이 가능, UI Framework에 상관없이 실행되어야 한다면 #3의 SynchronizationContext를 사용!
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //ThreadPool.QueueUserWorkItem(delegate
            //{
            //    Debug.WriteLine($"#1. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    Thread.Sleep(1500);
            //    this.Invoke((MethodInvoker)delegate
            //    {
            //        Debug.WriteLine($"#2. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //        textBox1.Text = "Control.Invoke()";
            //    });
            //});

            //# 3.
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //var context = SynchronizationContext.Current;   //# ConsoleApplication에서 Synchronization.Current == null에 주의!
            //ThreadPool.QueueUserWorkItem(delegate
            //{
            //    Debug.WriteLine($"#1. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    Thread.Sleep(1500);
            //    context.Post(delegate
            //    {
            //        Debug.WriteLine($"#2. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //        textBox1.Text = "SynchronizationContext";
            //    }, null);
            //});
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //# 1.
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //await Task.Run(() =>
            //{
            //    Debug.WriteLine($"#1. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    Thread.Sleep(1500);
            //});
            //Debug.WriteLine($"#2. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //textBox1.Text = "await Task.Run()";

            //# 2.#1과 동일한 코드
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //await Task.Run(() =>
            //{
            //    Debug.WriteLine($"#1. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    Thread.Sleep(1500);
            //}).ConfigureAwait(true);
            //Debug.WriteLine($"#2. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //textBox1.Text = "await Task.Run()";

            //# 3.InvalidOperationException이 발생하는 코드
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //await Task.Run(() =>
            //{
            //    Debug.WriteLine($"#1. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    Thread.Sleep(1500);
            //}).ConfigureAwait(false);
            //Debug.WriteLine($"#2. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //textBox1.Text = "await Task.Run()";

            //# 4.#1과 동일한 코드, 단! async/await를 사용하지 않음
            Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            Task.Run(() =>
            {
                Debug.WriteLine($"#1. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1500);
            }).ContinueWith((t) =>
            {
                Debug.WriteLine($"#2. Other ThreadId #{Thread.CurrentThread.ManagedThreadId}");
                textBox1.Text = "await Task.Run()";
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
