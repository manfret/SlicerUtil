using System.Collections.Generic;
using System.Linq;
using MSClipperLib;
using NUnit.Framework;
using Util;
using Util.GeometryBasics;
using Util.PolygonOptimizer;

namespace TestUtils.PolygonsOptimizer
{
    public partial class TestPolygonOptimizer
    {
        [Test]
        public void PolygonOptimizer_Optimize_OneSegmentPoligons()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(0, 1000);
            var s0 = new Segment(p0, p1);
            
            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(1000, 0);
            var s1 = new Segment(p2, p3);

            var polys = new List<List<Segment>> { new List<Segment> { s0 }, new List<Segment> { s1 } };

            var lastPoint = new IntPoint(-100, -100);
            var optimized = PolygonOptimizer.Optimize(polys, PolygonOptimizer.GetPointBySeamHiding, ref lastPoint, 0);

            Assert.AreEqual(p0, optimized[0].First().StartPoint);
            Assert.AreEqual(p2, optimized[1].First().StartPoint);
        }

        [Test]
        public void PolygonOptimizer_Optimize_OneSegmentPoligons_EndPointClosest()
        {
            var p0 = new IntPoint(0, 0);
            var p1 = new IntPoint(0, 1000);
            var s0 = new Segment(p0, p1);

            var p2 = new IntPoint(1000, 1000);
            var p3 = new IntPoint(1000, 0);
            var s1 = new Segment(p2, p3);

            var polys = new List<List<Segment>> { new List<Segment> { s0.Flip() }, new List<Segment> { s1 } };

            var lastPoint = new IntPoint(-100, -100);
            var optimized = PolygonOptimizer.Optimize(polys, PolygonOptimizer.GetPointBySeamHiding, ref lastPoint, 0);

            Assert.AreEqual(p0, optimized[0].First().StartPoint);
            Assert.AreEqual(p2, optimized[1].First().StartPoint);
        }

        [Test]
        public void PolygonOptimizer_Optimize_PlentySegmentsPolygons_ClosedPolys()
        {
            //https://yadi.sk/i/sCQmXxNu3afcG3

            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(6000, 0);
            var p02 = new IntPoint(6000, 4000);
            var p03 = new IntPoint(6000, 4000);
            var p04 = new IntPoint(0, 4000);
            var p05 = new IntPoint(3000, 2000);

            var p0 = new List<IntPoint> { p00, p01, p02, p03, p04, p05 }.ReorderList(2);
            var s0 = p0.PointsToSegments(true);

            var p10 = new IntPoint(10000, 0);
            var p11 = new IntPoint(16000, 0);
            var p12 = new IntPoint(16000, 4000);
            var p13 = new IntPoint(16000, 4000);
            var p14 = new IntPoint(10000, 4000);
            var p15 = new IntPoint(13000, 2000);

            var p1 = new List<IntPoint> { p10, p11, p12, p13, p14, p15 }.ReorderList(2);
            var s1 = p1.PointsToSegments(true);

            var lastPoint = new IntPoint(-1000, -1000);

            var optimized = PolygonOptimizer.Optimize(new List<List<Segment>>() {s0, s1}, PolygonOptimizer.GetPointBySeamHiding,
                ref lastPoint, 0);

            Assert.AreEqual(p05, optimized[0].First().StartPoint);
            Assert.AreEqual(p15, optimized[1].First().StartPoint);
        }

        [Test]
        public void PolygonOptimizer_Optimize_PlentySegmentsPolygons_OpenPolys()
        {
            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(6000, 0);
            var p02 = new IntPoint(6000, 4000);
            var p03 = new IntPoint(6000, 4000);
            var p04 = new IntPoint(0, 4000);
            var p05 = new IntPoint(3000, 2000);

            var p0 = new List<IntPoint> { p00, p01, p02, p03, p04, p05 }.ReorderList(2);
            var s0 = p0.PointsToSegments(false);

            var p10 = new IntPoint(10000, 0);
            var p11 = new IntPoint(16000, 0);
            var p12 = new IntPoint(16000, 4000);
            var p13 = new IntPoint(16000, 4000);
            var p14 = new IntPoint(10000, 4000);
            var p15 = new IntPoint(13000, 2000);

            var p1 = new List<IntPoint> { p10, p11, p12, p13, p14, p15 };
            var s1 = p1.PointsToSegments(false);

            var lastPoint = new IntPoint(-1000, -1000);

            var optimized = PolygonOptimizer.Optimize(new List<List<Segment>>() { s0, s1 }, PolygonOptimizer.GetPointBySeamHiding,
                ref lastPoint, 0);

            Assert.AreEqual(p01, optimized[0].First().StartPoint);
            Assert.AreEqual(p10, optimized[1].First().StartPoint);
        }

        [Test]
        public void PolygonOptimizer_SimpleOptimize_PolygonIsEmpty()
        {
            var lastPoint = new IntPoint(1000, 3000);

            var poly = new List<Segment> ();
            var tail = new List<Segment> ();
            var lst = new List<(List<Segment>, List<Segment>)>(){ };
            var simpleOptimized = PolygonOptimizer.OptimizeByStartPoint(lst, ref lastPoint);

            Assert.AreEqual(new List<List<Segment>>(), simpleOptimized);
        }

        [Test]
        public void PolygonOptimizer_SimpleOptimize_WithoutSeamHiding()
        {
            //https://yadi.sk/i/cPnd_9Ti3afggc

            var p00 = new IntPoint(4000, 0);
            var p01 = new IntPoint(6000, 0);
            var p02 = new IntPoint(6000, 2000);
            var p03 = new IntPoint(4000, 6000);
            var p04 = new IntPoint(4000, 5000);
            var p0 = new List<IntPoint> { p00,p01, p02, p03, p04 }.PointsToSegments(false);

            var p10 = new IntPoint(4000, 4000);
            var p11 = new IntPoint(4000, 3000);
            var p12 = new IntPoint(6000, 3000);
            var p13 = new IntPoint(6000, 5000);
            var p14 = new IntPoint(4000, 5000);
            var p1 = new List<IntPoint> { p10, p11, p12, p13, p14 }.PointsToSegments(false);

            var p20 = new IntPoint(8000, 3000);
            var p21 = new IntPoint(10000, 3000);
            var p22 = new IntPoint(10000, 5000);
            var p23 = new IntPoint(8000, 5000);
            var p24 = new IntPoint(8000, 4000);
            var p2 = new List<IntPoint> { p20, p21, p22, p23, p24 }.PointsToSegments(false);

            var p30 = new IntPoint(1000, 5000);
            var p31 = new IntPoint(2000, 5000);
            var p32 = new IntPoint(2000, 7000);
            var p33 = new IntPoint(0, 7000);
            var p34 = new IntPoint(0, 5000);
            var p3 = new List<IntPoint> { p30, p31, p32, p33, p34 }.PointsToSegments(false);

            var lastPoint = new IntPoint(1000, 3000);

            var tail = new List<Segment>();
            var lst = new List<(List<Segment>, List<Segment>)>() { (p0, tail), (p1, tail), (p2, tail), (p3, tail) };
            var simpleOptimized = PolygonOptimizer.OptimizeByStartPoint(lst, ref lastPoint);

            Assert.AreEqual(p3, simpleOptimized[0].polygon);
            Assert.AreEqual(p1, simpleOptimized[1].polygon);
            Assert.AreEqual(p2, simpleOptimized[2].polygon);
            Assert.AreEqual(p0, simpleOptimized[3].polygon);
        }

        [Test]
        public void PolygonOptimizer_OptimizeWithoutReorder_PolygonsEmpty()
        {
            var lastPoint = new IntPoint(-1000, -1000);

            var optimized = PolygonOptimizer.OptimizeWithoutReorder(new List<List<Segment>> { }, PolygonOptimizer.GetPointBySeamHiding, ref lastPoint);

            Assert.AreEqual(new List<List<Segment>>(), optimized);
        }

        [Test]
        public void PolygonOptimizer_OptimizeWithNesting_Nesting_Inner()
        {
            //https://yadi.sk/i/bfH4CnUGtMaq4Q

            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(8000, 0);
            var p02 = new IntPoint(8000, 3000);
            var p03 = new IntPoint(11000, 3000);
            var p04 = new IntPoint(11000, 2000);
            var p05 = new IntPoint(16000, 2000);
            var p06 = new IntPoint(16000, 7000);
            var p07 = new IntPoint(11000, 7000);
            var p08 = new IntPoint(11000, 6000);
            var p09 = new IntPoint(8000, 6000);
            var p10 = new IntPoint(8000, 8000);
            var p11 = new IntPoint(0, 8000);

            var p0 = new List<IntPoint> { p00, p01, p02, p03, p04, p05, p06, p07, p08, p09, p10, p11 };

            var p1 = p0.Offset(-1000);
            var p2 = p1.Offset(-1000);
            var p3 = p2.Offset(-1000);

            var p0s = new List<List<Segment>>() {p0.PointsToSegments(true)};
            var p1s = new List<List<Segment>>();
            foreach (var polygon in p1)
            {
                p1s.Add(polygon.PointsToSegments(true));
            }
            var p2s = new List<List<Segment>>();
            foreach (var polygon in p2)
            {
                p2s.Add(polygon.PointsToSegments(true));
            }
            var p3s = new List<List<Segment>>();
            foreach (var polygon in p3)
            {
                p3s.Add(polygon.PointsToSegments(true));
            }

            var perimeters = new List<List<List<Segment>>>() { p0s, p1s, p2s, p3s};

            var lastPoint = new IntPoint(16000, 0000);

            var optimized = PolygonOptimizer.OptimizeWithNesting(perimeters, ref lastPoint, 1500);

            Assert.AreEqual(5, optimized.Count);
            Assert.IsTrue(optimized[0].IsTheSamePoly(p0s[0]));
            Assert.IsTrue(optimized[1].IsTheSamePoly(p1s[0]));
            Assert.IsTrue(optimized[2].IsTheSamePoly(p2s[1]));
            Assert.IsTrue(optimized[3].IsTheSamePoly(p2s[0]));
            Assert.IsTrue(optimized[4].IsTheSamePoly(p3s[0]));

        }

        [Test]
        public void PolygonOptimizer_OptimizeWithNesting_Nesting_Outer()
        {
            //https://yadi.sk/i/bfH4CnUGtMaq4Q

            var p00 = new IntPoint(0, 0);
            var p01 = new IntPoint(8000, 0);
            var p02 = new IntPoint(8000, 3000);
            var p03 = new IntPoint(11000, 3000);
            var p04 = new IntPoint(11000, 2000);
            var p05 = new IntPoint(16000, 2000);
            var p06 = new IntPoint(16000, 7000);
            var p07 = new IntPoint(11000, 7000);
            var p08 = new IntPoint(11000, 6000);
            var p09 = new IntPoint(8000, 6000);
            var p10 = new IntPoint(8000, 8000);
            var p11 = new IntPoint(0, 8000);

            var p0 = new List<IntPoint> { p00, p01, p02, p03, p04, p05, p06, p07, p08, p09, p10, p11 };

            var p1 = p0.Offset(-1000);
            var p2 = p1.Offset(-1000);
            var p3 = p2.Offset(-1000);

            var p0s = new List<List<Segment>>() { p0.PointsToSegments(true) };
            var p1s = new List<List<Segment>>();
            foreach (var polygon in p1)
            {
                p1s.Add(polygon.PointsToSegments(true));
            }
            var p2s = new List<List<Segment>>();
            foreach (var polygon in p2)
            {
                p2s.Add(polygon.PointsToSegments(true));
            }
            var p3s = new List<List<Segment>>();
            foreach (var polygon in p3)
            {
                p3s.Add(polygon.PointsToSegments(true));
            }

            var perimeters = new List<List<List<Segment>>>() { p0s, p1s, p2s, p3s };

            var lastPoint = new IntPoint(16000, 0000);

            var optimized = PolygonOptimizer.OptimizeWithNesting(perimeters.GetReverse(), ref lastPoint, 1500);

            Assert.AreEqual(5, optimized.Count);
            Assert.IsTrue(optimized[0].IsTheSamePoly(p3s[0]));
            Assert.IsTrue(optimized[1].IsTheSamePoly(p2s[0]));
            Assert.IsTrue(optimized[2].IsTheSamePoly(p1s[0]));
            Assert.IsTrue(optimized[3].IsTheSamePoly(p0s[0]));
            Assert.IsTrue(optimized[4].IsTheSamePoly(p2s[1]));

        }

    }
}