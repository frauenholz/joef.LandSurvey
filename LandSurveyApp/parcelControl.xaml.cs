using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

namespace joef.landEngineering
{
    public partial class parcelControl : UserControl
    {
        public parcelControl()
        {
            InitializeComponent();

            this.cartesianVectorPathList.cartesianVectorPathChanged += new Action<cartesianVectorPathList, cartesianVectorPath>( cartesianVectorPathList_cartesianVectorPathChanged );
        }

        //void vectorList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //  this.draw(this.vectorList);
        //}


        void cartesianVectorPathList_cartesianVectorPathChanged( cartesianVectorPathList arg1, cartesianVectorPath arg2 )
        {
            this.draw( null );

            vector v = this.selectedVector;
            this.selectedVector = null;
            this.selectedVector = v;
        }

        private cartesianVectorPathList cartesianVectorPathList = new cartesianVectorPathList();

        public vectorList vectorList
        {
            get { return ( vectorList )GetValue( vectorListProperty ); }
            set { SetValue( vectorListProperty, value ); }
        }

        public static readonly DependencyProperty vectorListProperty =
            DependencyProperty.Register( "vectorList", typeof( vectorList ), typeof( parcelControl ), new PropertyMetadata( vectorList_changedCallback ) );

        private static void vectorList_changedCallback( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            // dp side effects

            if( d is parcelControl &&
                e.NewValue is vectorList )
            {
                parcelControl parcelControl = ( parcelControl )d;
                vectorList vectorList = ( vectorList )e.NewValue;
                parcelControl.draw( ( vectorList )e.NewValue );

                // hook undelying change event
                parcelControl.vectorList.CollectionChanged += ( sender1, e1 ) =>
                {
                    parcelControl.draw( parcelControl.vectorList );
                };
            }
        }

        private void draw( vectorList vectorList )
        {
            if( this != null )
            {
                Canvas canvas = ( Canvas )this.Content;

                // if the vevectorList == null, cartesianVectorPathList does not change
                if( vectorList != null )
                    this.cartesianVectorPathList.vectorList = vectorList;

                boundOrigin boundOrigin = this.cartesianVectorPathList.boundOrigin();
                this.Width = boundOrigin.width;
                this.Height = boundOrigin.height;

                Point origin = new Point( boundOrigin.origin.X,
                                         boundOrigin.origin.Y );

                canvas.Children.Clear();

                DrawFill( canvas,
                          this.cartesianVectorPathList,
                          origin );
                DrawPaths( canvas,
                          this.cartesianVectorPathList,
                          origin );
            }
        }

        private static void DrawPaths( Panel panel,
                                      cartesianVectorPathList cartesianVectorPathList,
                                      Point origin )
        {
            // draws individual paths that can be controled seperately

            if( panel != null &&
                cartesianVectorPathList != null )
            {
                Path path = null;

                Point vectorStart = origin;
                double offsetX = 0;
                double offsetY = 0;

                foreach( cartesianVectorPath item in cartesianVectorPathList )
                {
                    LineSegment lineSegment = new LineSegment();

                    // note: screen Y is opposite of cartesian Y
                    offsetX = vectorStart.X + item.offsetX;
                    offsetY = vectorStart.Y - item.offsetY;

                    lineSegment.Point = new Point( offsetX,
                                                   offsetY );


                    PathFigure pathFigure = new PathFigure();
                    pathFigure.StartPoint = vectorStart;
                    pathFigure.Segments.Add( lineSegment );

                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add( pathFigure );

                    path = item.path;
                    path.Stroke = new SolidColorBrush( Colors.Black );
                    //path.StrokeEndLineCap = PenLineCap.Square;
                    //path.StrokeLineJoin = PenLineJoin.Bevel;
                    path.StrokeThickness = 1;
                    path.Data = pathGeometry;

                    panel.Children.Add( path );

                    // begin the next path where the last one left off
                    vectorStart = new Point( offsetX,
                                             offsetY );
                }
            }
        }

        private static void DrawFill( Panel panel,
                                      cartesianVectorPathList cartesianVectorPathList,
                                      Point origin )
        {
            // draw the background of the shape

            if( panel != null &&
                cartesianVectorPathList != null )
            {
                Path path = null;

                Point vectorStart = origin;
                double offsetX = 0;
                double offsetY = 0;

                PathFigure pathFigure = new PathFigure();
                pathFigure.StartPoint = vectorStart;

                foreach( cartesianVectorPath item in cartesianVectorPathList )
                //          if (item.vector.visible)
                {
                    LineSegment lineSegment = new LineSegment();

                    // note: screen Y is opposite of cartesian Y
                    offsetX = vectorStart.X + item.offsetX;
                    offsetY = vectorStart.Y - item.offsetY;

                    lineSegment.Point = new Point( offsetX,
                                                  offsetY );


                    pathFigure.Segments.Add( lineSegment );

                    // begin the next path where the last one left off
                    vectorStart = new Point( offsetX,
                                            offsetY );
                }

                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add( pathFigure );

                path = new Path();
                path.Fill = new SolidColorBrush( Colors.LightGray );
                path.Data = pathGeometry;

                panel.Children.Add( path );
            }
        }

        public vector selectedVector
        {
            get
            {
                return this.cartesianVectorPathList.selectedVector;
            }
            set
            {
                this.cartesianVectorPathList.selectedVector = value;
            }
        }
    }

    ////  ////

    // takes vector data and calculated offsets to be applied to corresponding visual path objects
    public class cartesianVectorPath
    {
        public cartesianVectorPath( vector vector )
        {
            this.vector = vector;
            this.vector.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler( vector_PropertyChanged );

            this.path = new Path();

            this.setVisibility();
        }

        public vector vector { get; private set; }
        public Path path { get; private set; }
        public double offsetX
        {
            get
            {
                return misc.cartisianX( this.vector );
            }
        }
        public double offsetY
        {
            get
            {
                return misc.cartisianY( this.vector );
            }
        }

        private void setVisibility()
        {
            if( this.vector.visible )
                this.path.Visibility = Visibility.Visible;
            else
                this.path.Visibility = Visibility.Collapsed;
        }

        public event Action<cartesianVectorPath> vectorChanged;
        void vector_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            if( vectorChanged != null )
                vectorChanged( this );

            this.setVisibility();
        }
    }

    ////  ////

    public class cartesianVectorPathList : List<cartesianVectorPath>
    {
        public cartesianVectorPathList()
        {
            this.selectedPathStroke = new SolidColorBrush( Colors.Red );
            this.nonSelectedPathStroke = new SolidColorBrush( Colors.Black );
        }


        public vectorList vectorList
        {
            get
            {
                return this.vectorList_;
            }
            set
            {
                this.vectorList_ = value;

                this.Clear();

                if( vectorList_ != null )
                {
                    foreach( vector v in vectorList_ )
                    {
                        cartesianVectorPath cartesianVectorPath = new cartesianVectorPath( v );
                        this.Add( cartesianVectorPath );
                    }

                    vectorList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler( vectorList_CollectionChanged );
                }
                //this.OnPropertyChanged("vectorList");
            }
        }
        private vectorList vectorList_;


        void vectorList_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
        {
            if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add )
            {
                if( e.NewItems.Count > 0 &&
                    e.NewItems[ 0 ] is vector )
                {
                    cartesianVectorPath x = new cartesianVectorPath( ( vector )e.NewItems[ 0 ] );
                    this.Add( x );
                }
            }
            else
            if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove )
            {
                if( e.OldItems.Count > 0 &&
                    e.OldItems[ 0 ] is vector )
                {
                    var v = from item in this
                            where item.vector == ( vector )e.OldItems[ 0 ]
                            select item;
                    if( v.Count() > 0 )
                    {
                        cartesianVectorPath x = v.First();
                        this.Remove( x );
                    }
                }
            }
        }

        public new void Add( cartesianVectorPath cartesianVectorPath )
        {
            cartesianVectorPath.vectorChanged += new Action<cartesianVectorPath>( cartesianVectorPath_vectorChanged );

            cartesianVectorPath.path.Stroke = this.nonSelectedPathStroke;

            base.Add( cartesianVectorPath );
        }

        void cartesianVectorPath_vectorChanged( cartesianVectorPath obj )
        {
            if( this.cartesianVectorPathChanged != null )
                this.cartesianVectorPathChanged( this, obj );
        }

        public event Action<cartesianVectorPathList, cartesianVectorPath> cartesianVectorPathChanged;

        public boundOrigin boundOrigin()  // determine where in the control to start drawing and determine the size of the control
        {
            double offsetX = 0;
            double offsetY = 0;

            double minX = 0;
            double maxX = 0;
            double minY = 0;
            double maxY = 0;

            foreach( cartesianVectorPath cartesianVectorPath in this )
            {
                offsetX = offsetX + cartesianVectorPath.offsetX;
                offsetY = offsetY + cartesianVectorPath.offsetY;

                minX = Math.Min( minX, offsetX );
                maxX = Math.Max( maxX, offsetX );
                minY = Math.Min( minY, offsetY );
                maxY = Math.Max( maxY, offsetY );
            }


            boundOrigin result = new boundOrigin();
            result.origin.X = minX * -1;  // total horizontal displacement from origin
            result.origin.Y = maxY;       // total vertical displacement from origin
            result.width = maxX - minX;
            result.height = maxY - minY;

            return result;
        }

        public vector selectedVector
        {
            set
            {
                foreach( var item in this )
                {
                    if( item.vector == value )
                    {
                        item.path.Stroke = this.selectedPathStroke;

                        
                        // get existing PathGeometry
                        PathGeometry pathGeometry = ( PathGeometry )item.path.Data;

                        // create new PathFigure
                        PathFigure pathFigure = new PathFigure();
                        pathFigure.StartPoint = ( ( LineSegment )pathGeometry.Figures.First().Segments.First() ).Point;

                        // create new LineSegment
                        LineSegment lineSegment = new LineSegment();
                        lineSegment.Point = new Point( pathFigure.StartPoint.X + 20,
                                                       pathFigure.StartPoint.Y + 20 );

                        // add new LineSegment to new PathFigure
                        pathFigure.Segments.Add( lineSegment );

                        // add new PathFigure to existing PathGeometry
                        pathGeometry.Figures.Add( pathFigure );

                    }
                    else
                    {
                        item.path.Stroke = this.nonSelectedPathStroke;


                        // get existing PathGeometry
                        PathGeometry pathGeometry = ( PathGeometry )item.path.Data;

                        if ( pathGeometry.Figures.Last() != pathGeometry.Figures.First() )
                        {
                            // remove extra PathFigure from existing PathGeometry
                            pathGeometry.Figures.Remove( pathGeometry.Figures.Last() );
                        }
                    }
                }
            }
            get
            {
                var v = from item in this
                        where item.path.Stroke == this.selectedPathStroke
                        select item;
                if( v.Count() > 0 )
                    return v.First().vector;
                else
                    return null;
            }
        }

        private SolidColorBrush selectedPathStroke; // the stroke to be used on the path that is currently selected
        private SolidColorBrush nonSelectedPathStroke;
    }

    ////  ////

    public struct boundOrigin
    {
        public boundOrigin( double offsetX,
                           double offsetY,
                           double width,
                           double height )
        {
            this.origin = new Point( offsetX, offsetY );
            this.width = width;
            this.height = height;
        }
        public double width;
        public double height;

        public Point origin;  // the point to start drawing so that the shape lands completely within the height and width
    }
}
