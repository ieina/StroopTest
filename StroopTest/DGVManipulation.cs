﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StroopTest
{
    class DGVManipulation
    {
        internal static void moveDGVRowUp(DataGridView dgv)
        {
            int totalRows = dgv.Rows.Count;
            // get index of the row for the selected cell
            int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
            if (rowIndex == 0)
                return;
            // get index of the column for the selected cell
            int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
            DataGridViewRow selectedRow = dgv.Rows[rowIndex];
            dgv.Rows.Remove(selectedRow);
            dgv.Rows.Insert(rowIndex - 1, selectedRow);
            dgv.ClearSelection();
            dgv.Rows[rowIndex - 1].Cells[colIndex].Selected = true;
        }

        internal static void moveDGVRowDown(DataGridView dgv)
        {
            int totalRows = dgv.Rows.Count;
            // get index of the row for the selected cell
            int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
            if (rowIndex == totalRows - 1)
                return;
            // get index of the column for the selected cell
            int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
            DataGridViewRow selectedRow = dgv.Rows[rowIndex];
            dgv.Rows.Remove(selectedRow);
            dgv.Rows.Insert(rowIndex + 1, selectedRow);
            dgv.ClearSelection();
            dgv.Rows[rowIndex + 1].Cells[colIndex].Selected = true;
        }

        internal static void deleteDGVRow(DataGridView dgv)
        {
            if (dgv.RowCount > 1)
            {
                foreach (DataGridViewRow item in dgv.SelectedRows)
                {
                    dgv.Rows.RemoveAt(item.Index);
                }
            }
            else
            {
                dgv.Rows.Clear();
                dgv.Refresh();
            }
        }

        internal static void saveColumnToListFile(DataGridView dgv, int column, string path, string filename)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            try
            {
                saveFileDialog1.Filter = "List (.lst)|*.lst"; // salva em .lst
                saveFileDialog1.RestoreDirectory = true;

                if (string.IsNullOrWhiteSpace(filename))
                {
                    throw new Exception("Preencha o campo com o nome do arquivo!");
                }

                if (File.Exists(path + filename + ".lst"))
                {
                    DialogResult dr = MessageBox.Show("Uma lista com este nome já existe.\nDeseja sobrescrevê-la?", "", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.Cancel)
                    {
                        throw new Exception("A lista não será salva!");
                    }
                }

                StreamWriter w1 = new StreamWriter(path + filename + ".lst");
                for (int i = 0; i < dgv.RowCount; i++)
                {
                    w1.WriteLine(dgv.Rows[i].Cells[column].Value.ToString());
                }
                w1.Close();
                MessageBox.Show("A lista " + filename + ".lst foi salva com sucesso no diretório\n" + path);

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
