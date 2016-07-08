using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ChatBackend
{
    [Serializable]
    public class Command
    {
        public enum DrawAction
        {
            NONE,
            ADD,
            REMOVE,
            CHANGE,
            CLEAR
        };
        public Canvas canvas;
        public UIElement uiElement;
        public DrawAction action;

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double fontSize { get; set; }

        public Command(DrawAction action, double x1 = 0 , double y1 = 0, double x2 = 0, double y2 = 0, double size = 0)
        {
            this.action = action;
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
            fontSize = size;
        }


        public Command(Canvas canvas, UIElement uiElement, DrawAction action)
        {
            this.canvas = canvas;
            this.uiElement = uiElement;
            this.action = action;
        }

        public Command(Line line)
        {
        }

        public void Undo()
        {
            switch (action)
            {
                case DrawAction.ADD:
                    canvas.Children.Remove(uiElement);
                    break;
                case DrawAction.REMOVE:
                    break;
                case DrawAction.NONE:
                    break;
                case DrawAction.CHANGE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class FullCommand
    {
        public static Stack<FullCommand> Commands = new Stack<FullCommand>();

        public static void UndoAction()
        {
            if (Commands.Count == 0)
                return;
            Commands.Pop().Undo();
        }

        public List<Command> commands = new List<Command>();

        public FullCommand(List<Command> list)
        {
            commands.AddRange(list);
            Commands.Push(this);
        }

        public void Undo()
        {
            commands.ForEach(command => command.Undo());
        }
    }
}
