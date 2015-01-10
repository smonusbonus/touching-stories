using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchingStory
{
    public class Story
    {
        /// <summary>
        /// the variable name will be automatically matched with the node name in the json file 
        /// </summary>
        public string title;
        public string text;
        public string concept;
        public string character;
        public string location;
        public int id;
        public string subgenre;
        public List<string> tags;
    }

}
