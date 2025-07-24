using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

#if WINDOWS
// WinForms exists only on Windows; wrapping the alias keeps the file single‑platform‑safe.
using WinForms = System.Windows.Forms;
#endif

namespace UICustomizer.Common.Systems.Hooks.MainMenu;

/// <summary>
/// Lets the user choose an image file and hands the texture off to <see cref="LogoHook"/>.
/// </summary>
public static class LogoFileHelper
{
    /// <summary>Run when the “Choose File” button is clicked.</summary>
    public static void UploadFile()
    {
        if (Main.dedServ)                // no dialogs on headless servers
            return;

        string path = ShowOpenFileDialog();
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return;

        try
        {
            // load image into GPU memory
            using var fs = File.OpenRead(path);
            Texture2D tex = Texture2D.FromStream(Main.graphics.GraphicsDevice, fs);
            tex.Name = Path.GetFileName(path);

            // dispose previous custom logo (avoid VRAM leak)
            LogoHook.CustomLogoTexture?.Dispose();
            LogoHook.CustomLogoTexture = tex;

            Main.NewText($"Loaded custom logo: {tex.Name}", Microsoft.Xna.Framework.Color.LimeGreen);
        }
        catch (Exception ex)
        {
            Main.NewText($"Failed to load image – {ex.Message}", Microsoft.Xna.Framework.Color.Red);
        }
    }

    /// <returns>Full path of the chosen file, or <c>null</c> if cancelled / unsupported.</returns>
    private static string ShowOpenFileDialog()
    {
#if WINDOWS
        using var dlg = new WinForms.OpenFileDialog
        {
            Title  = "Select a logo image",
            Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*"
        };
        return dlg.ShowDialog() == WinForms.DialogResult.OK ? dlg.FileName : null;
#else
        Main.NewText("File chooser is only implemented on Windows for now.", Microsoft.Xna.Framework.Color.Orange);
        return null;
#endif
    }
}
