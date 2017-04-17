using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public sealed class Actor
    {
        private readonly TaskFactory _taskFactory = new TaskFactory(new ActorTaskScheduer());

        /// <summary>
        /// Enqueu a piece of work that returns no result
        /// </summary>
        public Task Enqueue(Action work)
        {
            //return Task.Run(work);
            return _taskFactory.StartNew(work, TaskCreationOptions.HideScheduler);
        }

        /// <summary>
        /// Enqueue a piece of work that returns a result
        /// </summary>
        public Task<T> Enqueue<T>(Func<T> work)
        {
            //return Task.Run(work);
            return _taskFactory.StartNew(work, TaskCreationOptions.HideScheduler);
        }

        /// <summary>
        /// Enqueue an async of work that returns no result
        /// </summary>
        public async Task Enqueue(Func<Task> work)
        {
            await _taskFactory.StartNew(work, TaskCreationOptions.HideScheduler)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Enqueue an async of work that returns a result
        /// </summary>
        public async Task<T> Enqueue<T>(Func<Task<T>> work)
        {
            return await await _taskFactory.StartNew(work, TaskCreationOptions.HideScheduler)
                .ConfigureAwait(false);
        }

        //# 2.위 async Task<T> Enqueue<T>(Func<Task<T>>)와 동일한 기능을 함
        ///// <summary>
        ///// Enqueue an async of work that returns a result
        ///// </summary>
        //public async Task<T> Enqueue<T>(Func<Task<T>> work)
        //{
        //    return await _taskFactory.StartNew(work).Unwrap().ConfigureAwait(false);
        //}
    }
}
