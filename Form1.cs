using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Globalization;
using SharpGL;


namespace WindowsFormsTest
{
    public partial class Form1 : Form
    {
        OpenGL gl;
        
        public Form1()
        {
            InitializeComponent();

            // Создаем экземпляр
            gl = new OpenGL();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// Угол поворота 
        float rtri = 60;

        // Путь к файлу
        //string filePath = "../../3D_models/sword/sword.obj";
        string filePath = "../../3D_models/robot/Rmk3.obj";

        // Много раз в секунду вызывается эта функция
        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {

        }

        // То что происходит при инициализации glControl 
        private void openGLControl1_Load(object sender, EventArgs e)
        {
            // Очистка экрана и буфера глубин
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Куб /////////////////////////////
            // Сбрасываем модельно-видовую матрицу
            //gl.LoadIdentity();
            //// Сдвигаем перо с право от центра и вглубь экрана
            //gl.Translate(2.5f, 0.0f, -10.0f);
            //// Вращаем куб вокруг ее оси Y
            //gl.Rotate(rtri, 0.0f, 1.5, 0.0f);
            //// Рисуем грани куба
            //gl.Begin(OpenGL.GL_QUADS);

            //drawCube();

            //gl.End();

            // Оbj файл /////////////////////////////
            // Сбрасываем модельно-видовую матрицу
            gl.LoadIdentity();

            // Сдвигаем перо влево от центра и вглубь экрана
            gl.Translate(0.0f, 0.0f, -45.0);

            // Вращаем куб вокруг ее оси Y
            gl.Rotate(rtri, -15.0f, -20.0f, 0.0f);
            
            // Рисуем четырехугольники - грани 3д объекта
            gl.Begin(OpenGL.GL_QUADS);

            drawObj(filePath);

            gl.End();

            // Контроль полной отрисовки следующего изображения
            gl.Flush();
        }

        // Рисует obj файл
        private void drawObj(string filepath)
        {
            // Парсит
            ObjParser.Parse(filepath);

            gl.Color(1.0f, 1.0f, 1.0f);

            foreach (List<List<int>> face in ObjParser.faces)
            {
                foreach (List<int> point in face)
                {
                    int vertexIndex = point[0]-1;
                    List<float> vertex = ObjParser.vertexes[vertexIndex];

                    gl.Vertex(vertex[0], vertex[1], vertex[2]);
                }
            }
        }

        // Рисует куб
        private void drawCube()
        {
            // Top
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            // Bottom
            gl.Color(1.0f, 0.5f, 0.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            // Front
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            // Back
            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            // Left
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            // Right
            gl.Color(1.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);

        }

        // При нажатии на текст в окошке
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    // Класс парсер
    // В метод Parse(filePath) дается путь к .obj файлу. Он его парсит и извлекает линии в отдельные списки в зависимости от токена в начале линии(v/vt/vn/f)
    // Списки не возвращаются, а храняться статически как свойства класса
    class ObjParser
    {
        // Координаты всех вершин треугольников. Каждый элемент это одна вершина и список, где хранятся значения координат по x, y, z
        public static List<List<float>> vertexes = new List<List<float>>();

        // Координаты текстур
        public static List<List<float>> textures = new List<List<float>>();

        // Нормали
        public static List<List<float>> normals = new List<List<float>>();

        // Лица треугольников
        // faces = [
        //      // Первое лицо
        //      [
        //            [553, 970],  // Первая точка. Элементы этого списка вершина и нормаль(v, vn)
        //            [],  // Вторая точка
        //            []   // Третья точка
        //      ],
        // Второе лицо
        //      [
        //            [],
        //            [],
        //            []
        //        ],
        //      [
        //            [],
        //            [],
        //            []
        //        ]
        // ]
        public static List<List<List<int>>> faces = new List<List<List<int>>>();

        // Делает из линии список, где значения это координаты вершины по x, y, z
        private static List<float> ModifyLine(string line)
        {
            // Массив всех 'слов' линии разделенный пробелом 
            string[] words = line.Split();

            // Удаляем токен(f, vn etc), то есть первый элемент массива
            string tokenlessValues = string.Join(" ", words.Skip(1));

            // Массив всех значений линии, разделенные пробелом 
            string[] values = tokenlessValues.Split();

            // Список координат вершин где будут 3 элемента: x, y, z
            List<float> result = new List<float>();

            foreach (string value in values)
            {
                // Добавляет в список каждую из координат в типе данных float
                result.Add(float.Parse(value.Trim(), CultureInfo.InvariantCulture));
            }

            return result;
        }

        // Парсит именно лицы, проебразует строку в вид более удобный для использования
        public static void ParseFace(string line)
        {
            // Массив всех 'слов' линии разделенный пробелом 
            string[] words = line.Split();

            // Удаляем токен(f, vn etc), то есть первый элемент массива. Строка без токена
            string tokenlessString = string.Join(" ", words.Skip(1));    // Пример: "1//1 246//1 332//1 117//1"

            // Список, где каждый элемент это одна точка. Каждая из точек тоже список, где элементы это v, vt и vn
            List<List<int>> currentFace = new List<List<int>>();

            // Виды строк: 
            // string face = "f 343435 343436 343434";
            // string face = "f 1006//725 560//725 567//725 1018//725";
            // string face = "f 23/2 32/34 35/34";
            // string face "f 1006/454/725 560/345/725 567/345/725 1018/345/725"

            foreach (string point in tokenlessString.Split(' '))
            {
                // Пропускает пустую сабстроку
                if (string.IsNullOrWhiteSpace(point))
                {
                    continue;
                }

                int delimetersCount = point.Count(c => c == '/');

                // Если у поллигона только вертексы есть
                if (delimetersCount == 0)
                {
                    currentFace.Add(new List<int> { int.Parse(point) });
                }
                // Если у поллигона есть все или все кроме нормали
                else if (delimetersCount == 1)
                {
                    List<int> pointValues = new List<int>();

                    foreach (string value in point.Split('/'))
                    {
                        pointValues.Add(int.Parse(value));
                    }
                    currentFace.Add(pointValues);
                }
                // У полигона нет текстуры
                else if (delimetersCount == 2)
                {
                    List<int> pointValues = new List<int>();

                    foreach (string value in point.Split(new string[] { "//" }, StringSplitOptions.None))
                    {
                        pointValues.Add(int.Parse(value));
                    }
                    currentFace.Add(pointValues);
                }

            }

            faces.Add(currentFace);
        }

        // Сортирует линии по отдельным спискам. Вершины в одном списке, нормали в одной списке и т.д
        private static void SortLines(string[] lines)
        {
            // Проверяeт каждую линию и добавляет ее в соответстующий список в зависимости от токена(v/vt и т.д)
            foreach (string line in lines)
            {
                // Вершина 
                if (line.StartsWith("v "))
                {
                    vertexes.Add(ModifyLine(line));
                }
                // Текстура
                else if (line.StartsWith("vt"))
                {
                    textures.Add(ModifyLine(line));
                }
                // Нормаль
                else if (line.StartsWith("vn"))
                {
                    normals.Add(ModifyLine(line));
                }
                // Лицо
                else if (line.StartsWith("f "))
                {
                    ParseFace(line);
                }
            }
        }

        // Это будет финальной функций которая будет делать все сразу.
        // Парсит файл, вытаскивает все нужные данные и организовывает их формате, который понимают движки для 3д рендеринга
        public static void Parse(string filePath)
        {
            // Получаем список со всеми линиями текста
            string[] lines = File.ReadAllLines(filePath);

            // Запихиваем из по спискам
            SortLines(lines);
        }
    }
}
