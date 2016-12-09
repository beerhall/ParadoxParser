using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ParadoxParser
{
    public class ParadoxParser
    {        
        public ParadoxParser(string filename, Encoding encoding)
        {
            StreamReader sr = new StreamReader(filename, encoding);
            text = sr.ReadToEnd();
        }

        public ParadoxObject parse()
        {
            return new ParadoxObject("", text, true);
        }

        private string text;
    }
}
