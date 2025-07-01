# MusicBrainz API version 2.0 

![workflow](https://github.com/hkaabasl/MusicBrainzProxy/actions/workflows/ci.yml/badge.svg)
![workflow](https://github.com/hkaabasl/MusicBrainzProxy/actions/workflows/ut.yml/badge.svg)
![workflow](https://github.com/hkaabasl/MusicBrainzProxy/actions/workflows/it.yml/badge.svg)

Simple and Partial Implementation of the MusicBrainz API version 2.0 (requires .NET9.0).

**GetArtistByIdAsync** : return Artist object including Artist Releases

**GetArtistReleaseAsync** : return release collection of Artist

**QueryArtistAsync**: return Artist collection or single Artist object with the releases

**Rate Limiting** added to protect the service from bots , misuse and able to apply consumer plan

**Retry** feature added to make the service more resilient  
