/* 
 * Note: 
 * 1. the xml file must be added <wrapper> and </wrapper> at the top and bottom of the xml file respectively, 
 * otherwise "System.Xml.XmlException: 'There are multiple root elements". 
 * 2. The delimiters.txt contains all the delimiters we want to remove, it is following the format such as "{{©Ø}}, \[\[wikt:©Ø\]\]".
 * ©Ø indicate where the contents locate; "[", "|" must have "\" in the front of it in order to keep regular expressions working properly.
 * 3. The delimiters.txt must be saved in utf-8.
 */

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Simplified_Wiki_XML
{
    public class XmlParser
    {
        /*
         * Read the textfile, and save the delimiters in the textfile into an array of strings.
         * return total num of pairs of delmiters.
         */
        static int readDelimiters(string[] delimiters, string fileName)
        {
            int count = 0;
            try
            {
                string line;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(string.Format(@"{0}\Simplified Wiki XML - Yiwen Gu\{1}", getDesktop(), fileName)))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        delimiters[count] = line;
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return count;
        }

        /*
         * Read the xml page by page and save them into a List of pagemodel.
         * return total num of pages.
         */
        static int readXML(List<PageModel> modelList, string fileName)
        {
            XmlTextReader reader = new XmlTextReader(string.Format(@"{0}\Simplified Wiki XML - Yiwen Gu\{1}", getDesktop(), fileName)); //read the file "wiki-extracted-10k-1-13-19-xml.xml"
            reader.WhitespaceHandling = WhitespaceHandling.None; //ignore all the white space nodes
            PageModel model = new PageModel();
            int count = 0;
            int key = -3;  //Key: for only reading the first ID on each Page 
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "page")
                    {
                    }
                    if (reader.Name == "id")
                    {
                        if (key < 0)
                        {
                            model.PageID = reader.ReadElementString().Trim();
                            key++;
                        }
                    }
                    if (reader.Name == "title")
                    {
                        if (key < 0)
                        {
                            model.Title = reader.ReadElementString().Trim();
                            key++;
                        }
                    }
                    if (reader.Name == "ns")
                    {
                        if (key < 0)
                        {
                            model.NameSpace = reader.ReadElementString().Trim();
                            key++;
                        }
                    }
                    if (reader.Name == "text")
                    {
                        model.Texts = reader.ReadElementString().Trim();
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "page")
                    {
                        modelList.Add(model);
                        model = new PageModel();
                        key = -3;
                        count++;
                    }
                }
            }
            return count;
        }

        /*
         * Divide string by "©Ø", the ascii 193. Now Pdelimiters[n][m] has all the delimiters, 
         * n = num of pairs of delimiters, m = 2, i.e each pair has 2 elements.
         */
        static void parseDelimiters(string[][]Pdelimiters, string[] delimiters, int numDelimiters)
        {

            for (int i = 0; i < numDelimiters; i++)
            {
                Pdelimiters[i] = new string[2];
            }


            for (int i = 0; i < numDelimiters; i++)
            {
                string[] words = delimiters[i].Split("©Ø");
                Pdelimiters[i] = words;
            }
        }

        /* 
         * using regular expressions to find and remove unnecessary contents in the XML.
         * Then save the results in string[] results. 
         */
        static void XMLremove(List<PageModel>modelList, string[][] Pdelimiters, int numDelimiters, string[] results)
        {
            int index = 0;
            foreach (PageModel page in modelList)
            {
                for (int i = 0; i < numDelimiters; i++)
                {
                    string pattern = "";
                    if (Pdelimiters[i][1] == "")
                    {
                        pattern = string.Format(@"{0}", Pdelimiters[i][0]);
                    }
                    else
                    {
                        pattern = string.Format(@"{0}.*?{1}", Pdelimiters[i][0], Pdelimiters[i][1]);
                    }
                    foreach (Match match in Regex.Matches(page.Texts, pattern))
                    {
                        Console.WriteLine("pattern::::" + pattern);
                        page.Texts = page.Texts.Replace(match.Value, "");
                        Console.WriteLine("Match::::" + match.Value);
                    }
                }
                results[index] = page.Texts;
                index++;
            }
        }

        /*
         * return the path of desktop.
         */
        static string getDesktop() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return path;
        }

        /* 
         * The main process.
         */
        static void Main(string[] args){    
            List<PageModel> modelList = new List<PageModel>();
            string[] delimiters = new string[30];
            string[] results = new string[100000];

            /* read XML and save it page by page into a List.
             * test.xml = 2%
             * test1.xml = 10%
             * wiki-extracted-10K-1-13-19-xml.xml = 100%
             */
            int numPages = readXML(modelList, "test1.xml");
            // read delmiters from delimiters.txt, which contains all the delimiters we want to remove.
            int numDelimiters = readDelimiters(delimiters, "delimiters.txt"); 
            string[][] Pdelimiters = new string[numDelimiters][];
            // split delimiters by ascii193, and save them into Pdelmiters.
            parseDelimiters(Pdelimiters, delimiters, numDelimiters);
            // find and remove all the matching objects. 
            XMLremove(modelList, Pdelimiters, numDelimiters, results);
            //output the file to results.txt.
            System.IO.File.WriteAllLines(string.Format(@"{0}\Simplified Wiki XML - Yiwen Gu\results.txt", getDesktop()), results, Encoding.UTF8);
        }
    }
}
 
 