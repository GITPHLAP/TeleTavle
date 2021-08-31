using System;
using System.Collections.Generic;
using System.Text;


namespace TeleTavleLibrary
{
    public class SearchResult
    {
        int rank;
        string searchWord;
        string url;
        string featuredSnippet;

        public int Rank { get => rank; set => rank = value; }
        public string SearchWord { get => searchWord; set => searchWord = value; }
        public string Url { get => url; set => url = value; }
        public string FeaturedSnippet { get => featuredSnippet; set => featuredSnippet = value; }
        public string UrlLocation => new Uri(Url).AbsolutePath.Remove(0, 1);
        public string SearchWordWithNum
        {
            get => SearchWordWithNum;

            set => SearchWordWithNum = value;

        }
    }
}