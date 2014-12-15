using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageMagick;
using Microsoft.Win32;

namespace ImageCopyrighter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The filename of the source file we're manipulating
        /// </summary>
        private string sourceFilename;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++) FontSizeBox.Items.Add(i*2);
            FontSizeBox.Items.Add(64);
            FontSizeBox.Items.Add(72);
            FontSizeBox.Items.Add(144);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Image Files (*.jpg, *.png, *.tiff, *.bmp)|*.jpg;*.png;*.tiff;*.bmp|All Files (*.*)|*.*",
                Title = "Open File",
                CheckFileExists = true
            };
            if(!(dialog.ShowDialog() ?? false)) return;

            sourceFilename = dialog.FileName;
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(dialog.FileName);
            image.EndInit();
            ImagePreview.Source = image;
            ImagePreview.Height = 100;

            CopyrightTextBox.Focus();
            UpdateSavebuttonEnablement();
        }

        private void CopyrightTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSavebuttonEnablement();
        }

        private void UpdateSavebuttonEnablement()
        {
            SaveButton.IsEnabled = CopyrightTextBox.Text.Trim().Any() && sourceFilename != null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "Image Files (*.jpg, *.png, *.tiff, *.bmp)|*.jpg;*.png;*.tiff;*.bmp|All Files (*.*)|*.*",
                Title = "Save File",
                FileName = sourceFilename.Split('.').First() + "_copyrighted." + sourceFilename.Split('.').Last()
            };
            if (!(dialog.ShowDialog() ?? false)) return;

            AddTextToImage(sourceFilename, CopyrightTextBox.Text, dialog.FileName);
            MessageBox.Show("Image was saved successfully.");
        }

        private void AddTextToImage(string sourceFilename, string text, string targetFilename)
        {
            //var magickImage = new MagickImage(sourceFilename);
            //magickImage.FontPointsize = int.Parse(FontSizeBox.Text);
            //magickImage.Annotate(text, Gravity.Southwest);
            //magickImage.Write(targetFilename);

            using (var magickImage = new MagickImage(sourceFilename))
            using (var captionImage = new MagickImage(new MagickColor("#FFFFFF"), magickImage.Width, 0))
            {
                captionImage.FontPointsize = int.Parse(FontSizeBox.Text);
                captionImage.Read(string.Format("caption:{0}", text));
                using(var result = new MagickImageCollection(new[] {magickImage, captionImage}).AppendVertically()) result.Write(targetFilename);
            }
        }

        private void FontSizeBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
