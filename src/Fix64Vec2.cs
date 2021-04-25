using System;
using System.Globalization;
using System.Text;

namespace FixMath.NET
{
    /// <summary>
    /// Represents a 2-point vector using Q31.32 fixed-point numbers.
    /// </summary>
    public struct Fix64Vec2 : IEquatable<Fix64Vec2>, IFormattable
    {
        private static int CombineHashCodes(int h1, int h2) => (h1 << 5) + h1 ^ h2;
        
        public static Fix64Vec2 Zero => new Fix64Vec2(Fix64.Zero);
        public static Fix64Vec2 One => new Fix64Vec2(Fix64.One);
        public static Fix64Vec2 Up => new Fix64Vec2(Fix64.Zero, Fix64.One);
        public static Fix64Vec2 Down => new Fix64Vec2(Fix64.Zero, -Fix64.One);
        public static Fix64Vec2 Right => new Fix64Vec2(Fix64.One, Fix64.Zero);
        public static Fix64Vec2 Left => new Fix64Vec2(-Fix64.One, Fix64.Zero);
        public static Fix64Vec2 MinValue => new Fix64Vec2(Fix64.MinValue);
        public static Fix64Vec2 MaxValue => new Fix64Vec2(Fix64.MaxValue);
        
        public readonly Fix64 X;
        public readonly Fix64 Y;

        public Fix64 SqrMagnitude => X * X + Y * Y;
        public Fix64 Magnitude => Fix64.Sqrt(SqrMagnitude);
        public Fix64Vec2 Normalized => this / Magnitude;

        public Fix64Vec2(Fix64 xy)
        {
            X = xy;
            Y = xy;
        }

        public Fix64Vec2(Fix64 x, Fix64 y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>Returns the length of the vector. </summary>
        /// <returns>The vector's length. </returns>
        public Fix64 Length() => Fix64.Sqrt(Dot(this, this));

        /// <summary>Returns the length of the vector squared. </summary>
        /// <returns>The vector's length squared. </returns>
        public Fix64 LengthSquared() => Dot(this, this);

        /// <summary>Computes the Euclidean distance between the two given points. </summary>
        /// <param name="value1">The first point. </param>
        /// <param name="value2">The second point. </param>
        /// <returns>The distance. </returns>
        public static Fix64 Distance(Fix64Vec2 value1, Fix64Vec2 value2)
        {
            var vector2 = value1 - value2;
            return Fix64.Sqrt(Dot(vector2, vector2));
        }
        
        /// <summary>Returns the Euclidean distance squared between two specified points. </summary>
        /// <param name="value1">The first point. </param>
        /// <param name="value2">The second point. </param>
        /// <returns>The distance squared. </returns>
        public static Fix64 DistanceSquared(Fix64Vec2 value1, Fix64Vec2 value2)
        {
            var vector2 = value1 - value2;
            return Dot(vector2, vector2);
        }
        
        /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one. </summary>
        /// <param name="value">The vector to normalize. </param>
        /// <returns>The normalized vector. </returns>
        public static Fix64Vec2 Normalize(Fix64Vec2 value)
        {
            var num = value.Length();
            return value / num;
        }
        
        /// <summary>Returns the reflection of a vector off a surface that has the specified normal. </summary>
        /// <param name="vector">The source vector. </param>
        /// <param name="normal">The normal of the surface being reflected off. </param>
        /// <returns>The reflected vector. </returns>
        public static Fix64Vec2 Reflect(Fix64Vec2 vector, Fix64Vec2 normal)
        {
            var num = Fix64Vec2.Dot(vector, normal);
            return vector - new Fix64(2) * num * normal;
        }
        
        /// <summary>Restricts a vector between a minimum and a maximum value. </summary>
        /// <param name="value1">The vector to restrict. </param>
        /// <param name="min">The minimum value. </param>
        /// <param name="max">The maximum value. </param>
        /// <returns>The restricted vector. </returns>
        public static Fix64Vec2 Clamp(Fix64Vec2 value1, Fix64Vec2 min, Fix64Vec2 max)
        {
            var x1 = value1.X;
            var num1 = x1 > max.X ? max.X : x1;
            var x2 = num1 < min.X ? min.X : num1;
            var y1 = value1.Y;
            var num2 = y1 > max.Y ? max.Y : y1;
            var y2 = num2 < min.Y ? min.Y : num2;
            return new Fix64Vec2(x2, y2);
        }
        
        /// <summary>Performs a linear interpolation between two vectors based on the given weighting. </summary>
        /// <param name="value1">The first vector. </param>
        /// <param name="value2">The second vector. </param>
        /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />. </param>
        /// <returns>The interpolated vector. </returns>
        public static Fix64Vec2 Lerp(Fix64Vec2 value1, Fix64Vec2 value2, Fix64 amount) => new Fix64Vec2(value1.X + (value2.X - value1.X) * amount, value1.Y + (value2.Y - value1.Y) * amount);
        
        // TODO: Transformations

        /// <summary>Returns a value that indicates whether this instance and another vector are equal. </summary>
        /// <param name="obj">The other vector. </param>
        /// <returns>
        /// <see langword="true" /> if the two vectors are equal; otherwise, <see langword="false" />. </returns>
        public override bool Equals(object obj)
        {
            return obj is Fix64Vec2 other && other.X == X && other.Y == Y;
        }
        
        /// <summary>Returns the dot product of two vectors. </summary>
        /// <param name="value1">The first vector. </param>
        /// <param name="value2">The second vector. </param>
        /// <returns>The dot product. </returns>
        public static Fix64 Dot(Fix64Vec2 value1, Fix64Vec2 value2) => value1.X * value2.X + value1.Y * value2.Y;

        /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
        /// <param name="value1">The first vector. </param>
        /// <param name="value2">The second vector. </param>
        /// <returns>The minimized vector. </returns>
        public static Fix64Vec2 Min(Fix64Vec2 value1, Fix64Vec2 value2) => new Fix64Vec2(value1.X < value2.X ? value1.X : value2.X, value1.Y < value2.Y ? value1.Y : value2.Y);

        /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
        /// <param name="value1">The first vector. </param>
        /// <param name="value2">The second vector. </param>
        /// <returns>The maximized vector. </returns>
        public static Fix64Vec2 Max(Fix64Vec2 value1, Fix64Vec2 value2) => new Fix64Vec2(value1.X > value2.X ? value1.X : value2.X, value1.Y > value2.Y ? value1.Y : value2.Y);

        /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements. </summary>
        /// <param name="value">A vector. </param>
        /// <returns>The absolute value vector. </returns>
        public static Fix64Vec2 Abs(Fix64Vec2 value) => new Fix64Vec2(Fix64.Abs(value.X), Fix64.Abs(value.Y));

        /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
        /// <param name="value">A vector. </param>
        /// <returns>The square root vector. </returns>
        public static Fix64Vec2 SquareRoot(Fix64Vec2 value) => new Fix64Vec2(Fix64.Sqrt(value.X), Fix64.Sqrt(value.Y));

        /// <summary>Adds two vectors together. </summary>
        /// <param name="left">The first vector to add. </param>
        /// <param name="right">The second vector to add. </param>
        /// <returns>The summed vector. </returns>
        public static Fix64Vec2 operator +(Fix64Vec2 left, Fix64Vec2 right) => new Fix64Vec2(left.X + right.X, left.Y + right.Y);

        /// <summary>Subtracts the second vector from the first. </summary>
        /// <param name="left">The first vector. </param>
        /// <param name="right">The second vector. </param>
        /// <returns>The vector that results from subtracting <paramref name="right" /> from <paramref name="left" />. </returns>
        public static Fix64Vec2 operator -(Fix64Vec2 left, Fix64Vec2 right) => new Fix64Vec2(left.X - right.X, left.Y - right.Y);

        /// <summary>Multiplies two vectors together. </summary>
        /// <param name="left">The first vector. </param>
        /// <param name="right">The second vector. </param>
        /// <returns>The product vector. </returns>
        public static Fix64Vec2 operator *(Fix64Vec2 left, Fix64Vec2 right) => new Fix64Vec2(left.X * right.X, left.Y * right.Y);

        /// <summary>Multiples the scalar value by the specified vector. </summary>
        /// <param name="left">The vector. </param>
        /// <param name="right">The scalar value. </param>
        /// <returns>The scaled vector. </returns>
        public static Fix64Vec2 operator *(Fix64 left, Fix64Vec2 right) => new Fix64Vec2(left, left) * right;

        /// <summary>Multiples the specified vector by the specified scalar value. </summary>
        /// <param name="left">The vector. </param>
        /// <param name="right">The scalar value. </param>
        /// <returns>The scaled vector. </returns>
        public static Fix64Vec2 operator *(Fix64Vec2 left, Fix64 right) => left * new Fix64Vec2(right, right);

        /// <summary>Divides the first vector by the second. </summary>
        /// <param name="left">The first vector. </param>
        /// <param name="right">The second vector. </param>
        /// <returns>The vector that results from dividing <paramref name="left" /> by <paramref name="right" />. </returns>
        public static Fix64Vec2 operator /(Fix64Vec2 left, Fix64Vec2 right) => new Fix64Vec2(left.X / right.X, left.Y / right.Y);

        /// <summary>Divides the specified vector by a specified scalar value.</summary>
        /// <param name="value1">The vector. </param>
        /// <param name="value2">The scalar value. </param>
        /// <returns>The result of the division. </returns>
        public static Fix64Vec2 operator /(Fix64Vec2 value1, Fix64 value2)
        {
          var num = Fix64.One / value2;
          return new Fix64Vec2(value1.X * num, value1.Y * num);
        }

        /// <summary>Negates the specified vector. </summary>
        /// <param name="value">The vector to negate. </param>
        /// <returns>The negated vector. </returns>
        public static Fix64Vec2 operator -(Fix64Vec2 value) => Zero - value;

        /// <summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.  </summary>
        /// <param name="left">The first vector to compare. </param>
        /// <param name="right">The second vector to compare. </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Fix64Vec2 left, Fix64Vec2 right) => left.Equals(right);

        /// <summary>Returns a value that indicates whether two specified vectors are not equal.  </summary>
        /// <param name="left">The first vector to compare. </param>
        /// <param name="right">The second vector to compare. </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />. </returns>
        public static bool operator !=(Fix64Vec2 left, Fix64Vec2 right) => !(left == right);

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        /// <summary>Returns the string representation of the current instance using default formatting. </summary>
        /// <returns>The string representation of the current instance. </returns>
        public override string ToString() => ToString("G", CultureInfo.CurrentCulture);

        /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements. </summary>
        /// <param name="format">A standard or custom numeric format string that defines the format of individual elements.</param>
        /// <returns>The string representation of the current instance. </returns>
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        
        /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements and the specified format provider to define culture-specific formatting.</summary>
        /// <param name="format">A standard or custom numeric format string that defines the format of individual elements. </param>
        /// <param name="formatProvider">A format provider that supplies culture-specific formatting information. </param>
        /// <returns>The string representation of the current instance. </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var stringBuilder = new StringBuilder();
            var numberGroupSeparator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            stringBuilder.Append('<');
            stringBuilder.Append(X.ToString(format, formatProvider));
            stringBuilder.Append(numberGroupSeparator);
            stringBuilder.Append(' ');
            stringBuilder.Append(Y.ToString(format, formatProvider));
            stringBuilder.Append('>');
            return stringBuilder.ToString();
        }

        public bool Equals(Fix64Vec2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}