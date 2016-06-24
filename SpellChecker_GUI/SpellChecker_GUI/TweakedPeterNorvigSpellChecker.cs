using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpellChecker_GUI
{
    ///////////////////////////
    //
    // Tweaks to the original:
    //
    // -Added the capability to spell check Portuguese, side by side with English.
    // -Added the capability of words with ‘ to the English language.
    // -Added in the Portuguese version the change of ‘ç’ to ‘ss’ and ‘ss’ to ‘ç’.
    // -Added support for returning a list of words suggestions with a complex sorting order
    // -Added support for loading words lists directly in a more compact way (faster).
    // -Added performance optimizations
    //

    internal class TweakedPeterNorvigSpellChecker
    {
        private const int FreqNumberOfCorrectWordsWithOutFreq = 10;

        // English
        private Dictionary<string, int> EN_wordDic;
        private static readonly string EN_alphabet = @"abcdefghijklmnopqrstuvwxyz'";
        private static readonly char[] EN_alphabetArray = EN_alphabet.ToArray();

        // Portuguese
        private Dictionary<string, int> PT_wordDic;
        private static readonly string PT_alphabet = @"abcdefghijklmnopqrstuvwxyzãõàáéíóúâêôç-";
        private static readonly char[] PT_alphabetArray = PT_alphabet.ToArray();

        // Currently using
        private Dictionary<string, int> NWORDS;
        private string alphabet;
        private char[] alphabetArray;
        private Regex regExCompWords;

        private Utils.Language currentLang;
        internal Utils.Language CurrentLang
        {
            get
            {
                return currentLang;
            }

            set
            {
                currentLang = value;
                switch (value)
                {
                    case Utils.Language.English:
                        NWORDS = EN_wordDic;
                        alphabet = EN_alphabet;
                        alphabetArray = EN_alphabetArray;
                        regExCompWords = Utils.RegExCompWordEN;
                        break;
                    case Utils.Language.Portuguese:
                        NWORDS = PT_wordDic;
                        alphabet = PT_alphabet;
                        alphabetArray = PT_alphabetArray;
                        regExCompWords = Utils.RegExCompWordPT;
                        break;
                }
            }
        }

        internal TweakedPeterNorvigSpellChecker(Utils.Language lang)
        {
            EN_wordDic = new Dictionary<string, int>(100000);
            PT_wordDic = new Dictionary<string, int>(100000);
            currentLang = lang;
        }

        internal void trainFromTextFile(Utils.Language lang, string trainPath)
        {
            string trainText = File.ReadAllText(trainPath, Encoding.UTF8);
            Regex regExWordsLang = null;
            Dictionary<string, int> dicWords = null;
            switch (lang)
            {
                case Utils.Language.English:
                    regExWordsLang = Utils.RegExCompWordEN;
                    dicWords = EN_wordDic;
                    break;
                case Utils.Language.Portuguese:
                    regExWordsLang = Utils.RegExCompWordPT;
                    dicWords = PT_wordDic;
                    break;
            }

            int numParts = 10;
            int partNumCharacters = trainText.Length / 10;
            for (int i = 0; i < numParts; i++)
            {
                int beginIndex = partNumCharacters * i;
                string trainTextPart = trainText.Substring(beginIndex, partNumCharacters);
                MatchCollection mColl = regExWordsLang.Matches(trainTextPart);
                foreach (Match match in mColl)
                {
                    string word = match.Value.ToLower();
                    int counts;
                    if (dicWords.TryGetValue(word, out counts))
                        dicWords[word] = ++counts;
                    else
                        dicWords.Add(word, 1);
                }

            }
        }

        internal void trainFromWordListFile(Utils.Language lang, string trainPath, bool withFreq)
        {
            string wordListText = File.ReadAllText(trainPath, Encoding.UTF8);
            Regex regExWordsLang = null;
            Dictionary<string, int> dicWords = null;
            switch (lang)
            {
                case Utils.Language.English:
                    regExWordsLang = Utils.RegExCompWordEN;
                    dicWords = EN_wordDic;
                    break;
                case Utils.Language.Portuguese:
                    regExWordsLang = Utils.RegExCompWordPT;
                    dicWords = PT_wordDic;
                    break;
            }
            string[] lines = wordListText.Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim() == "")
                    continue;
                if (withFreq)
                {
                    string[] items = line.Split(' ');
                    string word = items[0].Trim().ToLower();
                    int count = Int32.Parse(items[1]);
                    if (!dicWords.ContainsKey(word))
                    {
                        dicWords.Add(word, count);
                    }
                }
                else
                {
                    // WithOutFreq
                    string word = line.Trim().ToLower();
                    int count;
                    if (dicWords.TryGetValue(word, out count))
                    {
                        dicWords[word] = count + FreqNumberOfCorrectWordsWithOutFreq;
                    }
                    else
                    {
                        dicWords.Add(word, FreqNumberOfCorrectWordsWithOutFreq);
                    }
                }
            }
        }

        private HashSet<string> edits1(string word)
        {
            //List<Tuple<string, string>> splitsList = new List<Tuple<string, string>>();
            List<Utils.MyTupleStr> splitsList = new List<Utils.MyTupleStr>();

            List<string> deletesList = new List<string>(50);
            List<string> transposesList = new List<string>(50);
            List<string> replacesList = new List<string>(1000);
            List<string> insertsList = new List<string>(1000);

            // Splits            
            for (int i = 0; i < word.Length + 1; i++)
            {
                //splitsList.Add(new Tuple<string, string>(word.Substring(0, i), word.Substring(i)));
                splitsList.Add(new Utils.MyTupleStr(word.Substring(0, i), word.Substring(i)));
            }

            // Deletes
            foreach (var tuple in splitsList)
            {
                string a = tuple.Item1;
                string b = tuple.Item2;
                if (b != "")
                    deletesList.Add(a + b.Substring(1));
            }

            // Transposes
            foreach (var tuple in splitsList)
            {
                string a = tuple.Item1;
                string b = tuple.Item2;
                if (b.Length > 1)
                    transposesList.Add($"{a}{b[1]}{b[0]}{b.Substring(2)}");
            }

            // Replaces
            foreach (var tuple in splitsList)
            {
                string a = tuple.Item1;
                string b = tuple.Item2;
                if (b != "")
                {
                    foreach (var c in alphabetArray)
                        replacesList.Add($"{a}{c}{b.Substring(1)}");
                }
            }

            if (currentLang == Utils.Language.Portuguese)
            {
                // Replace "ç" => "ss"
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == 'ç')
                    {
                        string a = word.Substring(0, i);
                        string b = word.Substring(i + 1);
                        replacesList.Add($"{a}{"ss"}{b}");
                    }
                }

                // Replace "ss" => "ç"
                for (int i = 0; i < word.Length; i++)
                {
                    if (( i != 0) && (word.Length > 1))
                    {
                        if ((word[i - 1] == 's') && (word[i] == 's'))
                        {
                        string a = word.Substring(0, i-1);
                        string b = word.Substring(i + 1);
                        replacesList.Add($"{a}{"ç"}{b}");
                        }
                    }
                }

            }

            // Inserts
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
            foreach (var w in words)
            {
                int count;
                if (NWORDS.TryGetValue(w, out count))
                {
                    set.Add(w);
                }
            }
            return set;
        }

        private List<WordFreq> knownList(HashSet<string> wordsAlreadySeen, IEnumerable<string> words)
        {
            List<WordFreq> resultList = new List<WordFreq>();
            foreach (var w in words)
            {
                if (!wordsAlreadySeen.Contains(w))
                {
                    int count;
                    if (NWORDS.TryGetValue(w, out count))
                    {
                        wordsAlreadySeen.Add(w);
                        resultList.Add(new WordFreq(w, count));
                    }
                }
            }
            return resultList;
        }

        private List<WordFreq> knownList_edits2(HashSet<string> wordsAlreadySeen, string word)
        {
            List<WordFreq> resultList = new List<WordFreq>(30);
            foreach (var e1 in edits1(word))
            {
                foreach (var e2 in edits1(e1))
                {
                    if (!wordsAlreadySeen.Contains(e2))
                    {
                        int count;
                        if (NWORDS.TryGetValue(e2, out count))
                        {
                            wordsAlreadySeen.Add(e2);
                            resultList.Add(new WordFreq(e2, count));
                        }
                    }
                }
            }
            return resultList;
        }

        // Note: Note used because it's to heavy in terms of CPU.
        private List<WordFreq> knownList_edits3(HashSet<string> wordsAlreadySeen, string word)
        {
            List<WordFreq> resultList = new List<WordFreq>(50);
            foreach (var e1 in edits1(word))
            {
                foreach (var e2 in edits1(e1))
                {
                    foreach (var e3 in edits1(e2))
                    {
                        if (!wordsAlreadySeen.Contains(e3))
                        {
                            int count;
                            if (NWORDS.TryGetValue(e3, out count))
                            {
                                wordsAlreadySeen.Add(e3);
                                resultList.Add(new WordFreq(e3, count));
                            }
                        }
                    }
                }
            }
            return resultList;
        }

        internal bool wordExists(string word)
        {
            int count;
            if (NWORDS.TryGetValue(word.ToLower(), out count))
                return true;
            else
                return false;
        }

        internal List<WordFreq> correct(string word, out bool wordIsCorrect)
        {
            List<WordFreq> resultsList = new List<WordFreq>();
            if (word.Length == 0)
            {
                wordIsCorrect = true;
                return resultsList;
            }

            wordIsCorrect = false;
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
            else
            {
                wordIsCorrect = true;
            }

            foreach (var w in set)
            {
                int count;
                if (NWORDS.TryGetValue(w, out count))
                {
                    resultsList.Add(new WordFreq(w,count));
                }
            }

            resultsList = resultsList.OrderByDescending(o => (o.Word[0] == word[0]) ? 5 : 1).ThenByDescending(o => o.Count).ToList();

            return resultsList;
        }

        // This version implements a more agressive optimisation on the words that are suggested,
        // specially in details of the order by witch they are sorted.
        // It also generates more possible words, depending on the size of the word, it
        // returns words with edit distances of one two. Not three.
        internal List<WordFreq> correctV2(string word, out bool wordIsCorrect)
        {
            List<WordFreq> resultsList = new List<WordFreq>(50);
            if (word.Length == 0)
            {
                wordIsCorrect = true;
                return resultsList;
            }

            wordIsCorrect = false;
            word = word.ToLower();

            HashSet<string> dicWordsAlreadySeen = new HashSet<string>();

            resultsList = knownList(dicWordsAlreadySeen, new List<string> { word });
            if (resultsList.Count != 0)
                return resultsList;

            List<WordFreq> resultsListSecondPart = new List<WordFreq>(50);

            List<WordFreq> resultsListEdit_1 = null;
            List<WordFreq> resultsListEdit_2 = null;
            //List<WordFreq> resultsListEdit_3 = null;

            // Edit 1 distance.
            resultsListEdit_1 = knownList(dicWordsAlreadySeen, edits1(word));
            if (resultsListEdit_1.Count != 0)
            {
                resultsListEdit_1 = resultsListEdit_1.OrderByDescending(o => (o.Word[0] == word[0]) ? 5 : 1).ThenByDescending(o => o.Count).ToList();
                foreach (var wordFreq in resultsListEdit_1)
                {
                    if (wordFreq.Word[0] == word[0])
                        // Copy word started with letter, to the first part.
                        resultsList.Add(wordFreq);
                    else
                        // Copy rest of the words to the second part. 
                        resultsListSecondPart.Add(wordFreq);
                }
            }

            // Edit 2 distance.
            if ( ((word.Length >= 5) || (resultsListEdit_1.Count == 0)) && (word.Length <= 25) )
            {
                resultsListEdit_2 = knownList_edits2(dicWordsAlreadySeen, word);
                if (resultsListEdit_2.Count != 0)
                {
                    resultsListEdit_2 = resultsListEdit_2.OrderByDescending(o => (o.Word[0] == word[0]) ? 5 : 1).ThenByDescending(o => o.Count).ToList();
                    foreach (var wordFreq in resultsListEdit_2)
                    {
                        if (wordFreq.Word[0] == word[0])
                            // Copy word started with letter, to the first part.
                            resultsList.Add(wordFreq);
                        else
                            // Copy rest of the words to the second part. 
                            resultsListSecondPart.Add(wordFreq);
                    }
                }
            }

            /*
            // Edit 3 distance.
            if (word.Length >= 10 || ((resultsListEdit_1.Count == 0) && (resultsListEdit_2.Count == 0)) )
            {
                resultsListEdit_3 = knownList_edits3(dicWordsAlreadySeen, word);
                if (resultsListEdit_3.Count != 0)
                {
                    resultsListEdit_3 = resultsListEdit_3.OrderByDescending(o => (o.Word[0] == word[0]) ? 5 : 1).ThenByDescending(o => o.Count).ToList();
                    foreach (var wordFreq in resultsListEdit_3)
                    {
                        if (wordFreq.Word[0] == word[0])
                            // Copy word started with letter, to the first part.
                            resultsList.Add(wordFreq);
                        else
                            // Copy rest of the words to the second part. 
                            resultsListSecondPart.Add(wordFreq);
                    }
                }
            }
            */

            resultsList.AddRange(resultsListSecondPart);

            return resultsList;
        }


        internal bool dataOnWord(string word, out int counts)
        {
            if (NWORDS.TryGetValue(word.ToLower(), out counts))
                return true;
            else
                return false;
        }

        internal static string wordListToString(List<WordFreq> wordsList)
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < wordsList.Count; i++)
            {
                WordFreq wL = wordsList[i];
                if (i != 0)
                    strBuilder.Append(",");
                strBuilder.Append($" {wL.Word} : {wL.Count}");
            }
            return strBuilder.ToString();
        }

    }

    internal class Test_TweakedPeterNorvigSpellChecker
    {
        internal static void runTests()
        {
            runTestEN();             // From big.txt text file.
            //runTestPT();           // From text file.
            runTestWordListPT();     // From word dictionary.
        }

        private static void runTestEN()
        {
            string trainFilePath = @"..\..\trainFiles\big.txt";
            TweakedPeterNorvigSpellChecker sp = new TweakedPeterNorvigSpellChecker(Utils.Language.English);
            sp.CurrentLang = Utils.Language.Portuguese;
            sp.CurrentLang = Utils.Language.English;

            sp.trainFromTextFile(Utils.Language.English, trainFilePath);

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

        private static void runTestPT()
        {
            //string trainFilePath = @"..\..\trainFiles\big.txt";

            string trainFilePath = @"D:\Projectos_casa\Spell_Checker\ptwiki-20160601-pages-meta-current1.xml\ptwiki-20160601-pages-meta-current1.xml.001";


            TweakedPeterNorvigSpellChecker sp = new TweakedPeterNorvigSpellChecker(Utils.Language.Portuguese);
            sp.CurrentLang = Utils.Language.English;
            sp.CurrentLang = Utils.Language.Portuguese;

            sp.trainFromTextFile(Utils.Language.Portuguese, trainFilePath);

            testWord(sp, "diaa", "dia");

            int counts = 0;
            string word = "dia";
            if (sp.dataOnWord(word, out counts))
                Console.WriteLine($"word: {word} counts: {counts}");

            word = "dias";
            if (sp.dataOnWord(word, out counts))
                Console.WriteLine($"word: {word} counts: {counts}");

            testWord(sp, "bondaide", "bondade");
            testWord(sp, "cançaço", "cansaço");
            testWord(sp, "feelizz", "feliz");
            testWord(sp, "contenteee", "contente");
            testWord(sp, "diz-mee", "diz-me");
            testWord(sp, "plavras", "palavras");
            testWord(sp, "pnoto", "ponto");
            testWord(sp, "oje", "hoje");
            testWord(sp, "trabalhoss", "trabalho");
            testWord(sp, "iram", "irão");
        }

        private static void runTestWordListPT()
        {
            // string trainFilePath = @"..\..\trainFiles\big.txt";

            string trainWordListFilePath = @"D:\Projectos_casa\Spell_Checker\WordLists_PT\europarl-v7.pt-en.pt_WordList_158325_.txt";


            TweakedPeterNorvigSpellChecker sp = new TweakedPeterNorvigSpellChecker(Utils.Language.Portuguese);
            sp.CurrentLang = Utils.Language.English;
            sp.CurrentLang = Utils.Language.Portuguese;

            bool withFreq = true;
            sp.trainFromWordListFile(Utils.Language.Portuguese, trainWordListFilePath, withFreq);

            testWord(sp, "diaa", "dia");

            int counts = 0;
            string word = "dia";
            if (sp.dataOnWord(word, out counts))
                Console.WriteLine($"word: {word} counts: {counts}");

            word = "dias";
            if (sp.dataOnWord(word, out counts))
                Console.WriteLine($"word: {word} counts: {counts}");

            testWord(sp, "bondaide", "bondade");
            testWord(sp, "cançaço", "cansaço");
            testWord(sp, "feelizz", "feliz");
            testWord(sp, "contenteee", "contente");
            testWord(sp, "diz-mee", "diz-me");
            testWord(sp, "plavras", "palavras");
            testWord(sp, "pnoto", "ponto");
            testWord(sp, "oje", "hoje");
            testWord(sp, "trabalhoss", "trabalho");
            testWord(sp, "iram", "irão");
        }

        private static void testWord(TweakedPeterNorvigSpellChecker sp, string word, string expected)
        {
            bool wordIsCorrect;
            List<WordFreq> wordsList = sp.correct(word, out wordIsCorrect);
            string sugestedWord = "";
            if (wordsList.Count > 0)
                sugestedWord = wordsList[0].Word;
            Console.WriteLine($"correct('{word}') => '{sugestedWord}'; expected '{expected}'");
        }

    }

    internal class SpellCheckerConsoleInterface
    {
        internal SpellCheckerConsoleInterface()
        {

        }

        internal void startConsole()
        {
            TweakedPeterNorvigSpellChecker tsc = new TweakedPeterNorvigSpellChecker(Utils.Language.Portuguese);
            tsc.CurrentLang = Utils.Language.Portuguese;
            string trainWordListFilePath = @"D:\Projectos_casa\Spell_Checker\WordLists_PT\europarl-v7.pt-en.pt_WordList_158325_.txt";
            bool withFreq = true;
            tsc.trainFromWordListFile(Utils.Language.Portuguese, trainWordListFilePath, withFreq);

            Console.WriteLine("\n\nStarting Spell Checker console type \"quit\" to exit.");
            string word;
            do
            {
                Console.Write("\nWord: ");
                word = Console.ReadLine().Trim();
                if (word != "" && word.ToLower() != "quit")
                {
                    string firstWord = word.Split(' ')[0];
                    bool wordIsCorrect;
                    List<WordFreq> correctedWordsList = tsc.correct(firstWord, out wordIsCorrect);
                    if (wordIsCorrect)
                        Console.WriteLine($"Word is Correct!");
                    else
                    {
                        Console.WriteLine($"Correct word: {TweakedPeterNorvigSpellChecker.wordListToString(correctedWordsList)}");
                    }                    
                }
                    
            } while (word.ToLower() != "quit");
            Console.WriteLine("\n\n Quiting the program!");
        }

    }

    public static class ExtensionToHashSet
    {
        public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
        {
            bool allAdded = true;
            foreach (T item in items)
            {
                allAdded &= @this.Add(item);
            }
            return allAdded;
        }
    }

    internal class WordFreq
    {
        internal string Word { get; set; }
        internal int Count { get; set; }

        internal WordFreq(string word, int count )
        {
            this.Word = word;
            this.Count = count;
        }

        public override string ToString()
        {
            return $"{Word}:{Count}";
        }

        internal void incrementCount()
        {
            Count++;
        }

    }

}
