using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
    public class SearchResultSEF
    {
        public SearchResult SearchResult;
        public string Header;
        public string Description;

        public SearchResultSEF(SearchResult searchresult)
        {
            SearchResult = searchresult;
        }

        public SearchResultSEF()
        {

        }
    }
}
