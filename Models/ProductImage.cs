using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace demomalcevworkofmistake.Models;

public partial class Product
{
    private Bitmap? _imageBitmap;

    public Bitmap ImageBitmap
    {
        get
        {
            if (_imageBitmap != null)
                return _imageBitmap;

            var defaultUri = new Uri(
                "avares://demomalcevworkofmistake/Assets/product/picture.png");

            try
            {
                if (string.IsNullOrWhiteSpace(Image))
                    return _imageBitmap = new Bitmap(
                        AssetLoader.Open(defaultUri));

                var uri = new Uri(
                    $"avares://demomalcevworkofmistake/Assets/product/{Image.Trim()}");

                return _imageBitmap = new Bitmap(
                    AssetLoader.Open(uri));
            }
            catch
            {
                return _imageBitmap = new Bitmap(
                    AssetLoader.Open(defaultUri));
            }
        }
    }
}   