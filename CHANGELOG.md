# Changelog

All notable changes to CMS21 Quick Spray are listed here.
This project uses [Semantic Versioning](https://semver.org).

## [1.0.0] - 2026-11-18

First release.

### Added
- The rust penetrant spray on stuck bolts plays 5× faster.
- The speed-up applies only while the spray animation is playing. Normal game speed comes back as soon as it finishes.
- Its own config file, `UserData\QuickSpray.cfg`, with `Enabled` and `Multiplier` settings.
- Leaves the game alone while it's paused.
- If a spray is interrupted, normal speed is restored after 3 seconds.
- Plays nicely with QoLmod's debug menu game speed settings.

### Notes
- Built for MelonLoader v0.5.7 and CMS 2021 game version 1.0.40.

[1.0.0]: https://github.com/98freestyle/cms21-quick-spray/releases/tag/v1.0.0
