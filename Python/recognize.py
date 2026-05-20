import sys
import torch
import librosa
import warnings
from transformers import WhisperProcessor, WhisperForConditionalGeneration
import pykakasi

warnings.filterwarnings("ignore")

model_name = "openai/whisper-small"

processor = WhisperProcessor.from_pretrained(model_name)
model = WhisperForConditionalGeneration.from_pretrained(model_name)

device = "cuda" if torch.cuda.is_available() else "cpu"
model = model.to(device)

kakasi = pykakasi.kakasi()

while True:
    try:
        wav = input().strip()

        if wav == "":
            print("")
            sys.stdout.flush()
            continue

        speech, sr = librosa.load(wav, sr=16000)

        if len(speech) == 0:
            print("")
            sys.stdout.flush()
            continue

        inputs = processor(
            speech,
            sampling_rate=16000,
            return_tensors="pt"
        ).input_features.to(device)

        with torch.no_grad():
            predicted_ids = model.generate(inputs)

        text = processor.batch_decode(predicted_ids, skip_special_tokens=True)[0]

        # ここから：ひらがなだけ抽出
        hira = ""
        for item in kakasi.convert(text):
            h = item["hira"]
            for c in h:
                if "ぁ" <= c <= "ん":
                    hira += c

        print(hira)
        sys.stdout.flush()

    except Exception as e:
        print("")
        sys.stdout.flush()
