using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
     public interface IInformationCrawler<T,K>
    {
        public T CrawlInformation(K input); 
    }
}
