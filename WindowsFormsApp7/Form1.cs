using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        private string sourceFile = @"C:\Users\Lee SangDae\Desktop\아산병원\윈도우_무선_접속_가이드.pdf";
        private string targetFile = @"C:\Users\Lee SangDae\Desktop\아산병원\Temp.pdf";

        public Form1()
        {
            InitializeComponent();
        }

        #region == 1.Think Chunk, Not Chatty ==
        //# async 메서드는 C# 컴파일러가 button2_ClickStateMachine와 같은 코드를 생성함에 유의
        //# - try-catch 블럭 사용으로 인한 JIT가 Inline 처리를 할수 없는 성능상의 단점
        //# - StateMachine에 대응하는 객체가 생성되어 메모리 할당이 되는 성능상의 단점
        private async Task SimpleBodyAsync()
        {
            //# async로 선언되어 있지만, await가 없기 때문에 비동기가 아닌 동기로 실행 됨
            //# 기사의 내용과 다른게 컴파일러가 비동기 실행코드를 만들지는 않음, 다만 대응하는 StateMachine 클래스를 생성하고 사용하지 않음
            Debug.WriteLine("Hello, Async World!");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //# 1.C# 컴파일러가 아래 코드에 대응하는 코드를 생성한게 #2임
            string contents = await new HttpClient().GetStringAsync("http://www.daum.net");
            Debug.WriteLine($"Content: {contents}");

            //# 2.C# 컴파일러가 생성한 코드
            //var stateMachine = new button2_ClickStateMachine
            //{
            //    _sender = sender,
            //    _e = e,
            //    _builder = AsyncVoidMethodBuilder.Create(),
            //    _state = -1
            //};
            //stateMachine._builder.Start(ref stateMachine);
        }

        //private sealed class button2_ClickStateMachine : IAsyncStateMachine
        //{
        //    public object _sender;
        //    public EventArgs _e;
        //    public int _state;
        //    public AsyncVoidMethodBuilder _builder;
        //    private TaskAwaiter<string> awaiter;

        //    public void MoveNext()
        //    {
        //        try
        //        {
        //            if (_state != 0)
        //            {
        //                awaiter = new HttpClient().GetStringAsync("http://www.daum.net").GetAwaiter();
        //                if (!awaiter.IsCompleted)
        //                {
        //                    _state = 0;
        //                    var stateMachine = this;
        //                    _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                _state = -1;
        //            }

        //            string contents = awaiter.GetResult();
        //            Debug.WriteLine($"Contents: {contents}");
        //        }
        //        catch (Exception ex)
        //        {
        //            _state = -2;
        //            _builder.SetException(ex);
        //            return;
        //        }
        //        _state = -2;
        //        _builder.SetResult();
        //    }

        //    public void SetStateMachine(IAsyncStateMachine stateMachine)
        //    {

        //    }
        //} 
        #endregion

        #region == 2.Known When not use Async I ==
        //# Cache를 이용하여 메모리 할당을 감소 처리

        private async void button1_Click(object sender, EventArgs e)
        {
            string contents = await this.GetContentAsync("http://www.daum.net");
        }

        #region == 1.성능상 문제가 있는 Async 메서드 ==
        //# Cache에 있는 string -> Task<string>으로 생성하는 과정에서 Object allocation이 발생?
        //# 이에 대응하는 최적화된 코드는 #2 참조
        private static ConcurrentDictionary<string, string> _urlToContents = new ConcurrentDictionary<string, string>();

        public async Task<string> GetContentAsync(string url)
        {
            string contents;
            if (!_urlToContents.TryGetValue(url, out contents))
            {
                var response = await new HttpClient().GetAsync(url);
                contents = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                _urlToContents.TryAdd(url, contents);
            }
            return contents;
        }
        #endregion

        #region == 2.최적화된 Async 메서드 ==
        //private static ConcurrentDictionary<string, Task<string>> _urlToContents = new ConcurrentDictionary<string, Task<string>>();

        //public Task<string> GetContentAsync(string url)
        //{
        //    Task<string> contents;
        //    if (!_urlToContents.TryGetValue(url, out contents))
        //    {
        //        contents = this.GetContentInternalAsync(url);
        //        contents.ContinueWith(delegate
        //        {
        //            _urlToContents.TryAdd(url, contents);
        //        }
        //        , CancellationToken.None
        //        , TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
        //        , TaskScheduler.Current);
        //    }
        //    return contents;
        //}

        //private async Task<string> GetContentInternalAsync(string url)
        //{
        //    var response = await new HttpClient().GetAsync(url);
        //    return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
        //}
        #endregion

        #endregion

        #region == 3.Known When not use Async II ==
        private async void button3_Click(object sender, EventArgs e)
        {

            //# 1.동기
            //byte[] file = File.ReadAllBytes(sourceFile);
            //using (var source = new MemoryStreamEx(file))
            //using (var target = new MemoryStreamEx())
            //{
            //    byte[] buffer = new byte[1000];
            //    int numRead = 0;
            //    while ((numRead = source.Read(buffer, 0, buffer.Length)) > 0)
            //        target.Write(buffer, 0, numRead);

            //    using (var fileStream = new FileStream(targetFile, FileMode.Create))
            //    {
            //        target.Position = 0;
            //        target.CopyTo(fileStream);
            //    }
            //}

            //# 2.비동기 I
            //byte[] file = File.ReadAllBytes(sourceFile);
            //using (var source = new MemoryStreamEx(file))
            //using (var target = new MemoryStreamEx())
            //{
            //    byte[] buffer = new byte[1000];
            //    int numRead = 0;
            //    while ((numRead = await source.ReadAsync(buffer, 0, buffer.Length)) > 0)
            //        await target.WriteAsync(buffer, 0, numRead);

            //    using (var fileStream = new FileStream(targetFile, FileMode.Create))
            //    {
            //        target.Position = 0;
            //        target.CopyTo(fileStream);
            //    }
            //}

            //# 3.비동기 II
            //# - #2코드에서 SynchronizationContext 캡쳐 작업을 진행하지 않아 성능 향상
            byte[] file = File.ReadAllBytes(sourceFile);
            using (var source = new MemoryStreamEx(file))
            using (var target = new MemoryStreamEx())
            {
                byte[] buffer = new byte[1000];
                int numRead = 0;
                while ((numRead = await source.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                    await target.WriteAsync(buffer, 0, numRead).ConfigureAwait(false);

                using (var fileStream = new FileStream(targetFile, FileMode.Create))
                {
                    target.Position = 0;
                    target.CopyTo(fileStream);
                }
            }
        }
        #endregion

        #region == 4.Lift Your Way out of Garbage Collection ==
        private async void button4_Click(object sender, EventArgs e)
        {
            //# 1.dateTimeOffset은 await 이후에 사용되지 않지만, 컴파일러가 생성한 코드에서 StateMachine 클래스에 포함됨
            var dateTimeOffset = DateTimeOffset.Now;
            var now = dateTimeOffset.DateTime;
            await Task.Yield();
            Debug.WriteLine(now);

            //# 2.필요없는 객체를 StateMachine 클래스에 포함되지 않게 처리한 코드
            //var now = DateTimeOffset.Now.DateTime;
            //await Task.Yield();
            //Debug.WriteLine(now);
        }
        #endregion
    }

    public class MemoryStreamEx : MemoryStream
    {
        private Task<int> _lastReadTask;

        #region == Constructors ==
        public MemoryStreamEx() : base() { }

        public MemoryStreamEx(byte[] buffer) : base(buffer) { } 
        #endregion

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            //# 1.최적화전 코드(16M)
            //cancellationToken.ThrowIfCancellationRequested();
            //return Task.FromResult(base.Read(buffer, offset, count));

            //# 2.최적화된 코드(15.9M)
            //# - 위 #1번에서 반복적으로 동일한 크기의 Task<int> 객체를 생성하는 대신
            //# - Task<int> 객체를 Cache 처리하여 성능을 향상 시킴
            //# - 문제는 얼마나 성능 향상이 있는지 확인할수 있는 방법을 모름..
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            try
            {
                int readNum = base.Read(buffer, offset, count);
                var task = _lastReadTask;
                return (task != null && task.Result == readNum) ? task : (_lastReadTask = Task.FromResult<int>(readNum));
            }
            catch (OperationCanceledException oce)
            {
                return Task.FromException<int>(oce);
            }
            catch (Exception ex)
            {
                return Task.FromException<int>(ex);
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            base.Write(buffer, offset, count);
            return Task.CompletedTask;
        }
    }
}
