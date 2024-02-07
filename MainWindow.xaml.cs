using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HydrogenDesktopMascot
{
    public partial class MainWindow : Window
    {
        private Point _lastMousePosition;
        private bool _isMouseDown;

        private BitmapImage rightOpenEye;
        private BitmapImage leftOpenEye;
        private BitmapImage closeMonth;
        private BitmapImage rightCloseEye;
        private BitmapImage leftCloseEye;
        private BitmapImage openMonth;

        private int blinkCount;
        private int blinkCountBase = 70;
        private int blinkRandomCount;

        bool isBrush;
        bool isClose;

        double scale = 1;
        private double initWidth = 0;
        private double initHeight = 0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MouseLeave += on_MouseLeave;
            initWidth = Width;
            initHeight = Height;
            rightCloseEye = new BitmapImage(new Uri("./Hydrogen/rightEye_close.png", UriKind.Relative));
            leftCloseEye = new BitmapImage(new Uri("./Hydrogen/leftEye_close.png", UriKind.Relative));
            openMonth = new BitmapImage(new Uri("./Hydrogen/mouth_open.png", UriKind.Relative));
            rightOpenEye = new BitmapImage(new Uri("./Hydrogen/rightEye.png", UriKind.Relative));
            leftOpenEye = new BitmapImage(new Uri("./Hydrogen/leftEye.png", UriKind.Relative));
            closeMonth = new BitmapImage(new Uri("./Hydrogen/mouth.png", UriKind.Relative));
            Random rnd = new Random();
            blinkRandomCount = rnd.Next(-50, 50);
            blinkCount += blinkCountBase + blinkRandomCount;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // タイマーを使って定期的にマウス位置を取得
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10); // 10ミリ秒ごとにマウス位置を取得(パフォーマンスに不安)
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // マウスカーソルの位置を取得して処理する
            System.Drawing.Point mousePosition = System.Windows.Forms.Cursor.Position;
            System.Windows.Point wpfMousePosition = PointFromScreen(new Point(mousePosition.X, mousePosition.Y));

            // マウスカーソルの位置を使って処理を行う
            on_MouseMove(sender, wpfMousePosition);
            if (blinkCount > 0)
            {
                blinkCount--;
                if (isBrush) return;
                if (!isClose) return;
                if (_isMouseDown) return;
                rightEye.Source = rightOpenEye;
                leftEye.Source = leftOpenEye;
                isClose = false;
            }
            else
            {
                Random rnd = new Random();
                blinkRandomCount = rnd.Next(-50, 50);
                blinkCount += blinkCountBase + blinkRandomCount;
                rightEye.Source = rightCloseEye;
                leftEye.Source = leftCloseEye;
                isClose = true;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            _lastMousePosition = e.GetPosition(this);
            rightEye.Source = rightCloseEye;
            leftEye.Source = leftCloseEye;
            mouth.Source = closeMonth;
            CaptureMouse();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            rightEye.Source = rightOpenEye;
            leftEye.Source = leftOpenEye;
            mouth.Source = closeMonth;
            ReleaseMouseCapture();
        }

        private void on_MouseMove(object sender, Point p)
        {
            Point mousePosition = p;
            double distanceX = mousePosition.X - Width / 2;
            double distanceY = mousePosition.Y - Height / 3;

            MouseLook(distanceX, distanceY);
        }

        private void on_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            rightEye.Source = rightOpenEye;
            leftEye.Source = leftOpenEye;
            mouth.Source = closeMonth;
            isBrush = false;
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.Delta > 0) scale = 1;
            else scale = 0.5;

            
            // ウィンドウのサイズを変更
            Width = initWidth*scale;
            Height = initHeight*scale;

            // 画像のサイズも変更
            ScaleImage(backHair, scale);
            ScaleImage(leftArm, scale);
            ScaleImage(rightArm, scale);
            ScaleImage(leftLeg, scale);
            ScaleImage(rightLeg, scale);
            ScaleImage(body, scale);
            ScaleImage(neckless, scale);
            ScaleImage(face, scale);
            ScaleImage(centerBoa, scale);
            ScaleImage(leftSclera, scale);
            ScaleImage(leftIris, scale);
            ScaleImage(leftEye, scale);
            ScaleImage(rightSclera, scale);
            ScaleImage(rightIris, scale);
            ScaleImage(rightEye, scale);
            ScaleImage(cheek, scale);
            ScaleImage(mouth, scale);
            ScaleImage(nose, scale);
            ScaleImage(leftHair, scale);
            ScaleImage(rightHair, scale);
            ScaleImage(centerHair, scale);
            ScaleImage(topHair, scale);
            ScaleImage(leftBrows, scale);
            ScaleImage(rightBrows, scale);
        }

        private void ScaleImage(Image image, double delta)
        {
            // サイズを変更
            image.Width = initWidth * delta;
            image.Height = initHeight * delta;
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);

            if (!_isMouseDown)
            {
                // 撫でる
                if (mousePosition.Y < Height / 4)
                {
                    rightEye.Source = rightCloseEye;
                    leftEye.Source = leftCloseEye;
                    mouth.Source = openMonth;
                    isBrush = true;
                }
                else
                {
                    rightEye.Source = rightOpenEye;
                    leftEye.Source = leftOpenEye;
                    mouth.Source = closeMonth;
                    isBrush = false;
                }
            }

            if (!_isMouseDown) return;

            double deltaX = mousePosition.X - _lastMousePosition.X;
            double deltaY = mousePosition.Y - _lastMousePosition.Y;

            double newLeft = Left + deltaX;
            double newTop = Top + deltaY;

            Left = newLeft;
            Top = newTop;

        }

        private void MouseLook(double distanceX, double distanceY)
        {
            int MAXPOSITION = 400;
            distanceX = Math.Max(Math.Min(MAXPOSITION, distanceX),-MAXPOSITION);
            distanceY = Math.Max(Math.Min(MAXPOSITION, distanceY), -MAXPOSITION);

            ObjectLook(leftEye, distanceX,distanceY,75);
            ObjectLook(rightEye, distanceX, distanceY, 75);
            ObjectLook(leftSclera, distanceX, distanceY, 75);
            ObjectLook(rightSclera, distanceX, distanceY, 75);
            ObjectLook(leftIris, distanceX, distanceY, 40);
            ObjectLook(rightIris, distanceX, distanceY, 40);
            ObjectLook(leftBrows, distanceX, distanceY, 75);
            ObjectLook(rightBrows, distanceX, distanceY, 75);
            ObjectLook(mouth, distanceX, distanceY, 75);
            ObjectLook(nose, distanceX, distanceY, 75);
            ObjectLook(face, distanceX, distanceY, 100);
            ObjectLook(centerHair, distanceX, distanceY, 120);
            ObjectLook(backHair, distanceX, distanceY, -140);
            ObjectLook(topHair, distanceX, distanceY, 140);
            ObjectLook(rightHair, distanceX, distanceY, 125);
            ObjectLook(leftHair, distanceX, distanceY, 125);
            ObjectLook(body, distanceX, distanceY, -200,false);
            ObjectLook(neckless, distanceX, distanceY, -250, false);
            ObjectLook(centerBoa, distanceX, distanceY, 500);
            ObjectLook(rightArm, distanceX, distanceY, -100,false);
            ObjectLook(leftArm, distanceX, distanceY, -100, false);
            ObjectLook(rightLeg, distanceX, distanceY, -400, false);
            ObjectLook(leftLeg, distanceX, distanceY, -400, false);
        }

        private void ObjectLook(Image image, double distanceX, double distanceY, int weight, bool moveY = true, bool moveXReverse = false)
        {
            distanceX = distanceX * scale;
            distanceY = distanceY * scale;
            if (moveXReverse) distanceX = -distanceX;
            Canvas.SetLeft(image, distanceX / weight);
            if(moveY)
            {
                Canvas.SetTop(image, distanceY / weight);
            }
        }
    }
}
