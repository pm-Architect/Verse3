﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verse3.Core.Types
{
    public class BoundingBox : IEquatable<BoundingBox>
    {
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

        public CanvasPoint Location { get; set; }
        public CanvasSize Size { get; set; }
        public double Left
        {
            get
            {
                return this.Location.X;
            }
        }
        public double Right
        {
            get
            {
                return (this.Location.X + this.Size.Width);
            }
        }
        public double Top
        {
            get
            {
                return this.Location.Y;
            }
        }
        public double Bottom
        {
            get
            {
                return (this.Location.Y + this.Size.Height);
            }
        }

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
                new CanvasPoint(this.Top, this.Left),
                new CanvasPoint(this.Top, this.Right),
                new CanvasPoint(this.Bottom, this.Right),
                new CanvasPoint(this.Bottom, this.Left)
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
            return ((A.Size == B.Size) && (A.Location == B.Location));
        }
        public static bool operator !=(BoundingBox A, BoundingBox B)
        {
            return ((A.Size != B.Size) || (A.Location != B.Location));
        }
    }
}
