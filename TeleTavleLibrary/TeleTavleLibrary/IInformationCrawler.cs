using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
    interface IInformationCrawler<T,K>
    {
        T CrawlInformation(K input); 
    }
}
