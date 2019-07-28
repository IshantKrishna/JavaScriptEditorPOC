using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;


namespace DraggableTextBox
{
    public class TextBoxDragBehavior : Behavior<TextBox>
    {
        bool isDown, isDragging, isSelected;
        //UIElement selectedElement = null;
        double originalLeft, originalTop;
        Point startPoint;

        AdornerLayer adornerLayer;
        private Canvas ParentCanvas;

        

        

        protected override void OnAttached()
        {
            ParentCanvas = FindParent<Canvas>(AssociatedObject);
            base.OnAttached();
            //registering mouse events
            AssociatedObject.MouseLeftButtonDown += TextBoxMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += TextBoxMouseLeftButtonUp;
            AssociatedObject.MouseMove += TextBoxMouseMove;
            AssociatedObject.MouseLeave += TextBoxMouseLeave;
            AssociatedObject.LostFocus += TextLostFocus;
            AssociatedObject.GotFocus += TextGotFocus;
        }

        private void TextGotFocus(object sender, RoutedEventArgs e)
        {
            //adding adorner on selected element
             adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            adornerLayer.Add(new BorderAdorner(AssociatedObject));
                isSelected = true;
                e.Handled = true;
            
        }

        private void TextLostFocus(object sender, RoutedEventArgs e)
        {
            adornerLayer.Remove(adornerLayer.GetAdorners(AssociatedObject)[0]);
        }

        private void TextBoxMouseLeave(object sender, MouseEventArgs e)
        {
            //stop dragging on mouse leave
            StopDragging();
            e.Handled = true;
        }

        private void TextBoxMouseMove(object sender, MouseEventArgs e)
        {
            //handling mouse move event and setting canvas top and left value based on mouse movement
            if (isDown)
            {
                if ((!isDragging) &&
                    ((Math.Abs(e.GetPosition(AssociatedObject).X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(ParentCanvas).Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    isDragging = true;

                if (isDragging)
                {
                    Point position = Mouse.GetPosition(ParentCanvas);
                    Canvas.SetTop(AssociatedObject, position.Y - (startPoint.Y - originalTop));
                    Canvas.SetLeft(AssociatedObject, position.X - (startPoint.X - originalLeft));
                }
            }
        }

        private void TextBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //stop dragging on mouse left button up
            StopDragging();
            e.Handled = true;
        }

        private void TextBoxMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isSelected)
            {
                isSelected = false;
                if (AssociatedObject != null)
                {
                    adornerLayer.Remove(adornerLayer.GetAdorners(AssociatedObject)[0]);
                }
            }
        }

        private void StopDragging()
        {
            if (isDown)
            {
                isDown = isDragging = false;
            }
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            //we've reached the end of the tree
            if (parentObject == null) return null;
            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
