/* MadoromiTV alert logic
   Reads the same #hash parameters the Ashen alert used, so existing
   Streamer.bot actions only need the new page URL:
     alertType, platform, userName, nickname,
     bits, gifts, tier, recipient, reward, cost, anonymous,   (Twitch)
     giftName, coins, repeatCount, subMonth                    (TikTok)
   New, optional:
     viewers  (TwitchRaid)
     burst    (light | medium | heavy | none) to override the automatic pick
*/

(function () {
  "use strict";

  const ALERT_MS = 10000; // keep in step with the Streamer.bot wait

  // ---------- Copy ----------
  const TITLES = {
    TwitchFollow: "a new dreamer",
    TwitchSub: "sharing the dream",
    TwitchCheer: "a spark in the dark",
    TwitchGiftSub: "a dream, gifted",
    TwitchGiftBomb: "dreams for everyone",
    TwitchRewardRedemption: "a wish granted",
    TwitchRaid: "the dream grows",

    TTFollow: "a new dreamer",
    TTGift: "a gift drifts in",
    TTSub: "sharing the dream",

    YTSubscribe: "a new dreamer",
    YTMember: "sharing the dream",
    YTSuperChat: "a spark in the dark"
  };

  const BURSTS = {
    TwitchFollow: "light",
    TTFollow: "light",
    YTSubscribe: "light",
    TwitchGiftBomb: "heavy",
    TwitchRaid: "heavy"
    // everything else: medium
  };

  const PLATFORMS = {
    twitch:  { text: "via twitch",  icon: "../assets/icons/icon-twitch.png" },
    tiktok:  { text: "via tiktok",  icon: "../assets/icons/icon-tiktok.png" },
    youtube: { text: "via youtube", icon: "../assets/icons/icon-youtube.png" }
  };

  // ---------- Helpers ----------
  const $ = (id) => document.getElementById(id);

  function readParams() {
    const p = new URLSearchParams(window.location.hash.substring(1));
    return (name) => {
      const v = p.get(name);
      if (!v) return "";
      try { return decodeURIComponent(v.replace(/\+/g, " ")).trim(); }
      catch { return v.replace(/\+/g, " ").trim(); }
    };
  }

  function prettify(s) {
    return (s || "alert")
      .replace(/^(Twitch|TT|YT)/, "")
      .replace(/([a-z])([A-Z])/g, "$1 $2")
      .replace(/[_-]+/g, " ")
      .trim()
      .toLowerCase();
  }

  const num = (v) => { const n = parseInt(v, 10); return Number.isFinite(n) ? n : null; };
  const plural = (n, word) => `${n} ${word}${n === 1 ? "" : "s"}`;
  const tierText = (t) => (t ? (/^\d+$/.test(t) ? `tier ${t}` : t.toLowerCase()) : "");
  const join = (...parts) => parts.filter(Boolean).join(" · ");

  function buildSubtext(type, g) {
    const tier = tierText(g("tier"));
    switch (type) {
      case "TwitchCheer": {
        const b = num(g("bits"));
        return b ? plural(b, "bit") : "";
      }
      case "TwitchSub":
        return tier;
      case "TwitchGiftSub":
        return join(g("recipient") ? `to ${g("recipient")}` : "", tier);
      case "TwitchGiftBomb": {
        const n = num(g("gifts"));
        return join(n ? plural(n, "gifted sub") : "", tier);
      }
      case "TwitchRewardRedemption":
        return join(g("reward"), g("cost") ? `${g("cost")} points` : "");
      case "TwitchRaid": {
        const v = num(g("viewers"));
        return v ? `arriving with ${plural(v, "dreamer")}` : "";
      }
      case "TTGift": {
        const name = g("giftName").toLowerCase();
        const qty = num(g("repeatCount"));
        const c = num(g("coins"));
        const total = c !== null ? (qty && qty > 1 ? c * qty : c) : null;
        return join(name + (qty && qty > 1 ? ` ×${qty}` : ""), total !== null ? plural(total, "coin") : "");
      }
      case "TTSub": {
        const m = num(g("subMonth"));
        return m ? plural(m, "month") : "";
      }
      case "TwitchFollow":
      case "TTFollow":
      case "YTSubscribe":
        return "drifted in";
      default:
        return "";
    }
  }

  function pickBurst(type, g) {
    const forced = g("burst").toLowerCase();
    if (["light", "medium", "heavy", "none"].includes(forced)) return forced;
    if (type === "TTGift") {
      const qty = num(g("repeatCount")) || 1;
      const c = num(g("coins")) || 0;
      if (c * qty >= 100) return "heavy";
    }
    return BURSTS[type] || "medium";
  }

  // ---------- Fit the 1520x855 stage into the browser source ----------
  function fit() {
    const s = Math.min(window.innerWidth / 1520, window.innerHeight / 855);
    document.documentElement.style.setProperty("--fit", s.toFixed(4));
  }
  window.addEventListener("resize", fit);
  fit();

  // ---------- Shrink long names to one line ----------
  function fitName(start) {
    const el = $("user");
    const max = 820 - 64 * 2; // card width minus padding
    el.classList.remove("wrap");
    let size = start;
    el.style.fontSize = size + "px";
    while (el.scrollWidth > max && size > 36) {
      size -= 2;
      el.style.fontSize = size + "px";
    }
    if (el.scrollWidth > max) el.classList.add("wrap");
  }

  // ---------- Play ----------
  let endTimer = null;

  function play() {
    const g = readParams();
    const type = g("alertType");
    const user = g("nickname") || g("userName");
    const anon = g("anonymous").toLowerCase() === "true";

    document.body.classList.remove("playing", "big");
    clearTimeout(endTimer);
    if (!type && !user) return; // idle: show nothing

    $("title").textContent = TITLES[type] || prettify(type);
    $("user").textContent = anon ? "anonymous" : (user || "someone");
    $("sub").textContent = buildSubtext(type, g);

    const plat = PLATFORMS[g("platform").toLowerCase()];
    $("via").classList.toggle("hidden", !plat);
    $("viaText").textContent = plat ? plat.text : "";
    $("viaIcon").setAttribute("src", plat ? plat.icon : "");

    const burst = pickBurst(type, g);
    if (burst === "heavy") document.body.classList.add("big");

    const v = $("burst");
    if (burst !== "none") {
      v.src = `media/burst-${burst}.webm`;
      v.currentTime = 0;
      v.play().catch(() => {});
    } else {
      v.removeAttribute("src");
    }

    fitName(burst === "heavy" ? 72 : 64);

    // restart CSS animations
    void document.body.offsetWidth;
    document.body.classList.add("playing");

    endTimer = setTimeout(() => document.body.classList.remove("playing", "big"), ALERT_MS);
  }

  // Run on load, and again if Streamer.bot only changes the #hash
  $("burst").addEventListener("ended", (e) => { e.target.style.visibility = "hidden"; });
  $("burst").addEventListener("play", (e) => { e.target.style.visibility = "visible"; });

  window.addEventListener("hashchange", play);
  play();
})();
