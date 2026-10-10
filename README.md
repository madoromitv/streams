# streams

Overlay pages for **MadoromiTV** (微睡み), hosted on GitHub Pages and loaded in OBS as browser sources.

Site root: `https://madoromitv.github.io/streams/`

## Contents

| Path | What it is | Used in OBS as |
|---|---|---|
| `index.html` | Test page with a preview link for every alert type | — |
| `theme/madoromi-theme.css` | Shared palette, glass, fonts. Every overlay imports it. | — |
| `alerts/alert.html` (+ `alert.css`, `alert.js`) | Alert card. Streamer.bot sets the `#hash`. | Browser source "Alerts Screen", 1920 × 1080, Control audio via OBS on |
| `alerts/media/burst-*.webm` | Light / medium / heavy static bursts (transparent) | played by the alert page |
| `alerts/media/sfx-*.ogg` | Light / medium / heavy alert sounds | played by the alert page |
| `chat/chat.css` | Social Stream Ninja theme (16 px glass cards) | add `&css=https://madoromitv.github.io/streams/chat/chat.css` to the SSN dock URL; 400 × 615 |
| `goals/twitch-goal-*.css` | Twitch Creator Goal widgets (subs, bits, follows) | paste into the source's Custom CSS; 380 × 96 |
| `goals/tiktok-goal-*.css` | TikFinity goal widgets (likes, follows) | paste into Custom CSS; 504 × 96 |
| `goals/tiktok-likeathon.css` | TikFinity top likers, top 6 | paste into Custom CSS; 360 × 420 |
| `now/now.html` | "Now dreaming" game card; reads the live Twitch category every 60 s | Browser source 400 × 100; `#game=Name` overrides |
| `tools/title.html` | Clip title maker; downloads a 1080 × 1920 title PNG | open in a browser, not OBS |
| `assets/icons/`, `assets/logo-gradient.svg` | Platform icons and logo | — |

## Alert parameters (URL #hash)

`alertType`, `platform`, `userName`, `nickname`, `bits`, `gifts`, `tier`, `recipient`, `reward`, `cost`, `anonymous`, `giftName`, `coins`, `repeatCount`, `subMonth`, `viewers`.

Optional: `burst=light|medium|heavy|none` and `sound=light|medium|heavy|none` override the automatic pick. Page option `alert.html?silent` mutes a second copy.

Alert types: `TwitchFollow`, `TwitchSub`, `TwitchCheer`, `TwitchGiftSub`, `TwitchGiftBomb`, `TwitchRewardRedemption`, `TwitchRaid`, `TTFollow`, `TTGift`, `TTSub` (plus `YTSubscribe`, `YTMember`, `YTSuperChat`).

Each alert runs 10 seconds; Streamer.bot waits 10.5 s per alert in a blocking queue.

## Updating

Upload changed files through github.com → Add file → Upload files (drag a whole folder to keep its path), then wait about a minute for Pages to redeploy and refresh the OBS source.
