using System.Drawing;

namespace MG.Dashboard.Controller.Camera;

internal interface ICamera
{
    Task<Bitmap> TakePictureAsync(CancellationToken cancellationToken = default);

    Task<IObservable<Bitmap>> StartStreamAsync(CancellationToken cancellationToken = default);

    Task StopStreamAsync(CancellationToken cancellationToken = default);
}
