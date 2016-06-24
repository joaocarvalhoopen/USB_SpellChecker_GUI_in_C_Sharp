using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;

namespace SpellChecker_GUI
{
    internal class TextSeparator
    {
        // The caracter '-' isn't included because it can be used to make single word in Portuguese, but can also be in the beginning of a word where it is pontuation.
        // It has dosuble meaning. 
        private static readonly HashSet<Char> defaultPontuationCharPT = new HashSet<Char> { ',', '.', ':', ';',
                     '|', '\\', '!', '\"', '@', '#', '£', '$', '§', '%', '&', '/', '{', '}', '(', ')', '[', ']', '=',
                     '?', '\'', '»', '«', '*', '+', '¨', '´', '`', 'º', 'ª', '^', '~', '<', '>', '_',
                     '0','1','2','3','4','5','6','7','8','9', };

        // The caracter "'" isn't included because it can be used to make single word in English.
        private static readonly HashSet<Char> defaultPontuationCharEN = new HashSet<Char> { ',', '.', ':', ';',
                     '|', '\\', '!', '\"', '@', '#', '£', '$', '§', '%', '&', '/', '{', '}', '(', ')', '[', ']', '=',
                     '?', '»', '«', '*', '+', '¨', '´', '`', 'º', 'ª', '^', '~', '<', '>', '_', '-',
                     '0','1','2','3','4','5','6','7','8','9', };

        private static readonly HashSet<Char> defaultWhiteSpaceCharPT = new HashSet<Char> { ' ', '\t', };
        private static readonly HashSet<Char> defaultWhiteSpaceCharEN = new HashSet<Char> { ' ', '\t', };
        private static readonly HashSet<Char> defaultBreakLineCharPT = new HashSet<Char> { '\n', '\r', };
        private static readonly HashSet<Char> defaultBreakLineCharEN = new HashSet<Char> { '\n', '\r', };

        private static readonly HashSet<Char> defaultAcentuacaoCharPT = new HashSet<Char> { 'ç', 'ã', 'õ', 'â', 'ê', 'ô', 'à', 'á', 'é', 'í', 'ó', 'ú', };

        private List<TextSegment> textList;

        internal Utils.Language currentLang { get; set; }

        internal TextSeparator(Utils.Language lang)
        {
            currentLang = lang;
            textList = new List<TextSegment>();
        }

        internal List<TextSegment> getTextSegmentList()
        {
            return textList;
        }

        internal void textToWords(string inputText)
        {
            switch (currentLang)
            {
                case Utils.Language.English:
                    textToWordsRegExEN(inputText);
                    break;

                case Utils.Language.Portuguese:
                    textToWordsRegExPT(inputText);
                    break;

                default:
                    throw new Exception(" Error: Language not supported (" + currentLang.ToString() + ")");
            }
        }

        /////////
        // General regular expressions.
        private static string regExStrWhiteSpace = @"[ \t]+";
        private static Regex regExCompWhiteSpace = new Regex(regExStrWhiteSpace, RegexOptions.Compiled);

        private static string regExStrLineBreak = @"[\n\r]+";
        private static Regex regExCompLineBreak = new Regex(regExStrLineBreak, RegexOptions.Compiled);

        /*
        /////////
        // PT regular expressions pre-compilation.

        // Regular expression to detect words in the Portuguese language:
        // [A-Zãõàáéíóúâêôç]+([-][A-Zãõàáéíóúâêôç]+)+|[A-Zãõàáéíóúâêôç]+
        private static readonly string lettersClassForWordPatternPT = @"[a-zãõàáéíóúâêôç]";
        private static string regExStrWordPatternPT = String.Format(@"{0}+([-]{0}+)+|{0}+", lettersClassForWordPatternPT);
        private static Regex regExCompWordPatternPT = new Regex(regExStrWordPatternPT, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        */

        // We can put the caracter '-' in this reguler expression of pontuation because the word reguler expressions is tested before the pontuation reguler espression. 
        private static string regExStrPontuationPT = @"[,\.:;\|\\!""@#£\$§%&/\{\}\(\)\[\]=\?'»«\*\+¨´`ºª\^~<>_\-0123456789]+";
        private static Regex regExCompPontuationPT = new Regex(regExStrPontuationPT, RegexOptions.Compiled);
        private static Regex regExCompPontuationEN = new Regex(regExStrPontuationPT, RegexOptions.Compiled);

        public void textToWordsRegExPT(string inputText)
        {
            textList.Clear();
            Match m;
            for (int i = 0; i < inputText.Length;)
            {
                m = Utils.RegExCompWordPT.Match(inputText, i);
                if (m.Success && m.Index == i)
                {
                    int endSegIndex = m.Index + m.Length;
                    textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.Word, m.Index, endSegIndex, m.Length));
                    i = endSegIndex;
                }
                else
                {
                    m = regExCompWhiteSpace.Match(inputText, i);
                    if (m.Success && m.Index == i)
                    {
                        int endSegIndex = m.Index + m.Length;
                        textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.WhiteSpace, m.Index, endSegIndex, m.Length));
                        i = endSegIndex;
                    }
                    else
                    {
                        m = regExCompPontuationPT.Match(inputText, i);
                        if (m.Success && m.Index == i)
                        {
                            int endSegIndex = m.Index + m.Length;
                            textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.Pontuation, m.Index, endSegIndex, m.Length));
                            i = endSegIndex;
                        }
                        else
                        {
                            m = regExCompLineBreak.Match(inputText, i);
                            if (m.Success && m.Index == i)
                            {
                                int endSegIndex = m.Index + m.Length;
                                textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.LineBreak, m.Index, endSegIndex, m.Length));
                                i = endSegIndex;
                            }
                            else
                            {
                                textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.UnknownType, i, i + 1, 1));
                                i++;
                            }
                        }
                    }
                }
            }
        }

        private void textToWordsRegExEN(string inputText)
        {
            textList.Clear();
            Match m;
            for (int i = 0; i < inputText.Length;)
            {
                m = Utils.RegExCompWordEN.Match(inputText, i);
                if (m.Success && m.Index == i)
                {
                    int endSegIndex = m.Index + m.Length;
                    textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.Word, m.Index, endSegIndex, m.Length));
                    i = endSegIndex;
                }
                else
                {
                    m = regExCompWhiteSpace.Match(inputText, i);
                    if (m.Success && m.Index == i)
                    {
                        int endSegIndex = m.Index + m.Length;
                        textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.WhiteSpace, m.Index, endSegIndex, m.Length));
                        i = endSegIndex;
                    }
                    else
                    {                        
                        m = regExCompPontuationEN.Match(inputText, i);
                        if (m.Success && m.Index == i)
                        {
                            int endSegIndex = m.Index + m.Length;
                            textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.Pontuation, m.Index, endSegIndex, m.Length));
                            i = endSegIndex;
                        }
                        else
                        {
                            m = regExCompLineBreak.Match(inputText, i);
                            if (m.Success && m.Index == i)
                            {
                                int endSegIndex = m.Index + m.Length;
                                textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.LineBreak, m.Index, endSegIndex, m.Length));
                                i = endSegIndex;
                            }
                            else
                            {
                                textList.Add(new TextSegment(currentLang, m.Value, TextSegment.Type.UnknownType, i, i + 1, 1));
                                i++;
                            }
                        }
                    }
                }
            }
        }

        private bool joinSequencialTextSegments(bool createNewList, out List<TextSegment> newList)
        {
            newList = null;
            if (createNewList)
                newList = new List<TextSegment>();
            bool existsSegmentsTojoin = false;
            bool inJoin = false;
            TextSegment firstTextSegm = null;
            TextSegment textSegm = null;
            StringBuilder strBuilText = new StringBuilder();
            for (int i = 0; i < this.textList.Count; i++)
            {
                textSegm = this.textList[i];
                if (firstTextSegm == null)
                {
                    firstTextSegm = textSegm;
                    strBuilText.Append(textSegm.text);
                }
                else if (firstTextSegm != null)
                {
                    if (firstTextSegm.segmentLangU == textSegm.segmentLangU
                        && firstTextSegm.segmentType == textSegm.segmentType)
                    {
                        inJoin = true;
                        existsSegmentsTojoin = true;
                        strBuilText.Append(textSegm.text);
                    }
                    else
                    {
                        inJoin = false;
                        if (createNewList)
                        {
                            int wordLength = textSegm.startIndex - firstTextSegm.startIndex;
                            textList.Add(new TextSegment(firstTextSegm.segmentLangU, strBuilText.ToString(), firstTextSegm.segmentType,
                                firstTextSegm.startIndex, textSegm.startIndex, wordLength));
                        }
                        //strBuilText.Clear();
                        strBuilText.Remove(0, strBuilText.Length);
                        strBuilText.Append(textSegm.text);
                        firstTextSegm = textSegm;
                    }
                }
            }

            if (inJoin == true)
            {
                inJoin = false;
                if (createNewList)
                {
                    int wordLength = textSegm.startIndex - firstTextSegm.startIndex;
                    textList.Add(new TextSegment(firstTextSegm.segmentLangU, strBuilText.ToString(), firstTextSegm.segmentType,
                        firstTextSegm.startIndex, textSegm.startIndex, wordLength));
                }
                //strBuilText.Clear();
                strBuilText.Remove(0, strBuilText.Length);
            }

            return existsSegmentsTojoin;
        }

        internal void recalculateAllIndexs()
        {
            int lastIndex = 0;
            foreach(var textSeg in textList)
            {
                textSeg.startIndex = lastIndex;
                lastIndex = lastIndex + textSeg.length;
            }
        }

        internal string toStringCompressed()
        {
            StringBuilder str = new StringBuilder();
            foreach (var textSegm in this.textList)
            {
                str.Append(textSegm.ToStringCompressed());
            }
            return str.ToString();
        }

        internal string toStringCompressedWords()
        {
            StringBuilder str = new StringBuilder();
            foreach (var textSegm in this.textList)
            {
                str.Append(textSegm.ToStringCompressedWords());
            }
            return str.ToString();
        }

        internal string toCompleteTextString()
        {
            StringBuilder str = new StringBuilder();
            foreach (var textSegm in this.textList)
            {
                str.Append(textSegm.text);
            }
            return str.ToString();
        }

        internal int countNumSpellErrors()
        {
            int counter = 0;
            foreach (var textSegm in this.textList)
            {
                if (textSegm.stateType == TextSegment.StateType.SpellError)
                    counter++;
            }
            return counter;
        }

    }

    internal class TextSegment
    {
        internal enum Type
        {
            Word,
            WhiteSpace,
            Pontuation,
            LineBreak,
            UnknownType,
        }

        internal enum StateType
        {
            NotProcessed,
            SpellError,
            CorrectWord,
            CorrectWordNotInWordList,
        }

        internal Utils.Language segmentLangU { get; set; }
        internal string text { get; set; }
        internal Type segmentType { get; set; }
        internal int startIndex { get; set; }
        internal int endIndex { get; set; }
        internal int length { get; set; }
        internal StateType stateType { get; set; }


        internal TextSegment(Utils.Language lang, string text, TextSegment.Type segmentType, int startIndex, int endIndex, int length)
        {
            this.segmentLangU = lang;
            this.text = text;
            this.segmentType = segmentType;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.length = length;
            this.stateType = StateType.NotProcessed;

            if (endIndex - startIndex != length)
                throw new Exception("Error: Invalid textSegment indexs or length ( " + ToString() + " )");
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append($"{nameof(segmentLangU)}: {segmentLangU}");
            str.Append($"{nameof(text)}: {text}");
            str.Append($"{nameof(segmentType)}: {segmentType}");
            str.Append($"{nameof(startIndex)}: {startIndex}");
            str.Append($"{nameof(endIndex)}: {endIndex}");
            str.Append($"{nameof(length)}: {length}");
            str.Append($"{nameof(stateType)}: {stateType}");
            return str.ToString();
        }

        public string ToStringCompressed()
        {
            char ch = 'X';
            switch (segmentType)
            {
                case Type.Word:
                    ch = 'W';
                    break;
                case Type.WhiteSpace:
                    ch = 'S';
                    break;
                case Type.Pontuation:
                    ch = 'P';
                    break;
                case Type.LineBreak:
                    ch = 'B';
                    break;
                case Type.UnknownType:
                    ch = 'U';
                    break;
            }
            return timesString(ch, length);
        }

        public string ToStringCompressedWords()
        {
            if (segmentType == Type.Word)
                return text;
            else
                return timesString('_', length);
        }

        private string timesString(char ch, int numTimes)
        {
            if (numTimes <= 0)
                throw new ArgumentException($"{nameof(numTimes)}: {numTimes} has to be greater then 0.");
            StringBuilder str = new StringBuilder();
            str.Append(ch);
            for (int i = 0; i < numTimes - 1; i++)
            {
                if (ch == '_')
                    str.Append('_');
                else
                    str.Append('^');
            }
            return str.ToString();
        }

    }


    internal class Tests_TextSeparator
    {
        private StringBuilder strLog;

        internal string testsLog
        {
            get
            {
                return strLog.ToString();
            }
        }

        internal Tests_TextSeparator()
        {
            // Todo: Implement.
            strLog = new StringBuilder();
        }

        internal bool runTests()
        {
            bool allTestPassed = true;

            // Filled with tests.
            allTestPassed = test_001() ? allTestPassed : false;

            // TODO: Add more tests....

            strLog.AppendLine();
            string mesStr = allTestPassed ? "Result: All tests passed successfully!" :
                                         "Result: Some tests failled!";
            strLog.AppendLine(mesStr);
            return allTestPassed;
        }

        private bool test_001()
        {
            bool result = true;

            int testNum = 0;
            string inputStr = "";
            string testStr_ = "";
            string testWStr = "";

            // Empty string.
            testNum = 0;
            inputStr = "";
            testStr_ = "";
            testWStr = "";
            result = textSeparatorTest(testNum, inputStr, testStr_, testWStr) == true ? result : false;

            // Single letter.
            testNum = 1;
            inputStr = "A";
            testStr_ = "W";
            testWStr = "A";
            result = textSeparatorTest(testNum, inputStr, testStr_, testWStr) == true ? result : false;

            // Single pontuation.
            testNum = 2;
            inputStr = "#";
            testStr_ = "P";
            testWStr = "_";
            result = textSeparatorTest(testNum, inputStr, testStr_, testWStr) == true ? result : false;

            // Small word.
            testNum = 3;
            inputStr = "Batata";
            testStr_ = "W^^^^^";
            testWStr = "Batata";
            result = textSeparatorTest(testNum, inputStr, testStr_, testWStr) == true ? result : false;

            // Line of test.
            testNum = 4;
            inputStr = "Batatas, com couves à caçador!\n";
            testStr_ = "W^^^^^^PSW^^SW^^^^^SWSW^^^^^^PB";
            testWStr = "Batatas__com_couves_à_caçador__";
            result = textSeparatorTest(testNum, inputStr, testStr_, testWStr) == true ? result : false;

            // Line of test with '-'.
            testNum = 5;
            inputStr = "-Batatas, com couves à caçadora! Saber-me-á, muito bem!\nA\n\rB.";
            testStr_ = "PW^^^^^^PSW^^SW^^^^^SWSW^^^^^^^PSW^^^^^^^^^PSW^^^^SW^^PBWB^WP";
            testWStr = "_Batatas__com_couves_à_caçadora__Saber-me-á__muito_bem__A__B_";
            result = textSeparatorTest(testNum, inputStr, testStr_, testWStr) == true ? result : false;

            // Line of test with '-'.
            testNum = 6;
            inputStr = "-Batatas, com- couves-- à caçadora! -Saber-me-á--, muito--- -- bem!\nA\n\rB.";
            testStr_ = "PW^^^^^^PSW^^PSW^^^^^P^SWSW^^^^^^^PSPW^^^^^^^^^P^^SW^^^^P^^SP^SW^^PBWB^WP";
            testWStr = "_Batatas__com__couves___à_caçadora___Saber-me-á____muito_______bem__A__B_";
            result = textSeparatorTest(testNum, inputStr, testStr_, testWStr) == true ? result : false;

            // Todo: Implement.

            return result;
        }

        private bool textSeparatorTest(int testNum, string inputStr, string testStr, string testWordsStr)
        {
            TextSeparator ts = new TextSeparator(Utils.Language.Portuguese);            
            ts.textToWords(inputStr);
            string outputStr = ts.toStringCompressed();
            string outputWordsStr = ts.toStringCompressedWords();

            strLog.AppendLine();
            strLog.AppendLine($"testNum    :{testNum}");
            strLog.AppendLine($"inputStr   :[{inputStr}]");
            strLog.AppendLine($"testStr    :[{testStr}]");
            strLog.AppendLine($"outputStr  :[{outputStr}]");
            strLog.AppendLine($"testWoStr  :[{testWordsStr}]");
            strLog.AppendLine($"outputWoStr:[{outputWordsStr}]");

            bool testResult = true;
            if (testStr != outputStr)
            {
                strLog.AppendLine("Result: FAILLED output != tested str");
                testResult = false;
            }
            else
            {
                strLog.AppendLine("Result: OK output == tested str");
            }
            if (testWordsStr != outputWordsStr)
            {
                strLog.AppendLine("Result: FAILLED outputWords != tested words str");
                testResult = false;
            }
            else
            {
                strLog.AppendLine("Result: OK outputWords == tested Words str");
            }
            return testResult;
        }

        private bool test_002()
        {
            // Todo: Implement.

            return false;
        }

    }

}
