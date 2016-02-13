using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace Rester.Service
{
    internal interface IFilePicker
    {
        Task CreateTargetFileAsync(Func<StorageFile, Task> processFile);
        Task<StorageFile> PickSingleFileForImportAsync();
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class FilePicker : IFilePicker
    {
        private readonly IDialog _dialog;

        public FilePicker(IDialog dialog)
        {
            _dialog = dialog;
        }

        public async Task CreateTargetFileAsync(Func<StorageFile, Task> processFile)
        {
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices["Rester Database"] = new[] { ".rdb" };
            StorageFile storageFile = await savePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                CachedFileManager.DeferUpdates(storageFile);
                await processFile(storageFile);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(storageFile);
                if (status != FileUpdateStatus.Complete)
                {
                    string message = "File " + storageFile.Name + " couldn't be saved.";
                    await _dialog.ShowAsync(message, "Export Error");
                }
            }

        }

        public async Task<StorageFile> PickSingleFileForImportAsync()
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            //picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".rdb");
            return await picker.PickSingleFileAsync();
        }
    }
}