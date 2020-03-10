using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CustomEditor(typeof(MetadataInspector))]
public class MetadataInspectorEditor : Editor {

    // Script associated with me
    private MetadataInspector myScript;

    // This function is called when the object is loaded.
    public void OnEnable() {
        // Find the MetadataInspector script on the cooresponding gameobject
        myScript = (MetadataInspector)target;
        // Make image is something so it's not null
        if (myScript.image == null) { myScript.image = new Texture2D(128, 128); }
        // Check if there is file data yet
        if (myScript.fileData != null) {
            // Check if there is a file path set
            if (myScript.fileData.filePath != null && myScript.fileData.filePath != "") {
                // Load the metadata from the path
                LoadMetadata(myScript.fileData.filePath);
            }
        }
    }

    // When InspectorGUI is drawn
    public override void OnInspectorGUI() {
        //DrawDefaultInspector();
        // Make a GUI Button with text Load Image, and define what happens if clicked     
        if (GUILayout.Button("Load Image", GUILayout.ExpandWidth(false))) {
            // Open a file panel for all supported file types
            string imagePath = EditorUtility.OpenFilePanel("Select Image File", "", "JPEG,jpeg,JPG,jpg,PNG,png,WebP,WEBP,webp,GIF,gif,ICO,ico,BMP,bmp,TIFF,tiff,PSD,psd,PCX,pcx,RAW,raw,CRW,crw,CR2,cr2,NEF,nef,ORF,orf,RAF,raf,RW2,rw2,RWL,rwl,SRW,srw,ARW,arw,DNG,dng,X3F,x3f");
            // If there is a path
            if (imagePath.Length != 0) {
                // Load the metadata
                LoadMetadata(imagePath);
            }
        }
        // For each of the metadata keys
        foreach (string key in myScript.metadata.Keys) {
            // Create a label with the key's name
            GUILayout.Label(key, EditorStyles.boldLabel);
            // Create a text field with the key's value
            GUILayout.TextField(myScript.metadata[key], GUILayout.MaxWidth(Screen.width));
        }
        // Show the image in the inspector
        GUILayout.Label(myScript.image, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(512), GUILayout.MaxHeight(256));
    }

    // Load the Metadata
    private void LoadMetadata(string imagePath) {
        // Tell my MetadataInspector script to load the metadata
        myScript.LoadMetadataFromPath(imagePath);
    }
}
