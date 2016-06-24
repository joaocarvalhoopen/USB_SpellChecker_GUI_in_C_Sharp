using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpellChecker_GUI
{
    ////////////////////////////////////////////////////////////////////
    //                                                                //
    // IMPORTANT: Do not edit this Version use the "Tweaked" Version. //
    //                                                                //
    ////////////////////////////////////////////////////////////////////

    /*
     * Peter Norvig Python Code: http://norvig.com/spell-correct.html
     
    import re, collections

    def words(text): return re.findall('[a-z]+', text.lower()) 

    def train(features):
        model = collections.defaultdict(lambda: 1)
        for f in features:
            model[f] += 1
        return model

    NWORDS = train(words(file('big.txt').read()))

    alphabet = 'abcdefghijklmnopqrstuvwxyz'

    def edits1(word):
       splits     = [(word[:i], word[i:]) for i in range(len(word) + 1)]
       deletes    = [a + b[1:] for a, b in splits if b]
       transposes = [a + b[1] + b[0] + b[2:] for a, b in splits if len(b)>1]
       replaces   = [a + c + b[1:] for a, b in splits for c in alphabet if b]
       inserts    = [a + c + b     for a, b in splits for c in alphabet]
       return set(deletes + transposes + replaces + inserts)

    def known_edits2(word):
        return set(e2 for e1 in edits1(word) for e2 in edits1(e1) if e2 in NWORDS)

    def known(words): return set(w for w in words if w in NWORDS)

    def correct(word):
        candidates = known([word]) or known(edits1(word)) or known_edits2(word) or [word]
        return max(candidates, key=NWORDS.get)
     */


    ////////////////////////////////////////////////////////////////////
    //                                                                //
    // IMPORTANT: Do not edit this Version use the "Tweaked" Version. //
    //                                                                //
    ////////////////////////////////////////////////////////////////////

    internal class PeterNorvigSpellChecker
    {
        private Dictionary<string, int> NWORDS;
        private static readonly string alphabet = @"abcdefghijklmnopqrstuvwxyz";
        private static readonly char[] alphabetArray = alphabet.ToArray();
        private static string regExStrWordsEN = @"[a-z]+";
        private static Regex regExCompWordsEN = new Regex(regExStrWordsEN, RegexOptions.Compiled | RegexOptions.IgnoreCase);


        internal PeterNorvigSpellChecker()
        {
            NWORDS = new Dictionary<string, int>(1000000);
        }

        internal void train(string trainPath)
        {
            string trainText = File.ReadAllText(trainPath, Encoding.UTF8);
            MatchCollection mColl = regExCompWordsEN.Matches(trainText);
            foreach (Match match in mColl)
            {
                string word = match.Value.ToLower();
                int counts;
                if (NWORDS.TryGetValue(word, out counts))
                    NWORDS[word] = ++counts;
                else
                    NWORDS.Add(word, 1);
            }
        }

        private HashSet<string> edits1(string word)
        {
            //List<Tuple<string, string>> splitsList = new List<Tuple<string, string>>();
            List<Utils.MyTupleStr> splitsList = new List<Utils.MyTupleStr>();

            List<string> deletesList = new List<string>();
            List<string> transposesList = new List<string>();
            List<string> replacesList = new List<string>();
            List<string> insertsList = new List<string>();

            // Splits
            // splits = [(word[:i], word[i:]) for i in range(len(word) + 1)]
            for (int i = 0; i < word.Length + 1; i++)
            {
                //splitsList.Add(new Tuple<string, string>(word.Substring(0, i), word.Substring(i)));
                splitsList.Add(new Utils.MyTupleStr(word.Substring(0, i), word.Substring(i)));
            }

            // Deletes
            // deletes = [a + b[1:] for a, b in splits if b]
            foreach (var tuple in splitsList)
            {
                string a = tuple.Item1;
                string b = tuple.Item2;
                if (b != "")
                    deletesList.Add(a + b.Substring(1));
            }

            // Transposes
            // transposes = [a + b[1] + b[0] + b[2:] for a, b in splits if len(b) > 1]
            foreach (var tuple in splitsList)
            {
                string a = tuple.Item1;
                string b = tuple.Item2;
                if (b.Length > 1)
                    transposesList.Add($"{a}{b[1]}{b[0]}{b.Substring(2)}");  // Note: Test the substring of a index that doesn't exists.                
            }

            // Replaces
            // replaces = [a + c + b[1:] for a, b in splits for c in alphabet if b]
            foreach (var tuple in splitsList)
            {
                string a = tuple.Item1;
                string b = tuple.Item2;
                if (b != "")
                {
                    foreach (var c in alphabetArray)
                        replacesList.Add($"{a}{c}{b.Substring(1)}");  // Note: Test the substring of a index that doesn't exists.
                }
            }

            // Inserts
            // inserts = [a + c + b     for a, b in splits for c in alphabet]
            foreach (var tuple in splitsList)
            {
                string a = tuple.Item1;
                string b = tuple.Item2;
                foreach (var c in alphabetArray)
                    insertsList.Add($"{a}{c}{b}");
            }

            int capacity = deletesList.Count + transposesList.Count + replacesList.Count + insertsList.Count;
            deletesList.Capacity = capacity;
            deletesList.AddRange(transposesList);
            deletesList.AddRange(replacesList);
            deletesList.AddRange(insertsList);

            HashSet<string> set = new HashSet<string>(deletesList);
            return set;
        }

        private HashSet<String> known_edits2(string word)
        {
            HashSet<string> set = new HashSet<string>();
            // set(e2 for e1 in edits1(word) for e2 in edits1(e1) if e2 in NWORDS)
            foreach (var e1 in edits1(word))
            {
                foreach (var e2 in edits1(e1))
                {
                    if (NWORDS.ContainsKey(e2))
                        set.Add(e2);
                }
            }
            return set;
        }

        private HashSet<String> known(IEnumerable<string> words)
        {
            HashSet<string> set = new HashSet<string>();
            // set(w for w in words if w in NWORDS)
            foreach (var w in words)
            {
                if (NWORDS.ContainsKey(w))
                    set.Add(w);
            }
            return set;
        }

        // def correct(word):
        //    candidates = known([word]) or known(edits1(word)) or known_edits2(word) or[word]
        //    return max(candidates, key= NWORDS.get)
        internal string correct(string word)
        {
            word = word.ToLower();
            HashSet<string> set = new HashSet<string>();
            set = known(new List<string> { word });
            if (set.Count == 0)
            {
                set = known(edits1(word));
                if (set.Count == 0)
                {
                    set = known_edits2(word);
                    if (set.Count == 0)
                        set.Add(word);
                }
            }

            int maxCount = 0;
            string maxWord = word;
            foreach (var w in set)
            {
                int count;
                if (NWORDS.TryGetValue(w, out count))
                {
                    if (count > maxCount)
                    {
                        maxCount = count;
                        maxWord = w;
                    }
                }
            }
            return maxWord;
        }

        internal bool dataOnWord(string word, out int counts)
        {
            if (NWORDS.TryGetValue(word.ToLower(), out counts))
                return true;
            else
                return false;
        }

    }

    ////////////////////////////////////////////////////////////////////
    //                                                                //
    // IMPORTANT: Do not edit this Version use the "Tweaked" Version. //
    //                                                                //
    ////////////////////////////////////////////////////////////////////

    internal class Test_PeterNorvigSpellChecker
    {
        internal static void runTests()
        {
            string trainFilePath = @"..\..\trainFiles\big.txt";
            PeterNorvigSpellChecker sp = new PeterNorvigSpellChecker();
            sp.train(trainFilePath);

            testWord(sp, "Helo", "Hello");

            int counts = 0;
            string word = "Hello";
            if (sp.dataOnWord(word, out counts))
                Console.WriteLine($"word: {word} counts: {counts}");

            word = "felo";
            if (sp.dataOnWord(word, out counts))
                Console.WriteLine($"word: {word} counts: {counts}");

            //>>> correct('speling')
            //    'spelling'
            //>>> correct('korrecter')
            //    'corrector'

            testWord(sp, "speling", "spelling");
            testWord(sp, "korrecter", "corrector");  // Corrector doesn't exists in the big.txt file.

            /*
            correct('economtric') => 'economic'(121); expected 'econometric'(1)
            correct('embaras') => 'embargo'(8); expected 'embarrass'(1)
            correct('colate') => 'coat'(173); expected 'collate'(1)
            correct('orentated') => 'orentated'(1); expected 'orientated'(1)
            correct('unequivocaly') => 'unequivocal'(2); expected 'unequivocally'(1)
            correct('generataed') => 'generate'(2); expected 'generated'(1)
            correct('guidlines') => 'guideline'(2); expected 'guidelines'(1)
            */

            testWord(sp, "economtric", "economic");
            testWord(sp, "embaras", "embargo");
            testWord(sp, "colate", "coat");
            testWord(sp, "orentated", "orentated");
            testWord(sp, "unequivocaly", "unequivocal");
            testWord(sp, "generataed", "generate");
            testWord(sp, "guidlines", "guideline");
        }

        private static void testWord(PeterNorvigSpellChecker sp, string word, string expected)
        {
            Console.WriteLine($"correct('{word}') => '{sp.correct(word)}'; expected '{expected}'");
        }

    }
}
    ////////////////////////////////////////////////////////////////////
    //                                                                //
    // IMPORTANT: Do not edit this Version use the "Tweaked" Version. //
    //                                                                //
    ////////////////////////////////////////////////////////////////////

