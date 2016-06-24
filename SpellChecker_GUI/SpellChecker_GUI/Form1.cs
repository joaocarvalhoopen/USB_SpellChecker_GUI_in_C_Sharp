using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpellChecker_GUI
{
    public partial class MainForm : Form
    {
        private string allTextStr;
        private Utils.Language currentLanguage;
        private TextSeparator textSeparator;
        private List<TextSegment> textSegmentList;
        private TweakedPeterNorvigSpellChecker spellChecker;
        private bool existErrors;
        private TextSegment currentTextSeg;

        public MainForm()
        {
            InitializeComponent();

            this.startButton.Enabled = true;
            enableSpellCheckControls(false);
            allTextStr = this.richTextBox.Text;
            currentLanguage = Utils.Language.Portuguese;
            spellChecker = new TweakedPeterNorvigSpellChecker(currentLanguage);
            spellChecker.CurrentLang = Utils.Language.Portuguese;
            // Load the frequency word lists in Portuguese and English.
            // They are in the same directory of the executable assembly.
            string pt_wordListFilePath = @".\europarl-v7.pt-en.pt_WordList_158429_.txt";
            bool withFreq = true;
            spellChecker.trainFromWordListFile(Utils.Language.Portuguese, pt_wordListFilePath, withFreq);
            string pt_wordListWithOutFreqFilePath = @".\ProjNatura-Wordlist-big-20160406_UTF8.txt";
            withFreq = false;
            spellChecker.trainFromWordListFile(Utils.Language.Portuguese, pt_wordListWithOutFreqFilePath, withFreq);
            string en_wordListFilePath = @".\europarl-v7.pt-en.en_WordList_81676_EuroParlament_with_freq_UN.txt";
            withFreq = true;
            spellChecker.trainFromWordListFile(Utils.Language.English, en_wordListFilePath, withFreq);
            existErrors = false;
            this.langComboBox.Items.Add(nameof(Utils.Language.Portuguese));
            this.langComboBox.SelectedItem = this.langComboBox.Items[0];
            this.langComboBox.Items.Add(nameof(Utils.Language.English));
        }

        private void enableSpellCheckControls(bool state)
        {
            this.label1.Enabled = state;
            this.NumOfErrorsLabel.Enabled = state;
            this.upButton.Enabled = state;
            this.rightButton.Enabled = state;
            this.downButton.Enabled = state;
            this.leftButton.Enabled = state;
            this.acceptWordButton.Enabled = state;
            this.acceptedTextBox.Enabled = state;
            this.sugestedListBox.Enabled = state;            
            this.StopButton.Enabled = state;
            // This ones are inverted.
            this.langComboBox.Enabled = !state;
            this.selectAllButton.Enabled = !state;
            this.copyButton.Enabled = !state;
            this.pasteButton.Enabled = !state;
            this.biggerFontButton.Enabled = !state;
            this.smallerFontButton.Enabled = !state;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            existErrors = false;
            if (this.richTextBox.Text.Length == 0)
                return;
            this.richTextBox.ReadOnly = true;
            allTextStr = this.richTextBox.Text;
            
            // Process text string (Tokenise).
            textSeparator = new TextSeparator(currentLanguage);
            textSeparator.textToWords(allTextStr);
            textSegmentList = textSeparator.getTextSegmentList();
            // Spell check all text.
            TextSegment firstErrorTextSeg = null;
            foreach (var textSeg in textSegmentList)
            {
                if (textSeg.segmentType != TextSegment.Type.Word)
                    continue;
                if (!spellChecker.wordExists(textSeg.text))
                {
                    if (firstErrorTextSeg == null)
                    {
                        firstErrorTextSeg = textSeg;
                        existErrors = true;
                    }
                    textSeg.stateType = TextSegment.StateType.SpellError;                    
                }
            }
            if (existErrors)
            {
                // Mark the errors in the text in a different color.
                currentTextSeg = firstErrorTextSeg;
                correctWord(firstErrorTextSeg);
                
                this.startButton.Enabled = false;
                enableSpellCheckControls(true);

                this.NumOfErrorsLabel.Text = textSeparator.countNumSpellErrors().ToString();
            }
            else
                this.richTextBox.ReadOnly = false;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            this.richTextBox.Text = textSeparator.toCompleteTextString();

            enableSpellCheckControls(false);
            this.startButton.Enabled = true;
            this.richTextBox.ReadOnly = false;
            // To unmark the colored errors and paint all text in black.
            colorSegmentRichTextControl(Color.Black, 0, this.richTextBox.Text.Length);
            //this.richTextBox.Text = this.richTextBox.Text;
            this.acceptedTextBox.Text = "";
            this.sugestedListBox.Items.Clear();
            this.NumOfErrorsLabel.Text = "0";
        }

        private void correctWord(TextSegment textSeg)
        {
            colorAllRichTextControl(textSeg);
            this.acceptedTextBox.Text = textSeg.text;
            fillSuggestionsList(textSeg.text);
        }

        private void correctWordColloringDelta(TextSegment currentTextSeg, TextSegment oldTextSeg)
        {
            // Note: This is a optimization for changing between words on long texts strings. 
            // Convert old text segment color from Green to Blue.
            if (oldTextSeg.stateType == TextSegment.StateType.SpellError)
                colorSegmentRichTextControl(Color.Blue, oldTextSeg.startIndex, oldTextSeg.length);
            else
                colorSegmentRichTextControl(Color.Black, oldTextSeg.startIndex, oldTextSeg.length);
            // Convert old text segment color from Blue to Green.
            colorSegmentRichTextControl(Color.Green, currentTextSeg.startIndex, currentTextSeg.length);
            this.acceptedTextBox.Text = currentTextSeg.text;
            // The previous line makes a change to the text of the acceptedTextBox and that automatically
            // calls acceptedTextBox_TextChanged(), inside it is a call to fillSuggestionsList().
            
            // fillSuggestionsList(currentTextSeg.text);
        }

        private void colorAllRichTextControl(TextSegment currentSegToCorrect)
        {
            foreach (var textSeg in textSegmentList)
            {
                switch (textSeg.segmentType)
                {
                    case TextSegment.Type.Word:
                        if (textSeg == currentSegToCorrect)
                            colorSegmentRichTextControl(Color.Green, textSeg.startIndex, textSeg.length);
                        else if (textSeg.stateType == TextSegment.StateType.SpellError)
                            colorSegmentRichTextControl(Color.Blue, textSeg.startIndex, textSeg.length);
                        else if (textSeg.stateType == TextSegment.StateType.CorrectWordNotInWordList)
                            colorSegmentRichTextControl(Color.Cyan, textSeg.startIndex, textSeg.length);
                        else
                            colorSegmentRichTextControl(Color.Black, textSeg.startIndex, textSeg.length);
                        break;
                    case TextSegment.Type.WhiteSpace:
                    case TextSegment.Type.Pontuation:
                    case TextSegment.Type.LineBreak:
                    case TextSegment.Type.UnknownType:
                        colorSegmentRichTextControl(Color.Black, textSeg.startIndex, textSeg.length);
                        break;
                    default:
                        break;
                }
            }
            // Put the text that is correcting visible.
            if (currentSegToCorrect != null)
            {
                richTextBox.Select(currentSegToCorrect.startIndex, 0);
                richTextBox.ScrollToCaret();
            }
        }

        private void colorSegmentRichTextControl(Color color, int startIndex, int length)
        {
            richTextBox.SelectionStart = startIndex;
            richTextBox.SelectionLength = length;
            richTextBox.SelectionColor = color;

            richTextBox.SelectionStart = 0;
            richTextBox.SelectionLength = 0;
            richTextBox.SelectionColor = Color.Black;
        }

        private void sugestedListBox_Click(object sender, EventArgs e)
        {
            this.acceptedTextBox.Text = this.sugestedListBox.SelectedItem.ToString();
        }

        private void acceptWordButton_Click(object sender, EventArgs e)
        {
            // Change the word in the internal representation of the text.
            string correctedWord = this.acceptedTextBox.Text;
            currentTextSeg.text = correctedWord;
            currentTextSeg.length = correctedWord.Length;
            currentTextSeg.stateType = TextSegment.StateType.CorrectWord;
            textSeparator.recalculateAllIndexs();

            this.richTextBox.Text = textSeparator.toCompleteTextString();
            this.NumOfErrorsLabel.Text = textSeparator.countNumSpellErrors().ToString();

            this.acceptedTextBox.Text = "";
            this.sugestedListBox.Items.Clear();
            // If there are more errors then pass to the next error. 
            if (!goToRightError(currentTextSeg))
            {
                this.acceptedTextBox.Enabled = false;
                this.sugestedListBox.Enabled = false;
                this.acceptWordButton.Enabled = false;
                colorAllRichTextControl(null);
            }
            else
                colorAllRichTextControl(currentTextSeg);
        }

        private bool goToLeftError(TextSegment oldTextSeg)
        {
            int index = textSegmentList.IndexOf(oldTextSeg);
            for (int i = index - 1; i >= 0; i--)
            {
                TextSegment textSeg = textSegmentList[i];
                if (textSeg.stateType == TextSegment.StateType.SpellError)
                {
                    currentTextSeg = textSeg;
                    correctWordColloringDelta(currentTextSeg, oldTextSeg);                    
                    return true;
                }
            }
            return false;
        }

        private bool goToRightError(TextSegment oldTextSeg)
        {
            int index = textSegmentList.IndexOf(oldTextSeg);
            for (int i = index + 1; i < textSegmentList.Count; i++)
            {
                TextSegment textSeg = textSegmentList[i];
                if (textSeg.stateType == TextSegment.StateType.SpellError)
                {
                    currentTextSeg = textSeg;
                    correctWordColloringDelta(currentTextSeg, oldTextSeg);
                    return true;
                }
            }
            return false;
        }

        private bool goToUpError(TextSegment oldTextSeg)
        {
            int index = textSegmentList.IndexOf(oldTextSeg);
            bool beforeLineBreak = false;
            for (int i = index - 1; i >= 0; i--)
            {
                TextSegment textSeg = textSegmentList[i];
                if (textSeg.segmentType == TextSegment.Type.LineBreak)
                    beforeLineBreak = true;
                if (beforeLineBreak)
                {
                    if (textSeg.stateType == TextSegment.StateType.SpellError)
                    {
                        currentTextSeg = textSeg;
                        correctWordColloringDelta(currentTextSeg, oldTextSeg);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool goToDownError(TextSegment oldTextSeg)
        {
            int index = textSegmentList.IndexOf(oldTextSeg);
            bool afterLineBreak = false;
            for (int i = index + 1; i < textSegmentList.Count; i++)
            {
                TextSegment textSeg = textSegmentList[i];
                if (textSeg.segmentType == TextSegment.Type.LineBreak)
                    afterLineBreak = true;
                if (afterLineBreak)
                {
                    if (textSeg.stateType == TextSegment.StateType.SpellError)
                    {
                        currentTextSeg = textSeg;
                        correctWordColloringDelta(currentTextSeg, oldTextSeg);
                        return true;
                    }
                }
            }
            return false;
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            goToLeftError(currentTextSeg);
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            goToRightError(currentTextSeg);
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            goToUpError(currentTextSeg);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            goToDownError(currentTextSeg);
        }

        private void acceptedTextBox_TextChanged(object sender, EventArgs e)
        {
            string newWord = this.acceptedTextBox.Text;
            if (!this.sugestedListBox.Items.Contains(newWord))
                fillSuggestionsList(newWord);
        }

        private void fillSuggestionsList(string newWord)
        {
            bool wordIsCorrect;
            List<WordFreq> listWordFreq = spellChecker.correctV2(newWord, out wordIsCorrect);
            this.sugestedListBox.Items.Clear();
            foreach (var item in listWordFreq)
            {
                this.sugestedListBox.Items.Add(item.Word);
            }
        }

        private void langComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)(this.langComboBox.SelectedItem))
            {
                case nameof(Utils.Language.English):
                    currentLanguage = Utils.Language.English;
                    spellChecker.CurrentLang = currentLanguage;
                    break;
                case nameof(Utils.Language.Portuguese):
                    currentLanguage = Utils.Language.Portuguese;
                    spellChecker.CurrentLang = currentLanguage;
                    break;
                default:
                    break;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, int dwExtraInfo);

        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            this.richTextBox.SelectAll();
            // 1 - Save current cursor position above the button.
            // 2 - Create a point 20, 20 in application coordenates, that corresponde to the control where i want to make the rigth mouse click.
            // 3 - Convert the coordenates of the point from app coordenates to screen coordenates.
            // 4 - Move the cursor position inside .net. 
            // 5 - Call the user32.dll mouse_event function, with position 0, 0, because the position doesn't move the cursor.
            // 6 - Move the cursor position to the old position. This way the cursor doesn't appear to have moved.  
            uint X = 0; 
            uint Y = 0;
            Point oldPosition = Cursor.Position;
            Cursor.Position = this.richTextBox.PointToScreen(new Point(20, 20));
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
            Cursor.Position = oldPosition;
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            this.richTextBox.Copy();
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            this.richTextBox.Paste();
        }

        private void biggerFontButton_Click(object sender, EventArgs e)
        {
            Font oldFont = this.richTextBox.Font;
            this.richTextBox.Font = new Font(oldFont.FontFamily, oldFont.Size + 2);
        }

        private void smallerFontButton_Click(object sender, EventArgs e)
        {
            Font oldFont = this.richTextBox.Font;
            if (oldFont.Size >= 3)
                this.richTextBox.Font = new Font(oldFont.FontFamily, oldFont.Size - 2);
        }

    }
}
