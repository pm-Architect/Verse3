using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;

namespace Core
{
    public class Geometry2D
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Bounding Box
        /// </summary>
        [Serializable]
        public class BoundingBox : Observable, IEquatable<BoundingBox>, ISerializable
        {
            #region Constructors

            public BoundingBox()
            {
                this.Location = CanvasPoint.Unset;
                this.Size = CanvasSize.Unset;
            }
            public BoundingBox(CanvasPoint point, CanvasSize size)
            {
                this.Location = point;
                this.Size = size;
            }
            public BoundingBox(CanvasPoint pointA, CanvasPoint pointB)
            {
                BoundingBox b = BoundingBox.FromLTRB(pointA.X, pointA.Y, pointB.X, pointB.Y);
                this.Location = b.Location;
                this.Size = b.Size;
            }
            public BoundingBox(double x, double y, double w, double h)
            {
                this.Location = new CanvasPoint(x, y);
                this.Size = new CanvasSize(w, h);
            }
            public BoundingBox(SerializationInfo info, StreamingContext context)
            {
                this.Location = (CanvasPoint)info.GetValue("Location", typeof(CanvasPoint));
                this.Size = (CanvasSize)info.GetValue("Size", typeof(CanvasSize));
            }

            public static BoundingBox Unset = new BoundingBox();

            #endregion

            #region Properties

            private CanvasPoint _location = CanvasPoint.Unset;
            private CanvasSize _size = CanvasSize.Unset;
            public CanvasPoint Location
            {
                get { return _location; }
                set
                {
                    if (value != _location)
                    {
                        _location = value;
                        OnPropertyChanged("Location");
                    }
                }
            }
            public CanvasSize Size
            {
                get { return _size; }
                set
                {
                    if (value != _size)
                    {
                        _size = value;
                        OnPropertyChanged("Size");
                    }
                }
            }
            [JsonIgnore]
            public double Left
            {
                get
                {
                    return this.Location.X;
                }
            }
            [JsonIgnore]
            public double Right
            {
                get
                {
                    return (this.Location.X + this.Size.Width);
                }
            }
            [JsonIgnore]
            public double Top
            {
                get
                {
                    return this.Location.Y;
                }
            }
            [JsonIgnore]
            public double Bottom
            {
                get
                {
                    return (this.Location.Y + this.Size.Height);
                }
            }

            [JsonIgnore]
            public CanvasPoint Center
            {
                get
                {
                    return new CanvasPoint((this.Location.X + (this.Size.Width / 2.0)), (this.Location.Y + (this.Size.Height / 2.0)));
                }
            }

            #endregion

            #region Methods

            public CanvasPoint this[int index]
            {
                get
                {
                    if ((index < 0) || (index > 3)) throw new IndexOutOfRangeException("Index must be between [0-3]. Bounding Boxes have only 4 Points.");
                    switch (index)
                    {
                        case 0: return new CanvasPoint(this.Top, this.Left);
                        case 1: return new CanvasPoint(this.Top, this.Right);
                        case 2: return new CanvasPoint(this.Bottom, this.Right);
                        case 3: return new CanvasPoint(this.Bottom, this.Left);
                        default: throw new IndexOutOfRangeException("Index must be between [0-3]. Bounding Boxes have only 4 Points.");
                    }
                }
            }
            public CanvasPoint[] GetPoints()
            {
                CanvasPoint[] cpOut = {
                new CanvasPoint(this.Left, this.Top),
                new CanvasPoint(this.Right, this.Top),
                new CanvasPoint(this.Right, this.Bottom),
                new CanvasPoint(this.Left, this.Bottom)
            };
                return cpOut;
            }
            public static int[] Contains(BoundingBox[] boundingBoxes, CanvasPoint point)
            {
                List<int> iOut = new List<int>();
                for (int i = 0; i < boundingBoxes.Length; i++)
                {
                    if (boundingBoxes[i].Contains(point))
                    {
                        iOut.Add(i);
                        break;
                    }
                }
                return iOut.ToArray();
            }

            //TODO
            //public static int[] Contains(Element[] elements, CanvasPoint point)
            //{
            //    List<int> iOut = new List<int>();
            //    for (int i = 0; i < elements.Length; i++)
            //    {
            //        if (elements[i].Bounds.Contains(point))
            //        {
            //            iOut.Add(i);
            //            break;
            //        }
            //    }
            //    return iOut.ToArray();
            //}

            public bool Contains(CanvasPoint point, bool strict = true)
            {
                bool boolOut = true;
                if (strict)
                {
                    if (point.X <= this.Left) return false;
                    if (point.X >= this.Right) return false;
                    if (point.Y <= this.Top) return false;
                    if (point.Y >= this.Bottom) return false;
                }
                else
                {
                    if (point.X < this.Left) return false;
                    if (point.X > this.Right) return false;
                    if (point.Y < this.Top) return false;
                    if (point.Y > this.Bottom) return false;
                }
                return boolOut;
            }

            public bool[] Contains(CanvasPoint[] points, bool strict = true)
            {
                List<bool> o = new List<bool>();
                for (int i = 0; i < points.Length; i++)
                {
                    o.Add(this.Contains(points[i], strict));
                }
                return o.ToArray();
            }

            public bool Contains(BoundingBox box, bool strict = true)
            {
                bool boolOut = true;
                if (strict)
                {
                    boolOut = boolOut && (box.Left > this.Left);
                    boolOut = boolOut && (box.Right < this.Right);
                    boolOut = boolOut && (box.Top > this.Top);
                    boolOut = boolOut && (box.Bottom < this.Bottom);
                }
                else
                {
                    boolOut = boolOut && (box.Left >= this.Left);
                    boolOut = boolOut && (box.Right <= this.Right);
                    boolOut = boolOut && (box.Top >= this.Top);
                    boolOut = boolOut && (box.Bottom <= this.Bottom);
                }
                return boolOut;
            }
            public static BoundingBox FromLTRB(double l, double t, double r, double b)
            {
                double x, y, w, h;
                if (r <= l)
                {
                    x = l;
                    w = (r - l);
                }
                else
                {
                    x = r;
                    w = (l - r);
                }
                if (b <= t)
                {
                    y = t;
                    h = (b - t);
                }
                else
                {
                    y = b;
                    h = (t - b);
                }
                BoundingBox bbOut = new BoundingBox(new CanvasPoint(x, y), new CanvasSize(w, h));
                return bbOut;
            }
            public void Inflate(CanvasSize s)
            {
                this.Size = this.Size + s;
            }
            public void Offset(CanvasSize s)
            {
                this.Size = this.Size + s;
                this.Location = this.Location + ((CanvasPoint)s);
            }
            public void Offset(double x1, double x2, double y1, double y2)
            {
                this.Location = this.Location + new CanvasPoint(x1, y1);
                this.Size = this.Size + new CanvasSize(x2, y2);
            }
            public BoundingBox InflateCopy(CanvasSize s)
            {
                return new BoundingBox(this.Location, new CanvasSize((this.Size.Width + s.Width), (this.Size.Height + s.Height)));
            }
            public BoundingBox OffsetCopy(CanvasSize s)
            {
                return new BoundingBox(new CanvasPoint((this.Location.X + s.Width), (this.Location.Y + s.Height)), new CanvasSize((this.Size.Width + s.Width), (this.Size.Height + s.Height)));
            }
            public BoundingBox OffsetCopy(double x1, double x2, double y1, double y2)
            {
                return new BoundingBox(new CanvasPoint((this.Location.X + x1), (this.Location.Y + y1)), new CanvasSize((this.Size.Width + x2), (this.Size.Height + y2)));
            }
            public static bool CheckIntersection(BoundingBox a, BoundingBox b, bool strict = false)
            {
                //bool boolOut = false;
                //if (strict)
                //{
                //    boolOut = boolOut || (a.Left < b.Right)
                //}
                //else
                //{
                //    boolOut = boolOut || ((a.Left >= b.Left));
                //    boolOut = boolOut || ((a.Right <= b.Right));
                //    boolOut = boolOut || ((a.Top >= b.Top));
                //    boolOut = boolOut || ((a.Bottom <= b.Bottom));
                //}
                //return boolOut;
                //if (a != null && b != null)
                //{
                //    if (strict)
                //    {
                //        if (a.Top < b.Bottom || a.Bottom < b.Top)
                //        {

                //        }
                //    }
                //}
                foreach (CanvasPoint p in a.GetPoints()) if (b.Contains(p, strict)) return true;
                foreach (CanvasPoint p in b.GetPoints()) if (a.Contains(p, strict)) return true;
                return false;
            }
            public static BoundingBox Intersect(BoundingBox a, BoundingBox b)
            {
                if (a == b) return a;
                if (CheckIntersection(a, b))
                {
                    double nl, nt, nr, nb;
                    if (a.Left <= b.Left) nl = b.Left;
                    else nl = a.Left;
                    if (a.Top <= b.Top) nt = b.Top;
                    else nt = a.Top;
                    if (a.Right >= b.Right) nr = b.Right;
                    else nr = a.Right;
                    if (a.Bottom >= b.Bottom) nb = b.Bottom;
                    else nb = a.Bottom;
                    return BoundingBox.FromLTRB(nl, nt, nr, nb);
                }
                else return null;
            }
            public static BoundingBox Union(BoundingBox a, BoundingBox b)
            {
                if (a == b) return a;
                if (CheckIntersection(a, b))
                {
                    double nl, nt, nr, nb;
                    if (a.Left <= b.Left) nl = a.Left;
                    else nl = b.Left;
                    if (a.Top <= b.Top) nt = a.Top;
                    else nt = b.Top;
                    if (a.Right >= b.Right) nr = a.Right;
                    else nr = b.Right;
                    if (a.Bottom >= b.Bottom) nb = a.Bottom;
                    else nb = b.Bottom;
                    return BoundingBox.FromLTRB(nl, nt, nr, nb);
                }
                else return null;
            }
            public static BoundingBox MassUnion(BoundingBox[] array)
            {
                BoundingBox bbOut = null;
                for (int i = 0; i < array.Length; i++)
                {
                    BoundingBox bbTempA = array[i];
                    for (int j = 0; j < array.Length; j++)
                    {
                        BoundingBox bbTempB = BoundingBox.Union(bbTempA, array[j]);
                        bbTempA = bbTempB;
                    }
                    if (bbTempA != array[i])
                    {
                        if (bbOut == null) bbOut = bbTempA;
                        else
                        {
                            BoundingBox bbTempC = BoundingBox.Union(bbOut, bbTempA);
                            if (bbTempC != null) bbOut = bbTempC;
                        }
                    }
                }
                return bbOut;
            }
            public static BoundingBox Trim(BoundingBox box, BoundingBox cutter)
            {
                if (CheckIntersection(box, cutter))
                {
                    double nr, nb;
                    double nl = box.Left;
                    double nt = box.Top;
                    if ((box.Left == cutter.Left) || (box.Top == cutter.Top)) return null;
                    if (box.Left > cutter.Left) nr = cutter.Right;
                    else nr = cutter.Left;
                    if (box.Top > cutter.Top) nb = cutter.Top;
                    else nb = cutter.Bottom;
                    return BoundingBox.FromLTRB(nl, nt, nr, nb);
                }
                else return null;
            }
            public static BoundingBox GetBounds(CanvasPoint[] points)
            {
                BoundingBox bbOut = BoundingBox.Unset;
                for (int i = 0; i < points.Length; i++)
                {
                    if (bbOut == BoundingBox.Unset) bbOut = new BoundingBox(points[i], new CanvasSize(0.0, 0.0));
                    else
                    {
                        if (points[i].X < bbOut.Left) bbOut.Location.X = points[i].X;
                        if (points[i].Y < bbOut.Top) bbOut.Location.Y = points[i].Y;
                        if (points[i].X > bbOut.Right) bbOut.Size.Width = points[i].X;
                        if (points[i].Y > bbOut.Bottom) bbOut.Size.Height = points[i].Y;
                    }
                }
                return bbOut;
            }
            public bool Equals(BoundingBox other)
            {
                return ((this.Size == other.Size) && (this.Location == other.Location));
            }
            public override bool Equals(object obj)
            {
                if (!(obj is BoundingBox)) return false;
                BoundingBox c = obj as BoundingBox;
                return ((this.Size == c.Size) && (this.Location == c.Location));
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return ($"BoundingBox(X={this.Location.X}, Y={this.Location.Y}, W={this.Size.Width}, H={this.Size.Height})");
            }
            public bool IsValid()
            {
                if (this.Size.IsValid() && this.Location.IsValid()) return (this.Size.Area() > 0.0);
                else return false;
            }

            public static BoundingBox operator +(BoundingBox A, BoundingBox B)
            {
                return BoundingBox.Union(A, B);
            }
            public static BoundingBox operator -(BoundingBox A, BoundingBox B)
            {
                return BoundingBox.Intersect(A, B);
            }
            public static bool operator ==(BoundingBox A, BoundingBox B)
            {
                if (A is null || B is null) return false;
                if (!A.IsValid() || !B.IsValid()) return false;
                return ((A.Size == B.Size) && (A.Location == B.Location));
            }
            public static bool operator !=(BoundingBox A, BoundingBox B)
            {
                if (A is null || B is null) return false;
                if (!A.IsValid() || !B.IsValid()) return false;
                return ((A.Size != B.Size) || (A.Location != B.Location));
            }

            #endregion

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Location", this.Location);
                info.AddValue("Size", this.Size);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Canvas Point
        /// </summary>
        [Serializable]
        public class CanvasPoint : Observable, IEquatable<CanvasSize>, IEquatable<CanvasPoint>, ISerializable
        {
            //TODO: MAKE IMPLICIT CASTS TO OTHER POINT TYPES
            #region Constructors

            public static readonly CanvasPoint Unset = new CanvasPoint(double.NaN, double.NaN);
            public CanvasPoint()
            {
                this.X = double.NaN;
                this.Y = double.NaN;
            }

            public CanvasPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
            public CanvasPoint(SerializationInfo info, StreamingContext context)
            {
                this.X = info.GetDouble("X");
                this.Y = info.GetDouble("Y");
            }

            #endregion

            #region Properties
            private double _x;
            private double _y;
            public double X
            {
                get { return _x; }
                set
                {
                    if (value != _x)
                    {
                        _x = value;
                        OnPropertyChanged("X");
                    }
                }
            }
            public double Y
            {
                get { return _y; }
                set
                {
                    if (value != _y)
                    {
                        _y = value;
                        OnPropertyChanged("Y");
                    }
                }
            }

            #endregion

            #region Methods

            public Vector2 ToVector()
            {
                return new Vector2((float)X, (float)Y);
            }

            public override string ToString()
            {
                return ($"CanvasPoint({this.X.ToString()}, {this.Y.ToString()})");
            }

            public static CanvasPoint operator +(CanvasPoint A, CanvasPoint B)
            {
                return new CanvasPoint((A.X + B.X), (A.Y + B.Y));
            }
            public static CanvasPoint operator -(CanvasPoint A, CanvasPoint B)
            {
                return new CanvasSize((A.X - B.X), (A.Y - B.Y));
            }
            public static CanvasPoint operator /(CanvasPoint A, CanvasPoint B)
            {
                return new CanvasPoint((A.X / B.X), (A.Y / B.Y));
            }
            public static CanvasPoint operator /(CanvasPoint A, int B)
            {
                return new CanvasPoint((A.X / B), (A.Y / B));
            }
            public static CanvasPoint operator /(CanvasPoint A, double B)
            {
                return new CanvasPoint((A.X / B), (A.Y / B));
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

            #endregion

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("X", this.X);
                info.AddValue("Y", this.Y);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Canvas Size
        /// </summary>
        [Serializable]
        public class CanvasSize : Observable, IEquatable<CanvasSize>, IEquatable<CanvasPoint>, ISerializable
        {
            #region Constructors

            public static readonly CanvasSize Unset = new CanvasSize(double.NaN, double.NaN);

            public CanvasSize()
            {
                this.Width = double.NaN;
                this.Height = double.NaN;
            }

            public CanvasSize(double w, double h)
            {
                this.Width = w;
                this.Height = h;
            }
            public CanvasSize(SerializationInfo info, StreamingContext context)
            {
                this.Width = info.GetDouble("Width");
                this.Height = info.GetDouble("Height");
            }

            #endregion

            #region Properties
            private double _width;
            private double _height;
            public double Width
            {
                get { return _width; }
                set
                {
                    if (value != _width)
                    {
                        _width = value;
                        OnPropertyChanged("Width");
                    }
                }
            }
            public double Height
            {
                get { return _height; }
                set
                {
                    if (value != _height)
                    {
                        _height = value;
                        OnPropertyChanged("Height");
                    }
                }
            }

            #endregion

            #region Methods

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

            #endregion

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Width", this.Width);
                info.AddValue("Height", this.Height);
            }
        }
    }
}
