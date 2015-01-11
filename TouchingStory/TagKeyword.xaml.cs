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
        public static int test = 0;

        public List<Story> story_list;
        public TagKeyword()
        {
            InitializeComponent();
            
        }


        // helper function to get all children of an element (used for finding all lines)
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    // Use this if you also want children of children
                    //foreach (T childOfChild in FindVisualChildren<T>(child))
                    //{
                    //    yield return childOfChild;
                    //}
                }
            }
        }


        private void OnPreviewVisualizationMoved(object sender, TagVisualizerEventArgs e)
        {
            //draw a connection line from tagcenter to other tagcenter
            var conLine = new Line();
            GeneralTransform gt = VisualizedCells.TransformToVisual(SurfaceWindow1.mainGridView);
            Point position = gt.Transform(new Point(0d, 0d));
            //Point position = VisualizedCells.PointToScreen(new Point(0d, 0d));
            conLine.Stroke = System.Windows.Media.Brushes.Red;
            conLine.X1 = position.X + 305;
            conLine.X2 = position.X + 355;
            conLine.Y1 = position.Y - 105;
            conLine.Y2 = position.Y - 105;
            conLine.HorizontalAlignment = HorizontalAlignment.Left;
            conLine.VerticalAlignment = VerticalAlignment.Center;
            conLine.StrokeThickness = 10;

            SurfaceWindow1.mainGridView.Children.Add(conLine);  
        }



        private void TagKeyword_Loaded(object sender, RoutedEventArgs e)
        {

            //TODO: customize TagKeyword's UI based on this.VisualizedTag here

            int[] storyids = TouchingStory.SurfaceWindow1.listOfIds;
            story_list = TouchingStory.SurfaceWindow1.stories;
            
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
                        if (SurfaceWindow1.commonStories.ContainsKey(story_brief.Name) == false)
                        {
                            SurfaceWindow1.addTagID_commonStories(story_brief.Name, tk.Name);
                        }
                        SurfaceWindow1.addTagID_commonStories(story_brief.Name, this.Name);
                        //Dictionary<String, List<String>> aaa = SurfaceWindow1.commonStories;
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

                    // Add a Line Element from tagcenter to textbox
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

            // draw connection lines
            DrawConnections();
        }

        private void cell_TouchDown(object sender, TouchEventArgs e)
        {
            
            TextBlock story_brief = new TextBlock();
            TextBlock story_title = new TextBlock();
            TextBlock textblock = (TextBlock)sender;
            string id = textblock.Name.TrimStart(new Char[] { 'I', 'D' });
            int story_id = Int32.Parse(id);
            story_brief.Name = textblock.Name + "_window";
            TextBlock story_window = (TextBlock)SurfaceWindow1.homesurface.FindName(story_brief.Name);

            if (story_window == null)
            {
                ScatterView story_scatter_view = new ScatterView();                

                // Create the Grid
                Grid story_window_grid = new Grid();
                //story_window_grid.Width = Double.NaN;
                //story_window_grid.Height = 100;
                story_window_grid.Background = Brushes.WhiteSmoke;
                story_window_grid.HorizontalAlignment = HorizontalAlignment.Left;
                story_window_grid.VerticalAlignment = VerticalAlignment.Top;
                story_window_grid.ShowGridLines = true;

                // Define the Columns
                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                colDef1.Width = new GridLength(5, GridUnitType.Star);
                colDef2.Width = new GridLength(2, GridUnitType.Star);
                //colDef2.MinWidth = 50;
                story_window_grid.ColumnDefinitions.Add(colDef1);
                story_window_grid.ColumnDefinitions.Add(colDef2);

                // Define the Rows
                RowDefinition rowDef1 = new RowDefinition();
                RowDefinition rowDef2 = new RowDefinition();
                rowDef1.Height = new GridLength(1, GridUnitType.Star);
                rowDef1.MinHeight = 40;
                rowDef2.Height = new GridLength(9, GridUnitType.Star);
                story_window_grid.RowDefinitions.Add(rowDef1);
                story_window_grid.RowDefinitions.Add(rowDef2);

                // Define the closing Button
                Button closingButton = new Button();
                closingButton.TouchDown += new EventHandler<TouchEventArgs>(closeStoryWindow);                
                closingButton.Content = "X";
                closingButton.FontSize = 20;
                closingButton.Width = 30;
                closingButton.Height = 30;
                closingButton.Margin = new Thickness(0, 5, 5, 0);
                closingButton.Padding = new Thickness(5, 0, 5, 0);
                closingButton.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(closingButton, 0);
                Grid.SetColumn(closingButton, 1);

                // Get story based on id
                Story story = story_list.Find(x => x.id == story_id);

                // create title text block
                story_title.Text = story.title;
                story_title.FontSize = 14;
                story_title.FontWeight = FontWeights.Bold;
                story_title.Padding = new Thickness(20, 20, 10, 0);
                Grid.SetRow(story_title, 0);
                Grid.SetColumn(story_title, 0);

                // create story text block
                story_brief.Text = story.text;
                story_brief.Foreground = Brushes.Black;
                //story_brief.MinWidth = 300;
                SurfaceWindow1.homesurface.RegisterName(story_brief.Name, story_brief);                
                story_brief.Padding = new Thickness(20, 5, 30, 10);
                story_brief.LineHeight = Double.NaN;
                story_brief.FontSize = 11;
                story_brief.FontStretch = FontStretches.Normal;
                story_brief.TextAlignment = TextAlignment.Left;
                story_brief.TextWrapping = TextWrapping.Wrap;
                Grid.SetRow(story_brief, 1);
                Grid.SetColumn(story_brief, 0);

                // meta-data text elements
                TextBlock md_text_concept = new TextBlock();
                md_text_concept.Text = "Concept: " + story.concept;
                TextBlock md_text_subgenre = new TextBlock();
                md_text_subgenre.Text = "Subgenre: " + story.subgenre;
                TextBlock md_text_description = new TextBlock();
                md_text_description.Text = "Description: A " + story.subgenre + " is a bla bla bla bla bla";

                // meta-data stack panel
                StackPanel md_panel = new StackPanel();
                md_panel.Children.Add(md_text_concept);
                md_panel.Children.Add(md_text_subgenre);
                md_panel.Children.Add(md_text_description);
                Grid.SetRow(md_panel, 1);
                Grid.SetColumn(md_panel, 1);

                // Add the elements to the Grid Children collection
                story_window_grid.Children.Add(story_title);
                story_window_grid.Children.Add(closingButton);
                story_window_grid.Children.Add(story_brief);
                story_window_grid.Children.Add(md_panel);

                ScatterViewItem item = new ScatterViewItem();
                item.CanScale = true;
                
                item.Style = (Style)Application.Current.FindResource("ScatterViewItemStyle");
                item.Content = story_window_grid;
                item.Width = 500;
                item.Height = 300;
                story_scatter_view.Items.Add(item);
                //TagKeyword test = (TagKeyword)this.Visualizer.ActiveVisualizations[0];
               
                SurfaceWindow1.mainGridView.Children.Add(story_scatter_view);
               
            }
            else
            {
                Grid grid1 = (Grid)story_window.Parent;
                ScatterViewItem svi = (ScatterViewItem)grid1.Parent;
                ScatterView sv = (ScatterView)svi.Parent;
                sv.Opacity = 1;
            }
        }

        private void closeStoryWindow(object sender, TouchEventArgs e)
        {
            
            Button test_sender = (Button)sender;
            Grid grid1 = (Grid)test_sender.Parent;
            ScatterViewItem svi = (ScatterViewItem)grid1.Parent;
            ScatterView sv = (ScatterView)svi.Parent;
            sv.Opacity = 0;
            //SurfaceWindow1.mainGridView.Children.Remove(sv); 
        }

        public static void DrawConnections()
        {

            var list = SurfaceWindow1.commonStories;

            // To do: Get list with lines to draw

            // Delete all lines within the grid
            foreach (Line ln in FindVisualChildren<Line>(SurfaceWindow1.mainGridView))
            {
                SurfaceWindow1.mainGridView.Children.Remove(ln);
            }

            // code for debugging
            test = test + 1;
            if (test == 2)
            {

                // To do: Draw the new lines
                var list1 = SurfaceWindow1.commonStories;

                // To do: Get position textblock with certain id from a certain tag

                TagKeyword TagVis9 = (TagKeyword)SurfaceWindow1.mainGridView.FindName("tag_9");

                TagKeyword TagVis0 = (TagKeyword)SurfaceWindow1.mainGridView.FindName("tag_0");

                TextBlock textblock = (TextBlock)TagVis0.VisualizedCells.FindName("ID42714");

                //var TextboxWithStory = (TextBox)TagVisualization.FindName("Field_CompanyName");

                // Testline draw line from tag 0 to story with id .. in tag 2
                var conLine = new Line();

                //GeneralTransform gt = VisualizedCells.TransformToVisual(SurfaceWindow1.mainGridView);
                //Point position = gt.Transform(new Point(0d, 0d));

                Point positionTag = TagVis9.VisualizedCells.PointToScreen(new Point(0d, 0d));

                Point positionBlock = textblock.PointToScreen(new Point(0d, 0d));

                conLine.Stroke = System.Windows.Media.Brushes.Red;
                conLine.X1 = positionTag.X + 300;
                conLine.X2 = positionBlock.X;
                conLine.Y1 = positionTag.Y + 190;
                conLine.Y2 = positionBlock.Y - 40;
                conLine.HorizontalAlignment = HorizontalAlignment.Left;
                conLine.VerticalAlignment = VerticalAlignment.Center;
                conLine.StrokeThickness = 10;

                SurfaceWindow1.mainGridView.Children.Add(conLine);
            }
        }

    }
}
