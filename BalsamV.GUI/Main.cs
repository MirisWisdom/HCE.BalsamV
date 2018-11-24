using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BalsamV.GUI.Properties;

namespace BalsamV.GUI
{
    /// <summary>
    ///     Main model for the HCE.BalsamV program.
    /// </summary>
    public sealed class Main : INotifyPropertyChanged
    {
        /// <summary>
        ///     Atarashii Profile Configuration for the selected blam.sav.
        /// </summary>
        private Blam _blam;

        /// <summary>
        ///     Selected blam.sav exists.
        /// </summary>
        private bool _canEdit;

        /// <summary>
        ///     Selected Blam.sav absolute path.
        /// </summary>
        private string _path;

        /// <summary>
        ///     <see cref="_path" />
        /// </summary>
        public string Path
        {
            get => _path;
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged();
                OnPathChanged();
            }
        }

        /// <summary>
        ///     <see cref="_blam" />
        /// </summary>
        public Blam Blam
        {
            get => _blam;
            set
            {
                if (Equals(value, _blam)) return;
                _blam = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     <see cref="_canEdit" />
        /// </summary>
        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                if (value == _canEdit) return;
                _canEdit = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Attempts to auto-detect & load a blam.sav on the file system.
        ///     Initialisation is done with code directly taken from <see cref="BlamFactory.GetFromSystem" />,
        ///     for the purpose of exposing the deduced absolute path of the blam.sav binary.
        /// </summary>
        public void Initialise()
        {
            try
            {
                var name = LastprofFactory.DetectOnSystem().Name;
                Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "My Games", "Halo CE", "savegames", name, "blam.sav");

                Blam = BlamFactory.GetFromBinary(Path);
                CanEdit = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Could not detect blam.sav. Please load manually!");
                CanEdit = false;
            }
        }

        /// <summary>
        ///     Invoke patching of the blam.sav with the current Blam state.
        /// </summary>
        public void Save()
        {
            try
            {
                new BlamPatcher(Blam).PatchToBinary(Path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        ///     Invoke parsing of the selected blam.sav upon path change.
        /// </summary>
        private void OnPathChanged()
        {
            try
            {
                Blam = BlamFactory.GetFromBinary(Path);
                CanEdit = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                CanEdit = false;
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}