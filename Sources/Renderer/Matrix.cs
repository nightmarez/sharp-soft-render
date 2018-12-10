using System;

namespace Renderer
{
    /// <summary>
    /// Матрица.
    /// </summary>
    public sealed class Matrix
    {
        /// <summary>
        /// Массив для хранения значений матрицы.
        /// </summary>
        private readonly double[,] _arr;

        /// <summary>
        /// Размер матрицы.
        /// </summary>
        private const int Size = 4;

        public Matrix()
        {
            _arr = new double[Size, Size];
        }

        /// <summary>
        /// Создаёт единичную матрицу.
        /// </summary>
        public static Matrix CreateIdentity()
        {
            var matrix = new Matrix();

            for (int i = 0; i < Size; ++i)
                matrix._arr[i, i] = 1.0;

            return matrix;
        }

        /// <summary>
        /// Создаёт матрицу перемещения.
        /// </summary>
        public static Matrix CreateTranslate(double x, double y, double z)
        {
            var matrix = CreateIdentity();

            matrix[0, Size - 1] = x;
            matrix[1, Size - 1] = y;
            matrix[2, Size - 1] = z;

            return matrix;
        }

        /// <summary>
        /// Создаёт матрицу вращения по оси X.
        /// </summary>
        public static Matrix CreateRotateX(double angle)
        {
            var matrix = CreateIdentity();

            angle = angle.DegToRad();
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);

            matrix[1, 1] = angleCos;
            matrix[1, 2] = angleSin;
            matrix[2, 1] = -angleSin;
            matrix[2, 2] = angleCos;

            return matrix;
        }

        /// <summary>
        /// Создаёт матрицу вращения по оси Y.
        /// </summary>
        public static Matrix CreateRotateY(double angle)
        {
            var matrix = CreateIdentity();

            angle = angle.DegToRad();
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);

            matrix[0, 0] = angleCos;
            matrix[0, 2] = -angleSin;
            matrix[2, 0] = angleSin;
            matrix[2, 2] = angleCos;

            return matrix;
        }

        /// <summary>
        /// Создаёт матрицу вращения по оси Z.
        /// </summary>
        public static Matrix CreateRotateZ(double angle)
        {
            var matrix = CreateIdentity();

            angle = angle.DegToRad();
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);

            matrix[0, 0] = angleCos;
            matrix[0, 1] = angleSin;
            matrix[1, 0] = -angleSin;
            matrix[1, 1] = angleCos;

            return matrix;
        }

        /// <summary>
        /// Создаёт матрицу масштабирования.
        /// </summary>
        public static Matrix CreateScale(double x, double y, double z)
        {
            var matrix = CreateIdentity();

            matrix[0, 0] = x;
            matrix[1, 1] = y;
            matrix[2, 2] = z;

            return matrix;
        }

        /// <summary>
        /// Умножает матрицу на матрицу.
        /// </summary>
        Matrix Mult(Matrix matrix)
        {
	        var result = new Matrix();

	        for (int j = 0; j < Size; ++j)
                for (int i = 0; i < Size; ++i)
                    for (int k = 0; k < Size; ++k)
				        result[i, j] += this[k, j] * matrix[i, k];

	        return result;
        }

        /// <summary>
        /// Умножает матрицу на матрицу.
        /// </summary>
        public static Matrix operator *(Matrix mx0, Matrix mx1)
        {
            return mx0.Mult(mx1);
        }

        /// <summary>
        /// Доступ к элементам матрицы.
        /// </summary>
        public double this[int i, int j]
        {
            get { return _arr[i, j]; }
            set { _arr[i, j] = value; }
        }
    }
}
