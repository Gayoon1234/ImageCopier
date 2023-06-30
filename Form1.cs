using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageCopier
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private string pathFrom;
        private string pathTo;
        int index;
        private bool isLoaded = false;
        private string[] images;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void mainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnFrom_Click(object sender, EventArgs e)
        {
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbFrom.Text = fbd.SelectedPath;
            }
        }

        private void btnTo_Click(object sender, EventArgs e)
        {
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbTo.Text = fbd.SelectedPath;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(tbFrom.Text) && Directory.Exists(tbTo.Text))
            {
                pathFrom = tbFrom.Text;
                pathTo = tbTo.Text;
                images = findImages(pathFrom);
                lblImageCount.Text = "/" + images.Length;
                setImageTo(0);
                isLoaded = true;
            }
            else 
            {
                MessageBox.Show("Directory not valid");
            }
        }

        private string[] findImages(string directoryPath) {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            string[] imageFilePaths = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            imageFilePaths = Array.FindAll(imageFilePaths, path => IsImageFile(path, imageExtensions));

            return imageFilePaths;
        }

        private static bool IsImageFile(string filePath, string[] imageExtensions)
        {
            string extension = Path.GetExtension(filePath);
            return Array.Exists(imageExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
        private void setImageTo(int i) {
            pbImage.ImageLocation = images[i];
            lblImageName.Text = Path.GetFileName(images[i]);
            tbImageNum.Text = (i+1).ToString();
            index = i;
        }

        private void tbImageNum_Leave(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                int newIndex;
                bool tryParse = int.TryParse(tbImageNum.Text, out newIndex);
                if (tryParse)
                {
                    setImageTo(newIndex - 1);
                }
            }
        }

        private void nextImage() { 
            setImageTo((index+1)%images.Length);
        }

        private void prevImage()
        {
            setImageTo((index-1+images.Length)%images.Length);
        }

        private void copyImage() {
            try
            {
                File.Copy(images[index], Path.Combine(pathTo, Path.GetFileName(images[index])));
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while copying the file: " + ex.Message);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            nextImage();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            prevImage();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            copyImage();
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                prevImage();
            }
            else if (e.KeyCode == Keys.Right)
            {
                nextImage();
            }
            else if (e.KeyCode == Keys.Space) 
            {
                copyImage();
            }
        }

       
    }
}
