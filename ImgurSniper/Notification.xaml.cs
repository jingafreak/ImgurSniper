﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace ImgurSniper {
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window {
        private double _top;
        private double _left;
        private bool _autoHide;
        private TaskCompletionSource<bool> _task = new TaskCompletionSource<bool>();

        public enum NotificationType { Progress, Success, Error }

        public Notification(string text, NotificationType type, bool autoHide) {
            InitializeComponent();

            _left = SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width;
            _top = SystemParameters.WorkArea.Top + SystemParameters.WorkArea.Height;

            Left = _left;
            Top = _top - Height - 10;

            _autoHide = autoHide;

            contentLabel.Text = text;

            switch(type) {
                case NotificationType.Error:
                    errorIcon.Visibility = Visibility.Visible;
                    break;
                case NotificationType.Progress:
                    progressBar.Visibility = Visibility.Visible;
                    break;
                case NotificationType.Success:
                    successIcon.Visibility = Visibility.Visible;
                    break;
            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            FadeIn();

            if(_autoHide) {
                await Task.Delay(3000);

                FadeOut();
            }
        }

        public Task ShowAsync() {
            _autoHide = true;

            Show();

            return _task.Task;
        }

        public new void Close() {
            FadeOut();
        }

        //Open Animation
        private void FadeIn() {
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            DoubleAnimation slideInX = new DoubleAnimation(_left, _left - Width - 10, TimeSpan.FromMilliseconds(150));

            BeginAnimation(LeftProperty, slideInX);
            BeginAnimation(OpacityProperty, fadeIn);
        }

        //Close Animation
        private void FadeOut() {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(100));
            DoubleAnimation slideOutX = new DoubleAnimation(Left, _left, TimeSpan.FromMilliseconds(80));

            fadeOut.Completed += delegate {
                try {
                    _task.SetResult(true);
                } catch { }
                base.Close();
            };

            BeginAnimation(LeftProperty, slideOutX);
            BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
