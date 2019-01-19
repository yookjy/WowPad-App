using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Apps.UI.Notification
{
    public partial class MessageTip : UserControl
    {
        private BackgroundWorker bgWorker;

        public string HoldKey { get; set; }

        public MessageTip()
        {
            InitializeComponent();
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
            StoryboardFadeOut.Completed += StoryboardFadeOut_Completed;

            this.MessageText.Text = string.Empty;
            Hide();
        }

        void StoryboardFadeOut_Completed(object sender, EventArgs e)
        {
            Hide();
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null && e.Argument is int)
            {
                System.Threading.Thread.Sleep((int)e.Argument);
            }
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StoryboardFadeOut.Begin();
        }

        public void FadeIn(string message)
        {
            this.MessageText.Text = message;
            this.Visibility = System.Windows.Visibility.Visible;
            //페이드인 효과주기
            StoryboardFadeIn.Begin();
        }

        public void FadeInOut(string message, int miliseconds)
        {
            //페이드인 효과주기
            FadeIn(message);
            //페이드 아웃 및 숨김처리
            FadeOut(miliseconds + 1000);   //Fade-In 애니메이션 시간(1초) 보정
        }

        public void Show(string message)
        {
            this.MessageText.Text = message;
            this.LayoutRoot.Opacity = 0.9;
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            this.HoldKey = string.Empty;
        }

        public void FadeOut(int miliseconds, string holdKey)
        {
            this.HoldKey = holdKey;
            FadeOut(miliseconds);
        }

        public void FadeOut(int miliseconds)
        {
            if (bgWorker.IsBusy != true)
            {
                bgWorker.RunWorkerAsync(miliseconds);
            }
        }

        public bool IsVisiable
        {
            get
            {
                return this.Visibility == System.Windows.Visibility.Visible || bgWorker.IsBusy;
            }
        }
    }

}
