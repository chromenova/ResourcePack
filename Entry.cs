using Spectrum.API.Configuration;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.Experimental;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace TexturePack
{
    public class Entry : IPlugin
    {
        static bool debug = true;

        Spectrum.API.Logging.Logger Log;
        private Settings _settings;
        List<AssetBundle> texturePacks;

        public void Initialize(IManager manager, string ipcIdentifier)
        {
            Log = new Spectrum.API.Logging.Logger("TexturePack.log") { WriteToConsole = true };
            _settings = new Settings("TexturePackSettings");
            texturePacks = new List<AssetBundle>();

            string[] texturePackPaths = _settings.GetItem<string[]>("loadedTexturePacks");



            foreach (string texturePackPathName in texturePackPaths)
            {
                LogInfo("Loaded texture pack" + texturePackPathName);
                texturePacks.Add((new Assets(texturePackPathName)).Bundle);
            }

            Events.Level.PostLoad.Subscribe(data =>
            {
                foreach (AssetBundle texturePack in texturePacks)
                {
                    //texturePack.Unload(true);
                    string[] assetNames = texturePack.GetAllAssetNames();
                    LogInfo("Assets loaded!");
                    LogInfo(string.Join(", ", texturePack.GetAllAssetNames()));
                    LogInfo("Material Names:");
                    var allObjects = GameObject.FindObjectsOfType<MeshRenderer>();
                    foreach (MeshRenderer obj in allObjects)
                    {
                        foreach (Material mat in obj.materials)
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                Texture oldTex = mat.GetTexture(i);
                                if (oldTex != null)
                                {
                                    LogInfo(oldTex.name);
                                    foreach (string assetName in assetNames)
                                    {
                                        if (assetName.ToLower().Contains(oldTex.name.ToLower()))
                                        {
                                            LogInfo("Replacement texture found:");
                                            LogInfo(assetName);
                                            mat.SetTexture(i, texturePack.LoadAsset<Texture>(assetName));
                                            if(mat.GetTexture(i) == texturePack.LoadAsset<Texture>(assetName))
                                                LogInfo("Sucessfully replaced!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        void LogInfo(string message)
        {
            LogInfo(message, false);
        }
        void LogInfo(string message, bool sameLine)
        {
            if (debug)
                Log.Info(message, sameLine);
        }
    }
}