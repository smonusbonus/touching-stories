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
        List<Story> storiesList;
        public TagKeyword()
        {
            InitializeComponent();
            
        }

 
        private void TagKeyword_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: customize TagKeyword's UI based on this.VisualizedTag here

            // I need the subset of stories for display, i can get this with the value of the object
           
            storiesList = SurfaceWindow1.stories;
            for (byte k = 1; k < this.storiesList.Count + 1; k++)   
            {               
                TextBlock story_brief = new TextBlock();
                story_brief.Text = storiesList[k].text;
                VisualizedCells.Items.Add(story_brief);
            }
        }

    }
}
