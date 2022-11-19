using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SaveXML
{
    /// <summary>
    /// <see cref="FileOperations"/> provides methods for saving and loading XML files and converting them to class objects.
    /// </summary>
    public class FileOperations
    {
        /// <summary>
        /// <see cref="XMLFile"/> represents a XML file object that is wrapped with some additional metadata.
        /// </summary>
        public class XMLFile
        {
            string fileName;
            string xmlData;
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="filename">Complete file path of the saved (or to be saved) file.</param>
            public XMLFile(string filename)
            {
                fileName = filename;
            }
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="filename">Complete file path of the saved (or to be saved) file.</param>
            /// <param name="XMLdata">XML Data of that is to be saved or loaded.</param>
            public XMLFile(string filename, string XMLdata)
            {
                fileName = filename;
                XMLData = XMLdata;
            }
            /// <summary>
            /// File location with filename.
            /// </summary>
            public string Filename
            {
                get { return fileName; }
                set { fileName = value; }
            }
            /// <summary>
            /// XML data in form of string.
            /// </summary>
            public string XMLData
            {
                get { return xmlData; }
                set { xmlData = value; }
            }
            /// <summary>
            /// Gets <see cref="Object"/> form of XML Data. This can be explicitly converted to corresponding class object.
            /// </summary>
            /// <returns>Returns <see cref="Object"/>.</returns>
            public Object GetObject()
            {
                return Converters.CreateObject(xmlData, new Object());
            }
            /// <summary>
            /// Sets class object that will be converted to string.
            /// </summary>
            /// <param name="YourClassObject">Your class object that will be converted to XML string.</param>
            public void SetObject(Object YourClassObject)
            {
                XMLData = Converters.CreateXML(YourClassObject);
            }
        }
        /// <summary>
        /// Containes reponses returned by methods.
        /// </summary>
        public enum RESPONSE_CODES
        {
            /// <summary>
            /// <see cref="FILE_EXISTS"/> is returned when file already exists on the given location.
            /// </summary>
            FILE_EXISTS,
            /// <summary>
            /// <see cref="SAVE_FAILED"/> is returned when an unknown error is encountered when saving file.
            /// </summary>
            SAVE_FAILED,
            /// <summary>
            /// <see cref="SAVE_SUCCESS"/> is returned when file is saved successfully.
            /// </summary>
            SAVE_SUCCESS
        }
        /// <summary>
        /// Saves <see cref="XMLFile"/> by overwriting or creating a new file.
        /// </summary>
        /// <param name="xmlFile">XMLFile object that contains file settings and data.</param>
        /// <param name="overwrite">If true then existing file be be overwritten.</param>
        /// <returns>returns <see cref="RESPONSE_CODES"/>.</returns>
        public static RESPONSE_CODES Save(XMLFile xmlFile, bool overwrite)
        {
            if (overwrite == false)
            {
                if (File.Exists(xmlFile.Filename) == true)
                {
                    return RESPONSE_CODES.FILE_EXISTS;
                }
            }
            try
            {
                File.WriteAllText(xmlFile.Filename, Converters.CreateXML(xmlFile));
            }
            catch (Exception) { return RESPONSE_CODES.SAVE_FAILED; }
            return RESPONSE_CODES.SAVE_SUCCESS;
        }
        /// <summary>
        /// Saves <see cref="XMLFile"/> by overwriting or creating a new file. Encrypts data before saving if encryption key is provided.
        /// </summary>
        /// <param name="xmlFile">XMLFile object that contains file settings and data.</param>
        /// <param name="overwrite">If true then existing file be be overwritten.</param>
        /// <param name="encryptionKey">Encryption key that will encrypt the data before saving.</param>
        /// <returns>returns <see cref="RESPONSE_CODES"/>.</returns>
        public static RESPONSE_CODES Save(XMLFile xmlFile, bool overwrite, string encryptionKey)
        {
            if (encryptionKey != null && encryptionKey.Trim().Length > 0)
                xmlFile.XMLData = Crypto.EncryptStringAES(xmlFile.XMLData, encryptionKey);
            return Save(xmlFile, overwrite);
        }
        /// <summary>
        /// Loads <see cref="XMLFile"/> from given <see cref="FilePath"/>
        /// </summary>
        /// <param name="FilePath">Complete path including filename.</param>
        /// <returns>Returns <see cref="XMLFile"/> object.</returns>
        public static XMLFile Load(string FilePath)
        {
            return (XMLFile)Converters.CreateObject(File.ReadAllText(FilePath), new XMLFile(FilePath));
        }
        /// <summary>
        /// Loads <see cref="XMLFile"/> from given <see cref="FilePath"/> and decrypts data if decryption key is provided.
        /// </summary>
        /// <param name="FilePath">Complete path including filename.</param>
        /// <returns>Returns <see cref="XMLFile"/> object.</returns>
        public static XMLFile Load(string FilePath, string decryptionKey)
        {
            XMLFile xf = Load(FilePath);
            if (decryptionKey != null && decryptionKey.Trim().Length > 0)
                xf.XMLData = Crypto.DecryptStringAES(xf.XMLData, decryptionKey);
            return xf;
        }
    }
}
