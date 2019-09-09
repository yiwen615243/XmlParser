using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simplified_Wiki_XML
{
    public class PageModel
    {
        public PageModel(){}

        private string title;
        public string Title{
            get{return title;}
            set{title = value;}
        }
        private string nameSpace;
        public string NameSpace{
            get{return nameSpace;}
            set{nameSpace = value;}
        }
        private string pageID;
        public string PageID{
            get{return pageID;}
            set{pageID = value;}
        }
        private string texts;
        public string Texts{
            get{return texts;}
            set{texts = value;}
        }

        private string wikipediaLink;
        public string WikipediaLink{
            get{return wikipediaLink;}
            set{wikipediaLink = value;}
        }
    }
}