using AutoCEMI.Models;
using AutoCEMI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace AutoCEMI.GUI.ViewModels
{
    public partial class GameProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private GameProfile profile = new();

        [ObservableProperty]
        private string? profileLocationPath;

        [ObservableProperty]
        private string? selectedMod;

        // Undo/Redo
        private readonly Stack<List<string>> _undoStack = new();
        private readonly Stack<List<string>> _redoStack = new();
        private bool _isUndoingOrRedoing;

        private GameExecute gameExecute = new();
        
        public bool CanUndo()
        {
            if (_undoStack.Count > 0) return true;
            return false;
        }
        public bool CanRedo() {
            if (_redoStack.Count > 0) return true;
            return false;
        }

        public GameProfileViewModel()
        {
            MoveModUpCommand = new RelayCommand(MoveModUp);
            MoveModDownCommand = new RelayCommand(MoveModDown);
        }

        public IRelayCommand MoveModUpCommand { get; }
        public IRelayCommand MoveModDownCommand { get; }

        [RelayCommand]
        private async Task ExecuteProfile()
        {
            await gameExecute.RunAsync(Profile);
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        public void Undo()
        {
            if (_undoStack.Count == 0) return;

            _isUndoingOrRedoing = true;

            // Save current state to redo
            _redoStack.Push(Profile.Mods.LoadOrder.ToList());

            // Restore previous state
            var previous = _undoStack.Pop();
            Profile.Mods.LoadOrder.Clear();
            foreach (var item in previous)
                Profile.Mods.LoadOrder.Add(item);

            _isUndoingOrRedoing = false;
        }

        [RelayCommand(CanExecute = nameof(CanRedo))]
        public void Redo()
        {
            if (_redoStack.Count == 0) return;

            _isUndoingOrRedoing = true;

            // Save current to undo
            _undoStack.Push(Profile.Mods.LoadOrder.ToList());

            // Restore
            var next = _redoStack.Pop();
            Profile.Mods.LoadOrder.Clear();
            foreach (var item in next)
                Profile.Mods.LoadOrder.Add(item);

            _isUndoingOrRedoing = false;
        }

        private void SaveUndoState()
        {
            if (_isUndoingOrRedoing) return;

            _undoStack.Push(Profile.Mods.LoadOrder.ToList());
            _redoStack.Clear(); // Clear redo stack on new action
        }

        [RelayCommand]
        private void AddMod(string modName = "New mod")
        {
            int idx = Profile.Mods.LoadOrder.IndexOf(modName);

            if (idx >= 0) return;

            SaveUndoState();
            Profile.Mods.LoadOrder.Add(modName);
        }

        [RelayCommand]
        private void RemoveMod(string modName)
        {
            SaveUndoState();
            Profile.Mods.LoadOrder.Remove(modName);
        }

        private void MoveModUp()
        {
            int idx = Profile.Mods.LoadOrder.IndexOf(SelectedMod ?? string.Empty);
            if (idx > 0)
            {
                SaveUndoState();
                MoveItem(idx, idx - 1);
            }
        }

        private void MoveModDown()
        {
            int idx = Profile.Mods.LoadOrder.IndexOf(SelectedMod ?? string.Empty);
            if (idx >= 0 && idx < Profile.Mods.LoadOrder.Count - 1)
            {
                SaveUndoState();
                MoveItem(idx, idx + 1);
            }
        }

        private void MoveItem(int oldIndex, int newIndex)
        {
            var item = Profile.Mods.LoadOrder[oldIndex];
            Profile.Mods.LoadOrder.RemoveAt(oldIndex);
            Profile.Mods.LoadOrder.Insert(newIndex, item);
        }

        partial void OnProfileChanged(GameProfile value)
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
