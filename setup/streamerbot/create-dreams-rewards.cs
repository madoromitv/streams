// MadoromiTV — create all "Dreams" channel point rewards on Twitch.
// Paste into a Streamer.bot "Execute C# Code" sub-action, click Compile, then run the action ONCE.
// Rewards created this way belong to Streamer.bot, so it can edit, pause and enable them later.
// Launch rewards are created enabled; the rest are created disabled, ready to switch on.
// Needs Twitch Affiliate (channel points) and the broadcaster account connected in Streamer.bot.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

public class CPHInline
{
    // title, cost, prompt, needs text, enabled at launch, colour, cooldown (s), max per stream, max per user per stream, skip queue
    private static readonly object[][] Rewards = new object[][]
    {
        // Thematic (magenta)
        new object[]{"Drift in",            100,   "Say hello as you drift in.",                                   false, true,  "#BE0078",    0, 0, 1, true },
        new object[]{"Sweet dreams",        150,   "Leaving? Get a goodnight send-off.",                           false, false, "#BE0078",    0, 0, 0, false},
        new object[]{"Pillow fort",         300,   "Get tucked in for the rest of the stream.",                    false, false, "#BE0078",    0, 0, 1, false},
        new object[]{"Lucid moment",        500,   "Ask one question and get an honest answer.",                   true,  true,  "#BE0078",  300, 0, 0, false},
        new object[]{"Dream journal",       750,   "Add a prompt to the dream log, read out at the end.",          true,  false, "#BE0078",    0, 0, 1, false},
        // Entertainment (violet)
        new object[]{"Hydrate",             200,   "Make me take a sip.",                                          false, true,  "#8205B3",  600, 0, 0, false},
        new object[]{"Posture check",       200,   "Make me sit up properly.",                                     false, false, "#8205B3",  600, 0, 0, false},
        new object[]{"Blink",               500,   "Make the stream blink.",                                       false, false, "#8205B3",  300, 0, 0, true },
        new object[]{"Static",             1000,   "Hit the stream with a burst of static.",                       false, false, "#8205B3",  600, 0, 0, true },
        new object[]{"Narrate it",         1000,   "I narrate the next 2 minutes like an anime narrator.",         false, true,  "#8205B3",  900, 0, 0, false},
        new object[]{"In character",       1500,   "Name a character. I speak as them for 5 minutes.",             true,  false, "#8205B3", 1200, 0, 0, false},
        new object[]{"JRPG theme hum",     2000,   "Name a battle theme. I hum or sing it.",                       true,  false, "#8205B3", 1800, 0, 0, false},
        new object[]{"Sleep paralysis",    2500,   "No moving (or turning the camera) for 30 seconds.",            false, false, "#8205B3", 1200, 0, 0, false},
        new object[]{"Nightmare mode",     3000,   "Next fight: no items, no healing, or basic attacks only.",     false, true,  "#8205B3", 1800, 0, 0, false},
        // Rare (cyan)
        new object[]{"Choose the dialogue", 7500,  "Chat picks the next major dialogue choice.",                   false, false, "#36D6E7",    0, 1, 0, false},
        new object[]{"Name a party member",10000,  "Your name goes on a character or save file.",                 true,  false, "#36D6E7",    0, 1, 0, false},
        new object[]{"Wake up",            15000,  "End the chill: full chaos for one boss.",                      false, false, "#36D6E7",    0, 1, 0, false},
        new object[]{"Next dream",         25000,  "Pick, or start a vote on, the next game from the shortlist.",  true,  true,  "#36D6E7",    0, 1, 0, false},
    };

    public bool Execute()
    {
        string token = CPH.TwitchOAuthToken;
        string clientId = CPH.TwitchClientId;
        string broadcasterId = CPH.TwitchGetBroadcaster().UserId;

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        string url = "https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id=" + broadcasterId;
        int ok = 0, failed = 0;
        foreach (var r in Rewards)
        {
            int cooldown = (int)r[6], maxStream = (int)r[7], maxUser = (int)r[8];
            string json = "{"
                + "\"title\":" + Q((string)r[0]) + ","
                + "\"cost\":" + (int)r[1] + ","
                + "\"prompt\":" + Q((string)r[2]) + ","
                + "\"is_user_input_required\":" + B((bool)r[3]) + ","
                + "\"is_enabled\":" + B((bool)r[4]) + ","
                + "\"background_color\":" + Q((string)r[5]) + ","
                + "\"is_global_cooldown_enabled\":" + B(cooldown > 0) + ","
                + "\"global_cooldown_seconds\":" + Math.Max(cooldown, 1) + ","
                + "\"is_max_per_stream_enabled\":" + B(maxStream > 0) + ","
                + "\"max_per_stream\":" + Math.Max(maxStream, 1) + ","
                + "\"is_max_per_user_per_stream_enabled\":" + B(maxUser > 0) + ","
                + "\"max_per_user_per_stream\":" + Math.Max(maxUser, 1) + ","
                + "\"should_redemptions_skip_request_queue\":" + B((bool)r[9])
                + "}";
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers.Add("Client-Id", clientId);
                    wc.Headers.Add("Authorization", "Bearer " + token);
                    wc.Headers.Add("Content-Type", "application/json");
                    wc.UploadString(url, "POST", json);
                }
                ok++; CPH.LogInfo("Dreams reward created: " + r[0]);
            }
            catch (WebException ex)
            {
                failed++;
                string body = "";
                if (ex.Response != null)
                    using (var sr = new StreamReader(ex.Response.GetResponseStream())) body = sr.ReadToEnd();
                CPH.LogWarn("Dreams reward FAILED: " + r[0] + " -> " + ex.Message + " " + body);
            }
        }
        CPH.LogInfo("Dreams rewards done. Created " + ok + ", failed " + failed + ".");
        return true;
    }

    private static string B(bool b) { return b ? "true" : "false"; }
    private static string Q(string s) { return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""; }
}
