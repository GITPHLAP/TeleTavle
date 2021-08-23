using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
     public interface IInformationCrawler<T,K>
    {
        T CrawlInformation(K input); 
    }
}
