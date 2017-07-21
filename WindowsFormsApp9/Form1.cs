using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp9
{
    public partial class Form1 : Form
    {
        #region == Fields & Property ==
        /// <summary>
        /// 원본파일 경로
        /// </summary>
        private readonly string sourcePath;

        /// <summary>
        /// 대상파일 경로
        /// </summary>
        private readonly string destinationPath;
        #endregion

        #region == Constructors ==
        /// <summary>
        /// 생성자
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            sourcePath = @"C:\Users\Lee SangDae\Downloads\win32_11gR2_client.zip";
            destinationPath = @"C:\Users\Lee SangDae\Downloads\__Pending\" + Path.GetFileName(sourcePath);
        }
        #endregion

        #region == Event handlers of the Windows controls ==
        /// <summary>
        /// 동기 파일 복사
        /// </summary>
        private void btnSync_Click(object sender, EventArgs e)
        {
            var watch = Stopwatch.StartNew();
            using (var source = File.OpenRead(sourcePath))
            using (var destination = File.Create(destinationPath))
            {
                this.CopyTo(source, destination);
            }
            watch.Stop();
            Debug.WriteLine($"Synchronous CopyTo(): {watch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// APM 패턴을 이용한 파일 복사
        /// </summary>
        private void btnApm_Click(object sender, EventArgs e)
        {
            var watch = Stopwatch.StartNew();
            var source = File.OpenRead(sourcePath);
            var destination = File.Create(destinationPath);
            var ar = this.BeginCopyTo(source, destination);

            watch.Stop();
            Debug.WriteLine($"Asynchronous Programming Model Pattern CopyTo(): {watch.ElapsedMilliseconds}ms");

            while (!ar.IsCompleted)
                Thread.Sleep(100);

            destination.Dispose();
            source.Dispose();
        }

        /// <summary>
        /// TAP를 이용한 파일 복사
        /// </summary>
        private async void btnTap_Click(object sender, EventArgs e)
        {
            var watch = Stopwatch.StartNew();
            using (var source = File.OpenRead(sourcePath))
            using (var destination = File.Create(destinationPath))
            {
                await this.CopyToAsync(source, destination);
            }
            watch.Stop();
            Debug.WriteLine($"Task-based Asynchronous Pattern CopyTo(): {watch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// 넷플릭스의 영화목록을 TAP를 이용해 조회하는 예제
        /// </summary>
        private async void btnGetMovies_Click(object sender, EventArgs e)
        {
            //# NOTE: 2017-07-21 기준으로 넷플릭스에서 OData 서비스를 지원하지 않아 동작하지 않음
            var service = new NetflixMovieService();
            var movies = await service.GetMoviesAsync();
            Debug.WriteLine($"Move Count={movies.Count}");
        }
        #endregion

        #region == Methods ==
        /// <summary>
        /// 동기로 스트림 복사
        /// </summary>
        /// <param name="source">원본 스트림</param>
        /// <param name="destination">대상 스트림</param>
        private void CopyTo(Stream source, Stream destination)
        {
            //# Synchronous
            int numRead;
            var buffer = new byte[0x1000];

            while ((numRead = source.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, numRead);
        }

        /// <summary>
        /// APM를 이용한 스트림 복사
        /// </summary>
        /// <param name="source">원본 스트림</param>
        /// <param name="destination">대상 스트림</param>
        /// <returns><see cref="IAsyncResult"/></returns>
        private IAsyncResult BeginCopyTo(Stream source, Stream destination)
        {
            //# Asynchronous Programming Model Pattern
            var buffer = new byte[0x1000];
            var tcs = new TaskCompletionSource<bool>();
            Action<IAsyncResult> readWriteLoop = null;
            readWriteLoop = iar =>
            {
                try
                {
                    for (bool isRead = iar == null; ; isRead = !isRead)
                    {
                        if (isRead)
                        {
                            iar = source.BeginRead(buffer, 0, buffer.Length, result =>
                            {
                                if (result.CompletedSynchronously)
                                    return;
                                readWriteLoop(result);
                            }, null);

                            if (!iar.CompletedSynchronously)
                                return;
                        }
                        else
                        {
                            int numRead = source.EndRead(iar);
                            if (numRead == 0)
                            {
                                tcs.TrySetResult(true);
                                return;
                            }

                            iar = destination.BeginWrite(buffer, 0, numRead, result =>
                            {
                                try
                                {
                                    if (result.CompletedSynchronously)
                                        return;
                                    destination.EndWrite(result);
                                    readWriteLoop(null);
                                }
                                catch (Exception ex)
                                {
                                    tcs.SetException(ex);
                                }
                            }, null);

                            if (!iar.CompletedSynchronously)
                                return;
                            destination.EndWrite(iar);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            };

            readWriteLoop(null);
            return tcs.Task;
        }

        /// <summary>
        /// TAP를 이용한 스트림 복사
        /// </summary>
        /// <param name="source">원본 스트림</param>
        /// <param name="destination">대상 스트림</param>
        private async Task CopyToAsync(Stream source, Stream destination)
        {
            //# Task-based Asynchronous Pattern
            int numRead;
            var buffer = new byte[0x1000];

            while ((numRead = await source.ReadAsync(buffer, 0, buffer.Length)) > 0)
                await destination.WriteAsync(buffer, 0, numRead);
        }
        #endregion
    }
}
