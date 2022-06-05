using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Verse3.Core.Types
{
    public class CanvasPoint : IEquatable<CanvasSize>, IEquatable<CanvasPoint>
    {
        public static readonly CanvasPoint Unset = new CanvasPoint(double.NaN, double.NaN);

        public CanvasPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public Vector2 ToVector()
        {
            return new Vector2((float)X, (float)Y);
        }

        public override string ToString()
        {
            return ($"CanvasPoint({this.X.ToString()}, {this.Y.ToString()})");
        }

        public static CanvasSize operator +(CanvasPoint A, CanvasPoint B)
        {
            return new CanvasSize((A.X + B.X), (A.Y + B.Y));
        }
        public static CanvasSize operator -(CanvasPoint A, CanvasPoint B)
        {
            return new CanvasSize((A.X - B.X), (A.Y - B.Y));
        }
        public static bool operator ==(CanvasPoint A, CanvasPoint B)
        {
            return ((A.X == B.X) && (A.Y == B.Y));
        }
        public static bool operator !=(CanvasPoint A, CanvasPoint B)
        {
            return ((A.X != B.X) || (A.Y != B.Y));
        }
        public override bool Equals(object obj)
        {
            if (!(obj is CanvasPoint)) return false;
            CanvasPoint c = obj as CanvasPoint;
            return ((this.X == c.X) && (this.Y == c.Y));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public bool Equals(CanvasPoint other)
        {
            CanvasPoint s = this;
            return ((this.X == other.X) && (this.Y == other.Y));
        }
        public bool Equals(CanvasSize other)
        {
            return ((this.X == other.Width) && (this.Y == other.Height));
        }
        public static implicit operator CanvasPoint(CanvasSize v)
        {
            CanvasPoint s = new CanvasPoint(v.Width, v.Height);
            return s;
        }
        public bool IsValid()
        {
            if (this.X != double.NaN && this.Y != double.NaN) return true;
            else return false;
        }
    }
    public class CanvasSize : IEquatable<CanvasSize>, IEquatable<CanvasPoint>
    {
        public static readonly CanvasSize Unset = new CanvasSize(double.NaN, double.NaN);

        public CanvasSize(double w, double h)
        {
            this.Width = w;
            this.Height = h;
        }

        public double Width { get; set; }
        public double Height { get; set; }

        public Vector2 ToVector()
        {
            return new Vector2((float)Width, (float)Height);
        }
        public override string ToString()
        {
            return ($"CanvasSize({this.Width.ToString()}, {this.Height.ToString()})");
        }

        public static CanvasSize operator +(CanvasSize A, CanvasSize B)
        {
            return new CanvasSize((A.Width + B.Width), (A.Height + B.Height));
        }
        public static CanvasSize operator -(CanvasSize A, CanvasSize B)
        {
            return new CanvasSize((A.Width - B.Width), (A.Height - B.Height));
        }
        public static bool operator ==(CanvasSize A, CanvasSize B)
        {
            return ((A.Width == B.Width) && (A.Height == B.Height));
        }
        public static bool operator !=(CanvasSize A, CanvasSize B)
        {
            return ((A.Width != B.Width) || (A.Height != B.Height));
        }
        public override bool Equals(object obj)
        {
            if (!(obj is CanvasSize)) return false;
            CanvasSize c = obj as CanvasSize;
            return ((this.Width == c.Width) && (this.Height == c.Height));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public bool Equals(CanvasPoint other)
        {
            CanvasPoint s = this;
            return ((this.Width == other.X) && (this.Height == other.Y));
        }
        public bool Equals(CanvasSize other)
        {
            return ((this.Width == other.Width) && (this.Height == other.Height));
        }
        public double Area()
        {
            return (this.Width * this.Height);
        }

        public static implicit operator CanvasSize(CanvasPoint v)
        {
            CanvasSize s = new CanvasSize(v.X, v.Y);
            return s;
        }
        public bool IsValid()
        {
            if (this.Width != double.NaN && this.Height != double.NaN) return true;
            else return false;
        }
    }
}
