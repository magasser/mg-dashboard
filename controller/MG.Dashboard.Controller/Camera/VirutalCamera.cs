using System.Drawing;
using System.Reactive.Linq;

namespace MG.Dashboard.Controller.Camera;

internal sealed class VirutalCamera : ICamera
{
    private static readonly int FrameRate = 30;
    private static readonly string VirutalImageFile = Path.Combine("Resources", "Placeholder.jpeg");

    private readonly Bitmap _virtualImage;

    public VirutalCamera()
    {
        using (var stream = File.Open(VirutalImageFile, FileMode.Open))
        {
            var image = Image.FromStream(stream);

            _virtualImage = new Bitmap(image);
        }
    }

    /// <inheritdoc />
    public Task<Bitmap> TakePictureAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_virtualImage);

    /// <inheritdoc />
    public Task<IObservable<Bitmap>> StartStreamAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Observable.Interval(TimeSpan.FromSeconds(1) / FrameRate).Select(_ => _virtualImage));

    /// <inheritdoc />
    public Task StopStreamAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
