﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace LambentLight
{
    /// <summary>
    /// Class that contains the information of a specific FiveM build.
    /// </summary>
    public class Build
    {
        /// <summary>
        /// The Uri or URL for downloading the builds from the FiveM servers.
        /// </summary>
        public const string DownloadUri = "https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/{0}/server.zip";
        /// <summary>
        /// The ID of the build. This can be either the folder name or Upstream identifier.
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// If the build is available locally.
        /// </summary>
        public bool IsAvailable => Directory.Exists(Folder);
        /// <summary>
        /// The local folder where the build can be located.
        /// </summary>
        public string Folder => Path.Combine("Builds", ID);

        /// <summary>
        /// Creates a Build to use with LambentLight
        /// </summary>
        /// <param name="identifier">The folder name or Upstream identifier.</param>
        public Build(string identifier)
        {
            // Set our identifier
            ID = identifier;
        }

        /// <summary>
        /// Gets the string representation of a build.
        /// </summary>
        /// <returns>The ID of the build.</returns>
        public override string ToString()
        {
            return ID;
        }
    }

    /// <summary>
    /// A JSON converter for FiveM builds.
    /// </summary>
    public class BuildConverter : JsonConverter<Build>
    {
        public override Build ReadJson(JsonReader reader, Type objectType, Build existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Return the value from the known color
            return new Build((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, Build value, JsonSerializer serializer)
        {
            // We are only going to read, so disable the writing by raising an exception
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Class that manages the downloads and updates of builds.
    /// </summary>
    public static class BuildManager
    {
        /// <summary>
        /// The web client for REST calls.
        /// </summary>
        private static WebClient Client = new WebClient();
        /// <summary>
        /// The updated list of builds.
        /// </summary>
        public static List<Build> Builds = new List<Build>();

        /// <summary>
        /// Refreshes the list of builds.
        /// </summary>
        public static void Refresh()
        {
            // Let's store the fetched builds here
            string RawBuilds = "";

            // Try to request the list of builds
            try
            {
                RawBuilds = Client.DownloadString(Properties.Settings.Default.Builds);
            }
            // If we have failed (4XX-5XX codes)
            catch (WebException e)
            {
                // Set the build list to empty
                Builds = new List<Build>();
                // Notify the user
                MessageBox.Show($"Unable to fetch the new FiveM builds: Code {(int)e.Status} ({e.Status})", "Failed to update Builds");
            }

            // Create a temporary list of builds
            Builds = JsonConvert.DeserializeObject<List<Build>>(RawBuilds, new BuildConverter());
        }

        /// <summary>
        /// Fills the specified ComboBox with the list of builds.
        /// </summary>
        /// <param name="box">The ComboBox to fill.</param>
        public static void Fill(ComboBox box)
        {
            // Start by updating the list of builds
            Refresh();

            // Remove all of the items
            box.Items.Clear();

            // Iterate over the items
            foreach (Build StoredBuild in Builds)
            {
                // And add the build
                box.Items.Add(StoredBuild);
            }
        }
    }
}