using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NHunspell;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Hunspell spellChecker;
        private ContextMenuStrip contextMenu;

        public Form1()
        {
            InitializeComponent();
            LoadFonts();
            numericUpDown1.Value = 12;

            // Initialize spell checker with dictionary files.
            string affPath = "en_GB.aff"; // Make sure these files are in your output directory.
            string dicPath = "en_GB.dic";
            spellChecker = new Hunspell(affPath, dicPath);

            // Initialize context menu for suggestions.
            contextMenu = new ContextMenuStrip();
            richTextBox1.MouseDown += RichTextBox1_MouseDown;

            // Subscribe to RichTextBox text change event.
            richTextBox1.TextChanged += RichTextBox1_TextChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("pop");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Save your text"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChanges();
        }

        private void LoadFonts()
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            FontFamily[] fontFamilies = installedFontCollection.Families;

            foreach (FontFamily font in fontFamilies)
            {
                comboBox1.Items.Add(font.Name);
            }

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            LoadChanges();
        }

        private void LoadChanges()
        {
            float fontSize = 12;
            if (float.TryParse(numericUpDown1.Value.ToString(), out fontSize))
            {
                if (comboBox1.SelectedItem != null)
                {
                    string selectedFont = comboBox1.SelectedItem.ToString();
                    if (fontSize > 0) { richTextBox1.Font = new Font(selectedFont, fontSize); }
                }
                else
                {
                    MessageBox.Show("Please select a font from the dropdown.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid font size. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            PerformSpellCheck();
        }

        private void PerformSpellCheck()
        {
            int caretPosition = richTextBox1.SelectionStart;
            string[] words = richTextBox1.Text.Split(new char[] { ' ', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;

            foreach (string word in words)
            {
                if (!spellChecker.Spell(word))
                {
                    int start = richTextBox1.Text.IndexOf(word, StringComparison.OrdinalIgnoreCase);
                    if (start >= 0)
                    {
                        richTextBox1.Select(start, word.Length);
                        richTextBox1.SelectionColor = Color.Red;
                    }
                }
            }

            for (int i = 0; i < richTextBox1.Text.Length - 1; i++)
            {
                char currentChar = richTextBox1.Text[i];
                char nextChar = richTextBox1.Text[i + 1];

                if (currentChar == ' ' && nextChar == ',')
                {
                    richTextBox1.Select(i, 2);
                    richTextBox1.SelectionColor = Color.Blue;
                }
                else if (currentChar == ',' && nextChar != ' ')
                {
                    richTextBox1.Select(i, 2);
                    richTextBox1.SelectionColor = Color.Blue;
                }
            }

            richTextBox1.SelectionStart = caretPosition;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = Color.Black;
        }

        private void RichTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int clickIndex = richTextBox1.GetCharIndexFromPosition(e.Location);
                string word = GetWordAtIndex(clickIndex);
                
                // Find the start of the word
                int wordStart = clickIndex;
                while (wordStart > 0 && char.IsLetter(richTextBox1.Text[wordStart - 1]))
                {
                    wordStart--;
                }

                contextMenu.Items.Clear();

                if (!string.IsNullOrEmpty(word))
                {
                    if (!spellChecker.Spell(word))
                    {
                        List<string> suggestions = spellChecker.Suggest(word);
                        foreach (string suggestion in suggestions)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem(suggestion);
                            item.Click += (s, args) => ReplaceWord(wordStart, word.Length, suggestion);
                            contextMenu.Items.Add(item);
                        }

                        if (suggestions.Count == 0)
                        {
                            contextMenu.Items.Add(new ToolStripMenuItem("No suggestions") { Enabled = false });
                        }
                    }
                }

                string punctuationIssue = GetPunctuationIssue(clickIndex);
                if (!string.IsNullOrEmpty(punctuationIssue))
                {
                    ToolStripMenuItem fixPunctuation = new ToolStripMenuItem(punctuationIssue);
                    fixPunctuation.Click += (s, args) => FixPunctuation(clickIndex);
                    contextMenu.Items.Add(fixPunctuation);
                }

                if (contextMenu.Items.Count > 0)
                    contextMenu.Show(richTextBox1, e.Location);
            }
        }

        private string GetWordAtIndex(int index)
        {
            int start = index;
            int end = index;

            while (start > 0 && !char.IsWhiteSpace(richTextBox1.Text[start - 1]))
                start--;

            while (end < richTextBox1.Text.Length && !char.IsWhiteSpace(richTextBox1.Text[end]))
                end++;

            return richTextBox1.Text.Substring(start, end - start);
        }

        private void ReplaceWord(int index, int length, string suggestion)
        {
            richTextBox1.Select(index, length);
            richTextBox1.SelectedText = suggestion;
        }

        private string GetPunctuationIssue(int index)
        {
            if (index < 0 || index >= richTextBox1.Text.Length)
                return null;

            char currentChar = richTextBox1.Text[index];
            if (currentChar == ',' && index > 0 && richTextBox1.Text[index - 1] == ' ')
                return "Remove space before comma";
            if (currentChar == ',' && index < richTextBox1.Text.Length - 1 && richTextBox1.Text[index + 1] != ' ')
                return "Add space after comma";

            return null;
        }

        private void FixPunctuation(int index)
        {
            if (index < 0 || index >= richTextBox1.Text.Length)
                return;

            char currentChar = richTextBox1.Text[index];

            if (currentChar == ',' && index > 0 && richTextBox1.Text[index - 1] == ' ')
            {
                richTextBox1.Select(index - 1, 1);
                richTextBox1.SelectedText = string.Empty;
            }
            if (currentChar == ',' && index < richTextBox1.Text.Length - 1 && richTextBox1.Text[index + 1] != ' ')
            {
                richTextBox1.Select(index + 1, 0);
                richTextBox1.SelectedText = " ";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RunServer();
        }

        private async void RunServer()
        {
            try
            {
                // Create a txt file in the bin path with what is in the text box
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt");
                File.WriteAllText(filePath, richTextBox1.Text);
                
                // Start the Python script
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = "python", 
                    Arguments = "Server.py",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(start))
                {
                    // Read standard output
                    string result = await process.StandardOutput.ReadToEndAsync();
                    // Split the result into sentences
                    string[] sentences = result.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    // Create a form to display sentences for editing
                    Form editForm = new Form();
                    DataGridView dataGridView = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
                    dataGridView.Columns.Add("Sentence", "Sentence");
                    
                    // Add a checkbox column for confirmation
                    DataGridViewCheckBoxColumn confirmColumn = new DataGridViewCheckBoxColumn
                    {
                        Name = "Confirm",
                        HeaderText = "Confirm",
                        Width = 50,
                        ReadOnly = false // Allow editing
                    };
                    dataGridView.Columns.Add(confirmColumn);

                    // Store original sentences
                    string[] originalSentences = richTextBox1.Text.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    // Ensure both arrays have the same length
                    int minLength = Math.Min(originalSentences.Length, sentences.Length);

                    // Add sentences to DataGridView
                    for (int i = 0; i < minLength; i++)
                    {
                        dataGridView.Rows.Add(sentences[i].Trim() + ".", false);
                    }

                    editForm.Controls.Add(dataGridView);
                    Button confirmButton = new Button { Text = "Confirm Changes", Dock = DockStyle.Bottom };
                    editForm.Controls.Add(confirmButton);
                    confirmButton.Click += (s, args) =>
                    {
                        richTextBox1.Clear();
                        for (int i = 0; i < minLength; i++)
                        {
                            if (Convert.ToBoolean(dataGridView.Rows[i].Cells["Confirm"].Value))
                            {
                                richTextBox1.AppendText(dataGridView.Rows[i].Cells["Sentence"].Value.ToString() + " ");
                            }
                            else
                            {
                                // Replace with original sentence if not confirmed
                                richTextBox1.AppendText(originalSentences[i].Trim() + ". ");
                            }
                        }
                        editForm.Close();
                    };

                    editForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
