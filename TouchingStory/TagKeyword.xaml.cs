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
        int[] listofid;
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
                story_brief.Text = StringTool.Truncate(story.text, 40); ;
                VisualizedCells.Items.Add(story_brief);
            }
        }

    }
}
