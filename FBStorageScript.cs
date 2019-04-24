using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Storage;
using System;

using System.Net;

public class FBStorageScript : MonoBehaviour
{
    Firebase.Storage.StorageReference storage_ref;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        // Create a storage reference from our storage service
        storage_ref = storage.GetReferenceFromUrl("URL");
        
        uploadFile();
        downloadFile();
        //deleteFile();
    }

    private void uploadFile()
    {
        byte[] file_bytes = GetImage();

        var new_metadata = new Firebase.Storage.MetadataChange();
        new_metadata.ContentType = "image/jpg";

        // Upload the file to the path "myPic.jpg" by PutBytesAsync() method
        storage_ref.Child("myPic.jpg").PutBytesAsync(file_bytes, new_metadata).ContinueWith((Task<StorageMetadata> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                // print error messages
                Debug.Log("uploading file doesn't complete: " + task.Exception.ToString());
            }
            else
            {
                Debug.Log("uploading file Done");

                // Fetch the download URL
                storage_ref.Child("myPic.jpg").GetDownloadUrlAsync().ContinueWith((Task<Uri> task2) => {
                    if (!task2.IsFaulted && !task2.IsCanceled)
                    {
                        Debug.Log("Download URL: " + task2.Result);
                    }
                    else
                    {
                        Debug.Log("file not found!!");
                    }
                });
            }
        });

        // Upload the file to the path "myPic.jpg" by PutFileAsync() method
        /*storage_ref.Child("myPic.jpg").PutFileAsync("FilePath/FileName.xxx").ContinueWith((Task<StorageMetadata> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                // print error messages
                Debug.Log("uploading file doesn't complete: " + task.Exception.ToString());
            }
            else
            {
                Debug.Log("uploading file Done");
            }
        });*/
    }

    private void downloadFile()
    {
        
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 1 * 1024 * 1024;
        storage_ref.Child("myPic.jpg").GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] file_bytes = task.Result;
                Debug.Log("Finished downloading!");
            }
        });

        // Download to the local filesystem
        /*storage_ref.Child("myPic.jpg").GetFileAsync("FilePath/FileName.jpg").ContinueWith(task => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("File downloaded.");
            }
        });*/
    }
    private void deleteFile()
    {
        // Delete the file
        storage_ref.Child("myPic.jpg").DeleteAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("File deleted successfully.");
            }
        });
    }

    //------DOWNLOAD IMAGE------//
    private byte[] GetImage()
    {
        using (WebClient client = new WebClient())
        {
            byte[] pic = client.DownloadData("http://sfwallpaper.com/images/cute-image-22.jpg");
            return pic;
        }
    }
}
