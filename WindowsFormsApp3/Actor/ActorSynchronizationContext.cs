using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public class ActorSynchronizationContext : SynchronizationContext
    {
        private readonly TaskFactory _taskFactory;

        public ActorSynchronizationContext(ActorTaskScheduer actorTaskScheduler)
        {
            _taskFactory = new TaskFactory(actorTaskScheduler);
        }

        #region == Overriden members of the SynchronizationContext class ==
        /// <summary>
        /// When overriden in derived class, dispatchs an asynchronous message to synchronization context.
        /// </summary>
        /// <param name="callback">The <see cref="SendOrPostCallback"/> delegate to call.</param>
        /// <param name="state">The object passed to the delegate</param>
        public override void Post(SendOrPostCallback callback, object state)
        {
            _taskFactory.StartNew(() => callback(state), TaskCreationOptions.HideScheduler);
        } 
        #endregion
    }
}
