using AutoCEMI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class GameProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private GameProfile profile = new();

        [ObservableProperty]
        private string? selectedMod;

        public GameProfileViewModel()
        {
            MoveModUpCommand = new RelayCommand(MoveModUp);
            MoveModDownCommand = new RelayCommand(MoveModDown);
        }

        public IRelayCommand MoveModUpCommand { get; }
        public IRelayCommand MoveModDownCommand { get; }

        [RelayCommand]
        private void AddMod(string modName = "New mod")
        {
            int idx = Profile.Mods.LoadOrder.IndexOf(modName);

            if (idx >= 0)
            {
                return;
            }

            Profile.Mods.LoadOrder.Add(modName);
        }

        [RelayCommand]
        private void RemoveMod(string modName)
        {
            Profile.Mods.LoadOrder.Remove(modName);
        }

        private void MoveModUp()
        {
            int idx = Profile.Mods.LoadOrder.IndexOf(SelectedMod ?? string.Empty);
            if (idx > 0)
            {
                MoveItem(idx, idx - 1);
            }
        }

        private void MoveModDown()
        {
            int idx = Profile.Mods.LoadOrder.IndexOf(SelectedMod ?? string.Empty);
            if (idx >= 0 && idx < Profile.Mods.LoadOrder.Count - 1)
            {
                MoveItem(idx, idx + 1);
            }
        }

        private void MoveItem(int oldIndex, int newIndex)
        {
            var item = Profile.Mods.LoadOrder[oldIndex];
            Profile.Mods.LoadOrder.RemoveAt(oldIndex);
            Profile.Mods.LoadOrder.Insert(newIndex, item);
        }
    }
}
