using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System.Collections.Generic;

public class ExampleProject : MonoBehaviour {

    // Image on GUI
    public Image imageLoaded;

    // Input fields for image path and URL
    public InputField inputFieldDisk;
    public InputField inputFieldURL;

    // Error Text on GUI
    public Text textError;

    // Text fields to show some metadata on GUI
    public Text textDateTime;
    public Text textMake;
    public Text textCompression;
    public Text textSoftware;

    // The MetadataInspector script
    public MetadataInspector myMetadataInspector;

    // Sprite for UI
    private Sprite mySprite;

    // Load image from disk
    public void LoadFromDisk() {
        // If there is no text in the text box
        if (inputFieldDisk.text == "" || inputFieldDisk.text == null) {
            // Show an error message
            textError.text = "Invalid file path!";
            // Stop any coroutines that are running 
            StopAllCoroutines();
            // Hide the error message after delay
            StartCoroutine(HideErrorMessage(3f));
        }
        else {
            // Get the path from the GUI box
            myMetadataInspector.LoadMetadataFromPath(inputFieldDisk.text);
            // For each of the metadata keys
            foreach (string key in myMetadataInspector.metadata.Keys) {
                // Log the Dictionary Keys and Values
                Debug.Log(key + ": " + myMetadataInspector.metadata[key]);
            }
            // Update GUI elements
            UpdateGUI();
        }
    }

    // Load image from URL
    public void LoadFromURL() {
        // If there is no text in the text box
        if (inputFieldURL.text == "" || inputFieldURL.text == null) {
            // Show an error message
            textError.text = "Invalid URL!";
            // Stop any coroutines that are running 
            StopAllCoroutines();
            // Hide the error message after delay
            StartCoroutine(HideErrorMessage(3f));
        }
        else {

            // Get the URL from the GUI box
            myMetadataInspector.LoadMetadataFromURL(inputFieldURL.text);

            // For each of the metadata keys
            foreach (string key in myMetadataInspector.metadata.Keys) {
                // Log the Dictionary Keys and Values
                Debug.Log(key + ": " + myMetadataInspector.metadata[key]);
            }

            // Update GUI elements
            UpdateGUI();
        }
    }

    // Update the GUI elements
    private void UpdateGUI() {
        textDateTime.text = "";
        textMake.text = "";
        textCompression.text = "";
        textSoftware.text = "";
        GetDateMethodA();
        GetMakeMethodB();
        GetCompressionMethodB();
        GetSoftwareMethodB();
        // Create a new sprite to display the image in the GUI
        Rect newRect = new Rect(0f, 0f, myMetadataInspector.image.width, myMetadataInspector.image.height);
        Vector2 newPivot = new Vector2(0f,0f);
        Sprite newSprite = Sprite.Create(myMetadataInspector.image, newRect, newPivot );
        imageLoaded.sprite = newSprite;
    }

    IEnumerator HideErrorMessage(float delay) {
        // Wait for the delay amount
        yield return new WaitForSeconds(delay);
        // Hide the error message
        textError.text = "";
    }

    // Try to find a date value
    private void GetDateMethodA() {
        // For each key in the metadata dictionary
        foreach (string key in myMetadataInspector.metadata.Keys) {
            // if key has "date or Date"
            if (key.Contains("date") || key.Contains("Date")) {
                // Update the GUI text
                textDateTime.text = myMetadataInspector.metadata[key];
            }
        }
    }

    // Try to find a Make value
    private void GetMakeMethodB() {
        // For each of the directories
        foreach (Directory directory in myMetadataInspector.directories) {
            // Try to get a date value from the Exif Directory Base
            string value = directory.GetDescription(ExifDirectoryBase.TagMake);
            // If a value was found
            if (value != null) {
                // Update the GUI text
                textMake.text = value;
            }
        }
    }

    // Try to find a Compression value
    private void GetCompressionMethodB() {
        // For each of the directories
        foreach (Directory directory in myMetadataInspector.directories) {
            // Try to get a date value from the Exif Directory Base
            string value = directory.GetDescription(ExifDirectoryBase.TagCompression);
            // If a value was found
            if (value != null) {
                // Update the GUI text
                textCompression.text = value;
            }
        }
    }

    // Try to find a Compression value
    private void GetSoftwareMethodB() {
        // For each of the directories
        foreach (Directory directory in myMetadataInspector.directories) {
            // Try to get a date value from the Exif Directory Base
            string value = directory.GetDescription(ExifDirectoryBase.TagSoftware);
            // If a value was found
            if (value != null) {
                // Update the GUI text
                textSoftware.text = value;
            }
        }
    }
}
