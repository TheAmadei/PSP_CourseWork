using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.Direct2D1.Factory;
using WriteFactory = SharpDX.DirectWrite.Factory;

namespace Controller
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Direct2DComponent : DirectXComponent
    {
        private Factory factory2D;
        private WriteFactory factoryDWrite;
        private RenderTarget renderTarget2D;

        /// <summary>
        /// Объект 2D рендера.
        /// </summary>
        protected RenderTarget RenderTarget2D => renderTarget2D;
        /// <summary>
        /// Внутренняя инициализация.
        /// </summary>
        protected override void InternalInitialize()
        {
            base.InternalInitialize();

            factory2D = new Factory();
            factoryDWrite = new WriteFactory();

            using (var surface = BackBuffer.QueryInterface<Surface>())
            {
                renderTarget2D = new RenderTarget(factory2D, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
            }
            //_renderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
        }
        /// <summary>
        /// Внутренняя инициализация.
        /// </summary>
        protected override void InternalUninitialize()
        {
            Utilities.Dispose(ref renderTarget2D);
            Utilities.Dispose(ref factory2D);
            Utilities.Dispose(ref factoryDWrite);

            base.InternalUninitialize();
        }
        /// <summary>
        /// Начало рендера.
        /// </summary>
        protected override void BeginRender()
        {
            base.BeginRender();

            renderTarget2D.BeginDraw();
            renderTarget2D.Clear(Color.Orange);
        }
        /// <summary>
        /// Завершение рендера.
        /// </summary>
        protected override void EndRender()
        {
            renderTarget2D.EndDraw();

            base.EndRender();
        }
    }
}
