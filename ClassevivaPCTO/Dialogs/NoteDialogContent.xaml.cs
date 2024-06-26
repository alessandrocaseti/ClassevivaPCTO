﻿using ClassevivaPCTO.Utils;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class NoteDialogContent : Page
    {
        private readonly Note CurrentNote;
        private readonly ReadNoteResult CurrentReadResult;

        private readonly IClassevivaAPI _apiWrapper;

        public NoteDialogContent(Note note, ReadNoteResult readNoteResult)
        {
            InitializeComponent();

            CurrentNote = note;
            CurrentReadResult = readNoteResult;

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);

            MaxWidth = 600;
        }
    }
}