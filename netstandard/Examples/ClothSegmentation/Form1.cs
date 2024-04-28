using ClothONNX;
using System.Drawing.Imaging;
using UMapx.Core;
using UMapx.Imaging;

namespace ClothSegmentation
{
    public partial class Form1 : Form
    {
        private readonly ClothSegmentator _clothSegmentator;

        public Form1()
        {
            InitializeComponent();

            BackgroundImageLayout = ImageLayout.Zoom;
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            AllowDrop = true;
            Text = "ClothONNX: Cloth Segmentation";

            _clothSegmentator = new ClothSegmentator(ClothSegmentatorQuality.Medium);
            var image = new Bitmap("example.png");
            Process(image);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var file = ((string[])e.Data.GetData(DataFormats.FileDrop, true))[0];
            var image = new Bitmap(file);
            Process(image);
            Cursor = Cursors.Default;
        }

        private void Process(Bitmap image)
        {
            var results = _clothSegmentator.Forward(
                image: image,
                interpolationMode: InterpolationMode.Bicubic);

            using var mask = results.FromGrayscale();
            var maskColorFilter = new MaskColorFilter(Color.FromArgb(192, Color.Yellow));
            maskColorFilter.Apply(image, mask);

            //image.Save("output.png", ImageFormat.Png);

            BackgroundImage?.Dispose();
            BackgroundImage = image;
        }

    }
}