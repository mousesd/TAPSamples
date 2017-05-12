using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
        public Form1()
        {
            InitializeComponent();
        }

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

        #region == 3.Think Chunk, Not Chatty ==
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
    }
}
