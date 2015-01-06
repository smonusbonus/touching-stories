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
        public static int[] listOfIds; 
        public static TagVisualizer ms;
        public static SurfaceWindow homesurface;
        public static ScatterView mainSV;
        
        public SurfaceWindow1()
        {
            InitializeComponent();
            InitializeDefinitions();

            // load the json file into Story Class when starting the app
            //List<Story> story_list = LoadJson();

            // filter json based on keyword
            //listOfIds = FilterJson(story_list, "dood");
            
            
            // print ids of corresponding keyword
            /*foreach(int id in listOfIds) {
                System.Diagnostics.Debug.WriteLine(id);
            }*/
            ms = MyTagVisualizer;
            homesurface = HomeSurface;
            mainSV = MainScatterView;
            
            // Testing: this is a test to display one element of the json file 
            
            
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            Existed("aa");
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

        public List<Story> LoadJson()
        {
            // the stories.json is located in \TouchingStory\bin\Debug
            using (StreamReader r = new StreamReader("stories.json"))
            {
                // to use JsonConvert you need to install Nuget Package Manager (google how to do it) and then in this Manager install "Newtonsoft Json" package
                string json = r.ReadToEnd();
                stories = JsonConvert.DeserializeObject<List<Story>>(json);
                return stories;
            }
        }

        // this function filters the json data based on a tag-name
        public int[] FilterJson(List<Story> stories, string matchTag) 
        {
            //int[] idList;
            List<int> idList = new List<int>();

            for(byte i = 0; i < stories.Count; i++) 
            {
                //System.Diagnostics.Debug.WriteLine("i: " + i + " id: " + stories[i].id + " subgenre: " + stories[i].subgenre);

                foreach(string tag in stories[i].tags) 
                {
                    if(tag == matchTag)
                    {
                        System.Diagnostics.Debug.WriteLine("matching tag " + tag);
                        idList.Add(stories[i].id);
                    }
                }
            }

            return idList.ToArray();
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
                tagDef.PhysicalCenterOffsetFromTag = new Vector(0.0, 0.0);
                // Tag removal behavior (default).
                tagDef.TagRemovedBehavior = TagRemovedBehavior.Fade;
                // Orient UI to tag? (default).
                tagDef.UsesTagOrientation = true;
                // Add the definition to the collection.
                MyTagVisualizer.Definitions.Add(tagDef);
            }
            TagVisualizationDefinition tagDef0 =
                    new TagVisualizationDefinition();
            // The tag value that this definition will respond to.
            tagDef0.Value = 0xC1;
            // The .xaml file for the UI
            tagDef0.Source =
                new Uri("TagKeyword.xaml", UriKind.Relative);
            // The maximum number for this tag value.
            tagDef0.MaxCount = 2;
            // The visualization stays for 2 seconds.
            tagDef0.LostTagTimeout = 2000.0;
            // Orientation offset (default).
            tagDef0.OrientationOffsetFromTag = 0.0;
            // Physical offset (horizontal inches, vertical inches).
            tagDef0.PhysicalCenterOffsetFromTag = new Vector(0.0, 0.0);
            // Tag removal behavior (default).
            tagDef0.TagRemovedBehavior = TagRemovedBehavior.Fade;
            // Orient UI to tag? (default).
            tagDef0.UsesTagOrientation = true;
            // Add the definition to the collection.
            MyTagVisualizer.Definitions.Add(tagDef0);

            TagVisualizationDefinition tagDef1 =
                    new TagVisualizationDefinition();
            // The tag value that this definition will respond to.
            tagDef1.Value = 0xC0;
            // The .xaml file for the UI
            tagDef1.Source =
                new Uri("TagKeyword.xaml", UriKind.Relative);
            // The maximum number for this tag value.
            tagDef1.MaxCount = 2;
            // The visualization stays for 2 seconds.
            tagDef1.LostTagTimeout = 2000.0;
            // Orientation offset (default).
            tagDef1.OrientationOffsetFromTag = 0.0;
            // Physical offset (horizontal inches, vertical inches).
            tagDef1.PhysicalCenterOffsetFromTag = new Vector(0.0, 0.0);
            // Tag removal behavior (default).
            tagDef1.TagRemovedBehavior = TagRemovedBehavior.Fade;
            // Orient UI to tag? (default).
            tagDef1.UsesTagOrientation = true;
            // Add the definition to the collection.
            MyTagVisualizer.Definitions.Add(tagDef1);
        }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            List<Story> story_list = LoadJson();
            TagKeyword story = (TagKeyword)e.TagVisualization;
            switch (story.VisualizedTag.Value)
            {
                case 0xC0:
                    listOfIds = FilterJson(story_list, "dood");
                    break;
                case 0xC1:
                    listOfIds = FilterJson(story_list, "hekserij");
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    break;
            }
        } 
        
        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            TagVisualizer tagvisualizer = (TagVisualizer)e.Source;
            
            TagKeyword tagkeyword = (TagKeyword)e.TagVisualization;
            
        }

        public Boolean Existed(string story_name)
        {
            ms = ms;
            homesurface = homesurface;
            int a = 1;
            return true;
        }

    }
}