// MadoromiTV — "Going Live · Chat"
// Posts a welcome message in chat when a stream starts.
//   Twitch  › Channel   › Stream Online      → message in Twitch chat
//   YouTube › Broadcast › Broadcast Started  → message in YouTube chat
// Paste into an Execute C# Code sub-action, then Compile.
// Each platform posts at most once every 2 hours, so a dropped connection
// that restarts the stream doesn't post again.

using System;

public class CPHInline
{
    // ---- edit the wording here ----
    const string TWITCH_MSG  = "🌙 We're live! Drifting into {game} tonight. Grab a pillow, say hi, and spend your Dreams on the rewards ✨";
    const string YOUTUBE_MSG = "🌙 We're live! Drifting into {game}. Tap like to fill the dream bar, say hi, and subscribe to help us reach 500 ✨";
    const string FALLBACK_GAME = "a new dream";
    const int    COOLDOWN_MIN  = 120;
    const int    DELAY_MS      = 15000;   // give chat a moment to connect

    static string Nice(string game)
    {
        if (string.IsNullOrWhiteSpace(game)) return FALLBACK_GAME;
        game = game.Trim();
        if (game.Equals("Just Chatting", StringComparison.OrdinalIgnoreCase)) return "a cosy chat";
        return game;
    }

    public bool Execute()
    {
        EventSource source = CPH.GetSource();
        string key = source == EventSource.YouTube ? "mdrGoLiveYT" : "mdrGoLiveTwitch";

        // once per stream
        string last = CPH.GetGlobalVar<string>(key, true);
        DateTime lastTime;
        if (!string.IsNullOrEmpty(last) && DateTime.TryParse(last, out lastTime)
            && (DateTime.UtcNow - lastTime).TotalMinutes < COOLDOWN_MIN)
            return true;
        CPH.SetGlobalVar(key, DateTime.UtcNow.ToString("o"), true);

        CPH.Wait(DELAY_MS);

        if (source == EventSource.YouTube)
        {
            // game = the part of the YouTube title before " · " (title format: {Game} · {what's happening} | 微睡み)
            string title;
            CPH.TryGetArg("broadcast.title", out title);
            string game = FALLBACK_GAME;
            if (!string.IsNullOrWhiteSpace(title))
            {
                int dot = title.IndexOf(" · ");
                int bar = title.IndexOf(" | ");
                int cut = dot > 0 ? dot : (bar > 0 ? bar : title.Length);
                game = title.Substring(0, cut).Trim();
            }
            CPH.SendYouTubeMessageToLatestMonitored(YOUTUBE_MSG.Replace("{game}", Nice(game)));
        }
        else
        {
            string game;
            CPH.TryGetArg("game", out game);
            CPH.SendMessage(TWITCH_MSG.Replace("{game}", Nice(game)), true, true);
        }
        return true;
    }
}
