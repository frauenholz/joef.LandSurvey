using System;

using System.ComponentModel;
using System.Collections.Generic;

namespace joef.controls
{
    public class DatalistActions<T> : INotifyPropertyChanged
                                      where T : INotifyPropertyChanged, new()
    {
        public DatalistActions( IList<T> list )
        {
            if( list == null )
                throw new ArgumentNullException( "datalistActions.ctor" );

            this.list = list;
        }
        public IList<T> list { get; private set; }

        public T add()
        {
            T t = new T();
            this.list.Add( t );
            return t;
        }

        public void remove( T t )
        {
            if( t != null )
                this.list.Remove( t );
        }

        // this class moves items physical position in the actual underlying list, to move only in a view use PagedCollectionView

        public void moveUp( T t ) // toward the zero index
        {
            if( t != null )
            {
                int i = this.list.IndexOf( t );
                if( i > 0 )
                    i--;

                this.list.Remove( t );
                this.list.Insert( i, t );
            }
        }

        public void moveDown( T t ) // away from the zero index
        {
            if( t != null )
            {
                int i = this.list.IndexOf( t );
                if( i < this.list.Count - 1 )
                    i++;

                this.list.Remove( t );
                this.list.Insert( i, t );
            }
        }


        public T selected
        {
            get
            {
                return this.selected_;
            }
            set
            {
                this.selected_ = value;
                //        this.OnPropertyChanged("selected");
                this.OnPropertyChanged( "addEnabled" );
                this.OnPropertyChanged( "removeEnabled" );
                this.OnPropertyChanged( "upEnabled" );
                this.OnPropertyChanged( "downEnabled" );
            }
        }
        private T selected_;


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged( string s )
        {
            if( this.PropertyChanged != null )
                this.PropertyChanged( this, new PropertyChangedEventArgs( s ) );
        }


        public bool addEnabled
        {
            get { return true; }
        }

        public bool removeEnabled
        {
            get
            {
                return this.selected != null &&
                       this.list.IndexOf( this.selected ) >= 0;
            }
        }

        public bool upEnabled
        {
            get
            {
                return this.selected != null &&
                       this.list.IndexOf( this.selected ) > 0;
            }
        }

        public bool downEnabled
        {
            get
            {
                return this.selected != null &&
                       this.list.IndexOf( this.selected ) < this.list.Count - 1;
            }
        }

    }
}
