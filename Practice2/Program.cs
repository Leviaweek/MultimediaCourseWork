using Common;
using MathNet.Numerics.LinearAlgebra;
using ScottPlot;
using ScottPlot.Panels;
using ScottPlot.Statistics;
using ScottPlot.WinForms;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using Label = System.Windows.Forms.Label;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Practice2;

internal static class Program
{
    private const string FilePath = @"E:\Projects\University\Multimedia\Course work\C#\Image\pes.jpg";
    private static void Main()
    {
        var form = new Form();
        form.AutoScroll = true;
        var img = new Bitmap(FilePath);
        var grey = img.Transform(color => (byte)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B), 
            color => Color.FromArgb(color, color, color));

        form.Controls.Add(CreateImagePanel(grey, "Original grey image", 0, 5800));

        var distension = FirstStage(grey, form);

        var equalized = SecondStage(distension, grey, form);

        ThirdStage(equalized, form);

        Application.Run(form);
    }

    private static Bitmap FirstStage(Bitmap grey, Form form)
    {
        var intensities = grey.GetFlatten().ToArray();
        var formsPlots = new FormsPlot
        {
            Location = new Point(0, 0)
        };
        formsPlots.Plot.Axes.AddPanel(new TitlePanel
        {
            Label =
            {
                Text = "Original histogram"
            }
        });
        form.Controls.Add(formsPlots);
        formsPlots.Size = new Size(800, 600);
        var histogram1 = new Histogram(0, intensities.Max(), 10);
        histogram1.AddRange(intensities);

        formsPlots.Plot.Add.Bars(HistogramToBars(histogram1.Bins, histogram1.Counts, histogram1.BinSize));
        formsPlots.Plot.Axes.AutoScale();
        var cum = Vector<double>.Build.Dense(histogram1.GetCumulative());

        var normCum = cum / (histogram1.Counts.Max() / cum.Max());
        var formPlots2 = new FormsPlot
        {
            Location = new Point(0, 700),
            Size = new Size(800, 600)
        };
        form.Controls.Add(formPlots2);
        formPlots2.Plot.Add.Scatter(Enumerable.Range(0,
                        normCum.Count)
                    .ToArray(),
                (normCum / normCum.Max()).ToArray())
            .MarkerSize = 0;
        var iMin = grey.GetIMin();
        var iMax = grey.GetIMax();
        var k = iMax - iMin;
        var distension = grey.Transform(color =>
            Color.FromArgb(color.R - iMin,
                color.G - iMin,
                color.B - iMin), color => color).Transform(color => (byte)(color.R * (255 / k)), 
            color => Color.FromArgb(color, color, color));
        formPlots2.Plot.Axes.AutoScale();
        formPlots2.Plot.Axes.AddPanel(new TitlePanel
        {
            Label =
            {
                Text = "Original Cumulative function"
            }
        });
        form.Controls.Add(CreateImagePanel(distension, $"Distension Imax = {iMax} Imin = {iMin} K = {k}", 0, 1400));
        return distension;
    }

    private static Bitmap SecondStage(Bitmap distension, Bitmap grey, Form form)
    {
        var formPlots1 = new FormsPlot
        {
            Location = new Point(0, 2300),
            Size = new Size(800, 600)
        };
        form.Controls.Add(formPlots1);
        formPlots1.Plot.Axes.AutoScale();
        formPlots1.Plot.Axes.AddPanel(new TitlePanel
        {
            Label =
            {
                Text = "Distension histogram"
            }
        });

        var intensities = distension.GetFlatten().ToArray();
        var histogram = new Histogram(0, intensities.Max(), 256);
        histogram.AddRange(intensities);

        formPlots1.Plot.Add.Bars(HistogramToBars(histogram.Bins, histogram.Counts, histogram.BinSize));
        var cum = Vector<double>.Build.Dense(histogram.GetCumulative());
        var normCum = cum * (histogram.Counts.Max() / cum.Max());
        var formsPlot2 = new FormsPlot
        {
            Location = new Point(0, 3000),
            Size = new Size(800, 600)
        };
        form.Controls.Add(formsPlot2);
        formsPlot2.Plot.Add.Scatter(Enumerable.Range(0, normCum.Count).ToArray(),
            (normCum / normCum.Max()).ToArray()).MarkerSize = 0;
        formsPlot2.Plot.Axes.AutoScale();
        formsPlot2.Plot.Axes.AddPanel(new TitlePanel
        {
            Label =
            {
                Text = "Distension Cumulative function"
            }
        });
        var cumMax = cum.Max();
        var cumMin = cum.Min();
        var cumSum = cum.Select(x => x - cumMin * CustomMath.Step(x, 0))
            .Select(x => x * (255 / (cumMax - cumMin) * CustomMath.Step(x, 0)))
            .ToArray();
        var equalized = grey.Transform(color => (byte)cumSum[color.R],
            color => Color.FromArgb(color, color, color));
        var iMax = equalized.GetIMax();
        var iMin = equalized.GetIMin();
        var k = iMax - iMin;
        form.Controls.Add(CreateImagePanel(equalized, $"Equalization, Imax = {iMax}, Imin = {iMin}, K = {k}", 0, 3700));
        return equalized;
    }

    private static void ThirdStage(Bitmap equalized, Form form)
    {
        var intensities3 = equalized.GetFlatten().ToArray();
        var histogram3 = new Histogram(0, intensities3.Max(), 256);
        histogram3.AddRange(intensities3);
        var formsPlot1 = new FormsPlot
        {
            Location = new Point(0, 4400),
            Size = new Size(800, 600)
        };
        formsPlot1.Plot.Add.Bars(HistogramToBars(histogram3.Bins, histogram3.Counts, histogram3.BinSize));
        form.Controls.Add(formsPlot1);
        formsPlot1.Plot.Axes.AutoScale();
        formsPlot1.Plot.Axes.AddPanel(new TitlePanel
        {
            Label =
            {
                Text = "Equalization histogram"
            }
        });
        var cum3 = Vector<double>.Build.Dense(histogram3.GetCumulative());
        var normCum3 = cum3 * (histogram3.Counts.Max() / cum3.Max());
        var formsPlot2 = new FormsPlot()
        {
            Location = new Point(0, 5100),
            Size = new Size(800, 600)
        };
        form.Controls.Add(formsPlot2);
        formsPlot2.Plot.Add.Scatter(Enumerable.Range(0,
                        normCum3.Count)
                    .ToArray(),
                (normCum3 / normCum3.Max()).ToArray())
            .MarkerSize = 0;
        formsPlot2.Plot.Axes.AutoScale();
        formsPlot2.Plot.Axes.AddPanel(new TitlePanel
        {
            Label =
            {
                Text = "Distension Cumulative function"
            }
        });
    }

    private static List<Bar> HistogramToBars(double[] bins, double[] counts, double binSize)
    {
        return counts.Select((t,
                i) => new Bar
            {
                Position = bins[i],
                Value = t,
                FillColor = Colors.Blue,
                Size = binSize
            })
            .ToList();
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