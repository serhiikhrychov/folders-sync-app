# 📂 folders-sync-app

A simple program to synchronize your **Source** and **Replica** folders.

---

## 🚀 How It Works

- The **Source** folder is your main folder, containing the original files.
- The **Replica** folder is a synchronized copy of the Source folder.
- Changes from the Source are automatically synced to the Replica at a defined interval.

---

## 🛠 Usage

Before starting:
- Create your **Source** and **Replica** folders manually.

To run the app from the command line, use:

```bash
.\folders_sync.exe "D:\Source" "D:\Replica" 30 "D:\Logs\sync_log.txt"
```

Where:

"D:\Source" — path to your Source folder.

"D:\Replica" — path to your Replica folder.

30 — synchronization interval in seconds.

"D:\Logs\sync_log.txt" — path to the log file where synchronization events are recorded.
