# cTrader Indicator: MultiPivotLevels

![Image](https://github.com/user-attachments/assets/62115a36-d182-4e4d-ba28-fbe46cc9294e)

Indikator pivot points untuk cTrader dengan multiple timeframe dan metode perhitungan.

## Fitur

- **5 Metode Pivot**: Classic, Fibonacci, Camarilla, Woodie, DeMark
- **Auto Timeframe**: Otomatis pilih timeframe pivot sesuai chart
- **Previous Levels**: Tampilkan level pivot periode sebelumnya (0-10)
- **Kustomisasi Visual**: Warna, ketebalan (1-5), style garis
- **Label Harga**: Dapat dinonaktifkan, dengan offset control (0-20 bar)
- **Toggle R3/S3**: Sembunyikan level ekstrem jika tidak diperlukan
- **Debug Mode**: Troubleshooting dengan log detail

## Instalasi

1. Buka cTrader → Automate → Indicators
2. Klik "New" → Pilih "Indicator"
3. Copy paste kode
4. Build (Ctrl+B)
5. Tambahkan ke chart

## Parameter Utama

| Parameter | Default | Deskripsi |
|-----------|---------|-----------|
| Use Auto Timeframe | true | Pilih timeframe pivot otomatis |
| Pivot Timeframe | Weekly | Timeframe manual (jika auto OFF) |
| Pivot Method | Classic | Metode perhitungan pivot |
| Show Previous Levels | 1 | Jumlah periode sebelumnya (0-10) |
| Show Price Labels | true | Tampilkan label harga |
| Label Offset | 5 | Jarak label dari kanan (bar) |
| Line Thickness | 1 | Ketebalan garis (1-5) |
| Show R3/S3 | true | Tampilkan level ekstrem |
| Extend Lines Right | true | Perpanjang garis ke kanan |

## Auto Timeframe Mapping

| Chart TF | Pivot TF |
|----------|----------|
| M1, M5 | H1 |
| M15, M30 | H4 |
| H1 | Daily |
| H4 | Weekly |
| Daily | Weekly |
| Weekly | Monthly |

## Metode Pivot

### Classic (Standard)
```
P = (H + L + C) / 3
R1 = 2P - L
S1 = 2P - H
R2 = P + (H - L)
S2 = P - (H - L)
R3 = H + 2(P - L)
S3 = L - 2(H - P)
```

### Fibonacci
Menggunakan rasio Fibonacci (38.2%, 61.8%, 100%) dari range H-L.

### Camarilla
Fokus pada level support/resistance dekat dengan close price.

### Woodie
Memberikan bobot lebih pada close price.

### DeMark
Menggunakan relasi open-close untuk menentukan bias trend.

## Tips Penggunaan

- **Scalping/Intraday**: Gunakan H1 atau H4 pivot
- **Swing Trading**: Gunakan Daily atau Weekly pivot
- **Range Trading**: Perhatikan S1-R1 sebagai area support/resistance
- **Breakout**: R2/S2 atau R3/S3 sebagai target

## Troubleshooting

Aktifkan "Show Debug Info" untuk melihat log di cTrader Journal:
- Initialization status
- Pivot detection
- Object removal count
- Data validation errors

## Changelog

### v2.0.0
- ✨ Tambah kustomisasi line thickness & style
- ✨ Tambah label offset control
- ✨ Tambah toggle R3/S3
- ⚡ Optimasi update mechanism (no recreate)
- 🐛 Fix konflik dengan indikator lain
- 🐛 Fix visual flickering
- 🔒 Tambah data validation & error handling

## License

MIT License - Bebas digunakan dan dimodifikasi.

## Support

Issues dan suggestions: [GitHub Issues](https://github.com/msyamsudin/cTrader-Indicator-Pivot-Level/issues)
