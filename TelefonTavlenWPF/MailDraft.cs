using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using TeleTavleLibrary;

namespace TelefonTavlenWPF
{
    public class MailDraft
    {
        public FlowDocument CreateMailDraft(List<SearchResultSEF> searchResults, List<string> searchwords)
        {
            #region default draft
            FlowDocument draftflow = new FlowDocument();

            Paragraph par = new Paragraph(new Run("Hej"));
            draftflow.Blocks.Add(par);

            //to create a empty line
            draftflow.Blocks.Add(new Paragraph(new Run()));

            par = new Paragraph(new Run("Jeg vedlægger faktura [FakturaNr], som dækker de næste 3 måneders abonnement på telefontavlen."));
            draftflow.Blocks.Add(par);

            //to create a empty line
            draftflow.Blocks.Add(new Paragraph(new Run()));

            par = new Paragraph(new Run("Jeg vil også gerne gøre dig opmærksom på, at jeg bruger både Google, Facebook samt Instagram med relevante søgeord og billeder for at markedsføre [Virksomheds_Navn]. Hvis [Virksomheds_Navn] har nogle nyheder, som i gerne vil have frem både på Google, Facebook og Instagram, så send mig nogle billeder og noget beskrivelse."));
            draftflow.Blocks.Add(par);


            par = new Paragraph(new Bold(new Run("Det koster ikke ekstra")));
            draftflow.Blocks.Add(par);

            //to create a empty line
            draftflow.Blocks.Add(new Paragraph(new Run()));

            par = new Paragraph(new Run("Skulle du have brug for at få markedsført et bestemt produkt eller andet gennem en periode på f.eks. 6 måneder, så koster dette kun kr. 300 en gang for alle for hele perioden"));
            draftflow.Blocks.Add(par);

            //to create a empty line
            draftflow.Blocks.Add(new Paragraph(new Run()));

            par = new Paragraph(new Run("Dine søgeresultater via telefontavlen.dk er som følger på Googles gratis annoncer:"));
            draftflow.Blocks.Add(par);

            //to create a empty line
            draftflow.Blocks.Add(new Paragraph(new Run()));

            #endregion

            CreateSearchWordTable(draftflow, searchResults, searchwords);

            #region default draft - Lastline
            //to create a empty line
            draftflow.Blocks.Add(new Paragraph(new Run()));

            par = new Paragraph(new Run("Du har til d.d. haft lidt over [Antal Visninger] visninger på din profilside."));
            draftflow.Blocks.Add(par);
            #endregion

            return draftflow;
        }

        private void CreateSearchWordTable(FlowDocument document, List<SearchResultSEF> searchResults, List<string> searchwords)
        {
            Paragraph par = new Paragraph();

            //Table for searchwords
            //Create a table and add it to Flowdocument
            Table searchWordTable = new Table();
            document.Blocks.Add(searchWordTable);

            searchWordTable.CellSpacing = 30;

            //Add 2 columns
            int numberOfColumns = 2;
            for (int x = 0; x < numberOfColumns; x++)
            {
                searchWordTable.Columns.Add(new TableColumn());
            }

            // Create and add an empty TableRowGroup to hold the table's Rows.
            searchWordTable.RowGroups.Add(new TableRowGroup());

            // Add the first (title) row.
            searchWordTable.RowGroups[0].Rows.Add(new TableRow());

            // Alias the current working row for easy reference.
            TableRow currentRow = searchWordTable.RowGroups[0].Rows[0];

            // Add the header in the first row
            currentRow.Cells.Add(new TableCell(new Paragraph(new Bold(new Underline(new Run("Søgeord/søgetermer"))))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Bold(new Underline(new Run("Resultat"))))));

            int searchwordscount = 0;

            //groupe search result by the searchword and the nmake a list of ranks
            var groupedsearchResults = searchResults.GroupBy(re => re.SearchResult.SearchWord, re => re.SearchResult.Rank, (key, r) => new { SearchWord = key, Rank = r.ToList() });
            
            
            //GroupBy searchword and create a list with rank 
            foreach (string searchword in searchwords)
            {
                searchwordscount++;

                

                //add row and set the currentrow with the searchwordcounter
                searchWordTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = searchWordTable.RowGroups[0].Rows[searchwordscount];

                //the first cell in the currentrow set to the searchword
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(searchword))));

                //if search word match the grouped list then it should return the list else it will return null
                var result = groupedsearchResults.Where(r => r.SearchWord == searchword).FirstOrDefault();

                if (result == null)
                {
                    par = new Paragraph(new Bold(new Run("Der blev ikke fundet nogle søge resultater, for dette søgeord på Googles side 1 endnu, men jeg arbejder på sagen.")));
                }
                else
                {
                    //Find the featured snippet
                    var checkSnippet = searchResults.Where(x => x.SearchResult.SearchWord == searchword && x.SearchResult.FeaturedSnippet != null)
                        .FirstOrDefault();
                    string featuredSnippet = "";
                    if (checkSnippet != null)
                    {
                        featuredSnippet = checkSnippet.SearchResult.FeaturedSnippet;
                    }

                    if (result.Rank.Count > 1)
                    {
                        //Add a paragraph for the next cell
                        par = new Paragraph(new Bold(new Run("Her kommer dine annoncer frem på Google som nr. ")));

                        //add the first rank
                        par.Inlines.Add(new Bold(new Run(result.Rank[0].ToString())));

                        //foreach rank add it to the line
                        for (int i = 1; i < result.Rank.Count; i++)
                        {
                            par.Inlines.Add(new Bold(new Run(" og ")));
                            par.Inlines.Add(new Bold(new Run(result.Rank[i].ToString())));
                        }

                    }
                    else
                    {

                        //Add a paragraph for the next cell
                        par = new Paragraph(new Bold(new Run("Her kommer din annonce frem på Google som nr. ")));

                        //if only one rank then just add it
                        par.Inlines.Add(new Bold(new Run(result.Rank.First().ToString())));

                        
                    }
                    //If there is a featured snippet, then it will be shown
                    if (result.Rank.Where(x => x == 1).FirstOrDefault() == 1 && !string.IsNullOrEmpty(featuredSnippet))
                    {
                        par.Inlines.Add(new Run("\nMed en udvidet reklame som kan ses nedenunder: \n"));
                        par.Inlines.Add(new Run(featuredSnippet));
                    }
                }
                
                //add the paragraph to the cell
                currentRow.Cells.Add(new TableCell(par));
            }

        }
    }
}
