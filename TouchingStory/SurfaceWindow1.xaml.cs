using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using Newtonsoft.Json;

namespace TouchingStory
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    /// 
    
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public static List<Story> stories;
        public SurfaceWindow1()
        {
            InitializeComponent();
            InitializeDefinitions();
            // load the json file into Story Class when starting the app
            LoadJson();
            
            // Testing: this is a test to display one element of the json file 
            story_text.Content = stories[0].text;
            
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        public void LoadJson()
        {
            // the stories.json is located in \TouchingStory\bin\Debug
            using (StreamReader r = new StreamReader("stories.json"))
            {
                // to use JsonConvert you need to install Nuget Package Manager (google how to do it) and then in this Manager install "Newtonsoft Json" package
                string json = r.ReadToEnd();
                stories = JsonConvert.DeserializeObject<List<Story>>(json);
            }
        }
        
        private void InitializeDefinitions()
        {
            for (byte k = 1; k <= 5; k++)
            {
                TagVisualizationDefinition tagDef =
                    new TagVisualizationDefinition();
                // The tag value that this definition will respond to.
                tagDef.Value = k;
                // The .xaml file for the UI
                tagDef.Source =
                    new Uri("TagKeyword.xaml", UriKind.Relative);
                // The maximum number for this tag value.
                tagDef.MaxCount = 2;
                // The visualization stays for 2 seconds.
                tagDef.LostTagTimeout = 2000.0;
                // Orientation offset (default).
                tagDef.OrientationOffsetFromTag = 0.0;
                // Physical offset (horizontal inches, vertical inches).
                tagDef.PhysicalCenterOffsetFromTag = new Vector(2.0, 2.0);
                // Tag removal behavior (default).
                tagDef.TagRemovedBehavior = TagRemovedBehavior.Fade;
                // Orient UI to tag? (default).
                tagDef.UsesTagOrientation = true;
                // Add the definition to the collection.
                MyTagVisualizer.Definitions.Add(tagDef);
            }
        }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            TagKeyword story = (TagKeyword)e.TagVisualization;
            switch (story.VisualizedTag.Value)
            {
                case 1:
                    
                    story.CameraModel.Content = StringTool.Truncate(stories[0].text, 55);
                    story.myEllipse.Fill = SurfaceColors.Accent1Brush;
                    story.StoryCell.Text = StringTool.Truncate(stories[0].text, 30);
                    break;
                case 2:
                    story.CameraModel.Content = "Fabrikam, Inc. DEF-34";
                    story.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    break;
                case 3:
                    story.CameraModel.Content = "Fabrikam, Inc. GHI-56";
                    story.myEllipse.Fill = SurfaceColors.Accent3Brush;
                    break;
                case 4:
                    story.CameraModel.Content = "Fabrikam, Inc. JKL-78";
                    story.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    break;
                default:
                    story.CameraModel.Content = "UNKNOWN MODEL";
                    story.myEllipse.Fill = SurfaceColors.ControlAccentBrush;
                    break;
            }
        }

  

    }
}