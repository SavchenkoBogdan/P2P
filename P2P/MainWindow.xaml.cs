using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatBackend;

namespace P2P
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatBackend.ChatBackend _backend;

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            _backend = new ChatBackend.ChatBackend(this.DisplayMessage, this.UpdateCanvas);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _backend.StopService();
        }

        public void DisplayMessage(ChatBackend.CompositeType composite)
        {
            string username = composite.Username == null ? "" : composite.Username;
            string message = composite.Message == null ? "" : composite.Message;
            textBoxChatPane.Text += (username + ": " + message + Environment.NewLine);
        }

        private void textBoxEntryField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                _backend.SendMessage(textBoxEntryField.Text);
                textBoxEntryField.Clear();
            }
        }


        public void UpdateCanvas(ChatBackend.ImageType imageData)
        {
            var command = imageData.Comand;

            if (command.action == Command.DrawAction.ADD)
            {
                Line line = new Line();
                line.Stroke = SystemColors.WindowFrameBrush;
                line.StrokeThickness = command.fontSize;
                line.X1 = command.X1;
                line.Y1 = command.Y1;
                line.X2 = command.X2;
                line.Y2 = command.Y2;
                paintSurface.Children.Add(line);
            }
            else if (command.action == Command.DrawAction.CLEAR)
                ClearCanvas();
        }

        private float fontSize = 1f;
        Point currentPoint = new Point();

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(paintSurface);
                //tmpCommandsCollection.Clear();
            }
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();
                //DrawLine(currentPoint, e.GetPosition(paintSurface));
                line.Stroke = SystemColors.WindowFrameBrush;
                line.StrokeThickness = fontSize;

                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(paintSurface).X;
                line.Y2 = e.GetPosition(paintSurface).Y;
                currentPoint = e.GetPosition(paintSurface);
                paintSurface.Children.Add(line);

                var cmd = new Command(Command.DrawAction.ADD, line.X1, line.Y1, line.X2, line.Y2, fontSize);
                _backend.SendMessage(cmd);
                //tmpCommandsCollection.Add(new Command(paintSurface, line, Command.DrawAction.ADD));
            }
        }

        private void ClearCanvas()
        {
            paintSurface.Children.Clear();
            paintSurface.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
            _backend.SendMessage(new Command(Command.DrawAction.CLEAR));
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                fontSize = float.Parse((sender as TextBox).Text);
            }
            catch (Exception exception)
            {
                // ignored
            }
        }
    }
}
