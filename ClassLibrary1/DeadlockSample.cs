using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    /// <summary>
    /// UI가 있는 App에서 실행시 Deadlock이 발생하는 클래스 예제
    /// </summary>
    public class DeadlockSample
    {
        private async Task DelayAsync()
        {
            await Task.Delay(5000);

            //# async Task RunDeadlockAsync() 대신에 아래 코드를 사용하는 경우
            //# 적어도 Deadlock이 발생하지 않음
        }

        public void RunDeadlock()
        {
            Debug.WriteLine("DeadlockSample.RunDeadlock()");
            Debug.WriteLine($"SynchronizationContext.Current=" +
                $"{(SynchronizationContext.Current == null ? "null" : SynchronizationContext.Current.ToString())}");
            this.DelayAsync().Wait();
        }

        public async Task RunDeadlockAsync()
        {
            Debug.WriteLine("DeadlockSample.RunDeadlockSolutionAsync()");
            Debug.WriteLine($"SynchronizationContext.Current=" +
                $"{(SynchronizationContext.Current == null ? "null" : SynchronizationContext.Current.ToString())}");
            await this.DelayAsync();
        }
    }
}
