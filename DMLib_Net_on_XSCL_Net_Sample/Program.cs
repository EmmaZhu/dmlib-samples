using System;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Storage.DataMovement;

namespace DMLib_Net_on_XSCL_Net_Sample
{
    class Program
    {
        // This sample is to show how to create a container with 'BlobContainerClient' in XSCL, and upload files to the container with DMLib.Net

        // Container creations requires higher permissions, 
        // DMLib chooses not to create container in it to avoid holding a credential with too high permission for a long running operation.

        // DMLib defines 'CloudBlobHierarchicalItem' with container identity and a prefix to represent target place in blob service 
        // that cutomer may want to upload a local directory to.
        static void Main(string[] args)
        {
            string accountName = "myaccount";
            string accountKey = "myaccountkey==";

            BlobContainerClient blobContainerClient = new BlobContainerClient(new Uri($"https://{accountName}.blob.core.windows.net/containername"),
                new StorageSharedKeyCredential(accountName, accountKey));

            // Create the container 
            blobContainerClient.CreateIfNotExists();

            CloudBlobHierarchicalItem blobVirtualDirectory = new CloudBlobHierarchicalItem(new Uri($"https://{accountName}.blob.core.windows.net/containername"),
                "Prefix",
                new StorageSharedKeyCredential(accountName, accountKey));

            UploadDirectoryOptions options = new UploadDirectoryOptions();
            options.Recursive = true; // Indicate whether to recursively upload all items under the directory and its subdirectories.

            DirectoryTransferContext context = new DirectoryTransferContext();

            // Add a progress handler to monitor DMLib transfer progress
            context.ProgressHandler = new Progress<TransferStatus>((status) =>
            {
                Console.WriteLine(string.Format("{0} - {1} - {2} - {3}",
                    status.NumberOfFilesTransferred,
                    status.NumberOfFilesFailed,
                    status.NumberOfFilesSkipped,
                    status.BytesTransferred));
            });


            //Upload a local directory to a blob virtual directory
            TransferManager.UploadDirectoryAsync("LocalFolderPath", 
                blobVirtualDirectory, 
                options, 
                context).GetAwaiter().GetResult();
        }
    }
}
