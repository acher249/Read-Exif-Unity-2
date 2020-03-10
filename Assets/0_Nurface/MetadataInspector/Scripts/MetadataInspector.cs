using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System.Linq;
using System.IO;
using System.Net;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MetadataInspector : MonoBehaviour {

    // A String, String dictionary to save the metadata Key, Value
    public Dictionary<string, string> metadata = new Dictionary<string, string>();

    // Public Filedata 
    public FileData fileData;

    // The Image to be shown in the inspector window
    public Texture2D image;

    // Metadata directories
    public IEnumerable<MetadataExtractor.Directory> directories;

    // Use this for initialization
    void Start() {
        // Make image something so it's not null
        if (image == null) { image = new Texture2D(128, 128); }
        // Check if any file has been loaded yet
        if (fileData == null) {
            // Create a new FileData class to save important info
            fileData = new FileData();
        }
        // File Data already exists
        else {
            // Make sure there is a path value
            if (fileData.filePath != null && fileData.filePath != "") {
                Debug.Log(fileData.filePath);
                // Load metadata from the path
                LoadMetadataFromPath(fileData.filePath);
            }
        }
    }

    // This function is called when the script is loaded or a value is changed in the inspector(Called in the editor only).
    void OnValidate() {
#if UNITY_EDITOR
        // Set dirty on this gameobject to force an inspector update
        EditorUtility.SetDirty(gameObject);
#endif
    }

    // Load the Metadata
    public void LoadMetadataFromPath(string filePath) {
        // Load the image from file
        WWW www = new WWW("file:///" + filePath);
        // Load image into texture
        www.LoadImageIntoTexture(image);
        // Clear all existing dictionary values
        metadata.Clear();
        // Save file path
        fileData.filePath = filePath;
        // Read all metadata using ImageMetaDataReader class
        directories = ImageMetadataReader.ReadMetadata(filePath);
        // For each of the directories
        foreach (var directory in directories) {
            // AND all of their tags..
            foreach (var tag in directory.Tags) {
                //Debug.Log(directory.Name + " - " + tag.Name + " = " + tag.Description);
                // Check if the key already exists in the dictionary
                if (metadata.ContainsKey(tag.Name)) {
                    // Should create a new tag name here, in case of duplicate key names (example: "Image size")
                }
                // Key is not in dictionary
                else {
                    // Try to add value to dictionary
                    try {
                        // Add the key and value to the dictionary
                        metadata.Add(tag.Name, tag.Description);
                    }
                    // In case of any errors, like Shift-JIS encoding errors, etc
                    catch (Exception ex) {
                        // Log a warning
                        Debug.LogWarning(ex);
                    }
                }
            }
        }
    }

    // Serialized Class for File Data
    [Serializable]
    public class FileData {
        // Public string for file path, automatically serialized
        public string filePath;
    }

    // Load a Unity Texture from a URL
    private IEnumerator LoadTextureFromURL(string url) {
        WWW www = new WWW(url);
        // Keep looping until www is valid
        yield return www;
        // Load image into texture
        if (www.isDone) {
            // Update image's texture with the url image
            www.LoadImageIntoTexture(image);
            // Call onValidate manually to make sure the picture gets updated
            OnValidate();
        }
    }

    // Load Metadata from Stream into Dictionary
    private void LoadMetadataFromStream(Stream stream) {
        // Clear all existing dictionary values
        metadata.Clear();
        // Save file path to nothing
        fileData.filePath = "";
        // Read all metadata using ImageMetaDataReader class
        directories = ImageMetadataReader.ReadMetadata(stream);
        // For each of the directories
        foreach (var directory in directories) {
            // AND all of their tags..
            foreach (var tag in directory.Tags) {
                //Debug.Log(directory.Name + " - " + tag.Name + " = " + tag.Description);
                // Check if the key already exists in the dictionary
                if (metadata.ContainsKey(tag.Name)) {
                    // Should create a new tag name here, in case of duplicate key names (example: "Image size")
                }
                // Key is not in dictionary
                else {
                    // Try to add value to dictionary
                    try {
                        // Add the key and value to the dictionary
                        metadata.Add(tag.Name, tag.Description);
                    }
                    // In case of any errors, like Shift-JIS encoding errors, etc
                    catch (Exception ex) {
                        // Log a warning
                        Debug.LogWarning(ex);
                    }
                }
            }
        }
    }

    // Get an Image from a URL using C#.Net
    // http://arunsabat.blogspot.com/2011/02/get-image-from-image-url-using-cnet.html
    // *******************************************
    public void LoadMetadataFromURL(string Url) {
        // Array of bytes that make the image file
        byte[] imageData = DownloadData(Url);
        //ImageDetail imgDetail = new ImageDetail();
        //Image img = null;
        try {
            // Create a new MemoryStream from the imageData
            MemoryStream stream = new MemoryStream(imageData);
            //img = Image.FromStream(stream);
            
            // Load the Metadata from the image in memory
            LoadMetadataFromStream(stream);
            // Close the stream
            stream.Close();
            // Load the image into a texture (for the Editor)
            Debug.Log("Coroutine is getting called");
            StartCoroutine(LoadTextureFromURL(Url));
        }
        catch (Exception) {
        }   
    }
        
    private byte[] DownloadData(string Url) {
        string empty = string.Empty;
        return DownloadData(Url, out empty);
    }
    
    private byte[] DownloadData(string Url, out string responseUrl) {
        byte[] downloadedData = new byte[0];
        try {
            //Get a data stream from the url
            WebRequest req = WebRequest.Create(Url);
            WebResponse response = req.GetResponse();
            Stream stream = response.GetResponseStream();

            responseUrl = response.ResponseUri.ToString();

            //Download in chuncks
            byte[] buffer = new byte[1024];

            //Get Total Size
            //int dataLength = (int)response.ContentLength;

            //Download to memory
            //Note: adjust the streams here to download directly to the hard drive
            MemoryStream memStream = new MemoryStream();
            while (true) {
                //Try to read the data
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0) {
                    break;
                }
                else {
                    //Write the downloaded data
                    memStream.Write(buffer, 0, bytesRead);
                }
            }
            //Convert the downloaded stream to a byte array
            downloadedData = memStream.ToArray();

            //Clean up
            stream.Close();
            memStream.Close();
        }
        catch (Exception) {
            responseUrl = string.Empty;
            return new byte[0];
        }

        return downloadedData;
    }

}
