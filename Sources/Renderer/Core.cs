using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Renderer
{
    /// <summary>
    /// Основной класс программы.
    /// </summary>
    public sealed class Core
    {
        /// <summary>
        /// Экземпляр основного класса.
        /// </summary>
        private static Core _core;

        /// <summary>
        /// Окно программы.
        /// </summary>
        private Form _wnd;

        /// <summary>
        /// Буфер вершин для хранения модели.
        /// </summary>
        private VertexBuffer _vb;

        /// <summary>
        /// Фабрика загрузчиков 3D моделей.
        /// </summary>
        private LoadersFactory _loaders;

        /// <summary>
        /// Предыдущие координаты мыши.
        /// </summary>
        private int _prevX, _prevY;

        /// <summary>
        /// Нажата ли кнопка мыши.
        /// </summary>
        private bool _mouseDown;

        /// <summary>
        /// Контейнер для основных панелей (панель с кнопками и панель для отрисовки изображений).
        /// </summary>
        private SplitContainer _splitContainer;

        /// <summary>
        /// Масштаб.
        /// </summary>
        private int _zoom = 10;

        /// <summary>
        /// Матрица вращения модели.
        /// </summary>
        private Matrix _rotationMatrix =
            Matrix.CreateIdentity() *
            Matrix.CreateRotateX(30) *
            Matrix.CreateRotateY(30);

        /// <summary>
        /// Экземпляр основного класса.
        /// </summary>
        public static Core Instance
        {
            get { return _core ?? (_core = new Core()); }
        }

        /// <summary>
        /// Кнопки клавиатуры.
        /// </summary>
        private bool _e, _q, _a, _d, _s, _w; 

        /// <summary>
        /// Окно.
        /// </summary>
        public Form Window
        {
            get
            {
                // Если окно ещё не создано...
                if (_wnd == null)
                {
                    // Создаём окно.
                    _wnd = new Form
                    {
                        Width = 1024,
                        Height = 768,
                        StartPosition = FormStartPosition.CenterScreen
                    };

                    _wnd.KeyDown += (sender, e) => KeyDown(e.KeyCode);
                    _wnd.KeyUp += (sender, e) => KeyUp(e.KeyCode);

                    // Основной контейнер.
                    _splitContainer = new SplitContainer();
                    _splitContainer.Panel1MinSize = 150;
                    _splitContainer.SplitterIncrement = 10;
                    _splitContainer.FixedPanel = FixedPanel.Panel1;
                    _splitContainer.Dock = DockStyle.Fill;
                    _splitContainer.TabStop = false;
                    _wnd.Controls.Add(_splitContainer);
                    _wnd.Shown += (sender, e) =>
                    {
                        _splitContainer.SplitterDistance = 150;
                    };
                    RenderPanel.Cursor = Cursors.SizeAll;

                    // Контейнер для кнопок.
                    var menuSplitContainer = new SplitContainer();
                    menuSplitContainer.Orientation = Orientation.Horizontal;
                    menuSplitContainer.Dock = DockStyle.Fill;
                    menuSplitContainer.SplitterIncrement = 10;
                    menuSplitContainer.FixedPanel = FixedPanel.Panel1;
                    menuSplitContainer.SplitterWidth = 1;
                    menuSplitContainer.IsSplitterFixed = true;
                    menuSplitContainer.TabStop = false;
                    MenuPanel.Controls.Add(menuSplitContainer);

                    // Кнопки масштабирования.

                    var btnZoomIn = new Button();
                    btnZoomIn.Image = new Bitmap(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "Graphics", "zoomIn.jpg"));
                    btnZoomIn.AutoSize = true;
                    btnZoomIn.BackColor = Color.White;
                    btnZoomIn.TabStop = false;
                    btnZoomIn.KeyDown += (sender, e) => KeyDown(e.KeyCode);
                    btnZoomIn.KeyUp += (sender, e) => KeyUp(e.KeyCode);
                    menuSplitContainer.Panel1.Controls.Add(btnZoomIn);

                    var btnZoomOut = new Button();
                    btnZoomOut.Image = new Bitmap(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "Graphics", "zoomOut.jpg"));
                    btnZoomOut.AutoSize = true;
                    btnZoomOut.BackColor = Color.White;
                    btnZoomOut.Left = btnZoomIn.Left + btnZoomIn.Width;
                    btnZoomOut.TabStop = false;
                    btnZoomOut.KeyDown += (sender, e) => KeyDown(e.KeyCode);
                    btnZoomOut.KeyUp += (sender, e) => KeyUp(e.KeyCode);
                    menuSplitContainer.Panel1.Controls.Add(btnZoomOut);

                    btnZoomIn.Click += (sender, e) => ZoomIn(btnZoomOut, btnZoomIn, 2);
                    btnZoomOut.Click += (sender, e) => ZoomOut(btnZoomOut, btnZoomIn, 2);

                    menuSplitContainer.Panel1MinSize = Math.Max(btnZoomIn.Height, btnZoomOut.Height);

                    MenuPanel.Controls.Add(menuSplitContainer);

                    // Размер иконок моделей.
                    const int imgSize = 100;

                    // Массив иконок моделей.
                    var imageList = new ImageList();
                    imageList.ImageSize = new Size(imgSize, imgSize);

                    // Список иконок моделей.
                    var listView = new ListView();
                    listView.Dock = DockStyle.Fill;
                    listView.LargeImageList = imageList;
                    listView.MultiSelect = false;

                    // Загружаем все модели и рисуем иконки.
                    int i = 0;
                    var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "Objects"));
                    foreach (string fileName in files)
                    {
                        var tmp = new Bitmap(imgSize, imgSize);
                        using (var raw = new RawBitmap(tmp))
                        {
                            raw.Clear(Color.Azure);

                            VertexBuffer vb = Loaders.GetLoader(fileName).Load(fileName);
                            Vector volume = vb.Volume;
                            double scaleFactor = Math.Min(tmp.Width, tmp.Height) / 2.0 / Math.Max(volume.X, Math.Max(volume.Y, volume.Z));

                            Matrix matrix =
                                Matrix.CreateIdentity() *
                                Matrix.CreateScale(scaleFactor, scaleFactor, scaleFactor) *
                                Matrix.CreateRotateX(30) *
                                Matrix.CreateRotateY(30) *
                                Matrix.CreateTranslate(tmp.Width / 2.0, tmp.Height / 2.0, 0);

                            vb = vb.Mult(matrix);
                            vb.CalcTrianglesNormals();
                            vb.DrawTriangles(raw, Color.GhostWhite, true);

                            imageList.Images.Add(tmp);
                            var item = new ListViewItem(Path.GetFileName(fileName), i++);

                            listView.Items.Add(item);
                        }
                    }

                    menuSplitContainer.Panel2.Controls.Add(listView);

                    // Обработчик изменения индекса выбранной модели в списке:
                    // если выбрана новая модель, загружаем её.
                    listView.SelectedIndexChanged += (sender, e) =>
                    {
                        if (listView.SelectedIndices.Count > 0)
                        {
                            LoadModel(files[listView.SelectedIndices[0]]);
                            DrawScene();
                        }
                    };

                    listView.KeyDown += (sender, e) => KeyDown(e.KeyCode);
                    listView.KeyUp += (sender, e) => KeyUp(e.KeyCode);
                    listView.Items[0].Selected = true;

                    RenderPanel.MouseDown += (sender, e) =>
                    {
                        _mouseDown = true;
                        _prevX = e.X;
                        _prevY = e.Y;
                    };

                    RenderPanel.MouseUp += (sender, e) =>
                    {
                        _mouseDown = false;
                    };

                    RenderPanel.MouseMove += (sender, e) =>
                    {
                        if (_mouseDown)
                        {
                            _rotationMatrix =
                                _rotationMatrix *
                                Matrix.CreateRotateX(e.Y - _prevY) *
                                Matrix.CreateRotateY(_prevX - e.X);

                            _prevX = e.X;
                            _prevY = e.Y;

                            DrawScene();
                        }
                    };

                    RenderPanel.Paint += (sender, e) => DrawScene();
                    RenderPanel.Resize += (sender, e) => DrawScene();

                    // Таймер для трансформации объекта при управлении с клавиатуры.
                    var timer = new Timer();
                    timer.Interval = 20;
                    timer.Tick += (sender, e) =>
                    {
                        if ((_e && !_q) || (!_e && _q))
                        {
                            if (_e)
                            {
                                ZoomIn(btnZoomOut, btnZoomIn, 1);
                            }
                            else if (_q)
                            {
                                ZoomOut(btnZoomOut, btnZoomIn, 1);
                            }
                        }

                        const int angleSpeed = 5;

                        if ((_a && !_d) || (!_a && _d))
                        {
                            if (_a)
                            {
                                _rotationMatrix = _rotationMatrix * Matrix.CreateRotateY(angleSpeed);
                                DrawScene();
                            }
                            else if (_d)
                            {
                                _rotationMatrix = _rotationMatrix * Matrix.CreateRotateY(-angleSpeed);
                                DrawScene();
                            }
                        }

                        if ((_s && !_w) || (!_s && _w))
                        {
                            if (_s)
                            {
                                _rotationMatrix = _rotationMatrix * Matrix.CreateRotateX(angleSpeed);
                                DrawScene();
                            }
                            else if (_w)
                            {
                                _rotationMatrix = _rotationMatrix * Matrix.CreateRotateX(-angleSpeed);
                                DrawScene();
                            }
                        }
                    };
                    timer.Start();
                }

                return _wnd;
            }
        }

        /// <summary>
        /// Нажатие кнопки.
        /// </summary>
        private void KeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.E:
                    _e = true;
                    break;

                case Keys.Q:
                    _q = true;
                    break;

                case Keys.A:
                    _a = true;
                    break;

                case Keys.D:
                    _d = true;
                    break;

                case Keys.S:
                    _s = true;
                    break;

                case Keys.W:
                    _w = true;
                    break;
            }
        }

        /// <summary>
        /// Отпускание кнопки.
        /// </summary>
        private void KeyUp(Keys key)
        {
            switch (key)
            {
                case Keys.E:
                    _e = false;
                    break;

                case Keys.Q:
                    _q = false;
                    break;

                case Keys.A:
                    _a = false;
                    break;

                case Keys.D:
                    _d = false;
                    break;

                case Keys.S:
                    _s = false;
                    break;

                case Keys.W:
                    _w = false;
                    break;
            }
        }

        /// <summary>
        /// Приближение.
        /// </summary>
        private void ZoomIn(Button btnZoomOut, Button btnZoomIn, int step)
        {
            _zoom += step;
            btnZoomOut.Enabled = true;

            if (_zoom > 20)
            {
                _zoom = 20;
                btnZoomIn.Enabled = false;
            }

            DrawScene();
        }

        /// <summary>
        /// Отдаление.
        /// </summary>
        private void ZoomOut(Button btnZoomOut, Button btnZoomIn, int step)
        {
            _zoom -= step;
            btnZoomIn.Enabled = true;

            if (_zoom < 2)
            {
                _zoom = 2;
                btnZoomOut.Enabled = false;
            }

            DrawScene();
        }

        /// <summary>
        /// Загружает модель.
        /// </summary>
        public void LoadModel(string fileName)
        {
            _vb = Loaders.GetLoader(fileName).Load(fileName);
        }

        /// <summary>
        /// Панель для рендеринга.
        /// </summary>
        public Panel RenderPanel
        {
            get { return _splitContainer.Panel2; }
        }

        /// <summary>
        /// Панель с кнопками.
        /// </summary>
        public Panel MenuPanel
        {
            get { return _splitContainer.Panel1; }
        }

        public void DrawScene()
        {
            using (var bmp = new Bitmap(RenderPanel.ClientSize.Width, RenderPanel.ClientSize.Height))
            {
                using (var raw = new RawBitmap(bmp))
                {
                    raw.Clear(Color.Azure);

                    if (_vb != null)
                    {
                        Vector volume = _vb.Volume;
                        double scaleFactor = Math.Min(bmp.Width, bmp.Height) / 2.0 / Math.Max(volume.X, Math.Max(volume.Y, volume.Z)) * (_zoom / 10.0);

                        // Рисуем объект.

                        Matrix matrix =
                            Matrix.CreateIdentity() *
                            Matrix.CreateScale(scaleFactor, scaleFactor, scaleFactor) *
                            _rotationMatrix *
                            Matrix.CreateTranslate(bmp.Width / 2.0, bmp.Height / 2.0, 0);

                        VertexBuffer tmpVb = _vb.Clone().Mult(matrix);
                        tmpVb.CalcTrianglesNormals();
                        tmpVb.DrawTriangles(raw, Color.GhostWhite, true);

                        // Рисуем тень объекта.

                        matrix =
                            Matrix.CreateIdentity() *
                            Matrix.CreateScale(scaleFactor, scaleFactor, scaleFactor) *
                            _rotationMatrix *
                            Matrix.CreateScale(1, 1, 0) *
                            Matrix.CreateTranslate(-100 * (_zoom / 10.0), 100 * (_zoom / 10.0), -1000) *
                            Matrix.CreateTranslate(bmp.Width / 2.0, bmp.Height / 2.0, 0);

                        tmpVb = _vb.Clone().Mult(matrix);
                        tmpVb.CalcTrianglesNormals();
                        tmpVb.DrawTriangles(raw, Color.Gray, false);
                    }
                }

                using (Graphics gfx = RenderPanel.CreateGraphics())
                    gfx.DrawImageUnscaled(bmp, 0, 0);
            }
        }

        /// <summary>
        /// Фабрика загрузчиков 3D моделей.
        /// </summary>
        public LoadersFactory Loaders
        {
            get { return _loaders ?? (_loaders = new LoadersFactory()); }
        }

        private Core() { }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.Run(Window);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Instance.Run();
        }
    }
}
