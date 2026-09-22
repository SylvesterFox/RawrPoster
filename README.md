# RawrPoster

Material Design 3 editor for Telegram posts with reusable templates, local image attachments, FuzzySearch source lookup, saved hashtags, spoiler support and SQLite persistence.

## Local configuration

Set these user-level environment variables before using external publishing features. Never commit their values:

- `RAWRPOSTER_TELEGRAM_BOT_TOKEN`
- `RAWRPOSTER_FUZZYSEARCH_API_KEY`

Templates, the SQLite database, copied media and diagnostic logs are stored under the platform local application-data folder in `RawrPoster`.
