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

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //# 1.rootTask1 type is Task<Task>
            var rootTask1 = Task.Factory.StartNew(() =>
            {
                var innerTask = Task.Factory.StartNew(() => { });
                return innerTask;
            });
            Debug.WriteLine($"rootTask1 type is {rootTask1.GetType()}");

            //# 2.rootTask2 type is Task<Task<int>>
            var rootTask2 = Task.Factory.StartNew(() =>
            {
                var innerTask = Task.Factory.StartNew(() => { return 42; });
                return innerTask;
            });
            Debug.WriteLine($"rootTask2 type is {rootTask2.GetType()}");

            //# 3.rootTask3 type is Task<Task<int>>
            //# rootTask3의 유형이 어떻게 Task<Task<int>>인지 설명할수 있나?
            //# - By using the async keyword here, the compiler going to the map this delegate to be a Func<Task<int>>:
            //# - invoking the delegate will return the Task<int> to represent the eventual completion of this call.
            //# - And since the delegate is Func<Task<int>>, TResult is Task<int>, and thus of 'rootTask3' is
            //# - going to be Task<Task<int>>, not Task<int>
            var rootTask3 = Task.Factory.StartNew(async delegate
            {
                await Task.Delay(1000);
                return 42;
            });
            Debug.WriteLine($"rootTask3 type is {rootTask3.GetType()}");

            //# 3.1.rootTask3_1 type is Task<int>
            //var rootTask3_1 = Task.Factory.StartNew(delegate
            //{
            //    Task.Delay(1000);
            //    return 42;
            //});
            //Debug.WriteLine($"rootTask3_1 type is {rootTask3_1.GetType()}");

            //# 4.rootTask4 type is Task<int>
            var rootTask4 = Task.Factory.StartNew(async delegate
            {
                await Task.Delay(1000);
                return 42;
            }).Unwrap();
            Debug.WriteLine($"rootTask4 type is {rootTask4.GetType()}");

            //# 5.rootTask5 type is Task<int>
            var rootTask5 = Task.Run(async delegate
            {
                await Task.Delay(1000);
                return 42;
            });
            Debug.WriteLine($"rootTask5 type is {rootTask5.GetType()}");

            //# 5.1. #5와 동일한 코드
            var rootTask5_1 = Task.Factory.StartNew(async delegate
            {
                await Task.Delay(1000);
                return 42;
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
            Debug.WriteLine($"rootTask5_1 type is {rootTask5_1.GetType()}");

            //# 6.
            int result6 = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                return 42;
            });
            Debug.WriteLine($"Result6={result6}");

            //# 7.
            int result7 = await Task.Factory.StartNew(async delegate
            {
                await Task.Delay(1000);
                return 42;
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
            Debug.WriteLine($"Result7={result7}");

            //# 7.1. #7과 동일한 코드
            int result7_1 = await await Task.Factory.StartNew(async delegate
            {
                await Task.Delay(1000);
                return 42;
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            Debug.WriteLine($"Result71={result7_1}");
        }
    }
}
