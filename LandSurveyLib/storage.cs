using System;
using System.Windows;
//using System.Windows.Controls;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Win32;


namespace joef.controls
{
    public class storage
    {

        public string isolatedStrorageFilename { get; set; }
        public string filter { get; set; }
        public string defaultExt { get; set; }


        public bool isolatedStrorageFileExists
        {
            get
            {
                bool result = false;
                using( var appStore = IsolatedStorageFile.GetUserStoreForApplication() )
                {
                    result = appStore.FileExists( this.isolatedStrorageFilename );
                }
                return result;
            }
        }

        public static C loadFromFile<C>( string fileName ) where C : class, new()  // C is for collection
        {
            C collection = null;
            FileStream fileStream = File.Open( fileName, FileMode.Open );
            collection = loadDataFromStream<C>( fileStream );
            return collection;
        }

        //public C loadFromIsolatedStorage<C>() where C : class, new()  // C is for collection
        //{
        //    C collection = null;

        //    using( var appStore = IsolatedStorageFile.GetUserStoreForApplication() )
        //    {
        //        IsolatedStorageFileStream fileStream = null;
        //        bool fileExists = appStore.FileExists( this.isolatedStrorageFilename );

        //        if( fileExists )   // Deserializing a previously non-empty xml file will throw an error
        //        {
        //            fileStream = appStore.OpenFile( this.isolatedStrorageFilename,
        //                                           FileMode.Open );
        //            collection = this.loadDataFromStream<C>( fileStream );
        //        }
        //        else
        //            collection = new C();
        //    }
        //    return collection;
        //}

        public static C loadDataFromStream<C>( Stream fileStream ) where C : class, new()  // C is for collection
        {
            C collection = null;

            try
            {
                collection = loadData<C>( fileStream );
            }
            finally
            {
                if( fileStream != null )
                    fileStream.Close();
            }

            return collection;
        }

        private static C loadData<C>( Stream stream ) where C : class  // C is for collection
        {
            C result = default( C );
            if( stream != null )
            {
                XmlSerializer xmlSerializer = new XmlSerializer( typeof( C ) );
                result = xmlSerializer.Deserialize( stream ) as C;
            }
            return result;
        }




        //public void saveToIsolatedStorage<C>( C collection ) where C : class, new()  // C is for collection
        //{
        //    using( var store = IsolatedStorageFile.GetUserStoreForApplication() )
        //    {
        //        IsolatedStorageFileStream fileStream = null;
        //        fileStream = store.OpenFile( isolatedStrorageFilename,
        //                                    FileMode.OpenOrCreate );
        //        this.saveData<C>( fileStream, collection );
        //    }
        //}




        }

}
