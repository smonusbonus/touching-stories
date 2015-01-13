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
            conLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            conLine.X1 = position.X + 305;
            conLine.X2 = position.X + 355;
            conLine.Y1 = position.Y - 105;
            conLine.Y2 = position.Y - 105;
            conLine.HorizontalAlignment = HorizontalAlignment.Left;
            conLine.VerticalAlignment = VerticalAlignment.Center;
            conLine.StrokeThickness = 2;

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
                            SurfaceWindow1.addTagID_commonStories(story_brief.Name, this.Name);
                        }
                        else
                        {
                            if (SurfaceWindow1.commonStories[story_brief.Name].Contains(this.Name) == false)
                            {
                                SurfaceWindow1.addTagID_commonStories(story_brief.Name, this.Name);
                            }
                            //Dictionary<String, List<String>> aaa = SurfaceWindow1.commonStories;
                        }
                        break;
                    }
                    
                }

                if (story_existed == false)
                {
                    story_brief.Text = story.title;
                    story_brief.Foreground = Brushes.Black;
                    //story_brief.LineHeight = Double.NaN;
                    story_brief.FontSize = 12;
                    story_brief.TextAlignment = TextAlignment.Center;
                    story_brief.TextWrapping = TextWrapping.Wrap;
                    story_brief.VerticalAlignment = VerticalAlignment.Center;
                    story_brief.Width = 80;
                    story_brief.Margin = new Thickness(5, 0, 5, 0);
                    
                    // create new border element as parent for textblock
                    Border story_border = new Border();
                    story_border.Width = 100;
                    story_border.Height = 100;
                    story_border.BorderBrush = Brushes.SteelBlue;
                    story_border.BorderThickness = new Thickness(3, 3, 3, 3);
                    story_border.Background = Brushes.AliceBlue;
                    story_border.CornerRadius = new CornerRadius(180);
                    story_border.Margin = new Thickness(240, -50, 0, 0);
                    story_border.VerticalAlignment = VerticalAlignment.Center;
                    story_border.TouchDown += new EventHandler<TouchEventArgs>(cell_TouchDown);

                    // add story textblock to border element
                    story_border.Child = story_brief;

                    // arrange stories in a circle
                    // circleradius responsive to nr of stories
                    double adjustment = storyids.Length * 0.05;
                    double circleRadius = VisualizedCells.Height * 0.25 * (1 + adjustment);
                    //circleRadius = circleRadius * adj;
                    double radians = rotation * Math.PI / 180.0;
                    double x = circleRadius * Math.Cos(radians);
                    double y = circleRadius * Math.Sin(radians) + VisualizedCells.Height / 2;;
                    TranslateTransform tt = new TranslateTransform(x, y);
                    story_border.RenderTransformOrigin = new Point(0.0, 0.0);
                    story_border.RenderTransform = tt;

                    // Add a Line Element from tagcenter to textbox
                    var myLine = new Line();
                    myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    myLine.X1 = 300;
                    myLine.X2 = x + 240 + 50;
                    myLine.Y1 = 300;
                    myLine.Y2 = y - 50 + 50;
                    myLine.HorizontalAlignment = HorizontalAlignment.Left;
                    myLine.VerticalAlignment = VerticalAlignment.Center;
                    myLine.StrokeThickness = 2;
                    VisualizedCells.Children.Add(myLine);

                    //SurfaceWindow1.homesurface.RegisterName(story_brief.Name, story_brief);

                    VisualizedCells.RegisterName(story_brief.Name, story_brief);
                    VisualizedCells.Children.Add(story_border);

                    rotation += segment;
                }
                else
                {
                    Border border_parent = cell.Parent as Border;
                    border_parent.Background = Brushes.LightYellow;
                    
                }
            }

            // draw connection lines
            DrawConnections();
        }

        private void cell_TouchDown(object sender, TouchEventArgs e)
        {
            
            TextBlock story_brief = new TextBlock();
            TextBlock story_title = new TextBlock();
            Border border_elm = (Border)sender;
            TextBlock textblock = border_elm.Child as TextBlock;

            string id = textblock.Name.TrimStart(new Char[] { 'I', 'D' });
            int story_id = Int32.Parse(id);
            story_brief.Name = textblock.Name + "_window";
            TextBlock story_window = (TextBlock)SurfaceWindow1.homesurface.FindName(story_brief.Name);

            if (story_window == null)
            {
                ScatterView story_scatter_view = new ScatterView();                

                // Create the Grid
                Grid story_window_grid = new Grid();
                story_window_grid.Background = Brushes.WhiteSmoke;
                story_window_grid.HorizontalAlignment = HorizontalAlignment.Left;
                story_window_grid.VerticalAlignment = VerticalAlignment.Top;
                story_window_grid.ShowGridLines = false;

                // Define the Columns
                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                colDef1.Width = new GridLength(5, GridUnitType.Star);
                colDef2.Width = new GridLength(2, GridUnitType.Star);
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
                SurfaceWindow1.homesurface.RegisterName(story_brief.Name, story_brief);                
                story_brief.Padding = new Thickness(20, 5, 30, 10);
                story_brief.LineHeight = Double.NaN;
                story_brief.FontSize = 11;
                story_brief.FontStretch = FontStretches.Normal;
                story_brief.TextAlignment = TextAlignment.Left;
                story_brief.TextWrapping = TextWrapping.Wrap;
                story_brief.ClipToBounds = true;
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
                item.Height = 350;
                story_scatter_view.Items.Add(item);

                Canvas.SetZIndex(story_scatter_view, (int)70);
               
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
            // Delete all lines within the grid
            foreach (Line ln in FindVisualChildren<Line>(SurfaceWindow1.mainGridView))
            {
                SurfaceWindow1.mainGridView.Children.Remove(ln);
            }

            // To do: Get list with storyids for which lines to draw
            var connection_list = SurfaceWindow1.commonStories.Keys;

            // Loop over all connections in list
            foreach (var storyid in connection_list)
            {
                // get tags to which to draw connection lines
                var commontags = SurfaceWindow1.commonStories[storyid];

                if (commontags.Count > 1)
                {
                    // Get tagvisualization tag that contains textblock
                    TagKeyword TagVisOriginal = new TagKeyword();
                    for (int i = 0; i < commontags.Count; i++)
                    { 
                        TagVisOriginal = (TagKeyword)SurfaceWindow1.mainGridView.FindName(commontags[0]);
                        if (TagVisOriginal != null)
                            break;
                    }
                    // Get position textblock
                    TextBlock textblock = (TextBlock)TagVisOriginal.VisualizedCells.FindName(storyid);
                    //Point positionBlock = textblock.PointToScreen(new Point(0d, 0d));
                    //var transform = SurfaceWindow1.mainGridView.TransformToVisual(textblock);
                    //Point positionBlock = transform.Transform(new Point(0, 0));
                    Window rootvisual = Window.GetWindow(textblock);

                    Point positionBlock = textblock.TransformToAncestor(rootvisual).Transform(new Point(0, 0));

                    // counter because you do not want to draw line to the first tag (this one contains the textblock)
                    
                    foreach (var tag in commontags)
                    {
                        // Not for the first tag because this contains the textblock
                        if (tag != TagVisOriginal.Name)
                        {
                            // Get position tag
                            TagKeyword TagVis = (TagKeyword)SurfaceWindow1.mainGridView.FindName(tag);
                            TagData aaa = TagVis.VisualizedTag;
                            Point a = new Point(TagVis.Center.X, TagVis.Center.Y);
                            //Point positionTag = .PointToScreen(new Point(0d, 0d));

                            // Draw a line from storytextblock to tagcenter
                            var conLine = new Line();
                            conLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                            conLine.X1 = a.X;
                            conLine.X2 = positionBlock.X;
                            conLine.Y1 = a.Y;
                            conLine.Y2 = positionBlock.Y;
                            conLine.StrokeThickness = 2;
                            SurfaceWindow1.mainGridView.Children.Add(conLine);
                        }
          
                    }//foreach
                }//if
            }//foreach
        }//drawconnections()

        /*public void TagKeyword_Loaded_Again()
        {

            //TODO: customize TagKeyword's UI based on this.VisualizedTag here

            int[] storyids = new int[14];
            switch (this.VisualizedTag.Value)
            {
                case 0:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "heks", "character");
                    break;
                case 1:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "duivel", "character");
                    break;
                case 2:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "boer", "character");
                    break;
                case 3:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "soldaat", "character");
                    break;
                case 4:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "kind", "character");
                    break;
                case 5:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "spook", "character");
                    break;
                case 6:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "dorp", "location");
                    break;
                case 7:
                    storyids =SurfaceWindow1.FilterJsonBy(story_list, "kerk", "location");
                    break;
                case 8:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "kasteel", "location");
                    break;
                case 9:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "bos", "location");
                    break;
                case 10:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "boerderij", "location");
                    break;
                case 11:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "schip", "location");
                    break;
                case 12:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "herberg", "location");
                    break;
                case 13:
                    storyids = SurfaceWindow1.FilterJsonBy(story_list, "kerkhof", "location");
                    break;
                default:
                    break;
            }
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
                int count = 0;
                TextBlock cell = null;
                if (SurfaceWindow1.commonStories.ContainsKey(story_brief.Name) == false || (SurfaceWindow1.commonStories.ContainsKey(story_brief.Name) == true && SurfaceWindow1.commonStories[story_brief.Name].Count < 2))
                {
                    story_brief.Text = story.title;
                    //story_brief.Background = Brushes.LightBlue;
                    story_brief.Foreground = Brushes.Black;
                    //story_brief.LineHeight = Double.NaN;
                    story_brief.FontSize = 12;
                    story_brief.TextAlignment = TextAlignment.Center;
                    story_brief.TextWrapping = TextWrapping.Wrap;
                    story_brief.VerticalAlignment = VerticalAlignment.Center;
                    story_brief.Width = 80;
                    story_brief.Margin = new Thickness(5, 0, 5, 0);

                    // create new border element as parent for textblock
                    Border story_border = new Border();
                    story_border.Width = 100;
                    story_border.Height = 100;
                    story_border.BorderBrush = Brushes.SlateBlue;
                    story_border.BorderThickness = new Thickness(5, 5, 5, 5);
                    story_border.Background = Brushes.AliceBlue;
                    story_border.CornerRadius = new CornerRadius(180);
                    story_border.Margin = new Thickness(240, -50, 0, 0);
                    story_border.VerticalAlignment = VerticalAlignment.Center;
                    story_border.TouchDown += new EventHandler<TouchEventArgs>(cell_TouchDown);

                    // add story textblock to border element
                    story_border.Child = story_brief;

                    // arrange stories in a circle
                    // circleradius responsive to nr of stories
                    double adjustment = storyids.Length * 0.05;
                    double circleRadius = VisualizedCells.Height * 0.25 * (1 + adjustment);
                    //circleRadius = circleRadius * adj;
                    double radians = rotation * Math.PI / 180.0;
                    double x1 = circleRadius * Math.Cos(radians);
                    double y1 = circleRadius * Math.Sin(radians) + VisualizedCells.Height / 2; ;
                    TranslateTransform tt = new TranslateTransform(x1, y1);
                    story_border.RenderTransformOrigin = new Point(0.0, 0.0);
                    story_border.RenderTransform = tt;

                    // Add a Line Element from tagcenter to textbox
                    var myLine = new Line();
                    myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    myLine.X1 = 300;
                    myLine.X2 = x1 + 240 + 50;
                    myLine.Y1 = 300;
                    myLine.Y2 = y1 - 50 + 50;
                    myLine.HorizontalAlignment = HorizontalAlignment.Left;
                    myLine.VerticalAlignment = VerticalAlignment.Center;
                    myLine.StrokeThickness = 2;
                    VisualizedCells.Children.Add(myLine);

                    //SurfaceWindow1.homesurface.RegisterName(story_brief.Name, story_brief);
                    if ((TextBlock)VisualizedCells.FindName(story_brief.Name) != null)
                        VisualizedCells.UnregisterName(story_brief.Name);
                    VisualizedCells.RegisterName(story_brief.Name, story_brief);
                    VisualizedCells.Children.Add(story_border);

                    rotation += segment;
                }
            }

        }*/
    }
}
