using Common;

namespace Practice3;

internal static class Program
{
    private const float M = 0.0f;
    private const float VariantNumber = 3.0f;
    private const string FilePath = @"E:\Projects\University\Multimedia\Course work\C#\Image\pes.jpg";

    private static void Main()
    {
        var imagePositionOffset = 300;
        var form = new Form();
        form.AutoScroll = true;
        var imagePositionY = 0;
        form.Size = new Size(1280, 720);
        
        var img = new Bitmap(FilePath);
        form.Controls.Add(CreateImagePanel(img, "Original"));
        imagePositionY += imagePositionOffset;
        
        var random = new Random();
        const float sigma = VariantNumber / 100;
        Console.WriteLine(random.NextDouble() * sigma + M);
        var additive = img.Transform(color =>
        {
            var r = Clamp(color.R + (int)(random.NextDouble() * sigma + M));
            var g = Clamp(color.G + (int)(random.NextDouble() * sigma + M));
            var b = Clamp(color.B + (int)(random.NextDouble() * sigma + M));
            return Color.FromArgb(r, g, b);
        });
        form.Controls.Add(CreateImagePanel(additive, "Additive noise", 0, imagePositionY));
        imagePositionY += imagePositionOffset;

        Console.WriteLine(1 + sigma * (random.NextDouble() - 0.5));
        var multiplicative = img.Transform(color =>
        {
            var noiseFactor = 1 + sigma * (random.NextDouble() - 0.5);
            var r = Clamp((int)(color.R * noiseFactor));
            var g = Clamp((int)(color.G * noiseFactor));
            var b = Clamp((int)(color.B * noiseFactor));
            return Color.FromArgb(r, g, b);
        });
        form.Controls.Add(CreateImagePanel(multiplicative, "Multiplicative noise", 0, imagePositionY));
        imagePositionY += imagePositionOffset;

        var density = 0.1 + MathF.Abs(VariantNumber - 14) / 100;
        var saltAndPepper = img.AddSaltAndPepperNoise(density);
        form.Controls.Add(CreateImagePanel(saltAndPepper, "Salt & Pepper", 0, imagePositionY));
        imagePositionY += imagePositionOffset;

        int[] windowSizes = [3, 5, 7, 11];
        foreach (var windowSize in windowSizes)
        {
            form.Controls.Add(MeanFilterImage(additive, windowSize, $"Additive noise mean {windowSize}x{windowSize}",
                imagePositionY));
            imagePositionY += imagePositionOffset;
            form.Controls.Add(MeanFilterImage(multiplicative, windowSize,
                $"Multiplicative noise mean {windowSize}x{windowSize}", imagePositionY));
            imagePositionY += imagePositionOffset;
            form.Controls.Add(MeanFilterImage(saltAndPepper, windowSize,
                $"Salt & Pepper noise mean {windowSize}x{windowSize}", imagePositionY));
            imagePositionY += imagePositionOffset;
        }
        
        foreach (var windowSize in windowSizes)
        {
            form.Controls.Add(MedianFilterImage(additive, windowSize, $"Additive noise median {windowSize}x{windowSize}",
                imagePositionY));
            imagePositionY += imagePositionOffset;
            form.Controls.Add(MedianFilterImage(multiplicative, windowSize,
                $"Multiplicative noise median {windowSize}x{windowSize}", imagePositionY));
            imagePositionY += imagePositionOffset;
            form.Controls.Add(MedianFilterImage(saltAndPepper, windowSize,
                $"Salt & Pepper noise median {windowSize}x{windowSize}", imagePositionY));
            imagePositionY += imagePositionOffset;
        }
        
        Application.Run(form);
    }

    private static Panel MeanFilterImage(Bitmap image, int windowSize, string text, int position)
    {
        var result = image.ApplyMeanFilter(windowSize);
        return CreateImagePanel(result, text, 0, position);
    }
    private static Panel MedianFilterImage(Bitmap image, int windowSize, string text, int position)
    {
        var result = image.ApplyMedianFilter(windowSize);
        return CreateImagePanel(result, text, 0, position);
    }
    
    private static int Clamp(int value)
    {
        return Math.Max(0, Math.Min(255, value));
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