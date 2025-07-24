using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities.FileBrowser;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.Helpers;

/// <summary>
/// Lets the user choose an image file and hands the texture off to <see cref="LogoHook"/>.
/// </summary>
public static class LogoFileHelper
{
    /// <summary>Run when the “Choose File” button is clicked.</summary>
    public static string UploadFile()
    {
        if (Main.dedServ)
            return null;

        string path = ShowOpenFileDialog();
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;

        try
        {
            using var fs = File.OpenRead(path);
            Texture2D tex = Texture2D.FromStream(Main.graphics.GraphicsDevice, fs);
            tex.Name = Path.GetFileName(path);

            LogoHook.CustomLogoTexture?.Dispose();
            LogoHook.CustomLogoTexture = tex;

            Main.NewText($"Loaded custom logo: {tex.Name}", Color.LimeGreen);
            return tex.Name;
        }
        catch (Exception ex)
        {
            Main.NewText($"Failed to load image – {ex.Message}", Color.Red);
            return null;
        }
    }

    /// <returns>Full path of the chosen file, or <c>null</c> if cancelled / unsupported.</returns>
    private static string ShowOpenFileDialog()
    {
        var extensions = new ExtensionFilter[]
        {
        new() {
            Name = "Images",
            Extensions = ["png", "jpg", "jpeg"]
        }
        };

        // NFD returns null on cancel – just bubble that straight out.
        return OpenFilePanel("Open icon", extensions);
    }

    public static string OpenFilePanel(string title, ExtensionFilter[] extensions, string path = null)
    {
        string[] value = extensions.SelectMany((ExtensionFilter x) => x.Extensions).ToArray();
        string result = default(string);
        if ((int)nativefiledialog.NFD_OpenDialog(string.Join(",", value), path, out result) == 1)
        {
            return result;
        }
        return null;
    }
}
