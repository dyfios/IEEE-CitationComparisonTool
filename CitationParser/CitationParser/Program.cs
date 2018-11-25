using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CitationParser
{
    class Citation
    {
        public string rawCitation;
        public string authors;
        public string title;
        public string publication;

        public Citation(string raw, string auth, string titl, string pub)
        {
            rawCitation = raw;
            authors = auth;
            title = titl;
            publication = pub;
        }
    }

    class Program
    {
        static bool DEBUGON = false;

        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                List<Citation> citations1 = ProcessCitations(args[0]);
                List<Citation> citations2 = ProcessCitations(args[1]);
                CompareCitations(citations1, citations2);
            }
            else
            {
                Console.WriteLine("[ERROR] Please enter two file paths containing text lists of citations.");
            }
        }

        static void CompareCitations(List<Citation> first, List<Citation> second)
        {
            Console.WriteLine("Comparing Citations\n");
            foreach (Citation firstCitation in first)
            {
                foreach (Citation secondCitation in second)
                {
                    if (firstCitation.rawCitation == secondCitation.rawCitation && firstCitation.rawCitation != null)
                    {
                        Console.WriteLine("Found a citation match:\n" + firstCitation.rawCitation + "\n");
                    }
                    else
                    {
                        if (firstCitation.title == secondCitation.title && firstCitation.title != null
                            && firstCitation.title != "NOTLISTED")
                        {
                            Console.WriteLine("Found a title match:\n" + firstCitation.rawCitation
                                + "\n" + secondCitation.rawCitation + "\n");
                        }

                        if (firstCitation.authors == secondCitation.authors && firstCitation.authors != null
                            && firstCitation.authors != "NOTLISTED")
                        {
                            Console.WriteLine("Found an authors match:\n" + firstCitation.rawCitation
                                + "\n" + secondCitation.rawCitation + "\n");
                        }

                        if (firstCitation.publication == secondCitation.publication
                            && firstCitation.publication != null && firstCitation.publication != "NOTLISTED")
                        {
                            Console.WriteLine("Found a publication match:\n" + firstCitation.rawCitation
                                + "\n" + secondCitation.rawCitation + "\n");
                        }
                    }
                }
            }
        }

        static List<Citation> ProcessCitations(string filePath)
        {
            Debug("Reading citation file " + filePath + "\n");
            string[] lines = System.IO.File.ReadAllLines(filePath);

            List<Citation> citationsToReturn = new List<Citation>();
            int lineIndex = 0;
            foreach (string line in lines)
            {
                Debug("Processing citation\n" + line);
                string shortenedLine = FixQuotes(RemoveLeadingIndex(line));
                if (shortenedLine == null)
                {
                    Console.WriteLine("[ERROR] Unable to process citation. Check the leading index of citation " + (lineIndex + 1));
                    continue;
                }

                string title = GetTitleFromCitation(shortenedLine);
                if (title == null || title == "")
                {
                    Console.WriteLine("[ERROR] Error parsing title of citation " + (lineIndex + 1));
                }
                else
                {
                    Debug("Title: " + title.ToString());
                }

                string authors = GetAuthorsFromCitation(shortenedLine);
                if (authors == null || authors == "")
                {
                    Console.WriteLine("[ERROR] Error parsing authors of citation " + (lineIndex + 1));
                }
                else
                {
                    Debug("Authors: " + authors.ToString());
                }

                string publication = GetPublicationFromCitation(shortenedLine);
                if (publication == null || publication == "")
                {
                    Console.WriteLine("[ERROR] Error parsing publication of citation " + (lineIndex + 1));
                }
                else
                {
                    Debug("Publication: " + publication.ToString());
                }

                Debug("");
                citationsToReturn.Add(new Citation(shortenedLine,
                    authors == "" ? null : authors,
                    title == "" ? null : title,
                    publication == "" ? null : publication));

                lineIndex++;
            }

            return citationsToReturn;
        }

        static string GetAuthorsFromCitation(string citation)
        {
            int maxIndex = citation.IndexOf('"');
            if (maxIndex > 0)
            {
                return citation.Substring(0, maxIndex).
                TrimEnd(new char[] { ' ', ',' });
            }
            else
            {
                return null;
            }
        }

        static string GetTitleFromCitation(string citation)
        {
            Regex reg = new Regex("\".*?\"");
            return reg.Match(citation).ToString().
                TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"', ',' });
        }

        static string GetPublicationFromCitation(string citation)
        {
            string publication = citation.Substring(citation.LastIndexOf('"') + 1);

            int maxIndex = publication.IndexOf(',');
            if (maxIndex > 0)
            {
                return publication.Substring(0, publication.IndexOf(',')).TrimStart();
            }
            else
            {
                return null;
            }
        }

        static string RemoveLeadingIndex(string citation)
        {
            if (citation[0] == '[')
            {
                int endIndex = citation.IndexOf(']');
                if (endIndex < 0)
                {
                    return null;
                }
                citation = citation.Substring(endIndex + 1).TrimStart(' ');
            }
            return citation;
        }

        static string FixQuotes(string buffer)
        {
            if (buffer.IndexOf('\u2013') > -1) buffer = buffer.Replace('\u2013', '-');
            if (buffer.IndexOf('\u2014') > -1) buffer = buffer.Replace('\u2014', '-');
            if (buffer.IndexOf('\u2015') > -1) buffer = buffer.Replace('\u2015', '-');
            if (buffer.IndexOf('\u2017') > -1) buffer = buffer.Replace('\u2017', '_');
            if (buffer.IndexOf('\u2018') > -1) buffer = buffer.Replace('\u2018', '\'');
            if (buffer.IndexOf('\u2019') > -1) buffer = buffer.Replace('\u2019', '\'');
            if (buffer.IndexOf('\u201a') > -1) buffer = buffer.Replace('\u201a', ',');
            if (buffer.IndexOf('\u201b') > -1) buffer = buffer.Replace('\u201b', '\'');
            if (buffer.IndexOf('\u201c') > -1) buffer = buffer.Replace('\u201c', '\"');
            if (buffer.IndexOf('\u201d') > -1) buffer = buffer.Replace('\u201d', '\"');
            if (buffer.IndexOf('\u201e') > -1) buffer = buffer.Replace('\u201e', '\"');
            if (buffer.IndexOf('\u2026') > -1) buffer = buffer.Replace("\u2026", "...");
            if (buffer.IndexOf('\u2032') > -1) buffer = buffer.Replace('\u2032', '\'');
            if (buffer.IndexOf('\u2033') > -1) buffer = buffer.Replace('\u2033', '\"');
            return buffer;
        }

        static void Debug(string toPrint)
        {
            if (DEBUGON)
            {
                Console.WriteLine(toPrint);
            }
        }
    }
}