using System;
using DiscordRPC;
using TiltedLauncher;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace TiltedLauncher
{
    public class RPC
    {
        public static DiscordRpcClient client;
        public static Timestamps rpctimestamp { get; set; }
        private static RichPresence presence;
        public static void InitializeRPC()
        {
            client = new DiscordRpcClient(""); //your discord bot application id
            client.Initialize();
            Button[] buttons = { new Button() { Label = "Join Blanks Discord", Url = "https://discord.gg/" } };

            presence = new RichPresence()
            {
                Details = "In Launcher",
                State = "Project Name",
                Timestamps = rpctimestamp,
                Buttons = buttons,

                Assets = new Assets()
                {
                    LargeImageKey = "",
                    LargeImageText = "Project Name",
                    SmallImageKey = "",
                    SmallImageText = "",

                }
            };
            SetState("Project Name");
        }
        public static void SetState(string state, bool watching = false)
        {
            if (watching)
                state = "Project Name" + state;

            presence.State = state;
            client.SetPresence(presence);
        }
    }
}