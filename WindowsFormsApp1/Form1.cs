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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Compute(3);
        }

        private void Compute(int counter)
        {
            if (counter == 0)
                return;

            Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");

            //# 1.
            //# - SomeMethod()가 첫번째만 ThreadPool의 스레드를 사용, 이후 2, 3번째는 모두 UI 스레드를 사용하는 문제점
            //# - 이 코드에 대한 개선사항은 아래 #3, #4 참조
            var uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() => this.SomeMethod(counter))
                .ContinueWith(task =>
                {
                    textBox1.Text = task.Result.ToString();
                    this.Compute(counter - 1);
                }, uiTaskScheduler);

            //# 2.위 #1과 동일한 코드
            //var uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            //Task.Factory.StartNew(() => this.SomeMethod(counter), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current)
            //    .ContinueWith(task =>
            //    {
            //        textBox1.Text = task.Result.ToString();
            //        this.Compute(counter - 1);
            //    }, uiTaskScheduler);

            //# 3.
            //var uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            //Task.Factory.StartNew(() => this.SomeMethod(counter), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
            //    .ContinueWith(task =>
            //    {
            //        textBox1.Text = task.Result.ToString();
            //        this.Compute(counter - 1);
            //    }, uiTaskScheduler);

            //# 4.
            //var uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            //Task.Run(() => this.SomeMethod(counter))
            //    .ContinueWith(task =>
            //    {
            //        textBox1.Text = task.Result.ToString();
            //        this.Compute(counter - 1);
            //    }, uiTaskScheduler);
        }

        private int SomeMethod(int value)
        {
            Debug.WriteLine($"SomeMethod(), ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            return value;
        }
    }
}
