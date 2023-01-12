using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeGenrePlaylist
{
    public static class FileHelper
    {
        /// <summary>Gets a list of files by a specific search pattern.</summary>
        /// <param name="filePath">The folder in which to search for files.</param>
        /// <param name="searchPattern">The search pattern.  Multiple patterns are separated by the '|' character.</param>
        /// <param name="searchOption">The <see cref="SearchOption"/>.</param>
        /// <returns>
        /// An <see cref="ArrayList" of the files matching the search pattern(s)./>
        /// </returns>
        public static ArrayList GetFilesBySearchPattern(string filePath, string searchPattern, SearchOption searchOption)
        {
            if (string.IsNullOrEmpty(searchPattern))
                return null;

            string[] arrExt = searchPattern.Split('|');
            ArrayList arrLstFiles = new ArrayList();
            for (int i = 0; i < arrExt.Length; i++)
            {
                Console.Write($"{arrLstFiles.Count,-6}");
                arrLstFiles.AddRange(Directory.GetFiles(filePath, arrExt[i], searchOption));
                Console.Write("\b\b\b\b\b\b");
            }
            return arrLstFiles;
        }

        /// <summary>Contains a list of only the Property Name to be stored.</summary>
        private static List<string> storedProperties;

        /// <summary>Gets a list of files containing file properties.  The list only contains those files that have a specific property's value.</summary>
        /// <param name="files">A list of pathnames to search through.</param>
        /// <param name="fileProperty">The file property name being searched for.</param>
        /// <param name="filePropertyValue">The value that property must contain to be include in the returned list of files.</param>
        /// <returns>A list of files whose specific property name is set to the specified value.</returns>
        public static List<Dictionary<string, string>> GetFilePropertiesBySpecificProperty(ArrayList files, string fileProperty, string filePropertyValue)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            storedProperties = new List<string>()
            {
                fileProperty, "Album", "Contributing artists", "Title", "Authors", "Length"
            };
            int counter = 1;
            foreach (string file in files)
            {
                Console.Write($"{list.Count, 4}/{counter++, -6}");
                Dictionary<string, string> keyValuePairs = GetFileExtendedProperties(file, fileProperty, filePropertyValue);
                if (!(keyValuePairs is null))
                {
                    list.Add(keyValuePairs);
                }
                Console.Write("\b\b\b\b\b\b\b\b\b\b\b");
            }
            return list;
        }

        /// <summary>Gets the file's extended properties for files that match a specific property's value.</summary>
        /// <param name="filePath">The full path name of the file.</param>
        /// <param name="propertyName">The Name of the specific property to filter on.</param>
        /// <param name="propertyValue">The Value of the specific property to filter on.</param>
        /// <returns>A List of Key Value Pairs containing the extended properties.  If the file doesn't contain the specific property and/or its value, null is returned.</returns>
        private static Dictionary<string, string> GetFileExtendedProperties(string filePath, string propertyName, string propertyValue)
        {
            bool isFound = false;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "Pathname", filePath }
            };
            string baseFolder = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            //Method to load and execute the Shell object for Windows server 8 environment otherwise you get "Unable to cast COM object of type 'System.__ComObject' to interface type 'Shell32.Shell'"
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Shell32.IShellDispatch ishell = (Shell32.IShellDispatch)Activator.CreateInstance(shellAppType);
            Shell32.Folder shellFolder = ishell.NameSpace(baseFolder);

            //ParseName will find the specific file I'm looking for in the Shell32.Folder object
            Shell32.FolderItem folderitem = shellFolder.ParseName(fileName);
            if (folderitem != null)
            {
                for (int i = 0; i < short.MaxValue; i++)
                {
                    //Get the property name for property index i
                    string property = shellFolder.GetDetailsOf(null, i);

                    //Will be empty when all possible properties has been looped through, break out of loop
                    if (String.IsNullOrEmpty(property))
                        break;

                    if (storedProperties.Contains(property))
                    {
                        //Get the value of the property
                        string value = shellFolder.GetDetailsOf(folderitem, i);
                        if (String.IsNullOrWhiteSpace(value))
                            continue;

                        //Check if this is the specific property we are searching for
                        if (!isFound && property.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (value.Equals(propertyValue, StringComparison.InvariantCultureIgnoreCase))
                                isFound = true;
                            else
                                break;
                        }
                        //Store the property and its value in the list of key/value pairs
                        keyValuePairs[property] = value;
                    }
                }
            }
            return isFound ? keyValuePairs : null;
        }
    }
}
