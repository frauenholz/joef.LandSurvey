using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;
using joef.controls;
using joef.landEngineering;


namespace joef.landEngineering
{
    public partial class SurveyDataControl : UserControl, INotifyPropertyChanged
    {
        public SurveyDataControl()
        {
            InitializeComponent();
            //saveOnExitCheckBox.DataContext = userOptions;
            //dropShadow.DataContext = userOptions;
            this.userOptions.PropertyChanged += new PropertyChangedEventHandler( userOptions_PropertyChanged );

            this.userOptions.saveOnExit = true;
            this.userOptions.showDropShadow = true;
        }

        fileIO storage = new fileIO()
        {
            isolatedStrorageFilename = "parcelDescription.xml",
            filter = "xml files *.xml|*.xml",
            defaultExt = "xml"
        };


        public ICollectionView CollectionView
        {
            get
            {
                return this.collectionView;
            }
            set
            {
                bool changed = this.collectionView != value;

                if( changed )
                {
                    if ( this.collectionView != null )
                    {
                        this.collectionView.CollectionChanged -= this.CollectionView_CollectionChanged;
                        this.collectionView.CurrentChanged -= this.CollectionView_CurrentChanged;
                    }

                    this.collectionView = value;

                    if ( this.collectionView != null )
                    {
                        this.collectionView.CollectionChanged += this.CollectionView_CollectionChanged;
                        this.collectionView.CurrentChanged += this.CollectionView_CurrentChanged;
                    }

                    this.notifyPropertyChanged( "CollectionView" );

                    // side effects
                    this.notifyPropertyChanged( "Area" );
                }
            }
        }
        private ICollectionView collectionView;

        private void CollectionView_CurrentChanged( object sender, EventArgs e )
        {
            if( this.datalistActions != null )
                this.datalistActions.selected = this.CurrentVector;

            if ( this.parcelControl != null )
                this.parcelControl.selectedVector = this.CurrentVector;


            this.notifyPropertyChanged( "CurrentVector" );

            this.notifyPropertyChanged( "AngleBefore" );
            this.notifyPropertyChanged( "AngleAfter" );
        }

        private void CollectionView_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            // vector added or removed
            this.notifyPropertyChanged( "Area" );

            // vector added or removed is possibly next to selected vector
            this.notifyPropertyChanged( "AngleBefore" );
            this.notifyPropertyChanged( "AngleAfter" );
        }



        public vectorList vectorList
        {
            get
            {
                return this.vectorList_;
            }
            set
            {
                bool changed = this.vectorList_ != value;


                if( changed )
                {

                    this.vectorList_ = value;

                    // side effects
                    this.CollectionView = CollectionViewSource.GetDefaultView( this.vectorList_ );

                    this.parcelControl.vectorList = this.vectorList_;


                    // event leaks from over written instance???????
                    this.datalistActions = new DatalistActions<vector>( this.vectorList_ );
                    this.addButton.DataContext = this.datalistActions;
                    this.deleteButton.DataContext = this.datalistActions;
                    this.moveUpButton.DataContext = this.datalistActions;
                    this.moveDownButton.DataContext = this.datalistActions;
                }
            }
        }
        private vectorList vectorList_;



        public vector CurrentVector
        {
            get
            {
                return this.CollectionView?.CurrentItem as vector;
            }
        }


        public DatalistActions<vector> datalistActions { get; private set; }



        private void loadFromFileButton_Click( object sender, RoutedEventArgs e )
        {
            this.vectorList = this.storage.loadFromFile<vectorList>();
        }


        private void afterLoadDataFromStream()
        {
            try
            {
                if( this.vectorList != null )
                {
                    // references are not explicitly persited, must determine the original reference from the persited copy
                    foreach( vector vector in this.vectorList )
                    {
                        //// if there is a specified reference, determine the real veator
                        //if (vector.followsAsString != null)
                        //{
                        //  // look in the list for the real vector with the same signature
                        //  var v = from vv in this.vectorList
                        //          where vv.asString == vector.followsAsString
                        //          select vv;
                        //  if (v.Count() > 0)  // possible that more that one vector has the same signature
                        //    vector.follows = v.First();   // assign the actual vector
                        //}


                        // need to add handler here, OnDeserializedAttribute method does not fire, and the handler added in the vector.ctor does not work
                        vector.bearing.PropertyChanged += new PropertyChangedEventHandler( vector.bearing_PropertyChanged );
                    }
                }
                else
                    this.vectorList = new vectorList();
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }
        }



        private void saveToFileButton_Click( object sender, RoutedEventArgs e )
        {
            this.storage.saveToFile<vectorList>( this.vectorList );
        }




        private void addButton_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            vector vector = this.datalistActions.add();

            // default to visible
            vector.visible = true;

            // copy the previous vector groupName
            if( this.vectorList.Count > 0 )
                vector.groupName = this.vectorList[ this.vectorList.Count - 1 ].groupName;

            this.dataGrid.ScrollIntoView( vector, null );
        }

        private void deleteButton_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            vector vector = this.dataGrid.SelectedItem as vector;
            if( vector != null )
                this.datalistActions.remove( vector );
        }

        private void moveUpButton_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            vector vector = this.dataGrid.SelectedItem as vector;
            if( vector != null )
            {
                this.datalistActions.moveUp( vector );

                this.dataGrid.SelectedItem = vector;
            }
        }

        private void moveDownButton_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            vector vector = this.dataGrid.SelectedItem as vector;
            if( vector != null )
            {
                this.datalistActions.moveDown( vector );

                this.dataGrid.SelectedItem = vector;
            }
        }

        private void clearCellButton_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            vector vector = this.dataGrid.SelectedItem as vector;
            if( vector != null )
            {
                DataGridColumn c = this.dataGrid.CurrentColumn;

                if( c == this.dataGrid.Columns[ 0 ] )
                    vector.displacement = 0;

                else
                if( c == this.dataGrid.Columns[ 1 ] )
                    vector.bearing.clear();

                else
                if( c == this.dataGrid.Columns[ 2 ] )
                    vector.groupName = null;

                //else
                //if (c == this.dataGrid.Columns[3])
                //  vector.follows = null;
            }
        }

        public double Area
        {
            get
            {
                if ( this.vectorList != null )
                {
                    LandArea.Vector[] v = this.vectorList.Select( v0 => v0.ConverT() ).ToArray();
                    return Math.Round( misc.getArea( v ), 2 );
                }
                else
                    return 0;
            }
        }

        public double AngleBefore
        {
            get
            {
                double result = double.NaN;

                if ( this.vectorList != null &&
                     this.CurrentVector != null )
                {
                    vector previousVector = null;

                    if ( this.CurrentVector.Equals( this.vectorList.First() ) )
                        previousVector = this.vectorList.Last();
                    else
                    {
                        int i = this.vectorList.IndexOf( this.CurrentVector );
                        if ( i > -1 )
                            previousVector = this.vectorList[ i - 1];
                    }

                    if( previousVector != null )
                    {
                        var triangle = new LandArea.Triangle( this.CurrentVector.ConverT(), previousVector.ConverT() );
                        result = triangle.C;
                        result = Math.Round( result, 2 );
                    }
                    else
                        result = double.NaN;
                }

                return result;
            }
        }

        public double AngleAfter
        {
            get
            {
                double result = double.NaN;

                if ( this.vectorList != null &&
                     this.CurrentVector != null )
                {
                    vector nextVector = null;

                    if ( this.CurrentVector.Equals( this.vectorList.Last() ) )
                        nextVector = this.vectorList.First();
                    else
                    {
                        int i = this.vectorList.IndexOf( this.CurrentVector );
                        if ( i < this.vectorList.Count )
                            nextVector = this.vectorList[ i + 1 ];
                    }

                    if( nextVector != null )
                    {
                        var triangle = new LandArea.Triangle( this.CurrentVector.ConverT(), nextVector.ConverT() );
                        result = triangle.C;
                        result = Math.Round( result, 2 );
                    }
                    else
                        result = double.NaN;
                }

                return result;
            }
        }






        //private void followsComboBox_Loaded(object sender, RoutedEventArgs e)
        //{
        //  // set items source here, cannot use ElementName Bindining inside of an ItemsControl
        //  if (sender is ComboBox)
        //  {
        //    ComboBox c = (ComboBox)sender;

        //    c.ItemsSource = this.vectorList;


        //    // refresh selected binding because doing the ItemsSource binding here is too late
        //    BindingExpression x = c.GetBindingExpression(ComboBox.SelectedItemProperty);
        //    if (x != null &&
        //        x.DataItem is vector)
        //      ((vector)x.DataItem).refreshFollowsHack();

        //  }
        //}

        private userOptions userOptions = new userOptions();

        void userOptions_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if( e.PropertyName == "saveOnExit" )
            {
            }
            else
            if( e.PropertyName == "showDropShadow" )
                if( this.userOptions.showDropShadow )
                {
                    System.Windows.Media.Effects.DropShadowEffect dropShadowEffect = new System.Windows.Media.Effects.DropShadowEffect();
                    this.parcelControl.Effect = dropShadowEffect;
                    dropShadowEffect.BlurRadius = 10;
                    dropShadowEffect.ShadowDepth = 10;
                    Color color = Color.FromArgb( 0xFF, 0x73, 0xA9, 0xD8 );
                    dropShadowEffect.Color = color;
                    dropShadowEffect.Opacity = 0.5;
                }
                else
                    this.parcelControl.Effect = null;
        }

        private void Slider_ValueChanged( object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e )
        {
            this.sliderValueChanged( e.NewValue );
        }

        private void sliderValueChanged( double newValue )
        {
            // resize function controls
            if( this.parcelControl != null )
            {
                if( this.parcelControl != null &&
                    this.parcelControl.RenderTransform is TransformGroup )
                {
                    ScaleTransform t = ( ( TransformGroup )this.parcelControl.RenderTransform ).Children[ 0 ] as ScaleTransform;
                    if( t != null )
                    {
                        t.ScaleX = newValue;
                        t.ScaleY = newValue;
                    }
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void notifyPropertyChanged( string propertyName )
        {
            if( this.PropertyChanged != null )
                this.PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
        }

    }

    ////  ////

    public class nsList : List<LandArea.NS>
    {
        public nsList()
        {
            this.Add( LandArea.NS.N );
            this.Add( LandArea.NS.S );
        }
    }

    public class ewList : List<LandArea.EW>
    {
        public ewList()
        {
            this.Add( LandArea.EW.E );
            this.Add( LandArea.EW.W );
        }
    }
    ////  ////

    public class userOptions : INotifyPropertyChanged
    {
        public bool saveOnExit
        {
            set
            {
                this.saveOnExit_ = value;
                OnPropertyChanged( "saveOnExit" );
            }
            get
            {
                return this.saveOnExit_;
            }
        }
        private bool saveOnExit_;

        public bool showDropShadow
        {
            get
            {
                return this.showDropShadow_;
            }
            set
            {
                this.showDropShadow_ = value;
                this.OnPropertyChanged( "showDropShadow" );
            }
        }
        private bool showDropShadow_;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged( string s )
        {
            if( this.PropertyChanged != null )
                this.PropertyChanged( this, new PropertyChangedEventArgs( s ) );
        }
    }




    public class bearingDegreesConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            double result = double.NaN;

            if( value is vector )
            {
                vector v = ( vector )value;

                LandArea.Vector vector = v.ConverT();
                result = vector.Direction.CartesianDegrees;

                // adjust to cartisian, see landTriangle.fs
                result = result + 90;
                if ( result > 360 )
                    result = result - 360;

                result = Math.Round( result, 2 );
            }
            //if( value is rBearing )
            //{
            //    rBearing b = ( rBearing )value;

            //    LandArea.CompassBearing compassBearing = b.ConverT();
            //    result = compassBearing.degrees;
            //}

            return result;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }

}
