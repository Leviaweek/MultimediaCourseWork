namespace Multimedia;

internal static class Program
{
    private const string ImagePath = @"E:\Projects\University\Practice1\Course work\C#\Image\pes.jpg";
    private const byte L = 255;
    private const byte Low = 20;
    private const byte High = 160;
    private const double Gamma = 1.2d;
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var image = new Bitmap(ImagePath);
        
        var grayImage = image.ToGrayscale();
        
        var iMin = grayImage.GetIMin();
        var iMax = grayImage.GetIMax();

        var transformedImage = grayImage.ToTransformedGrayscale(Gamma, Low, High, iMin, iMax);

        var inverted = grayImage.ToNegative(iMax);

        var k = (iMax - iMin) / (double)L;
        
        var form = new Form
        {
            Text = "First practice",
            ClientSize = new Size(1280, 720),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen
        };
        
        var originalPanel = CreateImagePanel(image, "Original Image", 10, 10);
        var grayPanel = CreateImagePanel(grayImage, $"Grayscale Image K = {k:F1}", 300, 10);
        var transformedPanel = CreateImagePanel(transformedImage, $"Transformed Image (low={Low}, high={High}, γ={Gamma})", 10, 300);
        var invertedPanel = CreateImagePanel(inverted, $"Inverted Image Imax = {iMax:F1}, Imin = ({iMin:F1}, K = {k:F1})", 300, 300);
        form.Controls.Add(originalPanel);
        form.Controls.Add(grayPanel);
        form.Controls.Add(transformedPanel);
        form.Controls.Add(invertedPanel);
        form.ShowDialog();
    }

    private static Panel CreateImagePanel(Bitmap image, string text, int x = 0, int y = 0)
    {
        var panel = new Panel
        {
            Controls =
            {
                new Label
                {
                    Text = text,
                    Location = new Point(0, 0),
                    Size = new Size(image.Width + 50, 25),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Arial", 7, FontStyle.Bold)
                },
                new PictureBox
                {
                    Image = image,
                    SizeMode = PictureBoxSizeMode.Normal,
                    Size = new Size(image.Width, image.Height),
                    Location = new Point(5, 25),
                }
            },
            Size = new Size(300, 280),
            Location = new Point(x, y),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Margin = new Padding(10)
        };
        return panel;
    }
}