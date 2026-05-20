import pyttsx3
import sys

engine = pyttsx3.init()#初期化
engine.setProperty('rate', 150)
engine.setProperty('volume', 1.0)

# 英語音声優先（あれば）
voices = engine.getProperty('voices')
for v in voices:
    if "en" in v.id.lower():
        engine.setProperty('voice', v.id)
        break

while True:
    try:
        text = input().strip()

        if text == "":
            continue

        print("SPEAK:", text)
        sys.stdout.flush()

        engine.say(text)
        engine.runAndWait()

        # 応答は一応残す（Unityは読まない）
        print("OK")
        sys.stdout.flush()

    except Exception as e:
        print("ERR:", e)
        sys.stdout.flush()