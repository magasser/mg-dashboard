using System.Drawing;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Ports;

namespace MG.Dashboard.Controller.Camera;

internal sealed class PiCamera : ICamera, IDisposable
{
    private static readonly TimeSpan CameraWarmupTime = TimeSpan.FromSeconds(2);

    private readonly MMALCamera _camera;
    private readonly CompositeDisposable _handlerDisposables;

    private CancellationTokenSource? _streamTokenSource;

    public PiCamera()
    {
        _camera = MMALCamera.Instance;

        _handlerDisposables = new CompositeDisposable();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _streamTokenSource?.Cancel();
        _streamTokenSource?.Dispose();
        _handlerDisposables.Dispose();
        _camera.Cleanup();
    }

    public Task<Bitmap> TakePictureAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async Task<IObservable<Bitmap>> StartStreamAsync(CancellationToken cancellationToken = default)
    {
        var captureHandler = new BitmapStreamCaptureHandler();
        var splitter = new MMALSplitterComponent();
        var imgEncoder = new MMALImageEncoder(continuousCapture: true);
        var nullSink = new MMALNullSinkComponent();

        _camera.ConfigureCameraSettings();

        var portCOnfig = new MMALPortConfig(
            encodingType: MMALEncoding.JPEG,
            pixelFormat: MMALEncoding.I420,
            quality: 90);

        imgEncoder.ConfigureOutputPort(portCOnfig, captureHandler);

        _camera.Camera.VideoPort.ConnectTo(splitter);
        splitter.Outputs[0].ConnectTo(imgEncoder);
        _camera.Camera.PreviewPort.ConnectTo(nullSink);

        await Task.Delay(CameraWarmupTime).ConfigureAwait(false);

        _streamTokenSource = new CancellationTokenSource();

        _ = _camera.ProcessAsync(_camera.Camera.VideoPort, _streamTokenSource.Token);

        _handlerDisposables.Add(captureHandler);
        _handlerDisposables.Add(splitter);
        _handlerDisposables.Add(imgEncoder);
        _handlerDisposables.Add(nullSink);

        return captureHandler.BitmapStream;
    }

    public Task StopStreamAsync(CancellationToken cancellationToken = default)
    {
        _streamTokenSource?.Cancel();

        _handlerDisposables.Clear();

        return Task.CompletedTask;
    }

    private sealed class BitmapStreamCaptureHandler : InMemoryCaptureHandler
    {
        private readonly ISubject<Bitmap> _bitmapStreamSubject;

        public BitmapStreamCaptureHandler()
        {
            _bitmapStreamSubject = new Subject<Bitmap>();

            BitmapStream = _bitmapStreamSubject.AsObservable();
        }

        public IObservable<Bitmap> BitmapStream { get; }

        public override void Process(ImageContext context)
        {
            base.Process(context);

            if (context.Eos)
            {
                using (var stream = new MemoryStream(WorkingData.ToArray()))
                {
                    var bitmap = new Bitmap(stream);

                    _bitmapStreamSubject.OnNext(bitmap);
                }

                WorkingData.Clear();
            }
        }

        public override void Dispose()
        {
            _bitmapStreamSubject.OnCompleted();

            base.Dispose();
        }
    }
}
