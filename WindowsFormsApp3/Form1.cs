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

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //# 1.별도의 TaskScheduler가 없기(ThreadPool Scheduler를 사용) 때문에 Actor에서 실행하는 작업이 순서대로 실행되지 않음
            //# - Actor의 Enqueue되는 순서대로 작업이 실행되기 위해서는 별도의 TaskScheduer가 필요(ActorTaskScheduer)
            Debug.WriteLine("START");

            Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            var actor = new Actor();
            var tasks = Enumerable.Range(1, 10)
                            .Select(n => actor.Enqueue(() =>
                            {
                                Debug.WriteLine($"{n}*{n}={n * n}, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
                            })).ToArray();
            await Task.WhenAll(tasks);

            Debug.WriteLine("END");

            //# 2.Actor.async Task Enqueue(Func<Task>) 메서드 테스트
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //var actor = new Actor();
            //await actor.Enqueue(async () =>
            //{
            //    await Task.Delay(1000);
            //    Debug.WriteLine($"async delegate, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //});

            //# 3.Actor.async Task<T> Enqueue<T>(Func<Task<T>>) 메서드 테스트
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //var actor = new Actor();
            //int result = await actor.Enqueue(async () =>
            //{
            //    await Task.Delay(1000);
            //    Debug.WriteLine($"async delegate, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    return 47;
            //});
            //Debug.WriteLine($"Result={result}");

            //# 4.
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //var actor = new Actor();
            //await actor.Enqueue(() =>
            //{
            //    //# 4.1.
            //    //# - Actor.QueueTask() 메서드 "Running actor task" 두번 출력됨
            //    //# - Actor가 수행할 작업은 하나지만 작업중 Task.Factory.StartNew() 메서드는 현재(Current) 스케줄러를
            //    //# - 를 사용하기 때문에 두번 출력됨
            //    //# - 이를 방지하기 위해서는 Task.Run() 메서드를 사용하거나
            //    //# - Actor에서 수행하는 작업은 무조건 기본(Default) 스케줄러를 사용하는 방법으로 변경 필요
            //    //# - TaskCreationOptions.HideScheduler
            //    Debug.WriteLine($"Hello, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    Task.Factory.StartNew(() =>
            //    {
            //        Debug.WriteLine($"Bonjour, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    });

            //    //# 4.2. #4.1과 동일한 코드
            //    //Debug.WriteLine($"Hello, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    //Task.Factory.StartNew(() =>
            //    //{
            //    //    Debug.WriteLine($"Bonjour, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    //}, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current);

            //    //# 4.3. #4.1의 개선된 코드
            //    //Debug.WriteLine($"Hello, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    //Task.Run(() =>
            //    //{
            //    //    Debug.WriteLine($"Bonjour, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    //});
            //});

            //# 5.await Task.Run()이후 실행되는 Debug.WriteLine()메서드가 Actor가 수행하는 스케줄러에서 실행되지 않음
            //# - Actor가 수행하는 스케줄러에서 실행하기 위해서 ActorSynchronizationContext를 제공해야 함
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //var actor = new Actor();
            //await actor.Enqueue(async () =>
            //{
            //    Debug.WriteLine($"Should be on actor, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    await Task.Run(() =>
            //    {
            //        Debug.WriteLine($"Should be off actor, Thread #{Thread.CurrentThread.ManagedThreadId}");
            //    });
            //    Debug.WriteLine($"Should be on actor, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //});

            //# 6.Actor가 별도의 SynchronizationContext를 제공하더라도 아래 await Task.Run().ConfigureAwait(false)를
            //#   사용하면 이후의 코드가 Actor가 수행하는 스케줄러에서 실행되지 않음
            //Debug.WriteLine($"UI ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //var actor = new Actor();
            //await actor.Enqueue(async () =>
            //{
            //    Debug.WriteLine($"Should be on actor, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //    await Task.Run(() =>
            //    {
            //        Debug.WriteLine($"Should be off actor, Thread #{Thread.CurrentThread.ManagedThreadId}");
            //    }).ConfigureAwait(false);
            //    Debug.WriteLine($"Should be on actor, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
            //});
        }
    }
}
