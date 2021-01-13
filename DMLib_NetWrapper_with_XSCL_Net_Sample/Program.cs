using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.DataMovement;

namespace DMLib_NetWrapper_with_XSCL_Net_Sample
{
    class Program
    {
        // This sample is to show how to create a container with 'BlobContainerClient' in XSCL, and upload files to the container with DMLib.NetWrapper.

        // Container creations requires higher permissions, 
        // DMLib chooses not to create container in it to avoid holding a credential with too high permission for a long running operation.

        // DMLib defines 'BlobHierarchicalTransferItem' with container identity and a prefix to represent target place in blob service 
        // that cutomer may want to upload a local directory to.
        static void Main(string[] args)
        {
            string accountName = "myaccount";
            string accountKey = "myaccountkey==";

            BlobContainerClient blobContainerClient = new BlobContainerClient(new Uri($"https://{accountName}.blob.core.windows.net/containername"),
                new StorageSharedKeyCredential(accountName, accountKey));

            // Create the container 
            blobContainerClient.CreateIfNotExists();

            BlobHierarchicalTransferItem blobHierarchicalTransferItem = new BlobHierarchicalTransferItem(new Uri($"https://{accountName}.blob.core.windows.net/containername"),
                "Prefix",
                new AzureStorageSharedKeyCredentials(accountName, accountKey));

            HierarchicalUploadOptions options = new HierarchicalUploadOptions();
            options.Recursive = true; // Indicates whether to recursively upload all items under the directory and its subdirectories.

            TransferContext context = new TransferContext();
            // Add a progress handler to monitor DMLib transfer progress
            context.ProgressHandler = new Progress<TransferStatus>((status) =>
            {
                Console.WriteLine(string.Format("{0} - {1} - {2} - {3}",
                    status.NumberOfFilesTransferred,
                    status.NumberOfFilesFailed,
                    status.NumberOfFilesSkipped,
                    status.BytesTransferred));
            });

            TransferJob.CreateTransferJob("LocalFolderPath",
                blobHierarchicalTransferItem,
                options,
                context).RunAsync().GetAwaiter().GetResult();
        }
    }
}
