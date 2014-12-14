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
 
        private void TagKeyword_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: customize TagKeyword's UI based on this.VisualizedTag here

            int[] storyids = TouchingStory.SurfaceWindow1.listOfIds;
            List<Story> story_list = TouchingStory.SurfaceWindow1.stories;
            for (byte k = 0; k < storyids.Length; k++)   
            {               
                TextBlock story_brief = new TextBlock();                
                Story story = story_list.Find(x => x.id == storyids[k]);
                story_brief.Name = "ID" + story.id.ToString();
                story_brief.Text = StringTool.Truncate(story.text, 60);
                story_brief.Background = Brushes.LightBlue;
                story_brief.Foreground = Brushes.Black;
                story_brief.Padding = new Thickness(5, 10, 5, 10);
                story_brief.LineHeight = Double.NaN;
                story_brief.FontSize = 12;
                story_brief.FontStretch = FontStretches.UltraExpanded;
                story_brief.TextAlignment = TextAlignment.Left;
                story_brief.TextWrapping = TextWrapping.Wrap;
                story_brief.TouchDown += new EventHandler<TouchEventArgs>(cell_TouchDown);
                VisualizedCells.Items.Add(story_brief);
                
            }
        }

        private void cell_TouchDown(object sender, TouchEventArgs e)
        {
            TextBlock story_brief = new TextBlock();
            TextBlock textblock = (TextBlock)sender;
            story_brief.Name = textblock.Name + "_window";
            TextBlock test = (TextBlock)VisualizedCells.FindName(story_brief.Name);
            if (VisualizedCells.FindName(story_brief.Name) == null)
            {
                story_brief.Text = textblock.Text;
                story_brief.Background = Brushes.Gray;
                story_brief.Foreground = Brushes.Black;
                story_brief.Padding = new Thickness(5, 10, 5, 10);
                story_brief.LineHeight = Double.NaN;
                story_brief.FontSize = 12;
                story_brief.FontStretch = FontStretches.UltraExpanded;
                story_brief.TextAlignment = TextAlignment.Left;
                story_brief.TextWrapping = TextWrapping.Wrap;
                VisualizedCells.Items.Add(story_brief);
            }
        }



    }
}
