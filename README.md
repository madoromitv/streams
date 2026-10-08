# streams

Overlay pages for **MadoromiTV** (微睡み), hosted on GitHub Pages and loaded in OBS as browser sources.

Site root: `https://madoromitv.github.io/streams/`

Addresses used in OBS / Streamer.bot:
- Alert: `https://madoromitv.github.io/streams/alerts/alert.html#alertType=...`
- Chat (horizontal): add `&css=https://madoromitv.github.io/streams/chat/chat.css` to the Social Stream Ninja URL
- Chat (vertical): add `&css=https://madoromitv.github.io/streams/chat/chat-v.css`

## Contents

| Path | What it is |
|---|---|
| `index.html` | Test page with a preview link for every alert type |
| `theme/madoromi-theme.css` | Shared palette, glass panel, fonts. Edit colours here once. |
| `alerts/alert.html` (+ `alert.css`, `alert.js`) | Streamer.bot alert page. Same URL parameters as the old Ashen alert. |
| `alerts/media/burst-*.webm` | Light / medium / heavy static bursts (transparent) |
| `chat/chat.css` | Social Stream Ninja theme, horizontal (400 × 615) |
| `chat/chat-v.css` | Same theme, bigger text, vertical (1032 × 440) |
| `goals/twitch-goal-*.css` | Twitch Creator Goal widgets (subs, bits, follows). Paste into OBS Custom CSS, 380 × 96 |
| `goals/tiktok-goal-*.css` | TikFinity goal widgets (follows, likes). Paste into OBS Custom CSS, 504 × 96 |
| `goals/tiktok-likeathon.css` | TikFinity Likeathon (top likers). Paste into OBS Custom CSS, 360 × 420 |
| `assets/icons/`, `assets/logo-gradient.svg` | Platform icons and logo |

## Alert parameters (URL #hash)

Unchanged from Ashen: `alertType`, `platform`, `userName`, `nickname`, `bits`, `gifts`, `tier`, `recipient`, `reward`, `cost`, `anonymous`, `giftName`, `coins`, `repeatCount`, `subMonth`.

New, optional:
- `viewers`: for `alertType=TwitchRaid`
- `burst=light|medium|heavy|none`: overrides the automatic burst

Alert types: `TwitchFollow`, `TwitchSub`, `TwitchCheer`, `TwitchGiftSub`, `TwitchGiftBomb`, `TwitchRewardRedemption`, `TwitchRaid`, `TTFollow`, `TTGift`, `TTSub` (plus `YTSubscribe`, `YTMember`, `YTSuperChat`).

Runtime is 10 seconds, the same as before, so existing Streamer.bot waits still fit.
