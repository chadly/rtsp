# RTSP Camera Streaming to Chromecast

Stream an RTSP-enabled camera to Chromecast with Google Assistant integration.

> Hey Google, show the _Front Yard_ camera on the _Kitchen TV_

where _Front Yard_ is a camera with an RTSP stream (that doesn't natively support Chromecast or Google Assistant) and _Kitchen TV_ is a chromecast.

This is a work-in-progress.

-----------------------------------

To setup the streaming app as a service on a debian dist, save the following as `/etc/systemd/system/stream-cams.service`:

```ini
[Unit]
Description=Stream RTSP Cameras to Chromecast

[Service]
ExecStart=/opt/cams/stream/stream-cams
# Required on some systems
WorkingDirectory=/opt/cams/stream
Restart=always
# Restart service after 10 seconds if node service crashes
RestartSec=10
# Output to syslog
StandardOutput=syslog
StandardError=syslog
SyslogIdentifier=stream-cams

[Install]
WantedBy=multi-user.target
```

Then, to run the daemon on startup:

```
sudo systemctl enable stream-cams.service
```