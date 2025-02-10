using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace joef.landEngineering
{

    [DataContract( IsReference = true )]
    public class vector : notify
    {
        public vector()
        {
            this.bearing = new Bearing();
//            this.start = new location();

            // when the bearing changes, notify that this asString has changed
            this.bearing.PropertyChanged += new PropertyChangedEventHandler( bearing_PropertyChanged );
        }

        public void bearing_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if( e.PropertyName == "asString" )     // the bearing has changed so
                this.notifyPropertyChanged( "asString" ); // update this which shows the bearing
        }


        [DataMember()]
        public Bearing bearing
        {
            get
            {
                return this.bearing_;
            }
            set
            {
                bool changed = this.bearing_ != value;

                if( changed )
                {
                    if ( this.bearing_ != null )
                    {
                        this.bearing_.PropertyChanged -= this.Bearing_PropertyChanged;
                    }

                    this.bearing_ = value;

                    if ( this.bearing_ != null )
                    {
                        this.bearing_.PropertyChanged += this.Bearing_PropertyChanged;
                    }

                    this.notifyPropertyChanged( "bearing" );
                }
            }
        }

        private void Bearing_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            this.notifyPropertyChanged( "bearing" );
        }

        private Bearing bearing_;


        [DataMember()]
        public double displacement
        {
            get
            {
                return displacement_;
            }
            set
            {
                displacement_ = value;
                this.notifyPropertyChanged( "displacement" );
                this.notifyPropertyChanged( "asString" );
            }
        }
        private double displacement_;


        // at least one vector must have a starting point
 //       [DataMember()]
 //       public location start { get; set; }

        // if the vector does not have a starting point, then it must follow a vector (start where another vector ends)
        //  [System.Xml.Serialization.XmlIgnoreAttribute]
        //public vector  follows
        //{
        //  get
        //  {
        //    return follows_;
        //  }
        //  set
        //  {
        //    follows_ = value;

        //    if (follows_ != null)
        //      followsAsString_ = follows_.asString;
        //    else
        //      followsAsString_ = null;

        //    this.OnPropertyChanged("follows");
        //  }
        //}
        //private vector  follows_;

        //  [DataMember()]    // string representaion of the object, do not serialize the follows object because infinite nesting is serialization problem
        //public string  followsAsString
        //{
        //  get
        //  {
        //    return followsAsString_;
        //  }
        //  set   // needed for serialization
        //  {
        //    followsAsString_ = value;
        //  }
        //}
        //private string  followsAsString_;

        // a vector on one parcel may coincide with a vector on an adjacent parcel
        //      [DataMember()]
        //public vector  coincides
        //{
        //  get
        //  {
        //    return coincides_;
        //  }
        //  set
        //  {
        //    coincides_ = value;
        //    this.OnPropertyChanged("coincides");
        //  }
        //}
        //private vector  coincides_;

        // also used as a unique identifier
        public string asString
        {
            get
            {
                return string.Format( "{0}    {1}    {2}", new object[] {this.displacement_,
                                                                this.bearing.asString,
                                                                this.groupName_} );
            }
        }

        [DataMember()]
        public string groupName
        {
            get
            {
                return groupName_;
            }
            set
            {
                groupName_ = value;
                this.notifyPropertyChanged( "groupName" );
                this.notifyPropertyChanged( "asString" );
            }
        }
        private string groupName_;

        [DataMember()]
        public bool visible
        {
            get
            {
                return visible_;
            }
            set
            {
                visible_ = value;
                this.notifyPropertyChanged( "visible" );
            }
        }
        public bool visible_;


        public override string ToString()
        {
            return this.asString;
        }
    }


    ////  ////

    //[DataContract()]    //  0  1  2  3
    //public enum NS
    //{
    //    [EnumMember( Value = "North" )]
    //    N = LandArea.NS.N,

    //    [EnumMember( Value = "South" )]
    //    S = LandArea.NS.S
    //}
    //[DataContract()]    //  0  1  2  3
    //public enum EW
    //{
    //    [EnumMember( Value = "East" )]
    //    E = LandArea.EW.E,

    //    [EnumMember( Value = "West" )]
    //    W = LandArea.EW.W
    //}

    ////  ////

    [DataContract()]
    public class Bearing : notify
    {
        [DataMember()]
        public LandArea.NS ns
        {
            get
            {
                return ns_;
            }
            set
            {
                bool changed = this.ns_ != value;

                if( changed )
                {
                    this.ns_ = value;
                    this.notifyPropertyChanged( "ns" );

                    this.notifyPropertyChanged( "CartesianDegrees" );
                    this.notifyPropertyChanged( "asString" );
                }
            }
        }
        private LandArea.NS ns_;

        [DataMember()]
        public byte deg
        {
            get
            {
                return deg_;
            }
            set
            {
                bool changed = this.deg_ != value;

                if( changed )
                {
                    this.deg_ = value;
                    this.notifyPropertyChanged( "deg" );

                    this.notifyPropertyChanged( "CartesianDegrees" );
                    this.notifyPropertyChanged( "asString" );
                }
            }
        }
        private byte deg_;

        [DataMember()]
        public byte min
        {
            get
            {
                return min_;
            }
            set
            {
                bool changed = this.min_ != value;

                if( changed )
                {
                    this.min_ = value;
                    this.notifyPropertyChanged( "min" );

                    this.notifyPropertyChanged( "CartesianDegrees" );
                    this.notifyPropertyChanged( "asString" );
                }
            }
        }
        private byte min_;

        [DataMember()]
        public double sec
        {
            get
            {
                return sec_;
            }
            set
            {
                bool changed = this.sec != value;

                if( changed )
                {
                    this.sec_ = value;
                    this.notifyPropertyChanged( "sec" );

                    this.notifyPropertyChanged( "CartesianDegrees" );
                    this.notifyPropertyChanged( "asString" );
                }
            }
        }
        private double sec_;

        [DataMember()]
        public LandArea.EW ew
        {
            get
            {
                return ew_;
            }
            set
            {
                bool changed = this.ew_ != value;

                if( changed )
                {
                    this.ew_ = value;
                    this.notifyPropertyChanged( "ew" );

                    this.notifyPropertyChanged( "CartesianDegrees" );
                    this.notifyPropertyChanged( "asString" );
                }
            }
        }
        private LandArea.EW ew_;

        public double CartesianDegrees
        {
            get
            {
                return LandArea.GetCartesianDegrees( this.ns,
                                                     this.deg,
                                                     this.min,
                                                     this.sec,
                                                     this.ew );
            }
            set
            {
                LandArea.CompassBearing compassBearing = LandArea.GetCompassBearing( value );

                this.ns = compassBearing.ns;
                this.deg = ( byte )compassBearing.degrees;
                this.min = ( byte )compassBearing.minutes;
                this.sec = compassBearing.seconds;
                this.ew = compassBearing.ew;
                //bool changed = this.cartesianDegrees != value;

                //if( changed )
                //{
                //    this.cartesianDegrees = value;
                //    this.NotifyPropertyChanged( "CartesianDegrees" );
                //}
            }
        }

        public string asString
        {
            get
            {
                return string.Format( "{0} {1}° {2}′ {3}″ {4}",
                                      new object[] {
                                                        this.ns,
                                                        this.deg,
                                                        this.min,
                                                        this.sec,
                                                        this.ew
                                                    }
                                    );
            }
        }

        public void clear()
        {
            ns_ = LandArea.NS.N;
            deg_ = 0;
            min_ = 0;
            sec_ = 0;
            ew_ = LandArea.EW.W;
            this.notifyPropertyChanged( "asString" );
        }
    }

    ////  ////

    [DataContract()]
    public class longLat : notify
    {
        //  [DataMember()]
        //public NS  direction { get; set; }

        [DataMember()]
        public double displacement { get; set; }
    }

    ////  ////

    [DataContract()]
    public class location
    {
        [DataMember()]
        public longLat longitude { get; set; }

        [DataMember()]
        public longLat latitude { get; set; }
    }

    ////  ////

    [DataContract( IsReference = true )]
    public class notify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void notifyPropertyChanged( string s )
        {
            if( this.PropertyChanged != null )
                this.PropertyChanged( this, new PropertyChangedEventArgs( s ) );
        }
    }

    ////  ////

    [CollectionDataContract()]
    public class vectorList : ObservableCollection<vector>
    {
        protected override void InsertItem( int index, vector item )
        {
            base.InsertItem( index, item );

            item.PropertyChanged += vector_PropertyChanged;
        }

        private void vector_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
        }

        protected override void RemoveItem( int index )
        {
            vector item = this[ index ];
            item.PropertyChanged -= vector_PropertyChanged;

            base.RemoveItem( index );
        }
        //protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //  base.OnCollectionChanged(e);
        //}

        //  [DataMember()]
        //public int maxPk
        //{
        //  get
        //  {
        //    int m = 0;
        //    if (this.Count > 0)
        //      m = this.Max(v => v.pk);

        //    return m;
        //  }
        //set
        //{
        //  maxPk_ = value;
        //}
        //}
        //private int maxPk_;

    }

    ////  ////

//    public enum tRotation
//    {
//        clockwise = LandArea.ClockRotation.CW,          // NE, SW          - cartisian rotation
//        counterClockwise = LandArea.ClockRotation.CCW,  // SE, NW          + cartisian rotation
//        straigtAhead = LandArea.ClockRotation.straigtAhead,
//        reverse = LandArea.ClockRotation.reverse
//    }

    ////  ////


    public static class xxxxExtension
    {
        public static LandArea.Vector ConverT( this vector v )
        {
            LandArea.Vector vector = new LandArea.Vector( v.bearing.ns,
                                                          v.bearing.deg,
                                                          v.bearing.min,
                                                          Convert.ToUInt16( Math.Round( v.bearing.sec ) ),
                                                          v.bearing.ew,
                                                          v.displacement );
            return vector;
        }

//        public static LandArea.CompassBearing ConverT( this Bearing b )
//        {
//            LandArea.CompassBearing bearing = new LandArea.CompassBearing( b.ns,
//                                                                           b.deg,
//                                                                           b.min,
//                                                                           Convert.ToUInt16( Math.Round( b.sec ) ),
//                                                                           b.ew );
//            return bearing;
//        }

    }



    public class misc
    {
        private static double DegToRad( double degrees )
        {
            double result = 0;
            result = degrees / 360 * 2 * Math.PI;
            return result;
        }

        public static double GetCartesianDegrees( vector vector )
        {
            double degrees = LandArea.GetCartesianDegrees( vector.bearing.ns,
                                                           vector.bearing.deg,
                                                           vector.bearing.min,
                                                           Convert.ToUInt16( Math.Round( vector.bearing.sec ) ),
                                                           vector.bearing.ew );
            return degrees;
        }

        public static double cartisianX( vector vector )
        {
            double result = 0;
            if( vector != null )
            {
                //        double degrees = DMStoCartisianRotation(vector.bearing);
                double degrees = misc.GetCartesianDegrees( vector );
                result = -1 * Math.Cos( misc.DegToRad( degrees - 90 ) ) * vector.displacement;
            }
            return result;
        }

        public static double cartisianY( vector vector )
        {
            double result = 0;
            if( vector != null )
            {
                double degrees = misc.GetCartesianDegrees( vector );
                result = -1 * Math.Sin( misc.DegToRad( degrees - 90 ) ) * vector.displacement;
            }
            return result;
        }


        public static double getArea( LandArea.Vector[] lot )
        {
            return getArea( lot,
                            null,
                            null );
        }
        public static double getArea( LandArea.Vector[] lot,
                                      Action<LandArea.Triangle,int> printTrainglesAction,
                                      Action<double,double,double> printLastTriangleAction )
        {
            double areaCCW = 0;
            double areaCW = 0;

            LandArea.Triangle triangle = null;

            for( int i = 1; i < lot.Length; i++ )
            {

                LandArea.Vector previousVector = null;
                if( i == 1 )
                    previousVector = lot[ 0 ];
                else
                if( triangle != null )
                    previousVector = new LandArea.Vector( triangle.cartesianDegrees3,
                                                        triangle.c );

                LandArea.Vector nextVector = lot[ i ];

                triangle = new LandArea.Triangle( previousVector, nextVector );

                if ( printTrainglesAction != null )
                    printTrainglesAction( triangle, i );

                if( triangle.RotationClock == LandArea.ClockRotation.CCW )
                    areaCCW += triangle.Area;
                else
                if( triangle.RotationClock == LandArea.ClockRotation.CW )
                    areaCW += triangle.Area;
                else
                if( triangle.RotationClock == LandArea.ClockRotation.straigtAhead )
                {
                }
                else
                if( triangle.RotationClock == LandArea.ClockRotation.reverse )
                {
                }

            }
            if ( triangle != null )
            {

                double areaLastTriange = triangle.Area;
                double area = Math.Abs( areaCCW - areaCW );

                if( printLastTriangleAction != null )
                    printLastTriangleAction( areaCCW, areaCW, area );

                return area;
            }
            else
                return 0;
        }
        
        //    private static bool validNsews(NS ns,
        //                                   EW ew)
        //    {
        ////  can't combine N and S, or E and W
        //      bool result = false;
        //      result = (byte)ns + (byte)ew > 1 &&
        //               (byte)ns + (byte)ew < 5;
        //      return result;
        //    }

        //private static tRotation  rotationFromNsews(NS ns,
        //                                            EW ew)
        //{
        //  tRotation  result = tRotation.straigtAhead;

        //  int  s = (byte)ns + (byte)ew;

        //  if ((s == 2 || s == 4))
        //    result = tRotation.clockwise;         // NE, SW
        //  else
        //  if (s == 3)
        //    result = tRotation.counterClockwise; // SE, NW

        //  return result;
        //}

        //private static short getCartisianFromNsew(NS  ns)
        //{
        //  short result = 0;
        //  switch (ns)
        //  {
        //    case NS.N:
        //      result = 90;
        //      break;
        //    case NS.S:
        //      result = 270;
        //      break;
        //    default:
        //      break;
        //  }
        //  return result;
        //}
        //private static short getCartisianFromEW(EW  ew)
        //{
        //  short result = 0;
        //  switch (ew)
        //  {
        //    case EW.E:
        //      result = 180;
        //      break;
        //    case EW.W:
        //      result = 0;
        //      break;
        //    default:
        //      break;
        //  }
        //  return result;
        //}

        //private static short getRotationFactor(NS ns,
        //                                       EW ew)
        //{
        //  short result = 0;

        //  if (rotationFromNsews(ns, ew) == tRotation.clockwise)
        //    result = -1;
        //  else
        //    result = 1;

        //  return  result;
        //}

        //public static double  DMStoCartisianRotation(rBearing  aBearing)
        //{
        //  double result = 0;

        //  short rotationFactor = 0;
        //  double degrees = 0;

        //  rotationFactor = getRotationFactor(aBearing.ns, aBearing.ew);

        //  degrees = getCartisianFromNsew(aBearing.ns);
        //  result = degrees +
        //           rotationFactor * (aBearing.deg + aBearing.min/60 + aBearing.sec/3600);

        //  return result;
        //}

        //private static double  validCartisianRange(double aDeg)
        //{
        //  double  result = aDeg;

        //  while (result < 0)
        //    result = result + 360;
        //  while (result > 360)
        //    result = result - 360;

        //  return result;
        //}

        //    public static double  NsewDMStoCartisian(rBearing  aBearing)
        //    {
        //      // Degree minute seconds to cartisian degrees
        //      double  result = 0;
        //      double  startDeg = 0;
        //      double  rotate  = 0;

        ////  Assert(validNsews(aBearing.nsew0, aBearing.nsew00));
        //      startDeg = getCartisianFromNsew(aBearing.ns);
        //      rotate = DMStoCartisianRotation(aBearing);

        //      result = validCartisianRange(startDeg + rotate);

        //      return result;
        //    }

        //private static void  flipNsew(rBearing aBearing)
        //{
        //  if (aBearing.ns == NS.N)
        //    aBearing.ns = NS.S;
        //  else
        //  if (aBearing.ns == NS.S)
        //    aBearing.ns = NS.N;

        //  if (aBearing.ew == EW.E)
        //    aBearing.ew = EW.W;
        //  else
        //  if (aBearing.ew == EW.W)
        //    aBearing.ew = EW.E;
        //}

        //private static double  getTurn(rBearing  aBearing0,
        //                               rBearing  aBearing1)
        //{
        //  double  result = 0;
        //  result = 180 - (DMStoCartisianRotation(aBearing0) - DMStoCartisianRotation(aBearing1));
        //  return result;
        //}

    }

    ////  ////

}
