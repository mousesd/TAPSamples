using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp8
{
    #region == 1.Original ==
    //static class Program
    //{
    //    /// <summary>
    //    /// 해당 응용 프로그램의 주 진입점입니다.
    //    /// </summary>
    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        Application.Run(new Form1());
    //    }
    //}
    #endregion

    #region == 2.Beginning of an Object-Oriented Startup ==
    //class Program
    //{
    //    [STAThread]
    //    static void Main()
    //    {
    //        var program = new Program();
    //        program.Start();
    //    }

    //    private Program()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //    }

    //    private void Start()
    //    {
    //        //# Application.Run() that takes a form instance poses a few problems.
    //        //# One is a generic architecturl concern: I don't like that it ties my application's lifetime to displaying that form.
    //        Application.Run(new Form1());
    //    }
    //}
    #endregion

    #region == 3.Decoupling the Application from the Form ==
    //class Program
    //{
    //    private readonly Form1 _form;

    //    [STAThread]
    //    static void Main()
    //    {
    //        var program = new Program();
    //        program.Start();
    //    }

    //    private Program()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);

    //        //# This decoupling means I have to show the form myself;
    //        //# it also means that closing the form will no longer quit the app, so I need to wire that up explicitly
    //        _form = new Form1();
    //        _form.FormClosed += MainForm_Closed;
    //    }

    //    private void Start()
    //    {
    //        _form.Initialize();
    //        _form.Show();
    //        Application.Run();
    //    }

    //    private void MainForm_Closed(object sender, FormClosedEventArgs e)
    //    {
    //        Application.Exit();
    //    }
    //}
    #endregion

    #region == 4.Pulling out the Hosting Environment ==
    //# Separating the hosting environment has a few benefits. 
    //# For one, it makes testing easier (you can now test Program, to a limited extent).
    //# It also makes it easier to reuse the code elsewhere, perhaps embedded into a larger application or a “launcher” screen.
    class Program
    {
        private readonly Form1 _form;
        public event EventHandler<EventArgs> RequestedExit;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());

            var program = new Program();
            program.RequestedExit += Program_RequestedExit;
            var task = program.StartAsync();
            HandleExceptions(task);

            Debug.WriteLine("Just before Applciation.Run()");
            Application.Run();
        }

        private static async void HandleExceptions(Task task)
        {
            try
            {
                //# In this case, when I call “await Task.Yield”, HandleExceptions will return and Application.Run will execute.
                //# The remainder of HandleExceptions will then be posted as a continuation to the current SynchronizationContext,
                //# which means it will be picked up by Application.Run.

                //# await Task.Yield()
                //# - https://stackoverflow.com/questions/23431595/task-yield-real-usages/23441833#23441833
                //# - https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                //# - https://stackoverflow.com/questions/20319769/await-task-yield-and-its-alternatives
                await Task.Yield();
                await task;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HandleExceptions(): {ex.Message}");
                Application.Exit();
            }
        }

        private static void Program_RequestedExit(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private Program()
        {
            _form = new Form1();
            _form.FormClosed += MainForm_Closed;
        }

        private async Task StartAsync()
        {
            //# 1.
            //await _form.InitializeAsync();
            //_form.Show();

            //# 2.
            //try
            //{
            //    //await Task.Yield();
            //    await _form.InitializeAsync();
            //    _form.Show();
            //}
            //catch (Exception ex)
            //{
            //    //# 메시지박스 표시후 App이 종료되지 않는 이유에 대해 확인
            //    //# Application.Run()이 실행전 이라면 Application.Exit()는 동작하지 않음
            //    //# 동작하기 위해서는 await Task.Yield() 메서드를 추가하면 됨!
            //    MessageBox.Show($"StartAsync(): {ex.Message}");
            //    Application.Exit();
            //}

            //# 3.SplashScreen 기능이 포함된 경우
            using (var splashForm = new SplashScreen())
            {
                //# if user closes splash screen, quit that would also be a good opportunity to set a cancellation token
                //# This ensures the activation works so when the splash screen goes away,
                //# the main form(_form) is activated.
                splashForm.FormClosed += MainForm_Closed;
                splashForm.Owner = _form;
                splashForm.Show();

                await _form.InitializeAsync();
                _form.Show();

                splashForm.FormClosed -= MainForm_Closed;
                splashForm.Close();
            }

            //# Application.Run()가 Main() 메서드가 아닌 아래에 있는 경우는 프로그램이 시작후 바로 종료됨!
            //Debug.WriteLine("Just before Applciation.Run()");
            //Application.Run();
        }

        private void MainForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.OnRequestedExit(EventArgs.Empty);
        }

        protected virtual void OnRequestedExit(EventArgs e)
        {
            this.RequestedExit?.Invoke(this, e);
        }
    }
    #endregion
}
