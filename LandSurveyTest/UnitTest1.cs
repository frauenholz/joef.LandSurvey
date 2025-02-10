using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using joef.landEngineering;
using joef.controls;
using System.Diagnostics;

namespace LandSurvey.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod11()
        {
//            string filename = "parcelDescription 436-R-150.xml";
            string filename = "parcelDescription 436-L-110.xml";

            string dir = @"A:\usarios\Joe\OneDrive\dev\app\glosry\landSurvey\landArea_parcelData";
            string strorageFilename = Path.Combine( dir,
                                                    filename );
            vectorList vectorList = storage.loadFromFile<vectorList>( strorageFilename );

            // filter and convert vector type
            var vv = from v in vectorList
                     where v.groupName != ""
                     select new
                     {
                         groupName = v.groupName,
                         vector = new LandArea.Vector( v.bearing.ns,
                                                       v.bearing.deg,
                                                       v.bearing.min,
                                                       Convert.ToUInt16( Math.Round( v.bearing.sec ) ),
                                                       v.bearing.ew,
                                                       v.displacement )
                     };


            var xx = from x in vv
                     group x by x.groupName;

            double sqFtArea = 0;
            foreach( var item in xx )
            {
                string parcel = item.Key;

                List<LandArea.Vector> lot = new List<LandArea.Vector>();
                foreach( var item1 in item )
                    lot.Add( item1.vector );

                sqFtArea = misc.getArea( lot.ToArray() );
            }

            var this_sqFtTextBox_Text = sqFtArea.ToString();
            var this_acreTextBox_Text = ( sqFtArea / 43560 ).ToString();
        }

        [TestMethod]
        public void TestMethod12()
        {
            //LandArea.Vector[] lot = new LandArea.Vector[]   // 436-L-100
            //                         {
            //                           new LandArea.Vector(LandArea.NS.S, 28, 0, 0, LandArea.EW.W,
            //                                              200),
            //                           new LandArea.Vector(LandArea.NS.S, 89, 15, 0, LandArea.EW.W,
            //                                              171.09),
            //                           new LandArea.Vector(LandArea.NS.N, 28, 0, 0, LandArea.EW.E,
            //                                              200),
            //                           new LandArea.Vector(LandArea.NS.N, 89, 15, 0, LandArea.EW.E,
            //                                              171.09)
            //                         };
            LandArea.Vector[] lot = new LandArea.Vector[]   // 436-L-110
                                       {
                                     new LandArea.Vector(LandArea.NS.S, 88, 58, 0, LandArea.EW.W,
                                                        355.41),
                                     new LandArea.Vector(LandArea.NS.N, 23, 21, 10, LandArea.EW.E,
                                                        101.35),
                                     new LandArea.Vector(LandArea.NS.N, 89, 34, 50, LandArea.EW.E,
                                                        272.31),
                                     new LandArea.Vector(LandArea.NS.S, 72, 57, 20, LandArea.EW.E,
                                                        22.14),
                                     new LandArea.Vector(LandArea.NS.S, 83, 12, 0, LandArea.EW.E,
                                                        63.43),
                                     new LandArea.Vector(LandArea.NS.S, 28, 56, 50, LandArea.EW.W,
                                                        85.29)
                                       };
            double sqFtArea = misc.getArea( lot.ToArray() );

            var this_sqFtTextBox_Text = sqFtArea.ToString();
            var this_acreTextBox_Text = ( sqFtArea / 43560 ).ToString();

        }

    }



    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        public void TestMethod21()
        {
            LandArea.Vector[] lot = new LandArea.Vector[]   // 436-L-100
                                       {
                                   new LandArea.Vector(LandArea.NS.S, 28, 0, 0, LandArea.EW.W,
                                                      200),
                                   new LandArea.Vector(LandArea.NS.S, 89, 15, 0, LandArea.EW.W,
                                                      171.09),
                                   new LandArea.Vector(LandArea.NS.N, 28, 0, 0, LandArea.EW.E,
                                                      200),
                                   new LandArea.Vector(LandArea.NS.N, 89, 15, 0, LandArea.EW.E,
                                                      171.09)
                                       };
            printVectors( lot );
            var textBlock1_Text = misc.getArea( lot ).ToString();

        }

        [TestMethod]
        public void OpenFile()
        {
//            string filename = "parcelDescription 436-R-150.xml";
            string filename = "parcelDescription 436-L-110.xml";

            string dir = @"A:\usarios\Joe\OneDrive\dev\app\glosry\landSurvey\landArea_parcelData";
            string strorageFilename = Path.Combine( dir,
                                                    filename );
            vectorList vectorList = storage.loadFromFile<vectorList>( strorageFilename );

            Debug.WriteLine( "" );
            Debug.WriteLine( "" );
            Debug.WriteLine( strorageFilename );
            Debug.WriteLine( "" );

            
            // filter and convert vector type
            var vv = from v in vectorList
                     where v.groupName != ""
                     select new
                     {
                         groupName = v.groupName,
                         vector = new LandArea.Vector( ( LandArea.NS )v.bearing.ns,
                                                       v.bearing.deg,
                                                       v.bearing.min,
                                                       Convert.ToUInt16( Math.Round( v.bearing.sec ) ),
                                                       ( LandArea.EW )v.bearing.ew,
                                                       v.displacement )
                     };


            var xx = from x in vv
                     group x by x.groupName;

            foreach( var item in xx )
            {
                string parcel = item.Key;

                List<LandArea.Vector> lot = new List<LandArea.Vector>();
                foreach( var item1 in item )
                    lot.Add( item1.vector );

                this.printVectors( lot.ToArray() );
                double area = misc.getArea( lot.ToArray(),
                                            this.printTriangles,
                                            this.printLastArea );
            }
        }


        private void printVectors( LandArea.Vector[] lot )
        {
            Debug.WriteLine( "" );
            foreach( LandArea.Vector vector in lot )
            {
                Debug.WriteLine( $"{vector.Direction.CompassBearing.ToString()} • {vector.Length.ToString()}        ({vector.Direction.CartesianDegrees}°, {vector.Direction.Quadrant}, {vector.Direction.QuadrantNum})" );
            }
            Debug.WriteLine( "" );
            Debug.WriteLine( "" );
        }

        private void printTriangles( LandArea.Triangle triangle, int i )
        {
            Debug.WriteLine( i.ToString() + ") " +
                            triangle.Turn.RotationClock.ToString() +
                            ", area:" + triangle.Area.f() + "', " + ( triangle.Area / 43560 ).f() +
                            " | " +
                            "a:" + triangle.a.f() +
                            ", b:" + triangle.b.f() +
                            ", c:" + triangle.c.f() +
                            " | " +
                            "1:" + triangle.cartesianDegrees1.f() +
                            ", 2:" + triangle.cartesianDegrees2.f() +
                            ", 3:" + triangle.cartesianDegrees3.f() +
                            " | " +
                            "A:" + triangle.A.f() +
                            ", B:" + triangle.B.f() +
                            ", C:" + triangle.C.f()
                            );
            Debug.WriteLine( "" );
        }

        private void printLastArea( double areaCCW, double areaCW, double area )
        {
            Debug.WriteLine( "" );
            Debug.WriteLine( "areaCCW:" + areaCCW.f() + "', " + ( areaCCW / 43560 ).f() );
            Debug.WriteLine( "areaCW: " + areaCW.f() + "', " + ( areaCW / 43560 ).f() );

            Debug.WriteLine( "area:   " + area.f() + "', " + Math.Round( area / 43560, 4 ).ToString() );
            Debug.WriteLine( "" );
            Debug.WriteLine( "" );
        }

    }


    static class extention
    {
        public static string f( this double d )
        {
            return Math.Round( d, 2 ).ToString();
        }

        //public static string AsString( this Tuple<NS, ushort, ushort, ushort, EW> compassBearing )
        //{
        //    return ""; 
        //}
    }


}
