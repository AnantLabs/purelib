﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PureLib.Common {
    public class DownloadItem {
        private DownloadItemState _state;

        public string Url { get; private set; }
        public string Referer { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public CookieContainer Cookies { get; private set; }
        public string FilePath { get; private set; }
        public DownloadItemState State {
            get {
                return _state;
            }
            private set {
                if (_state != value)
                    OnStateChanged(this, _state, value);
                _state = value;
            }
        }
        public bool IsReady {
            get {
                return State == DownloadItemState.Queued;
            }
        }
        public bool IsStopped {
            get {
                return State == DownloadItemState.Stopped;
            }
        }
        public string FileName {
            get {
                return Path.GetFileName(FilePath);
            }
        }

        public virtual long TotalBytes { get; set; }
        public virtual long ReceivedBytes { get; set; }
        public virtual int Percentage { get; set; }

        public event DownloadItemStateChangedEventHandler StateChanged;

        public DownloadItem(string url, string referer, CookieContainer cookies, string path, DownloadItemState state = DownloadItemState.Queued) {
            _state = state;

            Url = url;
            Referer = referer;
            Cookies = cookies;
            FilePath = path;
        }

        public void Start() {
            State = DownloadItemState.Queued;
        }

        public void Download() {
            State = DownloadItemState.Downloading;
        }

        public void Stop() {
            State = DownloadItemState.Stopped;
        }

        public void Complete() {
            State = DownloadItemState.Completed;
            if (TotalBytes == 0)
                TotalBytes = new FileInfo(FilePath).Length;
            ReceivedBytes = TotalBytes;
            Percentage = 100;
        }

        protected virtual void OnStateChanged(DownloadItem item, DownloadItemState oldState, DownloadItemState newState) {
            if (StateChanged != null)
                StateChanged(this, new DownloadItemStateChangedEventArgs(item, oldState, newState));
        }
    }

    public class DownloadItemStateChangedEventArgs : EventArgs {
        public DownloadItem DownloadItem { get; private set; }
        public DownloadItemState OldState { get; private set; }
        public DownloadItemState NewState { get; private set; }

        public DownloadItemStateChangedEventArgs(DownloadItem item, DownloadItemState oldState, DownloadItemState newState) {
            DownloadItem = item;
            OldState = oldState;
            NewState = newState;
        }
    }
    public delegate void DownloadItemStateChangedEventHandler(object sender, DownloadItemStateChangedEventArgs e);
}
