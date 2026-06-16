using AutoCEMI.GUI.ViewModels;
using AutoCEMI.Models;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WinRT.Interop;

namespace AutoCEMI.GUI.Services
{
    internal class ServiceToWriteGameProfiles
    {
        public async Task SaveProfile(GameProfileViewModel vm)
        {
            if (vm.Profile == null)
                return;

            if (string.IsNullOrWhiteSpace(vm.ProfileLocationPath))
            {
                await SaveProfileAsNew(vm);
                return;
            }

            await WriteProfileToFile(vm.ProfileLocationPath, vm.Profile);
        }

        public async Task SaveProfileAsNew(GameProfileViewModel vm)
        {
            if (vm.Profile == null)
                return;

            var hwnd = WindowNative.GetWindowHandle(App.MainWindowInstance);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);

            var picker = new FileSavePicker(windowId)
            {
                SuggestedFileName = vm.Profile.Name
            };

            picker.FileTypeChoices.Add("JSON File", new[] { ".json" });

            var result = await picker.PickSaveFileAsync();
            if (result == null || string.IsNullOrWhiteSpace(result.Path))
                return;

            await WriteProfileToFile(result.Path, vm.Profile);
            vm.ProfileLocationPath = result.Path;
        }

        private async Task WriteProfileToFile(string path, GameProfile profile)
        {
            try
            {
                var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(path, json);
            }
            catch (Exception ex)
            {
                // TODO: log or show dialog
            }
        }
    }
}
