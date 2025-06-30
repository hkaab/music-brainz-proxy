# Music Brainz Proxy

A lightweight proxy server for interfacing with the MusicBrainz web service, designed to mitigate IP rate-limiting issues.

## Features

* **Bypass IP Rate-Limiting**: Access MusicBrainz data without encountering IP-based restrictions.
* **Simple Setup**: Easy to deploy using Docker or Python virtual environments.
* **Lightweight**: Minimal dependencies and no database required.

## Requirements

* **Python**: Version 3.6 or higher.
* **Docker** (optional): For containerized deployment.
* **Redis** (optional): For caching (configure via environment variables).

## Installation

### Using Docker

```bash
git clone https://github.com/hkaab/music-brainz-proxy.git
cd music-brainz-proxy
docker build -t music-brainz-proxy .
docker run -p 9999:8000 music-brainz-proxy
```

Access the proxy at `http://localhost:9999`.

### Using Python Virtual Environment

```bash
git clone https://github.com/hkaab/music-brainz-proxy.git
cd music-brainz-proxy
python3 -m venv venv
source venv/bin/activate
pip install -r requirements.txt
python app.py
```

## Usage

Send requests to the proxy to fetch MusicBrainz data:

```bash
curl "http://localhost:9999/albums?mbid=<MUSICBRAINZ_ID>"
```

Replace `<MUSICBRAINZ_ID>` with the desired MusicBrainz entity ID.

## Contributing

Contributions are welcome! Please fork the repository, create a new branch, and submit a pull request.

## License

This project is licensed under the MIT License.

---

Feel free to customize this template further to match your project's specifics.

