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
        /// 

        public static List<Story> stories;
        public static int[] listOfIds; 
        public static TagVisualizer ms;
        public static SurfaceWindow homesurface;
        public static Grid mainGridView;
        //public static Canvas mainGridView;
        public static Dictionary<String, List<String>> commonStories;

        public SurfaceWindow1()
        {
            InitializeComponent();
            InitializeDefinitions();
            
            ms = MyTagVisualizer;
            homesurface = HomeSurface;
            mainGridView = MainGridView;

            commonStories = new Dictionary<String,List<String>>();
   
            // Testing: this is a test to display one element of the json file 
            
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

        public List<Story> LoadJson()
        {
            // the stories.json is located in \TouchingStory\bin\Debug
            //using (StreamReader r = new StreamReader("stories.json"))
            using (StreamReader r = new StreamReader("stories_with_titles.json"))
            {
                // to use JsonConvert you need to install Nuget Package Manager (google how to do it) and then in this Manager install "Newtonsoft Json" package
                string json = r.ReadToEnd();
                stories = JsonConvert.DeserializeObject<List<Story>>(json);
                return stories;
            }
        }


        // this function filters the json data based on a tag-name and a filterBy parameter
        // which either can be "location" or "character"
        public static int[] FilterJsonBy(List<Story> stories, string matchTag, string filterBy)
        {
            List<int> listOfStoryIds = new List<int>();

            for (byte i = 0; i < stories.Count; i++)
            {
                //System.Diagnostics.Debug.WriteLine(stories[i].id);
                // make sure there's not more than 10 stories per tag, otherwise it gets too crowded on the screen
                if (listOfStoryIds.Count() > 10)
                {
                    break;
                }
                else
                {
                    if (filterBy == "character" && stories[i].character == matchTag)
                    {
                        listOfStoryIds.Add(stories[i].id);
                    }
                    if (filterBy == "location" && stories[i].location == matchTag)
                    {
                        listOfStoryIds.Add(stories[i].id);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine(listOfStoryIds);
            return listOfStoryIds.ToArray();
        }

        private void InitNewTag(int value) {
            TagVisualizationDefinition tagDef = new TagVisualizationDefinition();
            // The tag value that this definition will respond to.
            tagDef.Value = value;
            // The .xaml file for the UI
            tagDef.Source = new Uri("TagKeyword.xaml", UriKind.Relative);
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
        
        // here the tagged object definitions are executed
        private void InitializeDefinitions()
        {
            for (int i = 0; i < 14; i++)
            {
                InitNewTag(i);
            }
        }

        // once a tagged object is used this function executes 
        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            // Find value object and convert to string to use as name
            if (StartingMessage.IsVisible == true)
            {
                StartingMessage.Visibility = Visibility.Hidden;
            }

            TagVisualizer tagvisualizer = (TagVisualizer)e.Source;

            
            Canvas.SetZIndex(tagvisualizer, (int)50);
            
            var PlacedTag = e.TagVisualization.VisualizedTag;
            var ObjectName = "tag_" + PlacedTag.Value.ToString();
            // Register name of object for later use with registername(name, object)

            TagKeyword story = (TagKeyword)e.TagVisualization;

            story.Name = ObjectName;
            if ((TagKeyword)mainGridView.FindName(ObjectName) != null) 
            {
                mainGridView.UnregisterName(ObjectName);
            }
            mainGridView.RegisterName(ObjectName, story);
            List<Story> story_list = LoadJson();

            //System.Diagnostics.Debug.WriteLine(Canvas.GetZIndex(story));
            
            switch (story.VisualizedTag.Value)
            {
                case 0:
                    listOfIds = FilterJsonBy(story_list, "heks", "character");
                    break;
                case 1:
                    listOfIds = FilterJsonBy(story_list, "duivel", "character");
                    break;
                case 2:
                    listOfIds = FilterJsonBy(story_list, "boer", "character");
                    break;
                case 3:
                    listOfIds = FilterJsonBy(story_list, "soldaat", "character");
                    break;
                case 4:
                    listOfIds = FilterJsonBy(story_list, "kind", "character");
                    break;
                case 5:
                    listOfIds = FilterJsonBy(story_list, "spook", "character");
                    break;
                case 6:
                    listOfIds = FilterJsonBy(story_list, "dorp", "location");
                    break;
                case 7:
                    listOfIds = FilterJsonBy(story_list, "kerk", "location");
                    break;
                case 8:
                    listOfIds = FilterJsonBy(story_list, "kasteel", "location");
                    break;
                case 9:
                    listOfIds = FilterJsonBy(story_list, "bos", "location");
                    break;
                case 10:
                    listOfIds = FilterJsonBy(story_list, "boerderij", "location");
                    break;
                case 11:
                    listOfIds = FilterJsonBy(story_list, "schip", "location");
                    break;
                case 12:
                    listOfIds = FilterJsonBy(story_list, "herberg", "location");
                    break;
                case 13:
                    listOfIds = FilterJsonBy(story_list, "kerkhof", "location");
                    break;
                default:
                    break;
            }
        } 
        
        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            TagVisualizer tagvisualizer = (TagVisualizer)e.Source;            
            TagKeyword tagkeyword = (TagKeyword)e.TagVisualization;
            List<String> keys = new List<String>();
            foreach (string value in commonStories.Keys)
            {
                keys.Add(value);
             
            }
            for (int i =0; i<keys.Count; i++)
            {
                commonStories[keys[i]].Remove(tagkeyword.Name);
                if (commonStories[keys[i]].Count == 1)
                {
                    TagKeyword tagkeyword2 = (TagKeyword)mainGridView.FindName(commonStories[keys[i]][0]);
                    tagkeyword2.TagKeyword_Loaded_Again();
                }
                
            }
            TagKeyword.DrawConnections();      
        }

        private void OnVisualizationMoved(object sender, TagVisualizerEventArgs e)
        {
            TagKeyword tagkeyword = (TagKeyword)e.TagVisualization;
            TagKeyword.DrawConnections();

        }

        public static void addTagID_commonStories(string cellID, string TagID)
        {

            if (commonStories.ContainsKey(cellID) == false)
            {
                List<String> tagIDs = new List<String> { TagID };
                commonStories.Add(cellID, tagIDs);
            }
            else
            {
                if (commonStories[cellID].Contains(TagID) == false)
                  commonStories[cellID].Add(TagID);
            }
        }

        public static void removeTagID_commonStories(string cellID, string TagID)
        {

            if (commonStories.ContainsKey(cellID) == true)
            {
                commonStories[cellID].Remove(TagID);
            }
        }

        public static Boolean commonStories_containKeyValue(String cellID, String TagID)
        {
            if (commonStories.ContainsKey(cellID) == true)            
                return commonStories[cellID].Contains(TagID);            
            else
                return false;
        }
    }
}