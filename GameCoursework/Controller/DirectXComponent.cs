using System;
using System.Windows;
using System.Windows.Media;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace Controller
{
    /// <summary>
    /// Создание цепочки обмена буффером SharpDx в родительском элементе Win32Control.
    /// </summary>
    public abstract class DirectXComponent : Win32Control
    {
        private Device device;
        /// <summary>
        /// Цепочка обмена буффером.
        /// </summary>
        private SwapChain swapChain;
        /// <summary>
        /// Задний буффер.
        /// </summary>
        private Texture2D backBuffer;
        /// <summary>
        /// Объект представления данных.
        /// </summary>
        private RenderTargetView renderTargetView;
        /// <summary>
        /// Задний буффер.
        /// </summary>
        protected Texture2D BackBuffer => backBuffer;
        //protected RenderTargetView RenderTargetView => _renderTargetView;
        /// <summary>
        /// Длина.
        /// </summary>
        protected int SurfaceWidth { get; private set; }
        /// <summary>
        /// Ширина.
        /// </summary>
        protected int SurfaceHeight { get; private set; }
        /// <summary>
        /// Конструктор DirectX.
        /// </summary>
        protected DirectXComponent()
        {
        }
        /// <summary>
        /// Метод, в котором происходит отрисовка ресурсов DirectX.
        /// </summary>
        protected override sealed void Initialize()
        {
            InternalInitialize();

            //Поверхность отображения, на которой происходит отрисовка
            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }
        /// <summary>
        /// Метод, в котором происходит очистка ресурсов DirectX.
        /// </summary>
        protected override sealed void Uninitialize()
        {
            CompositionTarget.Rendering -= OnCompositionTargetRendering;
            InternalUninitialize();
        }
        /// <summary>
        /// Метод
        /// </summary>
        protected sealed override void Resized()
        {
            InternalUninitialize();
            InternalInitialize();
        }

        private void OnCompositionTargetRendering(object sender, EventArgs eventArgs)
        {
            BeginRender();
            Render();
            EndRender();
        }

        private double GetDpiScale()
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            return source.CompositionTarget.TransformToDevice.M11;
        }
        /// <summary>
        /// Создание ресурсов DirectX. Наследуемые методы должны начинаться с base.InternalInitialize();
        /// </summary>
        protected virtual void InternalInitialize()
        {
            var dpiScale = GetDpiScale();

            //Длина  и ширина окна, которая будет подстраиваться в зависимости от изменения размера окна.
            SurfaceWidth = (int)(ActualWidth < 0 ? 0 : Math.Ceiling(ActualWidth * dpiScale));
            SurfaceHeight = (int)(ActualHeight < 0 ? 0 : Math.Ceiling(ActualHeight * dpiScale));

            //Цепочка обмена для буфферов.
            var swapChainDescription = new SwapChainDescription
            {
                //Окно, в котором будет происходить отображение.
                OutputHandle = Hwnd,

                //Количество буферов.
                BufferCount = 1,

                //Оконный или полноэкранный режим.
                IsWindowed = true,

                //Задний буфер(размер окна, частота обновлений в секунду, формат буфера).
                ModeDescription = new ModeDescription(SurfaceWidth, SurfaceHeight, new Rational(60, 1), Format.B8G8R8A8_UNorm),

                //Для сглаживания отрисованных фигурю
                SampleDescription = new SampleDescription(1, 0),

                //Процесс получения процессором заднего буфера.
                SwapEffect = SwapEffect.Discard,

                //Использует элемент отображения как RenderTargetOutput. 
                Usage = Usage.RenderTargetOutput
            };

            //Как устройсво будет обмениваться с цепочкой(он будет использавоть граф. процессор, дескриптор цепочки обмена, устройство, цепочку обмена).
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDescription, out device, out swapChain);

            // Новый RenderTargetView из заднего буффера.
            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
            renderTargetView = new RenderTargetView(device, backBuffer);
        }
        /// <summary>
        /// Очистка ресурсов DirectX. Наследуемые методы должны заканчиваться с base.InternalUninitialize();
        /// </summary>
        protected virtual void InternalUninitialize()
        {
            Utilities.Dispose(ref renderTargetView);
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref swapChain);
            Utilities.Dispose(ref device);

        }
        /// <summary>
        /// Начало рендера.
        /// </summary>
        protected virtual void BeginRender()
        {
            //_device.ImmediateContext.Rasterizer.SetViewport(0, 0, (float)ActualWidth, (float)ActualHeight);
            //устанавливает объект рендеринга на тот, что был создан ранее
            device.ImmediateContext.OutputMerger.SetRenderTargets(renderTargetView);
        }
        /// <summary>
        /// Завершение рендера.
        /// </summary>
        protected virtual void EndRender()
        {
            //Заменяет заднюю часть передним буффером (1 - вертикальная синхронизация ).
            swapChain.Present(1, PresentFlags.None);
        }
        /// <summary>
        /// Отрисовка каждого кадра.
        /// </summary>
		protected abstract void Render();
    }
}
