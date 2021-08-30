using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
    public class SearchResultSEF
    {
        SearchResult searchResult;
        string header;
        string description;

        public string Description { get => description; set => description = value; }
        public string Header { get => header; set => header = value; }
        public SearchResult SearchResult { get => searchResult; set => searchResult = value; }

        public SearchResultSEF(SearchResult searchresult)
        {
            SearchResult = searchresult;
        }

        public SearchResultSEF()
        {

        }
    }
}
