﻿namespace HalftoneFx.Views
{
    using HalftoneFx.Presenters;

    using System.Drawing;

    public interface IWorkspaceView : IView<WorkspacePresenter>
    {
        void SetPicture(Image image);

        void UpdatePicture(Image image);

        void SetProgress(float percent);

        void SetPattern(Image image);

        void UpdatePalettes();

        void Error(string text);
    }
}
