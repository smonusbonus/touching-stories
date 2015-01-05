﻿using System;
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
                    story_brief.Text = StringTool.Truncate(story.text, 60);
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
            TextBlock test = (TextBlock) VisualizedCells.FindName(story_brief.Name);

            if (SurfaceWindow1.homesurface.FindName(story_brief.Name) == null)
            {
                
                story_brief.Text = textblock.Text;
                story_brief.Background = Brushes.WhiteSmoke;
                story_brief.Foreground = Brushes.Black;
                story_brief.Width = 500;
                SurfaceWindow1.homesurface.RegisterName(story_brief.Name, story_brief);
                story_brief.Foreground = Brushes.Black;
                story_brief.Padding = new Thickness(5, 10, 5, 10);
                story_brief.LineHeight = Double.NaN;
                story_brief.FontSize = 12;
                story_brief.FontStretch = FontStretches.UltraExpanded;
                story_brief.TextAlignment = TextAlignment.Left;
                story_brief.TextWrapping = TextWrapping.Wrap;
                VisualizedCells.Children.Add(story_brief);


            }
        }

    }
}
