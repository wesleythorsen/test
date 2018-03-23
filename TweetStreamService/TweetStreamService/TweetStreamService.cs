using System;
using System.ServiceProcess;
using System.Threading;

namespace TweetStreamService
{
    public partial class TweetStreamService : ServiceBase
    {
        private Thread _Thread;
        
        public TweetStreamService()
        {
            InitializeComponent();
        }
        
        protected override void OnStart(string[] args)
        {
            Library.WriteErrorLog("Service starting.");
            _Thread = new Thread(new ThreadStart(RunStreamer));
            _Thread.Start();
        }

        protected override void OnStop()
        {
            Library.WriteErrorLog("Service stoping.");
            _Thread.Abort();
        }

        private void RunStreamer()
        {
            TweetStreamer streamer = null;
            while (true)
            {
                Library.WriteErrorLog("Creating new tweet streamer.");
                streamer = new TweetStreamer();
                Thread.Sleep(2000);

                try
                {
                    streamer.Start();
                }
                catch (Exception ex)
                {
                    Library.WriteErrorLog(ex.GetInnermostException());
                }
                finally
                {
                    streamer.Stop();
                    streamer = null;
                }
            }
        }
    }
}
