using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities.FileBrowser;

namespace UICustomizer.MainMenu.Helpers;

/// <summary>
/// Lets the user choose an image file and hands the texture off to 
/// <see cref="BackgroundHook"/>. or <see cref="LogoHook"/>.
/// </summary>
public static class FileUploadHelper
{
    /// <summary>Run when the “Choose File” button is clicked.</summary>
    public static Texture2D ReadAndCreateTextureFromPath(string path)
    {
        try
        {
            using var fs = File.OpenRead(path);
            Texture2D tex = Texture2D.FromStream(Main.graphics.GraphicsDevice, fs);
            tex.Name = Path.GetFullPath(path);    // set name

            Log.Info($"Successfully loaded image – {tex.Name}");
            return tex;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load image – {ex.Message}");
            return null;
        }
    }

    /// <returns>Full path of the chosen file, or <c>null</c> if cancelled / unsupported.</returns>
    public static string OpenFileDialog()
    {
        var extensions = new ExtensionFilter
        {
            Name = "Images",
            Extensions = ["png", "jpg", "jpeg"]
        };

        // Concatenate extensions for NFD: "png,jpg,jpeg"
        string extensionStr = string.Join(',', extensions.Extensions);

        // Initial directory – use the user's Pictures folder or leave null for default
        //string startDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        nativefiledialog.nfdresult_t result = nativefiledialog.NFD_OpenDialog(
            extensionStr,              // filter list
            null,                  // initial directory
            out string outPath);

        return result == nativefiledialog.nfdresult_t.NFD_OKAY ? outPath : null;
    }
}
