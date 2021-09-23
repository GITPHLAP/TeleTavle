using System;
using System.Collections.Generic;
using System.Text;

namespace TelefonTavlenWPF
{
    public class FacebookPost
    {
        string url;
        string header;
        string description;
        string name;

        public FacebookPost(string url, string header, string description, string name)
        {
            Url = url;
            Header = header;
            Description = description;
            Name = name;
        }

        public string Url { get => url; set => url = value; }
        public string Header { get => header; set => header = value; }
        public string Description { get => description; set => description = value; }
        public string Name { get => name; set => name = value; }


    }
}
