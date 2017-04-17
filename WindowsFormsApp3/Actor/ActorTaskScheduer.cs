using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public sealed class ActorTaskScheduer : TaskScheduler
    {
        #region == Fields & Properties ==
        /// <summary>
        /// Lock Object
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Task Queues
        /// </summary>
        private readonly Queue<Task> _taskQueue = new Queue<Task>();

        /// <summary>
        /// SynchronizationContext
        /// </summary>
        private readonly ActorSynchronizationContext _context;

        /// <summary>
        /// 현재 작업여부
        /// </summary>
        private bool _isActive = false;

        /// <summary>
        /// Get the Indicates the maximum concurrency level this TaskScheduler is able to support.
        /// </summary>
        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }
        #endregion

        #region == Constructors ==
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="context"><see cref="ActorSynchronizationContext"/></param>
        public ActorTaskScheduer()
        {
            _context = new ActorSynchronizationContext(this);
        } 
        #endregion

        #region == Implements members of the TaskScheduer abstract class ==
        /// <summary>
        /// For debugger support only, generates an enumerable of Task instances currently queued to the scheduler waiting to be executed.
        /// </summary>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queues a Task to the scheduer.
        /// </summary>
        protected override void QueueTask(Task task)
        {
            lock (_lockObject)
            {
                _taskQueue.Enqueue(task);
                if (!_isActive)
                {
                    _isActive = true;
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        //# 1.아래는 코드는 while문에서 무한루프가 발생함, 반드시 #2 처럼 사용해야 함
                        //var nextTask = this.TryGetNextTask();
                        //while (nextTask != null)
                        //    this.TryExecuteTask(nextTask);

                        //# 2.
                        SynchronizationContext.SetSynchronizationContext(_context);

                        Task nextTask = null;
                        while ((nextTask = this.TryGetNextTask()) != null)
                        {
                            Debug.WriteLine($"Running actor task, ThreadId #{Thread.CurrentThread.ManagedThreadId}");
                            this.TryExecuteTask(nextTask);
                        }

                        SynchronizationContext.SetSynchronizationContext(null);
                    });
                }
            }
        }

        /// <summary>
        /// Determines whether the provided Task can be synchronously in this call, and it it can, executes it.
        /// </summary>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
        #endregion

        /// <summary>
        /// Queue에서 다음 작업을 얻기
        /// </summary>
        private Task TryGetNextTask()
        {
            lock (_lockObject)
            {
                if (_taskQueue.Count > 0)
                    return _taskQueue.Dequeue();
                else
                {
                    _isActive = false;
                    return null;
                }
            }
        }
    }
}
