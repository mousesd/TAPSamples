using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp1
{
    #region == 1.WPF에서 자동으로 생성하는 Main() 메서드 대신의 경우 ==
    //static class Program
    //{
    //    [STAThread]
    //    static void Main()
    //    {
    //        var app = new App();
    //        app.InitializeComponent();
    //        app.Run();
    //    }
    //}
    #endregion

    #region == 2.Beginning of an Object-Oriented Startup ==
    //class Program
    //{
    //    private readonly App _app;

    //    [STAThread]
    //    static void Main()
    //    {
    //        var program = new Program();
    //        program.Start();
    //    }

    //    private Program()
    //    {
    //        _app = new App();
    //        _app.InitializeComponent();
    //    }

    //    private void Start()
    //    {
    //        _app.Run();
    //    }
    //}
    #endregion

    #region == 3.Decoupling the Application from the Form ==
    //# Remove the StartupUri from App.xaml
    //class Program
    //{
    //    private readonly App _app;

    //    [STAThread]
    //    static void Main()
    //    {
    //        var program = new Program();
    //        program.Start();
    //    }

    //    private Program()
    //    {
    //        _app = new App();
    //        _app.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown; //# which decouples the application lifetime from that of the windows
    //    }

    //    private void Start()
    //    {
    //        var viewModel = new MainViewModel();
    //        viewModel.RequestedClose += ViewModel_RequestedClose;
    //        viewModel.Initialize();

    //        var window = new MainWindow();
    //        window.Closed += (sender, e) => viewModel.RequestClose();
    //        window.DataContext = viewModel;
    //        window.Show();
    //        _app.Run();
    //    }

    //    private void ViewModel_RequestedClose(object sender, EventArgs e)
    //    {
    //        _app.Shutdown();
    //    }
    //}
    #endregion

    #region == 4.Pulling out the Hosting Environment ==
    class Program
    {
        public event EventHandler<EventArgs> RequestedExit;

        [STAThread]
        static void Main()
        {
            var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var program = new Program();
            program.RequestedExit += (sender, e) => app.Shutdown();
            var task = program.StartAsync();
            HandleExceptions(task, app);

            app.Run();
        }

        private Program() { }

        private static async void HandleExceptions(Task task, Application app)
        {
            try
            {
                await Task.Yield();
                await task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                app.Shutdown();
            }
        }

        private async Task StartAsync()
        {
            var viewModel = new MainViewModel();
            viewModel.RequestedClose += ViewModel_RequestedClose;
            EventHandler closedWindow = (sender, e) => viewModel.RequestClose();

            var splashWindow = new SplashScreen();
            {
                splashWindow.Closed += closedWindow;
                splashWindow.Show();

                await viewModel.InitializeAsync();

                //# This will be on a threadpool thread instead of a UI thread,
                //# and won't work it will throw an exception because it's not an STA thread.
                var window = new MainWindow();
                window.Closed += closedWindow;
                window.DataContext = viewModel;
                window.Show();

                splashWindow.Owner = window;
                splashWindow.Closed -= closedWindow;
                splashWindow.Close();
            }
        }

        private void ViewModel_RequestedClose(object sender, EventArgs e)
        {
            this.OnRequestedExit(e);
        }

        protected virtual void OnRequestedExit(EventArgs e)
        {
            this.RequestedExit?.Invoke(this, e);
        }
    }
    #endregion
}
