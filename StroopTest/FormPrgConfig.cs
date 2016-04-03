﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StroopTest
{
    public partial class FormPrgConfig : Form
    {
        private string path;
        private string instrBoxText = "Escreva cada uma das intruções em linhas separadas.";
        private string hexPattern = "^#(([0-9a-fA-F]{2}){3}|([0-9a-fA-F]){3})$";
        StroopProgram programWrite;
        private List<Button> subDirectionList;
        private int subDirectionNumber = 0;
        private bool editMode;
        private string fontSize = "160";

        public FormPrgConfig(string dataFolderPath, bool editModeOn)
        {
            path = dataFolderPath;
            editMode = editModeOn;
            InitializeComponent();
            chooseExpoType.SelectedIndex = 0;
            subDirectionList = new List<Button>();
            subDirectionList.Add(subDirect1); subDirectionList.Add(subDirect2); subDirectionList.Add(subDirect3); subDirectionList.Add(subDirect4); subDirectionList.Add(subDirect5);
            for(int i = 0; i < subDirectionList.Count; i++)
            {
                subDirectionList[i].Enabled = false;
                if (i > 0) subDirectionList[i].Visible = false;
            }
            if(editModeOn == true)
            {
                editProgram();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(showSubsOn.Checked)
            {
                foreach (Button b in subDirectionList)
                {
                    b.Enabled = true;
                    b.Visible = true;
                }
                chooseColorSubs.Enabled = true; panel3.Enabled = true; panel3.BackColor = Color.Transparent; subDirect1.BackColor = Color.Transparent; // habilitar botões de posicao legenda
            }
            else
            {
                for (int i = 0; i < subDirectionList.Count; i++)
                {
                    subDirectionList[i].Enabled = false;
                    subDirectionList[i].BackColor = Color.LightGray;
                    if (i > 0) subDirectionList[i].Visible = false;
                }
                chooseColorSubs.Enabled = false; panel3.Enabled = false; panel3.BackColor = Color.LightGray;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (chooseExpoType.SelectedIndex)
            {
                case 0:
                    openWordList.Enabled = true;
                    openColorsList.Enabled = true;
                    openImgsList.Enabled = false;
                    numericUpDown1.Enabled = true;
                    checkBox1.Enabled = false;
                    break;
                case 1:
                    openWordList.Enabled = false;
                    openColorsList.Enabled = false;
                    openImgsList.Enabled = true;
                    numericUpDown1.Enabled = false;
                    checkBox1.Enabled = true;
                    break;
                case 2:
                    openWordList.Enabled = true;
                    openColorsList.Enabled = false;
                    openImgsList.Enabled = true;
                    numericUpDown1.Enabled = true;
                    checkBox1.Enabled = true;
                    break;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string colorCode = pickColor();
            chooseBackGColor.Text = colorCode;
            panel2.BackColor = ColorTranslator.FromHtml(colorCode);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string colorCode = pickColor();
            chooseColorSubs.Text = colorCode;
            panel3.BackColor = ColorTranslator.FromHtml(colorCode);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            openWordList.Text = openListFile();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            openColorsList.Text = openListFile();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            openColorsList.Text = openListFile();
        }

        private void editProgram()
        {
            StroopProgram program = new StroopProgram();
            FormDefine defineProgram = new FormDefine("Editar Programa: ", path + "/prg/", "prg");

            var result = defineProgram.ShowDialog();
            string programName = "error";

            try
            {
                if (result == DialogResult.OK)
                {
                    programName = defineProgram.ReturnValue;

                    program.readProgramFile(path + "/prg/" + programName + ".prg");

                    progName.Text = program.ProgramName;
                    numExpo.Value = program.NumExpositions;
                    timeExpo.Value = program.ExpositionTime;
                    if (program.ExpositionRandom) randExpoOn.Checked = true;
                    else randExpoOn.Checked = false;
                    timeInterval.Value = program.IntervalTime;
                    if (program.IntervalTimeRandom) randIntervalOn.Checked = true;
                    else randIntervalOn.Checked = false;

                    if (program.WordsListFile.ToLower() == "false")
                    {
                        openWordList.Enabled = false;
                    }
                    else
                    {
                        openWordList.Enabled = true;
                        openWordList.Text = program.WordsListFile;
                    }

                    if (program.ColorsListFile.ToLower() == "false")
                    {
                        openColorsList.Enabled = false;
                    }
                    else
                    {
                        openColorsList.Enabled = true;
                        openColorsList.Text = program.ColorsListFile;
                    }

                    if (program.BackgroundColor.ToLower() == "false")
                    {
                        chooseBackGColor.Text = "#FFFFFF";
                    }
                    else
                    {
                        panel2.BackColor = ColorTranslator.FromHtml(chooseBackGColor.Text);
                        chooseBackGColor.Text = program.BackgroundColor;
                    }

                    if (program.AudioCapture) captAudioOn.Checked = true;
                    else captAudioOn.Checked = false;

                    if (program.SubtitleShow) showSubsOn.Checked = true;
                    else showSubsOn.Checked = false;

                    if (program.SubtitleShow)
                    {
                        subDirectionNumber = program.SubtitlePlace;
                        selectSubDirectionNumber(subDirectionNumber);
                        if (program.SubtitleColor.ToLower() == "false")
                        {
                            chooseColorSubs.Text = program.SubtitleColor;
                            panel2.BackColor = ColorTranslator.FromHtml(chooseColorSubs.Text);
                        }
                        else
                        {
                            chooseColorSubs.Text = "escolher cor";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < subDirectionList.Count; i++) // Loop with for.
                        {
                            subDirectionList[i].Enabled = false;
                        }
                        subDirectionNumber = program.SubtitlePlace;
                        chooseColorSubs.Text = "escolher cor";
                    }

                    switch (program.ExpositionType)
                    {
                        case "txt":
                            chooseExpoType.SelectedIndex = 0;
                            break;
                        case "img":
                            chooseExpoType.SelectedIndex = 1;
                            break;
                        case "imgtxt":
                            chooseExpoType.SelectedIndex = 2;
                            break;
                        default:
                            chooseExpoType.SelectedIndex = 0;
                            break;
                    }

                    if (program.ImagesListFile.ToLower() != "false") { openImgsList.Enabled = true; openImgsList.Text = program.ImagesListFile; }
                    else { openImgsList.Enabled = false; openImgsList.Text = "false"; }

                    if (program.FixPoint == "+")
                    {
                        fixPointCross.Checked = true;
                        fixPointCircle.Checked = false;
                    }
                    else
                    {
                        if (program.FixPoint == "o")
                        {
                            fixPointCross.Checked = false;
                            fixPointCircle.Checked = true;
                        }
                        else
                        {
                            fixPointCross.Checked = false;
                            fixPointCircle.Checked = false;
                        }
                    }

                    if (program.InstructionText != null) // lê instrução se houver
                    {
                        textBox2.ForeColor = Color.Black;
                        textBox2.Text = program.InstructionText[0];
                        for (int i = 1; i < program.InstructionText.Count; i++)
                        {
                            textBox2.AppendText(Environment.NewLine + program.InstructionText[i]);
                        }
                    }
                    else
                    {
                        textBox2.Text = instrBoxText;
                    }
                }
                else
                {

                    Close();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            programWrite = new StroopProgram();

            try
            {
                programWrite.ProgramName = progName.Text;
                programWrite.NumExpositions = Convert.ToInt32(numExpo.Value);
                programWrite.ExpositionTime = Convert.ToInt32(timeExpo.Value);
                programWrite.ExpositionRandom = randExpoOn.Checked;
                programWrite.IntervalTime = Convert.ToInt32(timeInterval.Value);
                programWrite.IntervalTimeRandom = randIntervalOn.Checked;

                if (openWordList.Enabled && openWordList.Text != "error") { programWrite.WordsListFile = openWordList.Text; }
                else
                {
                    if (openWordList.Text == "error") { throw new Exception("Selecione o arquivo de lista de palavras!"); }
                    programWrite.WordsListFile = "false";
                }

                if (openColorsList.Enabled && openWordList.Text != "error") { programWrite.ColorsListFile = openColorsList.Text; }
                else
                {
                    if (openWordList.Text == "error") { throw new Exception("Selecione o arquivo de lista de cores!"); }
                    programWrite.ColorsListFile = "false";
                }
                
                if (Regex.IsMatch(chooseBackGColor.Text, hexPattern)) programWrite.BackgroundColor = chooseBackGColor.Text;
                else programWrite.BackgroundColor = "false";

                programWrite.AudioCapture = captAudioOn.Checked;
                programWrite.SubtitleShow = showSubsOn.Checked;

                if(programWrite.SubtitleShow)
                {
                    programWrite.SubtitlePlace = subDirectionNumber;
                    if (Regex.IsMatch(chooseColorSubs.Text, hexPattern)) programWrite.SubtitleColor = chooseColorSubs.Text;
                    else programWrite.SubtitleColor = "false";
                }
                else
                {
                    programWrite.SubtitlePlace = subDirectionNumber;
                    programWrite.SubtitleColor = "false";
                }

                switch(chooseExpoType.SelectedIndex)
                {
                    case 0:
                        programWrite.ExpositionType = "txt";
                        break;
                    case 1:
                        programWrite.ExpositionType = "img";
                        break;
                    case 2:
                        programWrite.ExpositionType = "imgtxt";
                        break;
                }
                
                if (openImgsList.Enabled) { programWrite.ImagesListFile = openImgsList.Text; }
                else { programWrite.ImagesListFile = "false"; }
                
                if (fixPointCross.Checked)
                {
                    programWrite.FixPoint = "+";
                }
                else
                {
                    if (fixPointCircle.Checked)
                    {
                        programWrite.FixPoint = "o";
                    }
                    if(!fixPointCross.Checked && !fixPointCircle.Checked)
                    {
                        programWrite.FixPoint = "false";
                    }
                }

                string textLines = "";
                if (textBox2.Lines.Length > 0 && textBox2.Text != instrBoxText) // lê instrução se houver
                {
                    for (int i = 0; i < textBox2.Lines.Length; i++)
                    {
                        programWrite.InstructionText.Add(textBox2.Lines[i]);
                        textLines = textLines + "\n" + textBox2.Lines[i];
                    }
                }
                else
                {
                    programWrite.InstructionText = null;
                }

                programWrite.FontWordLabel = numericUpDown1.Value.ToString();
                programWrite.ExpandImage = checkBox1.Checked;

                string text =    programWrite.ProgramName + " " +
                                 programWrite.NumExpositions.ToString() + " " +
                                 programWrite.ExpositionTime.ToString() + " " +
                                 programWrite.ExpositionRandom.ToString() + " " +
                                 programWrite.IntervalTime.ToString() + " " +
                                 programWrite.IntervalTimeRandom.ToString() + " " +
                                 programWrite.WordsListFile + " " +
                                 programWrite.ColorsListFile + " " +
                                 programWrite.BackgroundColor.ToUpper() + " " +
                                 programWrite.AudioCapture.ToString() + " " +
                                 programWrite.SubtitleShow.ToString() + " " +
                                 programWrite.SubtitlePlace.ToString() + " " +
                                 programWrite.SubtitleColor.ToUpper() + " " +
                                 programWrite.ExpositionType.ToLower() + " " +
                                 programWrite.ImagesListFile + " " +
                                 programWrite.FixPoint + " " +
                                 programWrite.FontWordLabel + " " +
                                 programWrite.ExpandImage
                                 ;

                
                saveProgramFile(text, programWrite.InstructionText);
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }       
        }

        private void saveProgramFile(string programText, List<string> instructions)
        {
            if (File.Exists(path + "prg/" + progName.Text + ".prg"))
            {
                DialogResult dialogResult = MessageBox.Show("O programa já existe, deseja sobre-escrevê-lo?", "Audio", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                {
                    throw new Exception("Escrita de programa cancelada!");
                }
            }

            StreamWriter writer = new StreamWriter(path + "prg/" + progName.Text + ".prg");
            writer.WriteLine(programText);
            if (instructions != null)
            {
                for (int i = 0; i < instructions.Count; i++)
                {
                    writer.WriteLine(instructions[i]);
                }
            }
            //writer.Dispose();
            writer.Close();
            MessageBox.Show("Programa salvo no diretório:\n" + path + "/prg/");
            this.Close();
        }

        private string openListFile()
        {
            string progName = "error";

            FormDefine defineProgram = new FormDefine("Lista: ", path + "/lst/", "lst");
            var result = defineProgram.ShowDialog();
            if (result == DialogResult.OK)
            {
                progName = defineProgram.ReturnValue + ".lst";
            }

            /*
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            StroopProgram programOpened = new StroopProgram();
            string nameListFile = "error";

            openFileDialog1.InitialDirectory = path + "/lst/";
            openFileDialog1.Filter = "Arquivos de lista (*.lst)|*.lst";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                nameListFile = Path.GetFileName(openFileDialog1.FileName);
            
            return nameListFile;
            */
            return progName;
        }

        string pickColor()
        {
            ColorDialog MyDialog = new ColorDialog();
            Color colorPicked = new Color();

            MyDialog.CustomColors = new int[] {
                                        ColorTranslator.ToOle(ColorTranslator.FromHtml("#F8E000")),
                                        ColorTranslator.ToOle(ColorTranslator.FromHtml("#007BB7")),
                                        ColorTranslator.ToOle(ColorTranslator.FromHtml("#7EC845")),
                                        ColorTranslator.ToOle(ColorTranslator.FromHtml("#D01C1F"))
                                      };
            colorPicked = this.BackColor;

            string hexColor = "#FFFFFF";

            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                colorPicked = MyDialog.Color;
                hexColor = "#" + colorPicked.R.ToString("X2") + colorPicked.G.ToString("X2") + colorPicked.B.ToString("X2");
            }
                
            return  hexColor;
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.ForeColor = Color.Black;
            if(textBox2.Text == "Escreva cada uma das intruções em linhas separadas.")
                textBox2.Text = "";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(fixPointCross.Checked && fixPointCircle.Checked)
                fixPointCircle.Checked = !fixPointCross.Checked;
        }

        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (fixPointCross.Checked && fixPointCircle.Checked)
                fixPointCross.Checked = !fixPointCircle.Checked;
        }

        private void subDirect1_Click(object sender, EventArgs e)
        {
            selectSubDirectionNumber(1);
        }

        private void subDirect2_Click(object sender, EventArgs e)
        {
            selectSubDirectionNumber(2);
        }

        private void subDirect3_Click(object sender, EventArgs e)
        {
            selectSubDirectionNumber(3);
        }

        private void subDirect4_Click(object sender, EventArgs e)
        {
            selectSubDirectionNumber(4);
        }

        private void subDirect5_Click(object sender, EventArgs e)
        {
            selectSubDirectionNumber(5);
        }

        private void selectSubDirectionNumber(int number)
        {
            for (int i = 0; i < subDirectionList.Count; i++) // Loop with for.
            {
                subDirectionList[i].BackColor = Color.LightGray;
            }
            subDirectionList[number - 1].BackColor = Color.Transparent;
            subDirectionNumber = number;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string colorCode = pickColor();
            button1.Text = colorCode;
            panel4.BackColor = ColorTranslator.FromHtml(colorCode);
        }
        
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            fontSize = numericUpDown1.Value.ToString();
        }
    }
}
