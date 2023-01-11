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
        public static ArrayList GetFilesBySearchPattern(string filePath, string searchPattern, SearchOption searchOption)
        {
            if (string.IsNullOrEmpty(searchPattern))
                return null;

            string[] arrExt = searchPattern.Split('|');
            ArrayList arrLstFiles = new ArrayList();
            for (int i = 0; i < arrExt.Length; i++)
            {
                arrLstFiles.AddRange(Directory.GetFiles(filePath, arrExt[i], searchOption));
            }
            return arrLstFiles;
        }

        public static ArrayList GetFilesByProperty(ArrayList files, string fileProperty, string filePropertyValue)
        {
            ArrayList arrLstFiles = new ArrayList();
            foreach (string file in files)
            {
                if (filePropertyValue.Equals(GetExtendedFileProperty(file, fileProperty), StringComparison.OrdinalIgnoreCase))
                {
                    arrLstFiles.Add(file);
                }
            }
            return arrLstFiles;
        }

        public static string GetExtendedFileProperty(string filePath, string propertyName)
        {
            string value = string.Empty;
            string baseFolder = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            //Method to load and execute the Shell object for Windows server 8 environment otherwise you get "Unable to cast COM object of type 'System.__ComObject' to interface type 'Shell32.Shell'"
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            Shell32.Folder shellFolder = (Shell32.Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { baseFolder });

            //ParseName will find the specific file I'm looking for in the Shell32.Folder object
            Shell32.FolderItem folderitem = shellFolder.ParseName(fileName);
            if (folderitem != null)
            {
                for (int i = 0; i < short.MaxValue; i++)
                {
                    //Get the property name for property index i
                    string property = shellFolder.GetDetailsOf(null, i);

                    //Will be empty when all possible properties has been looped through, break out of loop
                    if (String.IsNullOrEmpty(property)) break;

                    //Skip to next property if this is not the specified property
                    if (property == propertyName)
                    {
                        //Read value of property
                        value = shellFolder.GetDetailsOf(folderitem, i);
                        break;
                    }
                }
            }
            //returns string.Empty if no value was found for the specified property
            return value;
        }
    }
}
