using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    class MainViewModel
    {
        public event EventHandler<EventArgs> RequestedClose;

        public virtual void OnRequestedClose(EventArgs e)
        {
            this.RequestedClose?.Invoke(this, e);
        }

        public void RequestClose()
        {
            this.OnRequestedClose(EventArgs.Empty);
        }

        internal void Initialize()
        {
            Thread.Sleep(3000);
        }

        internal Task InitializeAsync()
        {
            return Task.Delay(3000);
            //throw new NotImplementedException();
        }
    }
}
