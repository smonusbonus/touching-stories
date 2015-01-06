using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;


namespace TouchingStory
{
    /// <summary>
    /// Interaction logic for TagKeyword.xaml
    /// </summary>
    public partial class TagKeyword : TagVisualization
    {
        public TagKeyword()
        {
            InitializeComponent();
            
        }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {

     
        }

        private void TagKeyword_Loaded(object sender, RoutedEventArgs e)
        {

            //TODO: customize TagKeyword's UI based on this.VisualizedTag here

            int[] storyids = TouchingStory.SurfaceWindow1.listOfIds;
            List<Story> story_list = TouchingStory.SurfaceWindow1.stories;

            double segment = 360.0 / storyids.Length;
            double rotation = 0.0;
            string value = this.VisualizedTag.Value.ToString();
            
            for (byte k = 0; k < storyids.Length; k++)   
            {
                //MyTagVisualization parent = (TagVisualization)VisualizedCells.Parent;
                TextBlock story_brief = new TextBlock();                
                Story story = story_list.Find(x => x.id == storyids[k]);
                story_brief.Name = "ID" + story.id.ToString();
                Boolean story_existed = false;
                int count = this.Visualizer.ActiveVisualizations.Count;
                TextBlock cell = null;
                for (int i = 0; i < count; i++)
                {
                    TagKeyword tk = (TagKeyword)this.Visualizer.ActiveVisualizations[i];
                    cell = (TextBlock)tk.FindName(story_brief.Name);
                    if (cell != null)
                    {
                        story_existed = true;
                        break;
                    }
                    
                }
                if (story_existed == false)
                {
                    story_brief.Text = story.title;
                    story_brief.Background = Brushes.LightBlue;
                    story_brief.Foreground = Brushes.Black;
                    story_brief.Padding = new Thickness(5, 10, 5, 10);
                    story_brief.LineHeight = Double.NaN;
                    story_brief.FontSize = 12;
                    story_brief.FontStretch = FontStretches.UltraExpanded;
                    story_brief.TextAlignment = TextAlignment.Left;
                    story_brief.TextWrapping = TextWrapping.Wrap;
                    story_brief.Width = 100;
                    story_brief.Height = 100;
                    story_brief.TouchDown += new EventHandler<TouchEventArgs>(cell_TouchDown);
                    // can be used to position the elements properly, although this is probably some kind of hack
                    story_brief.Margin = new Thickness(240, -50, 0, 0);
                    //Rotate the Surface Button so that the highlight on the Glass Button is coming from that same place on all of them.
                    double circleRadius = VisualizedCells.Height / 2;
                    double radians = rotation * Math.PI / 180.0;
                    double x = circleRadius * Math.Cos(radians);
                    double y = circleRadius * Math.Sin(radians) + VisualizedCells.Height / 2;
                    TranslateTransform tt = new TranslateTransform(x, y);
                    story_brief.RenderTransformOrigin = new Point(0.0, 0.0);
                    story_brief.RenderTransform = tt;


                    //draw a line from element to tagcenter

                    // Add a Line Element
                    var myLine = new Line();
                    myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    myLine.X1 = 300;
                    myLine.X2 = x + 240 + 50;
                    myLine.Y1 = 300;
                    myLine.Y2 = y - 50 + 50;
                    myLine.HorizontalAlignment = HorizontalAlignment.Left;
                    myLine.VerticalAlignment = VerticalAlignment.Center;
                    myLine.StrokeThickness = 10;
                    VisualizedCells.Children.Add(myLine);

                    //SurfaceWindow1.homesurface.RegisterName(story_brief.Name, story_brief);

                    VisualizedCells.RegisterName(story_brief.Name, story_brief);
                    VisualizedCells.Children.Add(story_brief);

                    rotation += segment;
                }
                else
                {
                    cell.Background = Brushes.LightYellow;
                }
            }
        }

        private void cell_TouchDown(object sender, TouchEventArgs e)
        {
            
            TextBlock story_brief = new TextBlock();
            TextBlock textblock = (TextBlock)sender;            
            story_brief.Name = textblock.Name + "_window";
            TextBlock story_window = (TextBlock)SurfaceWindow1.homesurface.FindName(story_brief.Name);

            if (story_window == null)
            {
                ScatterView story_scatter_view = new ScatterView();                
                StackPanel stackpanel = new StackPanel();
                stackpanel.Width = 200;
                stackpanel.Background = Brushes.WhiteSmoke;
                Button closingButton = new Button();
                closingButton.TouchDown += new EventHandler<TouchEventArgs>(closeStoryWindow);                
                closingButton.Content = "X";
                closingButton.FontSize = 20;
                closingButton.Margin = new Thickness(0, 5, 5, 0);
                closingButton.Padding = new Thickness(5, 0, 5, 0);
                closingButton.HorizontalAlignment = HorizontalAlignment.Right;
                story_brief.Text = textblock.Text;
                story_brief.Background = Brushes.WhiteSmoke;
                story_brief.Foreground = Brushes.Black;
                //story_brief.Width = 200;
                SurfaceWindow1.homesurface.RegisterName(story_brief.Name, story_brief);
                story_brief.Foreground = Brushes.Black;
                story_brief.Padding = new Thickness(5, 10, 5, 10);
                story_brief.LineHeight = Double.NaN;
                story_brief.FontSize = 12;
                story_brief.FontStretch = FontStretches.UltraExpanded;
                story_brief.TextAlignment = TextAlignment.Left;
                story_brief.TextWrapping = TextWrapping.Wrap;
                
                stackpanel.Children.Add(closingButton);
                stackpanel.Children.Add(story_brief);
                story_scatter_view.Items.Add(stackpanel);
                SurfaceWindow1.mainGridView.Children.Add(story_scatter_view);
            }
            else
            {
                StackPanel sp = (StackPanel)story_window.Parent;
                ScatterView sv = (ScatterView)sp.Parent;
                sv.Opacity = 1;
            }
        }

        private void closeStoryWindow(object sender, TouchEventArgs e)
        {
            
            Button test_sender = (Button)sender;
            StackPanel sp = (StackPanel)test_sender.Parent;
            ScatterView sv = (ScatterView)sp.Parent;
            sv.Opacity = 0;
            //SurfaceWindow1.mainGridView.Children.Remove(sv);
            int a = 0;
        }

    }
}
