using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace RotatingCubeWithMouse
{
    public class MainWindow : GameWindow
    {
        private float _cubeAngleX = 0f;
        private float _cubeAngleY = 0f;
        private Vector3 _cubePosition = Vector3.One;
        private float zDistance = -5.0f; // Позиция камеры (по Z)

        private Vector3[,] _vertexColors = new Vector3[6, 4]; // Цвета вершин для каждой стороны куба
        private Random _random = new Random();

        public MainWindow()
            : base(800, 600, GraphicsMode.Default, "Cubik")
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.Enable(EnableCap.DepthTest);
            GenerateRandomGradientColors();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Key.Up)) _cubeAngleX += 1.0f;
            if (keyboard.IsKeyDown(Key.Down)) _cubeAngleX -= 1.0f;
            if (keyboard.IsKeyDown(Key.Left)) _cubeAngleY += 1.0f;
            if (keyboard.IsKeyDown(Key.Right)) _cubeAngleY -= 1.0f;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                // Преобразование оконных координат в нормализованные координаты устройства (NDC)
                float mouseX = (2.0f * e.X / Width) - 1.0f;
                float mouseY = -((2.0f * e.Y / Height) - 1.0f); // Инвертируем ось Y

                // Учёт перспективы
                float aspectRatio = Width / (float)Height;
                float fov = MathHelper.PiOver4; // Угол обзора (45 градусов)

                // Вычисляем масштаб для преобразования NDC в мировые координаты
                float scale = Math.Abs(zDistance) * (float)Math.Tan(fov / 2.0f);

                // Преобразование в мировые координаты
                _cubePosition.X = mouseX * aspectRatio * scale;
                _cubePosition.Y = mouseY * scale;
                _cubePosition.Z = zDistance;

                // Генерируем новые случайные градиентные цвета
                GenerateRandomGradientColors();
            }
        }

        private void GenerateRandomGradientColors()
        {
            for (int i = 0; i < 6; i++) // Для каждой стороны куба
            {
                for (int j = 0; j < 4; j++) // Для каждой вершины стороны
                {
                    _vertexColors[i, j] = new Vector3(
                        (float)_random.NextDouble(),
                        (float)_random.NextDouble(),
                        (float)_random.NextDouble()
                    );
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Настройка проекции
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, Width / (float)Height, 0.1f, 100.0f);
            GL.LoadMatrix(ref projection);

            // Настройка модели вида
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(_cubePosition); // Перемещение куба
            GL.Rotate(_cubeAngleX, 1.0f, 0.0f, 0.0f);
            GL.Rotate(_cubeAngleY, 0.0f, 1.0f, 0.0f);

            DrawCube();
            SwapBuffers();
        }

        private void DrawCube()
        {
            GL.Begin(PrimitiveType.Quads);

            // Передняя сторона
            GL.Color3(_vertexColors[0, 0]);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.Color3(_vertexColors[0, 1]);
            GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.Color3(_vertexColors[0, 2]);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Color3(_vertexColors[0, 3]);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);

            // Задняя сторона
            GL.Color3(_vertexColors[1, 0]);
            GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.Color3(_vertexColors[1, 1]);
            GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.Color3(_vertexColors[1, 2]);
            GL.Vertex3(0.5f, 0.5f, -0.5f);
            GL.Color3(_vertexColors[1, 3]);
            GL.Vertex3(-0.5f, 0.5f, -0.5f);

            // Левая сторона
            GL.Color3(_vertexColors[2, 0]);
            GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.Color3(_vertexColors[2, 1]);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.Color3(_vertexColors[2, 2]);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);
            GL.Color3(_vertexColors[2, 3]);
            GL.Vertex3(-0.5f, 0.5f, -0.5f);

            // Правая сторона
            GL.Color3(_vertexColors[3, 0]);
            GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.Color3(_vertexColors[3, 1]);
            GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.Color3(_vertexColors[3, 2]);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Color3(_vertexColors[3, 3]);
            GL.Vertex3(0.5f, 0.5f, -0.5f);

            // Верхняя сторона
            GL.Color3(_vertexColors[4, 0]);
            GL.Vertex3(-0.5f, 0.5f, -0.5f);
            GL.Color3(_vertexColors[4, 1]);
            GL.Vertex3(0.5f, 0.5f, -0.5f);
            GL.Color3(_vertexColors[4, 2]);
            GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.Color3(_vertexColors[4, 3]);
            GL.Vertex3(-0.5f, 0.5f, 0.5f);

            // Нижняя сторона
            GL.Color3(_vertexColors[5, 0]);
            GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.Color3(_vertexColors[5, 1]);
            GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.Color3(_vertexColors[5, 2]);
            GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.Color3(_vertexColors[5, 3]);
            GL.Vertex3(-0.5f, -0.5f, 0.5f);

            GL.End();
        }

        [STAThread]
        public static void Main()
        {
            using (MainWindow window = new MainWindow())
            {
                window.Run(60.0);
            }
        }
    }
}
