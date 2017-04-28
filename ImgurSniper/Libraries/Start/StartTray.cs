﻿using ImgurSniper.Libraries.Helper;
using ImgurSniper.Libraries.Hotkeys;
using ImgurSniper.Properties;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ImgurSniper.Libraries.Start {
    public static class StartTray {
        private static HotKey imgHotKey = null, gifHotKey = null;

        public static async Task Initialize() {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Key imgKey = ConfigHelper.ShortcutImgKey;
            Key gifKey = ConfigHelper.ShortcutGifKey;
            bool usePrint = ConfigHelper.UsePrint;

            try {
                imgHotKey = usePrint
                    ? new HotKey(ModifierKeys.None, Key.PrintScreen)
                    : new HotKey(ModifierKeys.Control | ModifierKeys.Shift, imgKey);
                imgHotKey.HotKeyPressed += OpenFromShortcutImg;
            } catch {
                //ignored
            }
            try {
                gifHotKey = new HotKey(ModifierKeys.Control | ModifierKeys.Shift, gifKey);
                gifHotKey.HotKeyPressed += OpenFromShortcutGif;
            } catch {
                //ignored
            }

            ContextMenuStrip contextMenu = new ContextMenuStrip();

            //Icons
            Image iconGif = null, iconHelp = null, iconSettings = null, iconExit = null;

            try {
                iconGif = ImageHelper.LoadImage(Path.Combine(ConfigHelper.InstallDir, "Resources", "iconGif.png"));
                iconHelp = ImageHelper.LoadImage(Path.Combine(ConfigHelper.InstallDir, "Resources", "iconHelp.png"));
                iconSettings = ImageHelper.LoadImage(Path.Combine(ConfigHelper.InstallDir, "Resources", "iconSettings.png"));
                iconExit = ImageHelper.LoadImage(Path.Combine(ConfigHelper.InstallDir, "Resources", "iconExit.png"));
            } catch {
                //Images not found
            }

            //Item: GIF
            ToolStripItem gifMenuItem = contextMenu.Items.Add(strings.gif);
            gifMenuItem.Image = iconGif;
            gifMenuItem.Click += delegate { OpenFromShortcutGif(null); };
            gifMenuItem.Font = new Font(gifMenuItem.Font, gifMenuItem.Font.Style | System.Drawing.FontStyle.Bold);

            //Item: -
            contextMenu.Items.Add(new ToolStripSeparator());

            //Item: Help
            ToolStripItem helpMenuItem = contextMenu.Items.Add(strings.help);
            helpMenuItem.Image = iconHelp;
            helpMenuItem.Click += delegate {
                try {
                    Process.Start(Path.Combine(ConfigHelper.InstallDir, "ImgurSniper.UI.exe"), "Help");
                } catch {
                    // ignored
                }
            };

            //Item: Settings
            ToolStripItem settingsMenuItem = contextMenu.Items.Add(strings.settings);
            settingsMenuItem.Image = iconSettings;
            settingsMenuItem.Click += delegate {
                try {
                    Process.Start(Path.Combine(ConfigHelper.InstallDir, "ImgurSniper.UI.exe"));
                } catch {
                    // ignored
                }
            };

            //Item: Exit
            ToolStripItem exitMenuItem = contextMenu.Items.Add(strings.exit);
            exitMenuItem.Image = iconExit;
            exitMenuItem.Click += delegate { tcs.SetResult(true); };

            //NotifyIcon
            NotifyIcon nicon = new NotifyIcon {
                Icon = Resources.Logo,
                ContextMenuStrip = contextMenu,
                Visible = true,
                Text = strings.clickorpress +
                       (usePrint ? strings.printKeyShortcut : string.Format(strings.ctrlShiftShortcut, imgKey)) +
                       strings.toSnipeNew
            };
            nicon.MouseClick += (sender, e) => {
                if (e.Button == MouseButtons.Left) {
                    OpenFromShortcutImg();
                }
            };

            System.Windows.Application.Current.Exit += delegate {
                nicon.Icon = null;
                nicon.Visible = false;
                nicon.Dispose();
                nicon = null;

                iconGif?.Dispose();
                iconHelp?.Dispose();
                iconSettings?.Dispose();
                iconExit?.Dispose();
            };

            await tcs.Task;
        }

        private static void OpenFromShortcutGif(HotKey obj = null) {
            using (GifWindow window = new GifWindow()) {
                window.ShowDialog();
            }
        }

        private static void OpenFromShortcutImg(HotKey obj = null) {
            using (ScreenshotWindow window = new ScreenshotWindow()) {
                window.ShowDialog();
            }
        }
    }
}