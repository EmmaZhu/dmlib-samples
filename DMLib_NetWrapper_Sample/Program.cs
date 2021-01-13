using System;
using Azure.Storage.DataMovement;

namespace DMLib_NetWrapper_Sample
{
    class Program
    {
        // This sample is to show how to upload files to the container with DMLib.NetWrapper when the target container already exists.

        // In this sample, it doesn't use functionality in XSCL.Net and won't need to introduce dependency on XSCL.Net.

        // DMLib defines 'BlobHierarchicalTransferItem' with container identity and a prefix to represent target place in blob service 
        // that cutomer may want to upload a local directory to.
        static void Main(string[] args)
        {
            string accountName = "myaccount";
            string accountKey = "myaccountkey==";
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
