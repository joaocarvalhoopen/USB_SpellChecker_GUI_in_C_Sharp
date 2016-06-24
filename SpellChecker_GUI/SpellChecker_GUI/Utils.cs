using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;

namespace SpellChecker_GUI
{
    internal class Utils
    {
        internal enum Language
        {
            English,
            Portuguese
        }

        // PT regular expressions pre-compilation.

        // Regular expression to detect words in the Portuguese language:
        // [a-zãõàáéíóúâêôç]+([-][a-zãõàáéíóúâêôç]+)+|[a-zãõàáéíóúâêôç]+
        private static readonly string lettersClassForWordPatternPT = @"[a-zãõàáéíóúâêôç]";
        private static string regExStrWordPatternPT = String.Format(@"{0}+([-]{0}+)+|{0}+", lettersClassForWordPatternPT);
        private static Regex regExCompWordPatternPT = new Regex(regExStrWordPatternPT, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // EN regular expressions pre-compilation.

        // Regular expression to detect words in the Portuguese language:
        // [a-z]+['][a-z]+|[a-z]+
        private static readonly string lettersClassForWordPatternEN = @"[a-z]";
        private static string regExStrWordPatternEN = String.Format(@"{0}+[']{0}+|{0}+", lettersClassForWordPatternEN);
        private static Regex regExCompWordPatternEN = new Regex(regExStrWordPatternEN, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static Regex RegExCompWordPT
        {
            get
            {
                return regExCompWordPatternPT;
            }
        }

        internal static Regex RegExCompWordEN
        {
            get
            {
                return regExCompWordPatternEN;
            }
        }

        internal class MyTupleStr
        {
            internal string Item1 { get; set; }
            internal string Item2 { get; set; }

            internal MyTupleStr(string item1, string item2)
            {
                this.Item1 = item1;
                this.Item2 = item2;
            }
        }

    }
}
