using System;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Win32;

namespace joef.controls
{
    internal class fileIO : storage
    {
        public C loadFromFile<C>() where C : class, new()  // C is for collection
        {
            C collection = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = this.filter;
//            openFileDialog.InitialDirectory = @"A:\SkyDrive\app\_old\data\landArea_parcels";
            bool? ok = openFileDialog.ShowDialog();
            if( ok == true )
            {
                System.IO.Stream fileStream = openFileDialog.OpenFile();

                collection = loadDataFromStream<C>( fileStream );
            }
            else
                collection = new C();

            return collection;
        }

        public void saveToFile<C>( C collection ) where C : class, new()  // C is for collection
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = this.filter;
            saveFileDialog.DefaultExt = this.defaultExt;
            bool? ok = saveFileDialog.ShowDialog();
            if( ok == true )
            {
                System.IO.Stream fileStream = saveFileDialog.OpenFile();
                this.saveData<C>( fileStream, collection );
            }
        }
        public void saveToFile<C>( C collection,   // will throw exception, must save to isolated storage or from user initiated SaveFileDialog
                                  string filename ) where C : class, new()  // C is for collection
        {
            System.IO.Stream fileStream = new FileStream( filename, FileMode.OpenOrCreate );
            this.saveData<C>( fileStream, collection );
        }
        private void saveData<C>( Stream stream, C collection ) where C : class, new()  // C is for collection
        {
//            bool error = false;

            try
            {
                this.saveDataToSteam<C>( stream, collection );
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message );
//                error = true;
            }
            finally
            {
                if( stream != null )
                    stream.Close();
            }

//if( error )    // reload the last good file
//    this.loadFromIsolatedStorage<C>();
        }
        private void saveDataToSteam<C>( Stream stream, C collection ) where C : class  // C is for collection
        {
            if( stream != null )
            {
                // avoid xml or json write errors where there is extra text after the end tag
                stream.SetLength( 0 );

                XmlSerializer xmlSerializer = new XmlSerializer( typeof( C ) );
                xmlSerializer.Serialize( stream, collection );
            }
        }

    }
}
