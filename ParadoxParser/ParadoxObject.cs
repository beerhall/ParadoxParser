using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ParadoxParser
{
    public class ParadoxObject
    {
        public ParadoxObject(string _key, string _content, bool _isObject)
        {
            this.key = _key;
            this.content = _content;
            this.isObject = _isObject;
            if (isObject)
            {
                parseContext(content);
            }
        }

        public string getKey() { return key; }
        public ParadoxObject getChildByKey(string key)
        {
            foreach (ParadoxObject obj in children)
            {
                if (obj.key == key)
                {
                    return obj;
                }
            }
            return null;
        }
        public HashSet<ParadoxObject> getChildrenByKey()
        {
            HashSet<ParadoxObject> set = new HashSet<ParadoxObject>();
            foreach (ParadoxObject obj in children)
            {
                if (obj.key == key)
                {
                    set.Add(obj);
                }
            }
            return set;
        }
        public HashSet<ParadoxObject> getChildren()
        {
            return children;
        }
        public string getValue()
        {
            return content;
        }
        public HashSet<ParadoxObject> getAnoymousObjectChildren()
        {
            HashSet<ParadoxObject> set = new HashSet<ParadoxObject>();
            foreach (ParadoxObject obj in children)
            {
                if (isObject && string.IsNullOrEmpty(obj.key))
                {
                    set.Add(obj);
                }
            }
            return set;
        }

        private string key;
        private string content;
        private bool isObject;
        private HashSet<ParadoxObject> children = new HashSet<ParadoxObject>();

        private void parseContext(string text)
        {
            text = text.Trim();
            string[] strs = text.Split('\n');
            for (int line = 0; line < strs.Length; line++)
            {
                strs[line] = strs[line].Trim();
                //aaa = {...}
                if (Regex.IsMatch(strs[line], @"\w+\s*=\s*{.+}"))
                {
                    string key = strs[line].Substring(0, strs[line].IndexOf('='));
                    int start = strs[line].IndexOf('{') + 1;
                    int end = strs[line].LastIndexOf('}');
                    string content = strs[line].Substring(start, end - start);
                    this.children.Add(new ParadoxObject(key, content, true));
                }
                //aaa = bbb
                else if (Regex.IsMatch(strs[line], "\\w+\\s*=\\s*-*(((\\w|\\.+)+)|(\"(\\w|\\.| )+\"))"))
                {
                    string[] equation = strs[line].Split('=');
                    if (!isObject)
                    {
                        this.key = equation[0].Trim();
                        this.content = equation[1].Trim();
                    }
                    else
                    {
                        ParadoxObject obj = new ParadoxObject(equation[0].Trim(), equation[1].Trim(), false);
                        this.children.Add(obj);
                    }
                }
                //aaa = {
                else if (Regex.IsMatch(strs[line], @"\w+\s*=\s*{"))
                {
                    line = getObject(strs, line, 1, true);
                }
                //aaa = \n{
                else if (Regex.IsMatch(strs[line], @"\w+\s*="))
                {
                    line = getObject(strs, line, 2, true);
                }
                //{...}
                else if (strs[line] == "{")
                {
                    line = getObject(strs, line, 1, false);
                }
                //1.00 2.00 3.00
                else if (Regex.IsMatch(strs[line], @"\w+\s+(\w+\s+)*(\w)*"))
                {
                    while (strs[line].Contains("  "))
                    {
                        strs[line] = strs[line].Replace("  ", " ");
                    }
                    string[] tokens = strs[line].Split(' ');
                    foreach (string token in tokens)
                    {
                        this.children.Add(new ParadoxObject("", token, false));
                    }
                }
                //...
                else if (Regex.IsMatch(strs[line], @"(\w|\.)+"))
                {
                    if (isObject)
                    {
                        children.Add(new ParadoxObject("", strs[line].Trim(), false));
                    }
                    else
                    {
                        this.content = strs[line].Trim();
                    }
                }

            }
        }
        private int getObject(string[] strs, int line, int step, bool hasname)
        {
            StringBuilder sb = new StringBuilder();
            string key = hasname ? (strs[line].Split('=')[0].Trim()) : "";
            int level = 1;
            line = line + step;

            for (int cursor = 0; level != 0; cursor++)
            {
                if (cursor == strs[line].Length)
                {
                    sb.Append(strs[line] + '\n');
                    ++line;
                    cursor = 0;
                }
                if (strs[line][cursor] == '{')
                {
                    ++level;
                }
                else if (strs[line][cursor] == '}')
                {
                    --level;
                    if (level == 0)
                    {
                        sb.Append(strs[line]);
                    }
                }
            }
            string sbString = sb.ToString();
            children.Add(new ParadoxObject(key, sbString.Substring(0, sbString.Length - 1), true));
            return line;
        }
    }
}
