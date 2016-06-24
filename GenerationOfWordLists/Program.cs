using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FreqPTWordsInText_v001
{
    class Program
    {
        static void Main(string[] args)
        {
            // FreqWordsInText.test();

            TrigramProcessor.test();

            Console.WriteLine("\n\nEnd!");
            Console.ReadLine();
        }
    }

    internal class FreqWordsInText
    {
        private Dictionary<string, WordFreqPair> wordsDicPT;
        private Utils.Lang currentLang;

        internal FreqWordsInText(Utils.Lang lang)
        {
            wordsDicPT = new Dictionary<string, WordFreqPair>(1000000);
            currentLang = lang;
        }

        internal void processPTTxtFile(string[] pathTxtFiles)
        {
            wordsDicPT.Clear();

            bool collectNewWords = true;
            foreach (var pathStr in pathTxtFiles)
            {
                using (BinaryReader br = new BinaryReader(File.Open(pathStr, FileMode.Open)))
                {
                    int totalLength = (int)br.BaseStream.Length;    // Total length of the file

                    int maxNumBytesPerPart = 300 * 1024 * 1024; // 300MB
                    int numParts = totalLength / maxNumBytesPerPart;
                    int restBytes = totalLength % maxNumBytesPerPart;
                    byte[] bytes;
                    for (int i = 0; i < numParts; i++)
                    {
                        bytes = br.ReadBytes(maxNumBytesPerPart);
                        String textStr_300MB = System.Text.ASCIIEncoding.UTF8.GetString(bytes);
                        process_300MB_String(textStr_300MB, collectNewWords);
                    }
                    bytes = br.ReadBytes(maxNumBytesPerPart);
                    String textStrRest = System.Text.ASCIIEncoding.UTF8.GetString(bytes);
                    bytes = null;
                    process_300MB_String(textStrRest, collectNewWords);
                    textStrRest = null;
                }
                collectNewWords = false; // In the following files only counts the word that already exist in the dictionary.
            }

            //string allTextStr = File.ReadAllText(pathTxtFile, Encoding.UTF8);
            //process_300MB_String(allTextStr);

            Console.WriteLine($"Number of words in dictionary:{wordsDicPT.Count}");

            // Order the dictionry.
            List<WordFreqPair> wordList = wordsDicPT.Values.ToList<WordFreqPair>();
            wordList = wordList.OrderByDescending(val => val.Count).ToList();

            // Writes the dictionary ordered to file.
            StringBuilder strBuild = new StringBuilder(1000000);
            foreach (var wordFreqPair in wordList)
            {
                strBuild.AppendLine(wordFreqPair.ToString());
            }
            // Writes in the directory of the first file, with the name of the first file with WordList appended.
            string pathOutputWordListFile = $"{pathTxtFiles[0]}_WordList_{wordList.Count}_.txt";
            File.WriteAllText(pathOutputWordListFile, strBuild.ToString(), Encoding.UTF8);
        }

        private void process_300MB_String(string allTextStr, bool collectNewWords)
        {
            int maxNumCharPerPart = 1000000;
            int numParts = allTextStr.Length / maxNumCharPerPart;
            int numRestChar = allTextStr.Length % maxNumCharPerPart;
            int beginIndex;
            for (int i = 0; i < numParts; i++)
            {
                beginIndex = maxNumCharPerPart * i;
                findAndCountWordsPT(allTextStr, maxNumCharPerPart, beginIndex, collectNewWords);
            }
            beginIndex = maxNumCharPerPart * numParts;
            findAndCountWordsPT(allTextStr, numRestChar, beginIndex, collectNewWords);
        }

        private void findAndCountWordsPT(string allTextStr, int maxNumCharPerPart, int beginIndex, bool collectNewWords)
        {
            string textPart = allTextStr.Substring(beginIndex, maxNumCharPerPart);
            MatchCollection mColl = null;
            switch (currentLang)
            {
                case Utils.Lang.English:
                    mColl = Utils.RegExCompWordEN.Matches(textPart);
                    break;
                case Utils.Lang.Portuguese:
                    mColl = Utils.RegExCompWordPT.Matches(textPart);
                    break;
            }
            foreach (Match match in mColl)
            {
                string word = match.Value.ToLower();
                WordFreqPair wordFreqPair;
                if (wordsDicPT.TryGetValue(word, out wordFreqPair))
                {
                    wordFreqPair.Count++;
                }
                else
                {
                    if (collectNewWords)
                        wordsDicPT.Add(word, new WordFreqPair(word, 1));
                }
            }
        }

        internal static void test()
        {

            // Generating the Portuguese Euro Parlament WordListFile. 
            FreqWordsInText fw = new FreqWordsInText(Utils.Lang.Portuguese);
            String[] pathPTTextFileArray = { @"D:\JNC_Corrector_ortografico\European_Parliament_Parallel_Corpus\pt-en\europarl-v7.pt-en.pt" };
            fw.processPTTxtFile(pathPTTextFileArray);


            /*
            // Generating the English Euro Parlament plus the United Nations WordListFile (is's only one file). 
            FreqWordsInText fw = new FreqWordsInText(Utils.Lang.English);
            //String[] pathENTextFileArray = { @"D:\European_Parliament_Parallel_Corpus\pt-en\europarl-v7.pt-en.en",
            //                                 @"D:\United_nations_corpus_textual\UNv1.0.en-fr\en-fr\UNv1.0.en-fr.en"   };
            String[] pathENTextFileArray = { @"D:\European_Parliament_Parallel_Corpus\pt-en\europarl-v7.pt-en.en",
                                             @"D:\United_nations_corpus_textual\UNv1.0.en-fr\en-fr\UNv1.0.en-fr.en" };

            fw.processPTTxtFile(pathENTextFileArray);
            */
        }
    }

    internal class WordFreqPair
    {
        internal string Word { get; set; }
        internal int Count { get; set; }

        internal WordFreqPair(string word, int count)
        {
            this.Word = word;
            this.Count = count;
        }

        public override string ToString()
        {
            return $"{Word} {Count}";
        }
    }

    internal class Utils
    {
        internal enum Lang
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
    }

    internal class TrigramProcessor
    {
        private Dictionary<string, WordFreqPair> wordDic;
        private Dictionary<string, Trigram> trigramDic;

        internal Utils.Lang CurrentLang { get; set; }

        internal TrigramProcessor()
        {
            CurrentLang = Utils.Lang.English;
            wordDic = new Dictionary<string, WordFreqPair>(1000000);
            trigramDic = new Dictionary<string, Trigram>(1000000);
        }

        internal void readWordListFile(string wordListFilePath)
        {
            wordDic.Clear();

            string allTextStr = File.ReadAllText(wordListFilePath);
            string[] lines = allTextStr.Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim() == "")
                    continue;
                string[] items = line.Split(' ');
                string word = items[0].ToLower();
                int count = Int32.Parse(items[1]);
                if (!wordDic.ContainsKey(word))
                    wordDic.Add(word, new WordFreqPair(word, count));
            }
            Console.WriteLine($"WordList loaded into memory, count: {wordDic.Count}");
        }

        internal void processText(string[] inputTextFilePath)
        {
            trigramDic.Clear();

            foreach (var pathStr in inputTextFilePath)
            {
                using (BinaryReader br = new BinaryReader(File.Open(pathStr, FileMode.Open)))
                {
                    int totalLength = (int)br.BaseStream.Length;    // Total length of the file

                    int maxNumBytesPerPart = 300 * 1024 * 1024; // 300MB
                    int numParts = totalLength / maxNumBytesPerPart;
                    int restBytes = totalLength % maxNumBytesPerPart;
                    byte[] bytes;
                    for (int i = 0; i < numParts; i++)
                    {
                        bytes = br.ReadBytes(maxNumBytesPerPart);
                        String textStr_300MB = System.Text.ASCIIEncoding.UTF8.GetString(bytes);
                        process_trig_300MB_String(textStr_300MB);
                    }
                    bytes = br.ReadBytes(maxNumBytesPerPart);
                    String textStrRest = System.Text.ASCIIEncoding.UTF8.GetString(bytes);
                    bytes = null;
                    process_trig_300MB_String(textStrRest);
                    textStrRest = null;
                }
            }

            Console.WriteLine($"Number of unique trigram in dictionary:{trigramDic.Count}");

            Console.WriteLine("Ordering trigrams....");
            // Trigram the dictionry.
            List <Trigram> trigramList = trigramDic.Values.ToList<Trigram>();
            trigramDic = null;
            trigramList = trigramList.OrderByDescending(val => val.Count).ToList();

            Console.WriteLine("Writing trigrams to file....");
            // Writes the dictionary ordered to file.
            StringBuilder strBuild = new StringBuilder(1000000);
            foreach (var trigram in trigramList)
            {
                strBuild.AppendLine(trigram.ToString());
            }
            // Writes in the directory of the first file, with the name of the first file with WordList appended.
            string pathOutputTrigramListFile = $"{inputTextFilePath[0]}_TrigramList_{trigramList.Count}_.txt";
            File.WriteAllText(pathOutputTrigramListFile, strBuild.ToString(), Encoding.UTF8);
        }

        private void process_trig_300MB_String(string allTextStr)
        {
            int maxNumCharPerPart = 1000000;
            int numParts = allTextStr.Length / maxNumCharPerPart;
            int numRestChar = allTextStr.Length % maxNumCharPerPart;
            int beginIndex;
            for (int i = 0; i < numParts; i++)
            {
                beginIndex = maxNumCharPerPart * i;
                findAndCountTrigrams(allTextStr, maxNumCharPerPart, beginIndex);
            }
            beginIndex = maxNumCharPerPart * numParts;
            findAndCountTrigrams(allTextStr, numRestChar, beginIndex);
        }

        private void findAndCountTrigrams(string allTextStr, int maxNumCharPerPart, int beginIndex)
        {
            string textPart = allTextStr.Substring(beginIndex, maxNumCharPerPart);
            MatchCollection mColl = null;
            switch (CurrentLang)
            {
                case Utils.Lang.English:
                    mColl = Utils.RegExCompWordEN.Matches(textPart);
                    break;
                case Utils.Lang.Portuguese:
                    mColl = Utils.RegExCompWordPT.Matches(textPart);
                    break;
            }

            string trigWordA = null;
            string trigWordB = null;
            string trigWordC = null;

            for (int i = 0; i < mColl.Count; i++)
            {
                Match match = mColl[i];
                string wordMatch = match.Value.ToLower();

                WordFreqPair wordFreqPair;
                if (!wordDic.TryGetValue(wordMatch, out wordFreqPair))
                    continue;
                // Note: It's importante to get the pointer to the word already allocated
                //       because the much lower and smaller objects will be allocated.
                string word = wordFreqPair.Word;

                if (i == 0)
                    trigWordA = word;
                else if (i == 1)
                    trigWordB = word;
                else if (i == 2)
                    trigWordC = word;
                else
                {
                    trigWordA = trigWordB;
                    trigWordB = trigWordC;
                    trigWordC = word;
                    Trigram trigram;
                    string trigramKey = $"{trigWordA}{trigWordB}{trigWordC}";
                    if (trigramDic.TryGetValue(trigramKey, out trigram))
                    {
                        trigram.incrementCount();
                    }
                    else
                    {
                        trigramDic.Add(trigramKey, new Trigram(trigWordA, trigWordB, trigWordC, 1));
                    }
                }
            }
        }

        internal static void test()
        {
            string wordListPath = @"D:\JNC_Corrector_ortografico\European_Parliament_Parallel_Corpus\pt-en\europarl-v7.pt-en.pt_WordList_158429_.txt";
            string[] inputTrigramTextPath = {
                @"D:\European_Parliament_Parallel_Corpus\pt-en\europarl-v7.pt-en.pt"
               /*  , @"D:\European_Parliament_Parallel_Corpus\pt-en\PT_small_file.txt" */
                                              /* , @"" */};
            TrigramProcessor trigramPro = new TrigramProcessor();
            trigramPro.CurrentLang = Utils.Lang.Portuguese;
            trigramPro.readWordListFile(wordListPath);
            trigramPro.processText(inputTrigramTextPath);
        }

    }

    internal class Trigram
    {
        internal string WordA { get; set; }
        internal string WordB { get; set; }
        internal string WordC { get; set; }
        internal int Count { get; set; }

        internal Trigram(string wordA, string wordB, string wordC, int count)
        {
            this.WordA = wordA;
            this.WordB = wordB;
            this.WordC = wordC;
            this.Count = count;
        }

        public override string ToString()
        {
            return $"{WordA} {WordB} {WordC} {Count} ";
        }

        internal void incrementCount()
        {
            Count++;
        }
    }

}
